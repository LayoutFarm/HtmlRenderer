//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PixelFarm.DrawingGL;

namespace LayoutFarm.UI
{
    public partial class UISurfaceViewportControl : UserControl
    {
        TopWindowBridgeWinForm winBridge;
        RootGraphic rootgfx;
        ITopWindowEventRoot topWinEventRoot;
        InnerViewportKind innerViewportKind;
        List<Form> subForms = new List<Form>();


        //init once***
        static readonly UIPlatformWinForm winPlatform = UIPlatformWinForm.platform;

        public UISurfaceViewportControl()
        {
            InitializeComponent();
        }
        public UIPlatform Platform
        {
            get
            {
                return winPlatform;
            }
        }

        IntPtr hh1;
        OpenGL.GpuOpenGLSurfaceView openGLSurfaceView;
        CanvasGL2d canvas2d;
        GLCanvasPainter canvasPainter;

        void HandleGLPaint(object sender, System.EventArgs e)
        {
            //canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            //canvas2d.StrokeColor = PixelFarm.Drawing.Color.Black;
            //canvas2d.ClearColorBuffer();
            ////example
            //canvasPainter.FillColor = PixelFarm.Drawing.Color.Black;
            //canvasPainter.FillRectLBWH(20, 20, 150, 150);
            ////load bmp image 
            //////------------------------------------------------------------------------- 
            ////if (exampleBase != null)
            ////{
            ////    exampleBase.Draw(canvasPainter);
            ////}
            ////draw data 

            //openGLSurfaceView.SwapBuffers();
        }
        public void InitRootGraphics(
            RootGraphic rootgfx,
            ITopWindowEventRoot topWinEventRoot,
            InnerViewportKind innerViewportKind)
        {

            //create a proper bridge****

            this.rootgfx = rootgfx;
            this.topWinEventRoot = topWinEventRoot;
            this.innerViewportKind = innerViewportKind;
            switch (innerViewportKind)
            {
                case InnerViewportKind.GL:
                    {
                        //temp not suppport  
                        //TODO: review here
                        //PixelFarm.Drawing.DrawingGL.CanvasGLPortal.Start();

                        var bridge = new OpenGL.MyTopWindowBridgeOpenGL(rootgfx, topWinEventRoot);
                        var view = new OpenGL.GpuOpenGLSurfaceView();

                        view.Width = 800;
                        view.Height = 600;
                        openGLSurfaceView = view;
                        //view.Dock = DockStyle.Fill;
                        this.Controls.Add(view);
                        //--------------------------------------- 
                        view.Bind(bridge);
                        this.winBridge = bridge;
                        //--------------------------------------- 
                        view.SetGLPaintHandler(HandleGLPaint);
                        hh1 = view.Handle; //force real window handle creation
                        view.MakeCurrent();
                        //int max = Math.Max(this.Width, this.Height);
                        int max = Math.Max(view.Width, view.Height);
                        canvas2d = PixelFarm.Drawing.GLES2.GLES2Platform.CreateCanvasGL2d(max, max);
                        //---------------
                        //canvas2d.FlipY = true;//
                        //---------------
                        canvasPainter = new GLCanvasPainter(canvas2d, max, max);

                        //canvasPainter.SmoothingMode = PixelFarm.Drawing.SmoothingMode.HighQuality;
                        //----------------------
                        //1. win gdi based
                        //var printer = new WinGdiFontPrinter(canvas2d, view.Width, view.Height);
                        //canvasPainter.TextPrinter = printer;
                        //----------------------
                        //2. raw vxs
                        //var printer = new PixelFarm.Drawing.Fonts.VxsTextPrinter(canvasPainter);
                        //canvasPainter.TextPrinter = printer;
                        //----------------------
                        //3. agg texture based font texture
                        //var printer = new AggTextSpanPrinter(canvasPainter, 400, 50);
                        //printer.HintTechnique = Typography.Rendering.HintTechnique.TrueTypeInstruction_VerticalOnly;
                        //printer.UseSubPixelRendering = true;
                        //canvasPainter.TextPrinter = printer;

                        var printer = new GLBmpGlyphTextPrinter(canvasPainter, YourImplementation.BootStrapOpenGLES2.myFontLoader); 
                        canvasPainter.TextPrinter = printer;

                        //
                        //var myGLCanvas1 = new PixelFarm.Drawing.GLES2.MyGLCanvas(canvasPainter, 0, 0, view.Width, view.Height);
                        var myGLCanvas1 = new PixelFarm.Drawing.GLES2.MyGLCanvas(canvasPainter, 0, 0, view.Width, view.Height);
                        bridge.SetCanvas(myGLCanvas1);

                    }
                    break;
#if __SKIA__
                case InnerViewportKind.Skia:
                    {
                        //skiasharp ***

                        var bridge = new Skia.MyTopWindowBridgeSkia(rootgfx, topWinEventRoot);
                        var view = new CpuSurfaceView();
                        view.Dock = DockStyle.Fill;
                        this.Controls.Add(view);
                        //--------------------------------------- 
                        view.Bind(bridge);
                        this.winBridge = bridge;

                    }
                    break;
#endif
                case InnerViewportKind.GdiPlus:
                default:
                    {
                        var bridge = new GdiPlus.MyTopWindowBridgeGdiPlus(rootgfx, topWinEventRoot);
                        var view = new CpuSurfaceView();
                        view.Dock = DockStyle.Fill;
                        this.Controls.Add(view);
                        //--------------------------------------- 
                        view.Bind(bridge);
                        this.winBridge = bridge;
                    }
                    break;
            }
        }


        void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Size = new System.Drawing.Size(387, 277);
            this.ResumeLayout(false);
        }
        protected override void OnLoad(EventArgs e)
        {
            this.winBridge.OnHostControlLoaded();
        }
        public void PaintMe()
        {
            this.winBridge.PaintToOutputWindow();
        }
        public void PaintMeFullMode()
        {
            this.winBridge.PaintToOutputWindowFullMode();
        }
        public void PrintMe(PixelFarm.Drawing.WinGdi.MyGdiPlusCanvas targetCanvas)
        {
            //paint to external canvas    
            var winBridge = (GdiPlus.MyTopWindowBridgeGdiPlus)this.winBridge;
            if (winBridge != null)
            {
                winBridge.PrintToCanvas(targetCanvas);
            }
        }
#if DEBUG
        public IdbugOutputWindow IdebugOutputWin
        {
            get { return this.winBridge; }
        }
#endif
        public void TopDownRecalculateContent()
        {
            this.rootgfx.TopWindowRenderBox.TopDownReCalculateContentSize();
        }
        public void AddContent(RenderElement vi)
        {
            this.rootgfx.TopWindowRenderBox.AddChild(vi);
        }

        public void AddContent(RenderElement vi, object owner)
        {
            if (vi is RenderBoxBase)
            {
                if (owner is ITopWindowBox)
                {
                    var topWinBox = owner as ITopWindowBox;
                    if (topWinBox.PlatformWinBox == null)
                    {
                        //create platform winbox 
                        var newForm = new AbstractCompletionWindow();
                        newForm.LinkedParentForm = this.FindForm();
                        newForm.LinkedParentControl = this;
                        UISurfaceViewportControl newSurfaceViewport = this.CreateNewOne(300, 200);
                        newSurfaceViewport.Location = new System.Drawing.Point(0, 0);
                        newForm.Controls.Add(newSurfaceViewport);
                        vi.ResetRootGraphics(newSurfaceViewport.RootGfx);
                        vi.SetLocation(0, 0);
                        newSurfaceViewport.AddContent(vi);
                        //------------------------------------------------------                       
                        var platformWinBox = new PlatformWinBoxForm(newForm);
                        topWinBox.PlatformWinBox = platformWinBox;
                        platformWinBox.UseRelativeLocationToParent = true;
                        subForms.Add(newForm);
                    }
                }
                else
                {
                    this.rootgfx.TopWindowRenderBox.AddChild(vi);
                }
            }
            else
            {
                this.rootgfx.TopWindowRenderBox.AddChild(vi);
            }
        }

        public RootGraphic RootGfx
        {
            get
            {
                return this.rootgfx;
            }
        }
        public void Close()
        {
            this.winBridge.Close();
        }


        /// <summary>
        /// create new UIViewport based on this control's current platform
        /// </summary>
        /// <returns></returns>
        public UISurfaceViewportControl CreateNewOne(int w, int h)
        {
            //each viewport has its own root graphics 

            UISurfaceViewportControl newViewportControl = new UISurfaceViewportControl();
            newViewportControl.Size = new System.Drawing.Size(w, h);
            RootGraphic newRootGraphic = this.rootgfx.CreateNewOne(w, h);
            ITopWindowEventRoot topEventRoot = null;
            if (newRootGraphic is ITopWindowEventRootProvider)
            {
                topEventRoot = ((ITopWindowEventRootProvider)newRootGraphic).EventRoot;
            }
            newViewportControl.InitRootGraphics(
                newRootGraphic,//new root
                topEventRoot,
                this.innerViewportKind);
            return newViewportControl;
        }
        //-----------
    }

    class PlatformWinBoxForm : IPlatformWindowBox
    {
        AbstractCompletionWindow form;
        public PlatformWinBoxForm(AbstractCompletionWindow form)
        {
            this.form = form;
        }
        public bool UseRelativeLocationToParent
        {
            get;
            set;
        }
        bool IPlatformWindowBox.Visible
        {
            get
            {
                return form.Visible;
            }
            set
            {
                if (value)
                {
                    if (!form.Visible)
                    {
                        form.ShowForm();
                    }
                }
                else
                {
                    if (form.Visible)
                    {
                        form.Hide();
                    }
                }
            }
        }
        void IPlatformWindowBox.Close()
        {
            form.Close();
            form = null;
        }

        void IPlatformWindowBox.SetLocation(int x, int y)
        {
            if (this.UseRelativeLocationToParent)
            {
                //1. find parent form/control 
                var parentLoca = form.LinkedParentForm.Location;
                form.Location = new System.Drawing.Point(parentLoca.X + x, parentLoca.Y + y);
            }
            else
            {
                form.Location = new System.Drawing.Point(x, y);
            }
        }

        void IPlatformWindowBox.SetSize(int w, int h)
        {
            form.Size = new System.Drawing.Size(w, h);
        }
    }
}
