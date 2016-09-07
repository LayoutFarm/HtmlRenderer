//2016 MIT, WinterDev

using System;
using System.Collections.Generic;
namespace PixelFarm.Drawing.WinGdi
{
    class BufferBitmapStore
    {
        Stack<System.Drawing.Bitmap> bmpStack = new Stack<System.Drawing.Bitmap>();
        public BufferBitmapStore(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public System.Drawing.Bitmap GetFreeBmp()
        {
            if (bmpStack.Count > 0)
            {
                return bmpStack.Pop();
            }
            else
            {
                return new System.Drawing.Bitmap(Width, Height);
            }
        }
        public void RelaseBmp(System.Drawing.Bitmap bmp)
        {
            bmpStack.Push(bmp);
        }
    }
}