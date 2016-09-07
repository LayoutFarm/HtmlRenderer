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
namespace Pencil.Gaming
{
    public static unsafe class Glfw
    {
#pragma warning disable 0414

        public static bool Init()
        {
            return GlfwDelegates.glfwInit() == 1;
        }
        public static void Terminate()
        {
            GlfwDelegates.glfwTerminate();
        }
        public static void GetVersion(out int major, out int minor, out int rev)
        {
            GlfwDelegates.glfwGetVersion(out major, out minor, out rev);
        }
        public static string GetVersionString()
        {
            return new string(GlfwDelegates.glfwGetVersionString());
        }
        private static GlfwErrorFun errorCallback;
        public static GlfwErrorFun SetErrorCallback(GlfwErrorFun cbfun)
        {
            errorCallback = cbfun;
            return GlfwDelegates.glfwSetErrorCallback(cbfun);
        }
        public static unsafe GlfwMonitorPtr[] GetMonitors()
        {
            int count;
            GlfwMonitorPtr* array = GlfwDelegates.glfwGetMonitors(out count);
            GlfwMonitorPtr[] result = new GlfwMonitorPtr[count];
            for (int i = 0; i < count; ++i)
            {
                result[i] = array[i];
            }
            return result;
        }
        public static GlfwMonitorPtr GetPrimaryMonitor()
        {
            return GlfwDelegates.glfwGetPrimaryMonitor();
        }
        public static void GetMonitorPos(GlfwMonitorPtr monitor, out int xpos, out int ypos)
        {
            GlfwDelegates.glfwGetMonitorPos(monitor, out xpos, out ypos);
        }
        public static void GetMonitorPhysicalSize(GlfwMonitorPtr monitor, out int width, out int height)
        {
            GlfwDelegates.glfwGetMonitorPhysicalSize(monitor, out width, out height);
        }
        public static string GetMonitorName(GlfwMonitorPtr monitor)
        {
            return new string(GlfwDelegates.glfwGetMonitorName(monitor));
        }
        public static GlfwVidMode[] GetVideoModes(GlfwMonitorPtr monitor)
        {
            int count;
            GlfwVidMode* array = GlfwDelegates.glfwGetVideoModes(monitor, out count);
            GlfwVidMode[] result = new GlfwVidMode[count];
            for (int i = 0; i < count; ++i)
            {
                result[i] = array[i];
            }
            return result;
        }
        public static GlfwVidMode GetVideoMode(GlfwMonitorPtr monitor)
        {
            GlfwVidMode* vidMode = GlfwDelegates.glfwGetVideoMode(monitor);
            GlfwVidMode returnMode = new GlfwVidMode
            {
                RedBits = vidMode->RedBits,
                GreenBits = vidMode->GreenBits,
                BlueBits = vidMode->BlueBits,
                RefreshRate = vidMode->RefreshRate,
                Width = vidMode->Width,
                Height = vidMode->Height
            };
            return returnMode;
        }
        public static void SetGamma(GlfwMonitorPtr monitor, float gamma)
        {
            GlfwDelegates.glfwSetGamma(monitor, gamma);
        }
        public static void GetGammaRamp(GlfwMonitorPtr monitor, out GlfwGammaRamp ramp)
        {
            GlfwGammaRampInternal rampI;
            GlfwDelegates.glfwGetGammaRamp(monitor, out rampI);
            uint length = rampI.Length;
            ramp = new GlfwGammaRamp();
            ramp.Red = new uint[length];
            ramp.Green = new uint[length];
            ramp.Blue = new uint[length];
            for (int i = 0; i < ramp.Red.Length; ++i)
            {
                ramp.Red[i] = rampI.Red[i];
            }
            for (int i = 0; i < ramp.Green.Length; ++i)
            {
                ramp.Green[i] = rampI.Green[i];
            }
            for (int i = 0; i < ramp.Blue.Length; ++i)
            {
                ramp.Blue[i] = rampI.Blue[i];
            }
        }
        public static void SetGammaRamp(GlfwMonitorPtr monitor, ref GlfwGammaRamp ramp)
        {
            ramp.Length = (uint)ramp.Red.Length;
            GlfwDelegates.glfwSetGammaRamp(monitor, ref ramp);
        }
        public static void DefaultWindowHints()
        {
            GlfwDelegates.glfwDefaultWindowHints();
        }
        public static void WindowHint(WindowHint target, int hint)
        {
            GlfwDelegates.glfwWindowHint(target, hint);
        }
        public static GlfwWindowPtr CreateWindow(int width, int height, string title, GlfwMonitorPtr monitor, GlfwWindowPtr share)
        {
            return GlfwDelegates.glfwCreateWindow(width, height, title, monitor, share);
        }
        public static IntPtr GetNativePlatformWinHwnd(GlfwWindowPtr wnd)
        {
            return GlfwDelegates.glfwGetWin32Window(wnd);
        }
        public static void DestroyWindow(GlfwWindowPtr window)
        {
            GlfwDelegates.glfwDestroyWindow(window);
        }
        public static void GetFramebufferSize(GlfwWindowPtr window, out int width, out int height)
        {
            GlfwDelegates.glfwGetFramebufferSize(window, out width, out height);
        }
        public static bool WindowShouldClose(GlfwWindowPtr window)
        {
            return GlfwDelegates.glfwWindowShouldClose(window) == 1;
        }
        public static void SetWindowShouldClose(GlfwWindowPtr window, bool value)
        {
            GlfwDelegates.glfwSetWindowShouldClose(window, value ? 1 : 0);
        }
        public static void SetWindowTitle(GlfwWindowPtr window, string title)
        {
            GlfwDelegates.glfwSetWindowTitle(window, title);
        }
        public static void GetWindowPos(GlfwWindowPtr window, out int xpos, out int ypos)
        {
            GlfwDelegates.glfwGetWindowPos(window, out xpos, out ypos);
        }
        public static void SetWindowPos(GlfwWindowPtr window, int xpos, int ypos)
        {
            GlfwDelegates.glfwSetWindowPos(window, xpos, ypos);
        }
        public static void GetWindowSize(GlfwWindowPtr window, out int width, out int height)
        {
            GlfwDelegates.glfwGetWindowSize(window, out width, out height);
        }
        public static void SetWindowSize(GlfwWindowPtr window, int width, int height)
        {
            GlfwDelegates.glfwSetWindowSize(window, width, height);
        }
        public static void IconifyWindow(GlfwWindowPtr window)
        {
            GlfwDelegates.glfwIconifyWindow(window);
        }
        public static void RestoreWindow(GlfwWindowPtr window)
        {
            GlfwDelegates.glfwRestoreWindow(window);
        }
        public static void ShowWindow(GlfwWindowPtr window)
        {
            GlfwDelegates.glfwShowWindow(window);
        }
        public static void HideWindow(GlfwWindowPtr window)
        {
            GlfwDelegates.glfwHideWindow(window);
        }
        public static GlfwMonitorPtr GetWindowMonitor(GlfwWindowPtr window)
        {
            return GlfwDelegates.glfwGetWindowMonitor(window);
        }
        public static int GetWindowAttrib(GlfwWindowPtr window, WindowAttrib param)
        {
            return GlfwDelegates.glfwGetWindowAttrib(window, (int)param);
        }
        public static int GetWindowAttrib(GlfwWindowPtr window, WindowHint param)
        {
            return GlfwDelegates.glfwGetWindowAttrib(window, (int)param);
        }
        public static void SetWindowUserPointer(GlfwWindowPtr window, IntPtr pointer)
        {
            GlfwDelegates.glfwSetWindowUserPointer(window, pointer);
        }
        public static IntPtr GetWindowUserPointer(GlfwWindowPtr window)
        {
            return GlfwDelegates.glfwGetWindowUserPointer(window);
        }
        private static GlfwFramebufferSizeFun framebufferSizeFun;
        public static GlfwFramebufferSizeFun SetFramebufferSizeCallback(GlfwWindowPtr window, GlfwFramebufferSizeFun cbfun)
        {
            framebufferSizeFun = cbfun;
            return GlfwDelegates.glfwSetFramebufferSizeCallback(window, cbfun);
        }
        private static GlfwWindowPosFun windowPosFun;
        public static GlfwWindowPosFun SetWindowPosCallback(GlfwWindowPtr window, GlfwWindowPosFun cbfun)
        {
            windowPosFun = cbfun;
            return GlfwDelegates.glfwSetWindowPosCallback(window, cbfun);
        }
        private static GlfwWindowSizeFun windowSizeFun;
        public static GlfwWindowSizeFun SetWindowSizeCallback(GlfwWindowPtr window, GlfwWindowSizeFun cbfun)
        {
            windowSizeFun = cbfun;
            return GlfwDelegates.glfwSetWindowSizeCallback(window, cbfun);
        }
        private static GlfwWindowCloseFun windowCloseFun;
        public static GlfwWindowCloseFun SetWindowCloseCallback(GlfwWindowPtr window, GlfwWindowCloseFun cbfun)
        {
            windowCloseFun = cbfun;
            return GlfwDelegates.glfwSetWindowCloseCallback(window, cbfun);
        }
        private static GlfwWindowRefreshFun windowRefreshFun;
        public static GlfwWindowRefreshFun SetWindowRefreshCallback(GlfwWindowPtr window, GlfwWindowRefreshFun cbfun)
        {
            windowRefreshFun = cbfun;
            return GlfwDelegates.glfwSetWindowRefreshCallback(window, cbfun);
        }
        private static GlfwWindowFocusFun windowFocusFun;
        public static GlfwWindowFocusFun SetWindowFocusCallback(GlfwWindowPtr window, GlfwWindowFocusFun cbfun)
        {
            windowFocusFun = cbfun;
            return GlfwDelegates.glfwSetWindowFocusCallback(window, cbfun);
        }
        private static GlfwWindowIconifyFun windowIconifyFun;
        public static GlfwWindowIconifyFun SetWindowIconifyCallback(GlfwWindowPtr window, GlfwWindowIconifyFun cbfun)
        {
            windowIconifyFun = cbfun;
            return GlfwDelegates.glfwSetWindowIconifyCallback(window, cbfun);
        }
        public static void PollEvents()
        {
            GlfwDelegates.glfwPollEvents();
        }
        public static void WaitEvents()
        {
            GlfwDelegates.glfwWaitEvents();
        }
        public static int GetInputMode(GlfwWindowPtr window, InputMode mode)
        {
            return GlfwDelegates.glfwGetInputMode(window, mode);
        }
        public static void SetInputMode(GlfwWindowPtr window, InputMode mode, CursorMode value)
        {
            GlfwDelegates.glfwSetInputMode(window, mode, value);
        }
        public static bool GetKey(GlfwWindowPtr window, Key key)
        {
            return GlfwDelegates.glfwGetKey(window, key) != 0;
        }
        public static bool GetMouseButton(GlfwWindowPtr window, MouseButton button)
        {
            return GlfwDelegates.glfwGetMouseButton(window, button) != 0;
        }
        public static void GetCursorPos(GlfwWindowPtr window, out double xpos, out double ypos)
        {
            GlfwDelegates.glfwGetCursorPos(window, out xpos, out ypos);
        }
        public static void SetCursorPos(GlfwWindowPtr window, double xpos, double ypos)
        {
            GlfwDelegates.glfwSetCursorPos(window, xpos, ypos);
        }
        private static GlfwKeyFun keyFun;
        public static GlfwKeyFun SetKeyCallback(GlfwWindowPtr window, GlfwKeyFun cbfun)
        {
            keyFun = cbfun;
            return GlfwDelegates.glfwSetKeyCallback(window, cbfun);
        }
        private static GlfwCharFun charFun;
        public static GlfwCharFun SetCharCallback(GlfwWindowPtr window, GlfwCharFun cbfun)
        {
            charFun = cbfun;
            return GlfwDelegates.glfwSetCharCallback(window, cbfun);
        }
        private static GlfwMouseButtonFun mouseButtonFun;
        public static GlfwMouseButtonFun SetMouseButtonCallback(GlfwWindowPtr window, GlfwMouseButtonFun cbfun)
        {
            mouseButtonFun = cbfun;
            return GlfwDelegates.glfwSetMouseButtonCallback(window, cbfun);
        }
        private static GlfwCursorPosFun cursorPosFun;
        public static GlfwCursorPosFun SetCursorPosCallback(GlfwWindowPtr window, GlfwCursorPosFun cbfun)
        {
            cursorPosFun = cbfun;
            return GlfwDelegates.glfwSetCursorPosCallback(window, cbfun);
        }
        private static GlfwCursorEnterFun cursorEnterFun;
        public static GlfwCursorEnterFun SetCursorEnterCallback(GlfwWindowPtr window, GlfwCursorEnterFun cbfun)
        {
            cursorEnterFun = cbfun;
            return GlfwDelegates.glfwSetCursorEnterCallback(window, cbfun);
        }
        private static GlfwScrollFun scrollFun;
        public static GlfwScrollFun SetScrollCallback(GlfwWindowPtr window, GlfwScrollFun cbfun)
        {
            scrollFun = cbfun;
            return GlfwDelegates.glfwSetScrollCallback(window, cbfun);
        }
        public static bool JoystickPresent(Joystick joy)
        {
            return GlfwDelegates.glfwJoystickPresent(joy) == 1;
        }
        public static float[] GetJoystickAxes(Joystick joy)
        {
            int numaxes;
            float* axes = GlfwDelegates.glfwGetJoystickAxes(joy, out numaxes);
            float[] result = new float[numaxes];
            for (int i = 0; i < numaxes; ++i)
            {
                result[i] = axes[i];
            }
            return result;
        }
        public static byte[] GetJoystickButtons(Joystick joy)
        {
            int numbuttons;
            byte* buttons = GlfwDelegates.glfwGetJoystickButtons(joy, out numbuttons);
            byte[] result = new byte[numbuttons];
            for (int i = 0; i < numbuttons; ++i)
            {
                result[i] = buttons[i];
            }
            return result;
        }
        public static string GetJoystickName(Joystick joy)
        {
            return new string(GlfwDelegates.glfwGetJoystickName(joy));
        }
        public static void SetClipboardString(GlfwWindowPtr window, string @string)
        {
            GlfwDelegates.glfwSetClipboardString(window, @string);
        }
        public static string GetClipboardString(GlfwWindowPtr window)
        {
            return new string(GlfwDelegates.glfwGetClipboardString(window));
        }
        public static double GetTime()
        {
            return GlfwDelegates.glfwGetTime();
        }
        public static void SetTime(double time)
        {
            GlfwDelegates.glfwSetTime(time);
        }
        public static void MakeContextCurrent(GlfwWindowPtr window)
        {
            GlfwDelegates.glfwMakeContextCurrent(window);
        }
        public static GlfwWindowPtr GetCurrentContext()
        {
            return GlfwDelegates.glfwGetCurrentContext();
        }
        public static void SwapBuffers(GlfwWindowPtr window)
        {
            GlfwDelegates.glfwSwapBuffers(window);
        }
        public static void SwapInterval(int interval)
        {
            GlfwDelegates.glfwSwapInterval(interval);
        }
        public static bool ExtensionSupported(string extension)
        {
            return GlfwDelegates.glfwExtensionSupported(extension) == 1;
        }
        public static IntPtr GetProcAddress(string procname)
        {
            return GlfwDelegates.glfwGetProcAddress(procname);
        }

#pragma warning restore 0414
    }
}

