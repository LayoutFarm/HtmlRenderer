//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;



namespace LayoutFarm
{

    public class ArtImageBrush : ArtColorBrush
    {

        Image myImage;
        string imageBrushName;

        public ArtImageBrush(string imageBrushName)
        {
            this.imageBrushName = imageBrushName;

        }
        public Image MyImage
        {
            get
            {
                return this.myImage;
            }
            set
            {
                this.myImage = value;
            }
        }
    }
}