using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing; 

namespace LayoutFarm.Presentation
{
                static class InternalVisualRoleHelper
    {

        public static BoxStyle CreateSimpleRole(Color color)
        {
            BoxStyle simpleBeh = new BoxStyle();
            simpleBeh.SharedBgColorBrush = new ArtSolidBrush(color); 
            return simpleBeh;
            
        }
    }

}