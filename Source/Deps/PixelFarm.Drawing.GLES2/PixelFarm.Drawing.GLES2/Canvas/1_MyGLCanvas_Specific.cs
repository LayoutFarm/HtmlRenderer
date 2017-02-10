//BSD, 2014-2017, WinterDev 

using System;
using System.Collections.Generic;
using PixelFarm.DrawingGL;
using PixelFarm.Drawing.Fonts;
 
namespace PixelFarm.Drawing.GLES2
{



    public partial class MyGLCanvas : Canvas, IDisposable
    {
        CanvasGL2d canvasGL2;
        bool isDisposed;
        Stack<Rectangle> clipRectStack = new Stack<Rectangle>();
        //-------------------------------
        GLCanvasPainter painter1;
        Rectangle currentClipRect;
        Color currentTextColor;
        InternalPen internalPen;
        InternalBrush internalBrush;
        
        internal MyGLCanvas(
            CanvasGL2d canvasGL2d,
            int left, int top,
            int width,
            int height)
        {
            this.canvasGL2 = canvasGL2d;
            painter1 = new GLCanvasPainter(canvasGL2d, width, height);



            this.left = left;
            this.top = top;
            this.right = left + width;
            this.bottom = top + height;
            currentClipRect = new Rectangle(0, 0, width, height);

            
            this.CurrentFont = new RequestFont("tahoma", 10);
            this.CurrentTextColor = Color.Black;

            internalPen = new InternalPen(Color.Black);
            internalBrush = new InternalBrush(Color.Black);
#if DEBUG
            debug_canvas_id = dbug_canvasCount + 1;
            dbug_canvasCount += 1;
#endif
            this.StrokeWidth = 1;
        }

        public override string ToString()
        {
            return "visible_clip?";
        }

        public override void CloseCanvas()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;
            //win32MemDc.Dispose();
            //win32MemDc = null;
            ReleaseUnManagedResource();
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (isDisposed)
            {
                return;
            }
            this.CloseCanvas();
        }

        void ClearPreviousStoredValues()
        {

            canvasGL2.SetCanvasOrigin(0, 0);
            this.canvasOriginX = 0;
            this.canvasOriginY = 0;
            this.clipRectStack.Clear();
        }

        void ReleaseUnManagedResource()
        {
            clipRectStack.Clear();
            currentClipRect = new Rectangle(0, 0, this.Width, this.Height);
#if DEBUG

            debug_releaseCount++;
#endif
        }

        int CanvasOrgX { get { return (int)this.canvasOriginX; } }
        int CanvasOrgY { get { return (int)this.canvasOriginY; } }


        //=====================================
        //static 
        static readonly int[] _charFit = new int[1];
        static readonly int[] _charFitWidth = new int[1000];

        //debug
#if DEBUG
        static class dbugCounter
        {
            public static int dbugDrawStringCount;
        }


        public override void dbug_DrawRuler(int x)
        {
            int canvas_top = this.top;
            int canvas_bottom = this.Bottom;
            for (int y = canvas_top; y < canvas_bottom; y += 10)
            {
                this.DrawText(y.ToString().ToCharArray(), x, y);
            }
        }
        public override void dbug_DrawCrossRect(Color color, Rectangle rect)
        {
            var prevColor = this.StrokeColor;
            this.StrokeColor = color;
            DrawLine(rect.Left, rect.Top, rect.Right, rect.Bottom);
            DrawLine(rect.Left, rect.Bottom, rect.Right, rect.Top);
            this.StrokeColor = prevColor;
        }

#endif

    }
}