//2014 BSD,WinterDev   

//MatterHackers
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------

using OpenTK.Graphics.ES20;
using PixelFarm.DrawingGL;
namespace PixelFarm.Agg
{
    /// <summary
    /// to bitmap
    /// </summary>  
    class GLScanlineRasToDestBitmapRenderer
    {
        BasicShader scanlineShader;
        AggCoordList3f myLineBuffer = new AggCoordList3f();
        public GLScanlineRasToDestBitmapRenderer(BasicShader scanlineShader)
        {
            this.scanlineShader = scanlineShader;
        }
        internal void SetViewMatrix(MyMat4 mat)
        {
            scanlineShader.ViewMatrix = mat;
        }
        /// <summary>
        /// for fill shape
        /// </summary>
        /// <param name="sclineRas"></param>
        /// <param name="scline"></param>
        /// <param name="color"></param>
        /// <param name="shapeHint"></param>
        public void FillWithColor(GLScanlineRasterizer sclineRas,
                GLScanline scline,
                PixelFarm.Drawing.Color color)
        {
            //early exit
            if (color.A == 0) { return; }
            if (!sclineRas.RewindScanlines()) { return; }
            //----------------------------------------------- 

            scline.ResetSpans(sclineRas.MinX, sclineRas.MaxX);
            //-----------------------------------------------  
            var lineBuff = this.myLineBuffer;
            lineBuff.Clear();
            while (sclineRas.SweepScanline(scline))
            {
                int y = scline.Y;
                lineBuff.BeginNewLine(y);
                int num_spans = scline.SpanCount;
                byte[] covers = scline.GetCovers();
                //copy data from scanline to lineBuff
                //TODO: move linebuf built into the scanline?

                for (int i = 1; i <= num_spans; ++i)
                {
                    ScanlineSpan span = scline.GetSpan(i);
                    if (span.len > 0)
                    {
                        //outline
                        GLBlendSolidHSpan(span.x, span.len, true, lineBuff, color.A, covers, span.cover_index);
                    }
                    else
                    {
                        //fill
                        int x = span.x;
                        int x2 = (x - span.len - 1);
                        GLBlendHLine(x, x2, true, lineBuff, color.A, covers[span.cover_index]);
                    }
                }

                lineBuff.CloseLine();
            }
            //----------------------------------
            int nelements = myLineBuffer.Count;
            if (nelements > 0)
            {
                this.scanlineShader.AggDrawLines(myLineBuffer, nelements, color);
            }
        }

        /// <summary>
        /// for lines
        /// </summary>
        /// <param name="sclineRas"></param>
        /// <param name="scline"></param>
        /// <param name="color"></param>
        /// <param name="shapeHint"></param>
        public void DrawWithColor(GLScanlineRasterizer sclineRas,
                GLScanline scline,
                PixelFarm.Drawing.Color color)
        {
            //early exit
            if (color.A == 0) { return; }
            if (!sclineRas.RewindScanlines()) { return; }
            //----------------------------------------------- 

            scline.ResetSpans(sclineRas.MinX, sclineRas.MaxX);
            //-----------------------------------------------  
            var lineBuff = this.myLineBuffer;
            lineBuff.Clear();
            while (sclineRas.SweepScanline(scline))
            {
                int y = scline.Y;
                lineBuff.BeginNewLine(y);
                int num_spans = scline.SpanCount;
                byte[] covers = scline.GetCovers();
                //copy data from scanline to lineBuff
                //TODO: move linebuf built into the scanline?

                for (int i = 1; i <= num_spans; ++i)
                {
                    ScanlineSpan span = scline.GetSpan(i);
                    if (span.len > 0)
                    {
                        //outline
                        GLBlendSolidHSpan(span.x, span.len, false, lineBuff, color.A, covers, span.cover_index);
                    }
                    else
                    {
                        //fill
                        int x = span.x;
                        int x2 = (x - span.len - 1);
                        GLBlendHLine(x, x2, false, lineBuff, color.A, covers[span.cover_index]);
                    }
                }

                lineBuff.CloseLine();
            }
            //----------------------------------
            int nelements = myLineBuffer.Count;
            if (nelements > 0)
            {
                this.scanlineShader.AggDrawLines(myLineBuffer, nelements, color);
            }
        }
        const int BASE_MASK = 255;
        static void GLBlendHLine(int x1, int x2, bool isFillMode, AggCoordList3f lineBuffer, int srcColorAlpha, byte cover)
        {
            //if (color.A == 0) { return; }

            int len = x2 - x1 + 1;
            int alpha = (((int)(srcColorAlpha) * (cover + 1)) >> 8);
            if (isFillMode)
            {
                //same alpha...
                if (alpha == BASE_MASK)
                {
                    switch (len)
                    {
                        case 0:
                            {
                            }
                            break;
                        case 1:
                            {
                                //single px
                                lineBuffer.AddCoord(x1 - 1, alpha);
                                lineBuffer.AddCoord(x1, alpha);
                                lineBuffer.AddCoord(x1 + 1, alpha);
                            }
                            break;
                        default:
                            {
                                //var c = PixelFarm.Drawing.Color.FromArgb(alpha, color).ToARGB();
                                //lineBuffer.AddVertex(new VertexV2S1Cvr(x1, y, alpha));
                                //lineBuffer.AddVertex(new VertexV2S1Cvr(x2 + 1, y, alpha));
                                lineBuffer.AddCoord(x1 - 1, alpha);
                                lineBuffer.AddCoord(x1, alpha);
                                lineBuffer.AddCoord(x2 + 1, alpha);
                                //lineBuffer.AddCoord(x2 + 2, 0);
                                //lineBuffer.AddCoord(x2 + 2, 0);
                            }
                            break;
                    }
                }
                else
                {
                    //same alpha
                    lineBuffer.AddCoord(x1 - 1, alpha);
                    lineBuffer.AddCoord(x1, alpha);
                    lineBuffer.AddCoord(x2 + 1, alpha);
                    //lineBuffer.AddCoord(x2 + 2, 0);
                }
            }
            else
            {
                //same alpha...
                if (alpha == BASE_MASK)
                {
                    switch (len)
                    {
                        case 0:
                            {
                            }
                            break;
                        case 1:
                            {
                                //single px
                                lineBuffer.AddCoord(x1 - 1, 0);
                                lineBuffer.AddCoord(x1, alpha);
                                //lineBuffer.AddCoord(x1 + 1, alpha);
                            }
                            break;
                        default:
                            {
                                //var c = PixelFarm.Drawing.Color.FromArgb(alpha, color).ToARGB();
                                //lineBuffer.AddVertex(new VertexV2S1Cvr(x1, y, alpha));
                                //lineBuffer.AddVertex(new VertexV2S1Cvr(x2 + 1, y, alpha));
                                lineBuffer.AddCoord(x1 - 1, 0);
                                lineBuffer.AddCoord(x1, alpha);
                                lineBuffer.AddCoord(x2 + 1, alpha);
                                lineBuffer.AddCoord(x2 + 2, 0);
                            }
                            break;
                    }
                }
                else
                {
                    //same alpha
                    lineBuffer.AddCoord(x1 - 1, 0);
                    lineBuffer.AddCoord(x1, alpha);
                    lineBuffer.AddCoord(x2 + 1, alpha);
                    lineBuffer.AddCoord(x2 + 2, 0);
                }
            }
        }
        static void GLBlendSolidHSpan(
            int x1, int len, bool isFillMode, AggCoordList3f lineBuffer, int srcColorAlpha,
            byte[] covers, int coversIndex)
        {
            if (isFillMode)
            {
                unchecked
                {
                    int xpos = x1;
                    switch (len)
                    {
                        case 1:
                            {
                                lineBuffer.AddCoord(xpos - 1, 0);
                                //just one pix , 
                                //alpha change ***
                                int alpha = ((srcColorAlpha) * ((covers[coversIndex]) + 1)) >> 8;
                                //single px
                                lineBuffer.AddCoord(xpos, alpha);
                                xpos++;
                                coversIndex++;
                                //lineBuffer.AddCoord(xpos, 0);

                            }
                            break;
                        default:
                            {
                                lineBuffer.AddCoord(xpos - 1, 0);
                                int alpha = 0;
                                do
                                {
                                    //alpha change ***
                                    alpha = ((srcColorAlpha) * ((covers[coversIndex]) + 1)) >> 8;
                                    //single point
                                    lineBuffer.AddCoord(xpos, alpha);
                                    xpos++;
                                    coversIndex++;
                                }
                                while (--len != 0);
                                //close with single px
                                lineBuffer.AddCoord(xpos, 0);
                            }
                            break;
                    }
                }
            }
            else
            {
                unchecked
                {
                    int xpos = x1;
                    switch (len)
                    {
                        case 1:
                            {
                                lineBuffer.AddCoord(xpos - 1, 0);
                                //just one pix , 
                                //alpha change ***
                                int alpha = ((srcColorAlpha) * ((covers[coversIndex]) + 1)) >> 8;
                                //single px
                                lineBuffer.AddCoord(xpos, alpha);
                                xpos++;
                                coversIndex++;
                                //lineBuffer.AddCoord(xpos, 0);

                            }
                            break;
                        default:
                            {
                                lineBuffer.AddCoord(xpos - 1, 0);
                                int alpha = 0;
                                do
                                {
                                    //alpha change ***
                                    alpha = ((srcColorAlpha) * ((covers[coversIndex]) + 1)) >> 8;
                                    //single point
                                    lineBuffer.AddCoord(xpos, alpha);
                                    xpos++;
                                    coversIndex++;
                                }
                                while (--len != 0);
                                //close with single px
                                lineBuffer.AddCoord(xpos, 0);
                            }
                            break;
                    }
                }
            }
        }
    }
}
