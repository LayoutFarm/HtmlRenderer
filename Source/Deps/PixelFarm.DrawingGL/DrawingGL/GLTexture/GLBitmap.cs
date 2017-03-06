//MIT, 2014-2017, WinterDev

using System;
using OpenTK.Graphics.ES20;
using PixelFarm.Drawing;

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

    public class GLBitmap : PixelFarm.Drawing.Image
    {
        int textureId;
        int width;
        int height;
        byte[] rawBuffer;
        int[] rawIntBuffer;
        //PixelFarm.Drawing.Imaging.NativeImage bmp;
        IntPtr nativeImgMem;
        LazyBitmapBufferProvider lazyProvider;
        bool isInvertImage = false;
        static readonly bool isLittleEndian;
        static GLBitmap()
        {
            isLittleEndian = BitConverter.IsLittleEndian;
        }
        public GLBitmap(int w, int h, byte[] rawBuffer, bool isInvertImage)
        {
            this.width = w;
            this.height = h;
            this.rawBuffer = rawBuffer;
            this.isInvertImage = isInvertImage;
        }
        public GLBitmap(int w, int h, int[] rawIntBuffer, bool isInvertImage)
        {
            this.width = w;
            this.height = h;
            this.rawIntBuffer = rawIntBuffer;
            this.isInvertImage = isInvertImage;
        }

        public GLBitmap(int w, int h, IntPtr nativeImgMem)
        {
            this.width = w;
            this.height = h;
            this.nativeImgMem = nativeImgMem;
        }
        public GLBitmap(LazyBitmapBufferProvider lazyProvider)
        {
            this.width = lazyProvider.Width;
            this.height = lazyProvider.Height;
            this.lazyProvider = lazyProvider;
            this.isInvertImage = lazyProvider.IsInvert;
        }
        public GLBitmap(int textureId, int w, int h)
        {
            this.textureId = textureId;
            this.width = w;
            this.height = h;
        }
        public bool IsBigEndianPixel { get; set; }

        public bool IsInvert
        {
            get { return this.isInvertImage; }
            set { this.isInvertImage = value; }
        }
        public int TextureId { get { return textureId; } }
        public override int Width
        {
            get { return this.width; }
        }
        public override int Height
        {
            get { return this.height; }
        }

        public override bool IsReferenceImage
        {
            get { return false; }
        }
        public override int ReferenceX
        {
            get { return 0; }
        }
        public override int ReferenceY
        {
            get { return 0; }
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
                if (nativeImgMem != IntPtr.Zero)
                {
                    GL.TexImage2D(TextureTarget.Texture2D, 0,
                          PixelInternalFormat.Rgba, this.width, this.height, 0,
                          PixelFormat.Rgba, // 
                          PixelType.UnsignedByte, nativeImgMem);
                }
                else if (this.rawBuffer != null)
                {
                    unsafe
                    {
                        //ES20 dose not have BGRA 
                        //so in little-endian machine we need to convert 
                        fixed (byte* bmpScan0 = &this.rawBuffer[0])
                        {
                            GL.TexImage2D(TextureTarget.Texture2D, 0,
                            PixelInternalFormat.Rgba, this.width, this.height, 0,
                            PixelFormat.Rgba, // 
                            PixelType.UnsignedByte, (IntPtr)bmpScan0);
                        }
                    }
                }
                else if (this.rawIntBuffer != null)
                {
                    unsafe
                    {
                        fixed (int* head = &rawIntBuffer[0])
                        {

                            GL.TexImage2D(TextureTarget.Texture2D, 0,
                            PixelInternalFormat.Rgba, this.width, this.height, 0,
                            PixelFormat.Rgba, // 
                            PixelType.UnsignedByte, new IntPtr((void*)head));
                        }
                    }

                }
                //else if (this.bmp != null)
                //{
                //    GL.TexImage2D(TextureTarget.Texture2D, 0,
                //           PixelInternalFormat.Rgba, this.width, this.height, 0,
                //           PixelFormat.Rgba, // 
                //           PixelType.UnsignedByte, bmp.GetNativeImageHandle());
                //}
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

        public override void Dispose()
        {
            GL.DeleteTextures(1, ref textureId);
        }

        //public override BitmapData LockBits()
        //{
        //    throw new NotImplementedException();
        //}
        //public override void UnlockBits(BitmapData bmpdata)
        //{
        //    throw new NotImplementedException();
        //}
#if DEBUG

        public readonly int dbugId = dbugIdTotal++;
        static int dbugIdTotal;
#endif
    }
}