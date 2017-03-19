//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.RenderBoxes
{
    public interface IParentLink
    {
        RenderElement ParentRenderElement { get; }
        void AdjustLocation(ref Point p);
        RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point);
#if DEBUG
        string dbugGetLinkInfo();
#endif

    }
}