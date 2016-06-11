//2015 MIT, WinterDev
using System;
namespace VroomJs
{
    public static class JsBridge
    {
        public const string LIB_NAME = "libespr";
        static IntPtr hModuleV8;

        public static void LoadV8(string v8bridgeDll)
        {

            IntPtr v8mod = UnsafeNativeMethods.LoadLibrary(v8bridgeDll);
            hModuleV8 = v8mod;
            if (v8mod == IntPtr.Zero)
            {
                return;
            }
            NativeV8JsInterOp.V8Init();
        }

        public static void V8Init()
        {
            NativeV8JsInterOp.V8Init();
        }

        public static void UnloadV8()
        {
            if (hModuleV8 != IntPtr.Zero)
            {
                UnsafeNativeMethods.FreeLibrary(hModuleV8);
                hModuleV8 = IntPtr.Zero;
            }
        }
        public static int LibVersion
        {
            get { return JsContext.getVersion(); }
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