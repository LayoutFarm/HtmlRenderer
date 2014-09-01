//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LayoutFarm.Presentation
{
     
    public class ArtHitPointChain
    {
        public struct HitPair
        {
            public Point point;
            public ArtVisualElement elem;

            public static readonly HitPair Empty = new HitPair();
            public HitPair(ArtVisualElement elem, Point point)
            {
                this.point = point;
                this.elem = elem;
            }
            public static bool operator ==(HitPair pair1, HitPair pair2)
            {
                return ((pair1.elem == pair2.elem) && (pair1.point == pair2.point));
            }
            public static bool operator !=(HitPair pair1, HitPair pair2)
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



        LinkedList<HitPair> currentHitChain; LinkedList<HitPair> prevHitChain; readonly LinkedList<HitPair> hitChainA = new LinkedList<HitPair>(); readonly LinkedList<HitPair> hitChainB = new LinkedList<HitPair>();

        int globalOffsetX = 0;
        int globalOffsetY = 0;

        int visualrootStartTestX;
        int visualrootStartTestY;

        int testPointX;
        int testPointY;

#if DEBUG
        public dbugHitTestTracker dbugHitTracker;

#endif

        public ArtHitPointChain()
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
        public ArtVisualElement PrevHitElement
        {
            get
            {
                if (prevHitChain.Count > 0)
                {
                    return prevHitChain.Last.Value.elem;
                }
                else
                {
                    return null;
                }

            }
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
        public ArtVisualElement CurrentHitElement
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
        public LinkedListNode<HitPair> CurrentHitNode
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


        public void AddHit(ArtVisualElement aobj)
        {
            currentHitChain.AddLast(new HitPair(aobj, new Point(testPointX, testPointY)));
#if DEBUG
            dbugHitTracker.WriteTrackNode(currentHitChain.Count, new Point(testPointX, testPointY).ToString() + " on " + aobj.Rect.ToString() + aobj.GetType().Name);
#endif
        }
        public int Level
        {
            get
            {
                return currentHitChain.Count;
            }
        }
        public void RemoveHit(LinkedListNode<HitPair> hitPair)
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
        public ArtVisualElement HitTestOnPrevChain()
        {


            if (prevHitChain.Count > 0)
            {

                foreach (HitPair hp in prevHitChain)
                {
                    ArtVisualElement elem = hp.elem;
                    if (ArtVisualElement.IsTestableElement(elem))
                    {
                        if (elem.HitTestCoreNoRecursive(hp.point))
                        {

                            ArtVisualElement foundOverlapChild = elem.ParentVisualElement.FindOverlapedChildElementAtPoint(elem, hp.point);


                            if (foundOverlapChild == null)
                            {
                                Point leftTop = elem.Location;
                                globalOffsetX -= leftTop.X;
                                globalOffsetY -= leftTop.Y;
                                testPointX += leftTop.X;
                                testPointY += leftTop.Y;

                                currentHitChain.AddLast(new HitPair(elem, new Point(testPointX, testPointY)));
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

        LinkedList<ArtVisualElement> dragHitElements = new LinkedList<ArtVisualElement>();
        public void ClearDragHitElements()
        {
            dragHitElements.Clear();
        }


        public void AddDragHitElement(ArtVisualElement element)
        {
            dragHitElements.AddLast(element);
        }
        public void RemoveDragHitElement(ArtVisualElement element)
        {
            dragHitElements.Remove(element);
        }
        public IEnumerable<ArtVisualElement> GetDragHitElementIter()
        {
            LinkedListNode<ArtVisualElement> node = dragHitElements.First;
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
        public IEnumerable<HitPair> HitPairIter
        {
            get
            {
                foreach (HitPair hitPair in currentHitChain)
                {
                    yield return hitPair;
                }
            }
        }
#endif

    }

}
