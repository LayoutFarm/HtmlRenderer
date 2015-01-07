// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm
{
    public static class RenderElementHelper
    {

        public static void DrawBackground(RenderElement visualElement,
           Canvas canvasPage, int width, int height, Color color)
        {
           
            canvasPage.FillRectangle(color, 0, 0, width, height);

        }

    }
}