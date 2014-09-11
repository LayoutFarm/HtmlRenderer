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

        public static TextRunStyle CreateNewStyle(Color color)
        {

            if (color != Color.Empty)
            {

                TextRunStyle simpleBeh = new TextRunStyle();
                simpleBeh.SharedBgColorBrush = new ArtSolidBrush(color);
                return simpleBeh;
            }
            else
            {
                TextRunStyle simpleBeh = new TextRunStyle();
                simpleBeh.SharedBgColorBrush = new ArtSolidBrush(color);
                return simpleBeh;

            }

        }

    }

}