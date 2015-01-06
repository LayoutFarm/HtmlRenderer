//from github.com/vvvv/svg 
//license : Microsoft Public License (MS-PL) 

using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using System.Globalization;
using LayoutFarm;
namespace LayoutFarm.Svg.Transforms
{
    public sealed class SvgScale : SvgTransform
    {


        public float X
        {
            get;
            set;
        }

        public float Y
        {
            get;
            set;
        }

        //public override Matrix Matrix
        //{
        //    get
        //    {
        //        Matrix matrix = CurrentGraphicsPlatform.CreateMatrix();
        //        matrix.Scale(this.X, this.Y);
        //        return matrix;
        //    }
        //}

        //public override string WriteToString()
        //{
        //    return string.Format(CultureInfo.InvariantCulture, "scale({0}, {1})", this.X, this.Y);
        //}

        public SvgScale(float x) : this(x, x) { }

        public SvgScale(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        //public override object Clone()
        //{
        //    return new SvgScale(this.X, this.Y);
        //}
    }
}
