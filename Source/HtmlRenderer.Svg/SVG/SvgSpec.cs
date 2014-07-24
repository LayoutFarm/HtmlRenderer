//MS-PL, Apache2 
//2014, WinterDev

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using HtmlRenderer.Css;

namespace HtmlRenderer.SvgDom
{
    public class SvgVisualSpec
    {
        Color fillColor = Color.Black;
        Color strokeColor = Color.Transparent;
        public Color ActualColor
        {
            get { return this.fillColor; }
            set { this.fillColor = value; }
        }
        public Color StrokeColor
        {
            get { return this.strokeColor; }
            set { this.strokeColor = value; }
        }
        public CssLength StrokeWidth
        {
            get;
            set;
        }
    }
    public class SvgRectSpec : SvgVisualSpec
    { 
        public CssLength X
        {
            get;
            set;
        }
        public CssLength Y
        {
            get;
            set;
        }
        public CssLength Width
        {
            get;
            set;
        }
        public CssLength Height
        {
            get;
            set;
        }

        public CssLength CornerRadiusX
        {
            get;
            set;
        }
        public CssLength CornerRadiusY
        {
            get;
            set;
        } 
    }

    
    public class SvgPolygonSpec : SvgVisualSpec
    {

    }

}