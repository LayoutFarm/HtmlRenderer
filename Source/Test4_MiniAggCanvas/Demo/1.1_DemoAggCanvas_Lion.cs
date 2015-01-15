// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;

using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    [DemoNote("1.1 DemoAggCanvas_Lion.cs")]
    class DemoAggCanvasLion : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {

            var miniAggg = new MiniAggCanvasRenderElement(viewport.ViewportControl.RootGfx, 800, 600);
            viewport.AddContent(miniAggg);

            //var sampleButton = new LayoutFarm.CustomWidgets.EaseBox(30, 30);
            //viewport.AddContent(sampleButton);

            //int count = 0;
            //sampleButton.MouseDown += new EventHandler<UIMouseEventArgs>((s, e2) =>
            //{
            //    Console.WriteLine("click :" + (count++));
            //});

        }
    }
}