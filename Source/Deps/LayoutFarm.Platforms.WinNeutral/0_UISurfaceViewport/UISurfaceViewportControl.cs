//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Forms;

namespace LayoutFarm.UI.WinNeutral
{

    public partial class UISurfaceViewportControl : Control
    {
        TopWindowBridgeWinNeutral winBridge;
        RootGraphic rootgfx;
        ITopWindowEventRoot topWinEventRoot;
        InnerViewportKind innerViewportKind;
        List<Form> subForms = new List<Form>();
        int width;
        int height;
        int left;
        int top;

        public UISurfaceViewportControl()
        {

        }
        public LayoutFarm.UI.UIPlatform Platform
        {
            get { return LayoutFarm.UI.UIPlatformWinNeutral.platform; }
        }
        public PixelFarm.Drawing.Size Size
        {
            get { return new PixelFarm.Drawing.Size(width, height); }
            set
            {
                this.width = value.Width;
                this.height = value.Height;
            }
        }
        public PixelFarm.Drawing.Rectangle Bounds
        {
            get { return new PixelFarm.Drawing.Rectangle(left, top, width, height); }
            set
            {
                this.left = value.Left;
                this.top = value.Top;
                this.width = value.Width;
                this.height = value.Height;
            }
        }
        public void SwapBuffers()
        {
        }
        public void MakeCurrent()
        {
        }
        public void InitSetup2d(PixelFarm.Drawing.Rectangle rect)
        {

        }
        public OpenTK.Graphics.Color4 ClearColor
        {
            get;
            set;
        }
        public void SetupCanvas(PixelFarm.Drawing.Canvas canvas)
        {
            bridge.SetupCanvas(canvas);
        }
        //
        OpenGL.MyTopWindowBridgeOpenGL bridge;
        public void InitRootGraphics(
            RootGraphic rootgfx,
            ITopWindowEventRoot topWinEventRoot,
            InnerViewportKind innerViewportKind)
        {
            //1.
            this.rootgfx = rootgfx;
            this.topWinEventRoot = topWinEventRoot;
            this.innerViewportKind = innerViewportKind;
            switch (innerViewportKind)
            {
                case InnerViewportKind.GL:
                    {

                        ////temp not suppport
                        //PixelFarm.Drawing.DrawingGL.CanvasGLPortal.Start();
                        bridge = new OpenGL.MyTopWindowBridgeOpenGL(rootgfx, topWinEventRoot);
                        bridge.BindWindowControl(this);
                        //var view = new OpenGL.GpuOpenGLSurfaceView();
                        //view.Width = 800;
                        //view.Height = 600;
                        ////view.Dock = DockStyle.Fill;
                        //this.Controls.Add(view);
                        ////--------------------------------------- 
                        //view.Bind(bridge);
                        this.winBridge = bridge;
                    }
                    break;
                case InnerViewportKind.Skia:
                    {

                        //skiasharp ***
                        var bridge = new Skia.MyTopWindowBridgeSkia(rootgfx, topWinEventRoot);
                        //var view = new CpuSurfaceView();
                        //view.Dock = DockStyle.Fill;
                        //this.Controls.Add(view);
                        ////--------------------------------------- 
                        //view.Bind(bridge);
                        this.winBridge = bridge;
                    }
                    break;
                case InnerViewportKind.GdiPlus:
                default:
                    {
                        throw new NotSupportedException();
                        //var bridge = new GdiPlus.MyTopWindowBridgeGdiPlus(rootgfx, topWinEventRoot);
                        //var view = new CpuSurfaceView();
                        //view.Dock = DockStyle.Fill;
                        //this.Controls.Add(view);
                        ////--------------------------------------- 
                        //view.Bind(bridge);
                        //this.winBridge = bridge;
                    }
                    break;
            }
        }


        //void InitializeComponent()
        //{
        //    this.SuspendLayout();
        //    this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        //    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        //    this.BackColor = System.Drawing.Color.White;
        //    this.Size = new System.Drawing.Size(387, 277);
        //    this.ResumeLayout(false);
        //}
        protected override void OnLoad(EventArgs e)
        {
            this.winBridge.OnHostControlLoaded();
        }
        public void PaintMe(PixelFarm.DrawingGL.CanvasGL2d canvasGL2d)
        {
            canvasGL2d.DrawLine(0, 0, 100, 100);
            //this.winBridge.PaintToOutputWindow();
        }
        public void PaintMe()
        {
            //this.winBridge.PaintToOutputWindow();
        }
        public void PaintMeFullMode()
        {
            this.winBridge.PaintToOutputWindowFullMode();
        }
        public void PrintMe(object targetCanvas)
        {
            //paint to external canvas    
            //var winBridge = (GdiPlus.MyTopWindowBridgeGdiPlus)this.winBridge;
            //if (winBridge != null)
            //{
            //    winBridge.PrintToCanvas(targetCanvas);
            //}
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
                        throw new NotSupportedException();
                        ////create platform winbox 
                        //var newForm = new AbstractCompletionWindow();
                        //newForm.LinkedParentForm = this.FindForm();
                        //newForm.LinkedParentControl = this;
                        //UISurfaceViewportControl newSurfaceViewport = this.CreateNewOne(300, 200);
                        //newSurfaceViewport.Location = new System.Drawing.Point(0, 0);
                        //newForm.Controls.Add(newSurfaceViewport);
                        //vi.ResetRootGraphics(newSurfaceViewport.RootGfx);
                        //vi.SetLocation(0, 0);
                        //newSurfaceViewport.AddContent(vi);
                        ////------------------------------------------------------                       
                        //var platformWinBox = new PlatformWinBoxForm(newForm);
                        //topWinBox.PlatformWinBox = platformWinBox;
                        //platformWinBox.UseRelativeLocationToParent = true;
                        //subForms.Add(newForm);
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
            newViewportControl.Size = new PixelFarm.Drawing.Size(w, h);

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
            //if (this.UseRelativeLocationToParent)
            //{
            //    //1. find parent form/control 
            //    var parentLoca = form.LinkedParentForm.Location;
            //    form.Location = new System.Drawing.Point(parentLoca.X + x, parentLoca.Y + y);
            //}
            //else
            //{
            //    form.Location = new System.Drawing.Point(x, y);
            //}
        }

        void IPlatformWindowBox.SetSize(int w, int h)
        {
            //form.Size = new System.Drawing.Size(w, h);
        }
    }
}
