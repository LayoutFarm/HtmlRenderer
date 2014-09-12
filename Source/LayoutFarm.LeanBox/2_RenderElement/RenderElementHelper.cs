//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LayoutFarm.Presentation
{
    public static class RenderElementHelper
    {

        public static void DrawBackground(RenderElement visualElement,
           CanvasBase canvasPage, int width, int height, Color color)
        {
            ArtColorBrush colorBrush = new ArtSolidBrush(color);

            if (colorBrush == null)
            {
                return;
            }

            canvasPage.FillRectangle(colorBrush, 0, 0, width, height);

        }

    }
}