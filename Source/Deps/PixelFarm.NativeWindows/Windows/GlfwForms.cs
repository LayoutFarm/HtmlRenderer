//MIT, 2016-2017, WinterDev
using System;
using System.Collections.Generic;
using Pencil.Gaming;

namespace PixelFarm.Forms
{
    public static class GlfwApp
    {
        static Dictionary<GlfwWindowPtr, GlFwForm> existingForms = new Dictionary<GlfwWindowPtr, GlFwForm>();
        static List<GlFwForm> exitingFormList = new List<GlFwForm>();
        static GlfwWindowCloseFun s_windowCloseCb;
        static GlfwWindowFocusFun s_windowFocusCb;
        static GlfwWindowIconifyFun s_windowIconifyCb;
        static GlfwWindowPosFun s_windowPosCb;
        static GlfwWindowRefreshFun s_windowRefreshCb;
        static GlfwWindowSizeFun s_windowSizeCb;
        static GlfwCursorPosFun s_windowCursorPosCb;
        static GlfwCursorEnterFun s_windowCursorEnterCb;
        static GlfwMouseButtonFun s_windowMouseButtonCb;
        static GlfwScrollFun s_scrollCb;
        static GlfwKeyFun s_windowKeyCb; //key up, key down
        static GlfwCharFun s_windowCharCb; //key press

        static IntPtr latestGlWindowPtr;
        static GlFwForm latestForm;

        static GlfwApp()
        {
            s_windowCloseCb = (GlfwWindowPtr wnd) =>
            {

                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    //user can cancel window close here here
                    bool userCancel = false;
                    GlFwForm.InvokeOnClosing(found, ref userCancel);
                    if (userCancel)
                    {
                        return;
                    }
                    //--------------------------------------
                    latestForm = null;
                    latestGlWindowPtr = IntPtr.Zero;
                    //user let this window close ***
                    Glfw.SetWindowShouldClose(wnd, true);
                    Glfw.DestroyWindow(wnd); //destroy this
                    existingForms.Remove(wnd);
                    exitingFormList.Remove(found);
                    //--------------------------------------
                }
            };
            s_windowFocusCb = (GlfwWindowPtr wnd, bool focus) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    GlFwForm.SetFocusState(found, focus);
                }
            };
            s_windowIconifyCb = (GlfwWindowPtr wnd, bool iconify) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    GlFwForm.SetIconifyState(found, iconify);
                }
            };

            s_windowPosCb = (GlfwWindowPtr wnd, int x, int y) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    GlFwForm.InvokeOnMove(found, x, y);
                }
            };
            s_windowRefreshCb = (GlfwWindowPtr wnd) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    GlFwForm.InvokeOnRefresh(found);
                }
            };

            s_windowSizeCb = (GlfwWindowPtr wnd, int width, int height) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    GlFwForm.InvokeOnSizeChanged(found, width, height);
                }
            };
            s_windowCursorPosCb = (GlfwWindowPtr wnd, double x, double y) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    GlFwForm.InvokeCursorPos(found, x, y);
                }
            };
            s_windowCursorEnterCb = (GlfwWindowPtr wnd, bool enter) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    GlFwForm.SetCursorEnterState(found, enter);
                }
            };
            s_windowMouseButtonCb = (GlfwWindowPtr wnd, MouseButton btn, KeyActionKind keyAction) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    GlFwForm.InvokeMouseButton(found, btn, keyAction);
                }
            };
            s_scrollCb = (GlfwWindowPtr wnd, double xoffset, double yoffset) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    GlFwForm.InvokeOnScroll(found, xoffset, yoffset);
                }
            };
            s_windowKeyCb = (GlfwWindowPtr wnd, Key key, int scanCode, KeyActionKind action, KeyModifiers mods) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    GlFwForm.InvokeKey(found, key, scanCode, action, mods);
                }
            };
            s_windowCharCb = (GlfwWindowPtr wnd, char ch) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    GlFwForm.InvokeKeyPress(found, ch);
                }
            };
        }
        static bool GetGlfwForm(GlfwWindowPtr wnd, out GlFwForm found)
        {
            if (wnd.inner_ptr == latestGlWindowPtr)
            {
                found = latestForm;
                return true;
            }
            else
            {

                if (existingForms.TryGetValue(wnd, out found))
                {
                    latestGlWindowPtr = wnd.inner_ptr;
                    latestForm = found;
                    return true;
                }
                //reset
                latestGlWindowPtr = IntPtr.Zero;
                latestForm = null;
                return false;
            }
        }

        internal static void InitGlFwForm(GlFwForm f)
        {
            //create pointer

            GlfwWindowPtr glWindowPtr = Glfw.CreateWindow(f.Width, f.Height,
                f.Text,
                new GlfwMonitorPtr(),//default monitor
                new GlfwWindowPtr()); //default top window 

            f.InitGlFwForm(glWindowPtr, f.Width, f.Height);
            //-------------------
            //setup events for glfw window
            Glfw.SetWindowCloseCallback(glWindowPtr, s_windowCloseCb);
            Glfw.SetWindowFocusCallback(glWindowPtr, s_windowFocusCb);
            Glfw.SetWindowIconifyCallback(glWindowPtr, s_windowIconifyCb);
            Glfw.SetWindowPosCallback(glWindowPtr, s_windowPosCb);
            Glfw.SetWindowRefreshCallback(glWindowPtr, s_windowRefreshCb);
            Glfw.SetWindowSizeCallback(glWindowPtr, s_windowSizeCb);
            Glfw.SetCursorPosCallback(glWindowPtr, s_windowCursorPosCb);
            Glfw.SetCursorEnterCallback(glWindowPtr, s_windowCursorEnterCb);
            Glfw.SetMouseButtonCallback(glWindowPtr, s_windowMouseButtonCb);
            Glfw.SetScrollCallback(glWindowPtr, s_scrollCb);
            Glfw.SetKeyCallback(glWindowPtr, s_windowKeyCb);
            Glfw.SetCharCallback(glWindowPtr, s_windowCharCb);
            ////-------------------
            existingForms.Add(glWindowPtr, f);
            exitingFormList.Add(f);

        }
        public static GlFwForm CreateGlfwForm(int w, int h, string title)
        {
            GlfwWindowPtr glWindowPtr = Glfw.CreateWindow(w, h,
                title,
                new GlfwMonitorPtr(),//default monitor
                new GlfwWindowPtr()); //default top window 
            GlFwForm f = new GlFwForm();
            f.InitGlFwForm(glWindowPtr, w, h);
            f.Text = title;
            //-------------------
            //setup events for glfw window
            Glfw.SetWindowCloseCallback(glWindowPtr, s_windowCloseCb);
            Glfw.SetWindowFocusCallback(glWindowPtr, s_windowFocusCb);
            Glfw.SetWindowIconifyCallback(glWindowPtr, s_windowIconifyCb);
            Glfw.SetWindowPosCallback(glWindowPtr, s_windowPosCb);
            Glfw.SetWindowRefreshCallback(glWindowPtr, s_windowRefreshCb);
            Glfw.SetWindowSizeCallback(glWindowPtr, s_windowSizeCb);
            Glfw.SetCursorPosCallback(glWindowPtr, s_windowCursorPosCb);
            Glfw.SetCursorEnterCallback(glWindowPtr, s_windowCursorEnterCb);
            Glfw.SetMouseButtonCallback(glWindowPtr, s_windowMouseButtonCb);
            Glfw.SetScrollCallback(glWindowPtr, s_scrollCb);
            Glfw.SetKeyCallback(glWindowPtr, s_windowKeyCb);
            Glfw.SetCharCallback(glWindowPtr, s_windowCharCb);
            ////-------------------
            existingForms.Add(glWindowPtr, f);
            exitingFormList.Add(f);
            return f;
        }


        public static bool ShouldClose()
        {
            for (int i = exitingFormList.Count - 1; i >= 0; --i)
            {
                //if we have some form that should not close
                //then we just return
                if (!Glfw.WindowShouldClose(exitingFormList[i].GlfwWindowPtr))
                {
                    return false;
                }
            }
            return true;
        }
        public static void UpdateWindowsFrame()
        {
            int j = exitingFormList.Count;
            for (int i = 0; i < j; ++i)
            {
                /* Render here */
                /* Swap front and back buffers */
                GlFwForm form = exitingFormList[i];
                form.DrawFrame();
                Glfw.SwapBuffers(form.GlfwWindowPtr);
            }

        }
    }

    public class FormRenderUpdateEventArgs : EventArgs
    {
        public GlFwForm form;
    }


    public class GlFwForm : Form
    {
        SimpleAction drawFrameDel;
        string _windowTitle = "";
        GlfwWindowPtr _nativeGlFwWindowPtr;
        IntPtr _nativePlatformHwnd;
        GlfwWinInfo _winInfo;
        double _latestMouseX;
        double _latestMouseY;

        public GlFwForm()
        {

        }

        internal void InitGlFwForm(GlfwWindowPtr glWindowPtr, int w, int h)
        {
            base.Width = w;
            base.Height = h;
            _nativeGlFwWindowPtr = glWindowPtr;
            _nativePlatformHwnd = Glfw.GetNativePlatformWinHwnd(glWindowPtr);
            _winInfo = new PixelFarm.GlfwWinInfo(_nativeGlFwWindowPtr);
        }
        internal GlfwWindowPtr GlfwWindowPtr
        {
            get
            {
                return _nativeGlFwWindowPtr;
            }
        }
        public override void Close()
        {
            Glfw.HideWindow(this._nativeGlFwWindowPtr);
            Glfw.DestroyWindow(this._nativeGlFwWindowPtr);
        }

        internal static void SetFocusState(GlFwForm f, bool focus)
        {
            if (focus)
            {
                f.OnFocus();
            }
            else
            {
                f.OnLostFocus();
            }
        }

        protected virtual void OnMouseDown(MouseButton btn, double x, double y)
        {

        }
        protected virtual void OnMouseMove(double x, double y)
        {

        }
        protected virtual void OnMouseUp(MouseButton btn, double x, double y)
        {

        }
        internal static void InvokeMouseButton(GlFwForm f, MouseButton btn, KeyActionKind action)
        {
            //TODO: implement detail methods 
            switch (action)
            {
                default:
                    throw new NotFiniteNumberException();
                case KeyActionKind.Press:
                    f.OnMouseDown(btn, f._latestMouseX, f._latestMouseY);
                    break;
                case KeyActionKind.Release:
                    f.OnMouseUp(btn, f._latestMouseX, f._latestMouseY);
                    break;
                case KeyActionKind.Repeat:
                    break;
            }
        }
        internal static void InvokeCursorPos(GlFwForm f, double x, double y)
        {
            //TODO: implement detail methods
            f._latestMouseX = x;
            f._latestMouseY = y;
            f.OnMouseMove(x, y);
        }
        internal static void InvokeKeyPress(GlFwForm f, char c)
        {
            //TODO: implement detail methods
            f.OnKeyPress(c);
        }
        internal static void InvokeKey(GlFwForm f, Key key, int scanCode, KeyActionKind keyAction, KeyModifiers mods)
        {
            switch (keyAction)
            {
                default: throw new NotFiniteNumberException();
                case KeyActionKind.Press:
                    f.OnKeyDown(key, scanCode, mods);
                    break;
                case KeyActionKind.Repeat:
                    f.OnKeyRepeat(key, scanCode, mods);
                    break;
                case KeyActionKind.Release:
                    f.OnKeyUp(key, scanCode, mods);
                    break;
            }
        }
        protected virtual void OnKeyUp(Key key, int scanCode, KeyModifiers mods)
        {
        }
        protected virtual void OnKeyDown(Key key, int scanCode, KeyModifiers mods)
        {
        }
        protected virtual void OnKeyRepeat(Key key, int scanCode, KeyModifiers mods)
        {
        }
        protected virtual void OnKeyPress(char c)
        {
        }

        internal static void InvokeOnScroll(GlFwForm f, double xoffset, double yoffset)
        {
            //TODO: implement detail methods
        }
        internal static void SetIconifyState(GlFwForm f, bool iconify)
        {
            f.OnIconify(iconify);
        }
        internal static void InvokeOnMove(GlFwForm f, int x, int y)
        {
            //window moved
            //on pos changed
            //TODO: implement detail methods
        }
        internal static void InvokeOnSizeChanged(GlFwForm f, int w, int h)
        {
            //on pos changed
            //TODO: implement detail methods
            f.OnSizeChanged(EventArgs.Empty);
        }
        internal static void InvokeOnRefresh(GlFwForm f)
        {
            //TODO: implement detail methods
        }
        internal static void InvokeOnClosing(GlFwForm f, ref bool cancel)
        {
            f.OnClosing(ref cancel);
        }
        internal static void SetCursorEnterState(GlFwForm f, bool enter)
        {
            if (enter)
            {
                f.OnCursorEnter();
            }
            else
            {
                f.OnCursorLeave();
            }
        }
        protected virtual void OnCursorEnter()
        {

        }
        public virtual void OnCursorLeave()
        {

        }

        protected virtual void OnIconify(bool iconify)
        {

        }
        protected virtual void OnFocus()
        {
        }
        protected virtual void OnLostFocus()
        {
        }
        protected virtual void OnClosing(ref bool cancel)
        {

        }
        /// <summary>
        /// get platform's native handle
        /// </summary>
        public override IntPtr Handle
        {
            get
            {
                CheckNativeHandle();
                return _nativePlatformHwnd;
            }
        }
        public override string Text
        {
            get
            {
                return this._windowTitle;
            }
            set
            {
                this._windowTitle = value;
                if (!_nativeGlFwWindowPtr.IsEmpty)
                {
                    //if not empty 
                    //set to native window title
                    Glfw.SetWindowTitle(this._nativeGlFwWindowPtr, value);
                }
            }
        }
        void CheckNativeHandle()
        {
            if (_nativeGlFwWindowPtr.IsEmpty)
            {
                //create native glfw window
                this._nativeGlFwWindowPtr = Glfw.CreateWindow(this.Width,
                    this.Height,
                    this.Text,
                    new GlfwMonitorPtr(),//default monitor
                    new GlfwWindowPtr()); //default top window 

            }
        }

        public override void Show()
        {
            CheckNativeHandle();
            base.Show();
        }
        public override int Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                base.Width = value;
                Glfw.SetWindowSize(this._nativeGlFwWindowPtr, value, this.Height);
            }
        }
        public override int Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                base.Height = value;
                Glfw.SetWindowSize(this._nativeGlFwWindowPtr, this.Width, value);
            }
        }
        public void MakeCurrent()
        {
            Glfw.MakeContextCurrent(this._nativeGlFwWindowPtr);
        }
        public void Activate()
        {
            GLFWPlatforms.MakeCurrentWindow(this._winInfo);
        }
        public void SetDrawFrameDelegate(SimpleAction drawFrameDel)
        {
            this.drawFrameDel = drawFrameDel;
        }
        public void DrawFrame()
        {
            if (drawFrameDel != null)
            {
                MakeCurrent();
                drawFrameDel();
            }
        }
    }
}