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
    public class CssNameAttribute : Attribute
    {
        public CssNameAttribute(string name)
        {
            this.CssName = name;
        }
        public string CssName
        {
            get;
            private set;
        }
    }
    public class AttrNameAttribute : Attribute
    {
        public AttrNameAttribute(string name)
        {
            this.CssName = name;
        }
        public string CssName
        {
            get;
            private set;
        }
    }


    //--------------------------------------------------
    public enum CssDisplay : byte
    {
        [CssName(CssConstants.Inline)]
        Inline,//default

        [CssName(CssConstants.InlineBlock)]
        InlineBlock,

        [CssName(CssConstants.Block)]
        Block,

        [CssName(CssConstants.ListItem)]
        ListItem,

        [CssName(CssConstants.None)]
        None,

        [CssName(CssConstants.Table)]
        Table,

        [CssName(CssConstants.TableCell)]
        TableCell,

        [CssName(CssConstants.TableRow)]
        TableRow,

        [CssName(CssConstants.InlineTable)]
        InlineTable,

        [CssName(CssConstants.TableColumn)]
        TableColumn,

        [CssName(CssConstants.TableColumnGroup)]
        TableColumnGroup,

        [CssName(CssConstants.TableRowGroup)]
        TableRowGroup,

        [CssName(CssConstants.TableCaption)]
        TableCaption,

        [CssName(CssConstants.TableHeaderGroup)]
        TableHeaderGroup,

        [CssName(CssConstants.TableFooterGroup)]
        TableFooterGroup,
    }
    public enum CssWhiteSpace : byte
    {
        [CssName(CssConstants.Normal)]
        Normal,//default

        [CssName(CssConstants.Pre)]
        Pre,
        [CssName(CssConstants.PreLine)]
        PreLine,
        [CssName(CssConstants.PreWrap)]
        PreWrap,

        [CssName(CssConstants.NoWrap)]
        NoWrap,
    }
    public enum CssBorderStyle : byte
    {
        /// <summary>
        /// default
        /// </summary>
        [CssName(CssConstants.None)]
        None,
        [CssName(CssConstants.Hidden)]
        Hidden,

        [CssName(CssConstants.Visible)]
        Visible,//boundary-- extension ***

        [CssName(CssConstants.Dotted)]
        Dotted,

        [CssName(CssConstants.Dashed)]
        Dashed,

        [CssName(CssConstants.Solid)]
        Solid,

        [CssName(CssConstants.Double)]
        Double,

        [CssName(CssConstants.Groove)]
        Groove,
        [CssName(CssConstants.Ridge)]
        Ridge,
        [CssName(CssConstants.Inset)]
        Inset,
        [CssName(CssConstants.Outset)]
        Outset,

        Initial,

        [CssName(CssConstants.Inherit)]
        Inherit
    }
    public enum CssWordBreak : byte
    {
        [CssName(CssConstants.Normal)]
        Normal,//default
        [CssName(CssConstants.BreakAll)]
        BreakAll,
        [CssName(CssConstants.KeepAll)]
        KeepAll,
        Initial,
        [CssName(CssConstants.Inherit)]
        Inherit
    }

    public enum CssDirection : byte
    {
        [CssName(CssConstants.Ltr)]
        Ltl,//default
        [CssName(CssConstants.Rtl)]
        Rtl
    }
    public enum CssVerticalAlign : byte
    {
        [CssName(CssConstants.Baseline)]
        Baseline,
        [CssName(CssConstants.Sub)]
        Sub,
        [CssName(CssConstants.Super)]
        Super,
        [CssName(CssConstants.TextTop)]
        TextTop,
        [CssName(CssConstants.TextBottom)]
        TextBottom,
        [CssName(CssConstants.Top)]
        Top,
        [CssName(CssConstants.Bottom)]
        Bottom,
        [CssName(CssConstants.Middle)]
        Middle

    }
    public enum CssVisibility : byte
    {
        [CssName(CssConstants.Visible)]
        Visible,//default
        [CssName(CssConstants.Hidden)]
        Hidden,
        [CssName(CssConstants.Collapse)]
        Collapse,

        Initial,
        [CssName(CssConstants.Inherit)]
        Inherit
    }
    public enum CssTextAlign : byte
    {
        NotAssign,
        [CssName(CssConstants.Left)]
        Left,//default
        [CssName(CssConstants.Right)]
        Right,
        [CssName(CssConstants.Center)]
        Center,
        [CssName(CssConstants.Justify)]
        Justify,
        Initial,
        [CssName(CssConstants.Inherit)]
        Inherit
    }

    public enum CssPosition : byte
    {
        [CssName(CssConstants.Static)]
        Static,
        [CssName(CssConstants.Relative)]
        Relative,
        [CssName(CssConstants.Absolute)]
        Absolute,
        [CssName(CssConstants.Fixed)]
        Fixed
    }
    public enum CssTextDecoration : byte
    {
        NotAssign,
        [CssName(CssConstants.None)]
        None,
        [CssName(CssConstants.Underline)]
        Underline,
        [CssName(CssConstants.LineThrough)]
        LineThrough,
        [CssName(CssConstants.Overline)]
        Overline
    }
    public enum CssOverflow : byte
    {
        [CssName(CssConstants.Visible)]
        Visible,
        [CssName(CssConstants.Hidden)]
        Hidden,
        [CssName(CssConstants.Scroll)]
        Scroll,
        [CssName(CssConstants.Auto)]
        Auto,
        Initial,
        [CssName(CssConstants.Inherit)]
        Inherit
    }
    public enum CssBorderCollapse : byte
    {
        [CssName(CssConstants.Separate)]
        Separate,
        [CssName(CssConstants.Collapse)]
        Collapse,

        Initial,
        [CssName(CssConstants.Inherit)]
        Inherit
    }
    public enum CssEmptyCell : byte
    {
        [CssName(CssConstants.Show)]
        Show,
        [CssName(CssConstants.Hide)]
        Hide,
        Initial,
        [CssName(CssConstants.Inherit)]
        Inherit
    }

    public enum CssFloat : byte
    {
        [CssName(CssConstants.None)]
        None,
        [CssName(CssConstants.Left)]
        Left,
        [CssName(CssConstants.Right)]
        Right,

        Initial,
        [CssName(CssConstants.Inherit)]
        Inherit
    }

    public enum CssFontStyle : byte
    {
        [CssName(CssConstants.Normal)]
        Normal,
        [CssName(CssConstants.Italic)]
        Italic,
        [CssName(CssConstants.Oblique)]
        Oblique,
        Initial,
        [CssName(CssConstants.Inherit)]
        Inherit
    }
    public enum CssFontVariant : byte
    {
        [CssName(CssConstants.Normal)]
        Normal,
        [CssName(CssConstants.SmallCaps)]
        SmallCaps,
        Initial,
        [CssName(CssConstants.Inherit)]
        Inherit
    }
    public enum CssFontWeight : byte
    {
        NotAssign,
        [CssName(CssConstants.Normal)]
        Normal,
        [CssName(CssConstants.Bold)]
        Bold,
        [CssName(CssConstants.Bolder)]
        Bolder,
        [CssName(CssConstants.Lighter)]
        Lighter,



        [CssName("100")]
        _100,
        [CssName("200")]
        _200,
        [CssName("300")]
        _300,
        [CssName("400")]
        _400,
        [CssName("500")]
        _500,
        [CssName("600")]
        _600,

        Initial,        
        [CssName(CssConstants.Inherit)]
        Inherit
    }
}