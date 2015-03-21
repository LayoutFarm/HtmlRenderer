// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;


namespace LayoutFarm.Text
{
    static class StyleHelper
    {

        public static TextSpanStyle CreateNewStyle(Color color)
        {

            if (color != Color.Empty)
            {

                TextSpanStyle simpleBeh = new TextSpanStyle();
               // simpleBeh.SharedBgColorBrush = new ArtSolidBrush(color);
                return simpleBeh;
            }
            else
            {
                TextSpanStyle simpleBeh = new TextSpanStyle();
                //simpleBeh.SharedBgColorBrush = new ArtSolidBrush(color);
                return simpleBeh;

            }

        }

    }

}