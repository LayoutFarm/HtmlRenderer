//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;





namespace LayoutFarm.Presentation
{

    partial class RenderElement
    {
        protected static void InnerSetHasSubGroupLayer(RenderElement ve, bool value)
        {


            ve.HasSubGroundLayer = value;
        }
    }
}