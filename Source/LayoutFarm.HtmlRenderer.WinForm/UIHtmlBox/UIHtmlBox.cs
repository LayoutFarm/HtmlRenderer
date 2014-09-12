//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using LayoutFarm.Presentation.UI;
namespace LayoutFarm.Presentation
{

    public class UIHtmlBox : UIElement
    {
        HtmlRenderBox myHtmlBox;
        public UIHtmlBox(int width, int height)
        {
            myHtmlBox = new HtmlRenderBox(width, height);
        }
        public HtmlRenderBox PrimaryVisual
        {
            get
            {
                return this.myHtmlBox;
            }
        }
        public void LoadHtmlText(string html)
        {
            myHtmlBox.LoadHtmlText(html); 
        }

    }
}





