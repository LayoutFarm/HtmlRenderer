
#region License
//2016, MIT, WinterDev
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
using System.Security;
using System.Runtime.InteropServices;
namespace Pencil.Gaming
{
    static unsafe class Glfw32
    {
        const string NATIVE32_GLFW3 = "natives32/glfw3.dll";
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern int glfwInit();
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwTerminate();
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwGetVersion(out int major, out int minor, out int rev);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern sbyte* glfwGetVersionString();
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwErrorFun glfwSetErrorCallback(GlfwErrorFun cbfun);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwMonitorPtr* glfwGetMonitors(out int count);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwMonitorPtr glfwGetPrimaryMonitor();
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwGetMonitorPos(GlfwMonitorPtr monitor, out int xpos, out int ypos);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwGetMonitorPhysicalSize(GlfwMonitorPtr monitor, out int width, out int height);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern sbyte* glfwGetMonitorName(GlfwMonitorPtr monitor);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwVidMode* glfwGetVideoModes(GlfwMonitorPtr monitor, out int count);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwVidMode* glfwGetVideoMode(GlfwMonitorPtr monitor);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwSetGamma(GlfwMonitorPtr monitor, float gamma);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwGetGammaRamp(GlfwMonitorPtr monitor, out GlfwGammaRampInternal ramp);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwSetGammaRamp(GlfwMonitorPtr monitor, ref GlfwGammaRamp ramp);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwDefaultWindowHints();
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwWindowHint(WindowHint target, int hint);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwWindowPtr glfwCreateWindow(int width, int height, [MarshalAs(UnmanagedType.LPStr)] string title, GlfwMonitorPtr monitor, GlfwWindowPtr share);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwDestroyWindow(GlfwWindowPtr window);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern int glfwWindowShouldClose(GlfwWindowPtr window);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwSetWindowShouldClose(GlfwWindowPtr window, int value);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwSetWindowTitle(GlfwWindowPtr window, [MarshalAs(UnmanagedType.LPStr)] string title);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwGetWindowPos(GlfwWindowPtr window, out int xpos, out int ypos);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwSetWindowPos(GlfwWindowPtr window, int xpos, int ypos);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwGetWindowSize(GlfwWindowPtr window, out int width, out int height);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwSetWindowSize(GlfwWindowPtr window, int width, int height);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwIconifyWindow(GlfwWindowPtr window);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwRestoreWindow(GlfwWindowPtr window);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwShowWindow(GlfwWindowPtr window);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwHideWindow(GlfwWindowPtr window);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwMonitorPtr glfwGetWindowMonitor(GlfwWindowPtr window);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern int glfwGetWindowAttrib(GlfwWindowPtr window, int param);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwSetWindowUserPointer(GlfwWindowPtr window, IntPtr pointer);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr glfwGetWindowUserPointer(GlfwWindowPtr window);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwWindowPosFun glfwSetWindowPosCallback(GlfwWindowPtr window, GlfwWindowPosFun cbfun);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwWindowSizeFun glfwSetWindowSizeCallback(GlfwWindowPtr window, GlfwWindowSizeFun cbfun);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwWindowCloseFun glfwSetWindowCloseCallback(GlfwWindowPtr window, GlfwWindowCloseFun cbfun);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwWindowRefreshFun glfwSetWindowRefreshCallback(GlfwWindowPtr window, GlfwWindowRefreshFun cbfun);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwWindowFocusFun glfwSetWindowFocusCallback(GlfwWindowPtr window, GlfwWindowFocusFun cbfun);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwWindowIconifyFun glfwSetWindowIconifyCallback(GlfwWindowPtr window, GlfwWindowIconifyFun cbfun);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwPollEvents();
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwWaitEvents();
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern int glfwGetInputMode(GlfwWindowPtr window, InputMode mode);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwSetInputMode(GlfwWindowPtr window, InputMode mode, CursorMode value);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern int glfwGetKey(GlfwWindowPtr window, Key key);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern int glfwGetMouseButton(GlfwWindowPtr window, MouseButton button);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwGetCursorPos(GlfwWindowPtr window, out double xpos, out double ypos);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwSetCursorPos(GlfwWindowPtr window, double xpos, double ypos);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwKeyFun glfwSetKeyCallback(GlfwWindowPtr window, GlfwKeyFun cbfun);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwCharFun glfwSetCharCallback(GlfwWindowPtr window, GlfwCharFun cbfun);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwMouseButtonFun glfwSetMouseButtonCallback(GlfwWindowPtr window, GlfwMouseButtonFun cbfun);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwCursorPosFun glfwSetCursorPosCallback(GlfwWindowPtr window, GlfwCursorPosFun cbfun);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwCursorEnterFun glfwSetCursorEnterCallback(GlfwWindowPtr window, GlfwCursorEnterFun cbfun);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwScrollFun glfwSetScrollCallback(GlfwWindowPtr window, GlfwScrollFun cbfun);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern int glfwJoystickPresent(Joystick joy);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern float* glfwGetJoystickAxes(Joystick joy, out int numaxes);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern byte* glfwGetJoystickButtons(Joystick joy, out int numbuttons);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern sbyte* glfwGetJoystickName(Joystick joy);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwSetClipboardString(GlfwWindowPtr window, [MarshalAs(UnmanagedType.LPStr)] string @string);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern sbyte* glfwGetClipboardString(GlfwWindowPtr window);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern double glfwGetTime();
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwSetTime(double time);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwMakeContextCurrent(GlfwWindowPtr window);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwWindowPtr glfwGetCurrentContext();
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwSwapBuffers(GlfwWindowPtr window);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwSwapInterval(int interval);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern int glfwExtensionSupported([MarshalAs(UnmanagedType.LPStr)] string extension);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr glfwGetProcAddress([MarshalAs(UnmanagedType.LPStr)] string procname);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern void glfwGetFramebufferSize(GlfwWindowPtr window, out int width, out int height);
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern GlfwFramebufferSizeFun glfwSetFramebufferSizeCallback(GlfwWindowPtr window, GlfwFramebufferSizeFun cbfun);
        //-------------
        [DllImport(NATIVE32_GLFW3), SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr glfwGetWin32Window(GlfwWindowPtr window);
        //--------------------------------------------------------------
    }
}

