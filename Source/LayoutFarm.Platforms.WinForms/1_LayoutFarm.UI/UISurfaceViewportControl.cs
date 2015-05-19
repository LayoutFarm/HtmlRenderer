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
        List<Form> subForms = new List<Form>();
        public UISurfaceViewportControl()
        {
            InitializeComponent();
        }

        public void InitRootGraphics(
            RootGraphic rootgfx,
            ITopWindowEventRoot topWinEventRoot,
            InnerViewportKind innerViewportKind)
        {

            //1.
            this.rootgfx = rootgfx;
            switch (innerViewportKind)
            {
                case InnerViewportKind.GL:
                    {
                        PixelFarm.Drawing.DrawingGL.CanvasGLPortal.Start();

                        var bridge = new OpenGL.MyTopWindowBridgeOpenGL(rootgfx, topWinEventRoot);
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
                        var bridge = new GdiPlus.MyTopWindowBridgeGdiPlus(rootgfx, topWinEventRoot);
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
            this.winBridge.PaintToOutputWindow();
        }
        public void PaintMeFullMode()
        {
            this.winBridge.PaintToOutputWindowFullMode();
        }
        public void PrintMe(Canvas targetCanvas)
        {
            this.winBridge.PaintToCanvas(targetCanvas);
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
            var me = this as IPlatformWindowBox;
            if (me.UseRelativeLocationToParent)
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
