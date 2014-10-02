//2014 Apache2, WinterDev
using System;
 
namespace LayoutFarm
{

    public interface IHitElement
    {
        object GetController();
        bool IsTestable();
        
        bool Contains(LayoutFarm.Drawing.Point p);
        IHitElement FindOverlapSibling(LayoutFarm.Drawing.Point p);
        bool HitTestCore(HitPointChain chain);

        LayoutFarm.Drawing.Point ElementLocation { get; }
        LayoutFarm.Drawing.Point GetElementGlobalLocation();
        LayoutFarm.Drawing.Rectangle ElementBoundRect { get; }

        bool Focusable { get; }
        bool HasParent { get; }
        bool ContainsSubChain { get; }
    }

}