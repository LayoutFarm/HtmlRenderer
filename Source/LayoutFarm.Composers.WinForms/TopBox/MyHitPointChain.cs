//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm
{
    class MyHitPointChain : HitPointChain
    {
        List<HitPoint> currentHitChain;
        List<HitPoint> prevHitChain;
        readonly List<HitPoint> hitChainA = new List<HitPoint>();
        readonly List<HitPoint> hitChainB = new List<HitPoint>();
        

        List<RenderElement> dragHitElements = new List<RenderElement>();

        public MyHitPointChain()
        {
            currentHitChain = hitChainA;
            prevHitChain = hitChainB;
        }
        protected override void OnClearAll()
        {
            currentHitChain.Clear();
            prevHitChain.Clear();
        }
        public override void RemoveCurrentHitNode()
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
        public override RenderElement CurrentHitElement
        {
            get
            {
                if (currentHitChain.Count > 0)
                {
                    return currentHitChain[currentHitChain.Count - 1].elem;
                }
                else
                {
                    return null;
                }
            }
        }
        public override void AddHit(RenderElement hitElement)
        {


            currentHitChain.Add(new HitPoint(hitElement, new Point(testPointX, testPointY)));
#if DEBUG
            dbugHitTracker.WriteTrackNode(currentHitChain.Count,
                new Point(testPointX, testPointY).ToString() + " on "
                + hitElement.BoundRect.ToString() + hitElement.GetType().Name);
#endif

        }

       
        public void SwapHitChain()
        {
            if (currentHitChain == hitChainA)
            {
                prevHitChain = hitChainA;
                currentHitChain = hitChainB;
                this.TailObject = null;
            }
            else
            {
                prevHitChain = hitChainB;
                currentHitChain = hitChainA;
                this.TailObject = null;
                
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

                    RenderElement elem = hp.elem;
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
                    return currentHitChain[currentHitChain.Count - 1].elem;
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
        public override void ClearDragHitElements()
        {
            dragHitElements.Clear();
        }
        public override void AddDragHitElement(RenderElement element)
        {
            dragHitElements.Add(element);
        }
        public override void RemoveDragHitElement(RenderElement element)
        {
            dragHitElements.Remove(element);
        }
        public override IEnumerable<RenderElement> GetDragHitElementIter()
        {
            int j = dragHitElements.Count;
            for (int i = 0; i < j; ++i)
            {
                yield return dragHitElements[i];
            }
        }
        public override int DragHitElementCount
        {
            get
            {
                return dragHitElements.Count;
            }
        }
#if DEBUG
        public IEnumerable<HitPoint> dbugGetHitPairIter()
        {
            foreach (HitPoint hitPair in currentHitChain)
            {
                yield return hitPair;
            }

        }

        public dbugHitTestTracker dbugHitTracker;
#endif
    }

}