// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("3.9 DemoTabPage")]
    class DemoTabPage : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            var tabContainer = new LayoutFarm.CustomWidgets.TabPageContainer(300, 400);
            tabContainer.SetLocation(10, 10);
            tabContainer.BackColor = KnownColors.FromKnownColor(KnownColor.LightGray);
            viewport.AddContent(tabContainer);

            //add 
            for (int i = 0; i < 10; ++i)
            {
                var tabPage = new LayoutFarm.CustomWidgets.TabPage(400, 20);
                if ((i % 2) == 0)
                {
                    tabPage.BackColor = KnownColors.FromKnownColor(KnownColor.OrangeRed);
                }
                else
                {
                    tabPage.BackColor = KnownColors.FromKnownColor(KnownColor.Yellow);
                }
                tabContainer.AddItem(tabPage);
            }
            tabContainer.SelectedIndex = 1;
        }
    }
}