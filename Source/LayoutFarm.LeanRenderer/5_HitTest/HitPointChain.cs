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


    public class HitPointChain
    {
        LinkedList<HitPoint> currentHitChain;
        LinkedList<HitPoint> prevHitChain;
        readonly LinkedList<HitPoint> hitChainA = new LinkedList<HitPoint>();
        readonly LinkedList<HitPoint> hitChainB = new LinkedList<HitPoint>();

        int globalOffsetX = 0;
        int globalOffsetY = 0;

        int visualrootStartTestX;
        int visualrootStartTestY;

        int testPointX;
        int testPointY;

#if DEBUG
        public dbugHitTestTracker dbugHitTracker;

#endif

        public HitPointChain()
        {
            currentHitChain = hitChainA;
            prevHitChain = hitChainB;
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



        public void SetVisualRootStartTestPoint(int x, int y)
        {

            testPointX = x;
            testPointY = y;
            visualrootStartTestX = x;
            visualrootStartTestY = y;
        }
        public int LastestRootX
        {
            get
            {
                return visualrootStartTestX;
            }
        }
        public int LastestRootY
        {
            get
            {
                return visualrootStartTestY;
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
            currentHitChain.Clear();
            prevHitChain.Clear();
            testPointX = 0;
            testPointY = 0;

        } 
        public Point PrevHitPoint
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
        public IHitElement CurrentHitElement
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
        public Point CurrentHitPoint
        {
            get
            {
                if (currentHitChain.Count > 0)
                {
                    return currentHitChain.Last.Value.point;
                }
                else
                {
                    return Point.Empty;
                }
            }
        }
        public LinkedListNode<HitPoint> CurrentHitNode
        {
            get
            {
                if (currentHitChain.Count > 0)
                {
                    return currentHitChain.Last;
                }
                else
                {
                    return null;
                }
            }
        }


        public void AddHit(IHitElement aobj)
        {
            currentHitChain.AddLast(new HitPoint(aobj, new Point(testPointX, testPointY)));
#if DEBUG
            dbugHitTracker.WriteTrackNode(currentHitChain.Count,
                new Point(testPointX, testPointY).ToString() + " on " 
                + aobj.ElementBoundRect.ToString() + aobj.GetType().Name);
#endif
        }
        public int Level
        {
            get
            {
                return currentHitChain.Count;
            }
        }
        public void RemoveHit(LinkedListNode<HitPoint> hitPair)
        {
            currentHitChain.Remove(hitPair);
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
                    if (elem != null &&elem.IsTestable())
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

        LinkedList<RenderElement> dragHitElements = new LinkedList<RenderElement>();
        public void ClearDragHitElements()
        {
            dragHitElements.Clear();
        }


        public void AddDragHitElement(RenderElement element)
        {
            dragHitElements.AddLast(element);
        }
        public void RemoveDragHitElement(RenderElement element)
        {
            dragHitElements.Remove(element);
        }
        public IEnumerable<RenderElement> GetDragHitElementIter()
        {
            LinkedListNode<RenderElement> node = dragHitElements.First;
            while (node != null)
            {
                yield return node.Value;
                node = node.Next;
            }
        }
        public int DragHitElementCount
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
#endif

    }

}
