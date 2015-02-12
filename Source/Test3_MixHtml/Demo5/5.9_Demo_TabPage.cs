// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.WebWidgets
{
    [DemoNote("5.9 DemoTabPage")]
    class DemoTabPage : HtmlDemoBase
    {
        protected override void OnHtmlHostCreated()
        {

            var tabContainer = new LayoutFarm.HtmlWidgets.TabPageContainer(300, 400);
            tabContainer.SetLocation(10, 10);
            AddToViewport(tabContainer);
            for (int i = 0; i < 10; ++i)
            {
                var tabPage = new LayoutFarm.HtmlWidgets.TabPage();
                tabPage.PageTitle = "page" + i;

                //test bridge 
                var panel01 = new CustomWidgets.Panel(100, 100);
                tabPage.ContentUI = panel01;
                tabContainer.AddItem(tabPage);
            }

            //tabContainer.SelectedIndex = 1; 
        }

    }
}