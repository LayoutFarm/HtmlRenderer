//MIT, 2016-2017, WinterDev
using System; 
namespace TestGlfw
{
    //-----------------
    //for test/debug
    //sample only****
    //-----------------

    class MyNativeRGBA32BitsImage : IDisposable
    {
        int width;
        int height;
        int bitDepth;
        int stride;
        IntPtr unmanagedMem;
        public MyNativeRGBA32BitsImage(int width, int height)
        {
            //width and height must >0 
            this.width = width;
            this.height = height;
            this.bitDepth = 32;
            this.stride = width * (32 / 8);
            unmanagedMem = System.Runtime.InteropServices.Marshal.AllocHGlobal(stride * height);
            //this.pixelBuffer = new byte[stride * height];
        }
        public IntPtr Scan0
        {
            get { return this.unmanagedMem; }
        }
        public int Stride
        {
            get { return this.stride; }
        }
        public void Dispose()
        {
            if (unmanagedMem != IntPtr.Zero)
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(unmanagedMem);
                unmanagedMem = IntPtr.Zero;
            }
        }
    }
}