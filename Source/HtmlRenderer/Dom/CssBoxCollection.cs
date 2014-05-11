//2014 BSD,WinterCore

using System;
using System.Collections.Generic;
namespace HtmlRenderer.Dom
{

    class CssBoxCollection
    {
        List<CssBox> _boxes = new List<CssBox>();
        CssBox owner;
        public CssBoxCollection(CssBox owner)
        {
            this.owner = owner;
        }
        public IEnumerable<CssBox> GetChildBoxIter()
        {
            List<CssBox> tmp = _boxes;
            int j = tmp.Count;
            for (int i = 0; i < j; ++i)
            {
                yield return tmp[i];
            }
        }
        public void Add(CssBox box)
        {
            if (this.owner == box)
            {
                throw new NotSupportedException();
            }
            CssBox.UnsafeSetNodes(box, owner);
            this._boxes.Add(box);
        }
        public void Insert(int index, CssBox box)
        {
            this._boxes.Insert(index, box);
            CssBox.UnsafeSetNodes(box, owner);
        }
        internal void ChangeSiblingIndex(CssBox box, int toNewIndex)
        {
            this._boxes.Remove(box);
            this._boxes.Insert(toNewIndex, box);
            //this._boxes.Insert(index, box);
            //CssBox.UnsafeSetNodes(box, owner, null, null);
        }
        public int IndexOf(CssBox box)
        {
            return this._boxes.IndexOf(box);
        }
        public int Count
        {
            get
            {
                return this._boxes.Count;
            }
        }
        public bool Remove(CssBox box)
        {
            if (this._boxes.Remove(box))
            {
                CssBox.UnsafeSetNodes(box, null);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void RemoveAt(int index)
        {
            CssBox tmp = this._boxes[index];
            this._boxes.RemoveAt(index);
            CssBox.UnsafeSetNodes(tmp, null);
        }
        public CssBox this[int index]
        {
            get
            {
                return this._boxes[index];
            }
        }
        public IEnumerator<CssBox> GetEnumerator()
        {
            return this._boxes.GetEnumerator();
        }
    }


}