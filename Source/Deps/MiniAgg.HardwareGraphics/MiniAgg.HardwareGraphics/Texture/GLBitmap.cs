//MIT 2014, WinterDev

using System;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace LayoutFarm.DrawingGL
{
    public delegate void TransientGetBufferHead(IntPtr bufferHead);
    public class GLBitmap : IDisposable
    {
        int textureId;
        int width;
        int height;
        byte[] rawBuffer;
        LazyBitmapBufferProvider lazyProvider;
        bool isInvertImage = false;

        public GLBitmap(int w, int h, byte[] rawBuffer, bool isInvertImage)
        {
            this.width = w;
            this.height = h;
            this.rawBuffer = rawBuffer;
            this.isInvertImage = isInvertImage;
        }
        public GLBitmap(LazyBitmapBufferProvider lazyProvider)
        {
            this.width = lazyProvider.Width;
            this.height = lazyProvider.Height;
            this.lazyProvider = lazyProvider;
            this.isInvertImage = lazyProvider.IsInvert;
        }
        public bool IsInvert
        {
            get { return this.isInvertImage; }
        }
        public int Width
        {
            get { return this.width; }
        }
        public int Height
        {
            get { return this.height; }
        }

        public void TransientLoadBufferHead(TransientGetBufferHead bufferHeadDel)
        {
            if (this.rawBuffer != null)
            {
                unsafe
                {
                    fixed (byte* bmpScan0 = &this.rawBuffer[0])
                    {
                        bufferHeadDel((IntPtr)bmpScan0);
                        //GL.TexImage2D(TextureTarget.Texture2D, 0,
                        //PixelInternalFormat.Rgba, this.width, this.height, 0,
                        //PixelFormat.Bgra,
                        //PixelType.UnsignedByte, (IntPtr)bmpScan0);
                    }
                }
            }
            else
            {
                //use lazy provider
                IntPtr bmpScan0 = this.lazyProvider.GetRawBufferHead();
                bufferHeadDel(bmpScan0);
                //GL.TexImage2D(TextureTarget.Texture2D, 0,
                //       PixelInternalFormat.Rgba, this.width, this.height, 0,
                //       PixelFormat.Bgra,
                //       PixelType.UnsignedByte, (IntPtr)bmpScan0);
                this.lazyProvider.ReleaseBufferHead();
            }

        }
        public void Dispose()
        {
            // GL.DeleteTextures(1, ref textureId);
        }

#if DEBUG

        public readonly int dbugId = dbugIdTotal++;
        static int dbugIdTotal;
#endif
    }


}