﻿//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{
    class MyHitChain : HitChain
    {
        List<HitInfo> currentHitChain;
        List<HitInfo> prevHitChain;
        readonly List<HitInfo> hitChainA = new List<HitInfo>();
        readonly List<HitInfo> hitChainB = new List<HitInfo>();


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
        public override HitInfo GetHitInfo(int index)
        {
            return currentHitChain[index];
        }
        //public override Point PrevHitPoint
        //{
        //    get
        //    {
        //        if (prevHitChain.Count > 0)
        //        {
        //            //?
        //            return prevHitChain[prevHitChain.Count - 1].point;
        //        }
        //        else
        //        {
        //            return Point.Empty;
        //        }
        //    }
        //}
        public override Point CurrentHitPoint
        {
            get { return currentHitChain[currentHitChain.Count - 1].point; }
        }
        public override RenderElement CurrentHitElement
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
        public override void AddHitObject(RenderElement hitObject)
        {
            currentHitChain.Add(new HitInfo(hitObject, new Point(testPointX, testPointY)));
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
                foreach (HitInfo hp in prevHitChain)
                {
                    //top down test
                    RenderElement elem = hp.hitElement;
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

                                currentHitChain.Add(new HitInfo(elem, new Point(testPointX, testPointY)));
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
                    return currentHitChain[currentHitChain.Count - 1].hitElement;
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