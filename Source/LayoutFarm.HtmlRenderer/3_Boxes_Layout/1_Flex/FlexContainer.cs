// 2015 ,MIT, WinterDev  

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using System.Globalization;
using System.Text;
using LayoutFarm.Css;

namespace LayoutFarm.HtmlBoxes
{

    //here not follow exact w3c
    //just experimnent only

    //TODO: fix to complaint with w3c spec

    class FlexContainer
    {
        //if multiline
        List<FlexLine> flexLines;
        FlexLine singleLine;
        public bool IsMultiline { get; set; }
        public FlexWrap FlexWrap { get; set; }
        public FlexFlowDirection FlexFlowDirection { get; set; }
    }
    class FlexItem
    {
        CssBox box;
        float minSizeW;
        float minSizeH;
        float maxSizeW;
        float maxSizeH;
        const int MAXSIZE_W = 99999;
        const int MAXSIZE_H = 99999;

        public FlexItem(CssBox box)
        {
            this.box = box;
            this.MinWidth = CssLength.AutoLength;
            this.MinHeight = CssLength.AutoLength;

            if (box.Width.IsEmptyOrAuto)
            {
                //auto
                minSizeW = box.InnerContentWidth;
                maxSizeW = MAXSIZE_W;
            }
            else
            {
                maxSizeW = minSizeW = box.VisualWidth;

            }
            if (box.Height.IsEmptyOrAuto)
            {
                minSizeH = box.InnerContentHeight;
                maxSizeH = MAXSIZE_H;
            }
            else
            {
                maxSizeH = minSizeH = box.VisualHeight;
            }
            this.PlanWidth = minSizeW;
            this.PlanHeight = minSizeH;

        }
        public int FlexShrink
        {
            get
            {
                return CssBox.UnsafeGetBoxSpec(this.box).FlexShrink;
            }
        }
        public int FlexGrow
        {
            get
            {
                return CssBox.UnsafeGetBoxSpec(this.box).FlexGrow;
            }
        }
        public float MinSizeW
        {
            get { return this.minSizeW; }
        }
        public float MinSizeH
        {
            get { return this.minSizeH; }
        }
        public float MaxSizeW
        {
            get { return this.maxSizeW; }
        }
        public float MaxSizeH
        {
            get { return this.maxSizeH; }
        }
        public float PlanWidth
        {
            get;
            set;
        }
        public float PlanHeight
        {
            get;
            set;
        }
        public bool ReachMinWidth
        {
            get { return this.PlanWidth <= MinSizeW; }
        }
        public bool ReachMinHeight
        {
            get { return this.PlanHeight <= MinSizeH; }
        }
        public bool ReachMaxWidth
        {
            get { return this.PlanWidth >= maxSizeW; }
        }
        public bool ReachMaxHeight
        {
            get { return this.PlanHeight >= maxSizeH; }
        }


        public CssBox Box
        {
            get { return this.box; }
        }
        public CssLength FlexBasis { get; set; }

        public int DisplayOrder { get; set; }
        public CssLength MinWidth { get; set; }
        public CssLength MinHeight { get; set; }
    }

    class FlexLine
    {
        List<FlexItem> flexItems = new List<FlexItem>();
        CssBox flexCssBox;
        public FlexLine(CssBox flexCssBox)
        {
            this.flexCssBox = flexCssBox;
            AvaliableParentWidth = flexCssBox.VisualWidth;
            AvaliableParentHeight = flexCssBox.VisualHeight;
        }
        public void AddChild(FlexItem item)
        {
            this.flexItems.Add(item);
        }
        public void Clear()
        {
            this.flexItems.Clear();
        }
        public int Count
        {
            get { return this.flexItems.Count; }
        }
        public FlexItem GetItem(int index)
        {
            return this.flexItems[index];
        }

        float AvaliableParentWidth { get; set; }
        float AvaliableParentHeight { get; set; }

        public float LineHeightAfterArrange { get; private set; }
        public float LineWidthAfterArrange { get; private set; }
        public void Arrange()
        {
            int j = flexItems.Count;
            float curX = 0;
            float curY = 0;
            float availableW = AvaliableParentWidth;

            for (int i = 0; i < j; ++i)
            {
                FlexItem flexItem = flexItems[i];
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
                    FlexItem flexItem = flexItems[i];
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
                            FlexItem flexItem = flexItems[i];
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
            for (int i = flexItems.Count - 1; i >= 0; --i)
            {
                FlexItem flexItem = flexItems[i];
                CssBox box = flexItem.Box;
                if (maxHeight < box.VisualHeight)
                {
                    maxHeight = box.VisualHeight;
                }
            }
            if (maxHeight < this.AvaliableParentHeight)
            {
                //expand item or shrink
                if (this.flexCssBox.Height.IsEmptyOrAuto)
                {
                    //autoheight 
                    //then set new height for parent
                    this.LineHeightAfterArrange = maxHeight;
                }
                else
                {
                    //try expand flex item  
                    for (int i = flexItems.Count - 1; i >= 0; --i)
                    {
                        FlexItem flexItem = flexItems[i];
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