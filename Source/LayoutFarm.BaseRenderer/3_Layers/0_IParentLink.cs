// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic; 
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;

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