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
        ushort[] splitBuffer;
        int splitPartCount;

        public BridgeHtmlTextNode(HtmlDocument ownerDoc, char[] buffer)
            : base(ownerDoc, buffer)
        {
            this.splitBuffer = new ushort[buffer.Length];

        }
        public bool IsWhiteSpace
        {
            get
            {
                return !this.hasSomeChar;
            }
        }
        internal ushort[] GetSplitBuffer()
        {
            return this.splitBuffer;
        }

        internal void SetSplitPartCount(int len, bool hasSomeChar)
        {
            this.hasSomeChar = hasSomeChar;
            this.splitPartCount = len;
        }

        internal int SplitPartCount
        {
            get { return this.splitPartCount; }
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
        Text = 1,
        Whitespace,
        SingleWhitespace,
        LineBreak,
    }





}