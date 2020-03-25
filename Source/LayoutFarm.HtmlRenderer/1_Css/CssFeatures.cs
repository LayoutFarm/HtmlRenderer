//BSD, 2014-present, WinterDev 


using PixelFarm.Drawing;
namespace LayoutFarm.Css
{
    abstract class CssFeatureBase
    {
#if DEBUG
        static int dbugIdTotal = 0;
        int __aa_dbugId = dbugIdTotal++;
#endif

        protected readonly object _owner;
        bool _freezed;
        public CssFeatureBase(object owner)
        {
            _owner = owner;
        }
        public object Owner => _owner;
        public bool IsFreezed => _freezed;
        public void Freeze()
        {
            _freezed = true;
        }
        public void DeFreeze() { _freezed = false; }


        protected bool Assignable()
        {
            return !_freezed;
        }
    }

    class CssBorderFeature : CssFeatureBase
    {
        CssLength _leftWidth, _topWidth, _rightWidth, _bottomWidth, _borderSpacingV, _borderSpacingH;
        CssBorderStyle _leftStyle, _topStyle, _rightStyle, _bottomStyle;
        Color _leftColor, _topColor, _rightColor, _bottomColor;
        CssBorderCollapse _borderCollapse;
        public static readonly CssBorderFeature Default = new CssBorderFeature(null);
        static CssBorderFeature()
        {
            Default.Freeze();
        }

        public CssBorderFeature(object owner)
            : base(owner)
        {
            this.LeftWidth =
                this.TopWidth =
                this.RightWidth =
                this.BottomWidth = CssLength.Medium;
            //---------------------------------------------
            this.LeftStyle =
                this.TopStyle =
                this.RightStyle =
                this.BottomStyle = CssBorderStyle.None;
            //---------------------------------------------
            this.LeftColor =
                this.TopColor =
                this.RightColor =
                this.BottomColor = Color.Black;
            //---------------------------------------------
            this.BorderCollapse = CssBorderCollapse.Separate;
            this.BorderSpacingV = CssLength.ZeroNoUnit;
            this.BorderSpacingH = CssLength.ZeroNoUnit;
        }
        private CssBorderFeature(object owner, CssBorderFeature inheritFrom)
            : base(owner)
        {
            this.LeftWidth = inheritFrom.LeftWidth;
            this.TopWidth = inheritFrom.TopWidth;
            this.RightWidth = inheritFrom.RightWidth;
            this.BottomWidth = inheritFrom.BottomWidth;
            //---------------------------------------------------------
            this.LeftStyle = inheritFrom.LeftStyle;
            this.TopStyle = inheritFrom.TopStyle;
            this.RightStyle = inheritFrom.RightStyle;
            this.BottomStyle = inheritFrom.BottomStyle;
            //---------------------------------------------------------
            this.LeftColor = inheritFrom.LeftColor;
            this.TopColor = inheritFrom.TopColor;
            this.RightColor = inheritFrom.RightColor;
            this.BottomColor = inheritFrom.BottomColor;
            //---------------------------------------------------------
            this.BorderCollapse = inheritFrom.BorderCollapse;
            this.BorderSpacingH = inheritFrom.BorderSpacingH;
            this.BorderSpacingV = inheritFrom.BorderSpacingV;
            //---------------------------------------------------------
        }
        public CssLength LeftWidth
        {
            get => _leftWidth;
            set { if (Assignable()) _leftWidth = value; }
        }
        public CssLength TopWidth
        {
            get => _topWidth;
            set { if (Assignable()) _topWidth = value; }
        }
        public CssLength RightWidth
        {
            get => _rightWidth;
            set { if (Assignable()) _rightWidth = value; }
        }
        public CssLength BottomWidth
        {
            get => _bottomWidth;
            set { if (Assignable()) _bottomWidth = value; }
        }

        public CssBorderStyle LeftStyle
        {
            get => _leftStyle;
            set { if (Assignable()) _leftStyle = value; }
        }
        public CssBorderStyle TopStyle
        {
            get => _topStyle;
            set { if (Assignable()) _topStyle = value; }
        }
        public CssBorderStyle RightStyle
        {
            get => _rightStyle;
            set { if (Assignable()) _rightStyle = value; }
        }
        public CssBorderStyle BottomStyle
        {
            get => _bottomStyle;
            set { if (Assignable()) _bottomStyle = value; }
        }

        public Color LeftColor
        {
            get => _leftColor;
            set { if (Assignable()) _leftColor = value; }
        }
        public Color TopColor
        {
            get => _topColor;
            set { if (Assignable()) _topColor = value; }
        }
        public Color RightColor
        {
            get => _rightColor;
            set { if (Assignable()) _rightColor = value; }
        }
        public Color BottomColor
        {
            get => _bottomColor;
            set { if (Assignable()) _bottomColor = value; }
        }

        public CssBorderCollapse BorderCollapse
        {
            get => _borderCollapse;
            set { if (Assignable()) _borderCollapse = value; }
        }

        public CssLength BorderSpacingH
        {
            get => _borderSpacingH;
            set { if (Assignable()) _borderSpacingH = value; }
        }
        public CssLength BorderSpacingV
        {
            get => _borderSpacingV;
            set { if (Assignable()) _borderSpacingV = value; }
        }

        public CssBorderFeature GetMyOwnVersion(object checkOwner)
        {
            if (_owner == checkOwner)
            {
                return this;
            }
            else
            {
                //create new clone
                return new CssBorderFeature(checkOwner, this);
            }
        }



#if DEBUG
        public static bool dbugIsEq(dbugPropCheckReport rep, CssBorderFeature prop1, CssBorderFeature prop2)
        {
            int inCount = rep.Count;
            rep.Check("LeftWidth", CssLength.IsEq(prop1.LeftWidth, prop2.LeftWidth));
            rep.Check("TopWidth", CssLength.IsEq(prop1.TopWidth, prop2.TopWidth));
            rep.Check("RightWidth", CssLength.IsEq(prop1.RightWidth, prop2.RightWidth));
            rep.Check("BottomWidth", CssLength.IsEq(prop1.BottomWidth, prop2.BottomWidth));
            rep.Check("LeftStyle", prop1.LeftStyle == prop2.LeftStyle);
            rep.Check("TopStyle", prop1.TopStyle == prop2.TopStyle);
            rep.Check("RightStyle", prop1.RightStyle == prop2.RightStyle);
            rep.Check("BottomStyle", prop1.BottomStyle == prop2.BottomStyle);
            rep.Check("LeftColor", prop1.LeftColor == prop2.LeftColor);
            rep.Check("TopColor", prop1.TopColor == prop2.TopColor);
            rep.Check("RightColor", prop1.RightColor == prop2.RightColor);
            rep.Check("BottomColor", prop1.BottomColor == prop2.BottomColor);
            rep.Check("BorderCollapse", prop1.BorderCollapse == prop2.BorderCollapse);
            rep.Check("BorderSpacingH", CssLength.IsEq(prop1.BorderSpacingH, prop2.BorderSpacingH));
            rep.Check("BorderSpacingV", CssLength.IsEq(prop1.BorderSpacingV, prop2.BorderSpacingV));
            return inCount == rep.Count;
        }
#endif
    }


    class CssMarginFeature : CssFeatureBase
    {
        CssLength _left, _top, _right, _bottom;
        public static readonly CssMarginFeature Default = new CssMarginFeature(null);
        static CssMarginFeature()
        {
            Default.Freeze();
        }
        public CssMarginFeature(object owner)
            : base(owner)
        {
            this.Left =
                this.Top =
                this.Right =
                this.Bottom = CssLength.ZeroPx;
        }
        private CssMarginFeature(object newOwner, CssMarginFeature inheritFrom)
            : base(newOwner)
        {
            this.Left = inheritFrom.Left;
            this.Top = inheritFrom.Top;
            this.Right = inheritFrom.Right;
            this.Bottom = inheritFrom.Bottom;
        }

        public CssLength Left
        {
            get => _left;
            set { if (Assignable()) _left = value; }
        }
        public CssLength Top
        {
            get => _top;
            set { if (Assignable()) _top = value; }
        }
        public CssLength Right
        {
            get => _right;
            set { if (Assignable()) _right = value; }
        }

        public CssLength Bottom
        {
            get => _bottom;
            set { if (Assignable()) _bottom = value; }
        }

        public CssMarginFeature GetMyOwnVersion(object checkOwner)
        {
            if (_owner == checkOwner)
            {
                return this;
            }
            else
            {
                return new CssMarginFeature(checkOwner, this);
            }
        }



#if DEBUG
        public static bool dbugIsEq(dbugPropCheckReport rep, CssMarginFeature prop1, CssMarginFeature prop2)
        {
            int inCount = rep.Count;
            rep.Check("Left", CssLength.IsEq(prop1.Left, prop2.Left));
            rep.Check("Top", CssLength.IsEq(prop1.Top, prop2.Top));
            rep.Check("Right", CssLength.IsEq(prop1.Right, prop2.Right));
            rep.Check("Bottom", CssLength.IsEq(prop1.Bottom, prop2.Bottom));
            return inCount == rep.Count;
        }
#endif
    }
    class CssPaddingFeature : CssFeatureBase
    {
        CssLength _left, _top, _right, _bottom;
        public static readonly CssPaddingFeature Default = new CssPaddingFeature(null);
        static CssPaddingFeature()
        {
            Default.Freeze();
        }
        public CssPaddingFeature(object owner)
            : base(owner)
        {
            this.Left =
                   Top =
                   Right =
                   Bottom = CssLength.ZeroNoUnit;
        }
        private CssPaddingFeature(object newOwner, CssPaddingFeature inheritFrom)
            : base(newOwner)
        {
            this.Left = inheritFrom.Left;
            this.Left = inheritFrom.Left;
            this.Right = inheritFrom.Right;
            this.Bottom = inheritFrom.Bottom;
        }
        public CssLength Left
        {
            get => _left;
            set { if (Assignable()) _left = value; }
        }
        public CssLength Top
        {
            get => _top;
            set { if (Assignable()) _top = value; }
        }
        public CssLength Right
        {
            get => _right;
            set { if (Assignable()) _right = value; }
        }

        public CssLength Bottom
        {
            get => _bottom;
            set { if (Assignable()) _bottom = value; }
        }

        public CssPaddingFeature GetMyOwnVersion(object checkOwner)
        {
            if (_owner == checkOwner)
            {
                return this;
            }
            else
            {
                return new CssPaddingFeature(checkOwner, this);
            }
        }


#if DEBUG
        public static bool dbugIsEq(dbugPropCheckReport rep, CssPaddingFeature prop1, CssPaddingFeature prop2)
        {
            int inCount = rep.Count;
            rep.Check("Left", CssLength.IsEq(prop1.Left, prop2.Left));
            rep.Check("Top", CssLength.IsEq(prop1.Top, prop2.Top));
            rep.Check("Right", CssLength.IsEq(prop1.Right, prop2.Right));
            rep.Check("Bottom", CssLength.IsEq(prop1.Bottom, prop2.Bottom));
            return inCount == rep.Count;
        }
#endif
    }


    class CssListFeature : CssFeatureBase
    {
        CssListStyleType _listStyleType;
        string _listStyleImage;
        CssListStylePosition _listStylePosition;
        string _listStyle;
        public static readonly CssListFeature Default = new CssListFeature(null);
        static CssListFeature()
        {
            Default.Freeze();
        }

        public CssListFeature(object owner)
            : base(owner)
        {
            ListStyleType = CssListStyleType.Disc;
            ListStyleImage = string.Empty;
            ListStylePosition = CssListStylePosition.Outside;
            ListStyle = string.Empty;
        }
        private CssListFeature(object owner, CssListFeature inheritFrom)
            : base(owner)
        {
            ListStyleType = inheritFrom.ListStyleType;
            ListStyleImage = inheritFrom.ListStyleImage;
            ListStylePosition = inheritFrom.ListStylePosition;
            ListStyle = inheritFrom.ListStyle;
        }
        public CssListFeature GetMyOwnVersion(object checkOwner)
        {
            if (_owner == checkOwner)
            {
                return this;
            }
            else
            {
                return new CssListFeature(checkOwner, this);
            }
        }

        public CssListStyleType ListStyleType
        {
            get => _listStyleType;

            set
            {
                if (Assignable()) _listStyleType = value;
            }
        }
        public string ListStyleImage
        {
            get => _listStyleImage;

            set
            {
                if (Assignable()) _listStyleImage = value;
            }
        }
        public CssListStylePosition ListStylePosition
        {
            get => _listStylePosition;

            set
            {
                if (Assignable()) _listStylePosition = value;
            }
        }
        public string ListStyle
        {
            get => _listStyle;

            set
            {
                if (Assignable()) _listStyle = value;
            }
        }



#if DEBUG
        public static bool dbugIsEq(dbugPropCheckReport rep, CssListFeature prop1, CssListFeature prop2)
        {
            int inCount = rep.Count;
            rep.Check("ListStyleType", prop1.ListStyleType == prop2.ListStyleType);
            rep.Check("ListStyleType", prop1.ListStyleImage == prop2.ListStyleImage);
            rep.Check("ListStyleType", prop1.ListStylePosition == prop2.ListStylePosition);
            rep.Check("ListStyleType", prop1.ListStyle == prop2.ListStyle);
            return inCount == rep.Count;
        }
#endif
    }

    class CssCornerFeature : CssFeatureBase
    {
        CssLength _ne, _nw, _se, _sw;
        public static readonly CssCornerFeature Default = new CssCornerFeature(null);
        static CssCornerFeature()
        {
            Default.Freeze();
        }
        public CssCornerFeature(object owner)
            : base(owner)
        {
            this.NERadius =
                NWRadius =
                SERadius =
                SWRadius = CssLength.ZeroNoUnit;
        }
        private CssCornerFeature(object owner, CssCornerFeature inheritFrom)
            : base(owner)
        {
            this.NERadius = inheritFrom.NERadius;
            this.NWRadius = inheritFrom.NWRadius;
            this.SERadius = inheritFrom.SERadius;
            this.SWRadius = inheritFrom.SWRadius;
        }
        public CssCornerFeature GetMyOwnVersion(object checkOwner)
        {
            if (_owner == checkOwner)
            {
                return this;
            }
            else
            {
                return new CssCornerFeature(_owner, this);
            }
        }

        public CssLength NERadius
        {
            get => _ne;
            set { if (Assignable()) _ne = value; }
        }
        public CssLength NWRadius
        {
            get => _nw;
            set { if (Assignable()) _nw = value; }
        }
        public CssLength SERadius
        {
            get => _se;
            set { if (Assignable()) _se = value; }
        }
        public CssLength SWRadius
        {
            get => _sw;
            set { if (Assignable()) _sw = value; }
        }


#if DEBUG
        public static bool dbugIsEq(dbugPropCheckReport rep, CssCornerFeature prop1, CssCornerFeature prop2)
        {
            int inCount = rep.Count;
            rep.Check("NERadius", CssLength.IsEq(prop1.NERadius, prop2.NERadius));
            rep.Check("NWRadius", CssLength.IsEq(prop1.NWRadius, prop2.NWRadius));
            rep.Check("SERadius", CssLength.IsEq(prop1.SERadius, prop2.SERadius));
            rep.Check("SWRadius", CssLength.IsEq(prop1.SWRadius, prop2.SWRadius));
            return inCount == rep.Count;
        }
#endif
    }

    class CssFontFeature : CssFeatureBase
    {
        CssLength _fontSize;
        CssFontStyle _fontStyle;
        CssFontWeight _fontWeight;
        CssFontVariant _fontVariant;
        string _fontFam;
        public static readonly CssFontFeature Default = new CssFontFeature(null);
        static CssFontFeature()
        {
            Default.Freeze();
        }

        public CssFontFeature(object owner)
            : base(owner)
        {
            FontFamily = FontDefaultConfig.DEFAULT_FONT_NAME;
            FontSize = CssLength.FontSizeMedium;
            FontStyle = CssFontStyle.Normal;
            FontVariant = CssFontVariant.Normal;
            FontWeight = CssFontWeight.Normal;
        }
        private CssFontFeature(object owner, CssFontFeature inheritFrom)
            : base(owner)
        {
            this.FontFamily = inheritFrom.FontFamily;
            this.FontSize = inheritFrom.FontSize;
            this.FontStyle = inheritFrom.FontStyle;
            this.FontVariant = inheritFrom.FontVariant;
            this.FontWeight = inheritFrom.FontWeight;
        }

        public string FontFamily
        {
            get => _fontFam;
            set { if (Assignable()) _fontFam = value; }
        }

        public CssLength FontSize
        {
            get => _fontSize;
            set
            {
                if (Assignable()) _fontSize = value;
            }
        }
        public CssFontStyle FontStyle
        {
            get => _fontStyle;
            set
            {
                if (Assignable()) _fontStyle = value;
            }
        }
        public CssFontVariant FontVariant
        {
            get => _fontVariant;
            set
            {
                if (Assignable()) _fontVariant = value;
            }
        }
        public CssFontWeight FontWeight
        {
            get => _fontWeight;
            set
            {
                if (Assignable()) _fontWeight = value;
            }
        }


        //------------------------------------------------------------
        public CssFontFeature GetMyOwnVersion(object checkOwner)
        {
            if (_owner == checkOwner)
            {
                return this;
            }
            else
            {
                return new CssFontFeature(checkOwner, this);
            }
        }



#if DEBUG
        public static bool dbugIsEq(dbugPropCheckReport rep, CssFontFeature prop1, CssFontFeature prop2)
        {
            if (prop1 == prop2)
            {
                return true;
            }

            //---------------- 
            int inCount = rep.Count;
            rep.Check("FontFamily", prop1.FontFamily == prop2.FontFamily);
            rep.Check("FontSize", CssLength.IsEq(prop1.FontSize, prop2.FontSize));
            rep.Check("FontStyle", prop1.FontStyle == prop2.FontStyle);
            rep.Check("FontVariant", prop1.FontVariant == prop2.FontVariant);
            rep.Check("FontWeight", prop1.FontWeight == prop2.FontWeight);
            return inCount == rep.Count;
        }
#endif

    }

    public enum CssCursorName
    {
        Default,
        Pointer,
        Hand,
        IBeam,
    }
    class CssBackgroundFeature : CssFeatureBase
    {

        Color _bgColor, _bgGradient;
        float _bgGradientAngle;
        ImageBinder _imgBinder;
        string _bgPosition;
        CssLength _bgPosX, _bgPosY;
        CssBackgroundRepeat _bgRepeat;
        CssCursorName _cursorName;

        static CssBackgroundFeature()
        {
            Default.Freeze();
        }
        public CssBackgroundFeature(object owner)
            : base(owner)
        {
            this.BackgroundColor = Color.Transparent; //"transparent";
            this.BackgroundGradient = Color.Transparent;// "none";
            this.BackgroundGradientAngle = 90.0f;
            this.BackgroundImageBinder = ImageBinder.NoImage;
            this.BackgroundPosX = new CssLength(0, CssUnitOrNames.Percent);
            this.BackgroundPosY = new CssLength(0, CssUnitOrNames.Percent);
            this.BackgroundRepeat = CssBackgroundRepeat.Repeat;
            this.CursorName = CssCursorName.Default;
        }
        private CssBackgroundFeature(object owner, CssBackgroundFeature inheritFrom)
            : base(owner)
        {
            BackgroundColor = inheritFrom.BackgroundColor;
            BackgroundGradient = inheritFrom.BackgroundGradient;
            BackgroundGradientAngle = inheritFrom.BackgroundGradientAngle;
            BackgroundImageBinder = inheritFrom.BackgroundImageBinder;
            BackgroundPosX = inheritFrom.BackgroundPosX;
            BackgroundPosY = inheritFrom.BackgroundPosY;
            BackgroundRepeat = inheritFrom.BackgroundRepeat;
            CursorName = inheritFrom.CursorName;
        }

        public CssBackgroundFeature GetMyOwnVersion(object checkOwner)
        {
            if (_owner == checkOwner)
            {
                return this;
            }
            else
            {
                return new CssBackgroundFeature(checkOwner, this);
            }
        }
        //---------------------------------
        public Color BackgroundColor
        {
            get => _bgColor;
            set { if (Assignable()) _bgColor = value; }
        }
        public Color BackgroundGradient
        {
            get => _bgGradient;
            set { if (Assignable()) _bgGradient = value; }
        }
        public float BackgroundGradientAngle
        {
            get => _bgGradientAngle;
            set { if (Assignable()) _bgGradientAngle = value; }
        }

        public ImageBinder BackgroundImageBinder
        {
            get => _imgBinder;
            set { if (Assignable()) _imgBinder = value; }
        }
        public string BackgroundPosition
        {
            get => _bgPosition;
            set { if (Assignable()) _bgPosition = value; }
        }

        public CssLength BackgroundPosX
        {
            get => _bgPosX;
            set { if (Assignable()) _bgPosX = value; }
        }
        public CssLength BackgroundPosY
        {
            get => _bgPosY;
            set { if (Assignable()) _bgPosY = value; }
        }
        public CssBackgroundRepeat BackgroundRepeat
        {
            get => _bgRepeat;
            set { if (Assignable()) _bgRepeat = value; }
        }


        public CssCursorName CursorName
        {
            get => _cursorName;
            set { if (Assignable()) _cursorName = value; }
        }
        //---------------------------------
        public static readonly CssBackgroundFeature Default = new CssBackgroundFeature(null);
#if DEBUG
        public static bool dbugIsEq(dbugPropCheckReport rep, CssBackgroundFeature prop1, CssBackgroundFeature prop2)
        {
            if (prop1 == prop2)
            {
                return true;
            }
            //---------------- 
            int inMsgCount = rep.Count;
            rep.Check("BackgroundColor", prop1.BackgroundColor == prop2.BackgroundColor);
            rep.Check("BackgroundGradient", prop1.BackgroundGradient == prop2.BackgroundGradient);
            rep.Check("BackgroundGradientAngle", prop1.BackgroundGradientAngle == prop2.BackgroundGradientAngle);
            if (!(prop1.BackgroundImageBinder == null && prop2.BackgroundImageBinder == null))
            {
                if (prop1.BackgroundImageBinder != prop2.BackgroundImageBinder)
                {
                    if (prop1.BackgroundImageBinder.ImageSource != prop2.BackgroundImageBinder.ImageSource)
                    {
                        rep.Check("BackgroundImageBinder", prop1.BackgroundImageBinder == prop2.BackgroundImageBinder);
                    }
                }
            }
            rep.Check("BackgroundPosition", prop1.BackgroundPosition == prop2.BackgroundPosition);
            rep.Check("BackgroundPosX", CssLength.IsEq(prop1.BackgroundPosX, prop2.BackgroundPosX));
            rep.Check("BackgroundPosY", CssLength.IsEq(prop1.BackgroundPosY, prop2.BackgroundPosY));
            rep.Check("CssBackgroundRepeat", prop1.BackgroundRepeat == prop2.BackgroundRepeat);
            return inMsgCount == rep.Count;
        }
#endif
    }

    class CssBoxShadowFeature : CssFeatureBase
    {
        Color _shadowColor;
        CssLength _hOffset, _vOffset, _blurRadius, _spreadDistance;
        bool _inset;
        public static readonly CssBoxShadowFeature Default = new CssBoxShadowFeature(null);
        static CssBoxShadowFeature()
        {
            Default.Freeze();
        }
        public CssBoxShadowFeature(object owner)
            : base(owner)
        {
            this.HOffset =
               this.VOffset =
               this.BlurRadius =
               this.SpreadDistance = CssLength.ZeroNoUnit;
            this.ShadowColor = Color.FromArgb(220, KnownColors.Gray);
        }
        private CssBoxShadowFeature(object newOwner, CssBoxShadowFeature inheritFrom)
            : base(newOwner)
        {
            this.HOffset = inheritFrom.HOffset;
            this.VOffset = inheritFrom.VOffset;
            this.BlurRadius = inheritFrom.BlurRadius;
            this.ShadowColor = inheritFrom.ShadowColor;
            this.SpreadDistance = inheritFrom.SpreadDistance;
            this.Inset = inheritFrom.Inset;
        }
        public CssLength HOffset
        {
            get => _hOffset;
            set { if (Assignable()) _hOffset = value; }
        }
        public CssLength VOffset
        {
            get => _vOffset;
            set { if (Assignable()) _vOffset = value; }
        }
        public CssLength BlurRadius
        {
            get => _blurRadius;
            set { if (Assignable()) _blurRadius = value; }
        }
        public CssLength SpreadDistance
        {
            get => _spreadDistance;
            set { if (Assignable()) _spreadDistance = value; }
        }
        public Color ShadowColor
        {
            get => _shadowColor;
            set { if (Assignable()) _shadowColor = value; }
        }
        public bool Inset
        {
            get => _inset;
            set { if (Assignable()) _inset = value; }
        }

        public CssBoxShadowFeature GetMyOwnVersion(object checkOwner)
        {
            if (_owner == checkOwner)
            {
                return this;
            }
            else
            {
                return new CssBoxShadowFeature(checkOwner, this);
            }
        }


#if DEBUG
        public static bool dbugIsEq(dbugPropCheckReport rep, CssPaddingFeature prop1, CssPaddingFeature prop2)
        {
            int inCount = rep.Count;
            rep.Check("Left", CssLength.IsEq(prop1.Left, prop2.Left));
            rep.Check("Top", CssLength.IsEq(prop1.Top, prop2.Top));
            rep.Check("Right", CssLength.IsEq(prop1.Right, prop2.Right));
            rep.Check("Bottom", CssLength.IsEq(prop1.Bottom, prop2.Bottom));
            return inCount == rep.Count;
        }
#endif
    }


    class CssFlexFeature : CssFeatureBase
    {
        public static readonly CssFlexFeature Default = new CssFlexFeature(null);
        FlexFlowDirection _flexFlowDirection;
        FlexWrap _flexLineWrapping;
        int _flexOrder;
        int _flexGrow; //flex grow factor
        int _flexShrink = 1; //flex shrink factor,init =1
        CssLength _flexBasis = CssLength.MainSize; //initial value
        FlexJustifyContent _justifyContent;//axis alingment
        FlexAlignItem _alignItem;//cross-axis alignment
        FlexAlignSelf _alignSelf; //cross-axis alignment
        FlexAlignContent _alignContent;//packing flex lines
        //----
        // temp expriment extension
        int _flexExtensionNum;
        int _flexExtensionPartNo;
        static CssFlexFeature()
        {
            Default.Freeze();
        }

        public CssFlexFeature(object owner)
            : base(owner)
        {
        }
        private CssFlexFeature(object newOwner, CssFlexFeature inheritFrom)
            : base(newOwner)
        {
        }
        public CssFlexFeature GetMyOwnVersion(object checkOwner)
        {
            if (_owner == checkOwner)
            {
                return this;
            }
            else
            {
                return new CssFlexFeature(checkOwner, this);
            }
        }

        public FlexFlowDirection FlowDirection
        {
            get => _flexFlowDirection;
            set { if (Assignable()) _flexFlowDirection = value; }
        }
        public FlexWrap FlexWrap
        {
            get => _flexLineWrapping;
            set { if (Assignable()) _flexLineWrapping = value; }
        }

        public int FlexOrder
        {
            get => _flexOrder;
            set { if (Assignable()) _flexOrder = value; }
        }

        public int FlexGrow
        {
            get => _flexGrow;
            set { if (Assignable()) _flexGrow = value; }
        }
        public int FlexShrink
        {
            get => _flexShrink;
            set { if (Assignable()) _flexShrink = value; }
        }
        public CssLength FlexBasis
        {
            get => _flexBasis;
            set { if (Assignable()) _flexBasis = value; }
        }

        public FlexJustifyContent JustifyContent
        {
            get => _justifyContent;
            set { if (Assignable()) _justifyContent = value; }
        }
        public FlexAlignItem AlignItem
        {
            get => _alignItem;
            set { if (Assignable()) _alignItem = value; }
        }
        public FlexAlignSelf AlignSelf
        {
            get => _alignSelf;
            set { if (Assignable()) _alignSelf = value; }
        }
        public FlexAlignContent AlignContent
        {
            get => _alignContent;
            set { if (Assignable()) _alignContent = value; }
        }

        public int FlexExtensionNum
        {
            get => _flexExtensionNum;
            set { if (Assignable()) _flexExtensionNum = value; }
        }
        /// <summary>
        /// extension part number
        /// </summary>
        public int FlexExtensionPart
        {
            get => _flexExtensionPartNo;
            set { if (Assignable()) _flexExtensionPartNo = value; }
        }
    }
}