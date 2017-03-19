//Apache2, 2014-2017, WinterDev


namespace LayoutFarm
{
    partial class RenderElement
    {
        //public Point GetLocationLimitTo(RenderElement parentHint)
        //{
        //    RenderElement parentVisualElement = this.ParentVisualElement;
        //    if (parentVisualElement == parentHint)
        //    {
        //        return new Point(this.b_left, this.b_top);
        //    }
        //    else
        //    {
        //        if (parentVisualElement != null)
        //        {
        //            Point parentPos = parentVisualElement.GetLocationLimitTo(parentHint); return new Point(b_left + parentPos.X, b_top + parentPos.Y);
        //        }
        //        else
        //        {
        //            return new Point(b_left, b_top);
        //        }
        //    }
        //} 
        //public Point GetGlobalLocationRelativeTo(RenderElement relativeElement)
        //{

        //    Point relativeElemLoca = relativeElement.Location;
        //    Point relativeElementGlobalLocation = relativeElement.GetGlobalLocation();
        //    relativeElementGlobalLocation.Offset(
        //       b_left - relativeElemLoca.X, b_top - relativeElemLoca.Y);
        //    return relativeElementGlobalLocation;
        //} 
        //public Point GetLocationAsChildOf(RenderElement relativeElement)
        //{
        //    Point relativeElementGlobalLocation = relativeElement.GetGlobalLocation();
        //    Point thisGlobalLoca = GetGlobalLocation();
        //    return new Point(thisGlobalLoca.X - relativeElementGlobalLocation.X, thisGlobalLoca.Y - relativeElementGlobalLocation.Y);
        //}
        //public Point GetLocationAsSiblingOf(RenderElement relativeElement)
        //{
        //    RenderElement parent = relativeElement.ParentVisualElement;
        //    return GetLocationAsChildOf(parent);
        //}
    }
}