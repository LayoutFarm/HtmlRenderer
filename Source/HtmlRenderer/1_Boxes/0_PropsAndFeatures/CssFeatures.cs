//BSD, 2014, WinterFarm

using System;
using System.Collections.Generic;
using System.Drawing;

namespace HtmlRenderer.Dom
{
    abstract class CssFeatureBase
    {
        protected readonly object owner;
        bool freezed;
        public CssFeatureBase(object owner)
        {
            this.owner = owner;
        }
        public object Owner { get { return this.owner; } }
        public bool IsFreezed { get { return this.freezed; } }
        public void Freeze() { this.freezed = true; }
        public void DeFreeze() { this.freezed = false; }

        protected bool AttempAssignValue()
        {
            if (this.freezed)
            {
            }
            return !this.freezed;
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
            get { return this._leftWidth; }
            set { if (AttempAssignValue()) this._leftWidth = value; }
        }
        public CssLength TopWidth
        {
            get { return this._topWidth; }
            set { if (AttempAssignValue()) this._topWidth = value; }
        }
        public CssLength RightWidth
        {
            get { return this._rightWidth; }
            set { if (AttempAssignValue()) this._rightWidth = value; }
        }
        public CssLength BottomWidth
        {
            get { return this._bottomWidth; }
            set { if (AttempAssignValue()) this._bottomWidth = value; }
        }



        public CssBorderStyle LeftStyle
        {
            get { return this._leftStyle; }
            set { if (AttempAssignValue())  this._leftStyle = value; }

        }
        public CssBorderStyle TopStyle
        {
            get { return this._topStyle; }
            set { if (AttempAssignValue())  this._topStyle = value; }
        }
        public CssBorderStyle RightStyle
        {
            get { return this._rightStyle; }
            set { if (AttempAssignValue())  this._rightStyle = value; }
        }
        public CssBorderStyle BottomStyle
        {
            get { return this._bottomStyle; }
            set { if (AttempAssignValue())this._bottomStyle = value; }
        }

        public Color LeftColor
        {
            get { return this._leftColor; }
            set { if (AttempAssignValue())this._leftColor = value; }
        }
        public Color TopColor
        {
            get { return this._topColor; }
            set { if (AttempAssignValue())this._topColor = value; }
        }
        public Color RightColor
        {
            get { return this._rightColor; }
            set { if (AttempAssignValue())this._rightColor = value; }
        }
        public Color BottomColor
        {
            get { return this._bottomColor; }
            set { if (AttempAssignValue())this._bottomColor = value; }
        }

        public CssBorderCollapse BorderCollapse
        {
            get { return this._borderCollapse; }
            set { if (AttempAssignValue())this._borderCollapse = value; }
        }

        public CssLength BorderSpacingH
        {
            get { return this._borderSpacingH; }
            set { if (AttempAssignValue()) this._borderSpacingH = value; }
        }
        public CssLength BorderSpacingV
        {
            get { return this._borderSpacingV; }
            set { if (AttempAssignValue()) this._borderSpacingV = value; }
        }

        public CssBorderFeature GetMyOwnVersion(object checkOwner)
        {
            if (this.owner == checkOwner)
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
            get { return this._left; }
            set { if (AttempAssignValue()) this._left = value; }
        }
        public CssLength Top
        {
            get { return this._top; }
            set { if (AttempAssignValue()) this._top = value; }
        }
        public CssLength Right
        {
            get { return this._right; }
            set { if (AttempAssignValue()) this._right = value; }
        }

        public CssLength Bottom
        {
            get { return this._bottom; }
            set { if (AttempAssignValue()) this._bottom = value; }
        }

        public CssMarginFeature GetMyOwnVersion(object checkOwner)
        {
            if (this.owner == checkOwner)
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
            get { return this._left; }
            set { if (AttempAssignValue()) this._left = value; }
        }
        public CssLength Top
        {
            get { return this._top; }
            set { if (AttempAssignValue()) this._top = value; }
        }
        public CssLength Right
        {
            get { return this._right; }
            set { if (AttempAssignValue()) this._right = value; }
        }

        public CssLength Bottom
        {
            get { return this._bottom; }
            set { if (AttempAssignValue()) this._bottom = value; }
        }

        public CssPaddingFeature GetMyOwnVersion(object checkOwner)
        {
            if (this.owner == checkOwner)
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
            if (this.owner == checkOwner)
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
            get
            {
                return this._listStyleType;
            }
            set
            {
                if (AttempAssignValue()) this._listStyleType = value;
            }
        }
        public string ListStyleImage
        {
            get
            {
                return this._listStyleImage;
            }
            set
            {
                if (AttempAssignValue()) this._listStyleImage = value;
            }
        }
        public CssListStylePosition ListStylePosition
        {
            get
            {
                return this._listStylePosition;
            }
            set
            {
                if (AttempAssignValue()) this._listStylePosition = value;
            }
        }
        public string ListStyle
        {
            get
            {
                return this._listStyle;
            }
            set
            {
                if (AttempAssignValue()) this._listStyle = value;
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
            if (this.owner == checkOwner)
            {
                return this;
            }
            else
            {
                return new CssCornerFeature(owner, this);
            }
        }
        
        public CssLength NERadius
        {
            get { return this._ne; }
            set { if (AttempAssignValue()) this._ne = value; }
        }
        public CssLength NWRadius
        {
            get { return this._nw; }
            set { if (AttempAssignValue()) this._nw = value; }
        }
        public CssLength SERadius
        {
            get { return this._se; }
            set { if (AttempAssignValue()) this._se = value; }
        }
        public CssLength SWRadius
        {
            get { return this._sw; }
            set { if (AttempAssignValue()) this._sw = value; }
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

            FontFamily = ConstConfig.DEFAULT_FONT_NAME;
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
            get { return this._fontFam; }
            set { if (AttempAssignValue()) this._fontFam = value; }
        }

        public CssLength FontSize
        {
            get { return this._fontSize; }
            set
            {
                if (AttempAssignValue()) this._fontSize = value;
            }
        }
        public CssFontStyle FontStyle
        {
            get { return this._fontStyle; }
            set
            {
                if (AttempAssignValue()) this._fontStyle = value;
            }
        }
        public CssFontVariant FontVariant
        {
            get { return this._fontVariant; }
            set
            {
                if (AttempAssignValue()) this._fontVariant = value;
            }
        }
        public CssFontWeight FontWeight
        {
            get { return this._fontWeight; }
            set
            {
                if (AttempAssignValue()) this._fontWeight = value;
            }
        }


        //------------------------------------------------------------
        public CssFontFeature GetMyOwnVersion(object checkOwner)
        {
            if (this.owner == checkOwner)
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


    class CssBackgroundFeature : CssFeatureBase
    {
        Color _bgColor, _bgGradient;
        float _bgGradientAngle;
        ImageBinder _imgBinder;
        string _bgPosition;
        CssLength _bgPosX, _bgPosY;
        CssBackgroundRepeat _bgRepeat;


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
        }

        public CssBackgroundFeature GetMyOwnVersion(object checkOwner)
        {
            if (this.owner == checkOwner)
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
            get { return this._bgColor; }
            set { if (AttempAssignValue()) this._bgColor = value; }
        }
        public Color BackgroundGradient
        {
            get { return this._bgGradient; }
            set { if (AttempAssignValue()) this._bgGradient = value; }
        }
        public float BackgroundGradientAngle
        {
            get { return this._bgGradientAngle; }
            set { if (AttempAssignValue()) this._bgGradientAngle = value; }
        }

        public ImageBinder BackgroundImageBinder
        {
            get { return this._imgBinder; }
            set { if (AttempAssignValue()) this._imgBinder = value; }
        }
        public string BackgroundPosition
        {
            get { return this._bgPosition; }
            set { if (AttempAssignValue()) this._bgPosition = value; }
        }

        public CssLength BackgroundPosX
        {
            get { return this._bgPosX; }
            set { if (AttempAssignValue()) this._bgPosX = value; }
        }
        public CssLength BackgroundPosY
        {
            get { return this._bgPosY; }
            set { if (AttempAssignValue()) this._bgPosY = value; }
        }
        public CssBackgroundRepeat BackgroundRepeat
        {
            get { return this._bgRepeat; }
            set { if (AttempAssignValue()) this._bgRepeat = value; }
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


#if DEBUG
    public class dbugPropCheckReport
    {
        List<string> msgs = new List<string>();
        public void Check(string propName, bool testResult)
        {
            if (!testResult)
            {
                msgs.Add(propName);
            }
        }
        public int Count
        {
            get { return this.msgs.Count; }
        }
        public void ClearMsgs()
        {
            this.msgs.Clear();
        }
        public List<string> GetList()
        {
            return this.msgs;
        }
    }
#endif
}