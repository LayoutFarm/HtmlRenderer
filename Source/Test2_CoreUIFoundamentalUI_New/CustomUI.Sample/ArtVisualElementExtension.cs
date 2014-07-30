//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing; 

namespace LayoutFarm.Presentation
{


    public static class ArtVisualElementExtensions
    {

        public static void SetStyleDefinition(this ArtVisualElement ve, BoxStyle beh, VisualElementArgs vinv)
        {
            ve.SetStyle(beh, vinv);
        }

    }
}