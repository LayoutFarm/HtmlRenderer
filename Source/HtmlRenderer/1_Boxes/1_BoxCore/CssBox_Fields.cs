//BSD 2014, WinterDev
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


namespace HtmlRenderer.Dom
{

    //collection features

    partial class CssBox
    {

        readonly object _controller;
       
        //----------------------------------------------------
        /// <summary>
        /// the html tag that is associated with this css box, null if anonymous box
        /// </summary> 
        int _boxCompactFlags;
        //html rowspan: for td,th 
        int _rowSpan;
        int _colSpan;
        //---------------------------------------------------- 

        //condition 1 :this Box is BlockBox
        //1.1 contain lineBoxes for my children and  other children (share)
        LinkedList<CssLineBox> _clientLineBoxes;
        //1.2 contains box collection for my children
        readonly CssBoxCollection _aa_boxes;
        //----------------------------------------------------    
        //condition 2 :this Box is InlineBox          
        List<CssRun> _aa_contentRuns;
        char[] _buffer;

        bool isBrElement;
        bool _fixDisplayType;

        //----------------------------------------------------  
        //for other subbox , list item , shadow... 
        SubBoxCollection _subBoxes;
        //----------------------------------------------------   

        //state
        protected int _prop_pass_eval;

        /// <summary>
        /// Gets the childrenn boxes of this box
        /// </summary>      
        CssBoxCollection Boxes
        {
            get { return _aa_boxes; }
        }

        internal bool specialBlockInsideInline
        {
            get;
            set;
        }
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

        public IEnumerable<CssRun> GetRunBackwardIter()
        {
            if (this._aa_contentRuns != null)
            {
                var tmpRuns = this._aa_contentRuns;
                int j = tmpRuns.Count;
                for (int i = tmpRuns.Count - 1; i >= 0; --i)
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

        public CssBox GetFirstChild()
        {
            return this._aa_boxes[0];
        }
        //-----------------------------------
        public CssBox GetChildBox(int index)
        {

            return this._aa_boxes[index];
        }
        public void InsertChild(int index, CssBox box)
        {
            this.Boxes.Insert(index, box);
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
        internal void SetRowColSpan(int rowSpan, int colSpan)
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

        //==================================================
    
        public CssDisplay CssDisplay
        {
            get
            {
                return this._cssDisplay;
            }
        }
        internal SubBoxCollection SubBoxes
        {
            get
            {
                return this._subBoxes;
            }
            set
            {
                this._subBoxes = value;
            }
        }
        internal static void SetAsBrBox(CssBox box)
        {
            box.isBrElement = true;
        }
        internal static void ChangeDisplayType(CssBox box, CssDisplay newdisplay)
        {
             
            if (!box._fixDisplayType)
            {
                box._cssDisplay = newdisplay;
            }

            box.IsInline = (newdisplay == CssDisplay.BlockInsideInlineAfterCorrection) || ((newdisplay == CssDisplay.Inline ||
                    newdisplay == CssDisplay.InlineBlock)
                    && !box.IsBrElement);

            box._isVisible = box._cssDisplay != CssDisplay.None && box._myspec.Visibility == CssVisibility.Visible;
            //-------------------------
            //check containing property 
            //-------------------------
            switch (newdisplay)
            {
                case CssDisplay.BlockInsideInlineAfterCorrection:
                case CssDisplay.Block:
                case CssDisplay.ListItem:
                case CssDisplay.Table:
                case CssDisplay.TableCell:
                    box._boxCompactFlags |= CssBoxFlagsConst.HAS_CONTAINER_PROP;
                    break;
                default:
                    //not container properties 
                    box._boxCompactFlags &= ~CssBoxFlagsConst.HAS_CONTAINER_PROP;
                    break;
            }

            //-------------------------
        }

    }

}