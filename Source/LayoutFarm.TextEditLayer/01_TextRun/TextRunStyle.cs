//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;


using System.Reflection;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;
using System.Drawing;



namespace LayoutFarm 
{

    public class TextRunStyle
    {
        public TextRunStyle()
        {
            this.SharedBgColorBrush = new ArtSolidBrush(Color.White);
        }
        public bool FontBold
        {
            get;
            set;
        }
        ArtColorBrush sharedBgColorBrush = null;
        public ArtColorBrush SharedBgColorBrush
        {
            get
            {
                return sharedBgColorBrush;
            }
            set
            {
                this.sharedBgColorBrush = value;
            }
        }
        public Color FontColor
        {
            get;
            set;
        }
        public TextFontInfo textFontInfo;
         
        public int ContentHAlign;
        public int positionWidth = -1;
        public int positionHeight = -1; 
    }
}