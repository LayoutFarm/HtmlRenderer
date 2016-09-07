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
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GlfwErrorFun(GlfwError code, [MarshalAs(UnmanagedType.LPStr)] string desc);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GlfwMonitorFun(GlfwMonitorPtr mtor, ConnectionState @enum);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GlfwWindowCloseFun(GlfwWindowPtr wnd);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GlfwWindowPosFun(GlfwWindowPtr wnd, int x, int y);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GlfwWindowRefreshFun(GlfwWindowPtr wnd);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GlfwWindowSizeFun(GlfwWindowPtr wnd, int width, int height);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GlfwWindowFocusFun(GlfwWindowPtr wnd, bool focus);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GlfwKeyFun(GlfwWindowPtr wnd, Key key, int scanCode, KeyAction action, KeyModifiers mods);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GlfwCharFun(GlfwWindowPtr wnd, char ch);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GlfwMouseButtonFun(GlfwWindowPtr wnd, MouseButton btn, KeyAction action);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GlfwWindowIconifyFun(GlfwWindowPtr wnd, bool iconify);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GlfwCursorPosFun(GlfwWindowPtr wnd, double x, double y);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GlfwCursorEnterFun(GlfwWindowPtr wnd, bool enter);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GlfwScrollFun(GlfwWindowPtr wnd, double xoffset, double yoffset);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GlfwFramebufferSizeFun(GlfwWindowPtr wnd, int width, int height);
}

