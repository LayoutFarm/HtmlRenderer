//from github.com/vvvv/svg 
//license : Microsoft Public License (MS-PL) 


namespace LayoutFarm.Svg.Transforms
{
    /// <summary>
    /// The class which applies custom transform to this Matrix
    /// </summary>
    public sealed class SvgTransformMatrix : SvgTransform
    {
        float[] _elements;
        public float[] Elements
        {
            get { return this._elements; }
            set { this._elements = value; }
        }
        public SvgTransformMatrix(float[] elements)
        {
            this._elements = elements;
        }
    }

    public abstract class SvgTransform
    {

    }

    public sealed class SvgTranslate : SvgTransform
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

        //public override  Matrix Matrix
        //{
        //    get
        //    {
        //        Matrix matrix = CurrentGraphicsPlatform.CreateMatrix();
        //        matrix.Translate(this.X, this.Y);
        //        return matrix;
        //    }
        //}

        //public override string WriteToString()
        //{
        //    return string.Format(CultureInfo.InvariantCulture, "translate({0}, {1})", this.X, this.Y);
        //}

        public SvgTranslate(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public SvgTranslate(float x)
            : this(x, 0.0f)
        {
        }

        //public override object Clone()
        //{
        //    return new SvgTranslate(this.x, this.y);
        //}
    }
    /// <summary>
    /// The class which applies the specified skew vector to this Matrix.
    /// </summary>
    public sealed class SvgSkew : SvgTransform
    {
        public float AngleX { get; set; }
        public float AngleY { get; set; }
        //public override Matrix Matrix
        //{
        //    get
        //    {
        //        var matrix = CurrentGraphicsPlatform.CreateMatrix();
        //        matrix.Shear(
        //            (float)Math.Tan(AngleX/180*Math.PI),
        //            (float)Math.Tan(AngleY/180*Math.PI));
        //        return matrix;
        //    }
        //}

        //public override string WriteToString()
        //{
        //    return string.Format(CultureInfo.InvariantCulture, "skew({0}, {1})", this.AngleX, this.AngleY);
        //}

        public SvgSkew(float x, float y)
        {
            AngleX = x;
            AngleY = y;
        }

        //public override object Clone()
        //{
        //    return new SvgSkew(this.AngleX, this.AngleY);
        //}
    }

    /// <summary>
    /// The class which applies the specified shear vector to this Matrix.
    /// </summary>
    public sealed class SvgShear : SvgTransform
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
        //        matrix.Shear(this.X, this.Y);
        //        return matrix;
        //    }
        //}

        //public override string WriteToString()
        //{
        //    return string.Format(CultureInfo.InvariantCulture, "shear({0}, {1})", this.X, this.Y);
        //}

        public SvgShear(float x) : this(x, x) { }

        public SvgShear(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        //public override object Clone()
        //{
        //    return new SvgShear(this.X, this.Y);
        //}
    }

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

    public sealed class SvgRotate : SvgTransform
    {
        public float Angle
        {
            get;
            set;
        }
        public float CenterX
        {
            get;
            set;
        }
        public float CenterY
        {
            get;
            set;
        }

        //public override Matrix Matrix
        //{
        //    get
        //    {
        //        Matrix matrix = CurrentGraphicsPlatform.CreateMatrix();
        //        //rotate around axis
        //        //1. move to its center
        //        //2. rotate
        //        //3. translate back
        //        matrix.Translate(this.CenterX, this.CenterY);
        //        matrix.Rotate(this.Angle);
        //        matrix.Translate(-this.CenterX, -this.CenterY);
        //        return matrix;
        //    }
        //}

        //public override string WriteToString()
        //{
        //    return string.Format(CultureInfo.InvariantCulture, "rotate({0}, {1}, {2})", this.Angle, this.CenterX, this.CenterY);
        //}

        public SvgRotate(float angle)
        {
            this.Angle = angle;
        }

        public SvgRotate(float angle, float centerX, float centerY)
            : this(angle)
        {
            this.CenterX = centerX;
            this.CenterY = centerY;
        }

        //public override object Clone()
        //{
        //    return new SvgRotate(this.Angle, this.CenterX, this.CenterY);
        //}
    }
}