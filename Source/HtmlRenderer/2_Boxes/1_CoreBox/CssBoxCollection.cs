//2014 BSD,WinterDev


using System;
using System.Collections.Generic;

namespace HtmlRenderer.Boxes
{

    class CssBoxCollection
    {

        LinkedList<CssBox> _boxes = new LinkedList<CssBox>();
        CssBox owner;
        public CssBoxCollection(CssBox owner)
        {
            this.owner = owner;
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
        public void Add(CssBox box)
        {

#if DEBUG
            if (this.owner == box)
            {
                throw new NotSupportedException();
            }
#endif
            CssBox.UnsafeSetNodes2(box, this.owner, _boxes.AddLast(box));
        }
        public void InsertBefore(CssBox beforeBox, CssBox box)
        {
            var beforeLinkedNode = CssBox.UnsafeGetLinkedNode(beforeBox);
            CssBox.UnsafeSetNodes2(box, this.owner,
                this._boxes.AddBefore(beforeLinkedNode, box));
        }
        public int Count
        {
            get
            {
                return this._boxes.Count;
            }
        }
        public void Remove(CssBox box)
        {
            var linkedNode = CssBox.UnsafeGetLinkedNode(box);
            this._boxes.Remove(linkedNode);
            CssBox.UnsafeSetNodes2(box, null, null);
        }
        public CssBox GetFirstChild()
        {
            return this._boxes.First.Value;
        }
        public CssBox GetLastChild()
        {
            return this._boxes.Last.Value;
        }
        public IEnumerator<CssBox> GetEnumerator()
        {
            return this._boxes.GetEnumerator();
        }
        internal LinkedListNode<CssBox> GetNodeAtIndex(int index)
        {

            switch (index)
            {
                //hint
                case 0:
                    {
                        return this._boxes.First;//0
                    }
                case 1:
                    {
                        return this._boxes.First.Next;//0,1

                    }
                case 2:
                    {
                        return this._boxes.First.Next.Next;//0,1,2 
                    }
                case 3:
                    {
                        return this._boxes.First.Next.Next.Next;//0,1,2,3
                    }
                default:
                    {

                        int j = this._boxes.Count;
                        var cnode = this._boxes.First;
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
        public void ChangeSiblingIndex(CssBox box, int newIndex)
        {
            //find target linked node 
            LinkedListNode<CssBox> foundNode = this.GetNodeAtIndex(newIndex);
            //1. remove from current box
            this.Remove(box);
            //2. 
            CssBox.UnsafeSetNodes2(box, this.owner, this._boxes.AddBefore(foundNode, box)); 
        }
    }


}