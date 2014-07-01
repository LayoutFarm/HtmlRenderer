//BSD, 2014, WinterCore

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using HtmlRenderer.Entities;
using HtmlRenderer.Handlers;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{


    class CssBorderProp
    {

        object owner;
        public CssBorderProp(object owner)
        {
            this.owner = owner;
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
        private CssBorderProp(object owner, CssBorderProp inheritFrom)
        {
            this.owner = owner;
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


        public object Owner { get { return this.owner; } }

        public CssLength LeftWidth { get; set; }
        public CssLength TopWidth { get; set; }
        public CssLength RightWidth { get; set; }
        public CssLength BottomWidth { get; set; }

        public CssBorderStyle LeftStyle { get; set; }
        public CssBorderStyle TopStyle { get; set; }
        public CssBorderStyle RightStyle { get; set; }
        public CssBorderStyle BottomStyle { get; set; }

        public Color LeftColor { get; set; }
        public Color TopColor { get; set; }
        public Color RightColor { get; set; }
        public Color BottomColor { get; set; }

        public CssBorderCollapse BorderCollapse { get; set; }
        public CssLength BorderSpacingH { get; set; }
        public CssLength BorderSpacingV { get; set; }



        public CssBorderProp GetMyOwnVersion(object checkOwner)
        {
            if (this.owner == checkOwner)
            {
                return this;
            }
            else
            {
                //create new clone
                return new CssBorderProp(checkOwner, this);
            }
        }


        public static readonly CssBorderProp Default = new CssBorderProp(null);

#if DEBUG
        public static bool dbugIsEq(dbugPropCheckReport rep, CssBorderProp prop1, CssBorderProp prop2)
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

    class CssMarginProp
    {
        object owner;

        public CssMarginProp(object owner)
        {
            this.owner = owner;
            this.Left =
                this.Top =
                this.Right =
                this.Bottom = CssLength.ZeroPx;
        }
        private CssMarginProp(object newOwner, CssMarginProp inheritFrom)
        {
            this.owner = newOwner;

            this.Left = inheritFrom.Left;
            this.Top = inheritFrom.Top;
            this.Right = inheritFrom.Right;
            this.Bottom = inheritFrom.Bottom;
        }

        public CssLength Left { get; set; }
        public CssLength Top { get; set; }
        public CssLength Right { get; set; }
        public CssLength Bottom { get; set; }



        public CssMarginProp GetMyOwnVersion(object checkOwner)
        {
            if (this.owner == checkOwner)
            {
                return this;
            }
            else
            {
                return new CssMarginProp(checkOwner, this);
            }
        }
        public static readonly CssMarginProp Default = new CssMarginProp(null);


#if DEBUG
        public static bool dbugIsEq(dbugPropCheckReport rep, CssMarginProp prop1, CssMarginProp prop2)
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
    class CssPaddingProp
    {

        object owner;
        public CssPaddingProp(object owner)
        {
            this.owner = owner;
            this.Left =
                   Top =
                   Right =
                   Bottom = CssLength.ZeroNoUnit;
        }
        private CssPaddingProp(object newOwner, CssPaddingProp inheritFrom)
        {
            this.owner = newOwner;
            this.Left = inheritFrom.Left;
            this.Left = inheritFrom.Left;
            this.Right = inheritFrom.Right;
            this.Bottom = inheritFrom.Bottom;
        }
        public CssLength Left { get; set; }
        public CssLength Top { get; set; }
        public CssLength Right { get; set; }
        public CssLength Bottom { get; set; }

        public CssPaddingProp GetMyOwnVersion(object checkOwner)
        {
            if (this.owner == checkOwner)
            {
                return this;
            }
            else
            {
                return new CssPaddingProp(checkOwner, this);
            }
        }
        public static readonly CssPaddingProp Default = new CssPaddingProp(null);
        public object Owner
        {
            get { return this.owner; }
        }

#if DEBUG
        public static bool dbugIsEq(dbugPropCheckReport rep, CssPaddingProp prop1, CssPaddingProp prop2)
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


    class CssListProp
    {


        object owner;
        public CssListProp(object owner)
        {
            this.owner = owner;
            ListStyleType = CssListStyleType.Disc;
            ListStyleImage = string.Empty;
            ListStylePosition = CssListStylePosition.Outside;
            ListStyle = string.Empty;
        }
        private CssListProp(object owner, CssListProp inheritFrom)
        {
            this.owner = owner;
            ListStyleType = inheritFrom.ListStyleType;
            ListStyleImage = inheritFrom.ListStyleImage;
            ListStylePosition = inheritFrom.ListStylePosition;
            ListStyle = inheritFrom.ListStyle;
        }
        public CssListProp GetMyOwnVersion(object checkOwner)
        {
            if (this.owner == checkOwner)
            {
                return this;
            }
            else
            {
                return new CssListProp(checkOwner, this);
            }
        }

        public CssListStyleType ListStyleType { get; set; }
        public string ListStyleImage { get; set; }
        public CssListStylePosition ListStylePosition { get; set; }
        public string ListStyle { get; set; }

        public static readonly CssListProp Default = new CssListProp(null);

#if DEBUG
        public static bool dbugIsEq(dbugPropCheckReport rep, CssListProp prop1, CssListProp prop2)
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

    class CssCornerProp
    {

        object owner;
        public CssCornerProp(object owner)
        {
            this.owner = owner;
            this.NERadius =
                NWRadius =
                SERadius =
                SWRadius = CssLength.ZeroNoUnit;
        }
        private CssCornerProp(object owner, CssCornerProp inheritFrom)
        {
            this.owner = owner;

            this.NERadius = inheritFrom.NERadius;
            this.NWRadius = inheritFrom.NWRadius;
            this.SERadius = inheritFrom.SERadius;
            this.SWRadius = inheritFrom.SWRadius;

        }
        public CssCornerProp GetMyOwnVersion(object checkOwner)
        {
            if (this.owner == checkOwner)
            {
                return this;
            }
            else
            {
                return new CssCornerProp(owner, this);
            }
        }
        public static readonly CssCornerProp Default = new CssCornerProp(null);

        public CssLength NERadius { get; set; }
        public CssLength NWRadius { get; set; }
        public CssLength SERadius { get; set; }
        public CssLength SWRadius { get; set; }



#if DEBUG
        public static bool dbugIsEq(dbugPropCheckReport rep, CssCornerProp prop1, CssCornerProp prop2)
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
    class CssFontProp
    {
        object owner;
        Font _cacheFont;

        public CssFontProp(object owner)
        {
            this.owner = owner;
            FontFamily = "serif";
            FontSize = CssLength.FontSizeMedium;
            FontStyle = CssFontStyle.Normal;
            FontVariant = CssFontVariant.Normal;
            FontWeight = CssFontWeight.Normal;
        }
        private CssFontProp(object owner, CssFontProp inheritFrom)
        {
            this.owner = owner;
            this.FontFamily = inheritFrom.FontFamily;
            this.FontSize = inheritFrom.FontSize;
            this.FontStyle = inheritFrom.FontStyle;
            this.FontVariant = inheritFrom.FontVariant;
            this.FontWeight = inheritFrom.FontWeight;
        }

        public string FontFamily { get; set; }
        public CssLength FontSize { get; set; }
        public CssFontStyle FontStyle { get; set; }
        public CssFontVariant FontVariant { get; set; }
        public CssFontWeight FontWeight { get; set; }

        public CssFontProp GetMyOwnVersion(object checkOwner)
        {
            if (this.owner == checkOwner)
            {
                return this;
            }
            else
            {
                return new CssFontProp(checkOwner, this);
            }
        }
        public object Owner
        {
            get
            {
                return this.owner;
            }
        }
        internal Font GetCacheFont(CssBoxBase parentBox)
        {
            if (this._cacheFont != null)
            {
                return _cacheFont;
            }
            //---------------------------------------
            string fontFam = this.FontFamily;
            if (string.IsNullOrEmpty(FontFamily))
            {
                fontFam = CssConstants.FontSerif;
            }

            CssLength fontsize = this.FontSize;
            if (fontsize.IsEmpty)
            {
                fontsize = CssLength.MakeFontSizePtUnit(CssConstants.FontSize);
            }

            //-----------------------------------------------------------------------------
            FontStyle st = System.Drawing.FontStyle.Regular;
            if (FontStyle == CssFontStyle.Italic || FontStyle == CssFontStyle.Oblique)
            {
                st |= System.Drawing.FontStyle.Italic;
            }

            CssFontWeight fontWight = this.FontWeight;
            if (fontWight != CssFontWeight.Normal &&
                fontWight != CssFontWeight.Lighter &&
                fontWight != CssFontWeight.NotAssign &&
                fontWight != CssFontWeight.Inherit)
            {
                st |= System.Drawing.FontStyle.Bold;
            }

            float fsize = CssConstants.FontSize;
            bool relateToParent = false;

            if (fontsize.IsFontSizeName)
            {
                switch (fontsize.UnitOrNames)
                {

                    case CssUnitOrNames.FONTSIZE_MEDIUM:
                        fsize = CssConstants.FontSize; break;
                    case CssUnitOrNames.FONTSIZE_XX_SMALL:
                        fsize = CssConstants.FontSize - 4; break;
                    case CssUnitOrNames.FONTSIZE_X_SMALL:
                        fsize = CssConstants.FontSize - 3; break;
                    case CssUnitOrNames.FONTSIZE_LARGE:
                        fsize = CssConstants.FontSize + 2; break;
                    case CssUnitOrNames.FONTSIZE_X_LARGE:
                        fsize = CssConstants.FontSize + 3; break;
                    case CssUnitOrNames.FONTSIZE_XX_LARGE:
                        fsize = CssConstants.FontSize + 4; break;
                    case CssUnitOrNames.FONTSIZE_SMALLER:
                        {
                            relateToParent = true;
                            float parentFontSize = CssConstants.FontSize;
                            if (parentBox != null)
                            {
                                parentFontSize = parentBox.ActualFont.Size;
                            }
                            fsize = parentFontSize - 2;

                        } break;
                    case CssUnitOrNames.FONTSIZE_LARGER:
                        {
                            relateToParent = true;
                            float parentFontSize = CssConstants.FontSize;
                            if (parentBox != null)
                            {
                                parentFontSize = parentBox.ActualFont.Size;
                            }
                            fsize = parentFontSize + 2;

                        } break;
                    default:
                        throw new NotSupportedException();
                }
            }
            else
            {
                fsize = fontsize.Number;
            }

            if (fsize <= 1f)
            {
                fsize = CssConstants.FontSize;
            }

            if (!relateToParent)
            {
                return this._cacheFont = FontsUtils.GetCachedFont(fontFam, fsize, st);
            }
            else
            {
                //not store to cache font
                return FontsUtils.GetCachedFont(fontFam, fsize, st);
            }
        }
        public static readonly CssFontProp Default = new CssFontProp(null);


#if DEBUG
        public static bool dbugIsEq(dbugPropCheckReport rep, CssFontProp prop1, CssFontProp prop2)
        {
            if (prop1 == prop2)
            {
                return true;
            }
            //---------------- 
            rep.Check("FontFamily", prop1.FontFamily == prop2.FontFamily);
            rep.Check("FontSize", CssLength.IsEq(prop1.FontSize, prop2.FontSize));
            rep.Check("FontStyle", prop1.FontStyle == prop2.FontStyle);
            rep.Check("FontVariant", prop1.FontVariant == prop2.FontVariant);
            rep.Check("FontWeight", prop1.FontWeight == prop2.FontWeight);


            return false;
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

    class CssBackgroundProp
    {
        object owner;
        public CssBackgroundProp(object owner)
        {
            this.owner = owner;
            this.BackgroundColor = Color.Transparent; //"transparent";
            this.BackgroundGradient = Color.Transparent;// "none";
            this.BackgroundGradientAngle = 90.0f;
            this.BackgroundImageBinder = ImageBinder.NoImage;

            this.BackgroundPosX = new CssLength(0, CssUnitOrNames.Percent);
            this.BackgroundPosY = new CssLength(0, CssUnitOrNames.Percent);
            this.BackgroundRepeat = CssBackgroundRepeat.Repeat;
        }
        private CssBackgroundProp(object owner, CssBackgroundProp inheritFrom)
        {
            this.owner = owner;
            BackgroundColor = inheritFrom.BackgroundColor;
            BackgroundGradient = inheritFrom.BackgroundGradient;
            BackgroundGradientAngle = inheritFrom.BackgroundGradientAngle;
            BackgroundImageBinder = inheritFrom.BackgroundImageBinder;

            BackgroundPosX = inheritFrom.BackgroundPosX;
            BackgroundPosY = inheritFrom.BackgroundPosY;
            BackgroundRepeat = inheritFrom.BackgroundRepeat;
        }

        public CssBackgroundProp GetMyOwnVersion(object checkOwner)
        {
            if (this.owner == checkOwner)
            {
                return this;
            }
            else
            {
                return new CssBackgroundProp(checkOwner, this);
            }
        }
        //---------------------------------
        public Color BackgroundColor { get; set; }
        public Color BackgroundGradient { get; set; }
        public float BackgroundGradientAngle { get; set; }

        public ImageBinder BackgroundImageBinder { get; set; }
        public string BackgroundPosition { get; set; }

        public CssLength BackgroundPosX { get; set; }
        public CssLength BackgroundPosY { get; set; }
        public CssBackgroundRepeat BackgroundRepeat { get; set; }

        public static readonly CssBackgroundProp Default = new CssBackgroundProp(null);


#if DEBUG
        public static bool dbugIsEq(dbugPropCheckReport rep, CssBackgroundProp prop1, CssBackgroundProp prop2)
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

            return inMsgCount != rep.Count;
        }
#endif
    }
}