//BSD 2014, WinterDev 
//ArthurHub

using System;
using System.Collections.Generic;
using HtmlRenderer.Entities;
using HtmlRenderer.Utils;
using HtmlRenderer.WebDom;

namespace HtmlRenderer.Dom
{


    class BridgeHtmlElement : HtmlElement
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
        TextSplits originalSplits;

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
        internal void SetSplitParts(TextSplits splits, bool hasSomeChar)
        {
            this.originalSplits = splits;
            this.hasSomeChar = hasSomeChar;
        }
        internal TextSplits GetSplitParts()
        {
            return originalSplits;
        }

#if DEBUG
        public override string ToString()
        {
            return new string(base.GetOriginalBuffer());
        }
#endif
    }
    enum TextSplitPartKind : byte
    {
        Text,
        Whitespace,
        SingleWhitespace,
        LineBreak,
    }

    struct TextSplits
    {
        public readonly ushort singleChar;
        public readonly List<ushort> encodedSplits;
        public TextSplits(ushort singleChar, List<ushort> encodedSplits)
        {
            this.singleChar = singleChar;
            this.encodedSplits = encodedSplits;
        }
    }

    //class TextSplitStreamWriter
    //{
    //    public TextSplitStreamWriter()
    //    {

    //    }
    //}



    //    struct TextSplitPart
    //    {
    //        public readonly TextSplitPartKind kind;
    //        public readonly ushort length;
    //        public TextSplitPart(ushort length, TextSplitPartKind kind)
    //        {

    //            this.length = length;
    //            this.kind = kind;
    //        }

    //#if DEBUG
    //        //public readonly int dbugId;
    //        //static int dbugTotalId;
    //        public override string ToString()
    //        {
    //            return kind.ToString() + "(" + length + ")";
    //        }
    //#endif
    //    }

}