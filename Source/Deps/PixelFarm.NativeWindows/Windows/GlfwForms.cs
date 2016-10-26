//MIT, 2016, WinterDev
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
                    found.InvokeOnClosing(ref userCancel);
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
                    found.SetFocusState(focus);
                }
            };
            s_windowIconifyCb = (GlfwWindowPtr wnd, bool iconify) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    found.SetIconifyState(iconify);
                }
            };

            s_windowPosCb = (GlfwWindowPtr wnd, int x, int y) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    found.InvokeOnMove(x, y);
                }
            };
            s_windowRefreshCb = (GlfwWindowPtr wnd) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    found.InvokeOnRefresh();
                }
            };

            s_windowSizeCb = (GlfwWindowPtr wnd, int width, int height) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    found.InvokeOnSizeChanged(width, height);
                }
            };
            s_windowCursorPosCb = (GlfwWindowPtr wnd, double x, double y) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    found.InvokeCursorPos(x, y);
                }
            };
            s_windowCursorEnterCb = (GlfwWindowPtr wnd, bool enter) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    found.SetCursorEnterState(enter);
                }
            };
            s_windowMouseButtonCb = (GlfwWindowPtr wnd, MouseButton btn, KeyAction keyAction) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    found.InvokeMouseButton(btn, keyAction);
                }
            };
            s_scrollCb = (GlfwWindowPtr wnd, double xoffset, double yoffset) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    found.InvokeOnScroll(xoffset, yoffset);
                }
            };
            s_windowKeyCb = (GlfwWindowPtr wnd, Key key, int scanCode, KeyAction action, KeyModifiers mods) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    found.InvokeKey(key, scanCode, action, mods);
                }
            };
            s_windowCharCb = (GlfwWindowPtr wnd, char ch) =>
            {
                GlFwForm found;
                if (GetGlfwForm(wnd, out found))
                {
                    found.InvokeKeyPress(ch);
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
        public static GlFwForm CreateGlfwForm(int w, int h, string title)
        {
            GlfwWindowPtr glWindowPtr = Glfw.CreateWindow(w, h,
                title,
                new GlfwMonitorPtr(),//default monitor
                new GlfwWindowPtr()); //default top window 
            GlFwForm f = new GlFwForm(glWindowPtr, w, h);
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
    public class GlFwForm : Form
    {
        SimpleAction drawFrameDel;
        string _windowTitle = "";
        GlfwWindowPtr _nativeGlFwWindowPtr;
        IntPtr _nativePlatformHwnd;

        internal GlFwForm(GlfwWindowPtr glWindowPtr, int w, int h)
        {

            base.Width = w;
            base.Height = h;
            _nativeGlFwWindowPtr = glWindowPtr;
            _nativePlatformHwnd = Glfw.GetNativePlatformWinHwnd(glWindowPtr);
        }
        public GlfwWindowPtr GlfwWindowPtr
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

        internal void SetFocusState(bool focus)
        {
            if (focus)
            {
                OnFocus();
            }
            else
            {
                OnLostFocus();
            }
        }
        internal void InvokeKeyPress(char c)
        {
            //TODO: implement detail methods
            OnKeyPress(c);
        }
        internal void InvokeKey(Key key, int scanCode, KeyAction keyAction, KeyModifiers mods)
        {
            switch (keyAction)
            {
                case KeyAction.Press:
                    OnKeyDown(key, scanCode, mods);
                    break;
                case KeyAction.Repeat:
                    OnKeyRepeat(key, scanCode, mods);
                    break;
                case KeyAction.Release:
                    OnKeyUp(key, scanCode, mods);
                    break;
                default:
                    throw new NotFiniteNumberException();
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
        internal void InvokeOnScroll(double xoffset, double yoffset)
        {
            //TODO: implement detail methods
        }
        internal void SetIconifyState(bool iconify)
        {
            OnIconify(iconify);
        }
        internal void InvokeOnMove(int x, int y)
        {
            //on pos changed
            //TODO: implement detail methods
        }
        internal void InvokeOnSizeChanged(int w, int h)
        {
            //on pos changed
            //TODO: implement detail methods
        }
        internal void InvokeOnRefresh()
        {
            //TODO: implement detail methods
        }
        internal void InvokeCursorPos(double x, double y)
        {
            //TODO: implement detail methods
        }
        internal void InvokeMouseButton(MouseButton btn, KeyAction action)
        {
            //TODO: implement detail methods
        }
        internal void SetCursorEnterState(bool enter)
        {
            if (enter)
            {
                OnCursorEnter();
            }
            else
            {
                OnCursorLeave();
            }
        }
        protected virtual void OnCursorEnter()
        {

        }
        public virtual void OnCursorLeave()
        {

        }
        internal void InvokeOnClosing(ref bool cancel)
        {
            OnClosing(ref cancel);
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
                    this.Title,
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
        OpenTK.Graphics.GraphicsContext glfwContext;
        public void CreateOpenGLEsContext()
        {

            //make open gl es current context 
            GlfwWindowPtr currentContext = Glfw.GetCurrentContext();
            var contextHandler = new OpenTK.ContextHandle(currentContext.inner_ptr);
            glfwContext = OpenTK.Graphics.GraphicsContext.CreateDummyContext(contextHandler);
            bool isCurrent = glfwContext.IsCurrent;
            PixelFarm.GlfwWinInfo winInfo = new PixelFarm.GlfwWinInfo(_nativeGlFwWindowPtr.inner_ptr);
            glfwContext.MakeCurrent(winInfo);

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