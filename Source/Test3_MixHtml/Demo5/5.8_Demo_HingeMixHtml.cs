﻿//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using LayoutFarm.UI;

using HtmlRenderer.Composers;
using HtmlRenderer.WebDom;
using HtmlRenderer.WebDom.Extension;

using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    [DemoNote("5.8 HingeMixHtml")]
    class Demo_HingeMixHtml : DemoBase
    {
        Bitmap arrowBmp;
        HtmlIslandHost islandHost;
        LightHtmlBoxHost lightBoxHost;

        protected override void OnStartDemo(SampleViewport viewport)
        {
            //init host
            this.islandHost = new HtmlIslandHost();
            this.islandHost.BaseStylesheet = HtmlRenderer.Composers.CssParserHelper.ParseStyleSheet(null, true);

            lightBoxHost = new LightHtmlBoxHost(islandHost, viewport.P);
            lightBoxHost.SetRootGraphic(viewport.ViewportControl.WinTopRootGfx);
            
            //-----------
            var comboBox1 = CreateComboBox(20, 20);
            viewport.AddContent(comboBox1);

            var comboBox2 = CreateComboBox(50, 50);
            viewport.AddContent(comboBox2);

            //------------
            var menuItem = CreateMenuItem(10, 120);
            var menuItem2 = CreateMenuItem2(5, 5);

            menuItem.AddSubMenuItem(menuItem2);

            viewport.AddContent(menuItem);

        }
        LayoutFarm.CustomWidgets.ComboBox CreateComboBox(int x, int y)
        {
            LayoutFarm.CustomWidgets.ComboBox comboBox = new CustomWidgets.ComboBox(400, 20);
            comboBox.SetLocation(x, y);
            //--------------------
            //1. create landing part
            var landPart = new LayoutFarm.CustomWidgets.Panel(400, 20);
            landPart.BackColor = Color.Green;
            comboBox.LandPart = landPart;

            //--------------------------------------
            //add small px to land part
            //image
            //load bitmap with gdi+                
            if (arrowBmp == null)
            {
                arrowBmp = LoadBitmap("../../Demo/arrow_open.png");
            }
            LayoutFarm.CustomWidgets.ImageBox imgBox = new CustomWidgets.ImageBox(arrowBmp.Width, arrowBmp.Height);
            imgBox.Image = arrowBmp;
            //--------------------------------------
            //2. float part
            var floatPart = new LayoutFarm.CustomWidgets.Panel(400, 100);
            floatPart.BackColor = Color.Blue;
            comboBox.FloatPart = floatPart;

            //--------------------------------------
            //if click on this image then
            imgBox.MouseDown += (s, e) =>
            {
                e.CancelBubbling = true;

                if (comboBox.IsOpen)
                {
                    comboBox.CloseHinge();
                }
                else
                {
                    comboBox.OpenHinge();
                }
            };
            landPart.AddChildBox(imgBox);
            return comboBox;
        }
        LayoutFarm.CustomWidgets.MenuItem CreateMenuItem(int x, int y)
        {
            LayoutFarm.CustomWidgets.MenuItem mnuItem = new CustomWidgets.MenuItem(150, 20);
            mnuItem.SetLocation(x, y);
            //--------------------
            //1. create landing part
            var landPart = new LayoutFarm.CustomWidgets.Panel(150, 20);
            landPart.BackColor = Color.OrangeRed;
            mnuItem.LandPart = landPart;
            //--------------------------------------
            //add small px to land part
            //image
            //load bitmap with gdi+                
            if (arrowBmp == null)
            {
                arrowBmp = LoadBitmap("../../Demo/arrow_open.png");
            }
            LayoutFarm.CustomWidgets.ImageBox imgBox = new CustomWidgets.ImageBox(arrowBmp.Width, arrowBmp.Height);
            imgBox.Image = arrowBmp;
            landPart.AddChildBox(imgBox);

            //--------------------------------------
            //if click on this image then
            imgBox.MouseDown += (s, e) =>
            {
                e.CancelBubbling = true;

                if (mnuItem.IsOpen)
                {
                    mnuItem.CloseHinge();
                }
                else
                {
                    mnuItem.OpenHinge();
                }
            };
            //--------------------------------------
            //2. float part
            var floatPart = new LayoutFarm.CustomWidgets.MenuBox(400, 100);
            floatPart.BackColor = Color.Gray;
            mnuItem.FloatPart = floatPart;


            return mnuItem;
        }
        LayoutFarm.CustomWidgets.MenuItem CreateMenuItem2(int x, int y)
        {
            LayoutFarm.CustomWidgets.MenuItem mnuItem = new CustomWidgets.MenuItem(150, 20);
            mnuItem.SetLocation(x, y);
            //--------------------
            //1. create landing part
            var landPart = new LayoutFarm.CustomWidgets.Panel(150, 20);
            landPart.BackColor = Color.OrangeRed;
            mnuItem.LandPart = landPart;
            //--------------------------------------
            //add small px to land part
            //image
            //load bitmap with gdi+                
            if (arrowBmp == null)
            {
                arrowBmp = LoadBitmap("../../Demo/arrow_open.png");
            }
            LayoutFarm.CustomWidgets.ImageBox imgBox = new CustomWidgets.ImageBox(arrowBmp.Width, arrowBmp.Height);
            imgBox.Image = arrowBmp;
            landPart.AddChildBox(imgBox);

            //--------------------------------------
            //if click on this image then
            imgBox.MouseDown += (s, e) =>
            {
                e.CancelBubbling = true;

                if (mnuItem.IsOpen)
                {
                    mnuItem.CloseHinge();
                }
                else
                {
                    mnuItem.OpenHinge();
                }
            };
            //--------------------------------------
            //2. float part
            var floatPart = new LayoutFarm.CustomWidgets.MenuBox(400, 200);
            floatPart.BackColor = Color.LightGray;
            mnuItem.FloatPart = floatPart;
            //--------------------------------------
            //add mix html here

            {
                LightHtmlBox lightHtmlBox2 = lightBoxHost.CreateLightBox(floatPart.Width, floatPart.Height);
                lightHtmlBox2.SetLocation(0, 0);
                floatPart.AddChildBox(lightHtmlBox2);
                //light box can't load full html
                //all light boxs of the same lightbox host share resource with the host
                string html2 = @"<div>OK1</div><div>OK2</div><div>OK3</div><div>OK4</div>";
                //if you want to use ful l html-> use HtmlBox instead  
                lightHtmlBox2.LoadHtmlFragmentText(html2);
            }

            //--------------------------------------
            return mnuItem;
        }
        static Bitmap LoadBitmap(string filename)
        {
            System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(filename);
            Bitmap bmp = new Bitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
            return bmp;
        }
    }
}