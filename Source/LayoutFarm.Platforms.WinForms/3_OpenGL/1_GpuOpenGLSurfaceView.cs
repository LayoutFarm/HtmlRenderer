using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LayoutFarm.UI.OpenGL
{
    //app specific
    partial class GpuOpenGLSurfaceView : GLControl
    {
        MyTopWindowBridgeOpenGL winBridge;
        OpenTK.Graphics.Color4 clearColor;

        //EventHandler glPaintHandler;
        public GpuOpenGLSurfaceView()
        {
            OpenTK.Graphics.GraphicsMode gfxmode = new OpenTK.Graphics.GraphicsMode(
             DisplayDevice.Default.BitsPerPixel,//default 32 bits color
             16,//depth buffer => 16
             8,  //stencil buffer => 8 (  //if want to use stencil buffer then set stencil buffer too! )
             0,//number of sample of FSAA
             0,  //accum buffer
             2, // n buffer, 2=> double buffer
             false);//sterio
            ChildCtorOnlyResetGraphicMode(gfxmode);

            //-----------
            this.InitializeComponent();
        }
        public void Bind(MyTopWindowBridgeOpenGL winBridge)
        {
            //1. 
            this.winBridge = winBridge;
            this.winBridge.BindGLControl(this);
             
        }

        public OpenTK.Graphics.Color4 ClearColor
        {
            get { return clearColor; }
            set
            {
                clearColor = value;

                if (!this.DesignMode)
                {
                    MakeCurrent();
                    GL.ClearColor(
                        (float)clearColor.R / 255f,
                        (float)clearColor.G / 255f,
                        (float)clearColor.B / 255f,
                        (float)clearColor.A / 255f);
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            //------------------------------------------
            if (!this.DesignMode)
            {
                //MakeCurrent();
                ////winBridge.PaintMe();
                //////---------
                //////auto clear color ?
                //GL.ClearColor(1f, 1f, 1f, 1f);
                //GL.Clear(ClearBufferMask.ColorBufferBit);
                ////if (glPaintHandler != null)
                ////{
                ////    glPaintHandler(this, e);
                ////}
                //SwapBuffers();


            }
            else
            {
                base.OnPaint(e);
            }
        }
        
        public void InitSetup2d(Rectangle screenBound)
        {

            int properW = Math.Min(this.Width, this.Height);

            //init
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            //---------------------------------
            //-1 temp fix split scanline in some screen
            GL.Viewport(0, 0, properW, properW - 1);
            //---------------------------------

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            //origin on left-bottom
            GL.Ortho(0, properW, 0, properW, 0.0, 100);
            //GL.Ortho(0, properW, properW, 0, 0.0, 100);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        //-----------------------------
        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.winBridge != null)
            {
                this.winBridge.UpdateCanvasViewportSize(this.Width, this.Height);
            }
            base.OnSizeChanged(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.winBridge.HandleMouseEnterToViewport();
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            this.winBridge.HandleMouseLeaveFromViewport();
            base.OnMouseLeave(e);
        }
        //-----------------------------------------------------------------------------
        protected override void OnGotFocus(EventArgs e)
        {
            this.winBridge.HandleGotFocus(e);
            base.OnGotFocus(e);

        }
        protected override void OnLostFocus(EventArgs e)
        {
            this.winBridge.HandleGotFocus(e);
            base.OnLostFocus(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.winBridge.HandleMouseDown(e);
            base.OnMouseDown(e);

        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.winBridge.HandleMouseMove(e);
            base.OnMouseMove(e);

        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.winBridge.HandleMouseUp(e);
            base.OnMouseUp(e);

        } 
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            this.winBridge.HandleMouseWheel(e);
            base.OnMouseWheel(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            this.winBridge.HandleKeyDown(e);
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            this.winBridge.HandleKeyUp(e);
            base.OnKeyUp(e);
        }
        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            this.winBridge.HandleKeyPress(e);
            return;
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (this.winBridge.HandleProcessDialogKey(keyData))
            {
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

    }
}
