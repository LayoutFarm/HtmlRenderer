// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;


namespace LayoutFarm.HtmlBoxes
{
    class UIBox
    {
        public int Left
        {
            get;
            set;
        }
        public int Top
        {
            get;
            set;
        }
        public int Width
        {
            get;
            set;
        }
        public int Height
        {
            get;
            set;
        }
        public float SizeWidth { get; set; }
        public float SizeHeight { get; set; }
        public void SetBounds(int x, int y, int w, int h)
        {
            this.Left = x;
            this.Top = y;
            this.Width = w;
            this.Height = h;
        }

    }

}