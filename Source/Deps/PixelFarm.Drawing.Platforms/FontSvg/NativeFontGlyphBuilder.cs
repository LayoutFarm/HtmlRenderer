// 2015,2014 ,MIT, WinterDev
//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//-----------------------------------

using System;
using System.Collections.Generic;
namespace PixelFarm.Agg.Fonts
{
    static class NativeFontGlyphBuilder
    {
        static Agg.VertexSource.CurveFlattener curveFlattener = new Agg.VertexSource.CurveFlattener();
        unsafe internal static void CopyGlyphBitmap(FontGlyph fontGlyph, ExportGlyph* exportTypeFace)
        {
            FT_Bitmap* ftBmp = (FT_Bitmap*)exportTypeFace->bitmap;
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
            ActualImage actualImage = new ActualImage(w, h, Agg.Image.PixelFormat.Rgba32);
            int newstride = stride * 4;
            byte[] newBmp32Buffer = actualImage.GetBuffer();
            int src_p = 0;
            int target_p = 0;
            for (int r = 0; r < h; ++r)
            {
                for (int c = 0; c < w; ++c)
                {
                    byte srcColor = buff[src_p + c];
                    //expand to 4 channel
                    newBmp32Buffer[target_p] = 0; //R
                    newBmp32Buffer[target_p + 1] = 0; //G
                    newBmp32Buffer[target_p + 2] = 0; //B
                    newBmp32Buffer[target_p + 3] = srcColor; //A
                    target_p += 4;
                }
                src_p += stride;
            }
            fontGlyph.glyphImage32 = actualImage;
            ////------------------------------------------------
            //{
            //      //add System.Drawing to references 
            //      //save image for debug***
            //    
            //    byte[] buffer = new byte[size];
            //    Marshal.Copy((IntPtr)ftBmp->buffer, buffer, 0, size);
            //    ////save to 
            //    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            //    var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h),
            //        System.Drawing.Imaging.ImageLockMode.ReadWrite,
            //        bmp.PixelFormat);
            //    Marshal.Copy(buffer, 0, bmpdata.Scan0, size);
            //    bmp.UnlockBits(bmpdata);
            //    bmp.Save("d:\\WImageTest\\glyph.png");
            //} 


        }

        static FT_Vector GetMidPoint(FT_Vector v1, FT_Vector v2)
        {
            return new FT_Vector(
                (v1.x + v2.x) / 2,
                (v1.y + v2.y) / 2);
        }



        unsafe internal static void BuildGlyphOutline(FontGlyph fontGlyph, ExportGlyph* exportTypeFace)
        {
            FT_Outline outline = (*(FT_Outline*)exportTypeFace->outline);
            //outline version
            //------------------------------
            int npoints = outline.n_points;
            List<PixelFarm.VectorMath.Vector2> points = new List<PixelFarm.VectorMath.Vector2>(npoints);
            int startContour = 0;
            int cpoint_index = 0;
            int todoContourCount = outline.n_contours;
            PixelFarm.Agg.VertexSource.PathWriter ps = new Agg.VertexSource.PathWriter();
            fontGlyph.originalVxs = ps.Vxs;
            const int resize = 64;
            int controlPointCount = 0;
            while (todoContourCount > 0)
            {
                int nextContour = outline.contours[startContour] + 1;
                bool isFirstPoint = true;
                FT_Vector secondControlPoint = new FT_Vector();
                FT_Vector thirdControlPoint = new FT_Vector();
                bool justFromCurveMode = false;
                //FT_Vector vpoint = new FT_Vector();
                for (; cpoint_index < nextContour; ++cpoint_index)
                {
                    FT_Vector vpoint = outline.points[cpoint_index];
                    byte vtag = outline.tags[cpoint_index];
                    bool has_dropout = (((vtag >> 2) & 0x1) != 0);
                    int dropoutMode = vtag >> 3;
                    if ((vtag & 0x1) != 0)
                    {
                        //on curve
                        if (justFromCurveMode)
                        {
                            switch (controlPointCount)
                            {
                                case 1:
                                    {
                                        ps.Curve3(secondControlPoint.x / resize, secondControlPoint.y / resize,
                                            vpoint.x / resize, vpoint.y / resize);
                                    }
                                    break;
                                case 2:
                                    {
                                        ps.Curve4(secondControlPoint.x / resize, secondControlPoint.y / resize,
                                           thirdControlPoint.x / resize, thirdControlPoint.y / resize,
                                           vpoint.x / resize, vpoint.y / resize);
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
                                ps.MoveTo(vpoint.x / resize, vpoint.y / resize);
                            }
                            else
                            {
                                ps.LineTo(vpoint.x / resize, vpoint.y / resize);
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
                                        thirdControlPoint = vpoint;
                                    }
                                    else
                                    {
                                        //printf("[%d] bzc2nd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                        secondControlPoint = vpoint;
                                    }
                                }
                                break;
                            case 1:
                                {
                                    if (((vtag >> 1) & 0x1) != 0)
                                    {
                                        //printf("[%d] bzc3rd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                        thirdControlPoint = vpoint;
                                    }
                                    else
                                    {
                                        //we already have prev second control point
                                        //so auto calculate line to 
                                        //between 2 point
                                        var mid = GetMidPoint(secondControlPoint, vpoint);
                                        //----------
                                        //generate curve3
                                        ps.Curve3(secondControlPoint.x / resize, secondControlPoint.y / resize,
                                            mid.x / resize, mid.y / resize);
                                        //------------------------
                                        controlPointCount--;
                                        //------------------------
                                        //printf("[%d] bzc2nd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                        secondControlPoint = vpoint;
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
                                ps.Curve3(secondControlPoint.x / resize, secondControlPoint.y / resize,
                                    ps.LastMoveX, ps.LastMoveY);
                            }
                            break;
                        case 2:
                            {
                                ps.Curve4(secondControlPoint.x / resize, secondControlPoint.y / resize,
                                   thirdControlPoint.x / resize, thirdControlPoint.y / resize,
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

            fontGlyph.flattenVxs = curveFlattener.MakeVxs(fontGlyph.originalVxs);
        }
    }
}