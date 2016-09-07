//BSD, 2014-2016, WinterDev 
//ArthurHub  , Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.WebDom;
namespace LayoutFarm.Composers
{
    class HtmlTextNode : DomTextNode
    {
        //---------------------------------
        //this node may be simple text node  
        bool freeze;
        bool hasSomeChar;
        List<CssRun> runs;
        public HtmlTextNode(WebDocument ownerDoc, char[] buffer)
            : base(ownerDoc, buffer)
        {
        }
        public bool IsWhiteSpace
        {
            get
            {
                return !this.hasSomeChar;
            }
        }
        internal void SetSplitParts(List<CssRun> runs, bool hasSomeChar)
        {
            this.freeze = false;
            this.runs = runs;
            this.hasSomeChar = hasSomeChar;
        }
        public bool IsFreeze
        {
            get { return this.freeze; }
        }
#if DEBUG
        public override string ToString()
        {
            return new string(base.GetOriginalBuffer());
        }
#endif

        internal List<CssRun> InternalGetRuns()
        {
            this.freeze = true;
            return this.runs;
        }
        public void WriteTextNode(DomTextWriter writer)
        {
            //write inner run
            writer.InnerStringBuilder.Append(this.GetOriginalBuffer());
        }
    }
    public enum TextSplitPartKind : byte
    {
        Text = 1,
        Whitespace,
        SingleWhitespace,
        LineBreak,
    }
}