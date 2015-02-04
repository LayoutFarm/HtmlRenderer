// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.WebDom;

namespace LayoutFarm.HtmlWidgets
{

    public abstract class WidgetBase
    {
        int width;
        int height;
        int left;
        int top;

        public WidgetBase(int w, int h)
        {
            this.width = w;
            this.height = h;
        }
        public int Width
        {
            get { return this.width; }
        }
        public int Height
        {
            get { return this.height; }
        }
        public int Left
        {
            get { return this.left; } 
        }
        public int Top
        {
            get { return this.top; } 
        }

        public abstract UIElement GetPrimaryUIElement();

        public void SetLocation(int left, int top)
        {
            this.left = left;
            this.top = top;
        }
    }
}