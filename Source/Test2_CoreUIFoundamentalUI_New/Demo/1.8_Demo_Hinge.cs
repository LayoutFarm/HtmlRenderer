//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("1.8 Hinge")]
    class Demo_Hinge : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {

            var hingeBox1 = CreateHingeBox(20, 20);            
            //------------
            viewport.AddContent(hingeBox1);
            var hingeBox2 = CreateHingeBox(50, 50);
            viewport.AddContent(hingeBox2);

        }
        LayoutFarm.SampleControls.UIHinge CreateHingeBox(int x, int y)
        {
            LayoutFarm.SampleControls.UIHinge hingeBox = new SampleControls.UIHinge(400, 20);
            hingeBox.SetLocation(x, y);
            //--------------------
            //1. create landing part
            var landPart = new LayoutFarm.SampleControls.UIPanel(400, 20);
            landPart.BackColor = Color.Green;
            hingeBox.LandPart = landPart;
            //add small px to land part
            //image
            //load bitmap with gdi+           
            Bitmap bmp = LoadBitmap("../../Demo/arrow_open.png");
            LayoutFarm.SampleControls.UIImageBox imgBox = new SampleControls.UIImageBox(bmp.Width, bmp.Height);
            imgBox.Image = bmp;
            //--------------------------------------
            //2. float part
            var floatPart = new LayoutFarm.SampleControls.UIPanel(400, 100);
            floatPart.BackColor = Color.Blue;
            hingeBox.FloatPart = floatPart;

            //--------------------------------------
            //if click on this image then
            imgBox.MouseDown += (s, e) =>
            {
                e.CancelBubbling = true;

                if (hingeBox.IsOpen)
                {
                    hingeBox.CloseHinge();
                }
                else
                {
                    hingeBox.OpenHinge();
                }
            };
            landPart.AddChildBox(imgBox);
            return hingeBox;
        }

        static Bitmap LoadBitmap(string filename)
        {
            System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(filename);
            Bitmap bmp = new Bitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
            return bmp;
        }
    }
}