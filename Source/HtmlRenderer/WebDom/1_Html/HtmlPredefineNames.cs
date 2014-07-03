//BSD  2014 ,WinterDev

using System;
using System.Text;
using System.Collections;
namespace HtmlRenderer.WebDom
{


    public enum WellknownHtmlName
    {
      
        Unknown,

        [Map("id")]
        Id,
        [Map("class")]
        Class,

        [Map("html")]
        Html,
        [Map("head")]
        Head,
        [Map("body")]
        Body,

        [Map("div")]
        Div,
        [Map("span")]
        Span,
        [Map("img")]
        Img,
        [Map("table")]
        Table,
        [Map("tr")]
        TR,
        [Map("td")]
        TD,
        [Map("th")]
        TH,
        [Map("link")]
        Link,
        [Map("rel")]
        Rel,
        [Map("align")]
        Align,
        [Map("br")]
        Br,
        [Map("left")]
        Left,
        [Map("top")]
        Top,
        [Map("width")]
        Width,
        [Map("height")]
        Height,

        [Map("rowspan")]
        RowSpan,
        [Map("colspan")]
        ColSpan,

        [Map("area")]
        Area,
        [Map("base")]
        Base,
        [Map("basefont")]
        BaseFont,
        [Map("col")]
        Col,
        [Map("frame")]
        Frame,
        [Map("iframe")]
        IFrame,
        [Map("hr")]
        Hr,
        
        [Map("input")]
        Input,

        [Map("isindex")]
        IsIndex,
        
        [Map("meta")]
        Meta,
        [Map("param")]
        Param,

        [Map("background")]
        Background,
        [Map("bgcolor")]
        BackgroundColor,
        [Map("border")]
        Border,
        [Map("bordercolor")]
        BorderColor,

        [Map("margin")]
        Margin,
        [Map("padding")]
        Padding,

        [Map("cellpadding")]
        CellPadding,

        [Map("cellspacing")]
        CellSpacing,

        [Map("face")]
        Face,
        [Map("color")]
        Color,
        [Map("valign")]
        VAlign,
        [Map("vspace")]
        VSpace,
        [Map("hspace")]
        HSpace,
        [Map("size")]
        Size,
        [Map("nowrap")]
        Nowrap,
        [Map("dir")]
        Dir,
        [Map("justify")]
        Justify,
    }


    public static class HtmlPredefineNames
    {

        static readonly ValueMap<WellknownHtmlName> _wellKnownHtmlNameMap =
            new ValueMap<WellknownHtmlName>();

        static UniqueStringTable htmlUniqueStringTableTemplate = new UniqueStringTable();

        static HtmlPredefineNames()
        {   
            int j = _wellKnownHtmlNameMap.Count; 
            for (int i = 0; i < j; ++i)
            {
                htmlUniqueStringTableTemplate.AddStringIfNotExist(_wellKnownHtmlNameMap.GetStringFromValue((WellknownHtmlName)i));
            }
        }
        internal static UniqueStringTable CreateUniqueStringTableClone()
        {
            return htmlUniqueStringTableTemplate.Clone();
        }
        
    }


}