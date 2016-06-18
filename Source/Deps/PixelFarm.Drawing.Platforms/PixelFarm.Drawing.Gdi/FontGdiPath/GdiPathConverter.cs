// 2015,2014 ,MIT, WinterDev   

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace PixelFarm.Agg
{
    public static class GdiPathConverter
    {
        public static void ConvertToVxs(System.Drawing.Drawing2D.PathData pathdata, VertexStore outputVxs)
        {
            byte[] pointTypes = pathdata.Types;
            PointF[] points = pathdata.Points;
            int pointCount = points.Length;
            //from MSDN document
            //0 = start of figure (MoveTo)
            //1 = one of the two endpoints of a line (LineTo)
            //3 = an endpoint or control point of a cubic Bezier spline (4 points spline)
            //masks..
            //0x7 = 111b (binary) => for masking lower 3 bits
            //0x20 = (1<<6) specific that point is a marker
            //0x80 = (1<<7) specific that point is the last point of a closed subpath( figure)

            //----------------------------------
            //convert to Agg's VertexStorage  
            int curvePointCount = 0;
            for (int i = 0; i < pointCount; ++i)
            {
                byte pointType = pointTypes[i];
                PointF p = points[i];
                switch (0x7 & pointType)
                {
                    case 0:
                        //move to
                        outputVxs.AddMoveTo(p.X, p.Y);
                        curvePointCount = 0;
                        break;
                    case 1:
                        //line to
                        outputVxs.AddLineTo(p.X, p.Y);
                        curvePointCount = 0;
                        break;
                    case 3:
                        //end point of control point of cubic Bezier spline
                        {
                            switch (curvePointCount)
                            {
                                case 0:
                                    {
                                        outputVxs.AddP2c(p.X, p.Y);
                                        curvePointCount++;
                                    }
                                    break;
                                case 1:
                                    {
                                        outputVxs.AddP3c(p.X, p.Y);
                                        curvePointCount++;
                                    }
                                    break;
                                case 2:
                                    {
                                        outputVxs.AddLineTo(p.X, p.Y);
                                        curvePointCount = 0;//reset
                                    }
                                    break;
                                default:
                                    {
                                        throw new NotSupportedException();
                                    }
                            }
                        }
                        break;
                    default:
                        {
                        }
                        break;
                }

                if ((pointType >> 7) == 1)
                {
                    //close figure to
                    outputVxs.AddCloseFigure();
                }
                if ((pointType >> 6) == 1)
                {
                }
            }
        }
        public static void ConvertCharToVertexGlyph(System.Drawing.Font font, char c, VertexStore outputVxs)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddString(c.ToString(), font.FontFamily, 1, font.Size, new Point(0, 0), null);
                //get font shape from gpath
                ConvertToVxs(path.PathData, outputVxs);
            }
        }
    }
}