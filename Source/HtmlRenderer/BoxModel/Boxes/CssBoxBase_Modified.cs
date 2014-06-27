//BSD, 2014, WinterCore

using System;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{




    partial class CssBoxBase
    {

        
        static class CssBoxFlagsConst
        {
            public const int HAS_ASSIGNED_LOCATION = 1 << (2 - 1);
            public const int EVAL_ROWSPAN = 1 << (3 - 1);
            public const int EVAL_COLSPAN = 1 << (4 - 1);
            public const int HAS_EVAL_WHITESPACE = 1 << (5 - 1);
            public const int TEXT_IS_ALL_WHITESPACE = 1 << (6 - 1);
            public const int TEXT_IS_EMPTY = 1 << (7 - 1);
        } 
       
        CssBorderProp CheckBorderVersion()
        {
            return this._borderProps = this._borderProps.GetMyOwnVersion(this);
        }
        CssMarginProp CheckMarginVersion()
        {   
            return this._marginProps = this._marginProps.GetMyOwnVersion(this);
        }
        CssPaddingProp CheckPaddingVersion()
        {
            return this._paddingProps = this._paddingProps.GetMyOwnVersion(this);
        }
        CssCornerProp CheckCornerVersion()
        {
            return this._cornerProps = this._cornerProps.GetMyOwnVersion(this);
        }
        CssFontProp CheckFontVersion()
        {
            return this._fontProps = this._fontProps.GetMyOwnVersion(this);
        }
        CssListProp CheckListPropVersion()
        {
            return this._listProps = this._listProps.GetMyOwnVersion(this);
        }
        CssBackgroundProp CheckBgVersion()
        {
            return this._backgroundProps = this._backgroundProps.GetMyOwnVersion(this);
        }  
        

    }

}