#region License
// Copyright (c) 2013 Antonie Blom
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion


using System;
using System.Runtime.InteropServices;
namespace Pencil.Gaming
{
    [StructLayout(LayoutKind.Explicit, Size = sizeof(int) * 6)]
    public struct GlfwVidMode
    {
        [FieldOffset(sizeof(int) * 0)]
        public int
            Width;
        [FieldOffset(sizeof(int) * 1)]
        public int
            Height;
        [FieldOffset(sizeof(int) * 2)]
        public int
            RedBits;
        [FieldOffset(sizeof(int) * 3)]
        public int
            BlueBits;
        [FieldOffset(sizeof(int) * 4)]
        public int
            GreenBits;
        [FieldOffset(sizeof(int) * 5)]
        public int
            RefreshRate;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct GlfwGammaRampInternal
    {
        public uint* Red;
        public uint* Green;
        public uint* Blue;
        public uint Length;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GlfwGammaRamp
    {
        [MarshalAs(UnmanagedType.LPArray)]
        public uint[] Red;
        [MarshalAs(UnmanagedType.LPArray)]
        public uint[] Green;
        [MarshalAs(UnmanagedType.LPArray)]
        public uint[] Blue;
        internal uint Length;
    }

#pragma warning disable 0414

    [StructLayout(LayoutKind.Explicit)]
    public struct GlfwMonitorPtr
    {
        private GlfwMonitorPtr(IntPtr ptr)
        {
            inner_ptr = ptr;
        }

        [FieldOffsetAttribute(0)]
        private IntPtr
            inner_ptr;
        public readonly static GlfwMonitorPtr Null = new GlfwMonitorPtr(IntPtr.Zero);
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct GlfwWindowPtr
    {
        private GlfwWindowPtr(IntPtr ptr)
        {
            inner_ptr = ptr;
        }

        [FieldOffsetAttribute(0)]
        public IntPtr inner_ptr;
        public readonly static GlfwWindowPtr Null = new GlfwWindowPtr(IntPtr.Zero);
        public bool IsEmpty
        {
            get { return inner_ptr == IntPtr.Zero; }
        }
        public static bool IsEqual(GlfwWindowPtr ptr1, GlfwWindowPtr ptr2)
        {
            return ptr1.inner_ptr == ptr2.inner_ptr;
        }
    }

#pragma warning restore 0414
}


