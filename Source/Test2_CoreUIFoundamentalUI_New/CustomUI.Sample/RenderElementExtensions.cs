//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LayoutFarm.Presentation.UI
{

    public static class RenderElementExtensions
    { 
        public static void SetStyleDefinition(this RenderElement ve, BoxStyle beh, VisualElementArgs vinv)
        {
            ve.SetStyle(beh, vinv);
        }

    }
}