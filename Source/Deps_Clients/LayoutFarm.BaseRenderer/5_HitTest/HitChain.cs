//Apache2, 2014-2017, WinterDev

using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.RenderBoxes
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
        List<HitInfo> hitList = new List<HitInfo>();
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
            hitList.Clear();
        }

        public bool IsFree
        {
            get;
            set;
        }
#if DEBUG
        public dbugHitTestTracker dbugHitTracker;
#endif
        public int Count { get { return this.hitList.Count; } }
        public HitInfo GetHitInfo(int index) { return hitList[index]; }
        public RenderElement TopMostElement
        {
            get
            {
                if (hitList.Count > 0)
                {
                    return hitList[hitList.Count - 1].hitElement;
                }
                else
                {
                    return null;
                }
            }
        }
        public void AddHitObject(RenderElement hitObject)
        {
            hitList.Add(new HitInfo(hitObject, new Point(testPointX, testPointY)));
#if DEBUG
            if (dbugHitTracker != null)
            {
                dbugHitTracker.WriteTrackNode(hitList.Count,
                    new Point(testPointX, testPointY).ToString() + " on "
                    + hitObject.ToString());
            }
#endif
        }
        public void RemoveCurrentHit()
        {
            if (hitList.Count > 0)
            {
                hitList.RemoveAt(hitList.Count - 1);
            }
        }


#if DEBUG
        public bool dbugBreak;
#endif

    }
}
