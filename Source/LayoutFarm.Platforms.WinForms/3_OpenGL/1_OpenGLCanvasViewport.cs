// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.UI.OpenGL
{
    class OpenGLCanvasViewport : CanvasViewport
    {
        Canvas canvas;
        bool isClosed;
        public OpenGLCanvasViewport(RootGraphic root,
            Size viewportSize, int cachedPageNum)
            : base(root, viewportSize, cachedPageNum)
        {
        }
        protected override void OnClosing()
        {
            isClosed = true;
            if (canvas != null)
            {

                canvas.CloseCanvas();
                canvas = null;
            }
        }
        public override void CanvasInvlidateArea(Rectangle r)
        {
        }
        internal void NotifyWindowControlBinding()
        {
            this.canvas = PixelFarm.Drawing.DrawingGL.CanvasGLPortal.P.CreateCanvas(0, 0, 800, 600);

        }
        //----------
        //for test
#if DEBUG
        void dbugTest01()
        {
            canvas.Orientation = CanvasOrientation.LeftTop;
            canvas.ClearSurface(Color.White);

            canvas.FillRectangle(Color.Red, 50, 50, 100, 100);

            dbugGLOffsetCanvasOrigin(50, 50);
            //simulate draw content 
            canvas.FillRectangle(Color.Gray, 10, 10, 80, 80);
            dbugGLOffsetCanvasOrigin(-50, -50);
        }
        void dbugGLSetCanvasOrigin(int x, int y)
        {
            canvas.SetCanvasOrigin(x, y);
            //int properW = Math.Min(canvas.Width, canvas.Height);
            ////int max = 600;
            ////init             
            ////---------------------------------
            ////-1 temp fix split scanline in some screen
            ////OpenTK.Graphics.OpenGL.GL.Viewport(x, y, properW, properW - 1);
            //////--------------------------------- 
            //OpenTK.Graphics.OpenGL.GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            //OpenTK.Graphics.OpenGL.GL.LoadIdentity();
            //OpenTK.Graphics.OpenGL.GL.Ortho(0, properW, properW, 0, 0.0, 100);

            ////switch (this.orientation)
            ////{
            ////    case Drawing.CanvasOrientation.LeftTop:
            ////        {
            ////            OpenTK.Graphics.OpenGL.GL.Ortho(0, properW, properW, 0, 0.0, 100);
            ////        } break;
            ////    default:
            ////        {
            ////            OpenTK.Graphics.OpenGL.GL.Ortho(0, properW, 0, properW, 0.0, 100);
            ////        } break;
            ////}
            //OpenTK.Graphics.OpenGL.GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview);
            //OpenTK.Graphics.OpenGL.GL.LoadIdentity();
            //OpenTK.Graphics.OpenGL.GL.Translate(x, y, 0);
        }
        void dbugGLOffsetCanvasOrigin(int dx, int dy)
        {
            dbugGLSetCanvasOrigin(canvas.CanvasOriginX + dx, canvas.CanvasOriginY + dy);
        }
#endif
        //-------

        public void PaintMe()
        {
            if (isClosed) return;
            //---------------------------------------------

            canvas.Orientation = CanvasOrientation.LeftTop;
            //Test01(); 
            //return;
            //Test01();
            //return;
            //canvas.ClearSurface(Color.White);
            //canvas.FillRectangle(Color.Red, 20, 20, 200, 400);
            // return;
            //----------------------------------
            //gl paint here

            canvas.ClearSurface(Color.White);
            ////test draw rect
            //canvas.StrokeColor = PixelFarm.Drawing.Color.Blue;
            //canvas.DrawRectangle(Color.Blue, 20, 20, 200, 200);
            ////------------------------

            if (this.IsClosed) { return; }
            //------------------------------------ 

            this.rootGraphics.PrepareRender();
            //---------------
            this.rootGraphics.IsInRenderPhase = true;
#if DEBUG
            this.rootGraphics.dbug_rootDrawingMsg.Clear();
            this.rootGraphics.dbug_drawLevel = 0;
#endif
            UpdateAllArea(this.canvas, this.topWindowBox);

            this.rootGraphics.IsInRenderPhase = false;
#if DEBUG

            RootGraphic visualroot = RootGraphic.dbugCurrentGlobalVRoot;
            if (visualroot.dbug_RecordDrawingChain)
            {
                List<dbugLayoutMsg> outputMsgs = dbugOutputWindow.dbug_rootDocDebugMsgs;
                outputMsgs.Clear();
                outputMsgs.Add(new dbugLayoutMsg(null as RenderElement, "[" + debug_render_to_output_count + "]"));
                visualroot.dbug_DumpRootDrawingMsg(outputMsgs);
                dbugOutputWindow.dbug_InvokeVisualRootDrawMsg();
                debug_render_to_output_count++;
            }


            if (dbugHelper01.dbugVE_HighlightMe != null)
            {
                dbugOutputWindow.dbug_HighlightMeNow(dbugHelper01.dbugVE_HighlightMe.dbugGetGlobalRect());

            }
#endif
        }

        static void UpdateAllArea(Canvas mycanvas, IRenderElement topWindowRenderBox)
        {

            mycanvas.OffsetCanvasOrigin(-mycanvas.Left, -mycanvas.Top);
            Rectangle rect = mycanvas.Rect;
            //mycanvas.FillRectangle(Color.Blue, 50, 50, 100, 100);
            topWindowRenderBox.DrawToThisCanvas(mycanvas, rect);

#if DEBUG
            topWindowRenderBox.dbugShowRenderPart(mycanvas, rect);

#endif

            mycanvas.IsContentReady = true;
            mycanvas.OffsetCanvasOrigin(mycanvas.Left, mycanvas.Top);
        }
    }

}