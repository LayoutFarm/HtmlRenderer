//Apache2, 2014-present, WinterDev
//MS-PL, Apache2 some parts derived from github.com/vvvv/svg 

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.Composers;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.WebDom;
using PaintLab.Svg;
using LayoutFarm.Css;

namespace LayoutFarm.Svg
{

    
    class SvgImage : SvgElement
    {
        public SvgImage(SvgImageSpec spec, object controller)
            : base(WellknownSvgElementName.Image, spec)
        {
        }
    }
    class SvgPath : SvgElement
    {
        public SvgPath(SvgPathSpec spec, object controller)
            : base(WellknownSvgElementName.Path, spec)
        {
        }
    }
    class SvgRect : SvgElement
    {
        public SvgRect(SvgRectSpec spec, object controller)
            : base(WellknownSvgElementName.Rect, spec)
        {
        }
    }
    class SvgCircle : SvgElement
    {
        public SvgCircle(SvgCircleSpec spec, object controller)
            : base(WellknownSvgElementName.Circle, spec)
        {
        }
    }
    class SvgEllipse : SvgElement
    {
        public SvgEllipse(SvgEllipseSpec spec, object controller)
            : base(WellknownSvgElementName.Ellipse, spec)
        {
        }
    }
    class SvgPolygon : SvgElement
    {
        public SvgPolygon(SvgPolygonSpec spec, object controller)
            : base(WellknownSvgElementName.Polygon, spec)
        {
        }
    }
    class SvgPolyline : SvgElement
    {
        public SvgPolyline(SvgPolylineSpec spec, object controller)
            : base(WellknownSvgElementName.Polyline, spec)
        {
        }
    }

    class SvgDefinitionList : SvgElement
    {
        public SvgDefinitionList(object controller)
            : base(WellknownSvgElementName.Defs)
        {

        }
    }
    class SvgGroupElement : SvgElement
    {
        public SvgGroupElement()
           : base(WellknownSvgElementName.Group)
        {
        }
        public SvgGroupElement(SvgVisualSpec spec, object controller)
            : base(WellknownSvgElementName.Group, spec)
        {

        }
    }
    class SvgLinearGradient : SvgElement
    {
        public SvgLinearGradient(object controller)
            : base(WellknownSvgElementName.Gradient)
        {
        }
        public List<StopColorPoint> StopList { get; set; }
        public CssLength X1 { get; set; }
        public CssLength Y1 { get; set; }
        public CssLength X2 { get; set; }
        public CssLength Y2 { get; set; }
    }


    class SvgCreator
    {
        public CssBoxSvgRoot CreateSvgBox(CssBox parentBox,
            HtmlElement elementNode,
            Css.BoxSpec spec)
        {
            SvgDocument svgdoc = new SvgDocument();
            CreateSvgBoxContent(svgdoc.Root, elementNode);

            SvgRootEventPortal svgRootController = new SvgRootEventPortal(elementNode);
            CssBoxSvgRoot svgRoot = new CssBoxSvgRoot(
                elementNode.Spec,
                parentBox.RootGfx,
                svgdoc);
            svgRoot.SetController(svgRootController);
            svgRootController.SvgRoot = svgRoot;
            parentBox.AppendChild(svgRoot);

            return svgRoot;
        }
        void CreateSvgBoxContent(SvgElement parentElement, HtmlElement elementNode)
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
                        }
                        break;
                    case WellKnownDomNodeName.svg_circle:
                        {
                            //sample circle from 
                            //www.svgbasics.com/shapes.html
                            CreateSvgCircle(parentElement, node);
                        }
                        break;
                    case WellKnownDomNodeName.svg_ellipse:
                        {
                            CreateSvgEllipse(parentElement, node);
                        }
                        break;
                    case WellKnownDomNodeName.svg_polygon:
                        {
                            CreateSvgPolygon(parentElement, node);
                        }
                        break;
                    case WellKnownDomNodeName.svg_polyline:
                        {
                            CreateSvgPolyline(parentElement, node);
                        }
                        break;
                    case WellKnownDomNodeName.svg_defs:
                        {
                            CreateSvgDefs(parentElement, node);
                        }
                        break;
                    case WellKnownDomNodeName.svg_linearGradient:
                        {
                            CreateSvgLinearGradient(parentElement, node);
                        }
                        break;
                    case WellKnownDomNodeName.svg_path:
                        {
                            CreateSvgPath(parentElement, node);
                        }
                        break;
                    case WellKnownDomNodeName.svg_image:
                        {
                            CreateSvgImage(parentElement, node);
                        }
                        break;
                    case WellKnownDomNodeName.svg_g:
                        {
                            CreateSvgGroupElement(parentElement, node);
                        }
                        break;
                    default:
                        {
                        }
                        break;
                }
            }
        }
        void CreateSvgGroupElement(SvgElement parentNode, HtmlElement elem)
        {
            SvgVisualSpec spec = new SvgVisualSpec();
            SvgGroupElement svgGroupElement = new SvgGroupElement(spec, elem);
            parentNode.AddChild(svgGroupElement);
            foreach (WebDom.DomAttribute attr in elem.GetAttributeIterForward())
            {
                WebDom.WellknownName wellknownName = (WebDom.WellknownName)attr.LocalNameIndex;
                switch (wellknownName)
                {
                    case WebDom.WellknownName.Svg_Fill:
                        {
                            spec.FillColor = CssValueParser2.ParseCssColor(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Stroke:
                        {
                            spec.StrokeColor = CssValueParser2.ParseCssColor(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Stroke_Width:
                        {
                            spec.StrokeWidth = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    default:
                        {
                            //other attrs
                        }
                        break;
                }
            }

            CreateSvgBoxContent(svgGroupElement, elem);
        }
        void CreateSvgDefs(SvgElement parentNode, HtmlElement elem)
        {
            //inside single definition
            SvgDefinitionList svgDefList = new SvgDefinitionList(elem);
            parentNode.AddChild(svgDefList);
            CreateSvgBoxContent(svgDefList, elem);
        }
        void CreateSvgLinearGradient(SvgElement parentNode, HtmlElement elem)
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
                        }
                        break;
                    case WellknownName.Svg_X2:
                        {
                            linearGradient.X2 = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WellknownName.Svg_Y1:
                        {
                            linearGradient.Y1 = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WellknownName.Svg_Y2:
                        {
                            linearGradient.Y2 = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
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
                                            stopPoint.StopColor = CssValueParser2.ParseCssColor(attr.Value);
                                        }
                                        break;
                                    case WellknownName.Svg_Offset:
                                        {
                                            stopPoint.Offset = UserMapUtil.ParseGenericLength(attr.Value);
                                        }
                                        break;
                                }
                            }
                            stopColorPoints.Add(stopPoint);
                        }
                        break;
                }
            }
        }


        void CreateSvgRect(SvgElement parentNode, HtmlElement elem)
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
                        }
                        break;
                    case WebDom.WellknownName.Svg_Y:
                        {
                            spec.Y = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Width:
                        {
                            spec.Width = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Height:
                        {
                            spec.Height = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Fill:
                        {
                            spec.FillColor = CssValueParser2.ParseCssColor(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Stroke:
                        {
                            spec.StrokeColor = CssValueParser2.ParseCssColor(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Stroke_Width:
                        {
                            spec.StrokeWidth = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Rx:
                        {
                            spec.CornerRadiusX = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Ry:
                        {
                            spec.CornerRadiusY = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Transform:
                        {
                            //TODO: parse svg transform function  
                        }
                        break;
                    default:
                        {
                            //other attrs
                        }
                        break;
                }
            }
        }
        void CreateSvgCircle(SvgElement parentNode, HtmlElement elem)
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
                        }
                        break;
                    case WebDom.WellknownName.Svg_Cy:
                        {
                            spec.Y = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WellknownName.Svg_R:
                        {
                            spec.Radius = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Fill:
                        {
                            spec.FillColor = CssValueParser2.ParseCssColor(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Stroke:
                        {
                            spec.StrokeColor = CssValueParser2.ParseCssColor(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Stroke_Width:
                        {
                            spec.StrokeWidth = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Transform:
                        {
                            //TODO: parse svg transform function  
                        }
                        break;
                    default:
                        {
                            //other attrs
                        }
                        break;
                }
            }
        }
        void CreateSvgEllipse(SvgElement parentNode, HtmlElement elem)
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
                        }
                        break;
                    case WebDom.WellknownName.Svg_Cy:
                        {
                            spec.Y = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WellknownName.Svg_Rx:
                        {
                            spec.RadiusX = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WellknownName.Svg_Ry:
                        {
                            spec.RadiusY = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Fill:
                        {
                            spec.FillColor = CssValueParser2.ParseCssColor(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Stroke:
                        {
                            spec.StrokeColor = CssValueParser2.ParseCssColor(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Stroke_Width:
                        {
                            spec.StrokeWidth = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Transform:
                        {
                            //TODO: parse svg transform function  
                        }
                        break;
                    default:
                        {
                            //other attrs
                        }
                        break;
                }
            }
        }
        void CreateSvgPolygon(SvgElement parentNode, HtmlElement elem)
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
                        }
                        break;
                    case WebDom.WellknownName.Svg_Fill:
                        {
                            spec.FillColor = CssValueParser2.ParseCssColor(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Stroke:
                        {
                            spec.StrokeColor = CssValueParser2.ParseCssColor(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Stroke_Width:
                        {
                            spec.StrokeWidth = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Transform:
                        {
                            //TODO: parse svg transform function  
                        }
                        break;
                    default:
                        {
                            //other attrs
                        }
                        break;
                }
            }
        }
        void CreateSvgPolyline(SvgElement parentNode, HtmlElement elem)
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
                        }
                        break;
                    case WebDom.WellknownName.Svg_Fill:
                        {
                            spec.FillColor = CssValueParser2.ParseCssColor(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Stroke:
                        {
                            spec.StrokeColor = CssValueParser2.ParseCssColor(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Stroke_Width:
                        {
                            spec.StrokeWidth = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Transform:
                        {
                            //TODO: parse svg transform function  
                        }
                        break;
                    default:
                        {
                            //other attrs
                        }
                        break;
                }
            }
        }






        void CreateSvgPath(SvgElement parentNode, HtmlElement elem)
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
                        }
                        break;
                    case WebDom.WellknownName.Svg_Y:
                        {
                            spec.Y = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Width:
                        {
                            spec.Width = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Height:
                        {
                            spec.Height = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Fill:
                        {
                            spec.FillColor = CssValueParser2.ParseCssColor(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Stroke:
                        {
                            spec.StrokeColor = CssValueParser2.ParseCssColor(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Stroke_Width:
                        {
                            spec.StrokeWidth = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Transform:
                        {
                            //TODO: parse svg transform function   


                        }
                        break;
                    default:
                        {
                            //other attrs
                            switch (attr.Name)
                            {
                                case "d":
                                    {
                                        //parse vertex commands                                          
                                        spec.D = attr.Value;
                                    }
                                    break;
                            }
                        }
                        break;
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
                        }
                        break;
                    case WebDom.WellknownName.Svg_Y:
                        {
                            spec.Y = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Width:
                        {
                            spec.Width = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Height:
                        {
                            spec.Height = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Fill:
                        {
                            spec.FillColor = CssValueParser.GetActualColor(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Stroke:
                        {
                            spec.StrokeColor = CssValueParser.GetActualColor(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Stroke_Width:
                        {
                            spec.StrokeWidth = UserMapUtil.ParseGenericLength(attr.Value);
                        }
                        break;
                    case WebDom.WellknownName.Svg_Transform:
                        {
                            //TODO: parse svg transform function    
                        }
                        break;
                    case WellknownName.Href:
                        {
                            //image src***
                            spec.ImageSrc = attr.Value;
                        }
                        break;
                    default:
                        {
                        }
                        break;
                }
            }
        }

        public static void TranslateSvgAttributesMain(HtmlElement elem)
        {

        }
        static PointF[] ParsePointList(string str)
        {
            //
            List<PointF> output = new List<PointF>();
            ParsePointList(str, output);
            return output.ToArray();
        }

        static void ParsePointList(string str, List<PointF> output)
        {
            //easy parse 01
            string[] allPoints = str.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            //should be even number
            int j = allPoints.Length - 1;
            if (j > 1)
            {

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
                    output.Add(new PointF(x, y));
                }
            }
        }
    }
}