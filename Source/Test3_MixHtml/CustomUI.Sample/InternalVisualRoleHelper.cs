//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

namespace LayoutFarm.Presentation
{
    static class InternalVisualRoleHelper
    {

        public static TextRunStyle CreateSimpleRole(Color color)
        {
            TextRunStyle simpleBeh = new TextRunStyle();
            simpleBeh.SharedBgColorBrush = new ArtSolidBrush(color);
            return simpleBeh;

        }
    }

}