//MS-PL, Apache2 
//2014,2015, WinterDev

using System;
using PixelFarm.Drawing;
using System.Collections.Generic;

using LayoutFarm;
using LayoutFarm.Css;
using LayoutFarm.Svg;
using LayoutFarm.HtmlBoxes;
namespace LayoutFarm.Svg
{
    class SvgImageBinder : ImageBinder
    {
        public SvgImageBinder(string imgsrc)
            : base(imgsrc)
        {
        }
    }
}