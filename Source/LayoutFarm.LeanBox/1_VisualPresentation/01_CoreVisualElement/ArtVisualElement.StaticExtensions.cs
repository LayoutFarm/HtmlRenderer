//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;





namespace LayoutFarm.Presentation
{

    partial class ArtVisualElement
    {
        protected static void InnerSetHasSubGroupLayer(ArtVisualElement ve, bool value)
        {


            ve.HasSubGroundLayer = value;
        }
    }
}