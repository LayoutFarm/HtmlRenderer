//MIT 2014, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Text;
using PixelFarm.Agg;
using PixelFarm.Agg.VertexSource;

namespace LayoutFarm.DrawingGL
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

        static unsafe void CreateLineCoords(ArrayList<VertexC4V2f> vrx,
                 LayoutFarm.Drawing.Color color,
                 float x1, float y1, float x2, float y2)
        {
            uint color_uint = color.ToABGR();
            vrx.AddVertex(new VertexC4V2f(color_uint, x1, y1));
            vrx.AddVertex(new VertexC4V2f(color_uint, x2, y2));
        }
        static unsafe void CreateRectCoords(ArrayList<VertexC4V2f> vrx,
                   LayoutFarm.Drawing.Color color,
                   float x, float y, float w, float h)
        {
            uint color_uint = color.ToABGR();
            vrx.AddVertex(new VertexC4V2f(color_uint, x, y));
            vrx.AddVertex(new VertexC4V2f(color_uint, x + w, y));
            vrx.AddVertex(new VertexC4V2f(color_uint, x + w, y + h));

            vrx.AddVertex(new VertexC4V2f(color_uint, x + w, y + h));
            vrx.AddVertex(new VertexC4V2f(color_uint, x, y + h));
            vrx.AddVertex(new VertexC4V2f(color_uint, x, y));

        }
        static unsafe void CreatePolyLineRectCoords(ArrayList<VertexC4V2f> vrx,
                   LayoutFarm.Drawing.Color color,
                   float x, float y, float w, float h)
        {
            uint color_uint = color.ToABGR();
            vrx.AddVertex(new VertexC4V2f(color_uint, x, y));
            vrx.AddVertex(new VertexC4V2f(color_uint, x + w, y));
            vrx.AddVertex(new VertexC4V2f(color_uint, x + w, y + h));
            vrx.AddVertex(new VertexC4V2f(color_uint, x, y + h));
            vrx.AddVertex(new VertexC4V2f(color_uint, x, y));

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

    }
}