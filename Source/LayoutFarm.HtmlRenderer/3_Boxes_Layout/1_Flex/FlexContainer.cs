// 2015 ,MIT, WinterDev  

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using System.Globalization;
using System.Text;
using LayoutFarm.Css;

namespace LayoutFarm.HtmlBoxes
{
    //implement flex formatting context ...

    class FlexContainer
    {
        //if multiline
        List<FlexLine> flexLines;
        FlexLine singleLine;
        public FlexContainer()
        {

        }
        public bool IsMultiline { get; set; }
        public FlexWrap FlexWrap { get; set; }
        public FlexFlowDirection FlexFlowDirection { get; set; }
    }
    class FlexItem
    {

        public FlexItem()
        {
            this.MinWidth = CssLength.AutoLength;
            this.MinHeight = CssLength.AutoLength;
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
    }
}