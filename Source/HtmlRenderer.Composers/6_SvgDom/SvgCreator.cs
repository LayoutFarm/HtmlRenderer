//MS-PL, Apache2 
//2014, WinterDev

using System;
using System.Collections.Generic;
using HtmlRenderer.Drawing;
using HtmlRenderer.Boxes;
using HtmlRenderer.Css;
using HtmlRenderer.Composers;
using HtmlRenderer.Composers.BridgeHtml;

using HtmlRenderer.SvgDom;
using HtmlRenderer.WebDom;
namespace HtmlRenderer.Composers.BridgeHtml
{
    static class SvgCreator
    {

        public static CssBoxSvgRoot CreateSvgBox(CssBox parentBox,
            HtmlElement elementNode,
            Css.BoxSpec spec)
        {
            SvgFragment fragment = new SvgFragment();
            CssBoxSvgRoot rootBox = new CssBoxSvgRoot(parentBox,
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

            SvgRectSpec rectSpec = new SvgRectSpec();
            SvgRect rect = new SvgRect(rectSpec, elem);
            //translate attribute
            TranslateSvgRectAttributes(rectSpec, elem);
            parentNode.AddChild(rect);
        }

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
        public static void TranslateSvgRectAttributes(SvgRectSpec rectSpec, HtmlElement elem)
        {

            foreach (WebDom.DomAttribute attr in elem.GetAttributeIterForward())
            {
                WebDom.WellknownName wellknownName = (WebDom.WellknownName)attr.LocalNameIndex;

                switch (wellknownName)
                {
                    case WebDom.WellknownName.Svg_X:
                        {
                            rectSpec.X = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Y:
                        {
                            rectSpec.Y = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Width:
                        {
                            rectSpec.Width = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Height:
                        {
                            rectSpec.Height = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Fill:
                        {
                            rectSpec.ActualColor = CssValueParser.GetActualColor(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Stroke:
                        {
                            rectSpec.StrokeColor = CssValueParser.GetActualColor(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Stroke_Width:
                        {
                            rectSpec.StrokeWidth = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Rx:
                        {
                            rectSpec.CornerRadiusX = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Ry:
                        {
                            rectSpec.CornerRadiusY = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Transform:
                        {
                            //TODO: parse svg transform function 


                        } break;
                    default:
                        {
                            //other attrs
                        } break;

                }
            }
        }
    }

    static class SvgElementPortal
    {

        public static void HandleSvgMouseDown(CssBoxSvgRoot svgBox, HtmlEventArgs e)
        {

            SvgHitChain hitChain = new SvgHitChain();
            svgBox.HitTestCore(hitChain, e.X, e.Y); 
            PropagateEventOnBubblingPhase(hitChain, e);
        }

        static void PropagateEventOnBubblingPhase(SvgHitChain hitChain, HtmlEventArgs eventArgs)
        {
            int hitCount = hitChain.Count;
            //then propagate
            for (int i = hitCount - 1; i >= 0; --i)
            {
                SvgHitInfo hitInfo = hitChain.GetHitInfo(i);
                SvgDom.SvgElement svg = hitInfo.svg;
                if (svg != null)
                {
                    BridgeHtml.HtmlElement elem = SvgDom.SvgElement.UnsafeGetController(hitInfo.svg) as BridgeHtml.HtmlElement;
                    if (elem != null)
                    {
                        elem.DispatchEvent(eventArgs);
                    }
                }
            }
        }
    }

}