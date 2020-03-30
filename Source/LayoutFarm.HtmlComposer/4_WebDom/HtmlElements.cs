//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.UI;
using LayoutFarm.WebDom;
namespace LayoutFarm.Composers
{

    //out actual html element implementation

    static class CssParserPool
    {
        static Stack<WebDom.Parser.CssParser> s_pool = new Stack<WebDom.Parser.CssParser>();
        public static WebDom.Parser.CssParser GetFreeParser()
        {
            if (s_pool.Count > 0) return s_pool.Pop();
            return new WebDom.Parser.CssParser();
        }
        public static void ReleaseParser(ref WebDom.Parser.CssParser parser)
        {
            s_pool.Push(parser);
        }
    }



    /// <summary>
    /// general html implementation
    /// </summary>
    public class HtmlElement : LayoutFarm.WebDom.Impl.HtmlElement
    {
        protected CssBox _principalBox;
        protected Css.BoxSpec _boxSpec;
        HtmlElement _subParentNode;

        internal HtmlElement(HtmlDocument owner, int prefix, int localNameIndex)
            : base(owner, prefix, localNameIndex)
        {
            _boxSpec = new Css.BoxSpec();
        }
        public override void AddChild(DomNode childNode)
        {
#if DEBUG
            if (_principalBox != null && childNode.NodeKind == HtmlNodeKind.TextNode)
            {
            }
#endif
            base.AddChild(childNode);
        }
        public HtmlElement SubParentNode => _subParentNode;

        public void SetSubParentNode(HtmlElement subParentNode)
        {
            _subParentNode = subParentNode;
        }
        public override void SetAttribute(DomAttribute attr)
        {
            SetDomAttribute(attr);
            ImplSetAttribute(attr);
        }

        protected void ImplSetAttribute(DomAttribute attr)
        {

            //handle some attribute
            //special for some attributes 

            switch ((WellknownName)attr.LocalNameIndex)
            {
                case WellknownName.Style:
                    {
                        //TODO: parse and evaluate style here 
                        //****
                        WebDom.Parser.CssParser miniCssParser = CssParserPool.GetFreeParser();
                        //parse and evaluate the ruleset
                        CssRuleSet parsedRuleSet = miniCssParser.ParseCssPropertyDeclarationList(attr.Value.ToCharArray());

                        Css.BoxSpec spec = null;
                        if (this.ParentNode != null)
                        {
                            spec = ((HtmlElement)this.ParentNode).Spec;
                        }
                        foreach (WebDom.CssPropertyDeclaration propDecl in parsedRuleSet.GetAssignmentIter())
                        {

                            SpecSetter.AssignPropertyValue(
                                _boxSpec,
                                spec,
                                propDecl);
                        }
                        CssParserPool.ReleaseParser(ref miniCssParser);
                    }
                    break;
            }
        }
        public override void ClearAllElements()
        {
            //clear presentation 
            if (_principalBox != null)
            {
                _principalBox.Clear();
            }
            base.ClearAllElements();
        }

        protected override void OnElementChangedInIdleState(ElementChangeKind changeKind, DomAttribute attr)
        {
            //1. 
            this.OwnerDocument.SetDocumentState(DocumentState.ChangedAfterIdle);
            if (this.OwnerDocument.IsDocFragment) return;
            //------------------------------------------------------------------
            //2. need box-evaluation again ? 
            //we need to check that attr affect the dom or not
            //eg. font-color, bgcolor => not affect element size/layout
            //    custom attr => not affect element size/layout

            this.SkipPrincipalBoxEvalulation = false;



            ////-------------------
            //if (this.WellknownElementName == WellKnownDomNodeName.img)
            //{
            //    if (attr != null && attr.Name == "src")
            //    {
            //        //TODO: review this
            //        //has local effect
            //        //no propagation up
            //        return;
            //    }
            //}
            ////-------------------

            //3. propagate
            HtmlElement cnode = (HtmlElement)this.ParentNode;
            HtmlDocument owner = this.OwnerDocument as HtmlDocument;
            while (cnode != null)
            {
                cnode.SkipPrincipalBoxEvalulation = false;
                if (cnode.ParentNode != null)
                {
                    cnode = (HtmlElement)cnode.ParentNode;
                }
                else
                {
                    if (cnode.SubParentNode != null)
                    {
                        cnode = (HtmlElement)cnode.SubParentNode;
                    }
                    else
                    {
                        cnode = null;
                    }
                }
            }

            owner.IncDomVersion();

        }
        protected override void OnElementChanged()
        {
            CssBox box = _principalBox;
            if (box is CssScrollView)
            {
                return;
            }
            //change 
            var boxSpec = CssBox.UnsafeGetBoxSpec(box);

            //create scrollbar
            //...?
            //???
            var scrollView = new CssScrollView(((HtmlDocument)this.OwnerDocument).Host, boxSpec, box.RootGfx);
            scrollView.SetController(this);
            scrollView.SetVisualSize(box.VisualWidth, box.VisualHeight);
            scrollView.SetExpectedSize(box.VisualWidth, box.VisualHeight);
            box.ParentBox.InsertChild(box, scrollView);
            box.ParentBox.RemoveChild(box);
            //scrollbar width= 10
            scrollView.SetInnerBox(box);
            //change primary render element
            _principalBox = scrollView;
            scrollView.InvalidateGraphics();
        }
        public override void SetInnerHtml(string innerHtml)
        {
            // parse html and create dom node
            //clear content of this node
            this.ClearAllElements();
            //then apply new content 
            WebDocumentParser.ParseHtmlDom(
                new LayoutFarm.WebDom.Parser.TextSource(innerHtml.ToCharArray()),
                (HtmlDocument)this.OwnerDocument,
                this);
        }
        public override void GetGlobalLocation(out int x, out int y)
        {
            float globalX, globalY;
            _principalBox.GetGlobalLocation(out globalX, out globalY);
            x = (int)globalX;
            y = (int)globalY;
        }
        public override void GetGlobalLocationRelativeToRoot(out int x, out int y)
        {

            float globalX, globalY;
            _principalBox.GetGlobalLocationRelativeToRoot(out globalX, out globalY);
            x = (int)globalX;
            y = (int)globalY;
        }
        public override void SetLocation(int x, int y)
        {
            if (_principalBox != null)
            {
                _principalBox.SetLocation(x, y);
            }
            else
            {
                DomAttribute domAttr;
                if (this.TryGetAttribute(WellknownName.Style, out domAttr))
                {
                    //TODO: add to domAttr?
                    domAttr.Value += ";left:" + x + "px;top:" + y + "px;";
                }
                else
                {
                    //create new
                    throw new NotSupportedException();
                }
            }
        }
        internal void SetPrincipalBox(CssBox box)
        {
            _principalBox = box;
            this.SkipPrincipalBoxEvalulation = true;
        }

        public override float ActualWidth => _principalBox.VisualWidth;

        public override float ActualHeight => _principalBox.VisualHeight;

        //-------------------------------------------
        internal virtual CssBox GetPrincipalBox(CssBox parentCssBox, HtmlHost host)
        {
            //this method is called when HasCustomPrincipalBoxGenerator = true
            throw new NotImplementedException();
        }
        //
        internal virtual bool HasCustomPrincipalBoxGenerator => false;//use builtin cssbox generator***
        internal bool IsStyleEvaluated
        {
            get;
            set;
        }
        internal bool SkipPrincipalBoxEvalulation
        {
            get;
            set;
        }
        //
        internal static CssBox InternalGetPrincipalBox(HtmlElement element) => element._principalBox;
        //
        internal Css.BoxSpec Spec => _boxSpec;
        //
        internal CssBox CurrentPrincipalBox => _principalBox;

        public override bool RemoveChild(DomNode childNode)
        {
            //remove presentation
            var childElement = childNode as HtmlElement;
            if (childElement != null && childElement.ParentNode == this)
            {
                if (_principalBox != null)
                {
                    var cssbox = childElement.CurrentPrincipalBox;
                    if (cssbox != null)
                    {
                        //remove from parent
                        _principalBox.RemoveChild(cssbox);
                    }
                }
            }
            return base.RemoveChild(childNode);
        }

        public HtmlDocument OwnerHtmlDoc => OwnerDocument as HtmlDocument;



        public bool HasSpecialPresentation { get; set; }

        public System.Action<object> SpecialPresentationUpdate;

        //---------------------------------------------------------------------------------
        //TODO: review how to handle events
        HtmlEventHandler _mouseDownEventHandler;
        HtmlEventHandler _mouseMoveEventHandler;
        HtmlEventHandler _mouseUpEventHandler;
        HtmlEventHandler _mouseLostFocusEventHandler;
        HtmlEventHandler _keyDownEventHandler;

        public override void AttachEvent(UIEventName eventName, HtmlEventHandler handler)
        {
            switch (eventName)
            {
                case UIEventName.MouseDown:
                    {
                        if (_mouseDownEventHandler == null)
                        {
                            _mouseDownEventHandler = handler;
                        }
                        else
                        {
                            _mouseDownEventHandler += handler;
                        }
                    }
                    break;
                case UIEventName.MouseMove:
                    {
                        if (_mouseMoveEventHandler == null)
                        {
                            _mouseMoveEventHandler = handler;
                        }
                        else
                        {
                            _mouseMoveEventHandler += handler;
                        }
                    }
                    break;
                case UIEventName.MouseUp:
                    {
                        if (_mouseUpEventHandler == null)
                        {
                            _mouseUpEventHandler = handler;
                        }
                        else
                        {
                            _mouseUpEventHandler += handler;
                        }
                    }
                    break;
                case UIEventName.MouseLostFocus:
                    {
                        if (_mouseLostFocusEventHandler == null)
                        {
                            _mouseLostFocusEventHandler = handler;
                        }
                        else
                        {
                            _mouseLostFocusEventHandler += handler;
                        }
                    }
                    break;
                case UIEventName.KeyDown:
                    {
                        if (_keyDownEventHandler == null)
                        {
                            _keyDownEventHandler = handler;
                        }
                        else
                        {
                            _keyDownEventHandler += handler;
                        }
                    }
                    break;
            }
        }
        public override void RaiseEvent(UIEventName eventName, UIEventArgs e)
        {
            switch (eventName)
            {
                case UIEventName.MouseDown:
                    {
                        _mouseDownEventHandler?.Invoke((UIMouseEventArgs)e);
                    }
                    break;
                case UIEventName.MouseMove:
                    {
                        _mouseMoveEventHandler?.Invoke((UIMouseEventArgs)e);
                    }
                    break;
                case UIEventName.MouseUp:
                    {
                        _mouseUpEventHandler?.Invoke((UIMouseEventArgs)e);
                    }
                    break;
                case UIEventName.MouseLostFocus:
                    {
                        _mouseLostFocusEventHandler?.Invoke(e);
                    }
                    break;
                case UIEventName.KeyDown:
                    {
                        _keyDownEventHandler?.Invoke((UIKeyEventArgs)e);
                    }
                    break;
            }
        }
        public override void DetachEvent(UIEventName eventName, HtmlEventHandler handler)
        {
            switch (eventName)
            {
                case UIEventName.MouseDown:
                    {
                        if (_mouseDownEventHandler != null)
                        {
                            _mouseDownEventHandler -= handler;
                        }
                    }
                    break;
                case UIEventName.MouseMove:
                    {
                        if (_mouseMoveEventHandler != null)
                        {
                            _mouseMoveEventHandler -= handler;
                        }
                    }
                    break;
                case UIEventName.MouseUp:
                    {
                        if (_mouseUpEventHandler != null)
                        {
                            _mouseUpEventHandler -= handler;
                        }
                    }
                    break;
                case UIEventName.MouseLostFocus:
                    {
                        if (_mouseLostFocusEventHandler != null)
                        {
                            _mouseLostFocusEventHandler -= handler;
                        }
                    }
                    break;
                case UIEventName.KeyDown:
                    {
                        if (_keyDownEventHandler != null)
                        {
                            _keyDownEventHandler -= handler;
                        }
                    }
                    break;
            }
        }
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            _mouseDownEventHandler?.Invoke(e);
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(UIMouseEventArgs e)
        {
            _mouseMoveEventHandler?.Invoke(e);
            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            _mouseUpEventHandler?.Invoke(e);
            base.OnMouseUp(e);
        }
        protected override void OnLostMouseFocus(UIMouseLostFocusEventArgs e)
        {
            _mouseLostFocusEventHandler?.Invoke(e);
            base.OnLostMouseFocus(e);
        }
        protected override void OnKeyDown(UIKeyEventArgs e)
        {
            _keyDownEventHandler?.Invoke(e);
            base.OnKeyDown(e);
        }

    }


    sealed public class HtmlImageElement : HtmlElement
    {
        HtmlDocument _owner;
        internal HtmlImageElement(HtmlDocument owner, int prefix, int localNameIndex)
         : base(owner, prefix, localNameIndex)
        {
            _owner = owner;
            WellknownElementName = WellKnownDomNodeName.img;
        }
        public override void SetAttribute(DomAttribute attr)
        {
            //implementation specific...
            SetDomAttribute(attr);
            //handle some attribute
            switch ((WellknownName)attr.LocalNameIndex)
            {
                case WellknownName.Src:
                    {
                        if (_principalBox != null)
                        {
                            InternalSetImageBinder(_owner.GetImageBinder(attr.Value));
                        }
                    }
                    break;
                default:
                    ImplSetAttribute(attr);
                    break;
            }
        }
        void InternalSetImageBinder(ImageBinder imgBinder)
        {
            if (_principalBox == null) return;

            //
            CssBoxImage boxImg = (CssBoxImage)_principalBox;
            //implementation specific...                                           
            //if the binder is loaded , not need TempTranstionImageBinder
            boxImg.TempTranstionImageBinder = (imgBinder.State == BinderState.Loaded) ? null : boxImg.ImageBinder;
            boxImg.ImageBinder = imgBinder;
            boxImg.InvalidateGraphics();
        }
        public void SetImageSource(ImageBinder imgBinder)
        {
            DomAttribute attr = this.OwnerDocument.CreateAttribute("src", imgBinder.ImageSource);
            SetDomAttribute(attr);
            //----
            if (_principalBox != null)
            {
                InternalSetImageBinder(_owner.GetImageBinder(attr.Value));
            }
        }
        public void SetImageSource(string imgsrc)
        {
            //set image source
            DomAttribute attr = this.OwnerDocument.CreateAttribute("src", imgsrc);
            SetDomAttribute(attr);
            //----
            if (_principalBox != null)
            {
                InternalSetImageBinder(_owner.GetImageBinder(attr.Value));
            }
        }
    }

    public interface ISubDomExtender
    {
        void Write(System.Text.StringBuilder stbuilder);
    }
    public interface IHtmlInputSubDomExtender : ISubDomExtender
    {
        string GetInputValue();
        void SetInputValue(string value);
        void Focus();
    }
    public interface IHtmlTextAreaSubDomExtender : ISubDomExtender
    {
        string GetInputValue();
        void SetInputValue(string value);
        void Focus();
    }

    public sealed class HtmlTextAreaElement : HtmlElement, IHtmlTextArea
    {
        string _inputValue;
        IHtmlTextAreaSubDomExtender _subdomExt;
        internal HtmlTextAreaElement(HtmlDocument owner, int prefix, int localNameIndex)
         : base(owner, prefix, localNameIndex)
        {
            WellknownElementName = WellKnownDomNodeName.textarea;
        }
        public void Focus()
        {
            _subdomExt?.Focus();
        }
        public string Value
        {
            //TODO: add 'live' feature (connect with actual dom)
            get
            {
                if (_subdomExt != null)
                {
                    _inputValue = _subdomExt.GetInputValue();
                }
                return _inputValue;
            }
            set
            {
                if (_subdomExt != null)
                {
                    _subdomExt.SetInputValue(value);
                }
                _inputValue = value;
            }
        }
        public IHtmlTextAreaSubDomExtender SubDomExtender
        {
            get => _subdomExt;
            set
            {
                _subdomExt = value;
                if (value != null && _inputValue != null)
                {
                    _subdomExt.SetInputValue(_inputValue);
                }
            }
        }
    }


    public sealed class HtmlInputElement : HtmlElement, IHtmlInputElement
    {

        string _inputValue;
        string _inputType;
        string _name;
        IHtmlInputSubDomExtender _subdomExt;

        internal HtmlInputElement(HtmlDocument owner, int prefix, int localNameIndex)
        : base(owner, prefix, localNameIndex)
        {
            WellknownElementName = WellKnownDomNodeName.input;
        }
        public string inputType => _inputType;
        public string InputName
        {
            get => _name;
            set => _name = value;
        }
        public IHtmlInputSubDomExtender SubDomExtender
        {
            get => _subdomExt;
            set
            {
                _subdomExt = value;
                if (value != null && _inputValue != null)
                {
                    _subdomExt.SetInputValue(_inputValue);
                }
            }
        }
        public string Value
        {
            //TODO: add 'live' feature (connect with actual dom)
            get
            {
                if (_subdomExt != null)
                {
                    _inputValue = _subdomExt.GetInputValue();
                }
                return _inputValue;
            }
            set
            {
                if (_subdomExt != null)
                {
                    _subdomExt.SetInputValue(value);
                }
                _inputValue = value;
            }

        }
        public override void SetAttribute(DomAttribute attr)
        {
            //implementation specific...
            SetDomAttribute(attr);
            switch ((WellknownName)attr.LocalNameIndex)
            {
                case WellknownName.Name:
                    _name = attr.Value;
#if DEBUG
                    if (_name.StartsWith("."))
                    {

                    }
#endif
                    break;
                case WellknownName.Value:
                    _inputValue = attr.Value;
                    if (_subdomExt != null)
                    {
                        _subdomExt.SetInputValue(attr.Value);
                    }
                    break;
                case WellknownName.Type:
                    _inputType = attr.Value;
                    break;
                default:
                    ImplSetAttribute(attr);
                    break;
            }
        }

        public void Focus()
        {
            _subdomExt?.Focus();
        }

    }
    public sealed class HtmlOptionElement : HtmlElement, IHtmlOptionElement
    {
        string _optionValue;
        internal HtmlOptionElement(HtmlDocument owner, int prefix, int localNameIndex)
        : base(owner, prefix, localNameIndex)
        {
            WellknownElementName = WellKnownDomNodeName.option;
        }
        public override void SetAttribute(DomAttribute attr)
        {
            //implementation specific...
            SetDomAttribute(attr);
            switch ((WellknownName)attr.LocalNameIndex)
            {
                case WellknownName.Value:
                    _optionValue = attr.Value;
                    break;
                default:
                    ImplSetAttribute(attr);
                    break;
            }
        }
        public string Value
        {
            //TODO: add 'live' feature (connect with actual dom)
            get => _optionValue;
            set => _optionValue = value;
        }
    }


    public static class HtmlElementExtensions
    {
        /// <summary>
        /// create html div
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elem"></param>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static HtmlElement AddHtmlDivElement(this HtmlElement elem, Decorate<HtmlElement> dec = null)
        {
            HtmlElement div = elem.OwnerHtmlDoc.CreateHtmlDiv(dec);
            elem.AddChild(div);
            return div;
        }
        public static HtmlElement AddHtmlSpanElement(this HtmlElement elem, Decorate<HtmlElement> dec = null)
        {
            HtmlElement div = elem.OwnerHtmlDoc.CreateHtmlSpan(dec);
            elem.AddChild(div);
            return div;
        }
        public static HtmlImageElement AddHtmlImageElement(this HtmlElement elem, Decorate<HtmlImageElement> dec = null)
        {
            HtmlImageElement imgElem = elem.OwnerHtmlDoc.CreateHtmlImageElement(dec);
            elem.AddChild(imgElem);
            return imgElem;
        }
        public static void SetStyleAttribute(this HtmlElement elem, string cssStyleValue)
        {
            elem.SetAttribute("style", cssStyleValue);
        }
    }
}