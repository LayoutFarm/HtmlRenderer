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
            CssBoxSvgRoot rootBox = new CssBoxSvgRoot(
                elementNode,
                elementNode.Spec,
                fragment);
            parentBox.AppendChild(rootBox);


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
                    case WellKnownDomNodeName.svg_circle:
                        {
                            //sample circle from 
                            //www.svgbasics.com/shapes.html
                            CreateSvgCircle(fragment, node);
                        } break;
                    case WellKnownDomNodeName.svg_ellipse:
                        {
                            CreateSvgEllipse(fragment, node);
                        } break;
                    case WellKnownDomNodeName.svg_polygon:
                        {
                            CreateSvgPolygon(fragment, node);
                        } break;
                    case WellKnownDomNodeName.svg_polyline:
                        {
                            CreateSvgPolyline(fragment, node);
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

            SvgRectSpec spec = new SvgRectSpec();
            SvgRect shape = new SvgRect(spec, elem);
            //translate attribute
            TranslateSvgRectAttributes(spec, elem);
            parentNode.AddChild(shape);
        }
        static void CreateSvgCircle(SvgElement parentNode, HtmlElement elem)
        {

            SvgCircleSpec spec = new SvgCircleSpec();
            SvgCircle shape = new SvgCircle(spec, elem);
            //translate attribute
            TranslateSvgCircleAttributes(spec, elem);
            parentNode.AddChild(shape);
        }
        static void CreateSvgEllipse(SvgElement parentNode, HtmlElement elem)
        {

            SvgEllipseSpec spec = new SvgEllipseSpec();
            SvgEllipse shape = new SvgEllipse(spec, elem);
            //translate attribute
            TranslateSvgEllipseAttributes(spec, elem);
            parentNode.AddChild(shape);
        }
        static void CreateSvgPolygon(SvgElement parentNode, HtmlElement elem)
        {

            SvgPolygonSpec spec = new SvgPolygonSpec();
            SvgPolygon shape = new SvgPolygon(spec, elem);
            TranslateSvgPolygonAttributes(spec, elem);

            parentNode.AddChild(shape);
        }
        static void CreateSvgPolyline(SvgElement parentNode, HtmlElement elem)
        {

            SvgPolylineSpec spec = new SvgPolylineSpec();
            SvgPolyline shape = new SvgPolyline(spec, elem);
            TranslateSvgPolylineAttributes(spec, elem);

            parentNode.AddChild(shape);
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

        public static void TranslateSvgCircleAttributes(SvgCircleSpec spec, HtmlElement elem)
        {

            foreach (WebDom.DomAttribute attr in elem.GetAttributeIterForward())
            {
                WebDom.WellknownName wellknownName = (WebDom.WellknownName)attr.LocalNameIndex;

                switch (wellknownName)
                {
                    case WebDom.WellknownName.Svg_Cx:
                        {
                            spec.X = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Cy:
                        {
                            spec.Y = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WellknownName.Svg_R:
                        {
                            spec.Radius = UserMapUtil.ParseGenericLength(attr.Value);

                        } break;
                    case WebDom.WellknownName.Svg_Fill:
                        {
                            spec.ActualColor = CssValueParser.GetActualColor(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Stroke:
                        {
                            spec.StrokeColor = CssValueParser.GetActualColor(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Stroke_Width:
                        {
                            spec.StrokeWidth = UserMapUtil.ParseGenericLength(attr.Value);
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
        public static void TranslateSvgEllipseAttributes(SvgEllipseSpec spec, HtmlElement elem)
        {
            foreach (WebDom.DomAttribute attr in elem.GetAttributeIterForward())
            {
                WebDom.WellknownName wellknownName = (WebDom.WellknownName)attr.LocalNameIndex;

                switch (wellknownName)
                {
                    case WebDom.WellknownName.Svg_Cx:
                        {
                            spec.X = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Cy:
                        {
                            spec.Y = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WellknownName.Svg_Rx:
                        {
                            spec.RadiusX = UserMapUtil.ParseGenericLength(attr.Value);

                        } break;
                    case WellknownName.Svg_Ry:
                        {
                            spec.RadiusY = UserMapUtil.ParseGenericLength(attr.Value);

                        } break;
                    case WebDom.WellknownName.Svg_Fill:
                        {
                            spec.ActualColor = CssValueParser.GetActualColor(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Stroke:
                        {
                            spec.StrokeColor = CssValueParser.GetActualColor(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Stroke_Width:
                        {
                            spec.StrokeWidth = UserMapUtil.ParseGenericLength(attr.Value);
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
        public static void TranslateSvgPolylineAttributes(SvgPolylineSpec spec, HtmlElement elem)
        {
            foreach (WebDom.DomAttribute attr in elem.GetAttributeIterForward())
            {
                WebDom.WellknownName wellknownName = (WebDom.WellknownName)attr.LocalNameIndex;

                switch (wellknownName)
                {
                    case WebDom.WellknownName.Svg_Points:
                        {

                            //parse points
                            spec.Points = ParsePointList(attr.Value); 


                        } break;
                    case WebDom.WellknownName.Svg_Fill:
                        {
                            spec.ActualColor = CssValueParser.GetActualColor(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Stroke:
                        {
                            spec.StrokeColor = CssValueParser.GetActualColor(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Stroke_Width:
                        {
                            spec.StrokeWidth = UserMapUtil.ParseGenericLength(attr.Value);
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
        static List<System.Drawing.PointF> ParsePointList(string str)
        {


            //easy parse 01
            string[] allPoints = str.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            //should be even number
            int j = allPoints.Length - 1;
            if (j > 1)
            {
                var list = new List<System.Drawing.PointF>(j / 2);
                for (int i = 0; i < j; i += 2)
                {
                    float x, y;
                    if (!float.TryParse(allPoints[i], out x))
                    {
                        x = 0;
                    }
                    if (!float.TryParse(allPoints[i + 1], out y))
                    {
                        y = 0;
                    }


                    list.Add(new System.Drawing.PointF(x, y));
                }
                return list;

            }
            else
            {
                return new List<System.Drawing.PointF>();
            }

        }
        public static void TranslateSvgPolygonAttributes(SvgPolygonSpec spec, HtmlElement elem)
        {
            foreach (WebDom.DomAttribute attr in elem.GetAttributeIterForward())
            {
                WebDom.WellknownName wellknownName = (WebDom.WellknownName)attr.LocalNameIndex;

                switch (wellknownName)
                {
                    case WebDom.WellknownName.Svg_Points:
                        {
                            //parse points 
                            spec.Points = ParsePointList(attr.Value);

                        } break;
                    case WebDom.WellknownName.Svg_Fill:
                        {
                            spec.ActualColor = CssValueParser.GetActualColor(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Stroke:
                        {
                            spec.StrokeColor = CssValueParser.GetActualColor(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Stroke_Width:
                        {
                            spec.StrokeWidth = UserMapUtil.ParseGenericLength(attr.Value);
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