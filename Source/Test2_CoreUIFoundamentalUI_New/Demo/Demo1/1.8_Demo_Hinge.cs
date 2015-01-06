//2014,2015 Apache2, WinterDev
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
        Bitmap arrowBmp;
        protected override void OnStartDemo(SampleViewport viewport) 
        {
            var hingeBox1 = CreateComboBox(20, 20); 
            viewport.AddContent(hingeBox1);

            var hingeBox2 = CreateComboBox(50, 50);
            viewport.AddContent(hingeBox2);


            //------------
            var menuItem = CreateMenuItem(50, 100);
            var menuItem2 = CreateMenuItem(5, 5);
            menuItem.AddSubMenuItem(menuItem2);


            viewport.AddContent(menuItem);

        }
        LayoutFarm.CustomWidgets.UIComboBox CreateComboBox(int x, int y)
        {
            LayoutFarm.CustomWidgets.UIComboBox hingeBox = new CustomWidgets.UIComboBox(400, 20);
            hingeBox.SetLocation(x, y);
            //--------------------
            //1. create landing part
            var landPart = new LayoutFarm.CustomWidgets.UIPanel(400, 20);
            landPart.BackColor = Color.Green;
            hingeBox.LandPart = landPart;

            //--------------------------------------
            //add small px to land part
            //image
            //load bitmap with gdi+                
            if (arrowBmp == null)
            {
                arrowBmp = LoadBitmap("../../Demo/arrow_open.png");
            }
            LayoutFarm.CustomWidgets.UIImageBox imgBox = new CustomWidgets.UIImageBox(arrowBmp.Width, arrowBmp.Height);
            imgBox.Image = arrowBmp;
            //--------------------------------------
            //2. float part
            var floatPart = new LayoutFarm.CustomWidgets.UIPanel(400, 100);
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
        LayoutFarm.CustomWidgets.UIMenuItem CreateMenuItem(int x, int y)
        {
            LayoutFarm.CustomWidgets.UIMenuItem hingeBox = new CustomWidgets.UIMenuItem(150, 20);
            hingeBox.SetLocation(x, y);
            //--------------------
            //1. create landing part
            var landPart = new LayoutFarm.CustomWidgets.UIPanel(150, 20);
            landPart.BackColor = Color.OrangeRed;
            hingeBox.LandPart = landPart;
            //--------------------------------------
            //add small px to land part
            //image
            //load bitmap with gdi+                
            if (arrowBmp == null)
            {
                arrowBmp = LoadBitmap("../../Demo/arrow_open.png");
            }
            LayoutFarm.CustomWidgets.UIImageBox imgBox = new CustomWidgets.UIImageBox(arrowBmp.Width, arrowBmp.Height);
            imgBox.Image = arrowBmp;
            //--------------------------------------
            //2. float part
            var floatPart = new LayoutFarm.CustomWidgets.UIMenuBox(400, 100);
            floatPart.BackColor = Color.Gray;
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