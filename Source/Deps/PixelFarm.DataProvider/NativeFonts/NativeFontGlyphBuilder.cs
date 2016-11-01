//MIT, 2014-2016, WinterDev
//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//-----------------------------------

using System;
using System.Collections.Generic;
using PixelFarm.Agg;
namespace PixelFarm.Drawing.Fonts
{
    static class NativeFontGlyphBuilder
    {
        static Agg.VertexSource.CurveFlattener curveFlattener = new Agg.VertexSource.CurveFlattener();
        unsafe internal static void CopyGlyphBitmap(FontGlyph fontGlyph)
        {
            FT_Bitmap* ftBmp = (FT_Bitmap*)fontGlyph.glyphMatrix.bitmap;
            //image is 8 bits grayscale
            int h = ftBmp->rows;
            int w = ftBmp->width;
            int stride = ftBmp->pitch;
            int size = stride * h;
            //copy it to array  
            //bmp glyph is bottom up 
            //so .. invert it...

            byte[] buff = new byte[size];
            //------------------------------------------------
            byte* currentSrc = ftBmp->buffer;
            int srcpos = size;
            int targetpos = 0;
            for (int r = 1; r <= h; ++r)
            {
                srcpos -= stride;
                currentSrc = ftBmp->buffer + srcpos;
                for (int c = 0; c < stride; ++c)
                {
                    buff[targetpos] = *(currentSrc + c);
                    targetpos++;
                }
            }

            ////------------------------------------------------
            //IntPtr bmpPtr = (IntPtr)ftBmp->buffer;
            //Marshal.Copy((IntPtr)ftBmp->buffer, buff, 0, size);
            ////------------------------------------------------ 

            //------------------------------------------------
            fontGlyph.glyImgBuffer8 = buff;
            //convert to 32bpp
            //make gray value as alpha channel color value
            ActualImage actualImage = new ActualImage(w, h, Agg.Image.PixelFormat.ARGB32);
            byte[] newBmp32Buffer = actualImage.GetBuffer();
            int src_p = 0;
            int target_p = 0;
            for (int r = 0; r < h; ++r)
            {
                for (int c = 0; c < w; ++c)
                {
                    byte srcColor = buff[src_p + c];
                    //expand to 4 channel
                    newBmp32Buffer[target_p] = 0;
                    newBmp32Buffer[target_p + 1] = 0;
                    newBmp32Buffer[target_p + 2] = 0;
                    newBmp32Buffer[target_p + 3] = srcColor; //A
                    target_p += 4;
                }
                src_p += stride;
            }
            fontGlyph.glyphImage32 = actualImage;
        }

        static FtVec2 GetMidPoint(FT_Vector v1, FT_Vector v2)
        {
            return new FtVec2(
                ((double)v1.x + (double)v2.x) / 2d,
                ((double)v1.y + (double)v2.y) / 2d);
        }
        static FtVec2 GetMidPoint(FtVec2 v1, FtVec2 v2)
        {
            return new FtVec2(
                ((double)v1.x + (double)v2.x) / 2d,
                ((double)v1.y + (double)v2.y) / 2d);
        }
        static FtVec2 GetMidPoint(FtVec2 v1, FT_Vector v2)
        {
            return new FtVec2(
                (v1.x + (double)v2.x) / 2d,
                (v1.y + (double)v2.y) / 2d);
        }

        unsafe internal static void BuildGlyphOutline(FontGlyph fontGlyph)
        {
            FT_Outline outline = (*(FT_Outline*)fontGlyph.glyphMatrix.outline);
            //outline version
            //------------------------------
            int npoints = outline.n_points;

            int startContour = 0;
            int cpoint_index = 0;
            int todoContourCount = outline.n_contours;
            PixelFarm.Agg.VertexSource.PathWriter ps = new Agg.VertexSource.PathWriter();
            fontGlyph.originalVxs = ps.Vxs;
            int controlPointCount = 0;
            while (todoContourCount > 0)
            {
                int nextContour = outline.contours[startContour] + 1;
                bool isFirstPoint = true;
                FtVec2 secondControlPoint = new FtVec2();
                FtVec2 thirdControlPoint = new FtVec2();
                bool justFromCurveMode = false;
                //FT_Vector vpoint = new FT_Vector();
                for (; cpoint_index < nextContour; ++cpoint_index)
                {
                    FT_Vector vpoint = outline.points[cpoint_index];
                    byte vtag = outline.tags[cpoint_index];
                    bool has_dropout = (((vtag >> 2) & 0x1) != 0);
                    int dropoutMode = vtag >> 3;

                    Console.WriteLine(vpoint.ToString() + " " + vtag);

                    if ((vtag & 0x1) != 0)
                    {
                        //on curve
                        if (justFromCurveMode)
                        {
                            switch (controlPointCount)
                            {
                                case 1:
                                    {
                                        ps.Curve3(secondControlPoint.x / FT_RESIZE, secondControlPoint.y / FT_RESIZE,
                                            vpoint.x / FT_RESIZE, vpoint.y / FT_RESIZE);
                                    }
                                    break;
                                case 2:
                                    {
                                        ps.Curve4(secondControlPoint.x / FT_RESIZE, secondControlPoint.y / FT_RESIZE,
                                           thirdControlPoint.x / FT_RESIZE, thirdControlPoint.y / FT_RESIZE,
                                           vpoint.x / FT_RESIZE, vpoint.y / FT_RESIZE);
                                    }
                                    break;
                                default:
                                    {
                                        throw new NotSupportedException();
                                    }
                            }
                            controlPointCount = 0;
                            justFromCurveMode = false;
                        }
                        else
                        {
                            if (isFirstPoint)
                            {
                                isFirstPoint = false;
                                ps.MoveTo(vpoint.x / FT_RESIZE, vpoint.y / FT_RESIZE);
                            }
                            else
                            {
                                ps.LineTo(vpoint.x / FT_RESIZE, vpoint.y / FT_RESIZE);
                            }

                            if (has_dropout)
                            {
                                //printf("[%d] on,dropoutMode=%d: %d,y:%d \n", mm, dropoutMode, vpoint.x, vpoint.y);
                            }
                            else
                            {
                                //printf("[%d] on,x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                            }
                        }
                    }
                    else
                    {
                        switch (controlPointCount)
                        {
                            case 0:
                                {   //bit 1 set=> off curve, this is a control point
                                    //if this is a 2nd order or 3rd order control point
                                    if (((vtag >> 1) & 0x1) != 0)
                                    {
                                        //printf("[%d] bzc3rd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                        thirdControlPoint = new FtVec2(vpoint);
                                    }
                                    else
                                    {
                                        //printf("[%d] bzc2nd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                        secondControlPoint = new FtVec2(vpoint);
                                    }
                                }
                                break;
                            case 1:
                                {
                                    if (((vtag >> 1) & 0x1) != 0)
                                    {
                                        //printf("[%d] bzc3rd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                        thirdControlPoint = new FtVec2(vpoint);
                                    }
                                    else
                                    {
                                        //we already have prev second control point
                                        //so auto calculate line to 
                                        //between 2 point
                                        FtVec2 mid = GetMidPoint(secondControlPoint, vpoint);
                                        //----------
                                        //generate curve3
                                        ps.Curve3(secondControlPoint.x / FT_RESIZE, secondControlPoint.y / FT_RESIZE,
                                            mid.x / FT_RESIZE, mid.y / FT_RESIZE);
                                        //------------------------
                                        controlPointCount--;
                                        //------------------------
                                        //printf("[%d] bzc2nd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                        secondControlPoint = new FtVec2(vpoint);
                                    }
                                }
                                break;
                            default:
                                {
                                    throw new NotSupportedException();
                                }
                                break;
                        }

                        controlPointCount++;
                        justFromCurveMode = true;
                    }
                }
                //--------
                //close figure
                //if in curve mode
                if (justFromCurveMode)
                {
                    switch (controlPointCount)
                    {
                        case 0: break;
                        case 1:
                            {
                                ps.Curve3(secondControlPoint.x / FT_RESIZE, secondControlPoint.y / FT_RESIZE,
                                    ps.LastMoveX, ps.LastMoveY);
                            }
                            break;
                        case 2:
                            {
                                ps.Curve4(secondControlPoint.x / FT_RESIZE, secondControlPoint.y / FT_RESIZE,
                                   thirdControlPoint.x / FT_RESIZE, thirdControlPoint.y / FT_RESIZE,
                                   ps.LastMoveX, ps.LastMoveY);
                            }
                            break;
                        default:
                            { throw new NotSupportedException(); }
                    }
                    justFromCurveMode = false;
                    controlPointCount = 0;
                }
                ps.CloseFigure();
                //--------                   
                startContour++;
                todoContourCount--;
            }
        }
        internal static VertexStore FlattenVxs(VertexStore input)
        {
            return curveFlattener.MakeVxs(input);
        }

       

        const double FT_RESIZE = 64; //essential to be floating point
        internal unsafe static GlyphImage BuildMsdfFontImage(FontGlyph fontGlyph)
        {
            IntPtr shape = MyFtLib.CreateShape();
            FT_Outline outline = (*(FT_Outline*)fontGlyph.glyphMatrix.outline);            //outline version
            //------------------------------
            int npoints = outline.n_points;
            List<PixelFarm.VectorMath.Vector2> points = new List<PixelFarm.VectorMath.Vector2>(npoints);
            int startContour = 0;
            int cpoint_index = 0;
            int todoContourCount = outline.n_contours;
            int controlPointCount = 0;
            while (todoContourCount > 0)
            {
                int nextContour = outline.contours[startContour] + 1;
                bool isFirstPoint = true;
                //---------------
                //create contour 

                IntPtr cnt = MyFtLib.ShapeAddBlankContour(shape);
                FtVec2 secondControlPoint = new FtVec2();
                FtVec2 thirdControlPoint = new FtVec2();
                bool justFromCurveMode = false;
                FtVec2 lastMoveTo = new FtVec2();
                FtVec2 lastPoint = new FtVec2();
                FtVec2 current_point = new Fonts.FtVec2();
                for (; cpoint_index < nextContour; ++cpoint_index)
                {
                    FT_Vector xvpoint = outline.points[cpoint_index];
                    current_point = new FtVec2(xvpoint.x / FT_RESIZE, xvpoint.y / FT_RESIZE);
                    //Console.WriteLine(xvpoint.x.ToString() + "," + xvpoint.y);
                    byte vtag = outline.tags[cpoint_index];
                    bool has_dropout = (((vtag >> 2) & 0x1) != 0);
                    int dropoutMode = vtag >> 3;
                    if ((vtag & 0x1) != 0)
                    {
                        if (justFromCurveMode)
                        {
                            switch (controlPointCount)
                            {
                                case 1:
                                    {
                                        MyFtLib.ContourAddQuadraticSegment(cnt,
                                           lastPoint.x, lastPoint.y,
                                           secondControlPoint.x, secondControlPoint.y,
                                           current_point.x, current_point.y);
                                        lastPoint = current_point;
                                    }
                                    break;
                                case 2:
                                    {
                                        MyFtLib.ContourAddCubicSegment(cnt,
                                            lastPoint.x, lastPoint.y,
                                            secondControlPoint.x, secondControlPoint.y,
                                            thirdControlPoint.x, thirdControlPoint.y,
                                            current_point.x, current_point.y);
                                        lastPoint = current_point;
                                    }
                                    break;
                                default:
                                    {
                                        throw new NotSupportedException();
                                    }
                            }
                            controlPointCount = 0;
                            justFromCurveMode = false;
                        }
                        else
                        {
                            //line mode
                            if (isFirstPoint)
                            {
                                isFirstPoint = false;
                                lastMoveTo = lastPoint = current_point;
                            }
                            else
                            {
                                MyFtLib.ContourAddLinearSegment(cnt,
                                    lastPoint.x, lastPoint.y,
                                    current_point.x, current_point.y);
                                lastPoint = current_point;
                            }
                        }
                    }
                    else
                    {
                        if (isFirstPoint)
                        {
                            isFirstPoint = false;
                            lastMoveTo = lastPoint = current_point;
                        }

                        switch (controlPointCount)
                        {
                            case 0:
                                {   //bit 1 set=> off curve, this is a control point
                                    //if this is a 2nd order or 3rd order control point
                                    if (((vtag >> 1) & 0x1) != 0)
                                    {
                                        ////printf("[%d] bzc3rd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                        thirdControlPoint = new FtVec2(current_point.x, current_point.y);
                                    }
                                    else
                                    {
                                        ////printf("[%d] bzc2nd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                        secondControlPoint = new FtVec2(current_point.x, current_point.y);
                                    }
                                }
                                break;
                            case 1:
                                {
                                    if (((vtag >> 1) & 0x1) != 0)
                                    {
                                        ////printf("[%d] bzc3rd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                        thirdControlPoint = new FtVec2(current_point.x, current_point.y);
                                    }
                                    else
                                    {
                                        //we already have prev second control point
                                        //so auto calculate line to 
                                        //between 2 point
                                        FtVec2 mid = GetMidPoint(secondControlPoint, current_point);
                                        MyFtLib.ContourAddQuadraticSegment(cnt,
                                            lastPoint.x, lastPoint.y,
                                            secondControlPoint.x, secondControlPoint.y,
                                            mid.x, mid.y);
                                        lastPoint = mid;
                                        //------------------------
                                        controlPointCount--;
                                        //------------------------
                                        //printf("[%d] bzc2nd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                        secondControlPoint = current_point;
                                    }
                                }
                                break;
                            default:
                                {
                                    throw new NotSupportedException();
                                }
                        }

                        controlPointCount++;
                        justFromCurveMode = true;
                    }
                }
                //--------
                //close figure
                //if in curve mode
                if (justFromCurveMode)
                {
                    switch (controlPointCount)
                    {
                        case 0: break;
                        case 1:
                            {
                                MyFtLib.ContourAddQuadraticSegment(cnt,
                                          lastPoint.x, lastPoint.y,
                                          secondControlPoint.x, secondControlPoint.y,
                                          lastMoveTo.x, lastMoveTo.y);
                                lastPoint = current_point;
                            }
                            break;
                        case 2:
                            {
                                MyFtLib.ContourAddCubicSegment(cnt,
                                          lastPoint.x, lastPoint.y,
                                          secondControlPoint.x, secondControlPoint.y,
                                          thirdControlPoint.x, thirdControlPoint.y,
                                          lastMoveTo.x, lastMoveTo.y);
                                lastPoint = current_point;
                            }
                            break;
                        default:
                            { throw new NotSupportedException(); }
                    }
                    justFromCurveMode = false;
                    controlPointCount = 0;
                }
                else
                {
                    MyFtLib.ContourAddLinearSegment(cnt,
                                    current_point.x, current_point.y,
                                    lastMoveTo.x, lastMoveTo.y);
                    lastPoint = current_point;
                }

                //ps.CloseFigure();
                //--------                   
                startContour++;
                todoContourCount--;
            }
            //------------

            double s_left, s_bottom, s_right, s_top;
            MyFtLib.ShapeFindBounds(shape, out s_left, out s_bottom, out s_right, out s_top);
            RectangleF glyphBounds = new RectangleF((float)s_left, (float)s_top, (float)(s_right - s_left), (float)(s_top - s_bottom));
            //then create msdf texture
            if (!MyFtLib.ShapeValidate(shape))
            {
                throw new NotSupportedException();
            }
            MyFtLib.ShapeNormalize(shape);
            int borderXY = 0;
            int w = (int)Math.Ceiling(glyphBounds.Width) + (borderXY + borderXY);
            int h = (int)(Math.Ceiling(glyphBounds.Height)) + (borderXY + borderXY);
            if (w == 0)
            {
                w = 5;
                h = 5;
            }
            int[] outputBuffer = new int[w * h];
            GlyphImage glyphImage = new GlyphImage(w, h);
            glyphImage.BorderXY = borderXY;
            glyphImage.OriginalGlyphBounds = glyphBounds;
            unsafe
            {
                fixed (int* output_header = &outputBuffer[0])
                {
                    float dx = 0;
                    float dy = 0;
                    if (s_left < 0)
                    {
                        //make it positive
                        dx = (float)-s_left;
                    }
                    else if (s_left > 0)
                    {

                    }
                    if (s_bottom < 0)
                    {
                        //make it positive
                        dy = (float)-s_bottom;
                    }
                    else if (s_bottom > 0)
                    {

                    }
                    //this glyph image has border (for msdf to render correctly)
                    MyFtLib.MyFtGenerateMsdf(shape, w, h, 4, 1, dx + borderXY, dy + borderXY, -1, 3, output_header);
                    MyFtLib.DeleteUnmanagedObj(shape);
                }
                glyphImage.SetImageBuffer(outputBuffer, true);
            }
            return glyphImage;
        }
    }
    //--------
    public static class MsdfGen
    {
        //---------------------------------------------------------------------------
        public static GlyphImage BuildMsdfFontImage(FontGlyph fontGlyph)
        {
            return NativeFontGlyphBuilder.BuildMsdfFontImage(fontGlyph);
        }

        public static void SwapColorComponentFromBigEndianToWinGdi(int[] bitbuffer)
        {
            unsafe
            {
                int j = bitbuffer.Length;
                fixed (int* p0 = &(bitbuffer[j - 1]))
                {
                    int* p = p0;
                    for (int i = j - 1; i >= 0; --i)
                    {
                        int color = *p;
                        int a = color >> 24;
                        int b = (color >> 16) & 0xff;
                        int g = (color >> 8) & 0xff;
                        int r = color & 0xff;
                        *p = (a << 24) | (r << 16) | (g << 8) | b;
                        p--;
                    }
                }
            }
        }
    }
}