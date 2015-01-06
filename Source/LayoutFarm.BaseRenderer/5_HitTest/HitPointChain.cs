//2014,2015 Apache2, WinterDev
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



    public class HitChain
    {
        List<HitInfo> currentHitChain = new List<HitInfo>();
       
        int startTestX;
        int startTestY;

        int testPointX;
        int testPointY;

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
    
        public void OffsetTestPoint(int dx, int dy)
        {
           
            testPointX += dx;
            testPointY += dy;
        }
        public void ClearAll()
        {
            
            testPointX = 0;
            testPointY = 0;
            currentHitChain.Clear();

        }

#if DEBUG
        public dbugHitTestTracker dbugHitTracker;
#endif
        public int Count { get { return this.currentHitChain.Count; } }
        public HitInfo GetHitInfo(int index) { return currentHitChain[index]; }
        public RenderElement TopMostElement
        {
            get
            {
                if (currentHitChain.Count > 0)
                {
                    return currentHitChain[currentHitChain.Count - 1].hitElement;
                }
                else
                {
                    return null;
                }
            }
        } 
        public void AddHitObject(RenderElement hitObject)
        {
            currentHitChain.Add(new HitInfo(hitObject, new Point(testPointX, testPointY)));
#if DEBUG
            dbugHitTracker.WriteTrackNode(currentHitChain.Count,
                new Point(testPointX, testPointY).ToString() + " on "
                + hitObject.ToString());
#endif
        }
        public void RemoveCurrentHit()
        {
            if (currentHitChain.Count > 0)
            {
                currentHitChain.RemoveAt(currentHitChain.Count - 1);
            }
        }

        


#if DEBUG
        public bool dbugBreak;
#endif

    }

}
