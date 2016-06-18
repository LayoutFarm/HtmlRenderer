//MIT 2014, WinterDev

using System;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    public abstract class LazyBitmapBufferProvider
    {
        public abstract IntPtr GetRawBufferHead();
        public abstract void ReleaseBufferHead();
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract bool IsInvert { get; }
    }


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


        //---------------------------------
        //only after gl context is created
        internal int GetServerTextureId()
        {
            if (this.textureId == 0)
            {
                //server part
                //gen texture 
                GL.GenTextures(1, out this.textureId);
                //bind
                GL.BindTexture(TextureTarget.Texture2D, this.textureId);
                if (this.rawBuffer != null)
                {
                    unsafe
                    {
                        fixed (byte* bmpScan0 = &this.rawBuffer[0])
                        {
                            GL.TexImage2D(TextureTarget.Texture2D, 0,
                            PixelInternalFormat.Rgba, this.width, this.height, 0,
                            PixelFormat.Rgba,
                            PixelType.UnsignedByte, (IntPtr)bmpScan0);
                        }
                    }
                }
                else
                {
                    //use lazy provider
                    IntPtr bmpScan0 = this.lazyProvider.GetRawBufferHead();
                    GL.TexImage2D(TextureTarget.Texture2D, 0,
                           PixelInternalFormat.Rgba, this.width, this.height, 0,
                           PixelFormat.Rgba,
                           PixelType.UnsignedByte, (IntPtr)bmpScan0);
                    this.lazyProvider.ReleaseBufferHead();
                }
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            }
            return this.textureId;
        }
        public void Dispose()
        {
            GL.DeleteTextures(1, ref textureId);
        }

#if DEBUG

        public readonly int dbugId = dbugIdTotal++;
        static int dbugIdTotal;
#endif
    }
}