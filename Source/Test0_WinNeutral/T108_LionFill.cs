//MIT, 2014-2016,WinterDev

using System;
using PixelFarm.Drawing;
using PixelFarm.DrawingGL;
using PixelFarm.Agg;
namespace OpenTkEssTest
{

    public class T108_LionFill : SampleBase
    {
        CanvasGL2d canvas2d;
        SpriteShape lionShape;
        VertexStore lionVxs;
        GLCanvasPainter painter;
        public T108_LionFill()
        {

        }

        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            int max = Math.Max(this.Width, this.Height);
            canvas2d = CreateCanvasGL2d(max, max);
            lionShape = new SpriteShape();
            lionShape.ParseLion();
            //flip this lion vertically before use with openGL
            PixelFarm.Agg.Transform.Affine aff = PixelFarm.Agg.Transform.Affine.NewMatix(
                 PixelFarm.Agg.Transform.AffinePlan.Scale(1, -1),
                 PixelFarm.Agg.Transform.AffinePlan.Translate(0, 600));
            lionVxs = new VertexStore();
            aff.TransformToVxs(lionShape.Path.Vxs, lionVxs);
            painter = new GLCanvasPainter(canvas2d, max, max);
            SetPrinter(painter);
        }
        public void Init2(CanvasGL2d canvas2d)
        {
            int max = 800;
            this.canvas2d = canvas2d;
            lionShape = new SpriteShape();
            lionShape.ParseLion();
            //flip this lion vertically before use with openGL
            PixelFarm.Agg.Transform.Affine aff = PixelFarm.Agg.Transform.Affine.NewMatix(
                 PixelFarm.Agg.Transform.AffinePlan.Scale(1, -1),
                 PixelFarm.Agg.Transform.AffinePlan.Translate(0, 600));
            lionVxs = new VertexStore();
            aff.TransformToVxs(lionShape.Path.Vxs, lionVxs);
            painter = new GLCanvasPainter(canvas2d, max, max);
            SetPrinter(painter);
        }
        public GLCanvasPainter Painter { get { return this.painter; } }

        protected override void DemoClosing()
        {
            canvas2d.Dispose();
        }
        public void TestRender()
        {
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            //------------------------------- 
            int j = lionShape.NumPaths;
            int[] pathList = lionShape.PathIndexList;
            Color[] colors = lionShape.Colors;
            VertexStore myvxs = lionVxs;
            for (int i = 0; i < j; ++i)
            {
                painter.FillColor = colors[i];
                painter.Fill(new VertexStoreSnap(myvxs, pathList[i]));
            }

            painter.FillColor = Color.Red;
            painter.StrokeWidth = 1;
            painter.FillRectangle(10, 10, 50, 50);
            //-------------------------------
            SwapBuffer();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            canvas2d.ClearColorBuffer();
            //------------------------------- 
            int j = lionShape.NumPaths;
            int[] pathList = lionShape.PathIndexList;
            Color[] colors = lionShape.Colors;
            VertexStore myvxs = lionVxs;
            for (int i = 0; i < j; ++i)
            {
                painter.FillColor = colors[i];
                painter.Fill(new VertexStoreSnap(myvxs, pathList[i]));
            }

            painter.FillColor = Color.Red;
            painter.StrokeWidth = 1;
            painter.FillRectangle(10, 10, 50, 50);
            //-------------------------------
            SwapBuffer();
        }
    }
}

