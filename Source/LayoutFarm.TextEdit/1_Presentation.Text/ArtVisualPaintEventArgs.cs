//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;




namespace LayoutFarm.Presentation
{

    public class ArtVisualPaintEventArgs : EventArgs
    {
        public ArtCanvas canvas;
        public InternalRect updateArea;
        public ArtVisualPaintEventArgs(ArtCanvas canvas, InternalRect updateArea)
        {
            this.canvas = canvas;
            this.updateArea = updateArea;
        }
        public ArtCanvas Canvas
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

    public delegate void ArtVisualPaintEventHandler(object sender, ArtVisualPaintEventArgs e);

}