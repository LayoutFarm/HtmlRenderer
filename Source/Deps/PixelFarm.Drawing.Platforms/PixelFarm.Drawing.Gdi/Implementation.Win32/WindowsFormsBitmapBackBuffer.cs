//BSD 2014, WinterDev

/*
Copyright (c) 2014, Lars Brubaker
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met: 

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer. 
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies, 
either expressed or implied, of the FreeBSD Project.
*/

using System;
using System.Drawing;
using PixelFarm.Agg;
using PixelFarm.Agg.Image;
namespace Mini
{
    public class WindowsFormsBitmapBackBuffer
    {
        ActualImage actualImage;
        Bitmap bufferBmp;
        Graphics bufferGfx;
        int width;
        int height;
        public WindowsFormsBitmapBackBuffer()
        {
        }

        const int SRCCOPY = 0xcc0020;
        /// <summary>
        /// update actual image data to windowsBitmap
        /// </summary>
        /// <param name="rect"></param>
        public void UpdateToHardwareSurface(Graphics dest, RectInt rect)
        {
            //----------------------------------------------- 
            //copy from actual img buffer (src)
            BitmapHelper.CopyToWindowsBitmap(
                this.actualImage, //src from actual img buffer
                this.bufferBmp, //dest to buffer bmp
                rect);
            //-----------------------------------------------
            //prepare buffer dc ****
            IntPtr bufferDc = bufferGfx.GetHdc();
            IntPtr hBitmap = bufferBmp.GetHbitmap();
            IntPtr hOldObject = SelectObject(bufferDc, hBitmap);
            //------------------------------------------------
            //target dc
            IntPtr displayHdc = dest.GetHdc();
            //copy from buffer dc to target display dc 
            int result = BitBlt(displayHdc, 0, 0,
                 bufferBmp.Width,
                 bufferBmp.Height,
                bufferDc, 0, 0, SRCCOPY);
            SelectObject(bufferDc, hOldObject);
            //DeleteObject(hBitmap); 
            bufferGfx.ReleaseHdc(bufferDc);
            dest.ReleaseHdc(displayHdc);
        }


        /// <summary>
        /// copy buffer to 
        /// </summary>
        /// <param name="dest"></param>
        public void UpdateToHardwareSurface(Graphics dest)
        {
            //-----------------------------------------------
            //copy from actual img buffer (src) 
            BitmapHelper.CopyToWindowsBitmapSameSize(
                this.actualImage, //src from actual img buffer
                this.bufferBmp);//dest to buffer bmp                 
            //-----------------------------------------------
            //prepare buffer dc ****
            IntPtr bufferDc = bufferGfx.GetHdc();
            IntPtr hBitmap = bufferBmp.GetHbitmap();
            IntPtr hOldObject = SelectObject(bufferDc, hBitmap);
            //------------------------------------------------
            //target dc
            IntPtr displayHdc = dest.GetHdc();
            //copy from buffer dc to target display dc 
            int result = BitBlt(displayHdc, 0, 0,
                 bufferBmp.Width,
                 bufferBmp.Height,
                 bufferDc, 0, 0, SRCCOPY);
            //------------------------------------------------
            SelectObject(bufferDc, hOldObject);
            DeleteObject(hBitmap);//if not delete then mem leak***
            bufferGfx.ReleaseHdc(bufferDc);
            dest.ReleaseHdc(displayHdc);
        }
        public ImageGraphics2D Initialize(int width, int height, int bitDepth)
        {
            if (width > 0 && height > 0)
            {
                this.width = width;
                this.height = height;
                switch (bitDepth)
                {
                    case 24:
                        bufferBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        actualImage = new ActualImage(width, height, PixelFarm.Agg.Image.PixelFormat.Rgb24);
                        bufferGfx = Graphics.FromImage(bufferBmp);
                        return Graphics2D.CreateFromImage(actualImage);
                    case 32:

                        bufferBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                        //windowsBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        //widowsBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                        //widowsBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        //32bppPArgb                         
                        actualImage = new ActualImage(width, height, PixelFarm.Agg.Image.PixelFormat.Rgba32);
                        bufferGfx = Graphics.FromImage(bufferBmp);
                        return Graphics2D.CreateFromImage(actualImage);
                    case 128:
                    //windowsBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    //backingImageBufferByte = null;
                    //backingImageBufferFloat = new ImageBufferFloat(width, height, 128, new BlenderBGRAFloat());
                    //break;

                    default:
                        throw new NotImplementedException("Don't support this bit depth yet.");
                }
            }
            throw new NotSupportedException();
        }

        public Graphics2D CreateNewGraphic2D()
        {
            Graphics2D graphics2D;
            if (actualImage != null)
            {
                graphics2D = Graphics2D.CreateFromImage(actualImage);
            }
            else
            {
                throw new NotSupportedException();
                //graphics2D = bitmapBackBuffer.backingImageBufferFloat.NewGraphics2D();
            }

            return graphics2D;
        }
        //-------------

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        public static extern System.IntPtr SelectObject(System.IntPtr hdc, System.IntPtr h);
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern int BitBlt(
            IntPtr hdcDest,     // handle to destination DC (device context)
            int nXDest,         // x-coord of destination upper-left corner
            int nYDest,         // y-coord of destination upper-left corner
            int nWidth,         // width of destination rectangle
            int nHeight,        // height of destination rectangle
            IntPtr hdcSrc,      // handle to source DC
            int nXSrc,          // x-coordinate of source upper-left corner
            int nYSrc,          // y-coordinate of source upper-left corner
            System.Int32 dwRop  // raster operation code
            );
    }
}
