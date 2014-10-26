﻿//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm
{
    public struct HitInfo
    {
        public readonly Point point;
        public readonly RenderElement hitElement;
        public static readonly HitInfo Empty = new HitInfo();
        public HitInfo(RenderElement hitObject, Point point)
        {
            this.point = point;
            this.hitElement = hitObject; 
        }

        public static bool operator ==(HitInfo pair1, HitInfo pair2)
        {
            return ((pair1.hitElement == pair2.hitElement) && (pair1.point == pair2.point));
        }
        public static bool operator !=(HitInfo pair1, HitInfo pair2)
        {
            return ((pair1.hitElement == pair2.hitElement) && (pair1.point == pair2.point));
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
            return hitElement.ToString();
        }
#endif
    }



    public abstract class HitChain
    {

        protected int globalOffsetX = 0;
        protected int globalOffsetY = 0;

        int startTestX;
        int startTestY;

        protected int testPointX;
        protected int testPointY;

        public HitChain()
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

        public abstract int Count { get; }
        public abstract HitInfo GetHitInfo(int index);

        //public abstract Point PrevHitPoint { get; }
        public abstract RenderElement CurrentHitElement { get; }
        public abstract Point CurrentHitPoint { get; }

       
        public abstract void AddHitObject(RenderElement hitObject);
        public abstract void RemoveCurrentHit();

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
        

#if DEBUG
        public bool dbugBreak;
#endif

    }

}
