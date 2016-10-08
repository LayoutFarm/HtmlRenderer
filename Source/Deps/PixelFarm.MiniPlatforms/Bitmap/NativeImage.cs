using System;
using System.Runtime.InteropServices;

namespace PixelFarm.Drawing.Imaging
{
    static class NativeImageMethods
    {
        const string myfontLib = "myft.dll";

        [DllImport(myfontLib)]
        public static extern int MyFtLibGetVersion();
        [DllImport(myfontLib)]
        public static extern void DeleteUnmanagedObj(IntPtr unmanagedObject);
        [DllImport(myfontLib)]
        public static extern IntPtr stbi_load(string filename, out int w, out int h, out int comp, int requestOutputComponent);
    }

    public class NativeImage : IDisposable
    {
        IntPtr imgData;
        int width;
        int height;
        int component;

        public NativeImage(string filename)
        {
            //load image from filebname
            //1. check if file exit
            if (System.IO.File.Exists(filename))
            {
                //2. then load            
                imgData = NativeImageMethods.stbi_load(filename, out width, out height, out component, 4);
                //copy data from unmanaged to managed pointer
                //buffer = new byte[width * height * component];
                //Marshal.Copy(imgData, buffer, 0, buffer.Length);
                //then delete this
            }
        }
        public IntPtr GetNativeImageHandle()
        {
            return imgData;
        }
        public int ColorComponent
        {
            get { return component; }
        }
        public int Width
        {
            get { return this.width; }
        }
        public int Height
        {
            get { return this.height; }
        }
        public void Dispose()
        {
            if (imgData != IntPtr.Zero)
            {
                NativeImageMethods.DeleteUnmanagedObj(imgData);
                imgData = IntPtr.Zero;
            }
        }
    }

}