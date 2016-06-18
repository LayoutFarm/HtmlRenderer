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


using PixelFarm.Agg.Image;
namespace PixelFarm.Agg
{
    public enum ScanlineRenderMode
    {
        Default,
        Custom,
        SubPixelRendering
    }

    /// <summary>
    /// to bitmap
    /// </summary>  
    public class ScanlineRasToDestBitmapRenderer
    {
        ArrayList<ColorRGBA> tempSpanColors = new ArrayList<ColorRGBA>();
        public ScanlineRasToDestBitmapRenderer()
        {
        }
        public ScanlineRenderMode ScanlineRenderMode
        {
            get;
            set;
        }
        const float cover_1_3 = 255f / 3f;
        const float cover_2_3 = cover_1_3 * 2f;
        void SubPixRender(IImageReaderWriter dest, Scanline scanline, ColorRGBA color)
        {
            byte[] covers = scanline.GetCovers();
            int num_spans = scanline.SpanCount;
            int y = scanline.Y;
            byte[] buffer = dest.GetBuffer();
            IPixelBlender blender = dest.GetRecieveBlender();
            int last_x = int.MinValue;
            int prev_cover = 0;
            int bufferOffset = 0;
            ColorRGBA prevColor = ColorRGBA.White;
            for (int i = 1; i <= num_spans; ++i)
            {
                //render span by span 

                ScanlineSpan span = scanline.GetSpan(i);
                if (span.x != last_x + 1)
                {
                    bufferOffset = dest.GetBufferOffsetXY(span.x, y);
                    //when skip  then reset                     
                    prev_cover = 0;
                    prevColor = ColorRGBA.White;
                }

                last_x = span.x;
                int num_pix = span.len;
                if (num_pix < 0)
                {
                    //special encode***
                    num_pix = -num_pix; //make it positive value
                    last_x += (num_pix - 1);
                    //long span with coverage
                    int coverageValue = covers[span.cover_index];
                    //------------------------------------------- 
                    if (coverageValue >= 255)
                    {
                        //100% cover
                        int a = ((coverageValue + 1) * color.Alpha0To255) >> 8;
                        ColorRGBA newc = prevColor = new ColorRGBA(color.red, color.green, color.blue);
                        ColorRGBA todrawColor = new ColorRGBA(newc, a);
                        prev_cover = 255;//full
                        while (num_pix > 0)
                        {
                            blender.BlendPixel(buffer, bufferOffset, todrawColor);
                            bufferOffset += 4; //1 pixel 4 bytes
                            --num_pix;
                        }
                    }
                    else
                    {
                        prev_cover = coverageValue;
                        int a = ((coverageValue + 1) * color.Alpha0To255) >> 8;
                        ColorRGBA newc = prevColor = new ColorRGBA(color.red, color.green, color.blue);
                        ColorRGBA todrawColor = new ColorRGBA(newc, a);
                        while (num_pix > 0)
                        {
                            blender.BlendPixel(buffer, bufferOffset, todrawColor);
                            bufferOffset += 4; //1 pixel 4 bytes
                            --num_pix;
                        }
                    }
                }
                else
                {
                    int coverIndex = span.cover_index;
                    last_x += (num_pix - 1);
                    while (num_pix > 0)
                    {
                        int coverageValue = covers[coverIndex++];
                        if (coverageValue >= 255)
                        {
                            //100% cover
                            ColorRGBA newc = new ColorRGBA(color.red, color.green, color.blue);
                            prevColor = newc;
                            int a = ((coverageValue + 1) * color.Alpha0To255) >> 8;
                            blender.BlendPixel(buffer, bufferOffset, new ColorRGBA(newc, a));
                            prev_cover = 255;//full
                        }
                        else
                        {
                            //check direction : 


                            bool isLeftToRight = coverageValue >= prev_cover;
                            prev_cover = coverageValue;
                            byte c_r, c_g, c_b;
                            float subpix_percent = ((float)(coverageValue) / 256f);
                            if (coverageValue < cover_1_3)
                            {
                                if (isLeftToRight)
                                {
                                    c_r = 255;
                                    c_g = 255;
                                    c_b = (byte)(255 - (255f * (subpix_percent)));
                                }
                                else
                                {
                                    c_r = (byte)(255 - (255f * (subpix_percent)));
                                    c_g = 255;
                                    c_b = 255;
                                }

                                ColorRGBA newc = prevColor = new ColorRGBA(c_r, c_g, c_b);
                                int a = ((coverageValue + 1) * color.Alpha0To255) >> 8;
                                blender.BlendPixel(buffer, bufferOffset, new ColorRGBA(newc, a));
                            }
                            else if (coverageValue < cover_2_3)
                            {
                                if (isLeftToRight)
                                {
                                    c_r = prevColor.blue;
                                    c_g = (byte)(255 - (255f * (subpix_percent)));
                                    c_b = color.blue;
                                }
                                else
                                {
                                    c_r = color.blue;
                                    c_g = (byte)(255 - (255f * (subpix_percent)));
                                    c_b = 255;
                                }
                                ColorRGBA newc = prevColor = new ColorRGBA(c_r, c_g, c_b);
                                int a = ((coverageValue + 1) * color.Alpha0To255) >> 8;
                                blender.BlendPixel(buffer, bufferOffset, new ColorRGBA(newc, a));
                            }
                            else
                            {
                                //cover > 2/3 but not full 
                                if (isLeftToRight)
                                {
                                    c_r = (byte)(255 - (255f * (subpix_percent)));
                                    c_g = color.green;
                                    c_b = color.blue;
                                }
                                else
                                {
                                    c_r = prevColor.green;
                                    c_g = prevColor.blue;
                                    c_b = (byte)(255 - (255f * (subpix_percent)));
                                }

                                ColorRGBA newc = prevColor = new ColorRGBA(c_r, c_g, c_b);
                                int a = ((coverageValue + 1) * color.Alpha0To255) >> 8;
                                blender.BlendPixel(buffer, bufferOffset, new ColorRGBA(newc, a));
                            }
                        }
                        bufferOffset += 4; //1 pixel 4 bits 
                        --num_pix;
                    }
                }
            }
        }

        public void RenderWithColor(IImageReaderWriter dest,
                ScanlineRasterizer sclineRas,
                Scanline scline,
                ColorRGBA color)
        {
            if (!sclineRas.RewindScanlines()) { return; } //early exit
            //----------------------------------------------- 
            scline.ResetSpans(sclineRas.MinX, sclineRas.MaxX);
            switch (this.ScanlineRenderMode)
            {
                default:
                    {
                        //prev mode  
                        //this mode 
                        while (sclineRas.SweepScanline(scline))
                        {
                            //render solid single scanline
                            int y = scline.Y;
                            int num_spans = scline.SpanCount;
                            byte[] covers = scline.GetCovers();
                            //render each span in the scanline
                            for (int i = 1; i <= num_spans; ++i)
                            {
                                ScanlineSpan span = scline.GetSpan(i);
                                if (span.len > 0)
                                {
                                    //positive len 
                                    dest.BlendSolidHSpan(span.x, y, span.len, color, covers, span.cover_index);
                                }
                                else
                                {
                                    //fill the line, same coverage area
                                    int x = span.x;
                                    int x2 = (x - span.len - 1);
                                    dest.BlendHL(x, y, x2, color, covers[span.cover_index]);
                                }
                            }
                        }
                    }
                    break;
                case Agg.ScanlineRenderMode.SubPixelRendering:
                    {
                        int dbugMinScanlineCount = 0;
                        while (sclineRas.SweepScanline(scline))
                        {
                            //render solid single scanline
                            //if (dbugMinScanlineCount == 8)
                            //{

                            //}
                            SubPixRender(dest, scline, color);
                            //if (dbugMinScanlineCount == 8)
                            //{
                            //    break;
                            //}
                            dbugMinScanlineCount++;
                            //if (dbugMinScanlineCount > 2)
                            //{
                            //    break;
                            //}
                        }
                    }
                    break;
                case Agg.ScanlineRenderMode.Custom:
                    {
                        while (sclineRas.SweepScanline(scline))
                        {
                            CustomRenderSingleScanLine(dest, scline, color);
                        }
                    }
                    break;
            }
        }

        public void RenderWithSpan(IImageReaderWriter dest,
                ScanlineRasterizer sclineRas,
                Scanline scline,
                ISpanGenerator spanGenerator)
        {
            if (!sclineRas.RewindScanlines()) { return; } //early exit
            //-----------------------------------------------

            scline.ResetSpans(sclineRas.MinX, sclineRas.MaxX);
            spanGenerator.Prepare();
            if (dest.Stride / 4 > (tempSpanColors.AllocatedSize))
            {
                //if not enough -> alloc more
                tempSpanColors.Clear(dest.Stride / 4);
            }

            ColorRGBA[] colorArray = tempSpanColors.Array;
            while (sclineRas.SweepScanline(scline))
            {
                //render single scanline 
                int y = scline.Y;
                int num_spans = scline.SpanCount;
                byte[] covers = scline.GetCovers();
                for (int i = 1; i <= num_spans; ++i)
                {
                    ScanlineSpan span = scline.GetSpan(i);
                    int x = span.x;
                    int span_len = span.len;
                    bool firstCoverForAll = false;
                    if (span_len < 0)
                    {
                        span_len = -span_len;
                        firstCoverForAll = true;
                    } //make absolute value

                    //1. generate colors -> store in colorArray
                    spanGenerator.GenerateColors(colorArray, 0, x, y, span_len);
                    //2. blend color in colorArray to destination image
                    dest.BlendColorHSpan(x, y, span_len,
                        colorArray, 0,
                        covers, span.cover_index,
                        firstCoverForAll);
                }
            }
        }
        protected virtual void CustomRenderSingleScanLine(
            IImageReaderWriter dest,
            Scanline scline,
            ColorRGBA color)
        {
            //implement
        }
    }


    //----------------------------
    public class CustomScanlineRasToDestBitmapRenderer : ScanlineRasToDestBitmapRenderer
    {
    }
}
