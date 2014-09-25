//from github.com/vvvv/svg 
//license : Microsoft Public License (MS-PL) 

﻿using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using System.Globalization;
using HtmlRenderer;

namespace Svg.Transforms
{
    /// <summary>
    /// The class which applies the specified shear vector to this Matrix.
    /// </summary>
    public sealed class SvgShear : SvgTransform
    {
        private float shearFactorX;
        private float shearFactorY;

        public float X
        {
            get { return this.shearFactorX; }
            set { this.shearFactorX = value; }
        }

        public float Y
        {
            get { return this.shearFactorY; }
            set { this.shearFactorY = value; }
        }

        public override Matrix Matrix
        {
            get
            {
                Matrix matrix = CurrentGraphicPlatform.CreateMatrix();
                matrix.Shear(this.X, this.Y);
                return matrix;
            }
        }

        public override string WriteToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "shear({0}, {1})", this.X, this.Y);
        }

        public SvgShear(float x) : this(x, x) { }

        public SvgShear(float x, float y)
        {
            this.shearFactorX = x;
            this.shearFactorY = y;
        }


		public override object Clone()
		{
			return new SvgShear(this.X, this.Y);
		}
    }
}