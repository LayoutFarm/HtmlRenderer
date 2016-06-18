//MIT 2014, WinterDev

using System;
using OpenTK.Graphics.ES20;
using Tesselate;
using PixelFarm.Agg;
using PixelFarm.Agg.VertexSource;
namespace PixelFarm.DrawingGL
{
    public partial class CanvasGL2d
    {
        BasicShader basicShader;
        PixelFarm.Drawing.Color strokeColor = PixelFarm.Drawing.Color.Black;
        Tesselator tess = new Tesselator();
        TessListener2 tessListener = new TessListener2();
        //tools---------------------------------
        RoundedRect roundRect = new RoundedRect();
        Ellipse ellipse = new Ellipse();
        PathWriter ps = new PathWriter();
        Stroke aggStroke = new Stroke(1);
        GLScanlineRasterizer sclineRas;
        GLScanlineRasToDestBitmapRenderer sclineRasToGL;
        GLScanlinePacked8 sclinePack8;
        Arc arcTool = new Arc();
        CurveFlattener curveFlattener = new CurveFlattener();
        GLTextPrinter textPriner;
        int canvasOriginX = 0;
        int canvasOriginY = 0;
        int canvasW;
        int canvasH;
        MyMat4 orthoView;
        public CanvasGL2d(int canvasW, int canvasH)
        {
            this.canvasW = canvasW;
            this.canvasH = canvasH;
            sclineRas = new GLScanlineRasterizer();
            basicShader = new BasicShader();
            basicShader.InitShader();
            sclineRasToGL = new GLScanlineRasToDestBitmapRenderer(basicShader);
            sclinePack8 = new GLScanlinePacked8();
            tessListener.Connect(tess, Tesselate.Tesselator.WindingRuleType.Odd, true);
            textPriner = new GLTextPrinter(this);
            SetupFonts();
            ////--------------------------------------------------------------------------------
            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.ClearColor(1, 1, 1, 1);
            ////setup viewport size
            int max = Math.Max(canvasW, canvasH);
            ////square viewport
            //GL.Viewport(0, 0, max, max);
            orthoView = MyMat4.ortho(0, max, 0, max, 0, 1);
            ////-------------------------------------------------------------------------------
            sclineRasToGL.SetViewMatrix(orthoView);
        }
        public void Dispose()
        {
        }

        public CanvasSmoothMode SmoothMode
        {
            get;
            set;
        }
        public int Note1
        {
            get;
            set;
        }

        public void Clear(PixelFarm.Drawing.Color c)
        {
            //set value for clear color buffer

            GLHelper.ClearColor(c);
            GL.ClearStencil(0);
            //actual clear here !
            GL.Clear(ClearBufferMask.ColorBufferBit |
                ClearBufferMask.DepthBufferBit |
                ClearBufferMask.StencilBufferBit);
        }
        public double StrokeWidth
        {
            get { return this.aggStroke.Width; }
            set
            {
                //agg stroke
                this.aggStroke.Width = value;
            }
        }
        public PixelFarm.Drawing.Color StrokeColor
        {
            get { return this.strokeColor; }
            set { this.strokeColor = value; }
        }
        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        //--------------------------------------
                        ps.Clear();
                        ps.MoveTo(x1, y1);
                        ps.LineTo(x2, y2);
                        VertexStore vxs = aggStroke.MakeVxs(ps.Vxs);
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.strokeColor);
                        //--------------------------------------
                    }
                    break;
                default:
                    {
                        this.basicShader.DrawLine(x1, y1, x2, y2, this.strokeColor);
                    }
                    break;
            }
        }

        //-------------------------------------------------------------------------------
        public void DrawImage(GLBitmap bmp, float x, float y)
        {
            DrawImage(bmp,
                   new PixelFarm.Drawing.RectangleF(0, 0, bmp.Width, bmp.Height),
                   x, y, bmp.Width, bmp.Height);
        }
        public void DrawImage(GLBitmap bmp, float x, float y, float w, float h)
        {
            DrawImage(bmp,
                new PixelFarm.Drawing.RectangleF(0, 0, bmp.Width, bmp.Height),
                x, y, w, h);
        }
        public void DrawImage(GLBitmap bmp,
            PixelFarm.Drawing.RectangleF srcRect,
            float x, float y, float w, float h)
        {
            unsafe
            {
                GL.Enable(EnableCap.Texture2D);
                {
                    GL.BindTexture(TextureTarget.Texture2D, bmp.GetServerTextureId());
                    throw new NotSupportedException();
                    //GL.EnableClientState(ArrayCap.TextureCoordArray); //***

                    //texture source coord 1= 100% of original width
                    float* arr = stackalloc float[8];
                    float fullsrcW = bmp.Width;
                    float fullsrcH = bmp.Height;
                    if (bmp.IsInvert)
                    {
                        ////arr[0] = 0; arr[1] = 0;
                        arr[0] = srcRect.Left / fullsrcW; arr[1] = (srcRect.Top + srcRect.Height) / fullsrcH;
                        //arr[2] = 1; arr[3] = 0;
                        arr[2] = srcRect.Right / fullsrcW; arr[3] = (srcRect.Top + srcRect.Height) / fullsrcH;
                        //arr[4] = 1; arr[5] = 1;
                        arr[4] = srcRect.Right / fullsrcW; arr[5] = srcRect.Top / fullsrcH;
                        //arr[6] = 0; arr[7] = 1;
                        arr[6] = srcRect.Left / fullsrcW; arr[7] = srcRect.Top / fullsrcH;
                    }
                    else
                    {
                        arr[0] = srcRect.Left / fullsrcW; arr[1] = srcRect.Top / fullsrcH;
                        //arr[2] = 1; arr[3] = 1;
                        arr[2] = srcRect.Right / fullsrcW; arr[3] = srcRect.Top / fullsrcH;
                        //arr[4] = 1; arr[5] = 0;
                        arr[4] = srcRect.Right / fullsrcW; arr[5] = srcRect.Bottom / fullsrcH;
                        //arr[6] = 0; arr[7] = 0;
                        arr[6] = srcRect.Left / fullsrcW; arr[7] = srcRect.Bottom / fullsrcH;
                    }

                    //GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, (IntPtr)arr);
                    ////------------------------------------------ 
                    ////fill rect with texture 
                    //FillRectWithTexture(x, y, w, h);
                    //GL.DisableClientState(ArrayCap.TextureCoordArray);
                }
                GL.Disable(EnableCap.Texture2D);
            }
        }



        //-------------------------------------------------------------------------------
        public void DrawImage(GLBitmapReference bmp, float x, float y)
        {
            this.DrawImage(bmp.OwnerBitmap,
                 bmp.GetRectF(),
                 x, y, bmp.Width, bmp.Height);
        }
        //-------------------------------------------------------------------------------

        public void DrawImages(GLBitmap bmp, PixelFarm.Drawing.RectangleF[] destAndSrcPairs)
        {
            unsafe
            {
                GL.Enable(EnableCap.Texture2D);
                {
                    throw new NotSupportedException();
                    //GL.BindTexture(TextureTarget.Texture2D, bmp.GetServerTextureId());
                    //GL.EnableClientState(ArrayCap.TextureCoordArray); //***

                    ////texture source coord 1= 100% of original width
                    //float* arr = stackalloc float[8];
                    //float fullsrcW = bmp.Width;
                    //float fullsrcH = bmp.Height;

                    //int len = destAndSrcPairs.Length;
                    //if (len > 1)
                    //{
                    //    if ((len % 2 != 0))
                    //    {
                    //        len -= 1;
                    //    }
                    //    for (int i = 0; i < len; )
                    //    {
                    //        //each 

                    //        var destRect = destAndSrcPairs[i];
                    //        var srcRect = destAndSrcPairs[i + 1];
                    //        i += 2;

                    //        if (bmp.IsInvert)
                    //        {

                    //            ////arr[0] = 0; arr[1] = 0;
                    //            arr[0] = srcRect.Left / fullsrcW; arr[1] = (srcRect.Top + srcRect.Height) / fullsrcH;
                    //            //arr[2] = 1; arr[3] = 0;
                    //            arr[2] = srcRect.Right / fullsrcW; arr[3] = (srcRect.Top + srcRect.Height) / fullsrcH;
                    //            //arr[4] = 1; arr[5] = 1;
                    //            arr[4] = srcRect.Right / fullsrcW; arr[5] = srcRect.Top / fullsrcH;
                    //            //arr[6] = 0; arr[7] = 1;
                    //            arr[6] = srcRect.Left / fullsrcW; arr[7] = srcRect.Top / fullsrcH;
                    //        }
                    //        else
                    //        {

                    //            arr[0] = srcRect.Left / fullsrcW; arr[1] = srcRect.Top / fullsrcH;
                    //            //arr[2] = 1; arr[3] = 1;
                    //            arr[2] = srcRect.Right / fullsrcW; arr[3] = srcRect.Top / fullsrcH;
                    //            //arr[4] = 1; arr[5] = 0;
                    //            arr[4] = srcRect.Right / fullsrcW; arr[5] = srcRect.Bottom / fullsrcH;
                    //            //arr[6] = 0; arr[7] = 0;
                    //            arr[6] = srcRect.Left / fullsrcW; arr[7] = srcRect.Bottom / fullsrcH;
                    //        }
                    //        GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, (IntPtr)arr);
                    //        //------------------------------------------ 
                    //        //fill rect with texture                             
                    //        FillRectWithTexture(destRect.X, destRect.Y, destRect.Width, destRect.Height);
                    //    }
                    //} 
                    // GL.DisableClientState(ArrayCap.TextureCoordArray);
                }
                GL.Disable(EnableCap.Texture2D);
            }
        }

        public void FillVxs(PixelFarm.Drawing.Color color, VertexStore vxs)
        {
            //solid brush
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, color);
                    }
                    break;
                default:
                    {
                        //tess the vxs first
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, color);
                        //throw new NotSupportedException();
                    }
                    break;
            }
        }
        public void FillVxs(PixelFarm.Drawing.LinearGradientBrush brush, VertexStore vxs)
        {
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, brush.Color);
                    }
                    break;
                default:
                    {
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, brush.Color);
                    }
                    break;
            }
        }


        public void FillVxsSnap(PixelFarm.Drawing.Color color, VertexStoreSnap snap)
        {
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        sclineRas.Reset();
                        sclineRas.AddPath(snap);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, color);
                    }
                    break;
                default:
                    {
                        sclineRas.Reset();
                        sclineRas.AddPath(snap);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, color);
                    }
                    break;
            }
        }

        public void DrawVxs(VertexStore vxs)
        {
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        sclineRas.Reset();
                        sclineRas.AddPath(aggStroke.MakeVxs(vxs));
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.strokeColor);
                    }
                    break;
                default:
                    {
                        sclineRas.Reset();
                        sclineRas.AddPath(aggStroke.MakeVxs(vxs));
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.strokeColor);
                    }
                    break;
            }
        }

        public void DrawPolygon(float[] polygon2dVertices, int npoints)
        {
            //closed polyline
            //draw polyline
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        //draw polyon

                        ps.Clear();
                        //closed polygon
                        int j = npoints;
                        //first point
                        if (j < 2)
                        {
                            return;
                        }
                        ps.MoveTo(polygon2dVertices[0], polygon2dVertices[1]);
                        int nn = 2;
                        for (int i = 1; i < j; ++i)
                        {
                            ps.LineTo(polygon2dVertices[nn++],
                                polygon2dVertices[nn++]);
                        }
                        //close
                        ps.CloseFigure();
                        VertexStore vxs = aggStroke.MakeVxs(ps.Vxs);
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.strokeColor);
                        //--------------------------------------


                    }
                    break;
                default:
                    {
                        unsafe
                        {
                            fixed (float* arr = &polygon2dVertices[0])
                            {
                                DrawPolygonUnsafe(arr, npoints);
                            }
                        }
                    }
                    break;
            }
        }
        public void DrawEllipse(float x, float y, double rx, double ry)
        {
            ellipse.Reset(x, y, rx, ry);
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        VertexStore vxs = aggStroke.MakeVxs(ellipse.MakeVxs());
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.strokeColor);
                    }
                    break;
                default:
                    {
                        VertexStore vxs = ellipse.MakeVxs();
                        int n = vxs.Count;
                        unsafe
                        {
                            float* coords = stackalloc float[n * 2];
                            int i = 0;
                            int nn = 0;
                            double vx, vy;
                            var cmd = vxs.GetVertex(i, out vx, out vy);
                            while (i < n)
                            {
                                switch (cmd)
                                {
                                    case VertexCmd.MoveTo:
                                        {
                                            coords[nn++] = (float)vx;
                                            coords[nn++] = (float)vy;
                                        }
                                        break;
                                    case VertexCmd.LineTo:
                                        {
                                            coords[nn++] = (float)vx;
                                            coords[nn++] = (float)vy;
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
                                cmd = vxs.GetVertex(i, out vx, out vy);
                            }
                            //-------------------------------------- 
                            DrawPolygonUnsafe(coords, nn / 2);
                        }
                    }
                    break;
            }
        }
        public void DrawCircle(float x, float y, double radius)
        {
            DrawEllipse(x, y, radius, radius);
        }
        public void DrawRect(float x, float y, float w, float h)
        {
            //GL.EnableClientState(ArrayCap.ColorArray);
            //GL.EnableClientState(ArrayCap.VertexArray);
            //VboC4V3f vbo = GenerateVboC4V3f();
            //////points 
            //ArrayList<VertexC4V3f> vrx = new ArrayList<VertexC4V3f>();
            //CreatePolyLineRectCoords(vrx, this.strokeColor, x, y, w, h);
            //int pcount = vrx.Count;
            //vbo.BindBuffer();
            //DrawLineStripWithVertexBuffer(vrx, pcount);
            //vbo.UnbindBuffer();
            ////vbo.Dispose();
            //GL.DisableClientState(ArrayCap.ColorArray);
            //GL.DisableClientState(ArrayCap.VertexArray);

            CoordList2f coords = new CoordList2f();
            CreatePolyLineRectCoords(coords, x, y, w, h);
            //render
            this.basicShader.DrawLineStripsWithVertexBuffer(coords, coords.Count, this.strokeColor);
        }
        public void DrawRoundRect(float x, float y, float w, float h, float rx, float ry)
        {
            roundRect.SetRect(x, y, x + w, y + h);
            roundRect.SetRadius(rx, ry);
            var vxs = this.aggStroke.MakeVxs(roundRect.MakeVxs());
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.strokeColor);
                    }
                    break;
                default:
                    {
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.strokeColor);
                    }
                    break;
            }
        }



        static double DegToRad(double degree)
        {
            return degree * (Math.PI / 180d);
        }
        static double RadToDeg(double degree)
        {
            return degree * (180d / Math.PI);
        }

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
            VertexStore vxs = new VertexStore();
            bool stopLoop = false;
            foreach (VertexData vertexData in arcTool.GetVertexIter())
            {
                switch (vertexData.command)
                {
                    case VertexCmd.Stop:
                        stopLoop = true;
                        break;
                    default:
                        vxs.AddVertex(vertexData.x, vertexData.y, vertexData.command);
                        //yield return vertexData;
                        break;
                }
                //------------------------------
                if (stopLoop) { break; }
            }


            double scaleRatio = 1;
            if (centerFormArc.scaleUp)
            {
                int vxs_count = vxs.Count;
                double px0, py0, px_last, py_last;
                vxs.GetVertex(0, out px0, out py0);
                vxs.GetVertex(vxs_count - 1, out px_last, out py_last);
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
                    vxs = mat.TransformToVxs(vxs);
                }
                else
                {
                    //not scalue
                    var mat = PixelFarm.Agg.Transform.Affine.NewMatix(
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, -centerFormArc.cx, -centerFormArc.cy),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Rotate, DegToRad(xaxisRotationAngleDec)),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, centerFormArc.cx, centerFormArc.cy));
                    vxs = mat.TransformToVxs(vxs);
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
                    vxs = mat.TransformToVxs(vxs);
                }
            }

            vxs = aggStroke.MakeVxs(vxs);
            sclineRas.Reset();
            sclineRas.AddPath(vxs);
            sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.strokeColor);
        }

        struct CenterFormArc
        {
            public double cx;
            public double cy;
            public double radStartAngle;
            public double radSweepDiff;
            public bool scaleUp;
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


        public void DrawBezierCurve(float startX, float startY, float endX, float endY,
            float controlX1, float controlY1,
            float controlX2, float controlY2)
        {
            VertexStore vxs = new VertexStore();
            BezierCurve.CreateBezierVxs4(vxs,
                new PixelFarm.VectorMath.Vector2(startX, startY),
                new PixelFarm.VectorMath.Vector2(endX, endY),
                new PixelFarm.VectorMath.Vector2(controlX1, controlY1),
                new PixelFarm.VectorMath.Vector2(controlY2, controlY2));
            vxs = this.aggStroke.MakeVxs(vxs);
            sclineRas.Reset();
            sclineRas.AddPath(vxs);
            sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, this.strokeColor);
        }



        static VboC4V3f GenerateVboC4V3f()
        {
            VboC4V3f vboHandle = new VboC4V3f();
            //must open these ... before call this func
            //GL.EnableClientState(ArrayCap.ColorArray);
            //GL.EnableClientState(ArrayCap.VertexArray); 
            GL.GenBuffers(1, out vboHandle.VboID);
            return vboHandle;
        }

        static void DrawTrianglesWithVertexBuffer(ArrayList<VertexC4V3f> buffer, int nelements)
        {
            unsafe
            {
                VertexC4V3f[] vpoints = buffer.Array;
                IntPtr stride_size = new IntPtr(VertexC4V3f.SIZE_IN_BYTES * nelements);
                //GL.BufferData(BufferTarget.ArrayBuffer, stride_size, IntPtr.Zero, BufferUsageHint.StreamDraw);
                // Fill newly allocated buffer
                GL.BufferData(BufferTarget.ArrayBuffer, stride_size, vpoints, BufferUsage.StreamDraw);
                GL.DrawArrays(BeginMode.Triangles, 0, nelements);
            }
        }
        //static void DrawLinesWithVertexBuffer(ArrayList<VertexC4V3f> buffer, int nelements)
        //{
        //    unsafe
        //    {
        //        VertexC4V3f[] vpoints = buffer.Array;
        //        IntPtr stride_size = new IntPtr(VertexC4V3f.SIZE_IN_BYTES * nelements);
        //        GL.BufferData(BufferTarget.ArrayBuffer, stride_size, vpoints, BufferUsage.StreamDraw);
        //        GL.DrawArrays(BeginMode.Lines, 0, nelements);
        //    }
        //}
        //static void DrawLineStripWithVertexBuffer(ArrayList<VertexC4V3f> buffer, int nelements)
        //{
        //    unsafe
        //    {
        //        VertexC4V3f[] vpoints = buffer.Array;
        //        IntPtr stride_size = new IntPtr(VertexC4V3f.SIZE_IN_BYTES * nelements);
        //        GL.BufferData(BufferTarget.ArrayBuffer, stride_size, vpoints, BufferUsage.StreamDraw);
        //        GL.DrawArrays(BeginMode.LineStrip, 0, nelements);
        //    }
        //}
        void FillRectWithTexture(float x, float y, float w, float h)
        {
            unsafe
            {
                float* arr = stackalloc float[8];
                byte* indices = stackalloc byte[6];
                CreateRectCoords(arr, indices, x, y, w, h);
                throw new NotSupportedException();
                //GL.EnableClientState(ArrayCap.VertexArray);
                ////vertex
                //GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);
                //GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedByte, (IntPtr)indices);
                //GL.DisableClientState(ArrayCap.VertexArray);

            }
        }
        public void FillRect(PixelFarm.Drawing.Color color, float x, float y, float w, float h)
        {
            CoordList2f coords = new CoordList2f();
            CreateRectCoords(coords, x, y, w, h);
            this.basicShader.DrawTrianglesWithVertexBuffer(coords, coords.Count, color);
        }
        public void FillRect(PixelFarm.Drawing.LinearGradientBrush linearGradientBrush, float x, float y, float w, float h)
        {
            if (linearGradientBrush != null)
            {
                //use clip rect for fill rect gradient
                EnableClipRect();
                SetClipRect((int)x, (int)y, (int)w, (int)h);
                //early exit

                ////points 
                var colors = linearGradientBrush.GetColors();
                var points = linearGradientBrush.GetStopPoints();
                uint c1 = colors[0].ToABGR();
                uint c2 = colors[1].ToABGR();
                //create polygon for graident bg 
                var vrx = GLGradientColorProvider.CalculateLinearGradientVxs(
                     points[0].X, points[0].Y,
                     points[1].X, points[1].Y,
                     colors[0],
                     colors[1]);
                int pcount = vrx.Count;
                throw new NotSupportedException();
                //GL.EnableClientState(ArrayCap.ColorArray);
                //GL.EnableClientState(ArrayCap.VertexArray);

                //VboC4V3f vbo = GenerateVboC4V3f();
                //vbo.BindBuffer();
                //DrawTrianglesWithVertexBuffer(vrx, pcount);
                //vbo.UnbindBuffer();
                //DrawLineStripWithVertexBuffer()
                //GL.DisableClientState(ArrayCap.ColorArray);
                //GL.DisableClientState(ArrayCap.VertexArray);

                DisableClipRect();
            }
        }


        public void FillRoundRect(PixelFarm.Drawing.Color color, float x, float y, float w, float h, float rx, float ry)
        {
            roundRect.SetRect(x, y, x + w, y + h);
            roundRect.SetRadius(rx, ry);
            //create round rect vxs
            var vxs = roundRect.MakeVxs();
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.FillWithColor(sclineRas, sclinePack8, color);
                    }
                    break;
                default:
                    {
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.DrawWithColor(sclineRas, sclinePack8, color);
                    }
                    break;
            }
        }
        public void FillEllipse(PixelFarm.Drawing.Color color, float x, float y, float rx, float ry)
        {
            ellipse.Reset(x, y, rx, ry);
            var vxs = ellipse.MakeVxs();
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.FillWithColor(sclineRas, sclinePack8, color);
                    }
                    break;
                default:
                    {
                        //other mode
                        int n = vxs.Count;
                        //make triangular fan*** 
                        unsafe
                        {
                            float* coords = stackalloc float[(n * 2) + 4];
                            int i = 0;
                            int nn = 0;
                            int npoints = 0;
                            double vx, vy;
                            //center
                            coords[nn++] = (float)x;
                            coords[nn++] = (float)y;
                            npoints++;
                            var cmd = vxs.GetVertex(i, out vx, out vy);
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
                                cmd = vxs.GetVertex(i, out vx, out vy);
                            }
                            //close circle
                            coords[nn++] = coords[2];
                            coords[nn++] = coords[3];
                            npoints++;
                            ////fill triangular fan
                            //GL.EnableClientState(ArrayCap.VertexArray); //***
                            ////vertex 2d
                            //GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)coords);
                            //GL.DrawArrays(BeginMode.TriangleFan, 0, npoints);
                            //GL.DisableClientState(ArrayCap.VertexArray);

                            this.basicShader.FillTriangleFan(coords, npoints, color);
                        }
                    }
                    break;
            }
        }
        public void FillCircle(PixelFarm.Drawing.Color color, float x, float y, float radius)
        {
            FillEllipse(color, x, y, radius, radius);
        }

        public void FillPolygon(PixelFarm.Drawing.Color color, float[] vertex2dCoords)
        {
            FillPolygon(color, vertex2dCoords, vertex2dCoords.Length);
        }
        public void FillPolygon(PixelFarm.Drawing.Brush brush, float[] vertex2dCoords, int npoints)
        {
            //-------------
            //Tesselate
            //2d coods lis
            //n point 
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        //closed polygon

                        //closed polygon
                        int j = npoints / 2;
                        //first point
                        if (j < 2)
                        {
                            return;
                        }
                        ps.MoveTo(vertex2dCoords[0], vertex2dCoords[1]);
                        int nn = 2;
                        for (int i = 1; i < j; ++i)
                        {
                            ps.LineTo(vertex2dCoords[nn++],
                                vertex2dCoords[nn++]);
                        }
                        //close
                        ps.CloseFigure();
                        VertexStore vxs = ps.Vxs;
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        switch (brush.BrushKind)
                        {
                            case Drawing.BrushKind.Solid:
                                {
                                    var color = ((PixelFarm.Drawing.SolidBrush)brush).Color;
                                    sclineRasToGL.FillWithColor(sclineRas, sclinePack8, color);
                                }
                                break;
                            default:
                                {
                                }
                                break;
                        }
                    }
                    break;
                default:
                    {
                        var vertextList = TessPolygon(vertex2dCoords);
                        //-----------------------------   
                        //switch how to fill polygon
                        switch (brush.BrushKind)
                        {
                            case Drawing.BrushKind.LinearGradient:
                            case Drawing.BrushKind.Texture:
                                {
                                    var linearGradientBrush = brush as PixelFarm.Drawing.LinearGradientBrush;
                                    GL.ClearStencil(0); //set value for clearing stencil buffer 
                                    //actual clear here
                                    GL.Clear(ClearBufferMask.StencilBufferBit);
                                    //-------------------
                                    //disable rendering to color buffer
                                    GL.ColorMask(false, false, false, false);
                                    //start using stencil
                                    GL.Enable(EnableCap.StencilTest);
                                    //place a 1 where rendered
                                    GL.StencilFunc(StencilFunction.Always, 1, 1);
                                    //replace where rendered
                                    GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);
                                    //render  to stencill buffer
                                    //-----------------
                                    if (this.Note1 == 1)
                                    {
                                        ////create stencil with Agg shape
                                        int j = npoints / 2;
                                        //first point
                                        if (j < 2)
                                        {
                                            return;
                                        }
                                        ps.Clear();
                                        ps.MoveTo(vertex2dCoords[0], vertex2dCoords[1]);
                                        int nn = 2;
                                        for (int i = 1; i < j; ++i)
                                        {
                                            ps.LineTo(vertex2dCoords[nn++],
                                                vertex2dCoords[nn++]);
                                        }
                                        //close
                                        ps.CloseFigure();
                                        VertexStore vxs = ps.Vxs;
                                        sclineRas.Reset();
                                        sclineRas.AddPath(vxs);
                                        sclineRasToGL.FillWithColor(sclineRas, sclinePack8, PixelFarm.Drawing.Color.White);
                                        //create stencil with normal OpenGL 
                                    }
                                    else
                                    {
                                        //create stencil with normal OpenGL
                                        int j = vertextList.Count;
                                        int j2 = j * 2;
                                        VboC4V3f vbo = GenerateVboC4V3f();
                                        ArrayList<VertexC4V3f> vrx = new ArrayList<VertexC4V3f>();
                                        uint color_uint = PixelFarm.Drawing.Color.Black.ToABGR();   //color.ToABGR();
                                        for (int i = 0; i < j; ++i)
                                        {
                                            var v = vertextList[i];
                                            vrx.AddVertex(new VertexC4V3f(color_uint, (float)v.m_X, (float)v.m_Y));
                                        }
                                        throw new NotSupportedException();
                                        //GL.EnableClientState(ArrayCap.ColorArray);
                                        //GL.EnableClientState(ArrayCap.VertexArray);
                                        //int pcount = vrx.Count;
                                        //vbo.BindBuffer();
                                        //DrawTrianglesWithVertexBuffer(vrx, pcount);
                                        //vbo.UnbindBuffer();
                                        //GL.DisableClientState(ArrayCap.ColorArray);
                                        //GL.DisableClientState(ArrayCap.VertexArray);
                                    }
                                    //-------------------------------------- 
                                    //render color
                                    //--------------------------------------  
                                    //reenable color buffer 
                                    GL.ColorMask(true, true, true, true);
                                    //where a 1 was not rendered
                                    GL.StencilFunc(StencilFunction.Equal, 1, 1);
                                    //freeze stencill buffer
                                    GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
                                    if (this.Note1 == 1) //temp
                                    {
                                        //------------------------------------------
                                        //we already have valid ps from stencil step
                                        //------------------------------------------
                                        VertexStore vxs = ps.Vxs;
                                        sclineRas.Reset();
                                        sclineRas.AddPath(vxs);
                                        //-------------------------------------------------------------------------------------
                                        //1.  we draw only alpha chanel of this black color to destination color
                                        //so we use  BlendFuncSeparate  as follow ... 
                                        GL.BlendFuncSeparate(
                                             BlendingFactorSrc.DstColor, BlendingFactorDest.DstColor, //the same
                                             BlendingFactorSrc.One, BlendingFactorDest.Zero); //use alpha chanel from source
                                        sclineRasToGL.FillWithColor(sclineRas, sclinePack8, PixelFarm.Drawing.Color.Black);
                                        //at this point alpha component is fill in to destination 
                                        //-------------------------------------------------------------------------------------
                                        //2. then fill again!, 
                                        //we use alpha information from dest, 
                                        //so we set blend func to ... GL.BlendFunc(BlendingFactorSrc.DstAlpha, BlendingFactorDest.OneMinusDstAlpha)    

                                        GL.BlendFunc(BlendingFactorSrc.DstAlpha, BlendingFactorDest.OneMinusDstAlpha);
                                        {
                                            //draw box of gradient color
                                            if (brush.BrushKind == Drawing.BrushKind.LinearGradient)
                                            {
                                                var colors = linearGradientBrush.GetColors();
                                                var points = linearGradientBrush.GetStopPoints();
                                                uint c1 = colors[0].ToABGR();
                                                uint c2 = colors[1].ToABGR();
                                                //create polygon for graident bg 
                                                var vrx = GLGradientColorProvider.CalculateLinearGradientVxs(
                                                     points[0].X, points[0].Y,
                                                     points[1].X, points[1].Y,
                                                     colors[0],
                                                     colors[1]);
                                                int pcount = vrx.Count;
                                                throw new NotSupportedException();
                                                //GL.EnableClientState(ArrayCap.ColorArray);
                                                //GL.EnableClientState(ArrayCap.VertexArray);
                                                ////--- 
                                                //VboC4V3f vbo = GenerateVboC4V3f();
                                                //vbo.BindBuffer();
                                                //DrawTrianglesWithVertexBuffer(vrx, pcount);
                                                //vbo.UnbindBuffer();
                                                ////vbo.Dispose();
                                                //GL.DisableClientState(ArrayCap.ColorArray);
                                                //GL.DisableClientState(ArrayCap.VertexArray);
                                            }
                                            else if (brush.BrushKind == Drawing.BrushKind.Texture)
                                            {
                                                //draw texture image 
                                                PixelFarm.Drawing.TextureBrush tbrush = (PixelFarm.Drawing.TextureBrush)brush;
                                                PixelFarm.Drawing.Image img = tbrush.TextureImage;
                                                GLBitmap bmpTexture = (GLBitmap)tbrush.InnerImage2;
                                                this.DrawImage(bmpTexture, 0, 0);
                                                //GLBitmapTexture bmp = GLBitmapTexture.CreateBitmapTexture(fontGlyph.glyphImage32);
                                                //this.DrawImage(bmp, 0, 0);
                                                //bmp.Dispose();

                                            }
                                        }
                                        //restore back 
                                        //3. switch to normal blending mode 
                                        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                                    }
                                    else
                                    {
                                        //draw box of gradient color
                                        var colors = linearGradientBrush.GetColors();
                                        var points = linearGradientBrush.GetStopPoints();
                                        uint c1 = colors[0].ToABGR();
                                        uint c2 = colors[1].ToABGR();
                                        //create polygon for graident bg 
                                        var vrx = GLGradientColorProvider.CalculateLinearGradientVxs(
                                             points[0].X, points[0].Y,
                                             points[1].X, points[1].Y,
                                             colors[0],
                                             colors[1]);
                                        int pcount = vrx.Count;
                                        throw new NotSupportedException();
                                        //GL.EnableClientState(ArrayCap.ColorArray);
                                        //GL.EnableClientState(ArrayCap.VertexArray);
                                        ////--- 
                                        //VboC4V3f vbo = GenerateVboC4V3f();
                                        //vbo.BindBuffer();
                                        //DrawTrianglesWithVertexBuffer(vrx, pcount);
                                        //vbo.UnbindBuffer();
                                        ////vbo.Dispose();
                                        //GL.DisableClientState(ArrayCap.ColorArray);
                                        //GL.DisableClientState(ArrayCap.VertexArray);

                                    }
                                    GL.Disable(EnableCap.StencilTest);
                                }
                                break;
                            default:
                                {
                                    //unknown brush
                                    //int j = vertextList.Count;
                                    //int j2 = j * 2;
                                    //VboC4V3f vbo = GenerateVboC4V3f();
                                    //ArrayList<VertexC4V3f> vrx = new ArrayList<VertexC4V3f>();
                                    //uint color_int = color.ToABGR();
                                    //for (int i = 0; i < j; ++i)
                                    //{
                                    //    var v = vertextList[i];
                                    //    vrx.AddVertex(new VertexC4V3f(color_int, (float)v.m_X, (float)v.m_Y));
                                    //}
                                    ////------------------------------------- 
                                    //GL.EnableClientState(ArrayCap.ColorArray);
                                    //GL.EnableClientState(ArrayCap.VertexArray);
                                    //int pcount = vrx.Count;
                                    //vbo.BindBuffer();
                                    //DrawTrianglesWithVertexBuffer(vrx, pcount);
                                    //vbo.UnbindBuffer();
                                    //GL.DisableClientState(ArrayCap.ColorArray);
                                    //GL.DisableClientState(ArrayCap.VertexArray);
                                    ////-------------------------------------- 
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        public void FillPolygon(PixelFarm.Drawing.Color color, float[] vertex2dCoords, int npoints)
        {
            //-------------
            //Tesselate
            //2d coods lis
            //n point 
            switch (this.SmoothMode)
            {
                case CanvasSmoothMode.AggSmooth:
                    {
                        //closed polygon

                        //closed polygon
                        int j = npoints / 2;
                        //first point
                        if (j < 2)
                        {
                            return;
                        }
                        ps.MoveTo(vertex2dCoords[0], vertex2dCoords[1]);
                        int nn = 2;
                        for (int i = 1; i < j; ++i)
                        {
                            ps.LineTo(vertex2dCoords[nn++],
                                vertex2dCoords[nn++]);
                        }
                        //close
                        ps.CloseFigure();
                        VertexStore vxs = ps.Vxs;
                        sclineRas.Reset();
                        sclineRas.AddPath(vxs);
                        sclineRasToGL.FillWithColor(sclineRas, sclinePack8, color);
                    }
                    break;
                default:
                    {
                        var vertextList = TessPolygon(vertex2dCoords);
                        //-----------------------------   
                        //switch how to fill polygon
                        int j = vertextList.Count;
                        //-----------------------------   
                        //VboC4V3f vbo = GenerateVboC4V3f();
                        //ArrayList<VertexC4V3f> vrx = new ArrayList<VertexC4V3f>();
                        //uint color_int = color.ToABGR();
                        unsafe
                        {
                            float* vtx = stackalloc float[j * 2];
                            int n = 0;
                            for (int i = 0; i < j; ++i)
                            {
                                var v = vertextList[i];
                                vtx[n] = (float)v.m_X;
                                vtx[n + 1] = (float)v.m_Y;
                                n += 2;
                            }
                            //------------------------------------- 
                            this.basicShader.FillTriangles(vtx, j, color);
                        }
                        //GL.EnableClientState(ArrayCap.ColorArray);
                        //GL.EnableClientState(ArrayCap.VertexArray);
                        //int pcount = vrx.Count;
                        //vbo.BindBuffer();
                        //DrawTrianglesWithVertexBuffer(vrx, pcount);
                        //vbo.UnbindBuffer();

                        //GL.DisableClientState(ArrayCap.ColorArray);
                        //GL.DisableClientState(ArrayCap.VertexArray);
                        //--------------------------------------  
                    }
                    break;
            }
        }
        //-----------------------------------------------------


        public int CanvasOriginX
        {
            get { return this.canvasOriginX; }
        }
        public int CanvasOriginY
        {
            get { return this.canvasOriginY; }
        }

        public void SetCanvasOrigin(int x, int y)
        {
            int originalW = 800;
            //set new viewport
            GL.Viewport(x, y, originalW, originalW);
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(0, originalW, 0, originalW, 0.0, 100.0);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadIdentity();
        }
        public void EnableClipRect()
        {
            GL.Enable(EnableCap.ScissorTest);
        }
        public void DisableClipRect()
        {
            GL.Disable(EnableCap.ScissorTest);
        }
        public void SetClipRect(int x, int y, int w, int h)
        {
            GL.Scissor(x, y, w, h);
        }
    }
}