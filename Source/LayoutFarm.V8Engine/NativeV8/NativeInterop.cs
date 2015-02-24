using System;
using System.Collections.Generic; 
using System.Runtime.InteropServices;
using System.IO;


namespace NativeV8
{

    abstract class NativeMethodMap
    {
        protected Delegate del;
        string nativeMethodName;
        bool isResolved;
        public NativeMethodMap(string nativeMethodName)
        {
            this.nativeMethodName = nativeMethodName;
        }
        public string NativeMethodName
        {
            get
            {
                return this.nativeMethodName;
            }
        }
        public bool Resolve(IntPtr hModule)
        {
            IntPtr foundProcAddress = UnsafeMethods.GetProcAddress(hModule, this.nativeMethodName);

            if (foundProcAddress == IntPtr.Zero)
            {
                return false;
            }
                         
            //transform func pointer to delegate
            TransformFunctionPointer(foundProcAddress);

            return true;
        }
        public bool IsResolved
        {
            get
            {
                return this.isResolved;
            }
            protected set
            {
                this.isResolved = value;
            }
        }
        protected abstract void TransformFunctionPointer(IntPtr funcPointer);
        public Delegate GetDelegate()
        {
            return this.del;
        }
    }
    class NativeActionMap<T> : NativeMethodMap
    {
        public NativeActionMap(string nativeMethodName)
            : base(nativeMethodName)
        {

        }
        protected override void TransformFunctionPointer(IntPtr funcPointer)
        {
            this.del = Marshal.GetDelegateForFunctionPointer(funcPointer, typeof(T));
            this.IsResolved = true;
        }

    }
    class NativeFunctionMap<T> : NativeMethodMap
    {

        public NativeFunctionMap(string nativeMethodName)
            : base(nativeMethodName)
        {
        }
        protected override void TransformFunctionPointer(IntPtr funcPointer)
        {
            this.del = Marshal.GetDelegateForFunctionPointer(funcPointer, typeof(T));
            this.IsResolved = true;
        }
    }

    [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
    public class MethodCallingArgs
    {
        public int numArgs; 
    }

    public enum ManagedCallbackKind : int
    {
        Listener,
        MethodCall
    }
    static class UnsafeMethods
    {
        //-----------------------------------------------
        [DllImport("Kernel32.dll")]
        public static extern IntPtr LoadLibrary(string libraryName);
        [DllImport("Kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("Kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("Kernel32.dll")]
        public static extern uint SetErrorMode(int uMode);
        [DllImport("Kernel32.dll")]
        public static extern uint GetLastError();
        //----------------------------------------------- 
    }
}