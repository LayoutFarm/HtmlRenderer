//MIT, 2015-present, WinterDev  


using System.Collections.Generic;
using LayoutFarm.Css;
namespace LayoutFarm.HtmlBoxes
{
    //here not follow exact w3c
    //just experimnent only

    //TODO: fix to complaint with w3c spec

    class FlexContainer
    {
        //if multiline
        List<FlexLine> _flexLines;
        FlexLine _singleLine;
        public bool IsMultiline { get; set; }
        public FlexWrap FlexWrap { get; set; }
        public FlexDirection FlexFlowDirection { get; set; }
    }
    class FlexItem
    {
        CssBox _box;
        float _minSizeW;
        float _minSizeH;
        float _maxSizeW;
        float _maxSizeH;
        const int MAXSIZE_W = 99999;
        const int MAXSIZE_H = 99999;
        public FlexItem(CssBox box)
        {
            _box = box;
            this.MinWidth = CssLength.AutoLength;
            this.MinHeight = CssLength.AutoLength;
            if (box.Width.IsEmptyOrAuto)
            {
                //auto
                _minSizeW = box.InnerContentWidth;
                _maxSizeW = MAXSIZE_W;
            }
            else
            {
                _maxSizeW = _minSizeW = box.VisualWidth;
            }
            if (box.Height.IsEmptyOrAuto)
            {
                _minSizeH = box.InnerContentHeight;
                _maxSizeH = MAXSIZE_H;
            }
            else
            {
                _maxSizeH = _minSizeH = box.VisualHeight;
            }
            this.PlanWidth = _minSizeW;
            this.PlanHeight = _minSizeH;
        }
     
        public int FlexShrink => CssBox.UnsafeGetBoxSpec(_box).FlexShrink;

        public int FlexGrow => CssBox.UnsafeGetBoxSpec(_box).FlexGrow;

        public float MinSizeW => _minSizeW;

        public float MinSizeH => _minSizeH;

        public float MaxSizeW => _maxSizeW;

        public float MaxSizeH => _maxSizeH;

        public float PlanWidth { get; set; }
        public float PlanHeight { get; set; }
        public bool ReachMinWidth => this.PlanWidth <= MinSizeW;

        public bool ReachMinHeight => this.PlanHeight <= MinSizeH;

        public bool ReachMaxWidth => this.PlanWidth >= _maxSizeW;

        public bool ReachMaxHeight => this.PlanHeight >= _maxSizeH;

        public CssBox Box => _box;

        public CssLength FlexBasis { get; set; }

        public int DisplayOrder { get; set; }
        public CssLength MinWidth { get; set; }
        public CssLength MinHeight { get; set; }
    }

    class FlexLine
    {
        List<FlexItem> _flexItems = new List<FlexItem>();
        CssBox _flexCssBox;
        public FlexLine(CssBox flexCssBox)
        {
            _flexCssBox = flexCssBox;
            AvaliableParentWidth = flexCssBox.VisualWidth;
            AvaliableParentHeight = flexCssBox.VisualHeight;
        }
        public void AddChild(FlexItem item)
        {
            _flexItems.Add(item);
        }
        public void Clear()
        {
            _flexItems.Clear();
        }
        public int Count => _flexItems.Count;

        public FlexItem GetItem(int index) => _flexItems[index];


        float AvaliableParentWidth { get; set; }
        float AvaliableParentHeight { get; set; }

        public float LineHeightAfterArrange { get; private set; }
        public float LineWidthAfterArrange { get; private set; }
        public void Arrange()
        {
            int j = _flexItems.Count;
            float curX = 0;
            float curY = 0;
            float availableW = AvaliableParentWidth;
            for (int i = 0; i < j; ++i)
            {
                FlexItem flexItem = _flexItems[i];
                CssBox box = flexItem.Box;
                box.SetLocation(curX, curY);
                curX += flexItem.PlanWidth;
            }
            //-----------------------------------------------
            if (curX < availableW)
            {
                //find box that can expand
                List<FlexItem> widthResizableItems = new List<FlexItem>();
                for (int i = 0; i < j; ++i)
                {
                    FlexItem flexItem = _flexItems[i];
                    if (!flexItem.ReachMaxHeight)
                    {
                        widthResizableItems.Add(flexItem);
                    }
                }

                //remain some space
                //so expand it
                if ((j = widthResizableItems.Count) > 0)
                {
                    //how to expand it
                    //1. check grow feature
                    int totalExpandCount = 0;
                    for (int i = j - 1; i >= 0; --i)
                    {
                        totalExpandCount += widthResizableItems[i].FlexGrow;
                    }

                    if (totalExpandCount > 0)
                    {
                        float remainingW = availableW - curX;
                        float onePart = remainingW / totalExpandCount;
                        //add to plan width
                        for (int i = j - 1; i >= 0; --i)
                        {
                            widthResizableItems[i].PlanWidth += (onePart * widthResizableItems[i].FlexGrow);
                        }

                        //then rearrange the line again
                        curX = 0;//reset
                        for (int i = 0; i < j; ++i)
                        {
                            FlexItem flexItem = _flexItems[i];
                            CssBox box = flexItem.Box;
                            box.SetLocation(curX, curY);
                            box.SetVisualSize(flexItem.PlanWidth, flexItem.PlanHeight);
                            curX += flexItem.PlanWidth;
                        }
                    }
                }
            }
            else if (curX > availableW)
            {
                //use more than available width
                //find if it can shrink?

            }

            this.LineWidthAfterArrange = curX;
            //-----------------------------------------------
            //check for height
            float maxHeight = 0;
            for (int i = _flexItems.Count - 1; i >= 0; --i)
            {
                FlexItem flexItem = _flexItems[i];
                CssBox box = flexItem.Box;
                if (maxHeight < box.VisualHeight)
                {
                    maxHeight = box.VisualHeight;
                }
            }
            if (maxHeight < this.AvaliableParentHeight)
            {
                //expand item or shrink
                if (_flexCssBox.Height.IsEmptyOrAuto)
                {
                    //autoheight 
                    //then set new height for parent
                    this.LineHeightAfterArrange = maxHeight;
                }
                else
                {
                    //try expand flex item  
                    for (int i = _flexItems.Count - 1; i >= 0; --i)
                    {
                        FlexItem flexItem = _flexItems[i];
                        if (!flexItem.ReachMaxHeight)
                        {
                            flexItem.Box.SetVisualHeight(this.AvaliableParentHeight);
                        }
                    }
                    this.LineHeightAfterArrange = this.AvaliableParentHeight;
                }
            }
        }
    }
}