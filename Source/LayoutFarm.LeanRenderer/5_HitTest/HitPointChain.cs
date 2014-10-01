//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm
{
    public struct HitPoint
    {
        public Point point;
        public IHitElement elem;

        public static readonly HitPoint Empty = new HitPoint();

        public HitPoint(IHitElement elem, Point point)
        {
            this.point = point;
            this.elem = elem;
        }
        public static bool operator ==(HitPoint pair1, HitPoint pair2)
        {
            return ((pair1.elem == pair2.elem) && (pair1.point == pair2.point));
        }
        public static bool operator !=(HitPoint pair1, HitPoint pair2)
        {
            return ((pair1.elem == pair2.elem) && (pair1.point == pair2.point));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

#if DEBUG
        public override string ToString()
        {
            return elem.ToString();
        }
#endif
    }



    public abstract class HitPointChain
    {

        protected int globalOffsetX = 0;
        protected int globalOffsetY = 0;

        int startTestX;
        int startTestY;

        protected int testPointX;
        protected int testPointY;

        public HitPointChain()
        {

        }
        public Point TestPoint
        {
            get
            {
                return new Point(testPointX, testPointY);
            }
        }
        public void GetTestPoint(out int x, out int y)
        {
            x = this.testPointX;
            y = this.testPointY;
        }
        public void SetStartTestPoint(int x, int y)
        {

            testPointX = x;
            testPointY = y;
            startTestX = x;
            startTestY = y;
        }
        public int LastestRootX
        {
            get
            {
                return startTestX;
            }
        }
        public int LastestRootY
        {
            get
            {
                return startTestY;
            }
        }
        public void OffsetTestPoint(int dx, int dy)
        {
            globalOffsetX += dx;
            globalOffsetY += dy;
            testPointX += dx;
            testPointY += dy;
        }
        public void ClearAll()
        {
            globalOffsetX = 0;
            globalOffsetY = 0;
            testPointX = 0;
            testPointY = 0;
            OnClearAll();

        }
        protected abstract void OnClearAll();


        public abstract Point PrevHitPoint { get; }
        public abstract IHitElement CurrentHitElement { get; }
        public abstract Point CurrentHitPoint { get; }

        public abstract void AddHit(IHitElement hitElement);
        
        public abstract void RemoveCurrentHitNode();

        public int LastestElementGlobalX
        {
            get
            {
                return globalOffsetX;
            }
        }
        public int LastestElementGlobalY
        {
            get
            {
                return globalOffsetY;
            }
        }

        //-----------------------------------------------------
        //element dragging feature , plan move to another place ?
        public abstract void ClearDragHitElements();
        public abstract void AddDragHitElement(IHitElement element);
        public abstract void RemoveDragHitElement(IHitElement element);
        public abstract IEnumerable<IHitElement> GetDragHitElementIter();
        public abstract int DragHitElementCount { get; }

#if DEBUG
        public bool dbugBreak;
#endif

    }

}
