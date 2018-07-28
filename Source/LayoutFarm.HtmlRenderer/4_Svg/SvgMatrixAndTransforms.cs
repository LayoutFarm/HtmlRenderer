////from github.com/vvvv/svg 
////license : Microsoft Public License (MS-PL) 


//namespace LayoutFarm.Svg.Transforms
//{
//    /// <summary>
//    /// The class which applies custom transform to this Matrix
//    /// </summary>
//    public sealed class SvgTransformMatrix : SvgTransform
//    {
//        float[] _elements;
//        public float[] Elements
//        {
//            get { return this._elements; }
//            set { this._elements = value; }
//        }
//        public SvgTransformMatrix(float[] elements)
//        {
//            this._elements = elements;
//        }
//    }

//    public abstract class SvgTransform
//    {

//    }

//    public sealed class SvgTranslate : SvgTransform
//    {
//        public float X
//        {
//            get;
//            set;
//        }

//        public float Y
//        {
//            get;
//            set;
//        }
//        public SvgTranslate(float x, float y)
//        {
//            this.X = x;
//            this.Y = y;
//        }

//        public SvgTranslate(float x)
//            : this(x, 0.0f)
//        {
//        }
//    }
//    /// <summary>
//    /// The class which applies the specified skew vector to this Matrix.
//    /// </summary>
//    public sealed class SvgSkew : SvgTransform
//    {
//        public float AngleX { get; set; }
//        public float AngleY { get; set; }
//        public SvgSkew(float x, float y)
//        {
//            AngleX = x;
//            AngleY = y;
//        }
//    }

//    /// <summary>
//    /// The class which applies the specified shear vector to this Matrix.
//    /// </summary>
//    public sealed class SvgShear : SvgTransform
//    {
//        public float X
//        {
//            get;
//            set;
//        }

//        public float Y
//        {
//            get;
//            set;
//        }
//        public SvgShear(float x) : this(x, x) { }

//        public SvgShear(float x, float y)
//        {
//            this.X = x;
//            this.Y = y;
//        }
//    }

//    public sealed class SvgScale : SvgTransform
//    {
//        public float X
//        {
//            get;
//            set;
//        }

//        public float Y
//        {
//            get;
//            set;
//        }
//        public SvgScale(float x) : this(x, x) { }

//        public SvgScale(float x, float y)
//        {
//            this.X = x;
//            this.Y = y;
//        }

//    }

//    public sealed class SvgRotate : SvgTransform
//    {
//        public float Angle
//        {
//            get;
//            set;
//        }
//        public float CenterX
//        {
//            get;
//            set;
//        }
//        public float CenterY
//        {
//            get;
//            set;
//        }
//        public SvgRotate(float angle)
//        {
//            this.Angle = angle;
//        }

//        public SvgRotate(float angle, float centerX, float centerY)
//            : this(angle)
//        {
//            this.CenterX = centerX;
//            this.CenterY = centerY;
//        }
//    }
//}