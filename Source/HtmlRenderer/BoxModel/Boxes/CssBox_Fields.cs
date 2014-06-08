//BSD 2014, WinterCore

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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using HtmlRenderer.Entities;
using HtmlRenderer.Handlers;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

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

        /// <summary>
        /// the html tag that is associated with this css box, null if anonymous box
        /// </summary>
        private readonly IHtmlElement _htmltag;
        char[] _textBuffer;

        /// <summary>
        /// Do not use or alter this flag
        /// </summary>
        /// <remarks>
        /// Flag that indicates that CssTable algorithm already made fixes on it.
        /// </remarks>
        internal bool _tableFixed;

        protected bool _wordsSizeMeasured;

        private CssBox _listItemBox;
        //----------------------------------------------------
        //if this is inline box , this CssBox
        //may be 'flowed' into more than one CssLineBox
        CssLineBox _firstHostingLineBox;
        CssLineBox _lastHostingLineBox;
        //one CssBox may use more than one cssline     
        //---------------------------------------------------- 
        int _boxCompactFlags;
        //----------------------------------------------------
        int _rowSpan;
        int _colSpan;

        /// <summary>
        /// handler for loading background image
        /// </summary>
        ImageLoadHandler _imageLoadHandler;
        readonly WellknownHtmlTagName wellKnownTagName;
        //---------------------------------------------------- 

        //condition 1 :this Box is BlockBox
        //1.1 contain lineBoxes for my children and  other children (share)
        LinkedList<CssLineBox> _clientLineBoxes;
         

        //1.2 contains box collection for my children
        readonly CssBoxCollection _boxes;
        //----------------------------------------------------   

        //condition 2 :this Box is InlineBox 
        List<CssRun> _boxRuns;

        //----------------------------------------------------  


        /// <summary>
        /// Gets the childrenn boxes of this box
        /// </summary>
        CssBoxCollection Boxes
        {
            get { return _boxes; }
        }


        internal void AddRun(CssRun run)
        {
            if (this._boxRuns == null)
            {
                this._boxRuns = new List<CssRun>();
            }

            this._boxRuns.Add(run);
        }

        //internal PartialBoxStrip GetStripOrCreateIfNotExists(CssBox clientBox, CssLineBox lineBox, out bool isNew)
        //{
        //    if (this._clientStrips == null)
        //    {
        //        this._clientStrips = new CssStripCollection();
        //    }
        //    return this._clientStrips.GetOrCreateStripInfo(lineBox, clientBox, out isNew);
        //}
        internal int RunCount
        {
            get
            {
                return this._boxRuns != null ? this._boxRuns.Count : 0;
            }
        }
        public IEnumerable<CssBox> GetChildBoxIter()
        {
            return this._boxes.GetChildBoxIter();
        }
         
        public IEnumerable<CssRun> GetRunIter()
        {
            if (this._boxRuns != null)
            {
                var tmpRuns = _boxRuns;
                int j = tmpRuns.Count;
                for (int i = 0; i < j; ++i)
                {
                    yield return tmpRuns[i];
                }

            }
        }
        internal IEnumerable<CssLineBox> GetMyHostLineIter()
        {
            var hostLine = this.FirstHostingLineBox;
            while (hostLine != null)
            {
                yield return hostLine;
                hostLine = hostLine.NextLine;
            }
        }
        public int ChildCount
        {
            get
            {
                return this._boxes.Count;
            }
        }

        public CssBox GetFirstChild()
        {
            return this._boxes[0];
        }
        //-----------------------------------
        public CssBox GetChildBox(int index)
        {

            return this._boxes[index];
        }
        public void InsertChild(int index, CssBox box)
        {
            this.Boxes.Insert(index, box);
        }
        //--------
        internal void ResetLineBoxes()
        {
            if (this._clientLineBoxes != null)
            {
                _clientLineBoxes.Clear();
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

    }

}