//MS-PL, Apache2 
//2014, WinterDev

using System;
using System.Collections.Generic;
using HtmlRenderer.Drawing;
using HtmlRenderer.Boxes;
using HtmlRenderer.Css;
using HtmlRenderer.Boxes.Svg;
using HtmlRenderer.Composers;
using HtmlRenderer.Composers.BridgeHtml;

namespace HtmlRenderer.SvgDom
{
    static class SvgCreator
    {

        public static SvgRootBox CreateSvgBox(CssBox parentBox,
            HtmlElement elementNode,
            Css.BoxSpec spec)
        {
            SvgFragment fragment = new SvgFragment();
            SvgRootBox rootBox = new SvgRootBox(parentBox,
                elementNode,
                elementNode.Spec,
                fragment);
            //generate its child 

            int j = elementNode.ChildrenCount;
            for (int i = 0; i < j; ++i)
            {
                HtmlElement node = elementNode.GetChildNode(i) as HtmlElement;
                if (node == null)
                {
                    continue;
                }
                switch (node.WellknownElementName)
                {
                    case WellKnownDomNodeName.svg_rect:
                        {
                            CreateSvgRect(fragment, node);
                        } break;
                    default:
                        {

                        } break;
                }
            }
            return rootBox;
        }
        static void CreateSvgRect(SvgElement parentNode, HtmlElement elem)
        {
            SvgRect rect = new SvgRect();
            //translate attribute
            TranslateSvgRectAttributes(rect, elem);
            parentNode.AddChild(rect);
        }
        //public static void GenerateChildBoxes(SvgElement parentSVG, HtmlElement parentElement)
        //{

        //    int j = parentElement.ChildrenCount;
        //    for (int i = 0; i < j; ++i)
        //    {
        //        HtmlElement node = parentElement.GetChildNode(i) as HtmlElement;


        //        if (node == null)
        //        {
        //            continue;
        //        }
        //        node.WellknownElementName = UserMapUtil.EvaluateTagName(node.LocalName);
        //        switch (node.WellknownElementName)
        //        {
        //            case WellKnownDomNodeName.svg_rect:
        //                {
        //                    SvgRect rect = new SvgRect();
        //                    //translate attribute to real value
        //                    TranslateSvgAttributes(rect, node);
        //                } break;
        //            default:
        //                {

        //                } break;
        //        }
        //    }
        //}

        public static void TranslateSvgAttributesMain(HtmlElement elem)
        {
            //if (elem.WellknownElementName != WellKnownDomNodeName.svg)
            //{
            //    return;
            //}
            //int j = elem.ChildrenCount;
            //for (int i = 0; i < j; ++i)
            //{
            //    HtmlElement node = elem.GetChildNode(i) as HtmlElement;
            //    if (node == null)
            //    {
            //        continue;
            //    }
            //    node.WellknownElementName = UserMapUtil.EvaluateTagName(node.LocalName);
            //    switch (node.WellknownElementName)
            //    {
            //        case WellKnownDomNodeName.svg_rect:
            //            {
            //                SvgRect rect = new SvgRect();
            //                //translate attribute to real value

            //                node.AttachSvgElement = rect;
            //                TranslateSvgAttributes(rect, node);
            //                //add to node like 
            //                //boxspec

            //            } break;
            //        default:
            //            {

            //            } break;
            //    }
            //}
        }
        public static void TranslateSvgRectAttributes(SvgRect rect, HtmlElement elem)
        {

            foreach (WebDom.DomAttribute attr in elem.GetAttributeIterForward())
            {
                WebDom.WellknownName wellknownName = (WebDom.WellknownName)attr.LocalNameIndex;

                switch (wellknownName)
                {
                    case WebDom.WellknownName.Svg_X:
                        {
                            rect.X = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Y:
                        {
                            rect.Y = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Width:
                        {
                            rect.Width = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Height:
                        {
                            rect.Height = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Fill:
                        {
                            rect.ActualColor = CssValueParser.GetActualColor(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Stroke:
                        {
                            rect.StrokeColor = CssValueParser.GetActualColor(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Stroke_Width:
                        {
                            rect.StrokeWidth = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    default:
                        {
                            //other attrs
                        } break;

                }
            }
        }
    }

}