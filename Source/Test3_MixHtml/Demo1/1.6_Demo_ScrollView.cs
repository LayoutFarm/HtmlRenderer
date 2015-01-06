//2014,2015 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;  
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("1.6 ScrollView")]
    class Demo_ScrollView : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {

            var scbar = new LayoutFarm.CustomWidgets.ScrollBar(15, 200);
            scbar.SetLocation(10, 10);
            scbar.MinValue = 0;
            scbar.MaxValue = 170;
            scbar.SmallChange = 20;
            viewport.AddContent(scbar);
            //------------------------- 

            var panel = new LayoutFarm.CustomWidgets.Panel(300, 200);
            panel.SetLocation(30, 10);
            panel.BackColor = Color.LightGray;
            viewport.AddContent(panel);
            //------------------------- 

            //add relation between viewpanel and scroll bar 
            var scRelation = new LayoutFarm.CustomWidgets.ScrollingRelation(scbar, panel);

            //add content to panel
            for (int i = 0; i < 10; ++i)
            {
                var box1 = new LayoutFarm.CustomWidgets.EaseBox(30, 30);
                box1.BackColor = Color.OrangeRed;
                box1.SetLocation(i * 20, i * 40);

                panel.AddChildBox(box1);
            }
            //--------------------------   
            panel.SetViewport(0, 50);
        }
    }
}