// 2015,2014 ,MIT, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Text;
using PixelFarm.Agg;
using PixelFarm.Agg.VertexSource;

namespace PixelFarm.DrawingGL
{

    partial class CanvasGL2d
    {

        static double DegToRad(double degree)
        {
            return degree * (Math.PI / 180d);
        }
        static double RadToDeg(double degree)
        {
            return degree * (180d / Math.PI);
        }

        //-------------
        static unsafe void CreateRectCoords(float* arr, byte* indices,
                  float x, float y, float w, float h)
        {
            //cartesian 
            arr[0] = x; arr[1] = y;
            arr[2] = x + w; arr[3] = y;
            arr[4] = x + w; arr[5] = y + h;
            arr[6] = x; arr[7] = y + h;

            indices[0] = 0; indices[1] = 1; indices[2] = 2;
            indices[3] = 2; indices[4] = 3; indices[5] = 0;
        }

        static unsafe void CreateRectCoords(float* arr,
             float x, float y, float w, float h)
        {
            arr[0] = x; arr[1] = y;
            arr[2] = x + w; arr[3] = y;
            arr[4] = x + w; arr[5] = y + h;

            arr[6] = x + w; arr[7] = y + h;
            arr[8] = x; arr[9] = y + h;
            arr[10] = x; arr[11] = y;

        }


        List<Vertex> TessPolygon(float[] vertex2dCoords)
        {
            int ncoords = vertex2dCoords.Length / 2;
            List<Vertex> vertexts = new List<Vertex>(ncoords);
            int nn = 0;
            for (int i = 0; i < ncoords; ++i)
            {
                vertexts.Add(new Vertex(vertex2dCoords[nn++], vertex2dCoords[nn++]));
            }
            //-----------------------
            tessListener.Reset(vertexts);
            //-----------------------
            tess.BeginPolygon();
            tess.BeginContour();
            int j = vertexts.Count;
            for (int i = 0; i < j; ++i)
            {
                Vertex v = vertexts[i];
                tess.AddVertex(v.m_X, v.m_Y, 0, i);
            }
            tess.EndContour();
            tess.EndPolygon();
            return tessListener.resultVertexList;
        }


        struct CenterFormArc
        {
            public double cx;
            public double cy;
            public double radStartAngle;
            public double radSweepDiff;
            public bool scaleUp;

        }

        static void ComputeArc2(double x0, double y0,
                             double rx, double ry,
                             double xAngleRad,
                             bool largeArcFlag,
                             bool sweepFlag,
                             double x, double y, ref CenterFormArc result)
        {

            //from  SVG1.1 spec
            //----------------------------------
            //step1: Compute (x1dash,y1dash)
            //----------------------------------

            double dx2 = (x0 - x) / 2.0;
            double dy2 = (y0 - y) / 2.0;
            double cosAngle = Math.Cos(xAngleRad);
            double sinAngle = Math.Sin(xAngleRad);

            double x1 = (cosAngle * dx2 + sinAngle * dy2);
            double y1 = (-sinAngle * dx2 + cosAngle * dy2);
            // Ensure radii are large enough
            rx = Math.Abs(rx);
            ry = Math.Abs(ry);

            double prx = rx * rx;
            double pry = ry * ry;
            double px1 = x1 * x1;
            double py1 = y1 * y1;
            // check that radii are large enough


            double radiiCheck = px1 / prx + py1 / pry;
            if (radiiCheck > 1)
            {
                rx = Math.Sqrt(radiiCheck) * rx;
                ry = Math.Sqrt(radiiCheck) * ry;

                prx = rx * rx;
                pry = ry * ry;
                result.scaleUp = true;
            }

            //----------------------------------
            //step2: Compute (cx1,cy1)
            //----------------------------------
            double sign = (largeArcFlag == sweepFlag) ? -1 : 1;
            double sq = ((prx * pry) - (prx * py1) - (pry * px1)) / ((prx * py1) + (pry * px1));
            sq = (sq < 0) ? 0 : sq;
            double coef = (sign * Math.Sqrt(sq));
            double cx1 = coef * ((rx * y1) / ry);
            double cy1 = coef * -((ry * x1) / rx);


            //----------------------------------
            //step3:  Compute (cx, cy) from (cx1, cy1)
            //----------------------------------
            double sx2 = (x0 + x) / 2.0;
            double sy2 = (y0 + y) / 2.0;
            double cx = sx2 + (cosAngle * cx1 - sinAngle * cy1);
            double cy = sy2 + (sinAngle * cx1 + cosAngle * cy1);

            //----------------------------------
            //step4: Compute theta and anfkediff
            double ux = (x1 - cx1) / rx;
            double uy = (y1 - cy1) / ry;
            double vx = (-x1 - cx1) / rx;
            double vy = (-y1 - cy1) / ry;
            double p, n;
            // Compute the angle start
            n = Math.Sqrt((ux * ux) + (uy * uy));
            p = ux; // (1 * ux) + (0 * uy)
            sign = (uy < 0) ? -1d : 1d;
            double angleStart = (sign * Math.Acos(p / n));  // Math.toDegrees(sign * Math.Acos(p / n));

            // Compute the angle extent
            n = Math.Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
            p = ux * vx + uy * vy;
            sign = (ux * vy - uy * vx < 0) ? -1d : 1d;
            double angleExtent = (sign * Math.Acos(p / n));// Math.toDegrees(sign * Math.Acos(p / n));


            //if (!sweepFlag && angleExtent > 0)
            //{
            //    angleExtent -= 360f;
            //}
            //else if (sweepFlag && angleExtent < 0)
            //{
            //    angleExtent += 360f;
            //}

            result.cx = cx;
            result.cy = cy;
            result.radStartAngle = angleStart;
            result.radSweepDiff = angleExtent;

        }
        static Arc ComputeArc(double x0, double y0,
                              double rx, double ry,
                              double angle,
                              bool largeArcFlag,
                              bool sweepFlag,
                               double x, double y)
        {

            /** 
         * This constructs an unrotated Arc2D from the SVG specification of an 
         * Elliptical arc.  To get the final arc you need to apply a rotation
         * transform such as:
         * 
         * AffineTransform.getRotateInstance
         *     (angle, arc.getX()+arc.getWidth()/2, arc.getY()+arc.getHeight()/2);
         */
            //
            // Elliptical arc implementation based on the SVG specification notes
            //

            // Compute the half distance between the current and the final point
            double dx2 = (x0 - x) / 2.0;
            double dy2 = (y0 - y) / 2.0;
            // Convert angle from degrees to radians
            angle = ((angle % 360.0) * Math.PI / 180f);
            double cosAngle = Math.Cos(angle);
            double sinAngle = Math.Sin(angle);

            //
            // Step 1 : Compute (x1, y1)
            //
            double x1 = (cosAngle * dx2 + sinAngle * dy2);
            double y1 = (-sinAngle * dx2 + cosAngle * dy2);
            // Ensure radii are large enough
            rx = Math.Abs(rx);
            ry = Math.Abs(ry);
            double Prx = rx * rx;
            double Pry = ry * ry;
            double Px1 = x1 * x1;
            double Py1 = y1 * y1;
            // check that radii are large enough
            double radiiCheck = Px1 / Prx + Py1 / Pry;
            if (radiiCheck > 1)
            {
                rx = Math.Sqrt(radiiCheck) * rx;
                ry = Math.Sqrt(radiiCheck) * ry;
                Prx = rx * rx;
                Pry = ry * ry;
            }

            //
            // Step 2 : Compute (cx1, cy1)
            //
            double sign = (largeArcFlag == sweepFlag) ? -1 : 1;
            double sq = ((Prx * Pry) - (Prx * Py1) - (Pry * Px1)) / ((Prx * Py1) + (Pry * Px1));
            sq = (sq < 0) ? 0 : sq;
            double coef = (sign * Math.Sqrt(sq));
            double cx1 = coef * ((rx * y1) / ry);
            double cy1 = coef * -((ry * x1) / rx);

            //
            // Step 3 : Compute (cx, cy) from (cx1, cy1)
            //
            double sx2 = (x0 + x) / 2.0;
            double sy2 = (y0 + y) / 2.0;
            double cx = sx2 + (cosAngle * cx1 - sinAngle * cy1);
            double cy = sy2 + (sinAngle * cx1 + cosAngle * cy1);

            //
            // Step 4 : Compute the angleStart (angle1) and the angleExtent (dangle)
            //
            double ux = (x1 - cx1) / rx;
            double uy = (y1 - cy1) / ry;
            double vx = (-x1 - cx1) / rx;
            double vy = (-y1 - cy1) / ry;
            double p, n;
            // Compute the angle start
            n = Math.Sqrt((ux * ux) + (uy * uy));
            p = ux; // (1 * ux) + (0 * uy)
            sign = (uy < 0) ? -1d : 1d;
            double angleStart = (sign * Math.Acos(p / n));  // Math.toDegrees(sign * Math.Acos(p / n));

            // Compute the angle extent
            n = Math.Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
            p = ux * vx + uy * vy;
            sign = (ux * vy - uy * vx < 0) ? -1d : 1d;
            double angleExtent = (sign * Math.Acos(p / n));// Math.toDegrees(sign * Math.Acos(p / n));
            if (!sweepFlag && angleExtent > 0)
            {
                angleExtent -= 360f;
            }
            else if (sweepFlag && angleExtent < 0)
            {
                angleExtent += 360f;
            }
            //angleExtent %= 360f;
            //angleStart %= 360f;

            //
            // We can now build the resulting Arc2D in double precision
            //
            //Arc2D.Double arc = new Arc2D.Double();
            //arc.x = cx - rx;
            //arc.y = cy - ry;
            //arc.width = rx * 2.0;
            //arc.height = ry * 2.0;
            //arc.start = -angleStart;
            //arc.extent = -angleExtent;
            Arc arc = new Arc();
            arc.Init(x, y, rx, ry, -(angleStart), -(angleExtent));
            return arc;
        }






        //static unsafe void CreateLineCoords(ArrayList<VertexC4V2f> vrx,
        //         PixelFarm.Drawing.Color color,
        //         float x1, float y1, float x2, float y2)
        //{
        //    uint color_uint = color.ToABGR();
        //    vrx.AddVertex(new VertexC4V2f(color_uint, x1, y1));
        //    vrx.AddVertex(new VertexC4V2f(color_uint, x2, y2));
        //}
        //static unsafe void CreateRectCoords(ArrayList<VertexC4V2f> vrx,
        //           PixelFarm.Drawing.Color color,
        //           float x, float y, float w, float h)
        //{
        //    uint color_uint = color.ToABGR();
        //    vrx.AddVertex(new VertexC4V2f(color_uint, x, y));
        //    vrx.AddVertex(new VertexC4V2f(color_uint, x + w, y));
        //    vrx.AddVertex(new VertexC4V2f(color_uint, x + w, y + h));

        //    vrx.AddVertex(new VertexC4V2f(color_uint, x + w, y + h));
        //    vrx.AddVertex(new VertexC4V2f(color_uint, x, y + h));
        //    vrx.AddVertex(new VertexC4V2f(color_uint, x, y));

        //}
        //static unsafe void CreatePolyLineRectCoords(ArrayList<VertexC4V2f> vrx,
        //           PixelFarm.Drawing.Color color,
        //           float x, float y, float w, float h)
        //{
        //    uint color_uint = color.ToABGR();
        //    vrx.AddVertex(new VertexC4V2f(color_uint, x, y));
        //    vrx.AddVertex(new VertexC4V2f(color_uint, x + w, y));
        //    vrx.AddVertex(new VertexC4V2f(color_uint, x + w, y + h));
        //    vrx.AddVertex(new VertexC4V2f(color_uint, x, y + h));
        //    vrx.AddVertex(new VertexC4V2f(color_uint, x, y));

        //}

    }
}