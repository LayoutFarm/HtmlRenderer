using System;
using PixelFarm.Drawing;

namespace LayoutFarm.Css
{

    public class BoxSpecNode
    {

        BoxSpecNode parentNode;
        public BoxSpecNode ParentNode
        {
            get { return this.parentNode; }
            set { this.parentNode = value; }
        } 
    }
}