//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using LayoutFarm.Drawing;


namespace LayoutFarm.UI.WinForms
{

    public partial class UISurfaceViewportControl : UserControl
    {
        InnerViewportKind innerViewportKind = InnerViewportKind.GdiPlus;
        BasicSurfaceView mybasicSurfaceView;
        OpenTK.MyGLControl myGLControl;
        Canvas canvas;
        bool isInitGLControl = false;

        TopWindowRenderBox wintop;
        MyPlatformWindowBridge winBridge;

        public UISurfaceViewportControl()
        {
            InitializeComponent();
        }

        public void InitRootGraphics(
            TopWindowRenderBox wintop,
            IUserEventPortal userInputEvBridge,
            InnerViewportKind innerViewportKind)
        {
            //test*** force  to use gl
            //innerViewportKind = InnerViewportKind.GL;
            //----------------------------------------


            //1.
            this.wintop = wintop;
            this.winBridge = new MyPlatformWindowBridge(wintop, userInputEvBridge);

            switch (innerViewportKind)
            {
                case InnerViewportKind.GL:
                    {
                        this.innerViewportKind = InnerViewportKind.GL;
                        myGLControl = new OpenTK.MyGLControl();
                        myGLControl.Dock = DockStyle.Fill;

                        this.Controls.Add(myGLControl);
                        //--------------------------------------- 
                        LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.Start();
                        this.canvas = LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.P.CreateCanvas(0, 0, this.Width, this.Height);
                        this.winBridge.BindWindowControl(myGLControl);

                    } break;
                case InnerViewportKind.GdiPlus:
                default:
                    {
                        this.innerViewportKind = InnerViewportKind.GdiPlus;
                        mybasicSurfaceView = new BasicSurfaceView();
                        this.Controls.Add(mybasicSurfaceView);


                        mybasicSurfaceView.Dock = DockStyle.Fill;
                        mybasicSurfaceView.InitRootGraphics(this.winBridge);
                    } break;
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

            if (this.innerViewportKind == InnerViewportKind.GL
                && !isInitGLControl)
            {
                //init gl after this control is loaded
                //set myGLControl detail
                //1.
                myGLControl.InitSetup2d(Screen.PrimaryScreen.Bounds);
                isInitGLControl = true;

                //2.
                myGLControl.ClearColor = new OpenTK.Graphics.Color4(1f, 1f, 1f, 1f);
                //3.
                myGLControl.SetGLPaintHandler(GLPaintMe);

            }
        }
        void GLPaintMe(Object sender, EventArgs e)
        {
            //gl paint here
            canvas.ClearSurface(Color.White);
            myGLControl.SwapBuffers();
        }
        public void PaintMe()
        {
            switch (this.innerViewportKind)
            {
                case InnerViewportKind.GL:
                    {
                        if (isInitGLControl)
                        {
                            GLPaintMe(null, null);
                        }
                    } break;
                default:
                    {
                        this.winBridge.PaintMe();
                    } break;
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
            wintop.TopDownReCalculateContentSize();
        }
        public void AddContent(RenderElement vi)
        {
            var layer0 = wintop.Layers.Layer0 as VisualPlainLayer;
            if (layer0 != null)
            {
                layer0.AddChild(vi);
                vi.InvalidateGraphic();
            }
        }

        public RootGraphic WinTopRootGfx
        {
            get
            {
                return this.wintop.Root;
            }
        }
        public void Close()
        {
            this.winBridge.Close();
        }

    }



}
