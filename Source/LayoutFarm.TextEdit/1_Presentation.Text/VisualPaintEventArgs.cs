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
        public CanvasBase canvas;
        public InternalRect updateArea;
        public VisualPaintEventArgs(CanvasBase canvas, InternalRect updateArea)
        {   
            this.canvas = canvas;
            this.updateArea = updateArea;
        }
        public CanvasBase Canvas
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