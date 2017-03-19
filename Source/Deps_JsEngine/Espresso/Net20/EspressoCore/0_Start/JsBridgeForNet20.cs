using System;
using System.Runtime.InteropServices;
namespace Espresso
{

    static partial class JsBridge
    {
        static IntPtr hModuleV8;
        public static void LoadV8(string dllfile, bool doV8Init = true)
        {
            hModuleV8 = LoadLibrary(dllfile);
            if (hModuleV8 == IntPtr.Zero)
            {
                return;
            }

            //-------------------
            if (doV8Init)
            {    
                //sometime we set to false , and  let underlying lib init the v8 engine.
                NativeV8JsInterOp.V8Init();
            }
        }

        public static void UnloadV8()
        {
            if (hModuleV8 != IntPtr.Zero)
            {
                FreeLibrary(hModuleV8);
                hModuleV8 = IntPtr.Zero;
            }
        }

        //----------------------------------
        //if this is windows
        [DllImport("Kernel32.dll")]
        static extern IntPtr LoadLibrary(string libraryName);
        [DllImport("Kernel32.dll")]
        static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("Kernel32.dll")]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("Kernel32.dll")]
        static extern uint SetErrorMode(int uMode);
        [DllImport("Kernel32.dll")]
        static extern uint GetLastError();
        //if unix
    }

}