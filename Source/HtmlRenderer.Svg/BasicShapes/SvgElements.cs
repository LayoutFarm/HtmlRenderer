//MS-PL, Apache2 
//2014, WinterDev

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using HtmlRenderer.Css;

namespace HtmlRenderer.SvgDom
{

    public abstract class SvgNode
    {
    }


    public abstract class SvgElement : SvgNode
    {
        LinkedListNode<SvgElement> linkedNode = null;
        LinkedList<SvgElement> children;
        SvgElement parent;
        object controller;

        public SvgElement(object controller)
        {
            this.controller = controller;
        }


        public SvgElement Parent
        {
            get
            {
                return parent;
            }
        }
        public void AddChild(SvgElement child)
        {
            if (this.children == null)
            {
                this.children = new LinkedList<SvgElement>();
            }
            child.linkedNode = this.children.AddLast(child);
            child.parent = this;
        }
        public int Count
        {
            get
            {
                if (this.children == null)
                {
                    return 0;
                }
                else
                {
                    return this.children.Count;
                }
            }
        }

        public LinkedListNode<SvgElement> GetFirstNode()
        {
            return this.children.First;
        }
        public virtual void ReEvaluateComputeValue(float containerW, float containerH, float emHeight)
        {

        }

        /// <summary>
        /// get length in pixel
        /// </summary>
        /// <param name="length"></param>
        /// <param name="hundredPercent"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static float ConvertToPx(CssLength length, float hundredPercent, float emHeight)
        {
            //Return zero if no length specified, zero specified      
            switch (length.UnitOrNames)
            {
                case CssUnitOrNames.EmptyValue:
                    return 0;
                case CssUnitOrNames.Percent:
                    return (length.Number / 100f) * hundredPercent;
                case CssUnitOrNames.Ems:
                    return length.Number * emHeight;
                case CssUnitOrNames.Ex:
                    return length.Number * (emHeight / 2);
                case CssUnitOrNames.Pixels:
                    //atodo: check support for hi dpi
                    return length.Number;
                case CssUnitOrNames.Milimeters:
                    return length.Number * 3.779527559f; //3 pixels per millimeter      
                case CssUnitOrNames.Centimeters:
                    return length.Number * 37.795275591f; //37 pixels per centimeter 
                case CssUnitOrNames.Inches:
                    return length.Number * 96f; //96 pixels per inch 
                case CssUnitOrNames.Points:
                    return length.Number * (96f / 72f); // 1 point = 1/72 of inch   
                case CssUnitOrNames.Picas:
                    return length.Number * 16f; // 1 pica = 12 points 
                default:
                    return 0;
            }
        }


        public virtual void Paint(HtmlRenderer.Drawing.IGraphics g)
        {

        }
        public virtual bool HitTestCore(SvgHitChain svgChain, float x, float y)
        {

            return false;
        }


        //--------------------------------
        public static object UnsafeGetController(SvgElement elem)
        {
            return elem.controller;
        }

    }


    public class SvgFragment : SvgElement
    {
        public SvgFragment()
            : base(null)
        {
        }
    }

    public abstract class SvgVisualElement : SvgElement
    {

        public SvgVisualElement(object controller)
            : base(controller)
        {

        }
        public float ActualStrokeWidth
        {
            get;
            set;
        }
    }

    public class SvgRect : SvgVisualElement
    {

        Color strokeColor = Color.Transparent;
        Color fillColor = Color.Black;
        GraphicsPath _path;
        SvgRectSpec rectSpec;
        public SvgRect(SvgRectSpec rectSpec, object controller)
            : base(controller)
        {

            this.rectSpec = rectSpec;
        }

        //----------------------------
        public float ActualX
        {
            get;
            set;
        }
        public float ActualY
        {
            get;
            set;
        }
        public float ActualWidth
        {
            get;
            set;
        }
        public float ActualHeight
        {
            get;
            set;
        }

        public float ActualCornerRx
        {
            get;
            set;
        }
        public float ActualCornerRy
        {
            get;
            set;
        }
        //----------------------------
        public override void ReEvaluateComputeValue(float containerW, float containerH, float emHeight)
        {
            var myspec = this.rectSpec;
            this.fillColor = myspec.ActualColor;
            this.strokeColor = myspec.StrokeColor;

            this.ActualX = ConvertToPx(myspec.X, containerW, emHeight);
            this.ActualY = ConvertToPx(myspec.Y, containerW, emHeight);
            this.ActualWidth = ConvertToPx(myspec.Width, containerW, emHeight);
            this.ActualHeight = ConvertToPx(myspec.Height, containerW, emHeight);
            this.ActualStrokeWidth = ConvertToPx(myspec.StrokeWidth, containerW, emHeight);

            this.ActualCornerRx = ConvertToPx(myspec.CornerRadiusX, containerW, emHeight);
            this.ActualCornerRy = ConvertToPx(myspec.CornerRadiusY, containerW, emHeight);

            //update graphic path
            if (this.ActualCornerRx == 0 && this.ActualCornerRy == 0)
            {
                this._path = CreateRectGraphicPath(
                    this.ActualX,
                    this.ActualY,
                    this.ActualWidth,
                    this.ActualHeight);
            }
            else
            {
                this._path = CreateRoundRectGraphicPath(
                    this.ActualX,
                    this.ActualY,
                    this.ActualWidth,
                    this.ActualHeight,
                    this.ActualCornerRx,
                    this.ActualCornerRy);
            }

        }
        static GraphicsPath CreateRectGraphicPath(float x, float y, float w, float h)
        {
            var _path = new GraphicsPath();
            _path.StartFigure();
            _path.AddRectangle(new RectangleF(x, y, w, h));
            _path.CloseFigure();
            return _path;
        }
        static GraphicsPath CreateRoundRectGraphicPath(float x, float y, float w, float h, float c_rx, float c_ry)
        {
            var _path = new GraphicsPath();
            var arcBounds = new RectangleF();
            var lineStart = new PointF();
            var lineEnd = new PointF();
            var width = w;
            var height = h;
            var rx = c_rx * 2;
            var ry = c_ry * 2;

            // Start
            _path.StartFigure();

            // Add first arc
            arcBounds.Location = new PointF(x, y);
            arcBounds.Width = rx;
            arcBounds.Height = ry;
            _path.AddArc(arcBounds, 180, 90);

            // Add first line
            lineStart.X = Math.Min(x + rx, x + width * 0.5f);
            lineStart.Y = y;
            lineEnd.X = Math.Max(x + width - rx, x + width * 0.5f);
            lineEnd.Y = lineStart.Y;
            _path.AddLine(lineStart, lineEnd);

            // Add second arc
            arcBounds.Location = new PointF(x + width - rx, y);
            _path.AddArc(arcBounds, 270, 90);

            // Add second line
            lineStart.X = x + width;
            lineStart.Y = Math.Min(y + ry, y + height * 0.5f);
            lineEnd.X = lineStart.X;
            lineEnd.Y = Math.Max(y + height - ry, y + height * 0.5f);
            _path.AddLine(lineStart, lineEnd);

            // Add third arc
            arcBounds.Location = new PointF(x + width - rx, y + height - ry);
            _path.AddArc(arcBounds, 0, 90);

            // Add third line
            lineStart.X = Math.Max(x + width - rx, x + width * 0.5f);
            lineStart.Y = y + height;
            lineEnd.X = Math.Min(x + rx, x + width * 0.5f);
            lineEnd.Y = lineStart.Y;
            _path.AddLine(lineStart, lineEnd);

            // Add third arc
            arcBounds.Location = new PointF(x, y + height - ry);
            _path.AddArc(arcBounds, 90, 90);

            // Add fourth line
            lineStart.X = x;
            lineStart.Y = Math.Max(y + height - ry, y + height * 0.5f);
            lineEnd.X = lineStart.X;
            lineEnd.Y = Math.Min(y + ry, y + height * 0.5f);
            _path.AddLine(lineStart, lineEnd);

            // Close
            _path.CloseFigure();

            return _path;
        }

        //------------------------------------------------
        public override bool HitTestCore(SvgHitChain svgChain, float x, float y)
        {
            if (y >= this.ActualY & y < (this.ActualY + this.ActualHeight))
            {
                if (x >= this.ActualX && x < this.ActualX + this.ActualWidth)
                {

                    svgChain.AddHit(this, x, y);
                    return true;
                }
            }
            return false;
        }
        public override void Paint(Drawing.IGraphics g)
        {

            using (SolidBrush sb = new SolidBrush(this.fillColor))
            {
                g.FillPath(sb, this._path);

            }
            if (this.strokeColor != Color.Transparent
                && this.ActualStrokeWidth > 0)
            {
                using (SolidBrush sb = new SolidBrush(this.strokeColor))
                using (Pen pen = new Pen(sb))
                {
                    pen.Width = this.ActualStrokeWidth;
                    g.DrawPath(pen, this._path);
                }
            }

        }

    }


    public class SvgCircle : SvgVisualElement
    {

        Color strokeColor = Color.Transparent;
        Color fillColor = Color.Black;
        GraphicsPath _path;
        SvgCircleSpec spec;
        public SvgCircle(SvgCircleSpec spec, object controller)
            : base(controller)
        {

            this.spec = spec;
        }

        //----------------------------
        public float ActualX
        {
            get;
            set;
        }
        public float ActualY
        {
            get;
            set;
        }
        public float ActualRadius
        {
            get;
            set;
        }

        //----------------------------
        public override void ReEvaluateComputeValue(float containerW, float containerH, float emHeight)
        {
            var myspec = this.spec;
            this.fillColor = myspec.ActualColor;
            this.strokeColor = myspec.StrokeColor;

            this.ActualX = ConvertToPx(myspec.X, containerW, emHeight);
            this.ActualY = ConvertToPx(myspec.Y, containerW, emHeight);
            this.ActualRadius = ConvertToPx(myspec.Radius, containerW, emHeight);
            this.ActualStrokeWidth = ConvertToPx(myspec.StrokeWidth, containerW, emHeight);

            _path = new GraphicsPath();
            _path.StartFigure();
            _path.AddEllipse(this.ActualX - this.ActualRadius, this.ActualY - this.ActualRadius, 2 * this.ActualRadius, 2 * ActualRadius);
            _path.CloseFigure();

        }

        //------------------------------------------------
        public override bool HitTestCore(SvgHitChain svgChain, float x, float y)
        {
            //is in circle area ?

            return false;
        }
        public override void Paint(Drawing.IGraphics g)
        {

            using (SolidBrush sb = new SolidBrush(this.fillColor))
            {
                g.FillPath(sb, this._path);

            }
            if (this.strokeColor != Color.Transparent
                && this.ActualStrokeWidth > 0)
            {
                using (SolidBrush sb = new SolidBrush(this.strokeColor))
                using (Pen pen = new Pen(sb))
                {
                    pen.Width = this.ActualStrokeWidth;
                    g.DrawPath(pen, this._path);
                }
            }

        }

    }
    public class SvgEllipse : SvgVisualElement
    {

        Color strokeColor = Color.Transparent;
        Color fillColor = Color.Black;
        GraphicsPath _path;
        SvgEllipseSpec spec;
        public SvgEllipse(SvgEllipseSpec spec, object controller)
            : base(controller)
        {

            this.spec = spec;
        }

        //----------------------------
        public float ActualX
        {
            get;
            set;
        }
        public float ActualY
        {
            get;
            set;
        }
        public float ActualRadiusX
        {
            get;
            set;
        }
        public float ActualRadiusY
        {
            get;
            set;
        }

        //----------------------------
        public override void ReEvaluateComputeValue(float containerW, float containerH, float emHeight)
        {
            var myspec = this.spec;
            this.fillColor = myspec.ActualColor;
            this.strokeColor = myspec.StrokeColor;

            this.ActualX = ConvertToPx(myspec.X, containerW, emHeight);
            this.ActualY = ConvertToPx(myspec.Y, containerW, emHeight);
            this.ActualRadiusX = ConvertToPx(myspec.RadiusX, containerW, emHeight);
            this.ActualRadiusY = ConvertToPx(myspec.RadiusY, containerW, emHeight);

            this.ActualStrokeWidth = ConvertToPx(myspec.StrokeWidth, containerW, emHeight);

            this._path = new GraphicsPath();
            _path.StartFigure();
            _path.AddEllipse(this.ActualX - this.ActualRadiusX, this.ActualY - this.ActualRadiusY, 2 * this.ActualRadiusX, 2 * this.ActualRadiusY);
            _path.CloseFigure();
        }

        //------------------------------------------------
        public override bool HitTestCore(SvgHitChain svgChain, float x, float y)
        {
            //is in circle area ?

            return false;
        }
        public override void Paint(Drawing.IGraphics g)
        {

            using (SolidBrush sb = new SolidBrush(this.fillColor))
            {
                g.FillPath(sb, this._path);

            }
            if (this.strokeColor != Color.Transparent
                && this.ActualStrokeWidth > 0)
            {
                using (SolidBrush sb = new SolidBrush(this.strokeColor))
                using (Pen pen = new Pen(sb))
                {
                    pen.Width = this.ActualStrokeWidth;
                    g.DrawPath(pen, this._path);
                }
            }

        }

    }
    public class SvgPolygon : SvgVisualElement
    {
        PointF[] pointList;
        Color strokeColor = Color.Transparent;
        Color fillColor = Color.Black;
        GraphicsPath _path;
        SvgPolygonSpec spec;

        public SvgPolygon(SvgPolygonSpec polygonSpec, object controller)
            : base(controller)
        {
            this.spec = polygonSpec;
        }
        public override void ReEvaluateComputeValue(float containerW, float containerH, float emHeight)
        {
            var myspec = this.spec;
            this.fillColor = myspec.ActualColor;
            this.strokeColor = myspec.StrokeColor;
             
            this.pointList = spec.Points.ToArray();
            
            this.ActualStrokeWidth = ConvertToPx(myspec.StrokeWidth, containerW, emHeight);

            
            this._path = new GraphicsPath();
            this._path.StartFigure();



            PointF[] plist = this.pointList;
            int lim = plist.Length - 1;
            for (int i = 0; i < lim; ++i)
            {
                //p1,p2
                _path.AddLine(
                    plist[i],
                    plist[i + 1]);
            }
            //last point
            if (lim > 0)
            {
                _path.AddLine(plist[lim], plist[0]);
            }


            this._path.CloseFigure();
        }
        public PointF[] Points
        {
            get
            {
                return pointList;
            }
        } 
        public override void Paint(Drawing.IGraphics g)
        {
            using (SolidBrush sb = new SolidBrush(this.fillColor))
            {
                g.FillPath(sb, this._path);

            }
            if (this.strokeColor != Color.Transparent
                && this.ActualStrokeWidth > 0)
            {
                using (SolidBrush sb = new SolidBrush(this.strokeColor))
                using (Pen pen = new Pen(sb))
                {
                    pen.Width = this.ActualStrokeWidth;
                    g.DrawPath(pen, this._path);
                }
            }

        }
    }
    public class SvgPolyline : SvgVisualElement
    {
        Color strokeColor = Color.Transparent;
        Color fillColor = Color.Black;
        PointF[] pointList;
        GraphicsPath _path;
        SvgPolylineSpec spec;
        public SvgPolyline(SvgPolylineSpec polylineSpec, object controller)
            : base(controller)
        {
            this.spec = polylineSpec;
        }
        public override void ReEvaluateComputeValue(float containerW, float containerH, float emHeight)
        {
            var myspec = this.spec;
            this.fillColor = myspec.ActualColor;
            this.strokeColor = myspec.StrokeColor;

            this.pointList = spec.Points.ToArray();

            this.ActualStrokeWidth = ConvertToPx(myspec.StrokeWidth, containerW, emHeight);


            this._path = new GraphicsPath(); 
            PointF[] plist = this.pointList;
            int lim = plist.Length - 1;
            for (int i = 0; i < lim; ++i)
            {
                //p1,p2
                _path.AddLine(
                    plist[i],
                    plist[i + 1]);
            } 
        }
        public override void Paint(Drawing.IGraphics g)
        {
             
            if (this.strokeColor != Color.Transparent
                && this.ActualStrokeWidth > 0)
            {
                using (SolidBrush sb = new SolidBrush(this.strokeColor))
                using (Pen pen = new Pen(sb))
                {
                    pen.Width = this.ActualStrokeWidth;
                    g.DrawPath(pen, this._path);
                }
            }

        }
    }
    public class SvgLine : SvgVisualElement
    {
        Color strokeColor = Color.Transparent;
        Color fillColor = Color.Black;
        GraphicsPath _path;

        public SvgLine(object controller)
            : base(controller)
        {
        }

        public float ActualX1
        {
            get;
            set;
        }
        public float ActualY1
        {
            get;
            set;
        }
        public float ActualX2
        {
            get;
            set;
        }
        public float ActualY2
        {
            get;
            set;
        }

    }





}