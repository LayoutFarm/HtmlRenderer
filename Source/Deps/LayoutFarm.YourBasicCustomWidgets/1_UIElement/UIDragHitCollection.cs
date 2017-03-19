//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.UI
{
    public class UIDragHitCollection
    {
        List<UIElement> hitList;
        Rectangle hitArea;
        public UIDragHitCollection(List<UIElement> hitList, Rectangle hitArea)
        {
            this.hitArea = hitArea;
            this.hitList = hitList;
        }
        public Rectangle HitArea
        {
            get { return this.hitArea; }
        }
    }
}