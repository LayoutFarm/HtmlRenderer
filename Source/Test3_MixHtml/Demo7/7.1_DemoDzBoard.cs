// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.CustomWidgets;
using LayoutFarm.RenderBoxes;


namespace LayoutFarm.DzBoardSample
{
    [DemoNote("7.1 Demo DesignBoard")]
    class Demo_DzBoard : DemoBase
    {
        AppModule appModule = new AppModule();

        protected override void OnStartDemo(SampleViewport viewport)
        {
            appModule.StartModule(viewport);
        }
    }
}