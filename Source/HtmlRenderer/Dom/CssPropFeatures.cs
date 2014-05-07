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
        CssLength _borderLeftWidth;
        CssLength _borderTopWidth;
        CssLength _borderRightWidth;
        CssLength _borderBottomWidth;

        CssBorderStyle _borderLeftStyle;
        CssBorderStyle _borderTopStyle;
        CssBorderStyle _borderRightStyle;
        CssBorderStyle _borderBottomStyle;


        Color _borderTopColor;// = "black";
        Color _borderRightColor; //= "black";
        Color _borderBottomColor; //= "black";
        Color _borderLeftColor; //= "black";


        public static readonly CssBorderProp Default = new CssBorderProp(null);

        object owner;
        public CssBorderProp(object owner)
        {
            this.owner = owner;
            this._borderLeftWidth =
                this._borderTopWidth =
                this._borderRightWidth =
                this._borderBottomWidth = CssLength.Medium;
            //---------------------------------------------
            this._borderLeftStyle =
                this._borderTopStyle =
                this._borderRightStyle =
                this._borderBottomStyle = CssBorderStyle.None;
            //---------------------------------------------
            this._borderLeftColor =
                this._borderTopColor =
                this._borderRightColor =
                this._borderBottomColor = Color.Black;
            //---------------------------------------------

        }
        private CssBorderProp(object owner, CssBorderProp inheritFrom)
        {
            this.owner = owner;
            this._borderLeftWidth = inheritFrom._borderLeftWidth;
            this._borderTopWidth = inheritFrom._borderTopWidth;
            this._borderRightWidth = inheritFrom._borderRightWidth;
            this._borderBottomWidth = inheritFrom._borderBottomWidth;
            //---------------------------------------------------------
            this._borderLeftStyle = inheritFrom._borderLeftStyle;
            this._borderTopStyle = inheritFrom._borderTopStyle;
            this._borderRightStyle = inheritFrom._borderRightStyle;
            this._borderBottomStyle = inheritFrom._borderBottomStyle;
            //---------------------------------------------------------
            this._borderLeftColor = inheritFrom._borderLeftColor;
            this._borderTopColor = inheritFrom._borderTopColor;
            this._borderRightColor = inheritFrom._borderRightColor;
            this._borderBottomColor = inheritFrom._borderBottomColor;
            //---------------------------------------------------------

        }
        public CssLength LeftWidth
        {
            get { return this._borderLeftWidth; }
            set { this._borderLeftWidth = value; }
        }
        public CssLength TopWidth
        {
            get { return this._borderTopWidth; }
            set { this._borderTopWidth = value; }
        }
        public CssLength RightWidth
        {
            get { return this._borderRightWidth; }
            set { this._borderRightWidth = value; }
        }
        public CssLength BottomWidth
        {
            get { return this._borderBottomWidth; }
            set { this._borderBottomWidth = value; }
        }

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

        public CssBorderStyle LeftStyle
        {
            get { return this._borderLeftStyle; }
            set { this._borderLeftStyle = value; }
        }
        public CssBorderStyle TopStyle
        {
            get { return this._borderTopStyle; }
            set { this._borderTopStyle = value; }
        }
        public CssBorderStyle RightStyle
        {
            get { return this._borderRightStyle; }
            set { this._borderRightStyle = value; }
        }
        public CssBorderStyle BottomStyle
        {
            get { return this._borderBottomStyle; }
            set { this._borderBottomStyle = value; }
        }
        public Color LeftColor
        {
            get { return this._borderLeftColor; }
            set { this._borderLeftColor = value; }
        }
        public Color TopColor
        {
            get { return this._borderTopColor; }
            set { this._borderTopColor = value; }
        }
        public Color RightColor
        {
            get { return this._borderRightColor; }
            set { this._borderRightColor = value; }
        }
        public Color BottomColor
        {
            get { return this._borderBottomColor; }
            set { this._borderBottomColor = value; }
        }

    }

    class CssMarginProp
    {
        object owner;
        CssLength _marginBottom;
        CssLength _marginLeft;
        CssLength _marginRight;
        CssLength _marginTop;
        public CssMarginProp(object owner)
        {
            this.owner = owner;
            this._marginLeft =
                this._marginTop =
                this._marginRight =
                this._marginBottom = CssLength.ZeroPx;
        }
        private CssMarginProp(object newOwner, CssMarginProp inheritFrom)
        {
            this.owner = newOwner;

            this._marginLeft = inheritFrom._marginLeft;
            this._marginTop = inheritFrom._marginTop;
            this._marginRight = inheritFrom._marginRight;
            this._marginBottom = inheritFrom._marginBottom;
        }
        public static readonly CssMarginProp Default = new CssMarginProp(null);
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


        public CssLength Left
        {
            get { return this._marginLeft; }
            set { this._marginLeft = value; }
        }
        public CssLength Top
        {
            get { return this._marginTop; }
            set { this._marginTop = value; }
        }
        public CssLength Right
        {
            get { return this._marginRight; }
            set { this._marginRight = value; }
        }
        public CssLength Bottom
        {
            get { return this._marginBottom; }
            set { this._marginBottom = value; }
        }
    }
    class CssPaddingProp
    {
        CssLength _paddingLeft;
        CssLength _paddingBottom;
        CssLength _paddingRight;
        CssLength _paddingTop;

        object owner;
        public CssPaddingProp(object owner)
        {
            this.owner = owner;
            this._paddingLeft =
                   _paddingBottom =
                   _paddingRight =
                   _paddingTop = CssLength.ZeroNoUnit;
        }
        private CssPaddingProp(object newOwner, CssPaddingProp inheritFrom)
        {
            this.owner = newOwner;
            this._paddingLeft = inheritFrom._paddingLeft;
            this._paddingTop = inheritFrom._paddingTop;
            this._paddingRight = inheritFrom._paddingRight;
            this._paddingBottom = inheritFrom._paddingBottom;
        }
        public CssLength Left
        {
            get { return this._paddingLeft; }
            set { this._paddingLeft = value; }
        }
        public CssLength Top
        {
            get { return this._paddingTop; }
            set { this._paddingTop = value; }
        }
        public CssLength Right
        {
            get { return this._paddingRight; }
            set { this._paddingRight = value; }
        }
        public CssLength Bottom
        {
            get { return this._paddingBottom; }
            set { this._paddingBottom = value; }
        }

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
        string _listStyleType;
        string _listStyleImage;
        string _listStylePosition;
        string _listStyle;

        object owner;
        public CssListProp(object owner)
        {
            this.owner = owner;
            _listStyleType = "disc";
            _listStyleImage = string.Empty;
            _listStylePosition = "outside";
            _listStyle = string.Empty;
        }
        private CssListProp(object owner, CssListProp inheritFrom)
        {
            this.owner = owner;
            _listStyleType = inheritFrom._listStyleType;
            _listStyleImage = inheritFrom._listStyleImage;
            _listStylePosition = inheritFrom._listStylePosition;
            _listStyle = inheritFrom._listStyle;
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


        public string ListStyleType
        {
            get { return this._listStyleType; }
            set { this._listStyleType = value; }
        }
        public string ListStyleImage
        {
            get { return this._listStyleImage; }
            set { this._listStyleImage = value; }
        }
        public string ListStylePosition
        {
            get { return this._listStylePosition; }
            set { this._listStylePosition = value; }
        }
        public string ListStyle
        {
            get { return this._listStyle; }
            set { this._listStyle = value; }
        }

        public static readonly CssListProp Default = new CssListProp(null);
    }



    class CssCornerProp
    {
        CssLength _cornerNWRadius;// = CssLength.ZeroNoUnit;
        CssLength _cornerNERadius; //= CssLength.ZeroNoUnit;
        CssLength _cornerSERadius;//= CssLength.ZeroNoUnit;
        CssLength _cornerSWRadius; //= CssLength.ZeroNoUnit;

        object owner;
        public CssCornerProp(object owner)
        {
            this.owner = owner;
            this._cornerNERadius =
                _cornerNWRadius =
                _cornerSERadius =
                _cornerSWRadius = CssLength.ZeroNoUnit;
        }
        private CssCornerProp(object owner, CssCornerProp inheritFrom)
        {
            this.owner = owner;
            this._cornerNERadius = inheritFrom._cornerNERadius;
            this._cornerNWRadius = inheritFrom._cornerNWRadius;
            this._cornerSERadius = inheritFrom._cornerSERadius;
            this._cornerSWRadius = inheritFrom._cornerSWRadius;

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

        public CssLength NERadius
        {
            get { return this._cornerNERadius; }
            set { this._cornerNERadius = value; }
        }
        public CssLength NWRadius
        {
            get { return this._cornerNWRadius; }
            set { this._cornerNWRadius = value; }
        }
        public CssLength SERadius
        {
            get { return this._cornerSERadius; }
            set { this._cornerSERadius = value; }
        }
        public CssLength SWRadius
        {
            get { return this._cornerSWRadius; }
            set { this._cornerSWRadius = value; }
        }
    }
}