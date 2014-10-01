//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm
{   
    public class MyHitPointChain : HitPointChain
    {
        LinkedList<HitPoint> currentHitChain;
        LinkedList<HitPoint> prevHitChain;
        readonly LinkedList<HitPoint> hitChainA = new LinkedList<HitPoint>();
        readonly LinkedList<HitPoint> hitChainB = new LinkedList<HitPoint>();
        LinkedList<IHitElement> dragHitElements = new LinkedList<IHitElement>();

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
            currentHitChain.RemoveLast();
        }

        public override Point PrevHitPoint
        {
            get
            {

                if (prevHitChain.Count > 0)
                {
                    return prevHitChain.Last.Value.point;
                }
                else
                {
                    return Point.Empty;
                }
            }
        }
        public override Point CurrentHitPoint
        {
            get { return currentHitChain.Last.Value.point; }
        }
        public override IHitElement CurrentHitElement
        {
            get
            {
                if (currentHitChain.Count > 0)
                {
                    return currentHitChain.Last.Value.elem;
                }
                else
                {
                    return null;
                }
            }
        }
        public override void AddHit(IHitElement hitElement)
        {


            currentHitChain.AddLast(new HitPoint(hitElement, new Point(testPointX, testPointY)));
#if DEBUG
            dbugHitTracker.WriteTrackNode(currentHitChain.Count,
                new Point(testPointX, testPointY).ToString() + " on "
                + hitElement.ElementBoundRect.ToString() + hitElement.GetType().Name);
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
        public IHitElement HitTestOnPrevChain()
        {
            if (prevHitChain.Count > 0)
            {
                foreach (HitPoint hp in prevHitChain)
                {
                    IHitElement elem = hp.elem;
                    if (elem != null && elem.IsTestable())
                    {
                        if (elem.HitTestCoreNoRecursive(hp.point))
                        {
                            IHitElement foundOverlapChild = elem.FindOverlapSibling(hp.point);

                            if (foundOverlapChild == null)
                            {
                                Point leftTop = elem.ElementLocation;
                                globalOffsetX -= leftTop.X;
                                globalOffsetY -= leftTop.Y;
                                testPointX += leftTop.X;
                                testPointY += leftTop.Y;

                                currentHitChain.AddLast(new HitPoint(elem, new Point(testPointX, testPointY)));
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
                    return currentHitChain.Last.Value.elem;
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
        public override void AddDragHitElement(IHitElement element)
        {
            dragHitElements.AddLast(element);
        }
        public override void RemoveDragHitElement(IHitElement element)
        {
            dragHitElements.Remove(element);
        }
        public override IEnumerable<IHitElement> GetDragHitElementIter()
        {
            LinkedListNode<IHitElement> node = dragHitElements.First;
            while (node != null)
            {
                yield return node.Value;
                node = node.Next;
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