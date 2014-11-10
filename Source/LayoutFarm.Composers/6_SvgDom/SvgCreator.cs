//MS-PL, Apache2 
//2014, WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;
using HtmlRenderer.Boxes;
using HtmlRenderer.Css;
using HtmlRenderer.Composers;
using HtmlRenderer.Composers.BridgeHtml;


using HtmlRenderer.WebDom;
using LayoutFarm.UI;
using LayoutFarm.SvgDom;

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
            CreateSvgBoxContent(fragment, elementNode);

            return rootBox;

        }
        static void CreateSvgBoxContent(
          SvgElement parentElement,
          HtmlElement elementNode)
        {

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
                            CreateSvgRect(parentElement, node);
                        } break;
                    case WellKnownDomNodeName.svg_circle:
                        {
                            //sample circle from 
                            //www.svgbasics.com/shapes.html
                            CreateSvgCircle(parentElement, node);
                        } break;
                    case WellKnownDomNodeName.svg_ellipse:
                        {
                            CreateSvgEllipse(parentElement, node);
                        } break;
                    case WellKnownDomNodeName.svg_polygon:
                        {
                            CreateSvgPolygon(parentElement, node);
                        } break;
                    case WellKnownDomNodeName.svg_polyline:
                        {
                            CreateSvgPolyline(parentElement, node);
                        } break;
                    case WellKnownDomNodeName.svg_defs:
                        {
                            CreateSvgDefs(parentElement, node);
                        } break;
                    case WellKnownDomNodeName.svg_linearGradient:
                        {
                            CreateSvgLinearGradient(parentElement, node);
                        } break;
                    case WellKnownDomNodeName.svg_path:
                        {
                            CreateSvgPath(parentElement, node);
                        } break;
                    case WellKnownDomNodeName.svg_image:
                        {
                            CreateSvgImage(parentElement, node);
                        } break;
                    default:
                        {

                        } break;
                }
            }

        }
        static void CreateSvgDefs(SvgElement parentNode, HtmlElement elem)
        {
            //inside single definition
            SvgDefinitionList svgDefList = new SvgDefinitionList(elem);
            parentNode.AddChild(svgDefList);
            CreateSvgBoxContent(svgDefList, elem);
        }
        static void CreateSvgLinearGradient(SvgElement parentNode, HtmlElement elem)
        {
            //linear gradient definition

            SvgLinearGradient linearGradient = new SvgLinearGradient(elem);

            foreach (WebDom.DomAttribute attr in elem.GetAttributeIterForward())
            {
                WebDom.WellknownName wellknownName = (WebDom.WellknownName)attr.LocalNameIndex;
                switch (wellknownName)
                {
                    case WellknownName.Svg_X1:
                        {
                            linearGradient.X1 = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WellknownName.Svg_X2:
                        {
                            linearGradient.X2 = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WellknownName.Svg_Y1:
                        {
                            linearGradient.Y1 = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WellknownName.Svg_Y2:
                        {
                            linearGradient.Y2 = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                }
            }
            //------------------------------------------------------------
            int j = elem.ChildrenCount;
            List<StopColorPoint> stopColorPoints = new List<StopColorPoint>(j);
            for (int i = 0; i < j; ++i)
            {
                HtmlElement node = elem.GetChildNode(i) as HtmlElement;
                if (node == null)
                {
                    continue;
                }
                switch (node.WellknownElementName)
                {
                    case WellKnownDomNodeName.svg_stop:
                        {
                            //stop point
                            StopColorPoint stopPoint = new StopColorPoint();
                            foreach (WebDom.DomAttribute attr in node.GetAttributeIterForward())
                            {
                                WebDom.WellknownName wellknownName = (WebDom.WellknownName)attr.LocalNameIndex;
                                switch (wellknownName)
                                {
                                    case WellknownName.Svg_StopColor:
                                        {
                                            stopPoint.StopColor = CssValueParser.GetActualColor(attr.Value);
                                        } break;
                                    case WellknownName.Svg_Offset:
                                        {
                                            stopPoint.Offset = UserMapUtil.ParseGenericLength(attr.Value);
                                        } break;
                                }
                            }
                            stopColorPoints.Add(stopPoint);
                        } break;
                }
            }
        }


        static void CreateSvgRect(SvgElement parentNode, HtmlElement elem)
        {

            SvgRectSpec spec = new SvgRectSpec();
            SvgRect shape = new SvgRect(spec, elem);
            parentNode.AddChild(shape);
            foreach (WebDom.DomAttribute attr in elem.GetAttributeIterForward())
            {
                WebDom.WellknownName wellknownName = (WebDom.WellknownName)attr.LocalNameIndex;

                switch (wellknownName)
                {
                    case WebDom.WellknownName.Svg_X:
                        {
                            spec.X = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Y:
                        {
                            spec.Y = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Width:
                        {
                            spec.Width = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Height:
                        {
                            spec.Height = UserMapUtil.ParseGenericLength(attr.Value);
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
                    case WebDom.WellknownName.Svg_Rx:
                        {
                            spec.CornerRadiusX = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Ry:
                        {
                            spec.CornerRadiusY = UserMapUtil.ParseGenericLength(attr.Value);
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
        static void CreateSvgCircle(SvgElement parentNode, HtmlElement elem)
        {

            SvgCircleSpec spec = new SvgCircleSpec();
            SvgCircle shape = new SvgCircle(spec, elem);
            parentNode.AddChild(shape);
            //translate attribute            
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
        static void CreateSvgEllipse(SvgElement parentNode, HtmlElement elem)
        {

            SvgEllipseSpec spec = new SvgEllipseSpec();
            SvgEllipse shape = new SvgEllipse(spec, elem);
            parentNode.AddChild(shape);

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
        static void CreateSvgPolygon(SvgElement parentNode, HtmlElement elem)
        {

            SvgPolygonSpec spec = new SvgPolygonSpec();
            SvgPolygon shape = new SvgPolygon(spec, elem);
            parentNode.AddChild(shape);

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
        static void CreateSvgPolyline(SvgElement parentNode, HtmlElement elem)
        {

            SvgPolylineSpec spec = new SvgPolylineSpec();
            SvgPolyline shape = new SvgPolyline(spec, elem);
            parentNode.AddChild(shape);


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

        static void CreateSvgPath(SvgElement parentNode, HtmlElement elem)
        {

            SvgPathSpec spec = new SvgPathSpec();
            SvgPath svgPath = new SvgPath(spec, elem);
            parentNode.AddChild(svgPath);

            foreach (WebDom.DomAttribute attr in elem.GetAttributeIterForward())
            {
                WebDom.WellknownName wellknownName = (WebDom.WellknownName)attr.LocalNameIndex;
                switch (wellknownName)
                {
                    case WebDom.WellknownName.Svg_X:
                        {
                            spec.X = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Y:
                        {
                            spec.Y = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Width:
                        {
                            spec.Width = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Height:
                        {
                            spec.Height = UserMapUtil.ParseGenericLength(attr.Value);
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
                            switch (attr.Name)
                            {
                                case "d":
                                    {
                                        //parse vertex commands

                                        Svg.Pathing.SvgPathDataParser parser = new Svg.Pathing.SvgPathDataParser();
                                        svgPath.Segments = parser.Parse(attr.Value.ToCharArray());
                                    } break;
                            }
                        } break;

                }
            }

        }

        static void CreateSvgImage(SvgElement parentNode, HtmlElement elem)
        {
            SvgImageSpec spec = new SvgImageSpec();
            SvgImage svgImage = new SvgImage(spec, elem);
            parentNode.AddChild(svgImage);

            foreach (WebDom.DomAttribute attr in elem.GetAttributeIterForward())
            {
                WebDom.WellknownName wellknownName = (WebDom.WellknownName)attr.LocalNameIndex;
                switch (wellknownName)
                {
                    case WebDom.WellknownName.Svg_X:
                        {
                            spec.X = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Svg_Y:
                        {
                            spec.Y = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Width:
                        {
                            spec.Width = UserMapUtil.ParseGenericLength(attr.Value);
                        } break;
                    case WebDom.WellknownName.Height:
                        {
                            spec.Height = UserMapUtil.ParseGenericLength(attr.Value);
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
                    case WellknownName.Href:
                        {
                            //image src***
                            spec.ImageSrc = attr.Value;
                        }break;
                    default:
                        {
                             
                        } break;

                }
            }

        }

        public static void TranslateSvgAttributesMain(HtmlElement elem)
        {

        }
        static List<PointF> ParsePointList(string str)
        {


            //easy parse 01
            string[] allPoints = str.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            //should be even number
            int j = allPoints.Length - 1;
            if (j > 1)
            {
                var list = new List<PointF>(j / 2);
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


                    list.Add(new PointF(x, y));
                }
                return list;

            }
            else
            {
                return new List<PointF>();
            }

        }


    }

    static class SvgElementPortal
    {

        public static void HandleSvgMouseDown(CssBoxSvgRoot svgBox, UIEventArgs e)
        {

            SvgHitChain hitChain = new SvgHitChain();
            svgBox.HitTestCore(hitChain, e.X, e.Y);
            PropagateEventOnBubblingPhase(hitChain, e);
        }

        static void PropagateEventOnBubblingPhase(SvgHitChain hitChain, UIEventArgs eventArgs)
        {
            int hitCount = hitChain.Count;
            //then propagate
            for (int i = hitCount - 1; i >= 0; --i)
            {
                SvgHitInfo hitInfo = hitChain.GetHitInfo(i);
                SvgElement svg = hitInfo.svg;
                if (svg != null)
                {
                    var controller = SvgElement.UnsafeGetController(hitInfo.svg) as IEventListener;
                    if (controller != null)
                    {
                        //dispatch event 
                    }
                }
            }
        }
    }

}