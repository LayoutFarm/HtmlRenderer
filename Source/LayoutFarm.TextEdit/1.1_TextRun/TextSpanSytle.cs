// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;


using System.Reflection;
using System.Text.RegularExpressions;

using PixelFarm.Drawing;

namespace LayoutFarm.Text
{

    public class TextSpanSytle
    {
        public TextSpanSytle()
        {
            //this.SharedBgColorBrush = new ArtSolidBrush(Color.White);
        }
        public bool FontBold
        {
            get;
            set;
        }
        //ArtColorBrush sharedBgColorBrush = null;
        //public ArtColorBrush SharedBgColorBrush
        //{
        //    get
        //    {
        //        return sharedBgColorBrush;
        //    }
        //    set
        //    {
        //        this.sharedBgColorBrush = value;
        //    }
        //}
        public Color FontColor
        {
            get;
            set;
        }
        public FontInfo FontInfo;

        public int ContentHAlign;
        public int positionWidth = -1;
        public int positionHeight = -1;
    }
}