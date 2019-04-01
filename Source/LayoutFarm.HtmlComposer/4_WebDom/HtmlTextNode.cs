//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.WebDom;
namespace LayoutFarm.Composers
{
    public class HtmlTextNode : DomTextNode
    {
        //---------------------------------
        //this node may be simple text node  
        bool _freeze;
        bool _hasSomeChar;
        List<CssRun> _runs;
        bool _setSplitParts;

        internal HtmlTextNode(WebDocument ownerDoc, char[] buffer)
            : base(ownerDoc, buffer)
        {
        }
        //
        public bool IsWhiteSpace => !_hasSomeChar;
        //
        internal void SetSplitParts(List<CssRun> runs, bool hasSomeChar)
        {
            _freeze = false;
            _runs = runs;
            _hasSomeChar = hasSomeChar;
            _setSplitParts = true;
        }
        internal bool HasSetSplitPart => _setSplitParts;

        public bool IsFreeze => _freeze;

#if DEBUG
        public override string ToString()
        {
            return new string(base.GetOriginalBuffer());
        }
#endif

        internal List<CssRun> InternalGetRuns()
        {
            _freeze = true;
            return _runs;
        }
        public void WriteTextNode(DomTextWriter writer)
        {
            //write inner run
            writer.Write(this.GetOriginalBuffer());
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