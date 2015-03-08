using System;
namespace VroomJs
{
    public static class JsBridge
    {
        public const string LIB_NAME = "VroomJsNative";
        static IntPtr hModuleV8;

        public static void LoadV8(string v8bridgeDll)
        {

            IntPtr v8mod = UnsafeNativeMethods.LoadLibrary(v8bridgeDll);
            hModuleV8 = v8mod;
            if (v8mod == IntPtr.Zero)
            {
                return;
            }
        }
        public static void UnloadV8()
        {
            if (hModuleV8 != IntPtr.Zero)
            {
                UnsafeNativeMethods.FreeLibrary(hModuleV8);
                hModuleV8 = IntPtr.Zero;
            }
        }

        //---------------------------------------------
#if DEBUG
        public static void dbugTestCallbacks()
        {
            NativeV8JsInterOp.RegisterCallBacks();
            NativeV8JsInterOp.TestCallBack();
        }
#endif
    }

}