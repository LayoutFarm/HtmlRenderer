//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("3.1 DemoVisible")]
    class Demo_Visible : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {

            var box1 = new LayoutFarm.CustomWidgets.UIEaseBox(50, 50);
            box1.BackColor = Color.Red;
            box1.SetLocation(10, 10);
            viewport.AddContent(box1);


            //--------------------------------
            var box2 = new LayoutFarm.CustomWidgets.UIEaseBox(30, 30);
            box2.SetLocation(50, 50);
            viewport.AddContent(box2);


            //1. mouse down         
            box1.MouseDown += (s, e) =>
            {
                box1.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                box1.InvalidateGraphic(); 
                box2.Visible = false;
                
            };
            box1.MouseUp += (s, e) =>
            {
                box1.BackColor = Color.Red;
                box1.InvalidateGraphic();


                box2.Visible = true;
            }; 

        }
       
    }
}