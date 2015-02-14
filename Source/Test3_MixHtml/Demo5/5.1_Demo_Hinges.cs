// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;

using LayoutFarm.Composers;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.CustomWidgets;

namespace LayoutFarm.WebWidgets
{
    [DemoNote("5.1 HingeMixHtml")]
    class Demo_HingeMixHtml : HtmlDemoBase
    {


        protected override void OnHtmlHostCreated()
        {
            //------------------------ 
            var comboBox1 = CreateComboBox(20, 20);
            //this.sampleViewport.AddContent(comboBox1);
            AddToViewport(comboBox1);
            var comboBox2 = CreateComboBox(50, 50);
            //this.sampleViewport.AddContent(comboBox2);
            AddToViewport(comboBox2);
            //------------
            //var menuItem = CreateMenuItem(10, 120);
            //var menuItem2 = CreateMenuItem2(5, 5);

            //menuItem.AddSubMenuItem(menuItem2);

            //this.sampleViewport.AddContent(menuItem);
            //------------
            LayoutFarm.HtmlWidgets.MenuBox rootMenuBox = CreateMenuBox(10, 120);
            //add single menu item
            var rootMenuItem = new HtmlWidgets.MenuItem(150, 20);
            rootMenuItem.MenuItemText = "level0";
            rootMenuBox.AddChildBox(rootMenuItem);

            for (int i = 0; i < 10; ++i)
            {
                var menuItem = new HtmlWidgets.MenuItem(150, 20);
                menuItem.MenuItemText = "item" + i;
                //menuBox.AddChildBox(menuItem);
                rootMenuItem.AddSubMenuItem(menuItem);
            }

            AddToViewport(rootMenuBox);
            rootMenuBox.SetTopWindowRenderBox(this.sampleViewport.Root.TopWindowRenderBox);
        }

        LayoutFarm.HtmlWidgets.ComboBox CreateComboBox(int x, int y)
        {

            LayoutFarm.HtmlWidgets.ComboBox comboBox = new HtmlWidgets.ComboBox(400, 20);
            comboBox.SetLocation(x, y);
            ////--------------------
            ////1. create landing part 
            //var landPart = new LightHtmlBox(this.myHtmlHost, 400, 02);
            //landPart.LoadHtmlDom(CreateMenuItemDetail());
            //comboBox.LandPart = landPart;
            ////--------------------------------------
            ////add small px to land part
            ////image
            ////load bitmap with gdi+                

            ////LayoutFarm.CustomWidgets.ImageBox imgBox = new CustomWidgets.ImageBox(arrowBmp.Image.Width, arrowBmp.Image.Height);
            ////imgBox.ImageBinder = arrowBmp;
            ////--------------------------------------
            ////2. float part
            //var floatPart = new LightHtmlBox(this.myHtmlHost, 400, 100);
            //comboBox.FloatPart = floatPart;

            ////--------------------------------------
            ////if click on this image then
            //imgBox.MouseDown += (s, e) =>
            //{
            //    e.CancelBubbling = true;

            //    if (comboBox.IsOpen)
            //    {
            //        comboBox.CloseHinge();
            //    }
            //    else
            //    {
            //        comboBox.OpenHinge();
            //    }
            //};
            //landPart.AddChildBox(imgBox);
            return comboBox;
        }
        LayoutFarm.HtmlWidgets.MenuBox CreateMenuBox(int x, int y)
        {
            var menuBox = new LayoutFarm.HtmlWidgets.MenuBox(200, 400);
            menuBox.SetLocation(x, y);
            return menuBox;
        }
        LayoutFarm.HtmlWidgets.MenuItem CreateMenuItem(int x, int y)
        {
            LayoutFarm.HtmlWidgets.MenuItem mnuItem = new HtmlWidgets.MenuItem(150, 20);
            mnuItem.MenuItemText = "OKOK";

            //mnuItem.SetLocation(x, y);
            //--------------------
            ////1. create landing part
            //var landPart = new LayoutFarm.CustomWidgets.Panel(150, 20);
            //landPart.BackColor = Color.OrangeRed;
            //mnuItem.LandPart = landPart;
            //--------------------------------------
            //add small px to land part
            //image
            //load bitmap with gdi+                
            //if (arrowBmp == null)
            //{
            //    arrowBmp = LoadImage("../../Demo/arrow_open.png");
            //}
            //LayoutFarm.CustomWidgets.ImageBox imgBox = new CustomWidgets.ImageBox(arrowBmp.Image.Width, arrowBmp.Image.Height);
            //imgBox.ImageBinder = arrowBmp;
            //landPart.AddChildBox(imgBox);

            //--------------------------------------
            //if click on this image then
            //imgBox.MouseDown += (s, e) =>
            //{
            //    e.CancelBubbling = true;

            //    if (mnuItem.IsOpened)
            //    {
            //        mnuItem.Close();
            //    }
            //    else
            //    {
            //        mnuItem.Open();
            //    }
            //};
            //--------------------------------------
            //2. float part
            //var floatPart = new LayoutFarm.CustomWidgets.MenuBox(400, 100);
            //floatPart.BackColor = Color.Gray;
            //mnuItem.FloatPart = floatPart;


            return mnuItem;
        }
        //LayoutFarm.HtmlWidgets.MenuItem CreateMenuItem2(int x, int y)
        //{
        //    LayoutFarm.CustomWidgets.MenuItem mnuItem = new CustomWidgets.MenuItem(150, 20);
        //    mnuItem.SetLocation(x, y);
        //    //--------------------
        //    //1. create landing part
        //    var landPart = new LayoutFarm.CustomWidgets.Panel(150, 20);
        //    landPart.BackColor = Color.OrangeRed;
        //    mnuItem.LandPart = landPart;
        //    //--------------------------------------
        //    //add small px to land part
        //    //image
        //    //load bitmap with gdi+                
        //    //if (arrowBmp == null)
        //    //{
        //    //    arrowBmp = LoadImage("../../Demo/arrow_open.png");
        //    //}
        //    //LayoutFarm.CustomWidgets.ImageBox imgBox = new CustomWidgets.ImageBox(arrowBmp.Image.Width, arrowBmp.Image.Height);
        //    //imgBox.ImageBinder = arrowBmp;
        //    //landPart.AddChildBox(imgBox);

        //    //--------------------------------------
        //    //if click on this image then
        //    //imgBox.MouseDown += (s, e) =>
        //    //{
        //    //    e.CancelBubbling = true;

        //    //    if (mnuItem.IsOpened)
        //    //    {
        //    //        mnuItem.Close();
        //    //    }
        //    //    else
        //    //    {
        //    //        mnuItem.Open();
        //    //    }
        //    //};
        //    //--------------------------------------
        //    //2. float part
        //    var floatPart = new LayoutFarm.CustomWidgets.MenuBox(400, 200);
        //    floatPart.BackColor = Color.LightGray;
        //    mnuItem.FloatPart = floatPart;
        //    //--------------------------------------
        //    //add mix html here 
        //    {

        //        LightHtmlBox lightHtmlBox2 = new LightHtmlBox(this.myHtmlHost, floatPart.Width, floatPart.Height);
        //        lightHtmlBox2.SetLocation(0, 0);
        //        floatPart.AddChildBox(lightHtmlBox2);
        //        //light box can't load full html
        //        //all light boxs of the same lightbox host share resource with the host
        //        //string html2 = @"<div>OK1</div><div>OK2</div><div>OK3</div><div>OK4</div>";
        //        //if you want to use ful l html-> use HtmlBox instead  
        //        lightHtmlBox2.LoadHtmlDom(CreatePresentationNode(floatPart));
        //        //lightHtmlBox2.LoadHtmlFragmentText(html2);
        //    }

        //    //--------------------------------------
        //    return mnuItem;
        //}

        FragmentHtmlDocument CreateMenuItemDetail()
        {
            FragmentHtmlDocument htmldoc = this.myHtmlHost.CreateNewFragmentHtml();// new HtmlDocument();
            var rootNode = htmldoc.RootNode;
            rootNode.AddChild("div", div =>
            {
                div.AddTextContent("menu");
            });
            return htmldoc;
        }
        FragmentHtmlDocument CreatePresentationNode(MenuBox ownerMenuBox)
        {
            FragmentHtmlDocument htmldoc = this.myHtmlHost.CreateNewFragmentHtml();// new HtmlDocument();
            var rootNode = htmldoc.RootNode;
            //1. create body node             
            // and content  

            //style 2, lambda and adhoc attach event
            rootNode.AddChild("body", body =>
            {
                body.AddChild("div", div =>
                {
                    MenuBox menuBox = new MenuBox(200, 100);
                    div.AddChild("span", span =>
                    {
                        //test menubox 
                        span.AddTextContent("ABCD");
                        //3. attach event to specific span
                        span.AttachMouseDownEvent(e =>
                        {
#if DEBUG
                            // System.Diagnostics.Debugger.Break();                           
                            //test change span property 
                            //clear prev content and add new  text content                             
                            span.ClearAllElements();
                            span.AddTextContent("XYZ0001");
#endif

                            menuBox.SetLocation(50, 50);
                            //add to top window
                            menuBox.ShowMenu(sampleViewport.Root);

                            e.StopPropagation();

                        });
                    });

                    div.AddChild("span", span =>
                    {
                        span.AddTextContent("EFGHIJK");
                        span.AttachMouseDownEvent(e =>
                        {
                            span.ClearAllElements();
                            span.AddTextContent("LMNOP0003");

                            //test hide menu                             
                            menuBox.HideMenu();

                        });
                    });
                    //----------------------
                    div.AttachEvent(UIEventName.MouseDown, e =>
                    {
#if DEBUG
                        //this will not print 
                        //if e has been stop by its child
                        // System.Diagnostics.Debugger.Break();
                        //Console.WriteLine("div");
#endif

                    });
                });
            });


            return htmldoc;
        }
    }
}