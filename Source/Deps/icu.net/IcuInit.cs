//2015-2016 MIT, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
namespace Icu
{
    public static class IcuInit
    {
        public static void LoadIcuLibrary(string libFolder)
        {
            //1. load icudt first
            WinNativeMethods.LoadLibrary(libFolder + "\\icudt54.dll");
            //2. then
            WinNativeMethods.LoadLibrary(libFolder + "\\icuuc54.dll");
        }
    }

    static class WinNativeMethods
    {
        //-----------------------------------------------
        //this is Windows Specific class ***
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