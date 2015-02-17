// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.UI
{

    public partial class UISurfaceViewportControl : UserControl
    {


        TopWindowBridge winBridge;
        RootGraphic rootgfx;
        public UISurfaceViewportControl()
        {
            InitializeComponent();
        }
        public void InitRootGraphics(
            RootGraphic rootgfx,
            IUserEventPortal userInputEvBridge,
            InnerViewportKind innerViewportKind)
        {

            //1.
            this.rootgfx = rootgfx;

            switch (innerViewportKind)
            {
                case InnerViewportKind.GL:
                    {
                        PixelFarm.Drawing.DrawingGL.CanvasGLPortal.Start();


                        var bridge = new OpenGL.MyTopWindowBridgeOpenGL(rootgfx, userInputEvBridge);

                        var view = new OpenGL.GpuOpenGLSurfaceView();
                        view.Width = 800;
                        view.Height = 600;
                        //view.Dock = DockStyle.Fill;
                        this.Controls.Add(view);
                        //--------------------------------------- 
                        view.Bind(bridge);

                        this.winBridge = bridge;


                    } break;
                case InnerViewportKind.GdiPlus:
                default:
                    {
                        var bridge = new GdiPlus.MyTopWindowBridgeGdiPlus(rootgfx, userInputEvBridge);
                        var view = new GdiPlus.CpuGdiPlusSurfaceView();
                        view.Dock = DockStyle.Fill;
                        this.Controls.Add(view);
                        //--------------------------------------- 
                        view.Bind(bridge);

                        this.winBridge = bridge;
                    } break;
            }
        }
        public GraphicsPlatform P
        {
            get { return this.rootgfx.P; }
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
            this.winBridge.PaintMe();
        }
        public void PrintMe()
        {
            this.winBridge.PaintMeToPrinter();
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
            var layer0 = this.rootgfx.TopWindowRenderBox.Layer0 as PlainLayer;
            if (layer0 != null)
            {
                layer0.AddChild(vi);

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

    }



}
