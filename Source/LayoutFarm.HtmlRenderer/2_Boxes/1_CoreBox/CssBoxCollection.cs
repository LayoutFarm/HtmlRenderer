//BSD, 2014-present, WinterDev 

using System;
using System.Collections.Generic;
namespace LayoutFarm.HtmlBoxes
{
    class CssBoxAbsoluteLayer
    {
        List<CssBox> _boxes = new List<CssBox>();
        public CssBoxAbsoluteLayer()
        {
        }
        public void AddChild(CssBox box)
        {
#if DEBUG
            if (box.dbugIsInAbsLayer)
            {

            }
            else
            {
                box.dbugIsInAbsLayer = true;
            }
#endif
            _boxes.Add(box);

        }
        public CssBox GetBox(int index) { return _boxes[index]; }
        public int Count { get { return _boxes.Count; } }
        public void Clear()
        {
            //clear abs owner
            for (int i = _boxes.Count - 1; i >= 0; --i)
            {
                _boxes[i].ResetAbsoluteLayerOwner();
#if DEBUG
                _boxes[i].dbugIsInAbsLayer = false;
#endif
            }

            _boxes.Clear();
        }
        public bool Remove(CssBox box)
        {
            if (_boxes.Remove(box))
            {
#if DEBUG
                box.dbugIsInAbsLayer = false;
#endif
                box.ResetAbsoluteLayerOwner();
                return true;
            }
            return false;
        }

    }
    class CssBoxCollection
    {
        LinkedList<CssBox> _boxes = new LinkedList<CssBox>();

        public CssBoxCollection()
        {
        }


        public IEnumerable<CssBox> GetChildBoxIter()
        {
            var cNode = _boxes.First;
            while (cNode != null)
            {
                yield return cNode.Value;
                cNode = cNode.Next;
            }
        }
        public IEnumerable<CssBox> GetChildBoxBackwardIter()
        {
            var cNode = _boxes.Last;
            while (cNode != null)
            {
                yield return cNode.Value;
                cNode = cNode.Previous;
            }
        }

        public void AddChild(CssBox owner, CssBox box)
        {
#if DEBUG
            if (owner == box)
            {
                throw new NotSupportedException();
            }
#endif
            CssBox.UnsafeSetNodes(box, owner, _boxes.AddLast(box));
        }
        public void InsertBefore(CssBox owner, CssBox beforeBox, CssBox box)
        {
            var beforeLinkedNode = CssBox.UnsafeGetLinkedNode(beforeBox);
            CssBox.UnsafeSetNodes(box, owner,
                _boxes.AddBefore(beforeLinkedNode, box));
        }
        public void Clear()
        {
            var linkNode = _boxes.First;
            while (linkNode != null)
            {
                var box = linkNode.Value;
                CssBox.UnsafeSetParent(box, null);
                linkNode = linkNode.Next;
            }
            _boxes.Clear();
        }
        public int Count
        {
            get
            {
                return _boxes.Count;
            }
        }
        public void Remove(CssBox box)
        {
            var linkedNode = CssBox.UnsafeGetLinkedNode(box);
            _boxes.Remove(linkedNode);
            CssBox.UnsafeSetNodes(box, null, null);
        }
        public CssBox GetFirstChild()
        {
            return _boxes.First.Value;
        }
        public LinkedListNode<CssBox> GetFirstLinkedNode()
        {
            return _boxes.First;
        }
        public LinkedListNode<CssBox> GetLastLinkedNode()
        {
            return _boxes.Last;
        }

        public CssBox GetLastChild()
        {
            return _boxes.Last.Value;
        }
        public IEnumerator<CssBox> GetEnumerator()
        {
            return _boxes.GetEnumerator();
        }

#if DEBUG
        public bool dbugContains(CssBox box)
        {
            return _boxes.Contains(box);
        }
        internal LinkedListNode<CssBox> dbugGetNodeAtIndex(int index)
        {
            //for compat with old version
            switch (index)
            {
                //hint
                case 0:
                    {
                        return _boxes.First;//0
                    }
                case 1:
                    {
                        return _boxes.First.Next;//0,1
                    }
                case 2:
                    {
                        return _boxes.First.Next.Next;//0,1,2 
                    }
                case 3:
                    {
                        return _boxes.First.Next.Next.Next;//0,1,2,3
                    }
                default:
                    {
                        int j = _boxes.Count;
                        var cnode = _boxes.First;
                        for (int i = 0; i < j; ++i)
                        {
                            if (i == index)
                            {
                                return cnode;
                            }
                            else if (i > index)
                            {
                                return null;
                            }
                            cnode = cnode.Next;
                        }
                        return null;//not found                         
                    }
            }
        }

        public void dbugChangeSiblingIndex(CssBox owner, CssBox box, int newIndex)
        {
            //for compat with old version
            //find target linked node 
            LinkedListNode<CssBox> foundNode = this.dbugGetNodeAtIndex(newIndex);
            //1. remove from current box
            this.Remove(box);
            //2. 
            CssBox.UnsafeSetNodes(box, owner, _boxes.AddBefore(foundNode, box));
        }
#endif
    }
}