//MIT, 2016-2017, WinterDev 
using System;
using PixelFarm.Agg;
using PixelFarm.Agg.Transform;
using SkiaSharp;
namespace PixelFarm.Drawing.Skia
{

    public class SkiaCanvasPainter : CanvasPainter
    {
        RectInt _clipBox;
        Color _fillColor;
        Color _strokeColor;
        double _strokeWidth;
        bool _useSubPixelRendering;
        RequestFont _currentFont;

        int _height;
        int _width;
        Agg.VertexSource.RoundedRect roundRect;
        SmoothingMode _smoothingMode;
        //-----------------------
        SKCanvas _skCanvas;
        SKPaint _fill;
        SKPaint _stroke;
        //-----------------------
        public SkiaCanvasPainter(int w, int h)
        {

            _fill = new SKPaint();
            _stroke = new SKPaint();
            _stroke.IsStroke = true;
            _width = w;
            _height = h;
        }
        public SKCanvas Canvas
        {
            get { return _skCanvas; }
            set { _skCanvas = value; }
        }

        static bool defaultAntiAlias = false;
        public override SmoothingMode SmoothingMode
        {
            get
            {
                return _smoothingMode;
            }
            set
            {
                switch (_smoothingMode = value)
                {
                    case SmoothingMode.AntiAlias:
                        _fill.IsAntialias = _stroke.IsAntialias = true;
                        break;
                    default:
                        _fill.IsAntialias = _stroke.IsAntialias = defaultAntiAlias;
                        break;
                }
            }
        }

        public override void Draw(VertexStoreSnap vxs)
        {
            this.Fill(vxs);
        }
        public override RectInt ClipBox
        {
            get
            {
                return _clipBox;
            }
            set
            {
                _clipBox = value;
                //set clip rect to canvas ***
            }
        }

        public override RequestFont CurrentFont
        {
            get
            {
                return _currentFont;
            }

            set
            {
                _currentFont = value;
            }
        }
        public override Color FillColor
        {
            get
            {
                return _fillColor;
            }
            set
            {
                _fill.Color = ConvToSkColor(_fillColor = value);
            }
        }

        public override int Height
        {
            get
            {
                return _height;
            }
        }
        static SKColor ConvToSkColor(PixelFarm.Drawing.Color c)
        {
            return new SKColor(c.R, c.G, c.B, c.A);
        }
        public override Color StrokeColor
        {
            get
            {
                return _strokeColor;
            }
            set
            {
                _stroke.Color = ConvToSkColor(_strokeColor = value);
            }
        }
        public override double StrokeWidth
        {
            get
            {
                return _strokeWidth;
            }
            set
            {
                _stroke.StrokeWidth = (float)(_strokeWidth = value);
            }
        }

        public override bool UseSubPixelRendering
        {
            get
            {
                return _useSubPixelRendering;
            }
            set
            {
                _useSubPixelRendering = value;
            }
        }

        public override int Width
        {
            get
            {
                return _width;
            }
        }

        public override void Clear(Color color)
        {

            _skCanvas.Clear(ConvToSkColor(color));
        }
        public override void DoFilterBlurRecursive(RectInt area, int r)
        {
            //TODO: implement this
        }
        public override void DoFilterBlurStack(RectInt area, int r)
        {
            //since area is Windows coord
            //so we need to invert it 
            //System.Drawing.Bitmap backupBmp = this._gfxBmp;
            //int bmpW = backupBmp.Width;
            //int bmpH = backupBmp.Height;
            //System.Drawing.Imaging.BitmapData bmpdata = backupBmp.LockBits(
            //    new System.Drawing.Rectangle(0, 0, bmpW, bmpH),
            //    System.Drawing.Imaging.ImageLockMode.ReadWrite,
            //     backupBmp.PixelFormat);
            ////copy sub buffer to int32 array
            ////this version bmpdata must be 32 argb 
            //int a_top = area.Top;
            //int a_bottom = area.Bottom;
            //int a_width = area.Width;
            //int a_stride = bmpdata.Stride;
            //int a_height = Math.Abs(area.Height);
            //int[] src_buffer = new int[(a_stride / 4) * a_height];
            //int[] destBuffer = new int[src_buffer.Length];
            //int a_lineOffset = area.Left * 4;
            //unsafe
            //{
            //    IntPtr scan0 = bmpdata.Scan0;
            //    byte* src = (byte*)scan0;
            //    if (a_top > a_bottom)
            //    {
            //        int tmp_a_bottom = a_top;
            //        a_top = a_bottom;
            //        a_bottom = tmp_a_bottom;
            //    }

            //    //skip  to start line
            //    src += ((a_stride * a_top) + a_lineOffset);
            //    int index_start = 0;
            //    for (int y = a_top; y < a_bottom; ++y)
            //    {
            //        //then copy to int32 buffer 
            //        System.Runtime.InteropServices.Marshal.Copy(new IntPtr(src), src_buffer, index_start, a_width);
            //        index_start += a_width;
            //        src += (a_stride + a_lineOffset);
            //    }
            //    PixelFarm.Agg.Imaging.StackBlurARGB.FastBlur32ARGB(src_buffer, destBuffer, a_width, a_height, r);
            //    //then copy back to bmp
            //    index_start = 0;
            //    src = (byte*)scan0;
            //    src += ((a_stride * a_top) + a_lineOffset);
            //    for (int y = a_top; y < a_bottom; ++y)
            //    {
            //        //then copy to int32 buffer 
            //        System.Runtime.InteropServices.Marshal.Copy(destBuffer, index_start, new IntPtr(src), a_width);
            //        index_start += a_width;
            //        src += (a_stride + a_lineOffset);
            //    }
            //}
            ////--------------------------------
            //backupBmp.UnlockBits(bmpdata);
        }
        public override void Draw(VertexStore vxs)
        {
            VxsHelper.DrawVxsSnap(_skCanvas, new VertexStoreSnap(vxs), _stroke);
        }
        public override void DrawBezierCurve(float startX, float startY, float endX, float endY, float controlX1, float controlY1, float controlX2, float controlY2)
        {
            using (SKPath p = new SKPath())
            {
                p.MoveTo(startX, startY);
                p.CubicTo(controlX1, controlY1,
                    controlY1, controlY2,
                    endX, endY);
                _skCanvas.DrawPath(p, _stroke);
            }
        }

        public override void DrawImage(ActualImage actualImage, params AffinePlan[] affinePlans)
        {
            //1. create special graphics 
            throw new NotSupportedException();
            //using (System.Drawing.Bitmap srcBmp = CreateBmpBRGA(actualImage))
            //{
            //    var bmp = _bmpStore.GetFreeBmp();
            //    using (var g2 = System.Drawing.Graphics.FromImage(bmp))
            //    {
            //        //we can use recycle tmpVxsStore
            //        Affine destRectTransform = Affine.NewMatix(affinePlans);
            //        double x0 = 0, y0 = 0, x1 = bmp.Width, y1 = bmp.Height;
            //        destRectTransform.Transform(ref x0, ref y0);
            //        destRectTransform.Transform(ref x0, ref y1);
            //        destRectTransform.Transform(ref x1, ref y1);
            //        destRectTransform.Transform(ref x1, ref y0);
            //        var matrix = new System.Drawing.Drawing2D.Matrix(
            //           (float)destRectTransform.m11, (float)destRectTransform.m12,
            //           (float)destRectTransform.m21, (float)destRectTransform.m22,
            //           (float)destRectTransform.dx, (float)destRectTransform.dy);
            //        g2.Clear(System.Drawing.Color.Transparent);
            //        g2.Transform = matrix;
            //        //------------------------
            //        g2.DrawImage(srcBmp, new System.Drawing.PointF(0, 0));
            //        this._gfx.DrawImage(bmp, new System.Drawing.Point(0, 0));
            //    }
            //    _bmpStore.RelaseBmp(bmp);
            //}
        }
        public override void DrawImage(ActualImage actualImage, double x, double y)
        {
            //create Gdi bitmap from actual image
            int w = actualImage.Width;
            int h = actualImage.Height;
            switch (actualImage.PixelFormat)
            {
                case Agg.PixelFormat.ARGB32:
                    {

                        using (SKBitmap newBmp = new SKBitmap(actualImage.Width, actualImage.Height))
                        {
                            newBmp.LockPixels();
                            byte[] actualImgBuffer = ActualImage.GetBuffer(actualImage);
                            System.Runtime.InteropServices.Marshal.Copy(
                            actualImgBuffer,
                            0,
                            newBmp.GetPixels(),
                             actualImgBuffer.Length);
                            newBmp.UnlockPixels();
                        }
                        //newBmp.internalBmp.LockPixels();
                        //byte[] actualImgBuffer = ActualImage.GetBuffer(actualImage);

                        //System.Runtime.InteropServices.Marshal.Copy(
                        //     actualImgBuffer,
                        //     0,
                        //      newBmp.internalBmp.GetPixels(),
                        //      actualImgBuffer.Length);

                        //newBmp.internalBmp.UnlockPixels();
                        //return newBmp;

                        //copy data from acutal buffer to internal representation bitmap
                        //using (MySkBmp bmp = MySkBmp.CopyFrom(actualImage))
                        //{
                        //    _skCanvas.DrawBitmap(bmp.internalBmp, (float)x, (float)y);
                        //}
                    }
                    break;
                case Agg.PixelFormat.RGB24:
                    {
                    }
                    break;
                case Agg.PixelFormat.GrayScale8:
                    {
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        public override void DrawString(string text, double x, double y)
        {
            //use current brush and font
            _skCanvas.DrawText(text, (float)x, (float)y, _stroke);

            //_skCanvas.ResetMatrix();
            //_skCanvas.Translate(0.0F, (float)Height);// Translate the drawing area accordingly   


            ////draw with native win32
            ////------------

            ///*_gfx.DrawString(text,
            //    _latestWinGdiPlusFont.InnerFont,
            //    _currentFillBrush,
            //    new System.Drawing.PointF((float)x, (float)y));
            //*/
            ////------------
            ////restore back
            //_skCanvas.ResetMatrix();//again
            //_skCanvas.Scale(1f, -1f);// Flip the Y-Axis
            //_skCanvas.Translate(0.0F, -(float)Height);// Translate the drawing area accordingly                             
        }
        /// <summary>
        /// we do NOT store snap/vxs
        /// </summary>
        /// <param name="vxs"></param>
        public override void Fill(VertexStore vxs)
        {
            VxsHelper.FillVxsSnap(_skCanvas, new VertexStoreSnap(vxs), _fill);
        }
        /// <summary>
        /// we do NOT store snap/vxs
        /// </summary>
        /// <param name="snap"></param>
        public override void Fill(VertexStoreSnap snap)
        {
            VxsHelper.FillVxsSnap(_skCanvas, snap, _fill);
        }
        public override void FillCircle(double x, double y, double radius)
        {
            _skCanvas.DrawCircle((float)x, (float)y, (float)radius, _fill);
        }

        public override void FillCircle(double x, double y, double radius, Drawing.Color color)
        {

            var prevColor = FillColor;
            FillColor = color;
            _skCanvas.DrawCircle((float)x, (float)y, (float)radius, _fill);
            FillColor = prevColor;
        }

        public override void FillEllipse(double left, double bottom, double right, double top)
        {
            _skCanvas.DrawOval(
                new SKRect((float)left, (float)top, (float)right, (float)bottom),
                _fill);
        }
        public override void DrawEllipse(double left, double bottom, double right, double top)
        {
            _skCanvas.DrawOval(
              new SKRect((float)left, (float)top, (float)right, (float)bottom),
              _stroke);
        }
        public override void FillRectangle(double left, double bottom, double right, double top)
        {

            _skCanvas.DrawRect(
              new SKRect((float)left, (float)top, (float)right, (float)bottom),
                _fill);
        }
        public override void FillRectangle(double left, double bottom, double right, double top, Color fillColor)
        {
            var prevColor = FillColor;
            FillColor = fillColor;
            _skCanvas.DrawRect(
              new SKRect((float)left, (float)top, (float)right, (float)bottom),
                _fill);
            FillColor = prevColor;
        }
        public override void FillRectLBWH(double left, double bottom, double width, double height)
        {

            _skCanvas.DrawRect(
              new SKRect((float)left, (float)(bottom - height), (float)(left + width), (float)bottom),
                _fill);
        }

        VertexStorePool _vxsPool = new VertexStorePool();
        VertexStore GetFreeVxs()
        {

            return _vxsPool.GetFreeVxs();
        }
        void ReleaseVxs(ref VertexStore vxs)
        {
            _vxsPool.Release(ref vxs);
        }
        public override void DrawRoundRect(double left, double bottom, double right, double top, double radius)
        {
            if (roundRect == null)
            {
                roundRect = new PixelFarm.Agg.VertexSource.RoundedRect(left, bottom, right, top, radius);
                roundRect.NormalizeRadius();
            }
            else
            {
                roundRect.SetRect(left, bottom, right, top);
                roundRect.SetRadius(radius);
                roundRect.NormalizeRadius();
            }

            var v1 = GetFreeVxs();
            this.Draw(roundRect.MakeVxs(v1));
            ReleaseVxs(ref v1);
        }
        public override void FillRoundRectangle(double left, double bottom, double right, double top, double radius)
        {
            if (roundRect == null)
            {
                roundRect = new PixelFarm.Agg.VertexSource.RoundedRect(left, bottom, right, top, radius);
                roundRect.NormalizeRadius();
            }
            else
            {
                roundRect.SetRect(left, bottom, right, top);
                roundRect.SetRadius(radius);
                roundRect.NormalizeRadius();
            }
            var v1 = GetFreeVxs();
            this.Fill(roundRect.MakeVxs(v1));
            ReleaseVxs(ref v1);
        }
        public override void Line(double x1, double y1, double x2, double y2)
        {
            _skCanvas.DrawLine((float)x1, (float)y1, (float)x2, (float)y2, _stroke);
        }
        public override void Line(double x1, double y1, double x2, double y2, Color color)
        {
            var prevColor = StrokeColor;
            StrokeColor = color;
            _skCanvas.DrawLine((float)x1, (float)y1, (float)x2, (float)y2, _stroke);
            StrokeColor = prevColor;
        }
        public override void PaintSeries(VertexStore vxs, Color[] colors, int[] pathIndexs, int numPath)
        {
            var prevColor = FillColor;
            for (int i = 0; i < numPath; ++i)
            {
                _fill.Color = ConvToSkColor(colors[i]);
                VxsHelper.FillVxsSnap(_skCanvas, new VertexStoreSnap(vxs, pathIndexs[i]), _fill);
            }
            FillColor = prevColor;
        }

        public override void Rectangle(double left, double bottom, double right, double top)
        {
            _skCanvas.DrawLine((float)left, (float)top, (float)right, (float)bottom, _stroke);
        }
        public override void Rectangle(double left, double bottom, double right, double top, Color color)
        {
            var prevColor = StrokeColor;
            StrokeColor = color;
            _skCanvas.DrawLine((float)left, (float)top, (float)right, (float)bottom, _stroke);
            StrokeColor = prevColor;
        }
        public override void SetClipBox(int x1, int y1, int x2, int y2)
        {
            _skCanvas.ClipRect(new SKRect(x1, y1, x2, y2));
        }
        public override RenderVx CreateRenderVx(VertexStoreSnap snap)
        {
            var renderVx = new WinGdiRenderVx(snap);
            renderVx.path = VxsHelper.CreateGraphicsPath(snap);
            return renderVx;
        }
        public override void FillRenderVx(Brush brush, RenderVx renderVx)
        {
            //TODO: review brush implementation here
            WinGdiRenderVx wRenderVx = (WinGdiRenderVx)renderVx;
            VxsHelper.FillPath(_skCanvas, wRenderVx.path, _fill);
        }
        public override void DrawRenderVx(RenderVx renderVx)
        {
            WinGdiRenderVx wRenderVx = (WinGdiRenderVx)renderVx;
            VxsHelper.DrawPath(_skCanvas, wRenderVx.path, _stroke);
        }
        public override void FillRenderVx(RenderVx renderVx)
        {
            WinGdiRenderVx wRenderVx = (WinGdiRenderVx)renderVx;
            VxsHelper.FillPath(_skCanvas, wRenderVx.path, _fill);
        }
    }
}