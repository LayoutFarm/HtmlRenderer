//MS-PL, Apache2 
//2014, WinterDev

using System;
using LayoutFarm.Drawing;
using System.Collections.Generic;

using HtmlRenderer;
using HtmlRenderer.Css;
using LayoutFarm.SvgDom;

namespace LayoutFarm.SvgDom
{
    public abstract class SvgVisualElement : SvgElement
    {
        protected Color strokeColor = Color.Transparent;
        protected Color fillColor = Color.Black;
        protected GraphicsPath myCachedPath;
        public SvgVisualElement(object controller)
            : base(controller)
        {
        }
        public float ActualStrokeWidth
        {
            get;
            set;
        }
        protected bool IsPathValid
        {
            get;
            private set;
        }
        protected void NeedReEvaluePath() { this.IsPathValid = false; }
        protected void ValidatePath() { this.IsPathValid = true; }
        protected void ClearCachePath()
        {
            if (myCachedPath != null)
            {
                myCachedPath.Dispose();
                myCachedPath = null;
            }
        }


        public Color FillColor
        {
            get { return this.fillColor; }
            set { this.fillColor = value; }
        }
        public Color StrokeColor
        {
            get { return this.strokeColor; }
            set { this.strokeColor = value; }
        }
    }

    public class SvgRect : SvgVisualElement
    {
        SvgRectSpec rectSpec;
        float actualX, actualY, actualW, actualH, cornerX, cornerY;
        public SvgRect(SvgRectSpec rectSpec, object controller)
            : base(controller)
        {
            this.rectSpec = rectSpec;
        }

        //----------------------------
        public float ActualX
        {
            get { return this.actualX; }
            set { this.actualX = value; NeedReEvaluePath(); }
        }
        public float ActualY
        {
            get { return this.actualY; }
            set { this.actualY = value; NeedReEvaluePath(); }
        }
        public float ActualWidth
        {
            get { return this.actualW; }
            set { this.actualW = value; NeedReEvaluePath(); }
        }
        public float ActualHeight
        {
            get { return this.actualH; }
            set { this.actualH = value; NeedReEvaluePath(); }
        }

        public float ActualCornerX
        {
            get { return this.cornerX; }
            set { this.cornerX = value; NeedReEvaluePath(); }
        }
        public float ActualCornerY
        {
            get { return this.cornerY; }
            set { this.cornerY = value; NeedReEvaluePath(); }
        }
        //----------------------------
        public override void ReEvaluateComputeValue(ref ReEvaluateArgs args)
        {
            var myspec = this.rectSpec;
            this.fillColor = myspec.ActualColor;
            this.strokeColor = myspec.StrokeColor;

            this.ActualX = ConvertToPx(myspec.X, ref args);
            this.ActualY = ConvertToPx(myspec.Y, ref args);
            this.ActualWidth = ConvertToPx(myspec.Width, ref args);
            this.ActualHeight = ConvertToPx(myspec.Height, ref args);
            this.ActualStrokeWidth = ConvertToPx(myspec.StrokeWidth, ref args);
            this.ActualCornerX = ConvertToPx(myspec.CornerRadiusX, ref args);
            this.ActualCornerY = ConvertToPx(myspec.CornerRadiusY, ref args);

            //update graphic path

            if (this.IsPathValid) { return; }

            ClearCachePath();

            if (this.ActualCornerX == 0 && this.ActualCornerY == 0)
            {
                this.myCachedPath = CreateRectGraphicPath(
                    args.graphicsPlatform,
                    this.ActualX,
                    this.ActualY,
                    this.ActualWidth,
                    this.ActualHeight);
            }
            else
            {
                this.myCachedPath = CreateRoundRectGraphicPath(
                    args.graphicsPlatform,
                    this.ActualX,
                    this.ActualY,
                    this.ActualWidth,
                    this.ActualHeight,
                    this.ActualCornerX,
                    this.ActualCornerY);
            }
            ValidatePath();
        }
        static GraphicsPath CreateRectGraphicPath(GraphicsPlatform gfxPlatform, float x, float y, float w, float h)
        {
            var _path = gfxPlatform.CreateGraphicsPath();
            _path.StartFigure();
            _path.AddRectangle(new RectangleF(x, y, w, h));
            _path.CloseFigure();
            return _path;
        }
        static GraphicsPath CreateRoundRectGraphicPath(GraphicsPlatform gfxPlatform, float x, float y, float w, float h, float c_rx, float c_ry)
        {
            var _path = gfxPlatform.CreateGraphicsPath();
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
        public override void Paint(Painter p)
        {

            if (fillColor.A > 0)
            {
                p.FillPath(myCachedPath, fillColor);
            }
            if (this.strokeColor.A > 0
                && this.ActualStrokeWidth > 0)
            {
                p.DrawPath(myCachedPath, strokeColor, this.ActualStrokeWidth);
            }
        }
    }


    public class SvgCircle : SvgVisualElement
    {


        SvgCircleSpec spec;

        float actualX, actualY, radius;

        public SvgCircle(SvgCircleSpec spec, object controller)
            : base(controller)
        {

            this.spec = spec;
        }

        public float ActualX
        {
            get { return this.actualX; }
            set { this.actualX = value; NeedReEvaluePath(); }
        }
        public float ActualY
        {
            get { return this.actualY; }
            set { this.actualY = value; NeedReEvaluePath(); }
        }
        public float ActualRadius
        {
            get { return this.radius; }
            set { this.radius = value; NeedReEvaluePath(); }
        }

        //----------------------------
        public override void ReEvaluateComputeValue(ref ReEvaluateArgs args)
        {
            var myspec = this.spec;
            this.fillColor = myspec.ActualColor;
            this.strokeColor = myspec.StrokeColor;

            this.ActualX = ConvertToPx(myspec.X, ref args);
            this.ActualY = ConvertToPx(myspec.Y, ref args);
            this.ActualRadius = ConvertToPx(myspec.Radius, ref args);
            this.ActualStrokeWidth = ConvertToPx(myspec.StrokeWidth, ref args);

            //create new path
            if (this.IsPathValid) { return; }
            ClearCachePath();

            myCachedPath = args.graphicsPlatform.CreateGraphicsPath();
            myCachedPath.StartFigure();
            myCachedPath.AddEllipse(this.ActualX - this.ActualRadius, this.ActualY - this.ActualRadius, 2 * this.ActualRadius, 2 * ActualRadius);
            myCachedPath.CloseFigure();

            ValidatePath();
        }

        //------------------------------------------------
        public override bool HitTestCore(SvgHitChain svgChain, float x, float y)
        {
            //is in circle area ?

            return false;
        }
        public override void Paint(Painter p)
        {
            if (fillColor.A > 0)
            {
                p.FillPath(myCachedPath, this.fillColor);
            }
            if (this.strokeColor.A > 0 && this.ActualStrokeWidth > 0)
            {
                p.DrawPath(myCachedPath, this.StrokeColor, this.ActualStrokeWidth);
            }
        }
    }
    public class SvgEllipse : SvgVisualElement
    {


        SvgEllipseSpec spec;
        float actualX, actualY, radiusX, radiusY;

        public SvgEllipse(SvgEllipseSpec spec, object controller)
            : base(controller)
        {

            this.spec = spec;
        }

        public float ActualX
        {
            get { return this.actualX; }
            set { this.actualX = value; NeedReEvaluePath(); }
        }
        public float ActualY
        {
            get { return this.actualY; }
            set { this.actualY = value; NeedReEvaluePath(); }
        }
        public float ActualRadiusX
        {
            get { return this.radiusX; }
            set { this.radiusX = value; NeedReEvaluePath(); }
        }
        public float ActualRadiusY
        {
            get { return this.radiusY; }
            set { this.radiusY = value; NeedReEvaluePath(); }
        }


        //----------------------------
        public override void ReEvaluateComputeValue(ref ReEvaluateArgs args)
        {
            var myspec = this.spec;
            this.fillColor = myspec.ActualColor;
            this.strokeColor = myspec.StrokeColor;

            this.ActualX = ConvertToPx(myspec.X, ref args);
            this.ActualY = ConvertToPx(myspec.Y, ref args);
            this.ActualRadiusX = ConvertToPx(myspec.RadiusX, ref args);
            this.ActualRadiusY = ConvertToPx(myspec.RadiusY, ref args);
            this.ActualStrokeWidth = ConvertToPx(myspec.StrokeWidth, ref args);

            //path may note need
            if (this.IsPathValid) { return; }
            ClearCachePath();
            this.myCachedPath = args.graphicsPlatform.CreateGraphicsPath();
            myCachedPath.StartFigure();
            myCachedPath.AddEllipse(this.ActualX - this.ActualRadiusX, this.ActualY - this.ActualRadiusY, 2 * this.ActualRadiusX, 2 * this.ActualRadiusY);
            myCachedPath.CloseFigure();

            ValidatePath();
        }

        //------------------------------------------------
        public override bool HitTestCore(SvgHitChain svgChain, float x, float y)
        {
            //is in circle area ?

            return false;
        }
        public override void Paint(Painter p)
        {

            if (fillColor.A > 0)
            {

                p.FillPath(myCachedPath, fillColor);
            }
            if (this.strokeColor.A > 0
                && this.ActualStrokeWidth > 0)
            {
                p.DrawPath(myCachedPath, this.strokeColor, this.ActualStrokeWidth);
            }
        }

    }
    public class SvgPolygon : SvgVisualElement
    {
        PointF[] pointList;
        SvgPolygonSpec spec;

        public SvgPolygon(SvgPolygonSpec polygonSpec, object controller)
            : base(controller)
        {
            this.spec = polygonSpec;
        }
        public override void ReEvaluateComputeValue(ref ReEvaluateArgs args)
        {
            var myspec = this.spec;
            this.fillColor = myspec.ActualColor;
            this.strokeColor = myspec.StrokeColor;
            this.pointList = spec.Points.ToArray();
            this.ActualStrokeWidth = ConvertToPx(myspec.StrokeWidth, ref args);

            if (this.IsPathValid) { return; }
            ClearCachePath();

            this.myCachedPath = args.graphicsPlatform.CreateGraphicsPath();
            this.myCachedPath.StartFigure();
            PointF[] plist = this.pointList;
            int lim = plist.Length - 1;
            for (int i = 0; i < lim; ++i)
            {
                //p1,p2
                myCachedPath.AddLine(
                    plist[i],
                    plist[i + 1]);
            }
            //last point
            if (lim > 0)
            {
                myCachedPath.AddLine(plist[lim], plist[0]);
            }


            this.myCachedPath.CloseFigure();
            ValidatePath();
        }
        public PointF[] Points
        {
            get
            {
                return pointList;
            }
        }
        public override void Paint(Painter p)
        {

            if (this.fillColor.A > 0)
            {

                p.FillPath(myCachedPath, fillColor);
            }

            if (this.strokeColor.A > 0
                && this.ActualStrokeWidth > 0)
            {
                p.DrawPath(myCachedPath, this.strokeColor, this.ActualStrokeWidth);
            }

        }
    }
    public class SvgPolyline : SvgVisualElement
    {

        PointF[] pointList;

        SvgPolylineSpec spec;
        public SvgPolyline(SvgPolylineSpec polylineSpec, object controller)
            : base(controller)
        {
            this.spec = polylineSpec;
        }
        public override void ReEvaluateComputeValue(ref ReEvaluateArgs args)
        {
            var myspec = this.spec;
            this.fillColor = myspec.ActualColor;
            this.strokeColor = myspec.StrokeColor;
            this.ActualStrokeWidth = ConvertToPx(myspec.StrokeWidth, ref args);
            this.pointList = spec.Points.ToArray();


            if (this.IsPathValid) { return; }
            ClearCachePath();

            this.myCachedPath = args.graphicsPlatform.CreateGraphicsPath();
            PointF[] plist = this.pointList;
            int lim = plist.Length - 1;
            for (int i = 0; i < lim; ++i)
            {
                //p1,p2
                myCachedPath.AddLine(
                    plist[i],
                    plist[i + 1]);
            }
            ValidatePath();
        }
        public override void Paint(Painter p)
        {

            if (this.strokeColor.A > 0
                && this.ActualStrokeWidth > 0)
            {
                p.DrawPath(myCachedPath, this.strokeColor, this.ActualStrokeWidth);
            }
        }
    }
    public class SvgLine : SvgVisualElement
    {
        float actualX1, actualY1, actualX2, actualY2;
        SvgLineSpec spec;
        public SvgLine(SvgLineSpec spec, object controller)
            : base(controller)
        {
            this.spec = spec;
        }

        //----------------------------
        public float ActualX1
        {
            get { return this.actualX1; }
            set { this.actualX1 = value; NeedReEvaluePath(); }
        }
        public float ActualY1
        {
            get { return this.actualY1; }
            set { this.actualY1 = value; NeedReEvaluePath(); }
        }
        public float ActualX2
        {
            get { return this.actualX2; }
            set { this.actualX2 = value; NeedReEvaluePath(); }
        }
        public float ActualY2
        {
            get { return this.actualY2; }
            set { this.actualY2 = value; NeedReEvaluePath(); }
        }


        public override void ReEvaluateComputeValue(ref ReEvaluateArgs args)
        {
            SvgLineSpec myspec = this.spec;
            this.fillColor = myspec.ActualColor;
            this.strokeColor = myspec.StrokeColor;
            this.ActualStrokeWidth = ConvertToPx(myspec.StrokeWidth, ref args);

            this.ActualX1 = ConvertToPx(myspec.X1, ref args);
            this.ActualY1 = ConvertToPx(myspec.Y1, ref args);
            this.ActualX2 = ConvertToPx(myspec.X2, ref args);
            this.ActualX2 = ConvertToPx(myspec.Y2, ref args);

            ValidatePath();
        }
        public override void Paint(Painter p)
        {
            if (this.strokeColor.A > 0)
            {
                p.DrawLine(
                    this.actualX1, this.actualY1,
                    this.actualX2, this.actualY2,
                    this.StrokeColor,
                    this.ActualStrokeWidth);
            }
        }
    }

    public class SvgGroupElement : SvgVisualElement
    {
        SvgVisualSpec spec;
        //'g' element

        public SvgGroupElement(SvgVisualSpec spec, object controller)
            : base(controller)
        {
            this.spec = spec;
        }
        public override void ReEvaluateComputeValue(ref ReEvaluateArgs args)
        {
            this.fillColor = spec.ActualColor;
            this.strokeColor = spec.StrokeColor;
            this.ActualStrokeWidth = ConvertToPx(spec.StrokeWidth, ref args);

            var node = this.GetFirstNode();
            while (node != null)
            {
                node.Value.ReEvaluateComputeValue(ref args);
                node = node.Next;
            }
            ValidatePath();
        }
        public override void Paint(Painter p)
        {
            p.UseCurrentContext = true;
            p.CurrentContextFillColor = spec.ActualColor;
            p.CurrentContextPenColor = spec.StrokeColor;
            p.CurrentContextPenWidth = this.ActualStrokeWidth;


            var node = this.GetFirstNode();
            while (node != null)
            {
                node.Value.Paint(p);
                node = node.Next;
            }
            p.UseCurrentContext = false;
        }
    }


}