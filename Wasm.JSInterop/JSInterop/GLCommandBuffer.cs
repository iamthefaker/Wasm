using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;

namespace nkast.Wasm.JSInterop
{
    /// <summary>
    /// Batches void JSInvoke calls into a SharedArrayBuffer command buffer,
    /// reducing per-frame proxy overhead from O(N) to O(1) in multi-threaded WASM.
    /// Commands are written as int32 sequences: [argsSize, fid, uid, arg0..argN].
    /// One flush call per frame sends all commands to the main thread for execution.
    /// </summary>
    public static partial class GLCommandBuffer
    {
        public static bool Enabled;

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
            Enabled = true;
            Console.WriteLine($"WASM: [GLCommandBuffer] Initialized ({CAPACITY * 4 / 1024}KB at 0x{_bufferAddr:X})");
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
