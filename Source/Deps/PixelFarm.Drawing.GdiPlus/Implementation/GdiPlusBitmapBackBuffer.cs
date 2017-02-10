//BSD, 2014-2017, WinterDev

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
namespace PixelFarm.Agg.Imaging
{


    public class GdiBitmapBackBuffer : IDisposable
    {
        ActualImage actualImage;
        int width;
        int height;
        //------------------------------------
        Win32.NativeWin32MemoryDc nativeWin32Dc;

        public GdiBitmapBackBuffer()
        {
        }
        public void Dispose()
        {
            if (nativeWin32Dc != null)
            {
                nativeWin32Dc.Dispose();
                nativeWin32Dc = null;
            }
        }


        const int SRCCOPY = 0xcc0020;
        /// <summary>
        /// copy buffer to 
        /// </summary>
        /// <param name="dest"></param>
        public void UpdateToHardwareSurface(IntPtr displayHdc)
        {

            BitmapHelper.CopyToWindowsBitmapSameSize(
                this.actualImage,   //src from actual img buffer
                nativeWin32Dc.PPVBits);//dest to buffer bmp     

            bool result = Win32.MyWin32.BitBlt(displayHdc, 0, 0,
                 width,
                 height,
                 nativeWin32Dc.DC, 0, 0, SRCCOPY);
        }
        public void Initialize(int width, int height, int bitDepth, ActualImage actualImage)
        {
            if (width > 0 && height > 0)
            {
                this.width = width;
                this.height = height;

                //if (bitDepth != 32)
                //{
                //    throw new NotImplementedException("Don't support this bit depth yet.");
                //}
                //else
                //{
                //    actualImage = new ActualImage(width, height, PixelFormat.ARGB32);
                this.actualImage = actualImage;
                nativeWin32Dc = new Win32.NativeWin32MemoryDc(width, height, true);
                //    return Graphics2D.CreateFromImage(actualImage);
                //}
                return;
            }
            throw new NotSupportedException();
        }
        //public ImageGraphics2D Initialize(int width, int height, int bitDepth)
        //{
        //    if (width > 0 && height > 0)
        //    {
        //        this.width = width;
        //        this.height = height;
        //        if (bitDepth != 32)
        //        {
        //            throw new NotImplementedException("Don't support this bit depth yet.");
        //        }
        //        else
        //        {
        //            actualImage = new ActualImage(width, height, PixelFormat.ARGB32);
        //            nativeWin32Dc = new Win32.NativeWin32MemoryDc(width, height, true);
        //            return Graphics2D.CreateFromImage(actualImage);
        //        }
        //    }
        //    throw new NotSupportedException();
        //}

        //public Graphics2D CreateNewGraphic2D()
        //{
        //    Graphics2D graphics2D;
        //    if (actualImage != null)
        //    {
        //        graphics2D = Graphics2D.CreateFromImage(actualImage);
        //    }
        //    else
        //    {
        //        throw new NotSupportedException();
        //    }

        //    return graphics2D;
        //}
        //-------------
        ///// <summary>
        ///// update actual image data to windowsBitmap
        ///// </summary>
        ///// <param name="rect"></param>
        //public void UpdateToHardwareSurface(Graphics dest, RectInt rect)
        //{
        //    //----------------------------------------------- 
        //    //copy from actual img buffer (src)
        //    BitmapHelper.CopyToWindowsBitmap(
        //        this.actualImage, //src from actual img buffer
        //        this.bufferBmp, //dest to buffer bmp
        //        rect);
        //    //-----------------------------------------------
        //    //prepare buffer dc ****
        //    IntPtr bufferDc = bufferGfx.GetHdc();
        //    IntPtr hBitmap = bufferBmp.GetHbitmap();
        //    IntPtr hOldObject = Win32.MyWin32.SelectObject(bufferDc, hBitmap);
        //    //------------------------------------------------
        //    //target dc
        //    IntPtr displayHdc = dest.GetHdc();
        //    //copy from buffer dc to target display dc 
        //    bool result = Win32.MyWin32.BitBlt(displayHdc, 0, 0,
        //         bufferBmp.Width,
        //         bufferBmp.Height,
        //        bufferDc, 0, 0, SRCCOPY);
        //    Win32.MyWin32.SelectObject(bufferDc, hOldObject);
        //    //DeleteObject(hBitmap); 
        //    bufferGfx.ReleaseHdc(bufferDc);
        //    dest.ReleaseHdc(displayHdc);
        //}


    }


#if DEBUG
    //    public class dbugGdiPlusBitmapBackBufferOld : IDisposable
    //    {
    //        ActualImage actualImage;
    //        Bitmap bufferBmp;
    //        Graphics bufferGfx;
    //        int width;
    //        int height;

    //        IntPtr hBitmap;
    //        IntPtr hbmpScan0;

    //        int stride;

    //        public dbugGdiPlusBitmapBackBufferOld()
    //        {
    //        }
    //        public void Dispose()
    //        {
    //            if (hBitmap != IntPtr.Zero)
    //            {
    //                Win32.MyWin32.DeleteObject(hBitmap);
    //                hBitmap = IntPtr.Zero;
    //            }
    //        }
    //        const int SRCCOPY = 0xcc0020;

    //        /// <summary>
    //        /// copy buffer to 
    //        /// </summary>
    //        /// <param name="dest"></param>
    //        public void UpdateToHardwareSurface(Graphics dest)
    //        {

    //            IntPtr bufferDc = bufferGfx.GetHdc();
    //            IntPtr hOldObject = Win32.MyWin32.SelectObject(bufferDc, hBitmap);
    //            //-----------------------------------------------
    //            //TODO: review here
    //            //if actual image is not change from last copy
    //            //then we can use a cached?
    //            //-----------------------------------------------
    //            //copy from actual img buffer (src) 
    //            BitmapHelper.CopyToWindowsBitmapSameSize(
    //                this.actualImage,   //src from actual img buffer
    //                hbmpScan0);//dest to buffer bmp                 
    //            //-----------------------------------------------
    //            //prepare buffer dc ****

    //            //------------------------------------------------
    //            //target dc
    //            IntPtr displayHdc = dest.GetHdc();
    //            //copy from buffer dc to target display dc 
    //            bool result = Win32.MyWin32.BitBlt(displayHdc, 0, 0,
    //                 bufferBmp.Width,
    //                 bufferBmp.Height,
    //                 bufferDc, 0, 0, SRCCOPY);
    //            //------------------------------------------------
    //            Win32.MyWin32.SelectObject(bufferDc, hOldObject);

    //            bufferGfx.ReleaseHdc(bufferDc);
    //            dest.ReleaseHdc(displayHdc);
    //        }
    //#if DEBUG
    //        public void dbugUpdateToHardwareSurface2(Graphics dest)
    //        {

    //            //TODO: fixed this
    //            //this not correct size the target dc 
    //            //has different size with internal hBitmap
    //            IntPtr displayHdc = dest.GetHdc();
    //            //copy from buffer dc to target display dc 
    //            IntPtr hOldObject = Win32.MyWin32.SelectObject(displayHdc, hBitmap);

    //            unsafe
    //            {
    //                Win32.BITMAP win32Bitmap = new Win32.BITMAP();
    //                Win32.MyWin32.GetObject(hOldObject,
    //                    System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32.BITMAP)),
    //                      &win32Bitmap);
    //                hbmpScan0 = (IntPtr)win32Bitmap.bmBits;
    //                BitmapHelper.CopyToWindowsBitmapSameSize(
    //                this.actualImage,   //src from actual img buffer
    //                (IntPtr)win32Bitmap.bmBits);//dest to buffer bmp          
    //            }

    //            Win32.MyWin32.SelectObject(displayHdc, hOldObject);
    //            dest.ReleaseHdc(displayHdc);
    //            //-----------------------------------------------




    //            //IntPtr bufferDc = bufferGfx.GetHdc();
    //            //IntPtr hOldObject = Win32.MyWin32.SelectObject(bufferDc, hBitmap);
    //            ////-----------------------------------------------
    //            ////TODO: review here
    //            ////if actual image is not change from last copy
    //            ////then we can use a cached?
    //            ////-----------------------------------------------
    //            ////copy from actual img buffer (src) 
    //            //BitmapHelper.CopyToWindowsBitmapSameSize(
    //            //    this.actualImage,   //src from actual img buffer
    //            //    hbmpScan0);//dest to buffer bmp                 
    //            ////-----------------------------------------------
    //            ////prepare buffer dc ****

    //            ////------------------------------------------------
    //            ////target dc
    //            //IntPtr displayHdc = dest.GetHdc();
    //            ////copy from buffer dc to target display dc 
    //            //bool result = Win32.MyWin32.BitBlt(displayHdc, 0, 0,
    //            //     bufferBmp.Width,
    //            //     bufferBmp.Height,
    //            //     bufferDc, 0, 0, SRCCOPY);
    //            ////------------------------------------------------
    //            //Win32.MyWin32.SelectObject(bufferDc, hOldObject);

    //            //bufferGfx.ReleaseHdc(bufferDc);
    //            //dest.ReleaseHdc(displayHdc);
    //        }

    //        /// <summary>
    //        /// preseve for study, history  
    //        /// </summary>
    //        /// <param name="dest"></param>
    //        public void dbugUpdateToHardwareSurface_Old(Graphics dest)
    //        {
    //            //-----------------------------------------------
    //            //copy from actual img buffer (src) 
    //            BitmapHelper.CopyToGdiPlusBitmapSameSize(
    //                this.actualImage, //src from actual img buffer
    //                this.bufferBmp);//dest to buffer bmp                 
    //            //-----------------------------------------------
    //            //prepare buffer dc ****
    //            IntPtr bufferDc = bufferGfx.GetHdc();
    //            IntPtr hBitmap = bufferBmp.GetHbitmap();
    //            IntPtr hOldObject = Win32.MyWin32.SelectObject(bufferDc, hBitmap);
    //            //------------------------------------------------
    //            //target dc
    //            IntPtr displayHdc = dest.GetHdc();
    //            //copy from buffer dc to target display dc 
    //            bool result = Win32.MyWin32.BitBlt(displayHdc, 0, 0,
    //                 bufferBmp.Width,
    //                 bufferBmp.Height,
    //                 bufferDc, 0, 0, SRCCOPY);
    //            //------------------------------------------------
    //            Win32.MyWin32.SelectObject(bufferDc, hOldObject);

    //            Win32.MyWin32.DeleteObject(hBitmap);
    //            bufferGfx.ReleaseHdc(bufferDc);
    //            dest.ReleaseHdc(displayHdc);
    //        }
    //#endif
    //        public ImageGraphics2D Initialize(int width, int height, int bitDepth)
    //        {
    //            if (width > 0 && height > 0)
    //            {
    //                this.width = width;
    //                this.height = height;
    //                switch (bitDepth)
    //                {
    //                    case 24:
    //                        {
    //                            bufferBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
    //                            var bmpData = bufferBmp.LockBits(new Rectangle(0, 0, width, height),
    //                             System.Drawing.Imaging.ImageLockMode.ReadOnly, bufferBmp.PixelFormat);
    //                            this.stride = bmpData.Stride;
    //                            bufferBmp.UnlockBits(bmpData);


    //                            actualImage = new ActualImage(width, height, PixelFarm.Agg.Imaging.PixelFormat.RGB24);
    //                            bufferGfx = Graphics.FromImage(bufferBmp);
    //                            return Graphics2D.CreateFromImage(actualImage);
    //                        }
    //                    case 32:
    //                        {
    //                            bufferBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);//***
    //                            //windowsBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
    //                            //widowsBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
    //                            //widowsBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
    //                            //32bppPArgb  

    //                            var bmpData = bufferBmp.LockBits(new Rectangle(0, 0, width, height),
    //                                 System.Drawing.Imaging.ImageLockMode.ReadOnly, bufferBmp.PixelFormat);
    //                            this.stride = bmpData.Stride;
    //                            bufferBmp.UnlockBits(bmpData);

    //                            actualImage = new ActualImage(width, height, PixelFormat.ARGB32);
    //                            bufferGfx = Graphics.FromImage(bufferBmp);
    //                            hBitmap = bufferBmp.GetHbitmap();


    //                            Win32.BITMAP win32Bitmap = new Win32.BITMAP();
    //                            unsafe
    //                            {
    //                                Win32.MyWin32.GetObject(hBitmap,
    //                                    System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32.BITMAP)),
    //                                      &win32Bitmap);
    //                                hbmpScan0 = (IntPtr)win32Bitmap.bmBits;
    //                            }


    //                            return Graphics2D.CreateFromImage(actualImage);
    //                        }
    //                    case 128:
    //                    //windowsBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
    //                    //backingImageBufferByte = null;
    //                    //backingImageBufferFloat = new ImageBufferFloat(width, height, 128, new BlenderBGRAFloat());
    //                    //break;

    //                    default:
    //                        throw new NotImplementedException("Don't support this bit depth yet.");
    //                }
    //            }
    //            throw new NotSupportedException();
    //        }

    //        public Graphics2D CreateNewGraphic2D()
    //        {
    //            Graphics2D graphics2D;
    //            if (actualImage != null)
    //            {
    //                graphics2D = Graphics2D.CreateFromImage(actualImage);
    //            }
    //            else
    //            {
    //                throw new NotSupportedException();
    //                //graphics2D = bitmapBackBuffer.backingImageBufferFloat.NewGraphics2D();
    //            }

    //            return graphics2D;
    //        }
    //        //-------------
    //        ///// <summary>
    //        ///// update actual image data to windowsBitmap
    //        ///// </summary>
    //        ///// <param name="rect"></param>
    //        //public void UpdateToHardwareSurface(Graphics dest, RectInt rect)
    //        //{
    //        //    //----------------------------------------------- 
    //        //    //copy from actual img buffer (src)
    //        //    BitmapHelper.CopyToWindowsBitmap(
    //        //        this.actualImage, //src from actual img buffer
    //        //        this.bufferBmp, //dest to buffer bmp
    //        //        rect);
    //        //    //-----------------------------------------------
    //        //    //prepare buffer dc ****
    //        //    IntPtr bufferDc = bufferGfx.GetHdc();
    //        //    IntPtr hBitmap = bufferBmp.GetHbitmap();
    //        //    IntPtr hOldObject = Win32.MyWin32.SelectObject(bufferDc, hBitmap);
    //        //    //------------------------------------------------
    //        //    //target dc
    //        //    IntPtr displayHdc = dest.GetHdc();
    //        //    //copy from buffer dc to target display dc 
    //        //    bool result = Win32.MyWin32.BitBlt(displayHdc, 0, 0,
    //        //         bufferBmp.Width,
    //        //         bufferBmp.Height,
    //        //        bufferDc, 0, 0, SRCCOPY);
    //        //    Win32.MyWin32.SelectObject(bufferDc, hOldObject);
    //        //    //DeleteObject(hBitmap); 
    //        //    bufferGfx.ReleaseHdc(bufferDc);
    //        //    dest.ReleaseHdc(displayHdc);
    //        //}


    //    }

#endif
}
