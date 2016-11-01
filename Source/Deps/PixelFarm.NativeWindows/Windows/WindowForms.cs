//MIT, 2016, WinterDev
using System;
namespace PixelFarm.Forms
{
    public static class Application
    {
        public static void EnableVisualStyles() { }
        public static void SetCompatibleTextRenderingDefault(bool value) { }
        public static event EventHandler Idle;
        public static void Run(Form form) { }
        public static void Run(ApplicationContext appContext) { }
    }
    public delegate void SimpleAction();
    public class Timer
    {
        public void Dispose() { }
        public bool Enabled { get; set; }
        public int Interval { get; set; }
        public event EventHandler Tick;
    }
    public class FormClosedEventArgs : EventArgs { }
    public class PreviewKeyEventArgs : EventArgs { }
    public class ApplicationContext
    {
        Form mainForm;
        public ApplicationContext() { }
        public ApplicationContext(Form mainForm)
        {
            this.mainForm = mainForm;
        }

    }
    public class Form : Control
    {
        IntPtr _handle;
        public Form()
        {
            CreateNativeCefWindowHandle();
        }
        internal Form(IntPtr hwnd)
        {
            this._handle = hwnd;
        }
        public override IntPtr Handle
        {
            get
            {
                return _handle;
            }
        }
        void CreateNativeCefWindowHandle()
        {

        }
        public void Invoke(Delegate ac) { }
        public virtual void Close() { }
        public event EventHandler<FormClosingEventArgs> FormClosing;
        public event EventHandler<FormClosedEventArgs> FormClosed;

        public static new Form CreateFromNativeWindowHwnd(IntPtr hwnd)
        {
            Form newControl = new Form(hwnd);
            newControl.TopLevelControl = newControl;
            return newControl;
        }
        public string Title
        {
            get;
            set;
        }
    }



    public class ControlCollection
    {
        Control owner;
        internal ControlCollection(Control owner)
        {
            this.owner = owner;
        }
        public void Add(Control c)
        {
        }
        public void Remove(Control c)
        {

        }
        public void Clear()
        {
        }
    }

    public class Control : IDisposable
    {
        int _width;
        int _height;
        IntPtr _nativeHandle;
        public Control()
        {

        }
        internal Control(IntPtr nativeHwnd)
        {
            this._nativeHandle = nativeHwnd;
        }
        protected bool DesignMode { get; set; }
        protected virtual void OnHandleCreated(EventArgs e)
        {
        }
        public virtual void Show()
        {
        }
        protected virtual void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
        }
        protected virtual void OnSizeChanged(EventArgs e)
        {
        }
        public ControlCollection Controls
        {
            get;
            set;
        }
        public virtual int Width
        {
            get { return this._width; }
            set
            {
                this._width = value;
            }
        }
        public virtual int Height
        {
            get { return this._height; }
            set
            {
                this._height = value;
            }
        }
        public bool IsHandleCreated { get; set; }
        public virtual IntPtr Handle
        {
            get
            {
                return _nativeHandle;
            }
        }
        public void Dispose() { }
        public void SetSize(int w, int h)
        {
        }
        public bool Visible { get; set; }
        public virtual string Text { get; set; }

        public virtual Control TopLevelControl
        {
            get;
            set;
        }
        public Control Parent { get; set; }
        public static Control CreateFromNativeWindowHwnd(IntPtr hwnd)
        {
            Control newControl = new Control(hwnd);             
            return newControl;
        }
    }
    public class FormClosingEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }

    public class PreviewKeyDownEventArgs : EventArgs { }
}