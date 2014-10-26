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


        public override SolidBrush GetSharedSolidBrush()
        {
            if (sharedSolidBrush == null)
            {
                sharedSolidBrush = platform.CreateSolidBrush(Color.Black);// new System.Drawing.SolidBrush(Color.Black);
            }
            return sharedSolidBrush;
        }

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


        public override void ClearSurface(Rect rect)
        {
            PushClipArea(rect._left, rect._top, rect.Width, rect.Height);
            gx.Clear(System.Drawing.Color.White); PopClipArea();
        }
        public override void ClearSurface()
        {
            gx.Clear(System.Drawing.Color.White);
        }

        public override void FillPolygon(Brush brush, Point[] points)
        {

            gx.FillPolygon(ConvBrush(brush), ConvPointArray(points));
        }

        public override void FillPolygon(ArtColorBrush colorBrush, Point[] points)
        {
            if (colorBrush is ArtSolidBrush)
            {
                ArtSolidBrush solidBrush = (ArtSolidBrush)colorBrush;
                gx.FillPolygon(ConvBrush(colorBrush.myBrush), ConvPointArray(points));

            }
            else if (colorBrush is ArtGradientBrush)
            {
                ArtGradientBrush gradientBrush = (ArtGradientBrush)colorBrush;

                gx.FillPolygon(ConvBrush(colorBrush.myBrush), ConvPointArray(points));


            }
            else if (colorBrush is ArtImageBrush)
            {
                ArtImageBrush imgBrush = (ArtImageBrush)colorBrush;


            }
        }
        public override void FillRegion(ArtColorBrush colorBrush, Region rgn)
        {
            if (colorBrush is ArtSolidBrush)
            {
                ArtSolidBrush solidBrush = (ArtSolidBrush)colorBrush;
                gx.FillRegion(ConvBrush(solidBrush.myBrush), ConvRgn(rgn));

            }
            else if (colorBrush is ArtGradientBrush)
            {
                ArtGradientBrush gradientBrush = (ArtGradientBrush)colorBrush;
                gx.FillRegion(ConvBrush(colorBrush.myBrush), ConvRgn(rgn));

            }
            else if (colorBrush is ArtImageBrush)
            {
                ArtImageBrush imgBrush = (ArtImageBrush)colorBrush;
            }
        }

        public override void FillPath(GraphicsPath gfxPath, Color solidColor)
        {

            FillPath(gfxPath, new ArtSolidBrush(solidColor));
        }
        public override void FillPath(GraphicsPath gfxPath, Brush colorBrush)
        {
            gx.FillPath(ConvBrush(colorBrush), ConvPath(gfxPath));
        }
        public override void FillPath(GraphicsPath gfxPath, ArtColorBrush colorBrush)
        {
            gx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            if (colorBrush is ArtSolidBrush)
            {
                ArtSolidBrush solidBrush = (ArtSolidBrush)colorBrush;
                if (solidBrush.myBrush == null)
                {
                    solidBrush.myBrush = this.platform.CreateSolidBrush(solidBrush.Color);
                }
                gx.FillPath(ConvBrush(solidBrush.myBrush), ConvPath(gfxPath));

            }
            else if (colorBrush is ArtGradientBrush)
            {
                ArtGradientBrush gradientBrush = (ArtGradientBrush)colorBrush;
                gx.FillPath(ConvBrush(gradientBrush.myBrush), ConvPath(gfxPath));

            }
            else if (colorBrush is ArtImageBrush)
            {
                ArtImageBrush imgBrush = (ArtImageBrush)colorBrush;
            }

        }
        public override void DrawPath(GraphicsPath gfxPath)
        {
            gx.DrawPath(internalPen, gfxPath.InnerPath as System.Drawing.Drawing2D.GraphicsPath);
        }
        public override void DrawPath(GraphicsPath gfxPath, Color color)
        {
            internalPen.Color = ConvColor(color);
            internalPen.Alignment = System.Drawing.Drawing2D.PenAlignment.Right;
            gx.DrawPath(internalPen, ConvPath(gfxPath));
        }
        public override void DrawPath(GraphicsPath gfxPath, Pen pen)
        {
            gx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            gx.DrawPath(ConvPen(pen), ConvPath(gfxPath));
        }
        public override void FillRectangle(Color color, Rectangle rect)
        {
            internalBrush.Color = ConvColor(color);

            gx.FillRectangle(internalBrush, rect.ToRect());
        }
        public override void FillRectangle(Color color, RectangleF rectf)
        {
            internalBrush.Color = ConvColor(color);
            gx.FillRectangle(internalBrush, rectf.ToRectF());
        }
        public override void FillRectangle(Brush brush, Rectangle rect)
        {
            gx.FillRectangle(ConvBrush(brush), rect.ToRect());
        }
        public override void FillRectangle(Brush brush, RectangleF rectf)
        {
            gx.FillRectangle(ConvBrush(brush), rectf.ToRectF());
        }
        public override void FillRectangle(ArtColorBrush brush, RectangleF rectf)
        {

            gx.FillRectangle(ConvBrush(brush.myBrush), rectf.ToRectF());
        }
        public override void FillRectangle(Color color, int left, int top, int right, int bottom)
        {
            internalBrush.Color = ConvColor(color);
            gx.FillRectangle(internalBrush, left, top, right - left, bottom - top);
        }
        public override float GetBoundWidth(Region rgn)
        {
            return ConvRgn(rgn).GetBounds(gx).Width;

        }
        public override RectangleF GetBound(Region rgn)
        {
            return (ConvRgn(rgn).GetBounds(gx)).ToRectF();

        }


        public override void FillRectangle(ArtColorBrush colorBrush, int left, int top, int right, int bottom)
        {
            if (colorBrush is ArtSolidBrush)
            {
                ArtSolidBrush solidBrush = (ArtSolidBrush)colorBrush;

                if (solidBrush.myBrush == null)
                {
                    solidBrush.myBrush = this.platform.CreateSolidBrush(solidBrush.Color);
                }
                gx.FillRectangle(solidBrush.myBrush.InnerBrush as System.Drawing.Brush,
                    System.Drawing.Rectangle.FromLTRB(left, top, right, bottom));

            }
            else if (colorBrush is ArtGradientBrush)
            {
                ArtGradientBrush gradientBrush = (ArtGradientBrush)colorBrush;
                gx.FillRectangle(gradientBrush.myBrush.InnerBrush as System.Drawing.Brush,
                    System.Drawing.Rectangle.FromLTRB(left, top, right, bottom));

            }
            else if (colorBrush is ArtImageBrush)
            {
                ArtImageBrush imgBrush = (ArtImageBrush)colorBrush;

                if (imgBrush.MyImage != null)
                {

                    gx.DrawImageUnscaled(ConvBitmap(imgBrush.MyImage), 0, 0);
                }
            }
        }
        public override void DrawRectangle(Pen p, Rectangle rect)
        {
            gx.DrawRectangle(ConvPen(p), rect.ToRect());

        }

        public override void DrawRectangle(Color color, int left, int top, int width, int height)
        {

            internalPen.Color = ConvColor(color);
            gx.DrawRectangle(internalPen, left, top, width, height);
        }
        public override void DrawString(string str, Font f, Brush brush, float x, float y)
        {
            gx.DrawString(str, ConvFont(f), ConvBrush(brush), x, y);

        }
        public override void DrawString(string str, Font f, Brush brush, float x, float y, float w, float h)
        {
            gx.DrawString(str, ConvFont(f), ConvBrush(brush), new System.Drawing.RectangleF(x, y, w, h));

        }
        public override void DrawRectangle(Color color, float left, float top, float width, float height)
        {
            internalPen.Color = ConvColor(color);
            gx.DrawRectangle(internalPen, left, top, width, height);
        }
        public override void DrawRectangle(Color color, Rectangle rect)
        {
            internalPen.Color = ConvColor(color);
            gx.DrawRectangle(internalPen, rect.ToRect());

        }

        public override void DrawImage(Image image, Rectangle rect)
        {
            gx.DrawImage(
                ConvBitmap(image),
                rect.ToRect());
        }
        public override void DrawImage(Bitmap image, int x, int y, int w, int h)
        {
            gx.DrawImage(image.InnerImage as System.Drawing.Bitmap, x, y, w, h);
        }
        public override void DrawImageUnScaled(Bitmap image, int x, int y)
        {
            gx.DrawImageUnscaled(image.InnerImage as System.Drawing.Bitmap, x, y);
        }

        public override void DrawBezire(Point[] points)
        {
            gx.DrawBeziers(System.Drawing.Pens.Blue, ConvPointArray(points));
        }
        public override void DrawLine(Pen pen, Point p1, Point p2)
        {
            gx.DrawLine(ConvPen(pen), p1.ToPoint(), p2.ToPoint());
        }
        public override void DrawLine(Color c, int x1, int y1, int x2, int y2)
        {
            System.Drawing.Color prevColor = internalPen.Color;
            internalPen.Color = ConvColor(c);
            gx.DrawLine(internalPen, x1, y1, x2, y2);
            internalPen.Color = prevColor;
        }
        public override void DrawLine(Color c, float x1, float y1, float x2, float y2)
        {
            System.Drawing.Color prevColor = internalPen.Color;
            internalPen.Color = ConvColor(c);
            gx.DrawLine(internalPen, x1, y1, x2, y2);
            internalPen.Color = prevColor;
        }

        public override void DrawLine(Color color, Point p1, Point p2)
        {
            System.Drawing.Color prevColor = internalPen.Color;
            internalPen.Color = ConvColor(color);
            gx.DrawLine(internalPen, p1.ToPoint(), p2.ToPoint());
            internalPen.Color = prevColor;
        }
        public override void DrawLine(Color color, Point p1, Point p2, DashStyle lineDashStyle)
        {
            System.Drawing.Drawing2D.DashStyle prevLineDashStyle = (System.Drawing.Drawing2D.DashStyle)internalPen.DashStyle;
            internalPen.DashStyle = (System.Drawing.Drawing2D.DashStyle)lineDashStyle;

            internalPen.Color = ConvColor(color);
            gx.DrawLine(internalPen,
                p1.ToPoint(),
                p2.ToPoint());
            internalPen.DashStyle = prevLineDashStyle;

        }
        public override void DrawArc(Pen pen, Rectangle r, float startAngle, float sweepAngle)
        {
            gx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            gx.DrawArc(ConvPen(pen),
                r.ToRect(),
                startAngle,
                sweepAngle);
        }
        public override void DrawLines(Color color, Point[] points)
        {
            internalPen.Color = ConvColor(color);
            gx.DrawLines(internalPen,
               ConvPointArray(points));
        }
        public override void DrawPolygon(Point[] points)
        {
            gx.DrawPolygon(System.Drawing.Pens.Blue, ConvPointArray(points));
        }
        public override void FillPolygon(Point[] points)
        {
            gx.FillPolygon(System.Drawing.Brushes.Blue, ConvPointArray(points));
        }
        public override void FillEllipse(Point[] points)
        {
            gx.FillEllipse(System.Drawing.Brushes.Blue, points[0].X, points[0].Y, points[2].X - points[0].X, points[2].Y - points[0].Y);

        }
        public override void FillEllipse(Color color, Rectangle rect)
        {
            internalBrush.Color = ConvColor(color);
            gx.FillEllipse(internalBrush, rect.ToRect());

        }

        public override void FillEllipse(Color color, int x, int y, int width, int height)
        {
            internalBrush.Color = ConvColor(color);
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
        /// Draws a line connecting the two points specified by the coordinate pairs.
        /// </summary>
        /// <param name="pen"><see cref="T:System.Drawing.Pen"/> that determines the color, width, and style of the line. </param><param name="x1">The x-coordinate of the first point. </param><param name="y1">The y-coordinate of the first point. </param><param name="x2">The x-coordinate of the second point. </param><param name="y2">The y-coordinate of the second point. </param><exception cref="T:System.ArgumentNullException"><paramref name="pen"/> is null.</exception>
        public override void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            ReleaseHdc();
            gx.DrawLine(pen.InnerPen as System.Drawing.Pen, x1, y1, x2, y2);

            //System.Drawing.Color prevColor = internalPen.Color;
            //internalPen.Color = ConvColor(c);
            //gx.DrawLine(internalPen, x1, y1, x2, y2);
            //internalPen.Color = prevColor;

        }

        /// <summary>
        /// Draws a rectangle specified by a coordinate pair, a width, and a height.
        /// </summary>
        /// <param name="pen">A <see cref="T:System.Drawing.Pen"/> that determines the color, width, and style of the rectangle. </param><param name="x">The x-coordinate of the upper-left corner of the rectangle to draw. </param><param name="y">The y-coordinate of the upper-left corner of the rectangle to draw. </param><param name="width">The width of the rectangle to draw. </param><param name="height">The height of the rectangle to draw. </param><exception cref="T:System.ArgumentNullException"><paramref name="pen"/> is null.</exception>
        public override void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            ReleaseHdc();
            gx.DrawRectangle((System.Drawing.Pen)pen.InnerPen, x, y, width, height);
        }

        public void FillRectangle(Brush getSolidBrush, float left, float top, float width, float height)
        {
            ReleaseHdc();
            gx.FillRectangle((System.Drawing.Brush)getSolidBrush.InnerBrush, left, top, width, height);
        }
        public void FillRectangle(Color solidColor, float left, float top, float width, float height)
        {
            ReleaseHdc();
            using (SolidBrush b = this.platform.CreateSolidBrush(solidColor))
            {
                gx.FillRectangle((System.Drawing.Brush)b.InnerBrush, left, top, width, height);
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
        public void FillPath(Brush brush, GraphicsPath path)
        {
            ReleaseHdc();
            gx.FillPath(brush.InnerBrush as System.Drawing.Brush,
                path.InnerPath as System.Drawing.Drawing2D.GraphicsPath);
        }

        /// <summary>
        /// Fills the interior of a polygon defined by an array of points specified by <see cref="T:System.Drawing.PointF"/> structures.
        /// </summary>
        /// <param name="brush"><see cref="T:System.Drawing.Brush"/> that determines the characteristics of the fill. </param><param name="points">Array of <see cref="T:System.Drawing.PointF"/> structures that represent the vertices of the polygon to fill. </param><exception cref="T:System.ArgumentNullException"><paramref name="brush"/> is null.-or-<paramref name="points"/> is null.</exception>
        public override void FillPolygon(Brush brush, PointF[] points)
        {
            ReleaseHdc();
            //create Point
            var pps = ConvPointFArray(points);
            gx.FillPolygon(brush.InnerBrush as System.Drawing.Brush, pps);
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
            DrawLine(color, rect.Location, new Point(rect.Right, rect.Bottom));
            DrawLine(color, new Point(rect.Left, rect.Bottom), new Point(rect.Right, rect.Top));
        }

#endif
    }

}