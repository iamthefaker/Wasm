using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;

namespace nkast.Wasm.JSInterop
{
    public partial class JSObject
    {
        private static Dictionary<string, int> _fidMap = new Dictionary<string, int>();

        // --- JSImport native entry points (renamed to *_Impl for command buffer wrapping) ---

        [JSImport("globalThis.nkJSObject.JSRegisterFunction")]
        private static partial int JSRegisterFunction_Impl(int pidentifier, int identifierLength);

        [JSImport("globalThis.nkJSObject.JSInvoke0Int")]
        private static partial int JSInvoke0Int_Impl(int fid);

        [JSImport("globalThis.nkJSObject.JSInvoke1Void")]
        private static partial void JSInvoke1Void(int fid, int uid);

        [JSImport("globalThis.nkJSObject.JSInvoke1Bool")]
        private static partial bool JSInvoke1Bool_Impl(int fid, int uid);

        [JSImport("globalThis.nkJSObject.JSInvoke1Int")]
        private static partial int JSInvoke1Int_Impl(int fid, int uid);

        [JSImport("globalThis.nkJSObject.JSInvoke1Float")]
        private static partial float JSInvoke1Float_Impl(int fid, int uid);

        [JSImport("globalThis.nkJSObject.JSInvoke1Double")]
        private static partial double JSInvoke1Double_Impl(int fid, int uid);

        [JSImport("globalThis.nkJSObject.JSInvoke1String")]
        private static partial string JSInvoke1String_Impl(int fid, int uid);

        [JSImport("globalThis.nkJSObject.JSInvoke2Void")]
        private static partial void JSInvoke2Void(int fid, int uid, int d);

        [JSImport("globalThis.nkJSObject.JSInvoke2Bool")]
        private static partial bool JSInvoke2Bool_Impl(int fid, int uid, int d);

        [JSImport("globalThis.nkJSObject.JSInvoke2Int")]
        private static partial int JSInvoke2Int_Impl(int fid, int uid, int d);

        [JSImport("globalThis.nkJSObject.JSInvoke2Float")]
        private static partial float JSInvoke2Float_Impl(int fid, int uid, int d);

        [JSImport("globalThis.nkJSObject.JSInvoke2String")]
        private static partial string JSInvoke2String_Impl(int fid, int uid, int d);

        // --- Wrappers: return-type methods flush the command buffer before proxying ---

        private static int JSRegisterFunction(int pidentifier, int identifierLength)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            return JSRegisterFunction_Impl(pidentifier, identifierLength);
        }

        private static int JSInvoke0Int(int fid)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            return JSInvoke0Int_Impl(fid);
        }

        private static bool JSInvoke1Bool(int fid, int uid)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            return JSInvoke1Bool_Impl(fid, uid);
        }

        private static int JSInvoke1Int(int fid, int uid)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            return JSInvoke1Int_Impl(fid, uid);
        }

        private static float JSInvoke1Float(int fid, int uid)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            return JSInvoke1Float_Impl(fid, uid);
        }

        private static double JSInvoke1Double(int fid, int uid)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            return JSInvoke1Double_Impl(fid, uid);
        }

        private static string JSInvoke1String(int fid, int uid)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            return JSInvoke1String_Impl(fid, uid);
        }

        private static bool JSInvoke2Bool(int fid, int uid, int d)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            return JSInvoke2Bool_Impl(fid, uid, d);
        }

        private static int JSInvoke2Int(int fid, int uid, int d)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            return JSInvoke2Int_Impl(fid, uid, d);
        }

        private static float JSInvoke2Float(int fid, int uid, int d)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            return JSInvoke2Float_Impl(fid, uid, d);
        }

        private static string JSInvoke2String(int fid, int uid, int d)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            return JSInvoke2String_Impl(fid, uid, d);
        }


        private static unsafe int RegisterFunction(string identifier)
        {
            if (_fidMap.TryGetValue(identifier, out int fid))
                return fid;

            fixed (char* pidentifier = identifier)
            {
                fid = JSRegisterFunction((int)pidentifier, identifier.Length);
            }

            _fidMap.Add(identifier, fid);
            return fid;
        }

        protected static int StaticInvokeRetInt(string identifier)
        {
            int fid = RegisterFunction(identifier);
            return JSInvoke0Int(fid);
        }

        protected static int StaticInvokeRetInt(string identifier, int uid)
        {
            int fid = RegisterFunction(identifier);
            return JSInvoke1Int(fid, uid);
        }

        protected static int StaticInvokeRetInt(string identifier, int arg1, int arg2)
        {
            int fid = RegisterFunction(identifier);
            return JSInvoke2Int(fid, arg1, arg2);
        }

        protected void Invoke(string identifier)
        {
            int fid = RegisterFunction(identifier);
            if (GLCommandBuffer.ShouldBuffer) { GLCommandBuffer.WriteVoid0(fid, Uid); return; }
            JSInvoke1Void(fid, Uid);
        }

        protected bool InvokeRetBool(string identifier)
        {
            int fid = RegisterFunction(identifier);
            return JSInvoke1Bool(fid, Uid);
        }

        protected int InvokeRetInt(string identifier)
        {
            int fid = RegisterFunction(identifier);
            return JSInvoke1Int(fid, Uid);
        }

        protected float InvokeRetFloat(string identifier)
        {
            int fid = RegisterFunction(identifier);
            return JSInvoke1Float(fid, Uid);
        }

        protected double InvokeRetDouble(string identifier)
        {
            int fid = RegisterFunction(identifier);
            return JSInvoke1Double(fid, Uid);
        }

        protected string InvokeRetString(string identifier)
        {
            int fid = RegisterFunction(identifier);
            return JSInvoke1String(fid, Uid);
        }

        protected unsafe void Invoke<T1>(string identifier, T1 arg1)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, Net7Padding);
            if (GLCommandBuffer.ShouldBuffer) { GLCommandBuffer.WriteVoid(fid, Uid, (int)&args, 2); return; }
            JSInvoke2Void(fid, Uid, (int)&args);
        }

        const int Net7Padding = 0;
        protected unsafe bool InvokeRetBool<T1>(string identifier, T1 arg1)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, Net7Padding);
            return JSInvoke2Bool(fid, Uid, (int)&args);
        }

        protected unsafe int InvokeRetInt<T1>(string identifier, T1 arg1)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, Net7Padding);
            return JSInvoke2Int(fid, Uid, (int)&args);
        }

        protected unsafe float InvokeRetFloat<T1>(string identifier, T1 arg1)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, Net7Padding);
            return JSInvoke2Float(fid, Uid, (int)&args);
        }

        protected unsafe string InvokeRetString<T1>(string identifier, T1 arg1)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, Net7Padding);
            return JSInvoke2String(fid, Uid, (int)&args);
        }

        protected unsafe void Invoke<T1, T2>(string identifier, T1 arg1, T2 arg2)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2);
            if (GLCommandBuffer.ShouldBuffer) { GLCommandBuffer.WriteVoid(fid, Uid, (int)&args, 2); return; }
            JSInvoke2Void(fid, Uid, (int)&args);
        }

        protected unsafe bool InvokeRetBool<T1, T2>(string identifier, T1 arg1, T2 arg2)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2);
            return JSInvoke2Bool(fid, Uid, (int)&args);
        }

        protected unsafe int InvokeRetInt<T1, T2>(string identifier, T1 arg1, T2 arg2)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2);
            return JSInvoke2Int(fid, Uid, (int)&args);
        }

        protected unsafe float InvokeRetFloat<T1, T2>(string identifier, T1 arg1, T2 arg2)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2);
            return JSInvoke2Float(fid, Uid, (int)&args);
        }

        protected unsafe string InvokeRetString<T1, T2>(string identifier, T1 arg1, T2 arg2)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2);
            return JSInvoke2String(fid, Uid, (int)&args);
        }

        protected unsafe void Invoke<T1, T2, T3>(string identifier, T1 arg1, T2 arg2, T3 arg3)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3);
            if (GLCommandBuffer.ShouldBuffer) { GLCommandBuffer.WriteVoid(fid, Uid, (int)&args, 3); return; }
            JSInvoke2Void(fid, Uid, (int)&args);
        }

        protected unsafe bool InvokeRetBool<T1, T2, T3>(string identifier, T1 arg1, T2 arg2, T3 arg3)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3);
            return JSInvoke2Bool(fid, Uid, (int)&args);
        }

        protected unsafe int InvokeRetInt<T1, T2, T3>(string identifier, T1 arg1, T2 arg2, T3 arg3)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3);
            return JSInvoke2Int(fid, Uid, (int)&args);
        }

        protected unsafe float InvokeRetFloat<T1, T2, T3>(string identifier, T1 arg1, T2 arg2, T3 arg3)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3);
            return JSInvoke2Float(fid, Uid, (int)&args);
        }

        protected unsafe string InvokeRetString<T1, T2, T3>(string identifier, T1 arg1, T2 arg2, T3 arg3)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3);
            return JSInvoke2String(fid, Uid, (int)&args);
        }

        protected unsafe void Invoke<T1, T2, T3, T4>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4);
            if (GLCommandBuffer.ShouldBuffer) { GLCommandBuffer.WriteVoid(fid, Uid, (int)&args, 4); return; }
            JSInvoke2Void(fid, Uid, (int)&args);
        }

        protected unsafe bool InvokeRetBool<T1, T2, T3, T4>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4);
            return JSInvoke2Bool(fid, Uid, (int)&args);
        }

        protected unsafe int InvokeRetInt<T1, T2, T3, T4>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4);
            return JSInvoke2Int(fid, Uid, (int)&args);
        }

        protected unsafe float InvokeRetFloat<T1, T2, T3, T4>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4);
            return JSInvoke2Float(fid, Uid, (int)&args);
        }

        protected unsafe string InvokeRetString<T1, T2, T3, T4>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4);
            return JSInvoke2String(fid, Uid, (int)&args);
        }

        protected unsafe void Invoke<T1, T2, T3, T4, T5>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5);
            if (GLCommandBuffer.ShouldBuffer) { GLCommandBuffer.WriteVoid(fid, Uid, (int)&args, 5); return; }
            JSInvoke2Void(fid, Uid, (int)&args);
        }

        protected unsafe bool InvokeRetBool<T1, T2, T3, T4, T5>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5);
            return JSInvoke2Bool(fid, Uid, (int)&args);
        }

        protected unsafe int InvokeRetInt<T1, T2, T3, T4, T5>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5);
            return JSInvoke2Int(fid, Uid, (int)&args);
        }

        protected unsafe float InvokeRetFloat<T1, T2, T3, T4, T5>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5);
            return JSInvoke2Float(fid, Uid, (int)&args);
        }

        protected unsafe string InvokeRetString<T1, T2, T3, T4, T5>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5);
            return JSInvoke2String(fid, Uid, (int)&args);
        }

        protected unsafe void Invoke<T1, T2, T3, T4, T5, T6>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5, arg6);
            if (GLCommandBuffer.ShouldBuffer) { GLCommandBuffer.WriteVoid(fid, Uid, (int)&args, 6); return; }
            JSInvoke2Void(fid, Uid, (int)&args);
        }

        protected unsafe bool InvokeRetBool<T1, T2, T3, T4, T5, T6>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5, arg6);
            return JSInvoke2Bool(fid, Uid, (int)&args);
        }

        protected unsafe int InvokeRetInt<T1, T2, T3, T4, T5, T6>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5, arg6);
            return JSInvoke2Int(fid, Uid, (int)&args);
        }

        protected unsafe float InvokeRetFloat<T1, T2, T3, T4, T5, T6>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5, arg6);
            return JSInvoke2Float(fid, Uid, (int)&args);
        }

        protected unsafe string InvokeRetString<T1, T2, T3, T4, T5, T6>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5, arg6);
            return JSInvoke2String(fid, Uid, (int)&args);
        }

        protected unsafe void Invoke<T1, T2, T3, T4, T5, T6, T7>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            if (GLCommandBuffer.ShouldBuffer) { GLCommandBuffer.WriteVoid(fid, Uid, (int)&args, 7); return; }
            JSInvoke2Void(fid, Uid, (int)&args);
        }

        protected unsafe bool InvokeRetBool<T1, T2, T3, T4, T5, T6, T7>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return JSInvoke2Bool(fid, Uid, (int)&args);
        }

        protected unsafe int InvokeRetInt<T1, T2, T3, T4, T5, T6, T7>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return JSInvoke2Int(fid, Uid, (int)&args);
        }

        protected unsafe float InvokeRetFloat<T1, T2, T3, T4, T5, T6, T7>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return JSInvoke2Float(fid, Uid, (int)&args);
        }

        protected unsafe string InvokeRetString<T1, T2, T3, T4, T5, T6, T7>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return JSInvoke2String(fid, Uid, (int)&args);
        }

        protected unsafe void Invoke<T1, T2, T3, T4, T5, T6, T7, T8>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            int fid = RegisterFunction(identifier);
            var args = new FixedStruct8<T1, T2, T3, T4, T5, T6, T7, T8>(arg1,arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            if (GLCommandBuffer.ShouldBuffer) { GLCommandBuffer.WriteVoid(fid, Uid, (int)&args, 8); return; }
            JSInvoke2Void(fid, Uid, (int)&args);
        }

        protected unsafe bool InvokeRetBool<T1, T2, T3, T4, T5, T6, T7, T8>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            int fid = RegisterFunction(identifier);
            var args = new FixedStruct8<T1, T2, T3, T4, T5, T6, T7, T8>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return JSInvoke2Bool(fid, Uid, (int)&args);
        }

        protected unsafe int InvokeRetInt<T1, T2, T3, T4, T5, T6, T7, T8>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            int fid = RegisterFunction(identifier);
            var args = new FixedStruct8<T1, T2, T3, T4, T5, T6, T7, T8>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return JSInvoke2Int(fid, Uid, (int)&args);
        }

        protected unsafe float InvokeRetFloat<T1, T2, T3, T4, T5, T6, T7, T8>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            int fid = RegisterFunction(identifier);
            var args = new FixedStruct8<T1, T2, T3, T4, T5, T6, T7, T8>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return JSInvoke2Float(fid, Uid, (int)&args);
        }

        protected unsafe string InvokeRetString<T1, T2, T3, T4, T5, T6, T7, T8>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            int fid = RegisterFunction(identifier);
            var args = new FixedStruct8<T1, T2, T3, T4, T5, T6, T7, T8>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return JSInvoke2String(fid, Uid, (int)&args);
        }

        protected unsafe void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            int fid = RegisterFunction(identifier);
            var args = new FixedStruct9<T1, T2, T3, T4, T5, T6, T7, T8, T9>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            if (GLCommandBuffer.ShouldBuffer) { GLCommandBuffer.WriteVoid(fid, Uid, (int)&args, 9); return; }
            JSInvoke2Void(fid, Uid, (int)&args);
        }

        protected unsafe bool InvokeRetBool<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            int fid = RegisterFunction(identifier);
            var args = new FixedStruct9<T1, T2, T3, T4, T5, T6, T7, T8, T9>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return JSInvoke2Bool(fid, Uid, (int)&args);
        }

        protected unsafe int InvokeRetInt<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            int fid = RegisterFunction(identifier);
            var args = new FixedStruct9<T1, T2, T3, T4, T5, T6, T7, T8, T9>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return JSInvoke2Int(fid, Uid, (int)&args);
        }

        protected unsafe float InvokeRetFloat<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            int fid = RegisterFunction(identifier);
            var args = new FixedStruct9<T1, T2, T3, T4, T5, T6, T7, T8, T9>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return JSInvoke2Float(fid, Uid, (int)&args);
        }

        protected unsafe string InvokeRetString<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            int fid = RegisterFunction(identifier);
            var args = new FixedStruct9<T1, T2, T3, T4, T5, T6, T7, T8, T9>(arg1,arg2,arg3,arg4,arg5,arg6,arg7,arg8,arg9);
            return JSInvoke2String(fid, Uid, (int)&args);
        }

        protected unsafe void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, TA argA)
        {
            int fid = RegisterFunction(identifier);
            var args = new FixedStructA<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, argA);
            if (GLCommandBuffer.ShouldBuffer) { GLCommandBuffer.WriteVoid(fid, Uid, (int)&args, 10); return; }
            JSInvoke2Void(fid, Uid, (int)&args);
        }

        protected unsafe bool InvokeRetBool<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, TA argA)
        {
            int fid = RegisterFunction(identifier);
            var args = new FixedStructA<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, argA);
            return JSInvoke2Bool(fid, Uid, (int)&args);
        }

        protected unsafe int InvokeRetInt<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, TA argA)
        {
            int fid = RegisterFunction(identifier);
            var args = new FixedStructA<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, argA);
            return JSInvoke2Int(fid, Uid, (int)&args);
        }

        protected unsafe float InvokeRetFloat<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, TA argA)
        {
            int fid = RegisterFunction(identifier);
            var args = new FixedStructA<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, argA);
            return JSInvoke2Float(fid, Uid, (int)&args);
        }

        protected unsafe string InvokeRetString<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, TA argA)
        {
            int fid = RegisterFunction(identifier);
            var args = new FixedStructA<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, argA);
            return JSInvoke2String(fid, Uid, (int)&args);
        }
    

        // --- InvokeInline: buffer the call with inline data payload ---
        // Used for uniform/buffer uploads where we copy data into the command buffer
        // instead of passing a managed array reference.

        /// <summary>
        /// Buffer a void call with 2 scalar args + inline data. Only valid when ShouldBuffer is true.
        /// </summary>
        protected unsafe void InvokeInline2(string identifier, int arg0, int arg1, int* data, int dataLen)
        {
            int fid = RegisterFunction(identifier);
            GLCommandBuffer.WriteVoidInline2(fid, Uid, arg0, arg1, data, dataLen);
        }

        /// <summary>
        /// Buffer a void call with 3 scalar args + inline data. Only valid when ShouldBuffer is true.
        /// </summary>
        protected unsafe void InvokeInline3(string identifier, int arg0, int arg1, int arg2, int* data, int dataLen)
        {
            int fid = RegisterFunction(identifier);
            GLCommandBuffer.WriteVoidInline3(fid, Uid, arg0, arg1, arg2, data, dataLen);
        }

        // --- InvokeDirect: flush buffer then execute immediately ---
        // Used for GL calls that pass data pointers (arrays) which may be
        // invalid by the time a deferred flush executes.

        protected unsafe void InvokeDirect<T1, T2, T3>(string identifier, T1 arg1, T2 arg2, T3 arg3)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3);
            JSInvoke2Void(fid, Uid, (int)&args);
        }

        protected unsafe void InvokeDirect<T1, T2, T3, T4>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4);
            JSInvoke2Void(fid, Uid, (int)&args);
        }

        protected unsafe void InvokeDirect<T1, T2, T3, T4, T5>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5);
            JSInvoke2Void(fid, Uid, (int)&args);
        }

        protected unsafe void InvokeDirect<T1, T2, T3, T4, T5, T6>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5, arg6);
            JSInvoke2Void(fid, Uid, (int)&args);
        }

        protected unsafe void InvokeDirect<T1, T2, T3, T4, T5, T6, T7>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            int fid = RegisterFunction(identifier);
            var args = ValueTuple.Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            JSInvoke2Void(fid, Uid, (int)&args);
        }

        protected unsafe void InvokeDirect<T1, T2, T3, T4, T5, T6, T7, T8>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            int fid = RegisterFunction(identifier);
            var args = new FixedStruct8<T1, T2, T3, T4, T5, T6, T7, T8>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            JSInvoke2Void(fid, Uid, (int)&args);
        }

        protected unsafe void InvokeDirect<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            int fid = RegisterFunction(identifier);
            var args = new FixedStruct9<T1, T2, T3, T4, T5, T6, T7, T8, T9>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            JSInvoke2Void(fid, Uid, (int)&args);
        }

        protected unsafe void InvokeDirect<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(string identifier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, TA argA)
        {
            if (GLCommandBuffer.ShouldBuffer) GLCommandBuffer.Flush();
            int fid = RegisterFunction(identifier);
            var args = new FixedStructA<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, argA);
            JSInvoke2Void(fid, Uid, (int)&args);
        }
    }

    internal struct FixedStruct8<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public readonly T1 Value1;
        public readonly T2 Value2;
        public readonly T3 Value3;
        public readonly T4 Value4;
        public readonly T5 Value5;
        public readonly T6 Value6;
        public readonly T7 Value7;
        public readonly T8 Value8;

        public FixedStruct8(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) : this()
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
            Value4 = value4;
            Value5 = value5;
            Value6 = value6;
            Value7 = value7;
            Value8 = value8;
        }
    }

    internal struct FixedStruct9<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        public readonly T1 Value1;
        public readonly T2 Value2;
        public readonly T3 Value3;
        public readonly T4 Value4;
        public readonly T5 Value5;
        public readonly T6 Value6;
        public readonly T7 Value7;
        public readonly T8 Value8;
        public readonly T9 Value9;

        public FixedStruct9(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9) : this()
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
            Value4 = value4;
            Value5 = value5;
            Value6 = value6;
            Value7 = value7;
            Value8 = value8;
            Value9 = value9;
        }
    }

    internal struct FixedStructA<T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>
    {
        public readonly T1 Value1;
        public readonly T2 Value2;
        public readonly T3 Value3;
        public readonly T4 Value4;
        public readonly T5 Value5;
        public readonly T6 Value6;
        public readonly T7 Value7;
        public readonly T8 Value8;
        public readonly T9 Value9;
        public readonly TA ValueA;

        public FixedStructA(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, TA valueA) : this()
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
            Value4 = value4;
            Value5 = value5;
            Value6 = value6;
            Value7 = value7;
            Value8 = value8;
            Value9 = value9;
            ValueA = valueA;
        }
    }
}
