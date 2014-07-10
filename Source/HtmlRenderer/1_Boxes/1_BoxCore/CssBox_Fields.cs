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
        //----------------------------------------------------

        /// <summary>
        /// the root container for the hierarchy
        /// </summary>
        protected HtmlContainer _htmlContainer;
        //----------------------------------------------------
        /// <summary>
        /// the html tag that is associated with this css box, null if anonymous box
        /// </summary>
        readonly BridgeHtmlElement _htmlElement;
        int _boxCompactFlags;
        //----------------------------------------------------


        //eg td,th,col,colgroup
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
                if ((this._boxCompactFlags & CssBoxFlagsConst.EVAL_ROWSPAN) == 0)
                {
                    string att = this.GetAttribute("rowspan", "1");
                    int rowspan;
                    if (!int.TryParse(att, out rowspan))
                    {
                        rowspan = 1;
                    }
                    this._boxCompactFlags |= CssBoxFlagsConst.EVAL_ROWSPAN;
                    return this._rowSpan = rowspan;
                }
                return this._rowSpan;
            }
            set
            {
                this._rowSpan = value;
                this._boxCompactFlags |= CssBoxFlagsConst.EVAL_ROWSPAN;
            }
        }
        internal int ColSpan
        {
            get
            {
                if ((this._boxCompactFlags & CssBoxFlagsConst.EVAL_COLSPAN) == 0)
                {
                    //default  = 1
                    string att = this.GetAttribute("colspan", "1");
                    int colspan;
                    if (!int.TryParse(att, out colspan))
                    {
                        colspan = 1;
                    }
                    this._boxCompactFlags |= CssBoxFlagsConst.EVAL_COLSPAN;
                    return this._colSpan = colspan;
                }
                return this._colSpan;
            }
            set
            {
                this._colSpan = value;
            }
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
        public WellknownHtmlTagName WellknownTagName
        {
            get
            {
                return this.wellKnownTagName;
            }
            private set
            {
                this.wellKnownTagName = value;
            }
        }
        public CssDisplay CssDisplay
        {
            get
            {
                return this._cssDisplay;
            }
        }

        internal static void ChangeDisplayType(CssBox box, CssDisplay newdisplay)
        {
            //single point method that can change
            //CssBox._cssDisplay Type

            switch (box.wellKnownTagName)
            {
                //some wellknown Html element name 
                //has fixed predefine display type  *** 
                //fix definition 
                case WellknownHtmlTagName.table:
                    newdisplay = CssDisplay.Table;
                    break;
                case WellknownHtmlTagName.tr:
                    newdisplay = CssDisplay.TableRow;
                    break;
                case WellknownHtmlTagName.tbody:
                    newdisplay = CssDisplay.TableRowGroup;
                    break;
                case WellknownHtmlTagName.thead:
                    newdisplay = CssDisplay.TableHeaderGroup;
                    break;
                case WellknownHtmlTagName.tfoot:
                    newdisplay = CssDisplay.TableFooterGroup;
                    break;
                case WellknownHtmlTagName.col:
                    newdisplay = CssDisplay.TableColumn;
                    break;
                case WellknownHtmlTagName.colgroup:
                    newdisplay = CssDisplay.TableColumnGroup;
                    break;
                case WellknownHtmlTagName.td:
                case WellknownHtmlTagName.th:
                    newdisplay = CssDisplay.TableCell;
                    break;
                case WellknownHtmlTagName.caption:
                    newdisplay = CssDisplay.TableCaption;
                    break;
            }

            box._cssDisplay = newdisplay;

            box.IsInline = (newdisplay == CssDisplay.BlockInsideInlineAfterCorrection) || ((newdisplay == CssDisplay.Inline ||
                    newdisplay == CssDisplay.InlineBlock)
                    && !box.IsBrElement);

            box._isVisible = box._cssDisplay != Dom.CssDisplay.None && box._myspec.Visibility == CssVisibility.Visible;
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