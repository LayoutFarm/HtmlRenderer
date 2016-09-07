//MIT, 2014-2016,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.Drawing.Fonts;
namespace OpenTkEssTest
{
    [Info(OrderCode = "404")]
    [Info("T404_FontAtlas")]
    public class T404_FontAtlas : PrebuiltGLControlDemoBase
    {
        CanvasGL2d canvas2d;
        bool resInit;
        GLBitmap msdf_bmp;
        GLCanvasPainter painter;
        System.Drawing.Bitmap totalImg;
        SimpleFontAtlas fontAtlas;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
            painter = new GLCanvasPainter(canvas2d, max, max);

            //--------------------- 
            string fontfilename = "d:\\WImageTest\\a_total.xml";
            SimpleFontAtlasBuilder atlasBuilder = new SimpleFontAtlasBuilder();
            fontAtlas = atlasBuilder.LoadFontInfo(fontfilename);

            totalImg = new System.Drawing.Bitmap("d:\\WImageTest\\a_total.png");

            var bmpdata = totalImg.LockBits(new System.Drawing.Rectangle(0, 0, totalImg.Width, totalImg.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, totalImg.PixelFormat);
            var buffer = new int[totalImg.Width * totalImg.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, buffer, 0, buffer.Length);
            totalImg.UnlockBits(bmpdata);
            var glyph = new GlyphImage(totalImg.Width, totalImg.Height);
            glyph.SetImageBuffer(buffer, false);
            fontAtlas.TotalGlyph = glyph;




            //---------------------
        }
        protected override void DemoClosing()
        {
            canvas2d.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            canvas2d.ClearColorBuffer();
            if (!resInit)
            {
                // msdf_bmp = LoadTexture(@"..\msdf_75.png");
                //msdf_bmp = LoadTexture(@"d:\\WImageTest\\a001_x1_66.png");
                msdf_bmp = LoadTexture(totalImg);
                //msdf_bmp = LoadTexture(@"d:\\WImageTest\\a001_x1.png");
                //msdf_bmp = LoadTexture(@"d:\\WImageTest\\msdf_65.png");

                resInit = true;
            }

            painter.Clear(PixelFarm.Drawing.Color.White);
            //var f = painter.CurrentFont;

            //painter.DrawString("hello!", 0, 20);
            //canvas2d.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 200, 500, 15f);

            PixelFarm.Drawing.Rectangle r;
            fontAtlas.GetRect('A', out r);
            //canvas2d.DrawSubImageWithMsdf(msdf_bmp, ref r, 100, 500);
            canvas2d.DrawSubImageWithMsdf(msdf_bmp, ref r, 100, 500);
            PixelFarm.Drawing.Rectangle r2;
            fontAtlas.GetRect('B', out r2);
            canvas2d.DrawSubImageWithMsdf(msdf_bmp, ref r2, 100 + r.Width - 10, 500);

            //full image
            canvas2d.DrawImage(msdf_bmp, 100, 300);
            miniGLControl.SwapBuffers();
        }
    }
}

