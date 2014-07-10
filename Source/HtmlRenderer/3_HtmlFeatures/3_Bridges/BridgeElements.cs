//BSD 2014, WinterDev 
//ArthurHub

using System;
using System.Collections.Generic;
using HtmlRenderer.Entities;
using HtmlRenderer.Utils;
using HtmlRenderer.WebDom;

namespace HtmlRenderer.Dom
{

    

    public class BridgeHtmlElement : HtmlElement
    {
        BoxSpec boxSpec;
        public BridgeHtmlElement(HtmlDocument owner, int prefix, int localNameIndex)
            : base(owner, prefix, localNameIndex)
        {
            this.boxSpec = new BoxSpec();
        }
        public BoxSpec Spec
        {
            get { return this.boxSpec; }

        }
        public string GetAttributeValue(string attrName, string defaultValue)
        {
            var attr = base.FindAttribute(attrName);
            if (attr == null)
            {
                return defaultValue;
            }
            else
            {
                return attr.Value;
            }
        }
        public bool TryGetAttribute2(string attrName, out string value)
        {
            var attr = base.FindAttribute(attrName);
            if (attr == null)
            {
                value = null;
                return false;
            }
            else
            {
                value = attr.Value;
                return true;
            }
        }
    }


    sealed class BrigeRootElement : BridgeHtmlElement
    {
        public BrigeRootElement(HtmlDocument ownerDoc)
            : base(ownerDoc, 0, 0)
        {
        }
    }

    class BridgeHtmlTextNode : HtmlTextNode
    {
        //---------------------------------
        //this node may be simple text node  
        bool hasSomeChar;
        List<CssRun> runs;
        public BridgeHtmlTextNode(HtmlDocument ownerDoc, char[] buffer)
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
            this.runs = runs;
            this.hasSomeChar = hasSomeChar;
        }

#if DEBUG
        public override string ToString()
        {
            return new string(base.GetOriginalBuffer());
        }
#endif

        internal List<CssRun> InternalGetRuns()
        {
            return this.runs;
        }

    }


    enum TextSplitPartKind : byte
    {
        Text = 1,
        Whitespace,
        SingleWhitespace,
        LineBreak,
    }

    struct TextSplits
    {
        public readonly bool isWS;
        public readonly int startIndex;
        public readonly int totalLength;
        public ushort[] encodedSplits;
        public TextSplits(bool isWS, int startIndex, int totalLength)
        {
            this.isWS = isWS;
            this.startIndex = startIndex;
            this.totalLength = totalLength;
            this.encodedSplits = null;
        }
    }


}