//MIT 2014, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using PixelFarm.Agg;
using PixelFarm.Agg.VertexSource;

using OpenTK.Graphics.OpenGL;

namespace LayoutFarm.DrawingGL
{

    partial class CanvasGL2d
    {

        public LayoutFarm.Drawing.Color StrokeColor
        {
            get { return this.strokeColor; }
            set
            {
                this.strokeColor = value;
                GL.Color4(value.R, value.G, value.B, value.A);
            }
        }
        public void Clear(LayoutFarm.Drawing.Color c)
        {
            //set value for clear color buffer
            GL.ClearColor(
                (float)c.R / 255f,
                 (float)c.G / 255f,
                 (float)c.B / 255f,
                 (float)c.A / 255f);

            GL.ClearStencil(0);
            //actual clear here 
            GL.Clear(ClearBufferMask.ColorBufferBit |
                ClearBufferMask.DepthBufferBit |
                ClearBufferMask.StencilBufferBit);
        }
        public LayoutFarm.Drawing.CanvasOrientation Orientation
        {
            get { return this.canvasOrientation; }
            set
            {
                this.canvasOrientation = value;
                this.SetCanvasOrigin(this.canvasOriginX, this.canvasOriginY);
            }
        }
        public void SetCanvasOrigin(int x, int y)
        {
            this.canvasOriginX = x;
            this.canvasOriginY = y;
            int properW = Math.Min(this.canvasW, this.canvasH);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            switch (this.canvasOrientation)
            {
                case Drawing.CanvasOrientation.LeftTop:
                    {
                        GL.Ortho(0, properW, properW, 0, 0.0, 100);
                    } break;
                default:
                    {
                        GL.Ortho(0, properW, 0, properW, 0.0, 100);
                    } break;
            }

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(x, y, 0);
        }
        public void EnableClipRect()
        {
            GL.Enable(EnableCap.ScissorTest);
        }
        public void DisableClipRect()
        {
            GL.Disable(EnableCap.ScissorTest);
        }
        public void SetClipRectRel(int x, int y, int w, int h)
        {
            //OpenGL clip is relative to screen
            //not affected by coord-transform matrix? 
            switch (this.canvasOrientation)
            {
                case Drawing.CanvasOrientation.LeftTop:
                    {
                        //convert to left bottom mode 
                        GL.Scissor(this.canvasOriginX + x,
                           (this.canvasH - (y + h + this.canvasOriginY)), //flip Y --> to bootom 
                            w,
                            h);
                    } break;
                default:
                    {
                        GL.Scissor(this.canvasOriginX + x, this.canvasOriginY + y, w, h);
                    } break;
            }

        }
        public void FillPolygon(LayoutFarm.Drawing.Brush brush, float[] vertex2dCoords, int npoints)
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
                                    var color = ((LayoutFarm.Drawing.SolidBrush)brush).Color;
                                    sclineRasToGL.FillWithColor(sclineRas, sclinePack8, color);

                                } break;
                            default:
                                {
                                } break;
                        }

                    } break;
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
                                    var linearGradientBrush = brush as LayoutFarm.Drawing.LinearGradientBrush;
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
                                            ps.LineTo(
                                                vertex2dCoords[nn++],
                                                vertex2dCoords[nn++]);
                                        }
                                        //close
                                        ps.CloseFigure();

                                        VertexStore vxs = ps.Vxs;
                                        sclineRas.Reset();
                                        sclineRas.AddPath(vxs);
                                        sclineRasToGL.FillWithColor(sclineRas, sclinePack8, LayoutFarm.Drawing.Color.White);
                                        //create stencil with normal OpenGL 
                                    }
                                    else
                                    {
                                        //create stencil with normal OpenGL
                                        int j = vertextList.Count;
                                        int j2 = j * 2;
                                        //VboC4V3f vbo = GenerateVboC4V3f();
                                        ArrayList<VertexC4V2f> vrx = new ArrayList<VertexC4V2f>();
                                        uint color_uint = LayoutFarm.Drawing.Color.Black.ToABGR();   //color.ToABGR();
                                        for (int i = 0; i < j; ++i)
                                        {
                                            var v = vertextList[i];
                                            vrx.AddVertex(new VertexC4V2f(color_uint, (float)v.m_X, (float)v.m_Y));
                                        }

                                        DrawVertexList(DrawMode.Triangles, vrx, vrx.Count);
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
                                        //1.  we draw only alpha channel of this black color to destination color
                                        //so we use  BlendFuncSeparate  as follow ... 
                                        GL.ColorMask(false, false, false, true);
                                        //GL.BlendFuncSeparate(
                                        //     BlendingFactorSrc.DstColor, BlendingFactorDest.DstColor, //the same
                                        //     BlendingFactorSrc.One, BlendingFactorDest.Zero);

                                        //use alpha chanel from source***
                                        GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.Zero);
                                        sclineRasToGL.FillWithColor(sclineRas, sclinePack8, LayoutFarm.Drawing.Color.Black);

                                        //at this point alpha component is fill in to destination 
                                        //-------------------------------------------------------------------------------------
                                        //2. then fill again!, 
                                        //we use alpha information from dest, 
                                        //so we set blend func to ... GL.BlendFunc(BlendingFactorSrc.DstAlpha, BlendingFactorDest.OneMinusDstAlpha)    
                                        GL.ColorMask(true, true, true, true);
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

                                                ArrayList<VertexC4V2f>
                                                     vrx = GLGradientColorProvider.CalculateLinearGradientVxs(
                                                     points[0].X, points[0].Y,
                                                     points[1].X, points[1].Y,
                                                     colors[0],
                                                     colors[1]);

                                                DrawVertexList(DrawMode.Triangles, vrx, vrx.Count);

                                            }
                                            else if (brush.BrushKind == Drawing.BrushKind.Texture)
                                            {
                                                //draw texture image 
                                                LayoutFarm.Drawing.TextureBrush tbrush = (LayoutFarm.Drawing.TextureBrush)brush;
                                                LayoutFarm.Drawing.Image img = tbrush.TextureImage;
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
                                        DrawVertexList(DrawMode.Triangles, vrx, vrx.Count);
                                    }
                                    GL.Disable(EnableCap.StencilTest);
                                } break;
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
                                } break;
                        }


                    } break;
            }
        }

        public void DrawImages(GLBitmap bmp, LayoutFarm.Drawing.RectangleF[] destAndSrcPairs)
        {

            unsafe
            {

                GL.Enable(EnableCap.Texture2D);
                {
                    GL.BindTexture(TextureTarget.Texture2D, bmp.GetServerTextureId());
                    GL.EnableClientState(ArrayCap.TextureCoordArray); //***

                    //texture source coord 1= 100% of original width
                    float* arr = stackalloc float[8];
                    float fullsrcW = bmp.Width;
                    float fullsrcH = bmp.Height;

                    int len = destAndSrcPairs.Length;
                    if (len > 1)
                    {
                        if ((len % 2 != 0))
                        {
                            len -= 1;
                        }
                        if (this.canvasOrientation == Drawing.CanvasOrientation.LeftTop)
                        {
                            for (int i = 0; i < len; )
                            {
                                //each 

                                var destRect = destAndSrcPairs[i];
                                var srcRect = destAndSrcPairs[i + 1];
                                i += 2;

                                if (!bmp.IsInvert)
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
                                GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, (IntPtr)arr);
                                //------------------------------------------ 
                                //fill rect with texture                             
                                FillRectWithTexture(destRect.X, destRect.Y, destRect.Width, destRect.Height);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < len; )
                            {
                                //each 

                                var destRect = destAndSrcPairs[i];
                                var srcRect = destAndSrcPairs[i + 1];
                                i += 2;

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
                                GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, (IntPtr)arr);
                                //------------------------------------------ 
                                //fill rect with texture                             
                                FillRectWithTexture(destRect.X, destRect.Y, destRect.Width, destRect.Height);
                            }
                        }

                    }

                    GL.DisableClientState(ArrayCap.TextureCoordArray);
                }
                GL.Disable(EnableCap.Texture2D);
            }
        }

        public void DrawImage(GLBitmap bmp,
           LayoutFarm.Drawing.RectangleF srcRect,
           float x, float y, float w, float h)
        {
            unsafe
            {

                GL.Enable(EnableCap.Texture2D);
                {
                    GL.BindTexture(TextureTarget.Texture2D, bmp.GetServerTextureId());
                    GL.EnableClientState(ArrayCap.TextureCoordArray); //***

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
                    GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, (IntPtr)arr);
                    //------------------------------------------ 
                    //fill rect with texture 
                    FillRectWithTexture(x, y, w, h);
                    GL.DisableClientState(ArrayCap.TextureCoordArray);
                }
                GL.Disable(EnableCap.Texture2D);
            }
        }

        enum DrawMode
        {
            Points = OpenTK.Graphics.OpenGL.BeginMode.Points,
            Lines = OpenTK.Graphics.OpenGL.BeginMode.Lines,
            LineLoop = OpenTK.Graphics.OpenGL.BeginMode.LineLoop,
            LineStrips = OpenTK.Graphics.OpenGL.BeginMode.LineStrip,
            Triangles = OpenTK.Graphics.OpenGL.BeginMode.Triangles,
            TriangleFan = OpenTK.Graphics.OpenGL.BeginMode.TriangleFan,
        }

        static void DrawVertexList(DrawMode mode, ArrayList<VertexC4V2f> buffer, int nelements)
        {
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            unsafe
            {

                VertexC4V2f[] vtx = buffer.Array;
                fixed (void* h = &vtx[0])
                {
                    byte* byteH = (byte*)h;
                    GL.ColorPointer(4, ColorPointerType.UnsignedByte, VertexC4V2f.SIZE_IN_BYTES, (IntPtr)byteH);
                    GL.VertexPointer(VertexC4V2f.N_COORDS,
                        VertexC4V2f.VX_PTR_TYPE,
                        VertexC4V2f.SIZE_IN_BYTES,
                        (IntPtr)(byteH + VertexC4V2f.VX_OFFSET));
                }
                GL.DrawArrays((BeginMode)mode, 0, nelements);

            }
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.VertexArray);
        }


        void FillRectWithTexture(float x, float y, float w, float h)
        {
            unsafe
            {
                float* arr = stackalloc float[8];
                byte* indices = stackalloc byte[6];
                CreateRectCoords(arr, indices, x, y, w, h);
                GL.EnableClientState(ArrayCap.VertexArray);
                //vertex
                GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);
                GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedByte, (IntPtr)indices);
                GL.DisableClientState(ArrayCap.VertexArray);
            }
        }

        static unsafe void UnsafeDrawV2fList(DrawMode mode, float* polygon2dVertices, int vertexCount)
        {
            //1. enable client side memory
            GL.EnableClientState(ArrayCap.VertexArray); //***
            //2. load data from point to vertex array part
            GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)polygon2dVertices);
            //3. draw array
            GL.DrawArrays((BeginMode)mode, 0, vertexCount);
            //4. disable client side memory
            GL.DisableClientState(ArrayCap.VertexArray);
        }
        static unsafe void UnsafeDrawV2fList(DrawMode mode, float* polygon2dVertices, int vertexCount, LayoutFarm.Drawing.Color c)
        {
            GL.Color4(c.R, c.G, c.B, c.A);
            //1. enable client side memory
            GL.EnableClientState(ArrayCap.VertexArray); //***
            //2. load data from point to vertex array part
            GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)polygon2dVertices);
            //3. draw array
            GL.DrawArrays((BeginMode)mode, 0, vertexCount);
            //4. disable client side memory
            GL.DisableClientState(ArrayCap.VertexArray);
        }
    }
}