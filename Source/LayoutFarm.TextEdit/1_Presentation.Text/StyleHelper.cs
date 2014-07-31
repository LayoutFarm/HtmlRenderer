//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace LayoutFarm.Presentation.Text
{
    static class StyleHelper
    {

        public static BoxStyle CreateNewStyle(Color color)
        {

            if (color != Color.Empty)
            {

                BoxStyle simpleBeh = new BoxStyle();
                simpleBeh.SharedBgColorBrush = new ArtSolidBrush(color);
                return simpleBeh;
            }
            else
            {
                BoxStyle simpleBeh = new BoxStyle();
                simpleBeh.SharedBgColorBrush = new ArtSolidBrush(color);
                return simpleBeh;

            }

        }

    }

}