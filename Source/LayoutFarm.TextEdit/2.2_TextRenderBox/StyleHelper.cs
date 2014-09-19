//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace LayoutFarm.Text
{
    static class StyleHelper
    {

        public static TextSpanSytle CreateNewStyle(Color color)
        {

            if (color != Color.Empty)
            {

                TextSpanSytle simpleBeh = new TextSpanSytle();
                simpleBeh.SharedBgColorBrush = new ArtSolidBrush(color);
                return simpleBeh;
            }
            else
            {
                TextSpanSytle simpleBeh = new TextSpanSytle();
                simpleBeh.SharedBgColorBrush = new ArtSolidBrush(color);
                return simpleBeh;

            }

        }

    }

}