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
            innerViewportKind = InnerViewportKind.GL;
            //---------------------------------------- 
            //1.
            this.wintop = wintop;


            switch (innerViewportKind)
            {
                case InnerViewportKind.GL:
                    {
                        LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.Start(); 
                        MyPlatformWindowBridgeOpenGL winBridge = new MyPlatformWindowBridgeOpenGL(wintop, userInputEvBridge);
                         
                        var myGLControl = new OpenTK.MyGLControl();
                        myGLControl.Dock = DockStyle.Fill;

                        this.Controls.Add(myGLControl);
                        //--------------------------------------- 
                       
                        winBridge.BindGLControl(myGLControl);
                        this.winBridge = winBridge;


                    } break;
                case InnerViewportKind.GdiPlus:
                default:
                    {
                        MyPlatformWindowBridgeGdiPlus winBridge = new MyPlatformWindowBridgeGdiPlus(wintop, userInputEvBridge);
                         
                        var mybasicSurfaceView = new BasicSurfaceView();
                        this.Controls.Add(mybasicSurfaceView);
                        //--------------------------------------- 
                        winBridge.BindWindowControl(mybasicSurfaceView); 
                        mybasicSurfaceView.Dock = DockStyle.Fill;
                        mybasicSurfaceView.InitRootGraphics(winBridge);
                        this.winBridge = winBridge;
                    } break;
            }
        }
        public GraphicsPlatform P
        {
            get { return wintop.Root.P; }
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
