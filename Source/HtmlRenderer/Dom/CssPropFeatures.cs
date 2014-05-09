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
            ListStyleType = "disc";
            ListStyleImage = string.Empty;
            ListStylePosition = "outside";
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


        public string ListStyleType { get; set; }
        public string ListStyleImage { get; set; }
        public string ListStylePosition { get; set; }
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
        public CssFontProp(object owner)
        {
            this.owner = owner;
            FontFamily = "serif";
            FontSize = "medium";
            FontStyle = "normal";
            FontVariant = "normal";
            FontWeight = "normal";
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
        public string FontSize { get; set; }
        public string FontStyle { get; set; }
        public string FontVariant { get; set; }
        public string FontWeight { get; set; }

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
            this.BackgroundImage = "none";
            this.BackgroundPosition = "0% 0%";
            this.BackgroundRepeat = "repeat";
        }
        private CssBackgroundProp(object owner, CssBackgroundProp inheritFrom)
        {
            this.owner = owner;
            BackgroundColor = inheritFrom.BackgroundColor;
            BackgroundGradient = inheritFrom.BackgroundGradient;
            BackgroundGradientAngle = inheritFrom.BackgroundGradientAngle;
            BackgroundImage = inheritFrom.BackgroundImage;
            BackgroundPosition = inheritFrom.BackgroundPosition;
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

        public string BackgroundImage { get; set; }
        public string BackgroundPosition { get; set; }
        public string BackgroundRepeat { get; set; }

        public static readonly CssBackgroundProp Default = new CssBackgroundProp(null);
    }
}