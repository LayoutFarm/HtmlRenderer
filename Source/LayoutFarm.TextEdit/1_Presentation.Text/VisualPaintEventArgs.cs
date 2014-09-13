//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing; 
namespace LayoutFarm
{

    public class VisualPaintEventArgs : EventArgs
    {
        public Canvas canvas;
        public InternalRect updateArea;
        public VisualPaintEventArgs(Canvas canvas, InternalRect updateArea)
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
        public InternalRect UpdateArea
        {
            get
            {
                return updateArea;
            }
        }
    }

    public delegate void VisualPaintEventHandler(object sender, VisualPaintEventArgs e);

}