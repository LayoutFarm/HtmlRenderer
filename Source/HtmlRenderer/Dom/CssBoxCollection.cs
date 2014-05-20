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
            int count = _boxes.Count;
            if (count > 0)
            {
                CssBox.UnsafeSetNodes(box, owner, _boxes[count - 1]);
                this._boxes.Add(box);
            }
            else
            {
                CssBox.UnsafeSetNodes(box, owner, null);
                this._boxes.Add(box);
            }
        }
        public void Insert(int index, CssBox box)
        {
            int count = _boxes.Count;

            switch (index)
            {
                case 0:
                    {
                        //insert at pos
                        if (count > 0)
                        {
                            CssBox currentBoxAtIndex = _boxes[0];
                            CssBox.UnsafeSetNodes(currentBoxAtIndex, owner, box);
                        }
                        this._boxes.Insert(index, box);
                        CssBox.UnsafeSetNodes(box, owner, null);

                    } break;
                default:
                    {
                        CssBox currentBoxAtIndex = _boxes[index];
                        CssBox currentBoxAtPreIndex = _boxes[index - 1];

                        CssBox.UnsafeSetNodes(currentBoxAtIndex, owner, box);
                        CssBox.UnsafeSetNodes(box, owner, currentBoxAtPreIndex);

                        this._boxes.Insert(index, box);

                    } break;
            }
        }
        internal void ChangeSiblingIndex(CssBox box, int toNewIndex)
        {
            int existingIndex = this._boxes.IndexOf(box);
            if (existingIndex != toNewIndex)
            {
                this.RemoveAt(existingIndex);
                this.Insert(toNewIndex, box);
            }    
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
            this.RemoveAt(this._boxes.IndexOf(box));
        }
        public void RemoveAt(int index)
        {
            
            CssBox tmp = this._boxes[index];
            CssBox nextBox = null; 
            if (index < _boxes.Count - 1)
            { 
                nextBox = this._boxes[index + 1]; 
                CssBox preBox = null;
                if (index > 0)
                {
                    preBox = this._boxes[index - 1];
                }
                CssBox.UnsafeSetNodes(nextBox, owner, preBox);

            }
            this._boxes.RemoveAt(index);
            CssBox.UnsafeSetNodes(tmp, null, null);
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