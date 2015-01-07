// 2015,2014 ,MIT, WinterDev

using System;
using System.Text;  

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
}