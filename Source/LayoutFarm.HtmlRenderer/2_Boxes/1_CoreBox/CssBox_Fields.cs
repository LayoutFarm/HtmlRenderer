// 2015,2014 ,BSD, WinterDev
//ArthurHub

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
namespace LayoutFarm.HtmlBoxes
{


    partial class CssBox
    {

        //condition 1: 
        //  box with multiple children box under block-formatting context
        //  eg. 
        //      <div>
        //          <div></div>
        //          <div></div>
        //      </div>

        //condition 2:
        //box with multiple children box under line-formatting context
        //eg.
        //   <div>
        //      <span><u></u></span>
        //  </div>

        //condition 3:
        //box with runlist (no child box) ,  
        //its content runlist will be flow on itself or another box(condition2 box)
        //eg.
        //   <span> AAA </span>


        //condition 1: valid  
        //condition 2: valid  
        //condition 3  : invalid *
        CssBoxCollection _aa_boxes;

        //condition 1: invalid *
        //condition 2: invalid *
        //condition 3: valid 
        List<CssRun> _aa_contentRuns;


        //condition 1: invalid *
        //condition 2: valid  
        //condition 3: valid  
        LinkedList<CssLineBox> _clientLineBoxes;

        //----------------------------------------------------   
        //only in condition 3
        char[] _buffer;
        //----------------------------------------------------    


        internal int RunCount
        {
            get
            {
                return this._aa_contentRuns != null ? this._aa_contentRuns.Count : 0;
            }
        }
        public IEnumerable<CssBox> GetChildBoxIter()
        {
            return this._aa_boxes.GetChildBoxIter();
        }
        internal CssBox GetNextNode()
        {
            if (_linkedNode != null && _linkedNode.Next != null)
            {
                return _linkedNode.Next.Value;
            }
            return null;
        }
        internal CssBox GetPrevNode()
        {
            if (_linkedNode != null && _linkedNode.Previous != null)
            {
                return _linkedNode.Previous.Value;
            }
            return null;
        }
        public IEnumerable<CssRun> GetRunIter()
        {
            if (this._aa_contentRuns != null)
            {
                var tmpRuns = this._aa_contentRuns;
                int j = tmpRuns.Count;
                for (int i = 0; i < j; ++i)
                {
                    yield return tmpRuns[i];
                }
            }
        }


        public int ChildCount
        {
            get
            {
                return this._aa_boxes.Count;
            }
        }
        //-----------------------------------
        public CssBox GetFirstChild()
        {
            return this._aa_boxes.GetFirstChild();
        }
        public void AppendChild(CssBox box)
        {
            this._aa_boxes.AddChild(this, box);
        }
        public void InsertChild(CssBox beforeBox, CssBox box)
        {
            this._aa_boxes.InsertBefore(this, beforeBox, box);
        }
        public virtual void Clear()
        {
            //_aa_contentRuns may come from other data source
            //so just set it to null

            this._clientLineBoxes = null;
            this._aa_contentRuns = null;
            this._aa_boxes.Clear();
        }

        //-------------------------------------
        internal void ResetLineBoxes()
        {
            if (this._clientLineBoxes != null)
            {
                _clientLineBoxes.Clear();
            }
            else
            {
                _clientLineBoxes = new LinkedList<CssLineBox>();
            }
        }
        //-------------------------------------
        internal int RowSpan
        {
            get
            {
                return this._rowSpan;
            }
        }
        internal int ColSpan
        {
            get
            {
                return this._colSpan;
            }
        }

        public void SetRowSpanAndColSpan(int rowSpan, int colSpan)
        {
            this._rowSpan = rowSpan;
            this._colSpan = colSpan;
        }
        /// <summary>
        /// The margin top value if was effected by margin collapse.
        /// </summary>
        float CollapsedMarginTop
        {
            get;
            set;
        }
        protected bool HasCustomRenderTechnique
        {
            get
            {
                return (this._boxCompactFlags & BoxFlags.HAS_CUSTOM_RENDER_TECHNIQUE) != 0;
            }
            set
            {
                if (value)
                {
                    this._boxCompactFlags |= BoxFlags.HAS_CUSTOM_RENDER_TECHNIQUE;
                }
                else
                {
                    this._boxCompactFlags &= ~BoxFlags.HAS_CUSTOM_RENDER_TECHNIQUE;
                }
            }
        }




#if DEBUG
        internal void dbugChangeSiblingOrder(int siblingIndex)
        {
            if (siblingIndex < 0)
            {
                throw new Exception("before box doesn't exist on parent");
            }
            this._parentBox._aa_boxes.dbugChangeSiblingIndex(_parentBox, this, siblingIndex);
        }
#endif

    }

}