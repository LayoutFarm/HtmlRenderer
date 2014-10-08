//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{
    class MyHitChain : HitChain
    {
        List<HitPoint> currentHitChain;
        List<HitPoint> prevHitChain;
        readonly List<HitPoint> hitChainA = new List<HitPoint>();
        readonly List<HitPoint> hitChainB = new List<HitPoint>();


        public MyHitChain()
        {
            currentHitChain = hitChainA;
            prevHitChain = hitChainB;
        }
        protected override void OnClearAll()
        {
            currentHitChain.Clear();
            prevHitChain.Clear();
        }
        public override void RemoveCurrentHit()
        {
            if (currentHitChain.Count > 0)
            {
                currentHitChain.RemoveAt(currentHitChain.Count - 1);
            }
        }
        public override int Count
        {
            get { return this.currentHitChain.Count; }
        }
        public override HitPoint GetHitPoint(int index)
        {
            return currentHitChain[index];
        }
        public override Point PrevHitPoint
        {
            get
            {
                if (prevHitChain.Count > 0)
                {
                    //?
                    return prevHitChain[prevHitChain.Count - 1].point;
                }
                else
                {
                    return Point.Empty;
                }
            }
        }
        public override Point CurrentHitPoint
        {
            get { return currentHitChain[currentHitChain.Count - 1].point; }
        }
        public override object CurrentHitElement
        {
            get
            {
                if (currentHitChain.Count > 0)
                {
                    return currentHitChain[currentHitChain.Count - 1].hitObject as RenderElement;
                }
                else
                {
                    return null;
                }
            }
        }
        public override void AddHitObject(object hitObject)
        {
            currentHitChain.Add(new HitPoint(hitObject, new Point(testPointX, testPointY)));
#if DEBUG
            dbugHitTracker.WriteTrackNode(currentHitChain.Count,
                new Point(testPointX, testPointY).ToString() + " on "
                + hitObject.ToString());
#endif
        }

        public void SwapHitChain()
        {
            if (currentHitChain == hitChainA)
            {
                prevHitChain = hitChainA;
                currentHitChain = hitChainB;

            }
            else
            {
                prevHitChain = hitChainB;
                currentHitChain = hitChainA;

            }
            currentHitChain.Clear();
        }



        public RenderElement HitTestOnPrevChain()
        {
            if (prevHitChain.Count > 0)
            {
                foreach (HitPoint hp in prevHitChain)
                {
                    //top down test
                    RenderElement elem = hp.hitObject as RenderElement;
                    if (elem != null && elem.IsTestable)
                    {
                        if (elem.Contains(hp.point))
                        {
                            RenderElement foundOverlapChild = elem.FindOverlapedChildElementAtPoint(elem, hp.point);

                            if (foundOverlapChild == null)
                            {
                                Point leftTop = elem.Location;
                                globalOffsetX -= leftTop.X;
                                globalOffsetY -= leftTop.Y;
                                testPointX += leftTop.X;
                                testPointY += leftTop.Y;

                                currentHitChain.Add(new HitPoint(elem, new Point(testPointX, testPointY)));
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (currentHitChain.Count > 0)
                {
                    return currentHitChain[currentHitChain.Count - 1].hitObject as RenderElement;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


#if DEBUG

        public dbugHitTestTracker dbugHitTracker;
#endif
    }

}