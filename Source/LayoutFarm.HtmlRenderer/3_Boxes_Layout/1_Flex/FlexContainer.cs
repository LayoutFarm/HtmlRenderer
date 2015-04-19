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
            }
            else
            {
                minSizeW = box.SizeWidth;
            }
            if (box.Height.IsEmptyOrAuto)
            {
                minSizeH = box.InnerContentHeight;
            }
            else
            {
                minSizeH = box.SizeHeight;

            }
            this.PlanWidth = minSizeW;
            this.PlanHeight = minSizeH;

        }
        public float MinSizeW
        {
            get { return this.minSizeW; }
        }
        public float MinSizeH
        {
            get { return this.minSizeH; }
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



        public CssBox Box
        {
            get { return this.box; }
        }
        public CssLength FlexBasis { get; set; }
        public int FlexGrow { get; set; }
        public int FlexShrink { get; set; }
        public int DisplayOrder { get; set; }
        public CssLength MinWidth { get; set; }
        public CssLength MinHeight { get; set; }
    }

    class FlexLine
    {
        List<FlexItem> flexItems = new List<FlexItem>();
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

        public float AvaliableParentWidth { get; set; }
        public float AvaliableParentHeight { get; set; }

        public float LineHeightAfterArrange { get; private set; }

        public void Arrange()
        {
            int j = flexItems.Count;
            float curX = 0;
            float curY = 0;
            float availableW = AvaliableParentWidth;
            List<FlexItem> widthResizableItems = new List<FlexItem>();
            List<FlexItem> heightResizableItems = new List<FlexItem>();


            for (int i = 0; i < j; ++i)
            {
                FlexItem flexItem = flexItems[i];
                CssBox box = flexItem.Box;
                box.SetLocation(curX, curY);
                curX += flexItem.PlanWidth;

                

                if (!flexItem.ReachMinWidth)
                {
                    widthResizableItems.Add(flexItem);
                }
                if (!flexItem.ReachMinHeight)
                {
                    heightResizableItems.Add(flexItem);
                }
            }

            //-----------------------------------------------
            if (curX < availableW)
            {
                //remain some space
                //so expand it
                if (widthResizableItems.Count > 0)
                {

                }
            }
            else if (curX > availableW)
            {
                //use more than available width
                //find if it can shrink?

            }
            //-----------------------------------------------
            float maxHeight = 0;
            for (int i = 0; i < j; ++i)
            {
                FlexItem flexItem = flexItems[i];
                CssBox box = flexItem.Box;
                if (maxHeight < box.SizeHeight)
                {
                    maxHeight = box.SizeHeight;
                }
            }
            this.LineHeightAfterArrange = maxHeight;
        }
    }
}