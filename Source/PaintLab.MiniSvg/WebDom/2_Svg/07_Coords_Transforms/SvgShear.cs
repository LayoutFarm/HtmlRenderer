//from github.com/vvvv/svg 
//license : Microsoft Public License (MS-PL) 

namespace LayoutFarm.Svg.Transforms
{
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
}