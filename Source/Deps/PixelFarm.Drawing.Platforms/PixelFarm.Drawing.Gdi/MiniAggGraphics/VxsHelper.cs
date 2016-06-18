//2016 MIT, WinterDev

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using PixelFarm.Agg;
namespace PixelFarm.Drawing.WinGdi
{
    public static class VxsHelper
    {
        static System.Drawing.Pen _pen = new System.Drawing.Pen(System.Drawing.Color.Black);
        static System.Drawing.SolidBrush _br = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        public static System.Drawing.Drawing2D.GraphicsPath CreateGraphicsPath(VertexStore vxs)
        {
            //render vertice in store
            int vcount = vxs.Count;
            double prevX = 0;
            double prevY = 0;
            double prevMoveToX = 0;
            double prevMoveToY = 0;
            var brush_path = new System.Drawing.Drawing2D.GraphicsPath(FillMode.Winding);//*** winding for overlapped path
            for (int i = 0; i < vcount; ++i)
            {
                double x, y;
                PixelFarm.Agg.VertexCmd cmd = vxs.GetVertex(i, out x, out y);
                switch (cmd)
                {
                    case PixelFarm.Agg.VertexCmd.MoveTo:
                        prevMoveToX = prevX = x;
                        prevMoveToY = prevY = y;
                        brush_path.StartFigure();
                        break;
                    case PixelFarm.Agg.VertexCmd.LineTo:
                        brush_path.AddLine((float)prevX, (float)prevY, (float)x, (float)y);
                        prevX = x;
                        prevY = y;
                        break;
                    case PixelFarm.Agg.VertexCmd.CloseAndEndFigure:
                        brush_path.AddLine((float)prevX, (float)prevY, (float)prevMoveToX, (float)prevMoveToY);
                        prevMoveToX = prevX = x;
                        prevMoveToY = prevY = y;
                        brush_path.CloseFigure();
                        break;
                    case PixelFarm.Agg.VertexCmd.EndFigure:
                        break;
                    case PixelFarm.Agg.VertexCmd.HasMore:
                        break;
                    case PixelFarm.Agg.VertexCmd.Stop:
                        i = vcount + 1;//exit from loop
                        break;
                    default:
                        throw new NotSupportedException();
                        break;
                }
            }
            return brush_path;
        }
        public static System.Drawing.Drawing2D.GraphicsPath CreateGraphicsPath(VertexStoreSnap vxsSnap)
        {
            VertexSnapIter vxsIter = vxsSnap.GetVertexSnapIter();
            double prevX = 0;
            double prevY = 0;
            double prevMoveToX = 0;
            double prevMoveToY = 0;
            var brush_path = new System.Drawing.Drawing2D.GraphicsPath(FillMode.Winding);//*** winding for overlapped path  
            for (;;)
            {
                double x, y;
                VertexCmd cmd = vxsIter.GetNextVertex(out x, out y);
                switch (cmd)
                {
                    case PixelFarm.Agg.VertexCmd.MoveTo:
                        prevMoveToX = prevX = x;
                        prevMoveToY = prevY = y;
                        brush_path.StartFigure();
                        break;
                    case PixelFarm.Agg.VertexCmd.LineTo:

                        brush_path.AddLine((float)prevX, (float)prevY, (float)x, (float)y);
                        prevX = x;
                        prevY = y;
                        break;
                    case PixelFarm.Agg.VertexCmd.CloseAndEndFigure:
                        //from current point
                        //
                        brush_path.AddLine((float)prevX, (float)prevY, (float)prevMoveToX, (float)prevMoveToY);
                        prevX = prevMoveToX;
                        prevY = prevMoveToY;
                        brush_path.CloseFigure();
                        break;
                    case PixelFarm.Agg.VertexCmd.EndFigure:
                        goto EXIT_LOOP;
                        break;
                    case PixelFarm.Agg.VertexCmd.HasMore:
                        break;
                    case PixelFarm.Agg.VertexCmd.Stop:
                        goto EXIT_LOOP;
                    default:
                        throw new NotSupportedException();
                }
            }
        EXIT_LOOP:
            return brush_path;
        }
        public static void FillVxsSnap(Graphics g, VertexStoreSnap vxsSnap, ColorRGBA c)
        {
            System.Drawing.Drawing2D.GraphicsPath p = CreateGraphicsPath(vxsSnap);
            _br.Color = ToDrawingColor(c);
            g.FillPath(_br, p);
        }
        public static void DrawVxsSnap(Graphics g, VertexStoreSnap vxsSnap, ColorRGBA c)
        {
            System.Drawing.Drawing2D.GraphicsPath p = CreateGraphicsPath(vxsSnap);
            _pen.Color = ToDrawingColor(c);
            g.DrawPath(_pen, p);
        }
        public static System.Drawing.Color ToDrawingColor(ColorRGBA c)
        {
            return System.Drawing.Color.FromArgb(c.alpha, c.red, c.green, c.blue);
        }
    }
}