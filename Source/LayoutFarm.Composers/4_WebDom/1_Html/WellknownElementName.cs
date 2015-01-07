//BSD  2014 ,WinterDev

using System;
using System.Text;
using System.Collections.Generic;

namespace LayoutFarm.WebDom
{
    public enum WellknownName
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

        [Map("style")]
        Style,
        [Map("src")]
        Src,


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

        [Map("href")]
        Href,

        [Map("start")]
        Start,
        [Map("reversed")]
        Reversed,
        //-------------------


        [Map("x")]
        Svg_X,
        [Map("y")]
        Svg_Y,
        [Map("x1")]
        Svg_X1,
        [Map("y1")]
        Svg_Y1,
        [Map("x2")]
        Svg_X2,
        [Map("y2")]
        Svg_Y2,


        [Map("fill")]
        Svg_Fill,

        [Map("stroke")]
        Svg_Stroke,
        [Map("stroke-width")]
        Svg_Stroke_Width,

        [Map("rx")]
        Svg_Rx,
        [Map("ry")]
        Svg_Ry,

        [Map("r")]
        Svg_R,
        [Map("cx")]
        Svg_Cx,
        [Map("cy")]
        Svg_Cy,

        [Map("points")]
        Svg_Points,

        [Map("transform")]
        Svg_Transform,

        [Map("stop-color")]
        Svg_StopColor,

        [Map("offset")]
        Svg_Offset         
    }
}