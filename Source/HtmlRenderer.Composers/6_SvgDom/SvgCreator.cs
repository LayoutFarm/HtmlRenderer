//MS-PL, Apache2 
//2014, WinterDev

using System;
using System.Collections.Generic;
using HtmlRenderer.Drawing;
using HtmlRenderer.Boxes; 
using HtmlRenderer.Boxes.Svg;

namespace HtmlRenderer.SvgDom
{
    static class SvgCreator
    {

        public static CssBox CreateSvgBox(CssBox parentBox,
            Composers.BridgeHtml.BridgeHtmlElement elementNode,
            Css.BoxSpec spec)
        {

            SvgRootBox rootBox = new SvgRootBox(parentBox, elementNode, spec);

            return rootBox;
        }

    }

}