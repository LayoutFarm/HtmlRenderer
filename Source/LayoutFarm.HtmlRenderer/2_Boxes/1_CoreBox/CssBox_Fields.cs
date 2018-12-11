//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

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
        /// <summary>
        /// absolute position layer
        /// </summary>
        CssBoxAbsoluteLayer _absPosLayer;
        CssBlockRun _justBlockRun;
        //----------------------------------------------------   
        //only in condition 3
        char[] _buffer;
        //----------------------------------------------------    
        CssBoxDecorator _decorator;
        bool _mayHasViewport;
        bool _isOutOfFlowBox;

        CssBox _absLayerOwner;

        internal bool IsOutOfFlowBox
        {
            get => this._isOutOfFlowBox;
            set => this._isOutOfFlowBox = value;
        }


        internal int RunCount => _aa_contentRuns != null ? _aa_contentRuns.Count : 0;


        internal CssBoxDecorator Decorator
        {
            get => this._decorator;
            set => this._decorator = value;
        }
        public CssBlockRun JustBlockRun
        {
            get => this._justBlockRun;
            set => this._justBlockRun = value;

        }
        public IEnumerable<CssBox> GetChildBoxIter() => _aa_boxes.GetChildBoxIter();
        //
        public IEnumerable<CssBox> GetChildBoxBackwardIter() => _aa_boxes.GetChildBoxBackwardIter();
        //
        public IEnumerable<CssBox> GetAbsoluteChildBoxIter()
        {
            if (_absPosLayer != null)
            {
                int j = _absPosLayer.Count;
                for (int i = 0; i < j; ++i)
                {
                    yield return _absPosLayer.GetBox(i);
                }
            }

        }
        public IEnumerable<CssBox> GetAbsoluteChildBoxBackwardIter()
        {
            if (_absPosLayer != null)
            {
                int j = _absPosLayer.Count;
                for (int i = _absPosLayer.Count - 1; i >= 0; --i)
                {
                    yield return _absPosLayer.GetBox(i);
                }
            }
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
            if (_aa_contentRuns != null)
            {
                List<CssRun> tmpRuns = _aa_contentRuns;
                int j = tmpRuns.Count;
                for (int i = 0; i < j; ++i)
                {
                    yield return tmpRuns[i];
                }
            }
        }


        public int ChildCount => _aa_boxes.Count;


        //-----------------------------------
        public CssBox GetFirstChild() => _aa_boxes.GetFirstChild();

        public void RemoveChild(CssBox box)
        {
            switch (box.Position)
            {
                case Css.CssPosition.Absolute:
                case Css.CssPosition.Fixed:
                case Css.CssPosition.Relative:
                    {
                        //TODO: review here again
                        bool result = _absPosLayer.Remove(box);
                        if (!result)
                        {

                        }
                    }
                    break;
                default:
                    {
                        _aa_boxes.Remove(box);
                    }
                    break;
            }
        }

#if DEBUG
        public bool dbug_hasParent;
#endif
        public void AppendChild(CssBox box)
        {
#if DEBUG
            if (box.dbug_hasParent)
            {

            }
            else
            {

            }
            dbug_hasParent = true;
#endif
            switch (box.Position)
            {
                case Css.CssPosition.Absolute:
                    {
                        //first move this box to special layer of 'this' element 
                        //'take off normal flow'***
                        //css3 jan2015: absolute position
                        //use offset relative to its normal the box's containing box*** 

                        //absolute position box is removed from the normal flow entirely
                        //(it has no impact on later sibling)

                        CssBox ancester = FindContainerForAbsoluteBox();
                        if (box.ParentBox != null)
                        {
                            if (box.ParentBox != ancester)
                            {
                                //TODO: remove from current parent box
                                //and insert to newBOx
                                throw new NotSupportedException();
                            }
                        }
                        else
                        {
                            ancester.AppendToAbsoluteLayer(box);
                        }
                    }
                    break;
                case Css.CssPosition.Fixed:
                    {
                        //css3:
                        //similar to absolte positioning,                        
                        //only diff is that for a fixed positoned box,
                        //the containing block is estableing by the viewport

                        //removed from the normal flow entirely***

                        CssBox ancester = FindContainerForFixedBox();
                        if (box.ParentBox != null)
                        {
                            if (box.ParentBox != ancester)
                            {
                                //TODO: remove from current parent box
                                //and insert to newBOx
                                throw new NotSupportedException();
                            }
                        }
                        else
                        {
                            ancester.AppendToAbsoluteLayer(box);
                        }
                    }
                    break;
                case Css.CssPosition.Center:
                    {
                        //css3:
                        //a box is explicitly centerer with respect to its containing box
                        //removed from the normal flow entirely***
                        //TODO: err, revise here again
                        CssBox ancester = FindContainerForCenteredBox();
                        if (box.ParentBox != null)
                        {
                            if (box.ParentBox != ancester)
                            {
                                //TODO: remove from current parent box
                                //and insert to newBOx
                                throw new NotSupportedException();
                            }
                        }
                        else
                        {
                            ancester.AppendToAbsoluteLayer(box);
                        }
                    }
                    break;
                default:
                    {
                        _aa_boxes.AddChild(this, box);
                    }
                    break;
            }
        }
        //
        internal bool HasAbsoluteLayer => _absPosLayer != null;

        public void InsertChild(CssBox beforeBox, CssBox box)
        {
            _aa_boxes.InsertBefore(this, beforeBox, box);
        }
        public virtual void Clear()
        {
            //_aa_contentRuns may come from other data source
            //so just set it to null 
            _clientLineBoxes = null;
            _aa_contentRuns = null;
            _aa_boxes.Clear();
        }
        CssBox FindContainerForAbsoluteBox()
        {
            var node = this;
            while (node.Position == Css.CssPosition.Static)
            {
                if (node.ParentBox == null)
                {
                    return node;
                }
                else
                {
                    node = node.ParentBox;
                }
            }
            return node;
        }
        CssBox FindContainerForFixedBox()
        {
            var node = this;
            //its viewport
            while (node.ParentBox != null)
            {
                node = node.ParentBox;
            }
            return node;
        }
        CssBox FindContainerForCenteredBox()
        {
            //similar to absolutes
            var node = this;
            while (node.Position == Css.CssPosition.Static)
            {
                if (node.ParentBox == null)
                {
                    return node;
                }
                else
                {
                    node = node.ParentBox;
                }
            }
            return node;
        }
        /// <summary>
        /// append box to this element's absolute layer
        /// </summary>
        /// <param name="box"></param>
        public void AppendToAbsoluteLayer(CssBox box)
        {
            //find proper ancestor node for absolute position 
            if (_absPosLayer == null)
            {
                _absPosLayer = new CssBoxAbsoluteLayer();
            }
            if (box._absLayerOwner != null)
            {
                if (box._absLayerOwner == this)
                {
                    //ok , already in abs layer
                    return;
                }
                else
                {
                    //?
                }
            }
            box._absLayerOwner = this;
            if (box.ParentBox != null)
            {

                //box.ParentBox.RemoveChild(box);
            }
            _absPosLayer.AddChild(box);
        }
        internal bool IsAddedToAbsoluteLayer => _absLayerOwner != null;
        internal void ResetAbsoluteLayerOwner()
        {
            _absLayerOwner = null;
        }
        //-------------------------------------
        internal void ResetLineBoxes()
        {
            if (_clientLineBoxes != null)
            {
                _clientLineBoxes.Clear();
            }
            else
            {
                _clientLineBoxes = new LinkedList<CssLineBox>();
            }
        }
        //-------------------------------------
        internal int RowSpan => _rowSpan;
        internal int ColSpan => _colSpan;



        public void SetRowSpanAndColSpan(int rowSpan, int colSpan)
        {
            _rowSpan = rowSpan;
            _colSpan = colSpan;
        }
        /// <summary>
        /// The margin top value if was effected by margin collapse.
        /// </summary>
        float CollapsedMarginTop { get; set; }
        protected bool HasCustomRenderTechnique
        {
            get => (_boxCompactFlags & BoxFlags.HAS_CUSTOM_RENDER_TECHNIQUE) != 0;
            set
            {
                if (value)
                {
                    _boxCompactFlags |= BoxFlags.HAS_CUSTOM_RENDER_TECHNIQUE;
                }
                else
                {
                    _boxCompactFlags &= ~BoxFlags.HAS_CUSTOM_RENDER_TECHNIQUE;
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
            _parentBox._aa_boxes.dbugChangeSiblingIndex(_parentBox, this, siblingIndex);
        }
#endif

    }
}