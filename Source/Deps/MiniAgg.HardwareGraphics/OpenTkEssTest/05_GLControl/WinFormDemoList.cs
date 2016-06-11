using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "21")]
    [Info("T21_TestWinGLControl")]
    public class T21_TestWinGLControl : DemoBase
    {
        public override void Init()
        {
            FormTestWinGLControl form = new FormTestWinGLControl();
            form.Show();
        }
    }
    [Info(OrderCode = "22")]
    [Info("T22_DemoWinGLControl")]
    public class T22_FormTestWinGLControlDemo2 : DemoBase
    {
        public override void Init()
        {
            FormGLControlSimple form = new FormGLControlSimple();
            form.Show();
        }
    }

    [Info(OrderCode = "23")]
    [Info("T23_FormMultipleGLControlsFormDemo")]
    public class T23_FormMultipleGLControlsFormDemo : DemoBase
    {
        public override void Init()
        {
            FormMultipleGLControlsForm form = new FormMultipleGLControlsForm();
            form.Show();
        }
    }
    [Info(OrderCode = "24")]
    [Info("T24_FormTestGLCanvasDemo")]
    public class T24_FormTestGLCanvasDemo : DemoBase
    {
        GLBitmap hwBmp = null;
        public override void Init()
        {
            FormTestWinGLControl2 form = new FormTestWinGLControl2();
            CanvasGL2d canvas = new CanvasGL2d(this.Width, this.Height);
            GLTextPrinter glTextPrinter = new GLTextPrinter(canvas);
            form.SetGLPaintHandler((o, s) =>
            {
                canvas.Clear(PixelFarm.Drawing.Color.White);
                if (hwBmp == null)
                {
                    hwBmp = PixelFarm.Drawing.DrawingGL.GLBitmapTextureHelper.CreateBitmapTexture(
                        new Bitmap("../../../Data/Textures/logo-dark.jpg"));
                }
                //canvas.DrawImage(hwBmp, 10, 10);
                canvas.DrawImage(hwBmp, 300, 300, hwBmp.Width / 4, hwBmp.Height / 4);
                canvas.StrokeColor = PixelFarm.Drawing.Color.DeepPink;
                canvas.DrawLine(0, 300, 500, 300);
                //-----------------------------------------------------
                canvas.StrokeColor = PixelFarm.Drawing.Color.Magenta;
                //draw line test 
                canvas.DrawLine(20, 20, 600, 200);
                //-----------------------------------------------------
                //smooth with agg 

                canvas.SmoothMode = CanvasSmoothMode.AggSmooth;
                var fillColor = new PixelFarm.Drawing.Color(50, 255, 0, 0);  //  PixelFarm.Drawing.Color.Red;
                //rect polygon
                var polygonCoords = new float[]{
                        5,300,
                        40,300,
                        50,340,
                        10f,340};
                //canvas.DrawPolygon(polygonCoords);
                //fill polygon test                
                canvas.FillPolygon(fillColor, polygonCoords);
                var polygonCoords2 = new float[]{
                        5+10,300,
                        40+10,300,
                        50+10,340,
                        10f +10,340};
                canvas.StrokeColor = new PixelFarm.Drawing.Color(100, 0, 255, 0);  //  L
                canvas.DrawPolygon(polygonCoords2, polygonCoords2.Length / 2);
                int strokeW = 10;
                canvas.StrokeColor = PixelFarm.Drawing.Color.LightGray;
                for (int i = 1; i < 90; i += 10)
                {
                    canvas.StrokeWidth = strokeW;
                    double angle = OpenTK.MathHelper.DegreesToRadians(i);
                    canvas.DrawLine(20, 400, (float)(600 * Math.Cos(angle)), (float)(600 * Math.Sin(angle)));
                    strokeW--;
                    if (strokeW < 1)
                    {
                        strokeW = 1;
                    }
                }



                var color = PixelFarm.Drawing.Color.FromArgb(150, PixelFarm.Drawing.Color.Green);
                canvas.StrokeColor = color;
                ////---------------------------------------------
                ////draw ellipse and circle

                canvas.StrokeWidth = 0.75f;
                canvas.DrawCircle(400, 500, 50);
                canvas.FillCircle(color, 450, 550, 25);
                canvas.StrokeWidth = 3;
                canvas.DrawRoundRect(500, 450, 100, 100, 10, 10);
                canvas.StrokeWidth = 3;
                canvas.StrokeColor = PixelFarm.Drawing.Color.FromArgb(150, PixelFarm.Drawing.Color.Blue);
                //canvas.DrawBezierCurve(0, 0, 500, 500, 0, 250, 500, 250);
                canvas.DrawBezierCurve(120, 500 - 160, 220, 500 - 40, 35, 500 - 200, 220, 500 - 260);
                canvas.SmoothMode = CanvasSmoothMode.No;
                //canvas.DrawArc(150, 200, 300, 50, 0, 150, 150, SvgArcSize.Large, SvgArcSweep.Negative);
                canvas.DrawArc(100, 200, 300, 200, 30, 30, 50, SvgArcSize.Large, SvgArcSweep.Negative);
                // canvas.DrawArc(100, 200, 300, 200, 0, 100, 100, SvgArcSize.Large, SvgArcSweep.Negative);

                fillColor = PixelFarm.Drawing.Color.FromArgb(150, PixelFarm.Drawing.Color.Black);
                canvas.StrokeColor = fillColor;
                canvas.DrawLine(100, 200, 300, 200);
                //load font data
                var font = PixelFarm.Agg.Fonts.NativeFontStore.LoadFont("c:\\Windows\\Fonts\\Tahoma.ttf", 64);
                var fontGlyph = font.GetGlyph('{');
                //PixelFarm.Font2.MyFonts.SetShapingEngine();

                canvas.FillVxs(fillColor, fontGlyph.flattenVxs);
                glTextPrinter.CurrentFont = font;
                canvas.StrokeColor = PixelFarm.Drawing.Color.Black;
                canvas.DrawLine(0, 200, 500, 200);
                //test Thai words
                glTextPrinter.Print("ดุดีดำด่าด่ำญญู", 80, 200);
                //number
                glTextPrinter.Print("1234567890", 80, 200);
                GLBitmap bmp = PixelFarm.Drawing.DrawingGL.GLBitmapTextureHelper.CreateBitmapTexture(fontGlyph.glyphImage32);
                canvas.DrawImage(bmp, 50, 50);
                bmp.Dispose();
            });
            form.Show();
        }
    }
}