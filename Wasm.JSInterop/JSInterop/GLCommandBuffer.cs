using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;

namespace nkast.Wasm.JSInterop
{
    /// <summary>
    /// Batches void JSInvoke calls into a SharedArrayBuffer command buffer,
    /// reducing per-frame proxy overhead from O(N) to O(1) in multi-threaded WASM.
    /// Commands are written as int32 sequences: [argsSize, fid, uid, arg0..argN].
    /// One flush call per frame sends all commands to the main thread for execution.
    /// Thread-safe: only the render thread (the thread that called Initialize) may
    /// read/write the buffer. All other threads bypass the buffer and call JSInvoke directly.
    /// </summary>
    public static partial class GLCommandBuffer
    {
        public static bool Enabled;

        /// <summary>
        /// The managed thread ID of the render thread. Only this thread may use the buffer.
        /// GC finalizers, background tasks, etc. must bypass the buffer to avoid corruption.
        /// </summary>
        public static int RenderThreadId;

        /// <summary>True if the buffer is enabled AND the current thread is the render thread.</summary>
        public static bool ShouldBuffer => Enabled && Environment.CurrentManagedThreadId == RenderThreadId;

        private static unsafe int* _buffer;
        private static int _bufferAddr; // WASM byte address
        private static int _writePos;   // current write position (int32 index)
        private const int CAPACITY = 65536; // 256KB (65536 × 4 bytes)

        public static unsafe void Initialize()
        {
            var ptr = Marshal.AllocHGlobal(CAPACITY * sizeof(int));
            _buffer = (int*)ptr;
            _bufferAddr = (int)ptr;
            _writePos = 0;
            RenderThreadId = Environment.CurrentManagedThreadId;
            Enabled = true;
            Console.WriteLine($"WASM: [GLCommandBuffer] Initialized ({CAPACITY * 4 / 1024}KB at 0x{_bufferAddr:X}, renderThread={RenderThreadId})");
        }

        /// <summary>Write a zero-arg void call: JSInvoke1Void(fid, uid)</summary>
        public static unsafe void WriteVoid0(int fid, int uid)
        {
            if (_writePos + 3 > CAPACITY) Flush();
            _buffer[_writePos++] = 0; // argsSize = 0
            _buffer[_writePos++] = fid;
            _buffer[_writePos++] = uid;
        }

        /// <summary>Write a multi-arg void call: JSInvoke2Void(fid, uid, argsPtr)</summary>
        public static unsafe void WriteVoid(int fid, int uid, int argsPtr, int argsSize)
        {
            if (_writePos + 3 + argsSize > CAPACITY) Flush();
            _buffer[_writePos++] = argsSize;
            _buffer[_writePos++] = fid;
            _buffer[_writePos++] = uid;
            int* src = (int*)argsPtr;
            for (int i = 0; i < argsSize; i++)
                _buffer[_writePos++] = src[i];
        }

        /// <summary>
        /// Write a void call with 2 scalar int args + inline data copied from a pointer.
        /// Format: [2+dataLen, fid, uid, arg0, arg1, data0..dataN]
        /// The data is copied into the buffer so the source pointer can be freed immediately.
        /// Used for uniform uploads where the data is small (matrices, vectors).
        /// </summary>
        public static unsafe void WriteVoidInline2(int fid, int uid, int arg0, int arg1, int* data, int dataLen)
        {
            int totalArgs = 2 + dataLen;
            int totalSize = 3 + totalArgs;
            if (_writePos + totalSize > CAPACITY) Flush();
            _buffer[_writePos++] = totalArgs;
            _buffer[_writePos++] = fid;
            _buffer[_writePos++] = uid;
            _buffer[_writePos++] = arg0;
            _buffer[_writePos++] = arg1;
            for (int i = 0; i < dataLen; i++)
                _buffer[_writePos++] = data[i];
        }

        /// <summary>
        /// Write a void call with 3 scalar int args + inline data copied from a pointer.
        /// Used for buffer data uploads (type, usage/offset, byteLen, data).
        /// </summary>
        public static unsafe void WriteVoidInline3(int fid, int uid, int arg0, int arg1, int arg2, int* data, int dataLen)
        {
            int totalArgs = 3 + dataLen;
            int totalSize = 3 + totalArgs;
            if (_writePos + totalSize > CAPACITY) Flush();
            _buffer[_writePos++] = totalArgs;
            _buffer[_writePos++] = fid;
            _buffer[_writePos++] = uid;
            _buffer[_writePos++] = arg0;
            _buffer[_writePos++] = arg1;
            _buffer[_writePos++] = arg2;
            for (int i = 0; i < dataLen; i++)
                _buffer[_writePos++] = data[i];
        }

        /// <summary>
        /// Maximum inline data size in int32s. Commands larger than this
        /// should fall back to InvokeDirect to avoid filling the buffer.
        /// </summary>
        public const int MAX_INLINE_INTS = 8192; // 32KB

        /// <summary>Send all buffered commands to the main thread for execution.</summary>
        public static unsafe void Flush()
        {
            if (_writePos == 0) return;
            NativeFlush(_bufferAddr, _writePos);
            _writePos = 0;
        }

        // Returns int (cmdCount) to force synchronous proxy dispatch.
        // Void JSImport in the deputy model may be fire-and-forget,
        // which would let C# overwrite the buffer before JS finishes reading it.
        [JSImport("globalThis.nkGLCommandBuffer.flush")]
        private static partial int NativeFlush(int bufferPtr, int writePos);
    }
}
