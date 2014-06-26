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
                switch (fontsize.FontSizeName)
                {
                    case CssFontSizeConst.FONTSIZE_MEDIUM:
                        fsize = CssConstants.FontSize; break;
                    case CssFontSizeConst.FONTSIZE_XX_SMALL:
                        fsize = CssConstants.FontSize - 4; break;
                    case CssFontSizeConst.FONTSIZE_X_SMALL:
                        fsize = CssConstants.FontSize - 3; break;
                    case CssFontSizeConst.FONTSIZE_LARGE:
                        fsize = CssConstants.FontSize + 2; break;
                    case CssFontSizeConst.FONTSIZE_X_LARGE:
                        fsize = CssConstants.FontSize + 3; break;
                    case CssFontSizeConst.FONTSIZE_XX_LARGE:
                        fsize = CssConstants.FontSize + 4; break;
                    case CssFontSizeConst.FONTSIZE_SMALLER:
                        {
                            relateToParent = true;
                            float parentFontSize = CssConstants.FontSize;
                            if (parentBox != null)
                            {
                                parentFontSize = parentBox.ActualFont.Size;
                            }
                            fsize = parentFontSize - 2;

                        } break;
                    case CssFontSizeConst.FONTSIZE_LARGER:
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
    }



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

            this.BackgroundPosX = new CssLength(0, CssUnit.Percent);
            this.BackgroundPosY = new CssLength(0, CssUnit.Percent);
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
        public Color BackgroundColor { get; set; }
        public Color BackgroundGradient { get; set; }
        public float BackgroundGradientAngle { get; set; }

        public ImageBinder BackgroundImageBinder { get; set; }
        public string BackgroundPosition { get; set; }

        public CssLength BackgroundPosX { get; set; }
        public CssLength BackgroundPosY { get; set; }

        public CssBackgroundRepeat BackgroundRepeat { get; set; }
        public static readonly CssBackgroundProp Default = new CssBackgroundProp(null);
    }
}