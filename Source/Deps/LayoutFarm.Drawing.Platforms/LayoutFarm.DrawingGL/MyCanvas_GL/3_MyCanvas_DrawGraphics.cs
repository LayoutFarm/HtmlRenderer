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
namespace LayoutFarm.Drawing.DrawingGL
{

    partial class MyCanvasGL
    {
        float strokeWidth = 1f;
        Color fillSolidColor = Color.Transparent;
       
        public override float StrokeWidth
        {
            get
            {
                return this.strokeWidth;
            }
            set
            {
                this.strokeWidth = value;
            
            }
        }

        public override GraphicsPlatform Platform
        {
            get { return this.platform; }
        }


        //==========================================================
        public override void CopyFrom(Canvas sourceCanvas, int logicalSrcX, int logicalSrcY, Rectangle destArea)
        {
            throw new NotSupportedException();
            //MyCanvas s1 = (MyCanvas)sourceCanvas;

            //if (s1.gx != null)
            //{
            //    int phySrcX = logicalSrcX - s1.left;
            //    int phySrcY = logicalSrcY - s1.top;

            //    System.Drawing.Rectangle postIntersect =
            //        System.Drawing.Rectangle.Intersect(currentClipRect, destArea.ToRect());
            //    phySrcX += postIntersect.X - destArea.X;
            //    phySrcY += postIntersect.Y - destArea.Y;
            //    destArea = postIntersect.ToRect();

            //    IntPtr gxdc = gx.GetHdc();

            //    MyWin32.SetViewportOrgEx(gxdc, CanvasOrgX, CanvasOrgY, IntPtr.Zero);
            //    IntPtr source_gxdc = s1.gx.GetHdc();
            //    MyWin32.SetViewportOrgEx(source_gxdc, s1.CanvasOrgX, s1.CanvasOrgY, IntPtr.Zero);


            //    MyWin32.BitBlt(gxdc, destArea.X, destArea.Y, destArea.Width, destArea.Height, source_gxdc, phySrcX, phySrcY, MyWin32.SRCCOPY);


            //    MyWin32.SetViewportOrgEx(source_gxdc, -s1.CanvasOrgX, -s1.CanvasOrgY, IntPtr.Zero);

            //    s1.gx.ReleaseHdc();

            //    MyWin32.SetViewportOrgEx(gxdc, -CanvasOrgX, -CanvasOrgY, IntPtr.Zero);
            //    gx.ReleaseHdc();



            //}
        }
        public override void RenderTo(IntPtr destHdc, int sourceX, int sourceY, Rectangle destArea)
        {
            throw new NotImplementedException();
            //IntPtr gxdc = gx.GetHdc();
            //MyWin32.SetViewportOrgEx(gxdc, CanvasOrgX, CanvasOrgY, IntPtr.Zero);
            //MyWin32.BitBlt(destHdc, destArea.X, destArea.Y,
            //destArea.Width, destArea.Height, gxdc, sourceX, sourceY, MyWin32.SRCCOPY);
            //MyWin32.SetViewportOrgEx(gxdc, -CanvasOrgX, -CanvasOrgY, IntPtr.Zero);
            //gx.ReleaseHdc();
        }
       

        //public override void DrawRoundRect(int x, int y, int w, int h, Size cornerSize)
        //{

        //    int cornerSizeW = cornerSize.Width;
        //    int cornerSizeH = cornerSize.Height;

        //    System.Drawing.Drawing2D.GraphicsPath gxPath = new System.Drawing.Drawing2D.GraphicsPath();
        //    gxPath.AddArc(new System.Drawing.Rectangle(x, y, cornerSizeW * 2, cornerSizeH * 2), 180, 90);
        //    gxPath.AddLine(new System.Drawing.Point(x + cornerSizeW, y), new System.Drawing.Point(x + w - cornerSizeW, y));

        //    gxPath.AddArc(new System.Drawing.Rectangle(x + w - cornerSizeW * 2, y, cornerSizeW * 2, cornerSizeH * 2), -90, 90);
        //    gxPath.AddLine(new System.Drawing.Point(x + w, y + cornerSizeH), new System.Drawing.Point(x + w, y + h - cornerSizeH));

        //    gxPath.AddArc(new System.Drawing.Rectangle(x + w - cornerSizeW * 2, y + h - cornerSizeH * 2, cornerSizeW * 2, cornerSizeH * 2), 0, 90);
        //    gxPath.AddLine(new System.Drawing.Point(x + w - cornerSizeW, y + h), new System.Drawing.Point(x + cornerSizeW, y + h));

        //    gxPath.AddArc(new System.Drawing.Rectangle(x, y + h - cornerSizeH * 2, cornerSizeW * 2, cornerSizeH * 2), 90, 90);
        //    gxPath.AddLine(new System.Drawing.Point(x, y + cornerSizeH), new System.Drawing.Point(x, y + h - cornerSizeH));

        //    gx.FillPath(System.Drawing.Brushes.Yellow, gxPath);
        //    gx.DrawPath(System.Drawing.Pens.Red, gxPath);
        //    gxPath.Dispose();
        //}


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
                throw new NotImplementedException();
                //ReleaseHdc();
                //return (SmoothingMode)(gx.SmoothingMode);
            }
            set
            {
                throw new NotImplementedException();
                //ReleaseHdc();
                //gx.SmoothingMode = (System.Drawing.Drawing2D.SmoothingMode)value;
            }
        } 
    }

}