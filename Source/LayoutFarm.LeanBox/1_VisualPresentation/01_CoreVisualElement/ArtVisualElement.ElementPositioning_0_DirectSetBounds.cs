//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using LayoutFarm.Presentation;


namespace LayoutFarm.Presentation
{


    partial class ArtVisualElement
    {


        public static void DirectSetVisualElementSize(ArtVisualElement visualElement, int width, int height)
        {

            visualElement.b_width = width;
            visualElement.b_Height = height;
        }
 

        public static void DirectSetVisualElementLocation(ArtVisualElement visualElement, int x, int y)
        {

            visualElement.b_left = x;
            visualElement.b_top = y;


        }

    }
}