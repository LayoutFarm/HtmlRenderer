//2014,2015 BSD,WinterDev   

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

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace PixelFarm.Agg
{
    /// <summary
    /// to bitmap
    /// </summary>  
    public class GLScanlineRasToDestBitmapRenderer
    {
        ArrayList<VertexC4V2S> mySinglePixelBuffer = new ArrayList<VertexC4V2S>();
        ArrayList<VertexC4V2S> myLineBuffer = new ArrayList<VertexC4V2S>();
        public GLScanlineRasToDestBitmapRenderer()
        {
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





            this.mySinglePixelBuffer.Clear();
            this.myLineBuffer.Clear();
            while (sclineRas.SweepScanline(scline))
            {
                int y = scline.Y;
                int num_spans = scline.SpanCount;
                byte[] covers = scline.GetCovers();
                for (int i = 1; i <= num_spans; ++i)
                {
                    ScanlineSpan span = scline.GetSpan(i);
                    if (span.len > 0)
                    {
                        //outline
                        GLBlendSolidHSpan(span.x, y, span.len, color, covers, span.cover_index);
                    }
                    else
                    {
                        //fill
                        int x = span.x;
                        int x2 = (x - span.len - 1);
                        GLBlendHLine(x, y, x2, color, covers[span.cover_index]);
                    }
                }
            }


            DrawPointAndLineWithVertices();
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
            this.mySinglePixelBuffer.Clear();
            this.myLineBuffer.Clear();
            while (sclineRas.SweepScanline(scline))
            {
                int y = scline.Y;
                int num_spans = scline.SpanCount;
                byte[] covers = scline.GetCovers();
                for (int i = 1; i <= num_spans; ++i)
                {
                    ScanlineSpan span = scline.GetSpan(i);
                    if (span.len > 0)
                    {
                        //outline
                        GLBlendSolidHSpan(span.x, y, span.len, color, covers, span.cover_index);
                    }
                    else
                    {
                        //fill
                        int x = span.x;
                        int x2 = (x - span.len - 1);
                        GLBlendHLine(x, y, x2, color, covers[span.cover_index]);
                    }
                }
            }
            //---------------------------------------------
            DrawPointAndLineWithVertices();
        }
        void DrawPointAndLineWithVertices()
        {
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            //--------------------------------------------- 
            //points
            int nelements = mySinglePixelBuffer.Count;
            if (nelements > 0)
            {
                unsafe
                {
                    VertexC4V2S[] vpoints = this.mySinglePixelBuffer.Array;
                    fixed (VertexC4V2S* h = &vpoints[0])
                    {
                        //color and vertices
                        byte* byteH = (byte*)h;
                        GL.ColorPointer(4, ColorPointerType.UnsignedByte, VertexC4V2S.SIZE_IN_BYTES, (IntPtr)byteH);
                        GL.VertexPointer(VertexC4V2S.N_COORDS,
                            VertexC4V2S.VX_PTR_TYPE,
                            VertexC4V2S.SIZE_IN_BYTES,
                            (IntPtr)(byteH + VertexC4V2S.VX_OFFSET));
                    }
                    GL.DrawArrays(BeginMode.Points, 0, nelements);
                }
            }
            //---------------------------------------------
            //lines
            nelements = myLineBuffer.Count;
            if (nelements > 0)
            {
                unsafe
                {
                    VertexC4V2S[] vpoints = this.myLineBuffer.Array;
                    fixed (VertexC4V2S* h = &vpoints[0])
                    {
                        //color and vertices
                        byte* byteH = (byte*)h;
                        GL.ColorPointer(4, ColorPointerType.UnsignedByte, VertexC4V2S.SIZE_IN_BYTES, (IntPtr)byteH);
                        GL.VertexPointer(VertexC4V2S.N_COORDS,
                            VertexC4V2S.VX_PTR_TYPE,
                            VertexC4V2S.SIZE_IN_BYTES,
                            (IntPtr)(byteH + VertexC4V2S.VX_OFFSET));
                    }
                    GL.DrawArrays(BeginMode.Lines, 0, nelements);
                }
            }

            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.VertexArray);
            //------------------------ 
        }

        const int BASE_MASK = 255;
        //======================================================================================

        void GLBlendHLine(int x1, int y, int x2, PixelFarm.Drawing.Color color, byte cover)
        {
            //if (color.A == 0) { return; }

            int len = x2 - x1 + 1;
            int alpha = (((int)(color.A) * (cover + 1)) >> 8);
            switch (len)
            {
                case 0:
                    {
                    }
                    break;
                case 1:
                    {
                        this.mySinglePixelBuffer.AddVertex(new VertexC4V2S(
                            PixelFarm.Drawing.Color.FromArgb(alpha, color).ToARGB(),
                            x1, y));
                    }
                    break;
                default:
                    {
                        var lineBuffer = this.myLineBuffer;
                        var c = PixelFarm.Drawing.Color.FromArgb(alpha, color).ToARGB();
                        lineBuffer.AddVertex(new VertexC4V2S(c, x1, y));
                        lineBuffer.AddVertex(new VertexC4V2S(c, x2 + 1, y));
                        //var c = PixelFarm.Drawing.Color.FromArgb(alpha, color).ToARGB();

                        //for (int i = 0; i < len; ++i)
                        //{
                        //    //var c = PixelFarm.Drawing.Color.FromArgb(alpha, color);
                        //    singlePxBuff.AddVertex(new VertexC4XYZ3I(
                        //        c, x1 + i, y));
                        //}

                    }
                    break;
            }
            //}
            //else
            //{
            //    int xpos = x1;
            //    do
            //    {
            //        singlePxBuff.AddVertex(new VertexC4V2S(
            //            PixelFarm.Drawing.Color.FromArgb(alpha, color).ToARGB(),
            //            xpos, y));
            //        xpos++;
            //    }
            //    while (--len != 0);
            //}
        }
        void GLBlendSolidHSpan(int x, int y, int len,
            PixelFarm.Drawing.Color sourceColor,
            byte[] covers, int coversIndex)
        {
            //int colorAlpha = sourceColor.A;
            //if (colorAlpha == 0) { return; }

            unchecked
            {
                int xpos = x;
                var pointAndColors = this.mySinglePixelBuffer;
                do
                {
                    //alpha change
                    int alpha = ((sourceColor.A) * ((covers[coversIndex]) + 1)) >> 8;
                    if (alpha == BASE_MASK)
                    {
                        pointAndColors.AddVertex(
                            new VertexC4V2S(sourceColor.ToARGB(), xpos, y));
                    }
                    else
                    {
                        pointAndColors.AddVertex(
                            new VertexC4V2S(PixelFarm.Drawing.Color.FromArgb(alpha, sourceColor).ToARGB(),
                            xpos, y));
                    }
                    xpos++;
                    coversIndex++;
                }
                while (--len != 0);
            }
        }
        //====================================================================================== 
    }
}
