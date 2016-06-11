// 2015,2014 ,MIT, WinterDev

//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//-----------------------------------

using System;
using System.Runtime.InteropServices;
namespace PixelFarm.Agg
{
    //--------------------
    //on Windows...
    //--------------------

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

        [DllImport("Kernel32.dll")]
        public static extern IntPtr HeapCreate(int flOption, int initSize, int maxSize);
        public const int HEAP_CREATE_ENABLE_EXECUTE = 0x00040000;
        public const int HEAP_GENERATE_EXCEPTIONS = 0x00000004;
        public const int HEAP_NO_SERAILIZE = 0x00000001;
        [DllImport("Kernel32.dll")]
        public static extern bool HeapDestroy(IntPtr hHeap);
        [DllImport("Kernel32.dll")]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, int len);
        [DllImport("Kernel32.dll")]
        public static extern void VirtualAlloc(IntPtr address, int size, int flAllocType, int flProtect);
        //----------------------------------------------- 
    }
}