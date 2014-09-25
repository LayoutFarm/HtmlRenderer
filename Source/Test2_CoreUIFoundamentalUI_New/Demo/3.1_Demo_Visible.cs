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
        protected override void OnStartDemo(UISurfaceViewportControl viewport)
        {
            {
                var box1 = new LayoutFarm.SampleControls.UIButton(50, 50);
                box1.BackColor = Color.Red;
                box1.SetLocation(10, 10);
                SetupActiveBoxProperties(box1);
                viewport.AddContent(box1);

            }
            //--------------------------------
            {
                var box2 = new LayoutFarm.SampleControls.UIButton(30, 30);
                box2.SetLocation(50, 50); 
                viewport.AddContent(box2);
            }
        }
        static void SetupActiveBoxProperties(LayoutFarm.SampleControls.UIButton box)
        {
            //1. mouse down         
            box.MouseDown += (s, e) =>
            {
                box.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                box.InvalidateGraphic();
            };
            box.MouseDown += (s, e) =>
            {
                box.BackColor = Color.Red;
                box.InvalidateGraphic();
            }; 
        }

    }
}