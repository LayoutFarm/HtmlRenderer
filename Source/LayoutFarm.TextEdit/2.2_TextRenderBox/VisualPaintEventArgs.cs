// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PixelFarm.Drawing;
namespace LayoutFarm.Text
{

    public class VisualPaintEventArgs : EventArgs
    {
        public Canvas canvas;
        public Rectangle updateArea;
        public VisualPaintEventArgs(Canvas canvas, Rectangle updateArea)
        {   
            this.canvas = canvas;
            this.updateArea = updateArea;
        }
        public Canvas Canvas
        {
            get
            {
                return canvas;
            }
        }
        public Rectangle UpdateArea
        {
            get
            {
                return updateArea;
            }
        }
    }

    public delegate void VisualPaintEventHandler(object sender, VisualPaintEventArgs e);

}