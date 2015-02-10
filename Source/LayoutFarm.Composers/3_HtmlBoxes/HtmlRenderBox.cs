//2014,2015,2015 Apache2, WinterDev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.WebDom;
using LayoutFarm;
using LayoutFarm.Css;
using LayoutFarm.ContentManagers;
using LayoutFarm.Composers;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.HtmlBoxes
{

    public class HtmlRenderBox : RenderBoxBase
    {
        MyHtmlContainer myHtmlContainer;

        public HtmlRenderBox(RootGraphic rootgfx,
            int width, int height,
            MyHtmlContainer htmlCont)
            : base(rootgfx, width, height)
        {

            this.myHtmlContainer = htmlCont;
        }
        public override void ClearAllChildren()
        {

        }
        protected override void DrawContent(Canvas canvas, Rectangle updateArea)
        {
            myHtmlContainer.CheckDocUpdate();
            var painter = PainterStock.GetSharedPainter(myHtmlContainer, canvas);

            painter.SetViewportSize(this.Width, this.Height);

            int vwX, vwY;
            painter.OffsetCanvasOrigin(vwX = this.ViewportX, vwY = this.ViewportY);

            myHtmlContainer.PerformPaint(painter);

            painter.OffsetCanvasOrigin(-vwX, -vwY);

            PainterStock.ReleaseSharedPainter(painter);
        }
        public override void ChildrenHitTestCore(HitChain hitChain)
        {
        }
    }

    //===================================================================
    public class HtmlFragmentRenderBox : RenderBoxBase
    {

        MyHtmlContainer myHtmlCont;
        CssBox cssBox;

        public HtmlFragmentRenderBox(RootGraphic rootgfx,
            int width, int height)
            : base(rootgfx, width, height)
        {
        }

        public CssBox CssBox
        {
            get { return this.cssBox; }
        }
        public void SetHtmlContainer(MyHtmlContainer htmlCont, CssBox box)
        {
            this.myHtmlCont = htmlCont;
            this.cssBox = box;

        }
        public override void ClearAllChildren()
        {

        }
        protected override void DrawContent(Canvas canvas, Rectangle updateArea)
        {
            myHtmlCont.CheckDocUpdate();

            var painter = PainterStock.GetSharedPainter(this.myHtmlCont, canvas);
            painter.SetViewportSize(this.Width, this.Height);
#if DEBUG
            painter.dbugDrawDiagonalBox(Color.Blue, this.X, this.Y, this.Width, this.Height);
#endif
            int vwX, vwY;

            painter.OffsetCanvasOrigin(vwX = this.ViewportX, vwY = -this.ViewportY);

            myHtmlCont.PerformPaint(painter);

            painter.OffsetCanvasOrigin(-vwX, -vwY);

            PainterStock.ReleaseSharedPainter(painter);
        }
        public override void ChildrenHitTestCore(HitChain hitChain)
        {

        } 
    }

    static class PainterStock
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





