//2016 MIT, WinterDev

using PixelFarm.Agg;
namespace PixelFarm.DrawingGL
{
    public class GLCanvasPainter : GLCanvasPainterBase
    {
        //TODO:review here
        //we need this for render text
        System.Drawing.Graphics _winGfx;
        System.Drawing.Bitmap _winGfxBackBmp;
        System.Drawing.SolidBrush _winGfxBrush;
        System.Drawing.Font _winFont;
        public GLCanvasPainter(CanvasGL2d canvas, int w, int h)
            : base(canvas, w, h)
        {

            _winGfxBackBmp = new System.Drawing.Bitmap(w, h);
            _winGfx = System.Drawing.Graphics.FromImage(_winGfxBackBmp);
            _winGfxBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            _winFont = new System.Drawing.Font("tahoma", 10);
        }
        public override void DrawString(string text, double x, double y)
        {
            //in this version we draw string to image
            //and the write the image back to gl surface
            if (CurrentFont != null)
            {
                base.DrawString(text, x, y);
                return;
            }
            //------------------------------------------------------
            _winGfx.Clear(System.Drawing.Color.White);
            _winGfx.DrawString(text, _winFont, _winGfxBrush, 0, 0);
            //_winGfxBackBmp.Save("d:\\WImageTest\\a00123.png"); 

            System.Drawing.SizeF textAreaSize = _winGfx.MeasureString(text, _winFont);
            var bmpData = _winGfxBackBmp.LockBits(new System.Drawing.Rectangle(0, 0, _winGfxBackBmp.Width, _winGfxBackBmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, _winGfxBackBmp.PixelFormat);
            int width = (int)textAreaSize.Width;
            int height = (int)textAreaSize.Height;

            ActualImage actualImg = new ActualImage(width, height, Agg.Image.PixelFormat.ARGB32);
            //------------------------------------------------------
            //copy bmp from specific bmp area 
            //and convert to GLBmp  
            int stride = bmpData.Stride;
            byte[] buffer = actualImg.GetBuffer();
            unsafe
            {
                byte* header = (byte*)bmpData.Scan0;
                fixed (byte* dest0 = &buffer[0])
                {
                    byte* dest = dest0;
                    byte* rowHead = header;
                    int rowLen = width * 4;
                    for (int h = 0; h < height; ++h)
                    {

                        header = rowHead;
                        for (int n = 0; n < rowLen;)
                        {
                            //move next
                            *(dest + 0) = *(header + 0);
                            *(dest + 1) = *(header + 1);
                            *(dest + 2) = *(header + 2);
                            *(dest + 3) = *(header + 3);
                            header += 4;
                            dest += 4;
                            n += 4;
                        }
                        //finish one row
                        rowHead += stride;
                    }
                }
            }
            _winGfxBackBmp.UnlockBits(bmpData);
            //------------------------------------------------------
            GLBitmap glBmp = new GLBitmap(width, height, buffer, false);
            _canvas.DrawImageWithWhiteTransparent(glBmp, (float)x, (float)y);
            glBmp.Dispose();
        }

    }
}