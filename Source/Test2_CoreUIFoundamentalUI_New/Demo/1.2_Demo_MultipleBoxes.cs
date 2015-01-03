//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("1.2 MultpleBox")]
    class Demo_MultipleBox : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {

            for (int i = 0; i < 5; ++i)
            {
                var textbox = new LayoutFarm.SampleControls.UIEaseBox(30, 30);
                textbox.SetLocation(i * 40, i * 40);
                viewport.AddContent(textbox);
            }
            //--------------------
            //image
            //load bitmap with gdi+           
            Bitmap bmp = LoadBitmap("../../Demo/favorites32.png");
            LayoutFarm.SampleControls.UIImageBox imgBox = new SampleControls.UIImageBox(bmp.Width, bmp.Height);
            imgBox.Image = bmp;
            viewport.AddContent(imgBox);

        }
        static Bitmap LoadBitmap(string filename)
        {
            System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(filename);
            Bitmap bmp = new Bitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
            return bmp;
        }
    }
}