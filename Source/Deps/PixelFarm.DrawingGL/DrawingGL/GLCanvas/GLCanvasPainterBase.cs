//2016 MIT, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.Agg;
using PixelFarm.Agg.Transform;
using PixelFarm.Agg.VertexSource;
using PixelFarm.Drawing.Fonts;
using PixelFarm.Drawing.Text;


namespace PixelFarm.DrawingGL
{
    public abstract class GLCanvasPainterBase : CanvasPainter
    {
        protected CanvasGL2d _canvas;
        int _width;
        int _height;
        Color _fillColor;
        Color _strokeColor;
        RectInt _rectInt;

        RoundedRect roundRect;
        Arc arcTool;
        Ellipse ellipse = new Ellipse();
        Stroke _aggStroke = new Stroke(1);


        SmoothingMode _smoothingMode; //smoothing mode of this  painter
        public GLCanvasPainterBase(CanvasGL2d canvas, int w, int h)
        {
            _canvas = canvas;
            _width = w;
            _height = h;
            _rectInt = new RectInt(0, 0, w, h);
            arcTool = new Arc();
            CurrentFont = new RequestFont("tahoma", 14);

        }

        public override RectInt ClipBox
        {
            get
            {
                return _rectInt;
            }

            set
            {
                _rectInt = value;
            }
        }
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
                    case Drawing.SmoothingMode.HighQuality:
                    case Drawing.SmoothingMode.AntiAlias:
                        _canvas.SmoothMode = CanvasSmoothMode.Smooth;
                        break;
                    default:
                        _canvas.SmoothMode = CanvasSmoothMode.No;
                        break;
                }

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
                _fillColor = value;
            }
        }
        public override int Height
        {
            get
            {
                return this._height;
            }
        }

        public override Color StrokeColor
        {
            get
            {
                return _strokeColor;
            }
            set
            {
                _strokeColor = value;
                _canvas.StrokeColor = value;
            }
        }

        public override double StrokeWidth
        {
            get
            {
                return _canvas.StrokeWidth;
            }
            set
            {
                _canvas.StrokeWidth = (float)value;
            }
        }

        public override bool UseSubPixelRendering
        {
            get
            {
                return _canvas.SmoothMode == CanvasSmoothMode.Smooth;
            }

            set
            {
                _canvas.SmoothMode = value ? CanvasSmoothMode.Smooth : CanvasSmoothMode.No;
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
            _canvas.Clear(color);
        }
        public override void DoFilterBlurRecursive(RectInt area, int r)
        {
            //filter with glsl
        }
        public override void DoFilterBlurStack(RectInt area, int r)
        {
        }
        /// <summary>
        /// we do NOT store vxs
        /// </summary>
        /// <param name="vxs"></param>
        public override void Draw(VertexStore vxs)
        {
            _canvas.DrawGfxPath(this._strokeColor,
                InternalGraphicsPath.CreateGraphicsPath(new VertexStoreSnap(vxs)));
        }

        public override void DrawBezierCurve(float startX, float startY, float endX, float endY, float controlX1, float controlY1, float controlX2, float controlY2)
        {
            var v1 = GetFreeVxs();
            BezierCurve.CreateBezierVxs4(v1,
                new PixelFarm.VectorMath.Vector2(startX, startY),
                new PixelFarm.VectorMath.Vector2(endX, endY),
                new PixelFarm.VectorMath.Vector2(controlX1, controlY1),
                new PixelFarm.VectorMath.Vector2(controlY2, controlY2));
            _aggStroke.Width = this.StrokeWidth;

            var v2 = GetFreeVxs();
            _canvas.DrawGfxPath(_canvas.StrokeColor, InternalGraphicsPath.CreateGraphicsPath(new VertexStoreSnap(_aggStroke.MakeVxs(v1, v2))));
            ReleaseVxs(ref v2);
            ReleaseVxs(ref v1);
        }

        public override void DrawImage(ActualImage actualImage, params AffinePlan[] affinePlans)
        {
            //create gl bmp
            GLBitmap glBmp = new GLBitmap(actualImage.Width, actualImage.Height, ActualImage.GetBuffer(actualImage), false);
            _canvas.DrawImage(glBmp, 0, 0);
            glBmp.Dispose();
        }
        public override void DrawImage(ActualImage actualImage, double x, double y)
        {
            GLBitmap glBmp = new GLBitmap(actualImage.Width, actualImage.Height, ActualImage.GetBuffer(actualImage), false);
            _canvas.DrawImage(glBmp, (float)x, (float)y);
            glBmp.Dispose();
        }
        public override void DrawRoundRect(double left, double bottom, double right, double top, double radius)
        {
            if (roundRect == null)
            {
                roundRect = new RoundedRect(left, bottom, right, top, radius);
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

        TextureFont _currentTextureFont;
        internal void SetCurrentTextureFont(TextureFont textureFont)
        {
            _currentTextureFont = textureFont;
        }
        public override void DrawString(string text, double x, double y)
        {

            char[] chars = text.ToCharArray();
            int j = chars.Length;
            int buffsize = j * 2;
            //get kerning list 

            //get actual font for this canvas 
            TextureFont currentFont = _currentTextureFont;
            SimpleFontAtlas fontAtlas = currentFont.FontAtlas;
            ProperGlyph[] properGlyphs = new ProperGlyph[buffsize];
            TextShapingService.GetGlyphPos(currentFont, chars, 0, buffsize, properGlyphs);
            GLBitmap glBmp = (GLBitmap)currentFont.GLBmp;
            if (glBmp == null)
            {
                //create glbmp
                GlyphImage glyphImage = fontAtlas.TotalGlyph;
                int[] buffer = glyphImage.GetImageBuffer();
                glBmp = new GLBitmap(glyphImage.Width, glyphImage.Height, buffer, false);
            }
            //int j = chars.Length;
            //
            float c_x = (float)x;
            float c_y = (float)y;

            //TODO: review here ***
            //-----------------
            //1. layout each glyph before render *** 
            //
            float baseline = c_y - 24;//eg line height= 24 //create a list

            //--------------
            List<float> coords = new List<float>();
            float scale = 1f;
            for (int i = 0; i < buffsize; ++i)
            {
                ProperGlyph glyph1 = properGlyphs[i];
                uint codepoint = properGlyphs[i].codepoint;
                if (codepoint == 0)
                {
                    break;
                }
                //--------------------------------
                //if (codepoint == 1173 && i > 1)
                //{
                //    //check prev code point 
                //    codepoint = 1168;
                //}
                //--------------------------------
                TextureFontGlyphData glyphData;
                if (!fontAtlas.GetRectByCodePoint((int)codepoint, out glyphData))
                {
                    //Rectangle r = glyphData.Rect;
                    //float x_min = glyphData.BBoxXMin / 64;
                    ////draw each glyph at specific position                          
                    ////_canvas.DrawSubImageWithMsdf(glBmp, ref r, c_x + x_min, (float)(baseline + r.Height));
                    //_canvas.DrawSubImageWithMsdf(glBmp, ref r, c_x + x_min, (float)(baseline + r.Height)); 
                    ////c_x += r.Width - 10;
                    //c_x += (glyphData.AdvanceX / 64);
                    continue;
                }



                FontGlyph glyph = currentFont.GetGlyphByIndex(codepoint);
                int left = ((int)(glyph.glyphMatrix.img_horiBearingX * scale) >> 6);
                Rectangle r = glyphData.Rect;
                int adjustX = 0;
                int bboxYMin = glyph.glyphMatrix.bboxYmin >> 6;
                if (bboxYMin > 1 || bboxYMin < -1)
                {
                    //  adjustX = 3;
                }
                //scale down 0.8; 
                //_canvas.DrawSubImageWithMsdf(glBmp, ref r, adjustX + c_x + left,
                //    (float)(baseline + ((int)(glyphData.ImgHeight + glyph.glyphMatrix.bboxYmin) >> 6)), 1.1f);

                coords.Add(r.Left);
                coords.Add(r.Top);
                coords.Add(r.Width);
                coords.Add(r.Height);
                //-------------------------
                coords.Add(adjustX + c_x + left);
                //coords.Add(baseline + ((int)((glyphData.ImgHeight + glyph.glyphMatrix.bboxYmin) * scale) >> 6));
                coords.Add(baseline + ((int)((glyphData.ImgHeight + glyphData.BBoxYMin) * scale) >> 6));
                //int w = (int)(glyph.glyphMatrix.advanceX * scale) >> 6;
                int w = (int)(glyph.horiz_adv_x * scale) >> 6;
                c_x += w;
            }
            _canvas.DrawSubImageWithMsdf(glBmp, coords.ToArray(), scale);
        }


        //public override void DrawString(string text, double x, double y)
        //{

        //    char[] chars = text.ToCharArray();
        //    int j = chars.Length;
        //    int buffsize = j * 2;
        //    //get kerning list 
        //    TextureFont currentFont = this.CurrentFont as TextureFont;
        //    SimpleFontAtlas fontAtlas = currentFont.FontAtlas;
        //    ProperGlyph[] properGlyphs = new ProperGlyph[buffsize];
        //    currentFont.GetGlyphPos(chars, 0, buffsize, properGlyphs);
        //    GLBitmap glBmp = currentFont.GLBmp;
        //    if (glBmp == null)
        //    {
        //        //create glbmp
        //        GlyphImage glyphImage = fontAtlas.TotalGlyph;
        //        int[] buffer = glyphImage.GetImageBuffer();
        //        glBmp = new GLBitmap(glyphImage.Width, glyphImage.Height, buffer, false);
        //    }
        //    //int j = chars.Length;
        //    //
        //    float c_x = (float)x;
        //    float c_y = (float)y;

        //    //TODO: review here 
        //    //-----------------
        //    //1. layout each glyph before render *** 
        //    float baseline = c_y - 24;//eg line height= 24 
        //                              //create a list

        //    for (int i = 0; i < buffsize; ++i)
        //    {
        //        ProperGlyph glyph1 = properGlyphs[i];
        //        uint codepoint = properGlyphs[i].codepoint;
        //        if (codepoint == 0)
        //        {
        //            break;
        //        }
        //        if (codepoint == 1173 && i > 1)
        //        {
        //            //check prev code point 
        //            codepoint = 1168;
        //        }
        //        TextureFontGlyphData glyphData;
        //        if (!fontAtlas.GetRect((int)codepoint, out glyphData))
        //        {
        //            //Rectangle r = glyphData.Rect;
        //            //float x_min = glyphData.BBoxXMin / 64;
        //            ////draw each glyph at specific position                          
        //            ////_canvas.DrawSubImageWithMsdf(glBmp, ref r, c_x + x_min, (float)(baseline + r.Height));
        //            //_canvas.DrawSubImageWithMsdf(glBmp, ref r, c_x + x_min, (float)(baseline + r.Height)); 
        //            ////c_x += r.Width - 10;
        //            //c_x += (glyphData.AdvanceX / 64);
        //            continue;
        //        }

        //        //-------------------------------------------------------------
        //        //FontGlyph glyph = this.currentFont.GetGlyphByIndex(codepoint);
        //        FontGlyph glyph = currentFont.GetGlyphByIndex(codepoint);
        //        int left = (glyph.glyphMatrix.img_horiBearingX >> 6);
        //        Rectangle r = glyphData.Rect;
        //        int adjustX = 0;
        //        int bboxYMin = glyph.glyphMatrix.bboxYmin >> 6;
        //        if (bboxYMin > 1 || bboxYMin < -1)
        //        {
        //            //  adjustX = 3;
        //        }
        //        //scale down 0.8; 
        //        _canvas.DrawSubImageWithMsdf(glBmp, ref r, adjustX + c_x + left,
        //            (float)(baseline + ((int)(glyphData.ImgHeight + glyph.glyphMatrix.bboxYmin) >> 6)), 1.1f);
        //        int w = (glyph.glyphMatrix.advanceX) >> 6;
        //        c_x += (w);
        //    }
        //}
        //        public override void DrawString(string text, double x, double y)
        //        {
        //            ////in this version we draw string to image
        //            ////and the write the image back to gl surface
        //            //_winGfx.Clear(System.Drawing.Color.White);
        //            //_winGfx.DrawString(text, _winFont, _winGfxBrush, 0, 0);
        //            ////_winGfxBackBmp.Save("d:\\WImageTest\\a00123.png"); 

        //            //System.Drawing.SizeF textAreaSize = _winGfx.MeasureString(text, _winFont);
        //            //var bmpData = _winGfxBackBmp.LockBits(new System.Drawing.Rectangle(0, 0, _winGfxBackBmp.Width, _winGfxBackBmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, _winGfxBackBmp.PixelFormat);
        //            //int width = (int)textAreaSize.Width;
        //            //int height = (int)textAreaSize.Height;

        //            //ActualImage actualImg = new ActualImage(width, height, Agg.Image.PixelFormat.ARGB32);
        //            ////------------------------------------------------------
        //            ////copy bmp from specific bmp area 
        //            ////and convert to GLBmp  
        //            //int stride = bmpData.Stride;
        //            //byte[] buffer = actualImg.GetBuffer();
        //            //unsafe
        //            //{
        //            //    byte* header = (byte*)bmpData.Scan0;
        //            //    fixed (byte* dest0 = &buffer[0])
        //            //    {
        //            //        byte* dest = dest0;
        //            //        byte* rowHead = header;
        //            //        int rowLen = width * 4;
        //            //        for (int h = 0; h < height; ++h)
        //            //        {

        //            //            header = rowHead;
        //            //            for (int n = 0; n < rowLen;)
        //            //            {
        //            //                //move next
        //            //                *(dest + 0) = *(header + 0);
        //            //                *(dest + 1) = *(header + 1);
        //            //                *(dest + 2) = *(header + 2);
        //            //                *(dest + 3) = *(header + 3);
        //            //                header += 4;
        //            //                dest += 4;
        //            //                n += 4;
        //            //            }
        //            //            //finish one row
        //            //            rowHead += stride;
        //            //        }
        //            //    }
        //            //}
        //            //_winGfxBackBmp.UnlockBits(bmpData);
        //            ////------------------------------------------------------
        //            //GLBitmap glBmp = new GLBitmap(width, height, buffer, false);
        //            //_canvas.DrawImageWithWhiteTransparent(glBmp, (float)x, (float)y);
        //            //glBmp.Dispose();
        //        }
        public override void Fill(VertexStore vxs)
        {
            _canvas.FillGfxPath(
                _fillColor,
                InternalGraphicsPath.CreateGraphicsPath(new VertexStoreSnap(vxs))
                );
        }

        public override void Fill(VertexStoreSnap snap)
        {
            _canvas.FillGfxPath(
                _fillColor,
               InternalGraphicsPath.CreateGraphicsPath(snap));
        }
        public override void Draw(VertexStoreSnap snap)
        {
            _canvas.DrawGfxPath(
             this._fillColor,
             InternalGraphicsPath.CreateGraphicsPath(snap)
             );
        }

        public override void FillCircle(double x, double y, double radius)
        {
            FillEllipse(x - radius, y - radius, x + radius, y + radius);
        }

        public override void FillCircle(double x, double y, double radius, Color color)
        {
            Color prevColor = _fillColor;
            _fillColor = color;
            FillEllipse(x - radius, y - radius, x + radius, y + radius);
            _fillColor = prevColor; //reset
        }
        public void FillRoundRect(Color color, float x, float y, float w, float h, float rx, float ry)
        {
            roundRect.SetRect(x, y, x + w, y + h);
            roundRect.SetRadius(rx, ry);
            //create round rect vxs

            var vxs = roundRect.MakeVxs(GetFreeVxs());
            _canvas.FillGfxPath(_fillColor, InternalGraphicsPath.CreateGraphicsPath(new VertexStoreSnap(vxs)));
            ReleaseVxs(ref vxs);
        }
        public void DrawRoundRect(float x, float y, float w, float h, float rx, float ry)
        {
            roundRect.SetRect(x, y, x + w, y + h);
            roundRect.SetRadius(rx, ry);
            _aggStroke.Width = this.StrokeWidth;

            var v1 = GetFreeVxs();
            var v2 = GetFreeVxs();
            _aggStroke.MakeVxs(roundRect.MakeVxs(v1), v2);
            _canvas.DrawGfxPath(_strokeColor, InternalGraphicsPath.CreateGraphicsPath(new VertexStoreSnap(v2)));
            ReleaseVxs(ref v2);
            ReleaseVxs(ref v1);
        }

        public override void DrawEllipse(double left, double bottom, double right, double top)
        {
            double x = (left + right) / 2;
            double y = (bottom + top) / 2;
            double rx = Math.Abs(right - x);
            double ry = Math.Abs(top - y);
            ellipse.Reset(x, y, rx, ry);
            VertexStore vxs = ellipse.MakeVxs(GetFreeVxs());
            _canvas.DrawGfxPath(_strokeColor, InternalGraphicsPath.CreateGraphicsPath(new VertexStoreSnap(vxs)));
            ReleaseVxs(ref vxs);
        }
        public override void FillEllipse(double left, double bottom, double right, double top)
        {
            double x = (left + right) / 2;
            double y = (bottom + top) / 2;
            double rx = Math.Abs(right - x);
            double ry = Math.Abs(top - y);
            ellipse.Reset(x, y, rx, ry);
            var v1 = GetFreeVxs();
            ellipse.MakeVxs(v1);
            //other mode
            int n = v1.Count;
            //make triangular fan*** 

            float[] coords = new float[(n * 2) + 4];
            int i = 0;
            int nn = 0;
            int npoints = 0;
            double vx, vy;
            //center
            coords[nn++] = (float)x;
            coords[nn++] = (float)y;
            npoints++;
            var cmd = v1.GetVertex(i, out vx, out vy);
            while (i < n)
            {
                switch (cmd)
                {
                    case VertexCmd.MoveTo:
                        {
                            coords[nn++] = (float)vx;
                            coords[nn++] = (float)vy;
                            npoints++;
                        }
                        break;
                    case VertexCmd.LineTo:
                        {
                            coords[nn++] = (float)vx;
                            coords[nn++] = (float)vy;
                            npoints++;
                        }
                        break;
                    case VertexCmd.Stop:
                        {
                        }
                        break;
                    default:
                        {
                        }
                        break;
                }
                i++;
                cmd = v1.GetVertex(i, out vx, out vy);
            }


            //close circle
            coords[nn++] = coords[2];
            coords[nn++] = coords[3];
            npoints++;
            //----------------------------------------------
            _canvas.FillTriangleFan(_fillColor, coords, npoints);
            ReleaseVxs(ref v1);
        }
        public override void FillRectangle(double left, double bottom, double right, double top)
        {
            FillRect((float)left, (float)bottom, (float)(right - left), (float)(top - bottom));
        }
        public override void FillRectangle(double left, double bottom, double right, double top, Color fillColor)
        {
            FillRect((float)left, (float)bottom, (float)(right - left), (float)(top - bottom));
        }
        public override void FillRectLBWH(double left, double bottom, double width, double height)
        {
            FillRect((float)left, (float)bottom, (float)width, (float)height);
        }

        public override void FillRenderVx(Brush brush, RenderVx renderVx)
        {
            _canvas.FillRenderVx(brush, renderVx);
        }
        public override void FillRenderVx(RenderVx renderVx)
        {
            _canvas.FillRenderVx(_fillColor, renderVx);
        }
        public override void DrawRenderVx(RenderVx renderVx)
        {
            _canvas.DrawRenderVx(_strokeColor, renderVx);
        }



        void FillRect(float x, float y, float w, float h)
        {
            float[] coords = CreateRectTessCoordsTriStrip(x, y, w, h);
            _canvas.FillTriangleStrip(_fillColor, coords, 4);
        }
        static float[] CreateRectTessCoordsTriStrip(float x, float y, float w, float h)
        {
            //float x0 = x;
            //float y0 = y + h;
            //float x1 = x;
            //float y1 = y;
            //float x2 = x + w;
            //float y2 = y + h;
            //float x3 = x + w;
            //float y3 = y;
            return new float[]{
               x,y + h,
               x,y,
               x + w, y + h,
               x + w, y,
            };
        }
        public override void FillRoundRectangle(double left, double bottom, double right, double top, double radius)
        {
            if (roundRect == null)
            {
                roundRect = new Agg.VertexSource.RoundedRect(left, bottom, right, top, radius);
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
            _canvas.StrokeColor = _strokeColor;
            _canvas.DrawLine((float)x1, (float)y1, (float)x2, (float)y2);
        }
        public override void Line(double x1, double y1, double x2, double y2, Color color)
        {
            _canvas.StrokeColor = color;
            _canvas.DrawLine((float)x1, (float)y1, (float)x2, (float)y2);
        }
        public override void PaintSeries(VertexStore vxs, Color[] colors, int[] pathIndexs, int numPath)
        {
            //TODO: review here.
            //
            for (int i = 0; i < numPath; ++i)
            {
                _canvas.FillGfxPath(colors[i], InternalGraphicsPath.CreateGraphicsPath(new VertexStoreSnap(vxs, pathIndexs[i])));
            }
        }
        public override void Rectangle(double left, double bottom, double right, double top)
        {
            //draw rectangle
            _canvas.DrawRect((float)left, (float)bottom, (float)(right - left), (float)(top - bottom));
        }
        public override void Rectangle(double left, double bottom, double right, double top, Color color)
        {   //draw rectangle
            var prev = _canvas.StrokeColor;
            _canvas.StrokeColor = color;
            _canvas.DrawRect((float)left, (float)bottom, (float)(right - left), (float)(top - bottom));
            _canvas.StrokeColor = prev;
        }
        public override void SetClipBox(int x1, int y1, int x2, int y2)
        {
        }
        public void DrawCircle(float x, float y, double radius)
        {
            DrawEllipse(x - radius, y - radius, x + radius, y + radius);
        }
        //-----------------------------------------------------------------------------------------------------------------
        public override RenderVx CreateRenderVx(VertexStoreSnap snap)
        {
            //store internal gfx path inside render vx
            return new GLRenderVx(InternalGraphicsPath.CreateGraphicsPath(snap));
        }
        public RenderVx CreatePolygonRenderVx(float[] xycoords)
        {
            //store internal gfx path inside render vx
            return new GLRenderVx(InternalGraphicsPath.CreatePolygonGraphicsPath(xycoords));
        }

        struct CenterFormArc
        {
            public double cx;
            public double cy;
            public double radStartAngle;
            public double radSweepDiff;
            public bool scaleUp;
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
        //---------------------------------------------------------------------
        public void DrawArc(float fromX, float fromY, float endX, float endY,
         float xaxisRotationAngleDec, float rx, float ry,
         SvgArcSize arcSize, SvgArcSweep arcSweep)
        {
            //------------------
            //SVG Elliptical arc ...
            //from Apache Batik
            //-----------------

            CenterFormArc centerFormArc = new CenterFormArc();
            ComputeArc2(fromX, fromY, rx, ry,
                 DegToRad(xaxisRotationAngleDec),
                 arcSize == SvgArcSize.Large,
                 arcSweep == SvgArcSweep.Negative,
                 endX, endY, ref centerFormArc);
            arcTool.Init(centerFormArc.cx, centerFormArc.cy, rx, ry,
                centerFormArc.radStartAngle,
                (centerFormArc.radStartAngle + centerFormArc.radSweepDiff));

            VertexStore v1 = GetFreeVxs();
            bool stopLoop = false;
            foreach (VertexData vertexData in arcTool.GetVertexIter())
            {
                switch (vertexData.command)
                {
                    case VertexCmd.Stop:
                        stopLoop = true;
                        break;
                    default:
                        v1.AddVertex(vertexData.x, vertexData.y, vertexData.command);
                        //yield return vertexData;
                        break;
                }
                //------------------------------
                if (stopLoop) { break; }
            }

            double scaleRatio = 1;
            if (centerFormArc.scaleUp)
            {
                int vxs_count = v1.Count;
                double px0, py0, px_last, py_last;
                v1.GetVertex(0, out px0, out py0);
                v1.GetVertex(vxs_count - 1, out px_last, out py_last);
                double distance1 = Math.Sqrt((px_last - px0) * (px_last - px0) + (py_last - py0) * (py_last - py0));
                double distance2 = Math.Sqrt((endX - fromX) * (endX - fromX) + (endY - fromY) * (endY - fromY));
                if (distance1 < distance2)
                {
                    scaleRatio = distance2 / distance1;
                }
                else
                {
                }
            }

            if (xaxisRotationAngleDec != 0)
            {
                //also  rotate 
                if (centerFormArc.scaleUp)
                {
                    var mat = PixelFarm.Agg.Transform.Affine.NewMatix(
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, -centerFormArc.cx, -centerFormArc.cy),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Scale, scaleRatio, scaleRatio),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Rotate, DegToRad(xaxisRotationAngleDec)),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, centerFormArc.cx, centerFormArc.cy));
                    var v2 = GetFreeVxs();
                    mat.TransformToVxs(v1, v2);
                    ReleaseVxs(ref v1);
                    v1 = v2;
                }
                else
                {
                    //not scalue
                    var mat = PixelFarm.Agg.Transform.Affine.NewMatix(
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, -centerFormArc.cx, -centerFormArc.cy),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Rotate, DegToRad(xaxisRotationAngleDec)),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, centerFormArc.cx, centerFormArc.cy));
                    var v2 = GetFreeVxs();
                    mat.TransformToVxs(v1, v2);
                    ReleaseVxs(ref v1);
                    v1 = v2;
                }
            }
            else
            {
                //no rotate
                if (centerFormArc.scaleUp)
                {
                    var mat = PixelFarm.Agg.Transform.Affine.NewMatix(
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, -centerFormArc.cx, -centerFormArc.cy),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Scale, scaleRatio, scaleRatio),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, centerFormArc.cx, centerFormArc.cy));
                    var v2 = GetFreeVxs();
                    mat.TransformToVxs(v1, v2);
                    ReleaseVxs(ref v1);
                    v1 = v2;
                }
            }

            _aggStroke.Width = this.StrokeWidth;


            var v3 = _aggStroke.MakeVxs(v1, GetFreeVxs());
            _canvas.DrawGfxPath(_canvas.StrokeColor, InternalGraphicsPath.CreateGraphicsPath(new VertexStoreSnap(v3)));

            ReleaseVxs(ref v3);
            ReleaseVxs(ref v1);

        }
        static double DegToRad(double degree)
        {
            return degree * (Math.PI / 180d);
        }
        static double RadToDeg(double degree)
        {
            return degree * (180d / Math.PI);
        }



        static void ComputeArc2(double x0, double y0,
                             double rx, double ry,
                             double xAngleRad,
                             bool largeArcFlag,
                             bool sweepFlag,
                             double x, double y, ref CenterFormArc result)
        {
            //from  SVG1.1 spec
            //----------------------------------
            //step1: Compute (x1dash,y1dash)
            //----------------------------------

            double dx2 = (x0 - x) / 2.0;
            double dy2 = (y0 - y) / 2.0;
            double cosAngle = Math.Cos(xAngleRad);
            double sinAngle = Math.Sin(xAngleRad);
            double x1 = (cosAngle * dx2 + sinAngle * dy2);
            double y1 = (-sinAngle * dx2 + cosAngle * dy2);
            // Ensure radii are large enough
            rx = Math.Abs(rx);
            ry = Math.Abs(ry);
            double prx = rx * rx;
            double pry = ry * ry;
            double px1 = x1 * x1;
            double py1 = y1 * y1;
            // check that radii are large enough


            double radiiCheck = px1 / prx + py1 / pry;
            if (radiiCheck > 1)
            {
                rx = Math.Sqrt(radiiCheck) * rx;
                ry = Math.Sqrt(radiiCheck) * ry;
                prx = rx * rx;
                pry = ry * ry;
                result.scaleUp = true;
            }

            //----------------------------------
            //step2: Compute (cx1,cy1)
            //----------------------------------
            double sign = (largeArcFlag == sweepFlag) ? -1 : 1;
            double sq = ((prx * pry) - (prx * py1) - (pry * px1)) / ((prx * py1) + (pry * px1));
            sq = (sq < 0) ? 0 : sq;
            double coef = (sign * Math.Sqrt(sq));
            double cx1 = coef * ((rx * y1) / ry);
            double cy1 = coef * -((ry * x1) / rx);
            //----------------------------------
            //step3:  Compute (cx, cy) from (cx1, cy1)
            //----------------------------------
            double sx2 = (x0 + x) / 2.0;
            double sy2 = (y0 + y) / 2.0;
            double cx = sx2 + (cosAngle * cx1 - sinAngle * cy1);
            double cy = sy2 + (sinAngle * cx1 + cosAngle * cy1);
            //----------------------------------
            //step4: Compute theta and anfkediff
            double ux = (x1 - cx1) / rx;
            double uy = (y1 - cy1) / ry;
            double vx = (-x1 - cx1) / rx;
            double vy = (-y1 - cy1) / ry;
            double p, n;
            // Compute the angle start
            n = Math.Sqrt((ux * ux) + (uy * uy));
            p = ux; // (1 * ux) + (0 * uy)
            sign = (uy < 0) ? -1d : 1d;
            double angleStart = (sign * Math.Acos(p / n));  // Math.toDegrees(sign * Math.Acos(p / n));
            // Compute the angle extent
            n = Math.Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
            p = ux * vx + uy * vy;
            sign = (ux * vy - uy * vx < 0) ? -1d : 1d;
            double angleExtent = (sign * Math.Acos(p / n));// Math.toDegrees(sign * Math.Acos(p / n));
            //if (!sweepFlag && angleExtent > 0)
            //{
            //    angleExtent -= 360f;
            //}
            //else if (sweepFlag && angleExtent < 0)
            //{
            //    angleExtent += 360f;
            //}

            result.cx = cx;
            result.cy = cy;
            result.radStartAngle = angleStart;
            result.radSweepDiff = angleExtent;
        }
        static Arc ComputeArc(double x0, double y0,
                              double rx, double ry,
                              double angle,
                              bool largeArcFlag,
                              bool sweepFlag,
                               double x, double y)
        {
            /** 
         * This constructs an unrotated Arc2D from the SVG specification of an 
         * Elliptical arc.  To get the final arc you need to apply a rotation
         * transform such as:
         * 
         * AffineTransform.getRotateInstance
         *     (angle, arc.getX()+arc.getWidth()/2, arc.getY()+arc.getHeight()/2);
         */
            //
            // Elliptical arc implementation based on the SVG specification notes
            //

            // Compute the half distance between the current and the final point
            double dx2 = (x0 - x) / 2.0;
            double dy2 = (y0 - y) / 2.0;
            // Convert angle from degrees to radians
            angle = ((angle % 360.0) * Math.PI / 180f);
            double cosAngle = Math.Cos(angle);
            double sinAngle = Math.Sin(angle);
            //
            // Step 1 : Compute (x1, y1)
            //
            double x1 = (cosAngle * dx2 + sinAngle * dy2);
            double y1 = (-sinAngle * dx2 + cosAngle * dy2);
            // Ensure radii are large enough
            rx = Math.Abs(rx);
            ry = Math.Abs(ry);
            double Prx = rx * rx;
            double Pry = ry * ry;
            double Px1 = x1 * x1;
            double Py1 = y1 * y1;
            // check that radii are large enough
            double radiiCheck = Px1 / Prx + Py1 / Pry;
            if (radiiCheck > 1)
            {
                rx = Math.Sqrt(radiiCheck) * rx;
                ry = Math.Sqrt(radiiCheck) * ry;
                Prx = rx * rx;
                Pry = ry * ry;
            }

            //
            // Step 2 : Compute (cx1, cy1)
            //
            double sign = (largeArcFlag == sweepFlag) ? -1 : 1;
            double sq = ((Prx * Pry) - (Prx * Py1) - (Pry * Px1)) / ((Prx * Py1) + (Pry * Px1));
            sq = (sq < 0) ? 0 : sq;
            double coef = (sign * Math.Sqrt(sq));
            double cx1 = coef * ((rx * y1) / ry);
            double cy1 = coef * -((ry * x1) / rx);
            //
            // Step 3 : Compute (cx, cy) from (cx1, cy1)
            //
            double sx2 = (x0 + x) / 2.0;
            double sy2 = (y0 + y) / 2.0;
            double cx = sx2 + (cosAngle * cx1 - sinAngle * cy1);
            double cy = sy2 + (sinAngle * cx1 + cosAngle * cy1);
            //
            // Step 4 : Compute the angleStart (angle1) and the angleExtent (dangle)
            //
            double ux = (x1 - cx1) / rx;
            double uy = (y1 - cy1) / ry;
            double vx = (-x1 - cx1) / rx;
            double vy = (-y1 - cy1) / ry;
            double p, n;
            // Compute the angle start
            n = Math.Sqrt((ux * ux) + (uy * uy));
            p = ux; // (1 * ux) + (0 * uy)
            sign = (uy < 0) ? -1d : 1d;
            double angleStart = (sign * Math.Acos(p / n));  // Math.toDegrees(sign * Math.Acos(p / n));
            // Compute the angle extent
            n = Math.Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
            p = ux * vx + uy * vy;
            sign = (ux * vy - uy * vx < 0) ? -1d : 1d;
            double angleExtent = (sign * Math.Acos(p / n));// Math.toDegrees(sign * Math.Acos(p / n));
            if (!sweepFlag && angleExtent > 0)
            {
                angleExtent -= 360f;
            }
            else if (sweepFlag && angleExtent < 0)
            {
                angleExtent += 360f;
            }
            //angleExtent %= 360f;
            //angleStart %= 360f;

            //
            // We can now build the resulting Arc2D in double precision
            //
            //Arc2D.Double arc = new Arc2D.Double();
            //arc.x = cx - rx;
            //arc.y = cy - ry;
            //arc.width = rx * 2.0;
            //arc.height = ry * 2.0;
            //arc.start = -angleStart;
            //arc.extent = -angleExtent;
            Arc arc = new Arc();
            arc.Init(x, y, rx, ry, -(angleStart), -(angleExtent));
            return arc;
        }
    }
}