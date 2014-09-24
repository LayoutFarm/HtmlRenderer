//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm
{

    public class UIHtmlBox : UIElement
    {
        HtmlRenderBox myHtmlBox;
        int _width, _height;
        public UIHtmlBox(int width, int height)
        {
            this._width = width;
            this._height = height;
        }

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (myHtmlBox == null)
            {
                myHtmlBox = new HtmlRenderBox(rootgfx, _width, _height);
            }
            return myHtmlBox;
        }
        public void LoadHtmlText(string html)
        {
            myHtmlBox.LoadHtmlText(html);
            myHtmlBox.InvalidateGraphic();
        }
        public override void InvalidateGraphic()
        {
            myHtmlBox.InvalidateGraphic();
        }
    }
}





