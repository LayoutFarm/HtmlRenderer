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
    
    //--------------------------------------------------
    public enum CssDisplay : byte
    {
        [Map(CssConstants.Inline)]
        Inline,//default 

        [Map(CssConstants.InlineBlock)]
        InlineBlock,
        [Map(CssConstants.TableRow)]
        TableRow,
        [Map(CssConstants.InlineTable)]
        InlineTable,
        [Map(CssConstants.TableColumn)]
        TableColumn,

        [Map(CssConstants.TableColumnGroup)]
        TableColumnGroup,

        [Map(CssConstants.TableRowGroup)]
        TableRowGroup,

        [Map(CssConstants.TableCaption)]
        TableCaption,

        [Map(CssConstants.TableHeaderGroup)]
        TableHeaderGroup,

        [Map(CssConstants.TableFooterGroup)]
        TableFooterGroup,

        [Map(CssConstants.None)]
        None,

        //===========================================
        /// <summary>
        ///following group act as Containing box : Block,Table,TableCell, ListItem
        /// </summary>
        __CONTAINER_BEGIN_HERE,

        [Map(CssConstants.Block)]
        Block,
        [Map(CssConstants.Table)]
        Table,
        [Map(CssConstants.TableCell)]
        TableCell,
        [Map(CssConstants.ListItem)]
        ListItem,
        //===========================================
    }
    public enum CssWhiteSpace : byte
    {
        [Map(CssConstants.Normal)]
        Normal,//default

        [Map(CssConstants.Pre)]
        Pre,
        [Map(CssConstants.PreLine)]
        PreLine,
        [Map(CssConstants.PreWrap)]
        PreWrap,

        [Map(CssConstants.NoWrap)]
        NoWrap,
    }
    public enum CssBorderStyle : byte
    {
        /// <summary>
        /// default
        /// </summary>
        [Map(CssConstants.None)]
        None,
        [Map(CssConstants.Hidden)]
        Hidden,

        [Map(CssConstants.Visible)]
        Visible,//boundary-- extension ***

        [Map(CssConstants.Dotted)]
        Dotted,

        [Map(CssConstants.Dashed)]
        Dashed,

        [Map(CssConstants.Solid)]
        Solid,

        [Map(CssConstants.Double)]
        Double,

        [Map(CssConstants.Groove)]
        Groove,
        [Map(CssConstants.Ridge)]
        Ridge,
        [Map(CssConstants.Inset)]
        Inset,
        [Map(CssConstants.Outset)]
        Outset,
         
        [Map(CssConstants.Inherit)]
        Inherit,
         
        //extension
        Unknown
    }
    public enum CssWordBreak : byte
    {
        [Map(CssConstants.Normal)]
        Normal,//default
        [Map(CssConstants.BreakAll)]
        BreakAll,
        [Map(CssConstants.KeepAll)]
        KeepAll,
       
        [Map(CssConstants.Inherit)]
        Inherit
    }

    public enum CssDirection : byte
    {
        [Map(CssConstants.Ltr)]
        Ltl,//default
        [Map(CssConstants.Rtl)]
        Rtl
    }
    public enum CssVerticalAlign : byte
    {
        [Map(CssConstants.Baseline)]
        Baseline,
        [Map(CssConstants.Sub)]
        Sub,
        [Map(CssConstants.Super)]
        Super,
        [Map(CssConstants.TextTop)]
        TextTop,
        [Map(CssConstants.TextBottom)]
        TextBottom,
        [Map(CssConstants.Top)]
        Top,
        [Map(CssConstants.Bottom)]
        Bottom,
        [Map(CssConstants.Middle)]
        Middle

    }
    public enum CssVisibility : byte
    {
        [Map(CssConstants.Visible)]
        Visible,//default
        [Map(CssConstants.Hidden)]
        Hidden,
        [Map(CssConstants.Collapse)]
        Collapse,

        Initial,
        [Map(CssConstants.Inherit)]
        Inherit
    }
    public enum CssTextAlign : byte
    {
        NotAssign,
        [Map(CssConstants.Left)]
        Left,//default
        [Map(CssConstants.Right)]
        Right,
        [Map(CssConstants.Center)]
        Center,
        [Map(CssConstants.Justify)]
        Justify,
       
        [Map(CssConstants.Inherit)]
        Inherit
    }

    public enum CssPosition : byte
    {
        [Map(CssConstants.Static)]
        Static,
        [Map(CssConstants.Relative)]
        Relative,
        [Map(CssConstants.Absolute)]
        Absolute,
        [Map(CssConstants.Fixed)]
        Fixed
    }
    public enum CssTextDecoration : byte
    {
        NotAssign,
        [Map(CssConstants.None)]
        None,
        [Map(CssConstants.Underline)]
        Underline,
        [Map(CssConstants.LineThrough)]
        LineThrough,
        [Map(CssConstants.Overline)]
        Overline
    }
    public enum CssOverflow : byte
    {
        [Map(CssConstants.Visible)]
        Visible,
        [Map(CssConstants.Hidden)]
        Hidden,
        [Map(CssConstants.Scroll)]
        Scroll,
        [Map(CssConstants.Auto)]
        Auto,
         
        [Map(CssConstants.Inherit)]
        Inherit
    }
    public enum CssBorderCollapse : byte
    {
        [Map(CssConstants.Separate)]
        Separate,
        [Map(CssConstants.Collapse)]
        Collapse,

        Initial,
        [Map(CssConstants.Inherit)]
        Inherit
    }
    public enum CssEmptyCell : byte
    {
        [Map(CssConstants.Show)]
        Show,
        [Map(CssConstants.Hide)]
        Hide,
        Initial,
        [Map(CssConstants.Inherit)]
        Inherit
    }

    public enum CssFloat : byte
    {
        [Map(CssConstants.None)]
        None,
        [Map(CssConstants.Left)]
        Left,
        [Map(CssConstants.Right)]
        Right,

        Initial,
        [Map(CssConstants.Inherit)]
        Inherit
    }

    public enum CssFontStyle : byte
    {
        [Map(CssConstants.Normal)]
        Normal,
        [Map(CssConstants.Italic)]
        Italic,
        [Map(CssConstants.Oblique)]
        Oblique,
        Initial,
        [Map(CssConstants.Inherit)]
        Inherit,

        Unknown,
    }
    public enum CssFontVariant : byte
    {
        [Map(CssConstants.Normal)]
        Normal,
        [Map(CssConstants.SmallCaps)]
        SmallCaps,
        Initial,
        [Map(CssConstants.Inherit)]
        Inherit,
        Unknown,
    }
    public enum CssFontWeight : byte
    {
        NotAssign,
        [Map(CssConstants.Normal)]
        Normal,
        [Map(CssConstants.Bold)]
        Bold,
        [Map(CssConstants.Bolder)]
        Bolder,
        [Map(CssConstants.Lighter)]
        Lighter,
         
        [Map("100")]
        _100,
        [Map("200")]
        _200,
        [Map("300")]
        _300,
        [Map("400")]
        _400,
        [Map("500")]
        _500,
        [Map("600")]
        _600,

         
        [Map(CssConstants.Inherit)]
        Inherit,

        Unknown,

    }
    public enum CssListStylePosition : byte
    {
        [Map(CssConstants.Outset)]
        Outside,
        [Map(CssConstants.Inside)]
        Inside,

        
        [Map(CssConstants.Inherit)]
        Inherit
    }
    public enum CssListStyleType : byte
    {
        [Map(CssConstants.None)]
        None,
        [Map(CssConstants.Disc)]
        Disc,

        [Map(CssConstants.Circle)]
        Circle,
        [Map(CssConstants.Separate)]
        Square,
        //-----------------------------
        
        [Map(CssConstants.Inherit)]
        Inherit,

        //-----------------------------
        [Map(CssConstants.Decimal)]
        Decimal,
        [Map(CssConstants.DecimalLeadingZero)]
        DecimalLeadingZero,

        [Map(CssConstants.LowerAlpha)]
        LowerAlpha,
        [Map(CssConstants.UpperAlpha)]
        UpperAlpha,

        [Map(CssConstants.LowerLatin)]
        LowerLatin,
        [Map(CssConstants.UpperLatin)]
        UpperLatin,

        [Map(CssConstants.LowerGreek)]
        LowerGreek,

        [Map(CssConstants.LowerRoman)]
        LowerRoman,
        [Map(CssConstants.UpperRoman)]
        UpperRoman,

        [Map(CssConstants.Armenian)]
        Armenian,
        [Map(CssConstants.Georgian)]
        Georgian,
        [Map(CssConstants.Hebrew)]
        Hebrew,
        [Map(CssConstants.Hiragana)]
        Hiragana,
        [Map(CssConstants.HiraganaIroha)]
        HiraganaIroha,
        [Map(CssConstants.Katakana)]
        Katakana,
        [Map(CssConstants.KatakanaIroha)]
        KatakanaIroha,
    }

    public enum CssNamedBorderWidth : byte
    {
        Unknown,

        [Map(CssConstants.Thin)]
        Thin,
        [Map(CssConstants.Medium)]
        Medium,
        [Map(CssConstants.Thick)]
        Thick
    }

}