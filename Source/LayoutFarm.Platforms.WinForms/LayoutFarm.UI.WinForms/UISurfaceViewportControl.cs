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
            //1.
            this.wintop = wintop;
            this.winBridge = new MyPlatformWindowBridge(wintop, userInputEvBridge);

            switch (innerViewportKind)
            {
                case InnerViewportKind.GL:
                    {
                        innerViewportKind = InnerViewportKind.GL;
                        myGLControl = new OpenTK.MyGLControl();
                        myGLControl.Dock = DockStyle.Fill;
                        this.Controls.Add(myGLControl);
                        this.winBridge.BindWindowControl(myGLControl);
                    } break;
                case InnerViewportKind.GdiPlus:
                default:
                    {
                        innerViewportKind = InnerViewportKind.GdiPlus;
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
        public void PaintMe()
        {
            switch (this.innerViewportKind)
            {
                case InnerViewportKind.GL:
                    {

                    } break;
                default:
                    {
                        this.winBridge.PaintMe();
                    } break;
            }
        }
    }



}
