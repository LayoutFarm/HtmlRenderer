//2014,2015 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm;


namespace LayoutFarm
{


    partial class RenderElement
    {


        public static void DirectSetVisualElementSize(RenderElement visualElement, int width, int height)
        {

            visualElement.b_width = width;
            visualElement.b_Height = height;
        }
 

        public static void DirectSetVisualElementLocation(RenderElement visualElement, int x, int y)
        {

            visualElement.b_left = x;
            visualElement.b_top = y;


        }

    }
}