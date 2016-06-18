//2016 MIT, WinterDev

using System;
using System.Drawing;
using PixelFarm.Agg;
using PixelFarm.Agg.Transform;
namespace PixelFarm.Drawing.WinGdi
{
    public class GdiPlusCanvasPainter : CanvasPainter
    {
        RectInt _clipBox;
        ColorRGBA _fillColor;
        int _width, _height;
        Agg.Fonts.Font _font;
        ColorRGBA _strokeColor;
        double _strokeWidth;
        bool _useSubPixelRendering;
        Graphics _internalGfx;
        Agg.VertexSource.CurveFlattener curveFlattener;
        System.Drawing.Font _currentFont;
        System.Drawing.SolidBrush _currentFillBrush;
        System.Drawing.Pen _currentPen;
        Agg.VertexSource.RoundedRect roundRect;
        MyImageReaderWriter sharedImageWriterReader = new MyImageReaderWriter();
        System.Drawing.Bitmap _bufferBmp;
        public GdiPlusCanvasPainter(System.Drawing.Bitmap bufferBmp)
        {
            _width = 800;
            _height = 600;
            _bufferBmp = bufferBmp;
            _internalGfx = Graphics.FromImage(bufferBmp);
            //credit:
            //http://stackoverflow.com/questions/1485745/flip-coordinates-when-drawing-to-control
            _internalGfx.ScaleTransform(1.0F, -1.0F);// Flip the Y-Axis
            _internalGfx.TranslateTransform(0.0F, -(float)Height);// Translate the drawing area accordingly            
            _currentFont = new System.Drawing.Font("Tahoma", 10);
            _currentFillBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            _currentPen = new System.Drawing.Pen(System.Drawing.Color.Black);
        }
        public System.Drawing.Drawing2D.SmoothingMode SmoothingMode
        {
            get { return _internalGfx.SmoothingMode; }
            set { _internalGfx.SmoothingMode = value; }
        }
        public System.Drawing.Drawing2D.CompositingMode CompositingMode
        {
            get { return _internalGfx.CompositingMode; }
            set { _internalGfx.CompositingMode = value; }
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
            }
        }

        public override Agg.Fonts.Font CurrentFont
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
            }
        }
        public override ColorRGBA FillColor
        {
            get
            {
                return _fillColor;
            }
            set
            {
                _fillColor = value;
                _currentFillBrush.Color = VxsHelper.ToDrawingColor(value);
            }
        }

        public override int Height
        {
            get
            {
                return _height;
            }
        }

        public override ColorRGBA StrokeColor
        {
            get
            {
                return _strokeColor;
            }
            set
            {
                _strokeColor = value;
                _currentPen.Color = VxsHelper.ToDrawingColor(value);
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
                _strokeWidth = value;
                _currentPen.Width = (float)value;
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

        public override void Clear(ColorRGBA color)
        {
            _internalGfx.Clear(VxsHelper.ToDrawingColor(color));
        }
        public override void DoFilterBlurRecursive(RectInt area, int r)
        {
            //TODO: implement this
        }
        public override void DoFilterBlurStack(RectInt area, int r)
        {
            //since area is Windows coord
            //so we need to invert it 
            System.Drawing.Bitmap backupBmp = this._bufferBmp;
            int bmpW = backupBmp.Width;
            int bmpH = backupBmp.Height;
            System.Drawing.Imaging.BitmapData bmpdata = backupBmp.LockBits(
                new System.Drawing.Rectangle(0, 0, bmpW, bmpH),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                 backupBmp.PixelFormat);
            //copy sub buffer to int32 array
            //this version bmpdata must be 32 argb 
            int a_top = area.Top;
            int a_bottom = area.Bottom;
            int a_width = area.Width;
            int a_stride = bmpdata.Stride;
            int a_height = Math.Abs(area.Height);
            int[] src_buffer = new int[(a_stride / 4) * a_height];
            int[] destBuffer = new int[src_buffer.Length];
            int a_lineOffset = area.Left * 4;
            unsafe
            {
                IntPtr scan0 = bmpdata.Scan0;
                byte* src = (byte*)scan0;
                if (a_top > a_bottom)
                {
                    int tmp_a_bottom = a_top;
                    a_top = a_bottom;
                    a_bottom = tmp_a_bottom;
                }

                //skip  to start line
                src += ((a_stride * a_top) + a_lineOffset);
                int index_start = 0;
                for (int y = a_top; y < a_bottom; ++y)
                {
                    //then copy to int32 buffer 
                    System.Runtime.InteropServices.Marshal.Copy(new IntPtr(src), src_buffer, index_start, a_width);
                    index_start += a_width;
                    src += (a_stride + a_lineOffset);
                }
                PixelFarm.Agg.Image.StackBlurARGB.FastBlur32ARGB(src_buffer, destBuffer, a_width, a_height, r);
                //then copy back to bmp
                index_start = 0;
                src = (byte*)scan0;
                src += ((a_stride * a_top) + a_lineOffset);
                for (int y = a_top; y < a_bottom; ++y)
                {
                    //then copy to int32 buffer 
                    System.Runtime.InteropServices.Marshal.Copy(destBuffer, index_start, new IntPtr(src), a_width);
                    index_start += a_width;
                    src += (a_stride + a_lineOffset);
                }
            }
            //--------------------------------
            backupBmp.UnlockBits(bmpdata);
        }
        public override void Draw(VertexStore vxs)
        {
            VxsHelper.DrawVxsSnap(_internalGfx, new VertexStoreSnap(vxs), _strokeColor);
        }
        public override void DrawBezierCurve(float startX, float startY, float endX, float endY, float controlX1, float controlY1, float controlX2, float controlY2)
        {
            _internalGfx.DrawBezier(_currentPen,
                 startX, startY,
                 controlX1, controlY1,
                 controlX2, controlY2,
                 endX, endY);
        }
        public override void DrawEllipse()
        {
            throw new NotImplementedException();
        }
        public override void DrawImage(ActualImage actualImage, params AffinePlan[] affinePlans)
        {
            //1. create special graphics 
            using (var srcBmp = CreateBmpBRGA(actualImage))
            using (var bmp = new System.Drawing.Bitmap(800, 600))
            using (Graphics g2 = System.Drawing.Graphics.FromImage(bmp))
            {
                //we can use recycle tmpVxsStore
                Affine destRectTransform = Affine.NewMatix(affinePlans);
                double x0 = 0, y0 = 0, x1 = bmp.Width, y1 = bmp.Height;
                destRectTransform.Transform(ref x0, ref y0);
                destRectTransform.Transform(ref x0, ref y1);
                destRectTransform.Transform(ref x1, ref y1);
                destRectTransform.Transform(ref x1, ref y0);
                var matrix = new System.Drawing.Drawing2D.Matrix(
                   (float)destRectTransform.m11, (float)destRectTransform.m12,
                   (float)destRectTransform.m21, (float)destRectTransform.m22,
                   (float)destRectTransform.dx, (float)destRectTransform.dy);
                g2.Clear(System.Drawing.Color.Transparent);
                g2.Transform = matrix;
                //------------------------
                g2.DrawImage(srcBmp, new System.Drawing.PointF(0, 0));
                this._internalGfx.DrawImage(bmp, new System.Drawing.Point(0, 0));
            }
        }

        static System.Drawing.Bitmap CreateBmpBRGA(ActualImage actualImage)
        {
            int w = actualImage.Width;
            int h = actualImage.Height;
            //copy data to bitmap
            //bgra  
            var bmp = new System.Drawing.Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            byte[] acutalBuffer = actualImage.GetBuffer();
            var bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
            System.Runtime.InteropServices.Marshal.Copy(acutalBuffer, 0, bmpData.Scan0, acutalBuffer.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }
        public void DrawImage(System.Drawing.Bitmap bmp, float x, float y)
        {
            _internalGfx.DrawImage(bmp, x, y);
        }
        public override void DrawImage(ActualImage actualImage, double x, double y)
        {
            //create Gdi bitmap from actual image
            int w = actualImage.Width;
            int h = actualImage.Height;
            switch (actualImage.PixelFormat)
            {
                case Agg.Image.PixelFormat.Rgba32:
                    {
                        //copy data from acutal buffer to internal representation bitmap
                        using (var bmp = CreateBmpBRGA(actualImage))
                        {
                            this._internalGfx.DrawImageUnscaled(bmp, new System.Drawing.Point((int)x, (int)y));
                        }
                    }
                    break;
                case Agg.Image.PixelFormat.Rgb24:
                    {
                    }
                    break;
                case Agg.Image.PixelFormat.GrayScale8:
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
            _internalGfx.ResetTransform();
            _internalGfx.TranslateTransform(0.0F, (float)Height);// Translate the drawing area accordingly   
            _internalGfx.DrawString(text, _currentFont, _currentFillBrush, new System.Drawing.PointF((float)x, (float)y));
            //restore back
            _internalGfx.ResetTransform();//again
            _internalGfx.ScaleTransform(1.0F, -1.0F);// Flip the Y-Axis
            _internalGfx.TranslateTransform(0.0F, -(float)Height);// Translate the drawing area accordingly                
        }

        public override void Fill(VertexStore vxs)
        {
            VxsHelper.FillVxsSnap(_internalGfx, new VertexStoreSnap(vxs), _fillColor);
        }

        public override void Fill(VertexStoreSnap snap)
        {
            VxsHelper.FillVxsSnap(_internalGfx, snap, _fillColor);
        }

        public override void Fill(VertexStore vxs, ISpanGenerator spanGen)
        {
            //fill with ispan generator
            throw new NotImplementedException();
        }

        public override void FillCircle(double x, double y, double radius)
        {
            _internalGfx.FillEllipse(_currentFillBrush, (float)x, (float)y, (float)(radius + radius), (float)(radius + radius));
        }

        public override void FillCircle(double x, double y, double radius, ColorRGBA color)
        {
            var prevColor = _currentFillBrush.Color;
            _currentFillBrush.Color = VxsHelper.ToDrawingColor(color);
            _internalGfx.FillEllipse(_currentFillBrush, (float)x, (float)y, (float)(radius + radius), (float)(radius + radius));
            _currentFillBrush.Color = prevColor;
        }

        public override void FillEllipse(double left, double bottom, double right, double top, int nsteps)
        {
            _internalGfx.FillEllipse(_currentFillBrush, new System.Drawing.RectangleF((float)left, (float)top, (float)(right - left), (float)(bottom - top)));
        }

        public override void FillRectangle(double left, double bottom, double right, double top)
        {
            _internalGfx.FillRectangle(_currentFillBrush, System.Drawing.RectangleF.FromLTRB((float)left, (float)top, (float)right, (float)bottom));
        }
        public override void FillRectangle(double left, double bottom, double right, double top, ColorRGBA fillColor)
        {
            System.Drawing.Color prevColor = _currentFillBrush.Color;
            _currentFillBrush.Color = VxsHelper.ToDrawingColor(fillColor);
            _internalGfx.FillRectangle(_currentFillBrush, System.Drawing.RectangleF.FromLTRB((float)left, (float)top, (float)right, (float)bottom));
            _currentFillBrush.Color = prevColor;
        }
        public override void FillRectLBWH(double left, double bottom, double width, double height)
        {
            _internalGfx.FillRectangle(_currentFillBrush, new System.Drawing.RectangleF((float)left, (float)(bottom - height), (float)width, (float)height));
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
            this.Draw(roundRect.MakeVxs());
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
            this.Fill(roundRect.MakeVxs());
        }

        public override VertexStore FlattenCurves(VertexStore srcVxs)
        {
            if (curveFlattener == null)
            {
                curveFlattener = new Agg.VertexSource.CurveFlattener();
            }
            return curveFlattener.MakeVxs(srcVxs);
        }

        public override void Line(double x1, double y1, double x2, double y2)
        {
            _internalGfx.DrawLine(_currentPen, new System.Drawing.PointF((float)x1, (float)y1), new System.Drawing.PointF((float)x2, (float)y2));
        }

        public override void Line(double x1, double y1, double x2, double y2, ColorRGBA color)
        {
            var prevColor = _currentPen.Color;
            _currentPen.Color = VxsHelper.ToDrawingColor(color);
            _internalGfx.DrawLine(_currentPen, new System.Drawing.PointF((float)x1, (float)y1), new System.Drawing.PointF((float)x2, (float)y2));
            _currentPen.Color = prevColor;
        }
        public override void PaintSeries(VertexStore vxs, ColorRGBA[] colors, int[] pathIndexs, int numPath)
        {
            for (int i = 0; i < numPath; ++i)
            {
                VxsHelper.FillVxsSnap(_internalGfx, new VertexStoreSnap(vxs, pathIndexs[i]), colors[i]);
            }
        }

        public override void Rectangle(double left, double bottom, double right, double top)
        {
            _internalGfx.DrawRectangle(_currentPen, System.Drawing.Rectangle.FromLTRB((int)left, (int)top, (int)right, (int)bottom));
        }
        public override void Rectangle(double left, double bottom, double right, double top, ColorRGBA color)
        {
            _internalGfx.DrawRectangle(_currentPen, System.Drawing.Rectangle.FromLTRB((int)left, (int)top, (int)right, (int)bottom));
        }
        public override void SetClipBox(int x1, int y1, int x2, int y2)
        {
            _internalGfx.SetClip(new System.Drawing.Rectangle(x1, y1, x2 - x1, y2 - y1));
        }
    }
}