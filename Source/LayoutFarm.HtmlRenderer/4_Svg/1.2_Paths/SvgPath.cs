using System;
using System.Collections.Generic;
using System.Text;

using LayoutFarm.Drawing;

using Svg.Pathing;
using Svg.Transforms;

namespace LayoutFarm.SvgDom
{

    public class SvgPath : SvgVisualElement
    {
         
        Color fillColor = Color.Black;

        GraphicsPath _path;

        SvgPathSpec spec;
        List<Svg.Pathing.SvgPathSeg> segments;
        public SvgPath(SvgPathSpec spec, object controller)
            : base(controller)
        {
            this.spec = spec;
        }
        public List<Svg.Pathing.SvgPathSeg> Segments
        {
            get { return this.segments; }
            set { this.segments = value; }
        }
        public override void ReEvaluateComputeValue(float containerW, float containerH, float emHeight)
        {
            var myspec = this.spec;
            this.fillColor = myspec.ActualColor;
            if (segments == null)
            {
                this._path = null;
            }
            else
            {
                GraphicsPath gpath = this._path = CurrentGraphicPlatform.CreateGraphicPath();

                List<SvgPathSeg> segs = this.segments;
                int segcount = segs.Count;

                float lastMoveX = 0;
                float lastMoveY = 0;

                float lastX = 0;
                float lastY = 0;
                for (int i = 0; i < segcount; ++i)
                {
                    SvgPathSeg seg = segs[i];

                    switch (seg.Command)
                    {
                        case SvgPathCommand.MoveTo:
                            {
                                var moveTo = (SvgPathSegMoveTo)seg;
                                lastX = lastMoveX = moveTo.X;
                                lastY = lastMoveY = moveTo.Y;

                                gpath.StartFigure();

                            } break;
                        case SvgPathCommand.LineTo:
                            {
                                var lineTo = (SvgPathSegLineTo)seg;
                                gpath.AddLine(new PointF(lastX, lastY),
                                    new PointF(lastX = lineTo.X, lastY = lineTo.Y));

                            } break;
                        case SvgPathCommand.ZClosePath:
                            {
                                gpath.CloseFigure();

                            } break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }
        }
        public override void Paint(IGraphics g)
        { 
            using (SolidBrush sb = g.Platform.CreateSolidBrush(this.fillColor))
            {
                g.FillPath(sb, this._path);                 
            } 
        }

    }
}