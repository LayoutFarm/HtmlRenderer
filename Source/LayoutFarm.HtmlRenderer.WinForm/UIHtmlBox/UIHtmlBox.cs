//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LayoutFarm.Presentation
{

    public class UIHtmlBox : UIElement
    {
        ArtVisualHtmlBox myHtmlBox;
        public UIHtmlBox(int width, int height)
        {
            myHtmlBox = new ArtVisualHtmlBox(width, height);
        }
        public ArtVisualHtmlBox PrimaryVisual
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





