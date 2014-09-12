//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using LayoutFarm.Presentation;

namespace LayoutFarm.Presentation.Grid
{
    public class GridLayer : VisualLayer
    {
        public GridLayer(RenderElement owner)
            : base(owner)
        {

        }
        public override void dbug_DumpElementProps(dbugLayoutMsgWriter writer)
        {
            throw new NotImplementedException();
        }
        public override IEnumerable<RenderElement> GetDrawingIter()
        {
            throw new NotImplementedException();
        }
        public override void AddTop(RenderElement ve)
        {
            throw new NotImplementedException();
        }

        public override bool HitTestCore(HitPointChain artHitResult)
        {
            throw new NotImplementedException();
        }
        public override void Clear()
        {
            throw new NotImplementedException();
        }
        public override void DrawChildContent(CanvasBase canvasPage, InternalRect updateArea)
        {
            throw new NotImplementedException();
        }
        public override IEnumerable<RenderElement> GetVisualElementReverseIter()
        {
            throw new NotImplementedException();
        }
        public override void TopDownReCalculateContentSize()
        {
            throw new NotImplementedException();
        }
        public override IEnumerable<RenderElement> GetVisualElementIter()
        {
            throw new NotImplementedException();
        }
        public override void TopDownReArrangeContent()
        {
            throw new NotImplementedException();
        }
        public override bool PrepareDrawingChain(VisualDrawingChain chain)
        {
            throw new NotImplementedException();
        }
    }

}