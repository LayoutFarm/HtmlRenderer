// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("1.6 ScrollView")]
    class Demo_ScrollView : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            AddScrollView1(viewport, 0, 0);

            AddScrollView2(viewport, 250, 0);
        }
        void AddScrollView1(SampleViewport viewport, int x, int y)
        {
            var panel = new LayoutFarm.CustomWidgets.Panel(200, 175);
            panel.SetLocation(x + 30, y + 30);
            panel.BackColor = Color.LightGray;
            viewport.AddContent(panel);
            //-------------------------  
            {
                //vertical scrollbar
                var vscbar = new LayoutFarm.CustomWidgets.ScrollBar(15, 200);
                vscbar.SetLocation(x + 10, y + 10);
                vscbar.MinValue = 0;
                vscbar.MaxValue = 170;
                vscbar.SmallChange = 20;
                viewport.AddContent(vscbar);
                //add relation between viewpanel and scroll bar 
                var scRelation = new LayoutFarm.CustomWidgets.ScrollingRelation(vscbar, panel);
            }
            //-------------------------  
            {
                //horizontal scrollbar
                var hscbar = new LayoutFarm.CustomWidgets.ScrollBar(200, 15);
                hscbar.ScrollBarType = CustomWidgets.ScrollBarType.Horizontal;
                hscbar.SetLocation(x + 30, y + 10);
                hscbar.MinValue = 0;
                hscbar.MaxValue = 170;
                hscbar.SmallChange = 20;
                viewport.AddContent(hscbar);
                //add relation between viewpanel and scroll bar 
                var scRelation = new LayoutFarm.CustomWidgets.ScrollingRelation(hscbar, panel);
            }

            //add content to panel
            for (int i = 0; i < 10; ++i)
            {
                var box1 = new LayoutFarm.CustomWidgets.EaseBox(30, 30);
                box1.BackColor = Color.OrangeRed;
                box1.SetLocation(i * 20, i * 40);

                panel.AddChildBox(box1);
            }
            //--------------------------   

            panel.SetViewport(0, 0);
        }
        void AddScrollView2(SampleViewport viewport, int x, int y)
        {
            var panel = new LayoutFarm.CustomWidgets.Panel(400, 175);
            panel.SetLocation(x + 30, y + 30);
            panel.BackColor = Color.LightGray;
            viewport.AddContent(panel);
            //-------------------------  
            {
                //vertical scrollbar
                var vscbar = new LayoutFarm.CustomWidgets.ScrollBar(15, 200);
                vscbar.SetLocation(x + 10, y + 10);
                vscbar.MinValue = 0;
                vscbar.MaxValue = 170;
                vscbar.SmallChange = 20;
                viewport.AddContent(vscbar);
                //add relation between viewpanel and scroll bar 
                var scRelation = new LayoutFarm.CustomWidgets.ScrollingRelation(vscbar, panel);
            }
            //-------------------------  
            {
                //horizontal scrollbar
                var hscbar = new LayoutFarm.CustomWidgets.ScrollBar(300, 15);
                hscbar.ScrollBarType = CustomWidgets.ScrollBarType.Horizontal;
                hscbar.SetLocation(x + 30, y + 10);
                hscbar.MinValue = 0;
                hscbar.MaxValue = 170;
                hscbar.SmallChange = 20;
                viewport.AddContent(hscbar);
                //add relation between viewpanel and scroll bar 
                var scRelation = new LayoutFarm.CustomWidgets.ScrollingRelation(hscbar, panel);
            }
            //-------------------------  
            //load images...
            for (int i = 0; i < 5; ++i)
            {

                var bmp = LoadBitmap("../../images/0" + (i + 1) + ".jpg");

                var imgbox = new LayoutFarm.CustomWidgets.ImageBox(bmp.Width, bmp.Height);
                panel.AddChildBox(imgbox);
                imgbox.Image = bmp;
                imgbox.BackColor = Color.OrangeRed;
                imgbox.SetLocation(i * 20, (i * 50) + 5);

            }
            //--------------------------   

            panel.SetViewport(0, 0);
        }
        static Bitmap LoadBitmap(string filename)
        {
            System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(filename);
            Bitmap bmp = new Bitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
            return bmp;
        }
    }
}