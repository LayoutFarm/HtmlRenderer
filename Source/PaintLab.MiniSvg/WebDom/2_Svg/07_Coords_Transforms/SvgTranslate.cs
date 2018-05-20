//from github.com/vvvv/svg 
//license : Microsoft Public License (MS-PL) 

namespace LayoutFarm.Svg.Transforms
{
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
}