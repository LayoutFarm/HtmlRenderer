//2014 BSD, WinterDev
//ArthurHub

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
namespace LayoutFarm
{

    partial class MyCanvas
    {
        public override void CopyFrom(Canvas sourceCanvas, int logicalSrcX, int logicalSrcY, Rectangle destArea)
        {
            MyCanvas s1 = (MyCanvas)sourceCanvas;

            if (s1.gx != null)
            {
                int phySrcX = logicalSrcX - s1.left;
                int phySrcY = logicalSrcY - s1.top;

                System.Drawing.Rectangle postIntersect =
                    System.Drawing.Rectangle.Intersect(currentClipRect, destArea.ToRect());
                phySrcX += postIntersect.X - destArea.X;
                phySrcY += postIntersect.Y - destArea.Y;
                destArea = postIntersect.ToRect();

                IntPtr gxdc = gx.GetHdc();

                MyWin32.SetViewportOrgEx(gxdc, CanvasOrgX, CanvasOrgY, IntPtr.Zero);
                IntPtr source_gxdc = s1.gx.GetHdc();
                MyWin32.SetViewportOrgEx(source_gxdc, s1.CanvasOrgX, s1.CanvasOrgY, IntPtr.Zero);


                MyWin32.BitBlt(gxdc, destArea.X, destArea.Y, destArea.Width, destArea.Height, source_gxdc, phySrcX, phySrcY, MyWin32.SRCCOPY);


                MyWin32.SetViewportOrgEx(source_gxdc, -s1.CanvasOrgX, -s1.CanvasOrgY, IntPtr.Zero);

                s1.gx.ReleaseHdc();

                MyWin32.SetViewportOrgEx(gxdc, -CanvasOrgX, -CanvasOrgY, IntPtr.Zero);
                gx.ReleaseHdc();



            }
        }
        public override void RenderTo(IntPtr destHdc, int sourceX, int sourceY, Rectangle destArea)
        {
            IntPtr gxdc = gx.GetHdc();
            MyWin32.SetViewportOrgEx(gxdc, CanvasOrgX, CanvasOrgY, IntPtr.Zero);
            MyWin32.BitBlt(destHdc, destArea.X, destArea.Y,
            destArea.Width, destArea.Height, gxdc, sourceX, sourceY, MyWin32.SRCCOPY);
            MyWin32.SetViewportOrgEx(gxdc, -CanvasOrgX, -CanvasOrgY, IntPtr.Zero);
            gx.ReleaseHdc();
        }
        public override void ClearSurface()
        {
            ReleaseHdc();
            gx.Clear(System.Drawing.Color.White);
        }



        //public override void FillPolygon(ArtColorBrush colorBrush, Point[] points)
        //{
        //    if (colorBrush is ArtSolidBrush)
        //    {
        //        ArtSolidBrush solidBrush = (ArtSolidBrush)colorBrush;
        //        gx.FillPolygon(ConvBrush(colorBrush.myBrush), ConvPointArray(points));

        //    }
        //    else if (colorBrush is ArtGradientBrush)
        //    {
        //        ArtGradientBrush gradientBrush = (ArtGradientBrush)colorBrush;

        //        gx.FillPolygon(ConvBrush(colorBrush.myBrush), ConvPointArray(points));


        //    }
        //    else if (colorBrush is ArtImageBrush)
        //    {
        //        ArtImageBrush imgBrush = (ArtImageBrush)colorBrush;


        //    }
        //}
        public override void FillRegion(Region rgn)
        {
            gx.FillRegion(internalBrush, ConvRgn(rgn));
        }

        public override void FillPath(GraphicsPath gfxPath )
        {
             
            gx.FillPath(internalBrush, gfxPath.InnerPath as System.Drawing.Drawing2D.GraphicsPath);
        }

        public override void DrawPath(GraphicsPath gfxPath)
        {
            gx.DrawPath(internalPen, gfxPath.InnerPath as System.Drawing.Drawing2D.GraphicsPath);
        }


       
       
        public override void FillRectangle(Brush brush, float left, float top, float width, float height)
        {
            gx.FillRectangle(ConvBrush(brush), left, top, width, height);
        }
       

        public override void FillRectangle(Color color, float left, float top, float right, float bottom)
        {
            ReleaseHdc();
            internalBrush.Color = ConvColor(color);
            gx.FillRectangle(internalBrush, left, top, right - left, bottom - top);
        }

        public override RectangleF GetBound(Region rgn)
        {
            return (ConvRgn(rgn).GetBounds(gx)).ToRectF();
        } 
     
        public override void DrawRectangle(Color color, float left, float top, float width, float height)
        {
            ReleaseHdc();
            internalPen.Color = ConvColor(color);
            gx.DrawRectangle(internalPen, left, top, width, height);
        }
        
        public override void DrawBezier(Point[] points)
        {
            gx.DrawBeziers(internalPen, ConvPointArray(points));
        }
        public override void DrawLine(float x1, float y1, float x2, float y2)
        {
            ReleaseHdc();
            gx.DrawLine(internalPen, x1, y1, x2, y2);
        }

        public override void DrawLine(PointF p1, PointF p2)
        {
            gx.DrawLine(internalPen, p1.X, p1.Y, p2.X, p2.Y);

        }

        public override void DrawLines(Point[] points)
        {

            gx.DrawLines(internalPen,
               ConvPointArray(points));
        }
        public override void DrawPolygon(PointF[] points)
        {
            gx.DrawPolygon(internalPen, ConvPointFArray(points));
        }

        public override void FillEllipse(Point[] points)
        {
            gx.FillEllipse(internalBrush, points[0].X, points[0].Y, points[2].X - points[0].X, points[2].Y - points[0].Y);

        }
         

        public override void FillEllipse(int x, int y, int width, int height)
        {
            gx.FillEllipse(internalBrush, x, y, width, height);
        }
        public override void DrawRoundRect(int x, int y, int w, int h, Size cornerSize)
        {

            int cornerSizeW = cornerSize.Width;
            int cornerSizeH = cornerSize.Height;

            System.Drawing.Drawing2D.GraphicsPath gxPath = new System.Drawing.Drawing2D.GraphicsPath();
            gxPath.AddArc(new System.Drawing.Rectangle(x, y, cornerSizeW * 2, cornerSizeH * 2), 180, 90);
            gxPath.AddLine(new System.Drawing.Point(x + cornerSizeW, y), new System.Drawing.Point(x + w - cornerSizeW, y));

            gxPath.AddArc(new System.Drawing.Rectangle(x + w - cornerSizeW * 2, y, cornerSizeW * 2, cornerSizeH * 2), -90, 90);
            gxPath.AddLine(new System.Drawing.Point(x + w, y + cornerSizeH), new System.Drawing.Point(x + w, y + h - cornerSizeH));

            gxPath.AddArc(new System.Drawing.Rectangle(x + w - cornerSizeW * 2, y + h - cornerSizeH * 2, cornerSizeW * 2, cornerSizeH * 2), 0, 90);
            gxPath.AddLine(new System.Drawing.Point(x + w - cornerSizeW, y + h), new System.Drawing.Point(x + cornerSizeW, y + h));

            gxPath.AddArc(new System.Drawing.Rectangle(x, y + h - cornerSizeH * 2, cornerSizeW * 2, cornerSizeH * 2), 90, 90);
            gxPath.AddLine(new System.Drawing.Point(x, y + cornerSizeH), new System.Drawing.Point(x, y + h - cornerSizeH));

            gx.FillPath(System.Drawing.Brushes.Yellow, gxPath);
            gx.DrawPath(System.Drawing.Pens.Red, gxPath);
            gxPath.Dispose();
        }


        /// <summary>
        /// Gets or sets the rendering quality for this <see cref="T:System.Drawing.Graphics"/>.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Drawing.Drawing2D.SmoothingMode"/> values.
        /// </returns>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public override SmoothingMode SmoothingMode
        {
            get
            {
                ReleaseHdc();
                return (SmoothingMode)(gx.SmoothingMode);
            }
            set
            {
                ReleaseHdc();
                gx.SmoothingMode = (System.Drawing.Drawing2D.SmoothingMode)value;
            }
        }
        /// <summary>
        /// Draws the specified portion of the specified <see cref="T:System.Drawing.Image"/> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image"><see cref="T:System.Drawing.Image"/> to draw. </param>
        /// <param name="destRect"><see cref="T:System.Drawing.RectangleF"/> structure that specifies the location and size of the drawn image. The image is scaled to fit the rectangle. </param>
        /// <param name="srcRect"><see cref="T:System.Drawing.RectangleF"/> structure that specifies the portion of the <paramref name="image"/> object to draw. </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="image"/> is null.</exception>
        public override void DrawImage(Image image, RectangleF destRect, RectangleF srcRect)
        {
            ReleaseHdc();
            gx.DrawImage(image.InnerImage as System.Drawing.Image,
                destRect.ToRectF(),
                srcRect.ToRectF(),
                System.Drawing.GraphicsUnit.Pixel);
        }

        /// <summary>
        /// Draws the specified <see cref="T:System.Drawing.Image"/> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image"><see cref="T:System.Drawing.Image"/> to draw. </param><param name="destRect"><see cref="T:System.Drawing.Rectangle"/> structure that specifies the location and size of the drawn image. </param><exception cref="T:System.ArgumentNullException"><paramref name="image"/> is null.</exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public override void DrawImage(Image image, RectangleF destRect)
        {
            ReleaseHdc();
            gx.DrawImage(image.InnerImage as System.Drawing.Image, destRect.ToRectF());
        }

        /// <summary>
        /// Draws a <see cref="T:System.Drawing.Drawing2D.GraphicsPath"/>.
        /// </summary>
        /// <param name="pen"><see cref="T:System.Drawing.Pen"/> that determines the color, width, and style of the path. </param><param name="path"><see cref="T:System.Drawing.Drawing2D.GraphicsPath"/> to draw. </param><exception cref="T:System.ArgumentNullException"><paramref name="pen"/> is null.-or-<paramref name="path"/> is null.</exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public void DrawPath(Pen pen, GraphicsPath path)
        {
            gx.DrawPath(pen.InnerPen as System.Drawing.Pen,
                path.InnerPath as System.Drawing.Drawing2D.GraphicsPath);
        }

        /// <summary>
        /// Fills the interior of a <see cref="T:System.Drawing.Drawing2D.GraphicsPath"/>.
        /// </summary>
        /// <param name="brush"><see cref="T:System.Drawing.Brush"/> that determines the characteristics of the fill. </param><param name="path"><see cref="T:System.Drawing.Drawing2D.GraphicsPath"/> that represents the path to fill. </param><exception cref="T:System.ArgumentNullException"><paramref name="brush"/> is null.-or-<paramref name="path"/> is null.</exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/></PermissionSet>
        public override void FillPath(GraphicsPath path, Brush brush)
        {
            ReleaseHdc();
            gx.FillPath(brush.InnerBrush as System.Drawing.Brush,
                path.InnerPath as System.Drawing.Drawing2D.GraphicsPath);
        }

        /// <summary>
        /// Fills the interior of a polygon defined by an array of points specified by <see cref="T:System.Drawing.PointF"/> structures.
        /// </summary>
        /// <param name="brush"><see cref="T:System.Drawing.Brush"/> that determines the characteristics of the fill. </param><param name="points">Array of <see cref="T:System.Drawing.PointF"/> structures that represent the vertices of the polygon to fill. </param><exception cref="T:System.ArgumentNullException"><paramref name="brush"/> is null.-or-<paramref name="points"/> is null.</exception>
        public override void FillPolygon(PointF[] points)
        {
            ReleaseHdc();
            //create Point
            var pps = ConvPointFArray(points);
            gx.FillPolygon(this.internalBrush, pps);
        }

#if DEBUG
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
            DrawLine(rect.Location, new Point(rect.Right, rect.Bottom));
            DrawLine(new Point(rect.Left, rect.Bottom), new Point(rect.Right, rect.Top));
            this.StrokeColor = prevColor;
        }

#endif
    }

}