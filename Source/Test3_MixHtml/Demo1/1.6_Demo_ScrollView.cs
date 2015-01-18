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

            var panel = new LayoutFarm.CustomWidgets.Panel(300, 200);
            panel.SetLocation(30,30);
            panel.BackColor = Color.LightGray;
            viewport.AddContent(panel);
            //------------------------- 

            {
                //vertical scrollbar
                var vscbar = new LayoutFarm.CustomWidgets.ScrollBar(15, 200);
                vscbar.SetLocation(10, 10);
                vscbar.MinValue = 0;
                vscbar.MaxValue = 170;
                vscbar.SmallChange = 20;
                viewport.AddContent(vscbar);
                //add relation between viewpanel and scroll bar 
                var scRelation = new LayoutFarm.CustomWidgets.ScrollingRelation(vscbar, panel);
            }

            {
                //horizontal scrollbar
                var hscbar = new LayoutFarm.CustomWidgets.ScrollBar(300, 15);
                hscbar.ScrollBarType = CustomWidgets.ScrollBarType.Horizontal;
                hscbar.SetLocation(30, 10);
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
    }
}