//2014,2015,  Apache2, WinterDev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm;
using LayoutFarm.Css;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.HtmlBoxes
{
    static class PainterStock2
    {
        internal static PaintVisitor GetSharedPainter(HtmlContainer htmlCont, Canvas canvas)
        {
            PaintVisitor painter = null;
            if (painterStock.Count == 0)
            {
                painter = new PaintVisitor();
            }
            else
            {
                painter = painterStock.Dequeue();
            }

            painter.Bind(htmlCont, canvas);

            return painter;
        }
        internal static void ReleaseSharedPainter(PaintVisitor p)
        {
            p.UnBind();
            painterStock.Enqueue(p);
        }
        static Queue<PaintVisitor> painterStock = new Queue<PaintVisitor>();
    }
}