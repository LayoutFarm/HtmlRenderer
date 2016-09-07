//MIT, 2014-2016,WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.Agg;
namespace OpenTkEssTest
{
    [Info(OrderCode = "109")]
    [Info("T109_LionFillWithRenderVx")]
    public class T109_LionFillWithRenderVx : PrebuiltGLControlDemoBase
    {
        CanvasGL2d canvas2d;
        SpriteShape lionShape;
        VertexStore lionVxs;
        GLCanvasPainter painter;
        List<RenderVx> lionRenderVxList = new List<RenderVx>();
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
            lionShape = new SpriteShape();
            lionShape.ParseLion();
            //flip this lion vertically before use with openGL
            PixelFarm.Agg.Transform.Affine aff = PixelFarm.Agg.Transform.Affine.NewMatix(
                 PixelFarm.Agg.Transform.AffinePlan.Scale(1, -1),
                 PixelFarm.Agg.Transform.AffinePlan.Translate(0, 600));
            lionVxs = aff.TransformToVxs(lionShape.Path.Vxs);
            painter = new GLCanvasPainter(canvas2d, max, max);
            //convert lion vxs to renderVx

            int j = lionShape.NumPaths;
            int[] pathList = lionShape.PathIndexList;
            for (int i = 0; i < j; ++i)
            {
                lionRenderVxList.Add(painter.CreateRenderVx(new VertexStoreSnap(lionVxs, pathList[i])));
            }
        }
        protected override void DemoClosing()
        {
            canvas2d.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            canvas2d.ClearColorBuffer();
            //-------------------------------

            int j = lionRenderVxList.Count;
            Color[] colors = lionShape.Colors;
            for (int i = 0; i < j; ++i)
            {
                canvas2d.FillRenderVx(colors[i], lionRenderVxList[i]);
            }
            //-------------------------------
            miniGLControl.SwapBuffers();
        }
    }
}

