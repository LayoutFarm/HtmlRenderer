//BSD 2014, WinterDev 
//ArthurHub

using System;
using System.Collections.Generic;
using HtmlRenderer.Diagnostics;
using HtmlRenderer.Drawing;
using HtmlRenderer.WebDom;
using HtmlRenderer.Boxes;

namespace HtmlRenderer.Composers.BridgeHtml
{
    class HtmlElement : DomElement
    {

        CssBox principalBox;
        Css.BoxSpec boxSpec;
        CssRuleSet elementRuleSet;

        public HtmlElement(BridgeHtmlDocument owner, int prefix, int localNameIndex)
            : base(owner, prefix, localNameIndex)
        {
            this.boxSpec = new Css.BoxSpec();
        }
        public Css.BoxSpec Spec
        {
            get { return this.boxSpec; }
        }
        public WellKnownDomNodeName WellknownElementName { get; set; }
        public bool TryGetAttribute(WellknownName wellknownHtmlName, out DomAttribute result)
        {
            var found = base.FindAttribute((int)wellknownHtmlName);
            if (found != null)
            {
                result = found;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
        public bool TryGetAttribute(WellknownName wellknownHtmlName, out string value)
        {
            DomAttribute found;
            if (this.TryGetAttribute(wellknownHtmlName, out found))
            {
                value = found.Value;
                return true;
            }
            else
            {
                value = null;
                return false;
            }

        }

        //------------------------------------
        protected CssBox GetPrincipalBox()
        {
            return this.principalBox;
        }

        internal void SetPrincipalBox(CssBox box)
        {
            this.principalBox = box;
            this.SkipPrincipalBoxEvalulation = true;
        }

        internal bool SkipPrincipalBoxEvalulation
        {
            get;
            set;

        }
        internal static CssBox InternalGetPrincipalBox(HtmlElement element)
        {
            return element.principalBox;
        }
        //------------------------------------
        protected override void OnChangeInIdleState(ElementChangeKind changeKind)
        {
            //1. 
            this.OwnerDocument.SetDocumentState(DocumentState.ChangedAfterIdle);
            //2.
            this.SkipPrincipalBoxEvalulation = false;
            var cnode = this.ParentNode;
            while (cnode != null)
            {
                ((HtmlElement)cnode).SkipPrincipalBoxEvalulation = false;
                cnode = cnode.ParentNode;
            }
        }

        internal static void InvokeNotifyChangeOnIdleState(HtmlElement elem, ElementChangeKind changeKind)
        {
            elem.OnChangeInIdleState(changeKind);
        }

        internal CssRuleSet ElementRuleSet
        {
            get
            {
                return this.elementRuleSet;
            }
            set
            {
                this.elementRuleSet = value;
            }
        }
        internal bool IsStyleEvaluated
        {
            get;
            set;
        }
        //------------------------------------
        protected override void OnMouseDown(HtmlEventArgs e)
        {   


            var box = this.GetPrincipalBox();
            if (box != null)
            {

                SvgRootBox svgBox = box as SvgRootBox;
                if (svgBox != null)
                {
                    SvgHitChain hitChain = new SvgHitChain();
                    svgBox.HitTestCore(hitChain, e.X, e.Y);

                    PropagateEventOnBubblingPhase(hitChain, e);
                }
            }
            base.OnMouseDown(e);
        }
        static void PropagateEventOnBubblingPhase(SvgHitChain hitChain, HtmlEventArgs eventArgs)
        {
            int hitCount = hitChain.Count;
            //then propagate
            for (int i = hitCount - 1; i >= 0; --i)
            {
                SvgHitInfo hitInfo = hitChain.GetHitInfo(i);
                
            }
        }
    }

    sealed class RootElement : HtmlElement
    {
        public RootElement(BridgeHtmlDocument ownerDoc)
            : base(ownerDoc, 0, 0)
        {
        }
    }

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

    }

    enum TextSplitPartKind : byte
    {
        Text = 1,
        Whitespace,
        SingleWhitespace,
        LineBreak,
    }


    static class HtmlPredefineNames
    {

        static readonly ValueMap<WellknownName> _wellKnownHtmlNameMap =
            new ValueMap<WellknownName>();

        static UniqueStringTable htmlUniqueStringTableTemplate = new UniqueStringTable();

        static HtmlPredefineNames()
        {
            int j = _wellKnownHtmlNameMap.Count;
            for (int i = 0; i < j; ++i)
            {
                htmlUniqueStringTableTemplate.AddStringIfNotExist(_wellKnownHtmlNameMap.GetStringFromValue((WellknownName)(i + 1)));
            }
        }
        public static UniqueStringTable CreateUniqueStringTableClone()
        {
            return htmlUniqueStringTableTemplate.Clone();
        }

    }

}