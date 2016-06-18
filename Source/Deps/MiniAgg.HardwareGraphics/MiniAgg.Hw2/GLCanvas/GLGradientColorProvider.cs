using System;
using PixelFarm.Agg;
namespace PixelFarm.DrawingGL
{
    static class GLGradientColorProvider
    {
        static int screenW = 800;
        static int screenH = 600;
        public static ArrayList<VertexC4V3f> CalculateLinearGradientVxs(
            float x1, float y1,
            float x2, float y2,
            PixelFarm.Drawing.Color c1,
            PixelFarm.Drawing.Color c2)
        {
            //1. gradient distance
            float dx = x2 - x1;
            float dy = y2 - y1;
            float distance = (float)Math.Pow(dx * dx + dy * dy, 0.5f);
            //find angle
            double angleRad = Math.Atan2(dy, dx);
            if (dx < 0)
            {
                //swap
                float tmpx = x2;
                x2 = x1;
                x1 = tmpx;
                float tmpy = y2;
                y2 = y1;
                y1 = tmpy;
                PixelFarm.Drawing.Color tmpc = c2;
                c2 = c1;
                c1 = tmpc;
            }

            ArrayList<VertexC4V3f> vrx = new ArrayList<VertexC4V3f>();
            //left solid rect pane 
            AddRect(vrx,
                c1.ToABGR(), c1.ToABGR(),
                -600, -800,
                x1 + 600, 1800);
            //color gradient pane 
            AddRect(vrx,
                c1.ToABGR(), c2.ToABGR(),
                x1, -800,
                distance, 1800);
            //right solid pane
            if (1200 - (x1 + distance) > 0)
            {
                AddRect(vrx,
                    c2.ToABGR(), c2.ToABGR(),
                    (x1 + distance), -800,
                    1200 - (x1 + distance), 1800);
            }
            //----------------------------------------------
            //translate vertex around x1,y1

            PixelFarm.Agg.Transform.AffinePlan[] affPlans =
                new PixelFarm.Agg.Transform.AffinePlan[]{
                   PixelFarm.Agg.Transform.AffinePlan.Translate(-x1,-y1),
                   PixelFarm.Agg.Transform.AffinePlan.Rotate(angleRad),
                   PixelFarm.Agg.Transform.AffinePlan.Translate(x1,y1)};
            var txMatrix = PixelFarm.Agg.Transform.Affine.NewMatix(affPlans);
            int j = vrx.Count;
            for (int i = j - 1; i >= 0; --i)
            {
                VertexC4V3f v = vrx[i];
                double v_x = v.x;
                double v_y = v.y;
                txMatrix.Transform(ref v_x, ref v_y);
                vrx[i] = new VertexC4V3f(v.color, (float)v_x, (float)v_y);
            }
            return vrx;
        }
        static void AddRect(ArrayList<VertexC4V3f> vrx,
            uint c1, uint c2,
            float x, float y,
            float w, float h)
        {
            //horizontal gradient
            vrx.AddVertex(new VertexC4V3f(c1, x, y));
            vrx.AddVertex(new VertexC4V3f(c2, x + w, y));
            vrx.AddVertex(new VertexC4V3f(c2, x + w, y + h));
            vrx.AddVertex(new VertexC4V3f(c2, x + w, y + h));
            vrx.AddVertex(new VertexC4V3f(c1, x, y + h));
            vrx.AddVertex(new VertexC4V3f(c1, x, y));
        }
    }
}
