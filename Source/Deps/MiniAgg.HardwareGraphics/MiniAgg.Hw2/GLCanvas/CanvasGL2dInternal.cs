//MIT 2014, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Agg;
namespace PixelFarm.DrawingGL
{
    public partial class CanvasGL2d
    {
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
        static unsafe void CreateLineCoords(ArrayList<VertexC4V3f> vrx,
                 PixelFarm.Drawing.Color color,
                 float x1, float y1, float x2, float y2)
        {
            uint color_uint = color.ToABGR();
            vrx.AddVertex(new VertexC4V3f(color_uint, x1, y1));
            vrx.AddVertex(new VertexC4V3f(color_uint, x2, y2));
        }
        static unsafe void CreateRectCoords(ArrayList<VertexC4V3f> vrx,
                   PixelFarm.Drawing.Color color,
                   float x, float y, float w, float h)
        {
            uint color_uint = color.ToABGR();
            vrx.AddVertex(new VertexC4V3f(color_uint, x, y));
            vrx.AddVertex(new VertexC4V3f(color_uint, x + w, y));
            vrx.AddVertex(new VertexC4V3f(color_uint, x + w, y + h));
            vrx.AddVertex(new VertexC4V3f(color_uint, x + w, y + h));
            vrx.AddVertex(new VertexC4V3f(color_uint, x, y + h));
            vrx.AddVertex(new VertexC4V3f(color_uint, x, y));
        }
        static unsafe void CreateRectCoords(CoordList2f coordList,
                  float x, float y, float w, float h)
        {
            coordList.AddCoord(x, y);
            coordList.AddCoord(x + w, y);
            coordList.AddCoord(x + w, y + h);
            coordList.AddCoord(x + w, y + h);
            coordList.AddCoord(x, y + h);
            coordList.AddCoord(x, y);
        }
        static unsafe void CreatePolyLineRectCoords(CoordList2f coords,
                  float x, float y, float w, float h)
        {
            coords.AddCoord(x, y);
            coords.AddCoord(x + w, y);
            coords.AddCoord(x + w, y + h);
            coords.AddCoord(x, y + h);
            coords.AddCoord(x, y);
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

        //---test only ----
        void DrawLineAgg(float x1, float y1, float x2, float y2)
        {
            ps.Clear();
            ps.MoveTo(x1, y1);
            ps.LineTo(x2, y2);
            VertexStore vxs = aggStroke.MakeVxs(ps.Vxs);
            int n = vxs.Count;
            unsafe
            {
                float* coords = stackalloc float[(n * 2)];
                int i = 0;
                int nn = 0;
                int npoints = 0;
                double vx, vy;
                var cmd = vxs.GetVertex(i, out vx, out vy);
                while (i < n)
                {
                    switch (cmd)
                    {
                        case VertexCmd.MoveTo:
                            {
                                coords[nn] = (float)vx;
                                coords[nn + 1] = (float)vy;
                                nn += 2;
                                npoints++;
                            }
                            break;
                        case VertexCmd.LineTo:
                            {
                                coords[nn] = (float)vx;
                                coords[nn + 1] = (float)vy;
                                nn += 2;
                                npoints++;
                            }
                            break;
                        case VertexCmd.Stop:
                            {
                            }
                            break;
                        default:
                            {
                            }
                            break;
                    }
                    i++;
                    cmd = vxs.GetVertex(i, out vx, out vy);
                }
                throw new NotSupportedException();
                ////--------------------------------------
                //GL.EnableClientState(ArrayCap.VertexArray); //***
                ////vertex 2d
                //GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)coords);
                //GL.DrawArrays(BeginMode.LineLoop, 0, npoints); 
                //GL.DisableClientState(ArrayCap.VertexArray);
                //--------------------------------------
            }
        }

        unsafe void DrawPolygonUnsafe(float* polygon2dVertices, int npoints)
        {
            //GL.EnableClientState(ArrayCap.VertexArray); //***
            ////vertex 2d 
            //GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)polygon2dVertices);
            //GL.DrawArrays(BeginMode.LineLoop, 0, npoints);
            //GL.DisableClientState(ArrayCap.VertexArray);

            this.basicShader.DrawLineLoopWithVertexBuffer(polygon2dVertices, npoints, this.strokeColor);
        }
    }
}