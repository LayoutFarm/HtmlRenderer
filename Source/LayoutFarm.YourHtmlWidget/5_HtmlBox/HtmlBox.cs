//Apache2, 2014-present, WinterDev

using System;
using System.Text;
using LayoutFarm.Composers;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class HtmlBox : AbstractRectUI, IEventPortal
    {
        WaitingContentKind _waitingContentKind;
        string _waitingHtmlString;
        HtmlDocument _waitingHtmlDoc;
        enum WaitingContentKind : byte
        {
            NoWaitingContent,
            HtmlFragmentString,
            HtmlString,
            HtmlDocument
        }


        //
        int _latest_selMouseDownX;
        int _latest_selMouseDownY;

        HtmlHost _htmlhost;
        MyHtmlVisualRoot _htmlVisualRoot;
        //presentation
        HtmlRenderBox _htmlRenderBox;
        HtmlInputEventAdapter _inputEventAdapter; 
        public HtmlBox(HtmlHost htmlHost, int width, int height)
            : base(width, height)
        {
            this._htmlhost = htmlHost;
        }
        internal HtmlHost HtmlHost
        {
            get { return this._htmlhost; }
        }
        public string BaseUrl
        {
            get
            {
                return _htmlhost.BaseUrl;
            }
            set
            {
                _htmlhost.BaseUrl = value;
            }
        }
        protected override void OnContentLayout()
        {
            this.PerformContentLayout();
        }
        public override void PerformContentLayout()
        {
            this.RaiseLayoutFinished();
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this._htmlRenderBox; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this._htmlRenderBox != null; }
        }

        HtmlInputEventAdapter GetInputEventAdapter()
        {
            if (_inputEventAdapter == null)
            {
                _inputEventAdapter = new HtmlInputEventAdapter();
                _inputEventAdapter.Bind(_htmlVisualRoot);
            }
            return _inputEventAdapter;
        }

        void IEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {
            e.CurrentContextElement = this;
            GetInputEventAdapter().MouseUp(e, _htmlRenderBox.CssBox);
        }
        void IEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {

            this.Focus();
            e.CurrentContextElement = this;
            GetInputEventAdapter().MouseDown(e, _htmlRenderBox.CssBox);

            if (e.Shift && !e.CancelBubbling)
            {
                //simulate html selection
                if (_htmlVisualRoot.CurrentSelectionRange != null)
                {
                    //extend from existing selection
                    SelectionRange selRange = _htmlVisualRoot.CurrentSelectionRange;

                    PixelFarm.Drawing.Rectangle existingArea = _htmlVisualRoot.CurrentSelectionRange.SnapSelectionArea;
                    //
                    SimulateMouseSelection(
                        _latest_selMouseDownX,
                        _latest_selMouseDownY,
                        e.X,
                        e.Y);
                    //not set _latest_selMouseDownX,Y
                }
                else
                {
                    SimulateMouseSelection(
                        _latest_selMouseDownX,
                        _latest_selMouseDownY,
                        e.X,
                        e.Y);

                    _latest_selMouseDownX = e.X; //set new
                    _latest_selMouseDownY = e.Y; //set new
                }
            }
            else
            {
                _latest_selMouseDownX = e.X; //set new
                _latest_selMouseDownY = e.Y; //set new
            }
        }
        void IEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {

            e.CurrentContextElement = this;
            GetInputEventAdapter().MouseMove(e, _htmlRenderBox.CssBox);
        }
        void IEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {
            e.CurrentContextElement = this;
        }
        void IEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            e.CurrentContextElement = this;
            GetInputEventAdapter().KeyDown(e, _htmlRenderBox.CssBox);
        }
        void IEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            e.CurrentContextElement = this;
            GetInputEventAdapter().KeyPress(e, _htmlRenderBox.CssBox);
        }
        void IEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {

            e.CurrentContextElement = this;
            GetInputEventAdapter().KeyUp(e, _htmlRenderBox.CssBox);
        }
        bool IEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            e.CurrentContextElement = this;
            return GetInputEventAdapter().ProcessDialogKey(e, _htmlRenderBox.CssBox);
        }
        void IEventPortal.PortalGotFocus(UIFocusEventArgs e)
        {
            e.CurrentContextElement = this;
        }
        void IEventPortal.PortalLostFocus(UIFocusEventArgs e)
        {
            e.CurrentContextElement = this;
        }

        void SimulateMouseSelection(int startX, int startY, int endX, int endY)
        {
            HtmlInputEventAdapter evAdapter = GetInputEventAdapter();
            {
                UIMouseEventArgs mouseDown = new UIMouseEventArgs();
                mouseDown.SetLocation(startX, startY);
                evAdapter.MouseDown(mouseDown);
            }

            {
                UIMouseEventArgs mouseDrag = new UIMouseEventArgs();
                mouseDrag.IsDragging = true;
                mouseDrag.SetLocation(endX, endY);
                evAdapter.MouseMove(mouseDrag);
            }
            {
                UIMouseEventArgs mouseUp = new UIMouseEventArgs();
                mouseUp.SetLocation(endX, endY);
                evAdapter.MouseUp(mouseUp);
            }

        }
        protected override void OnKeyUp(UIKeyEventArgs e)
        {
            if (e.Ctrl)
            {
                switch (e.KeyCode)
                {
                    case UIKeys.C:
                        {
                            //ctrl+ c => copy to clipboard
                            StringBuilder stbuilder = new StringBuilder();
                            //copy only text ***
                            //TODO: copy 'portable' html text***
                            this._htmlVisualRoot.CopySelection(stbuilder);
                            LayoutFarm.UI.Clipboard.SetText(stbuilder.ToString());
                        }
                        break;
                    case UIKeys.A:
                        {

                            //ctrl+a=> select all
                            //simulate mouse selection

                            SimulateMouseSelection(0, 0,
                                (int)_htmlVisualRoot.RootCssBox.VisualWidth - 2,
                                (int)_htmlVisualRoot.RootCssBox.VisualHeight - 2);

                        }
                        break;
                }
            }

            e.CurrentContextElement = this;
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_htmlRenderBox == null)
            {
                var newFrRenderBox = new HtmlRenderBox(rootgfx, this.Width, this.Height);
                newFrRenderBox.SetController(this);
                newFrRenderBox.HasSpecificWidthAndHeight = true;
                newFrRenderBox.SetLocation(this.Left, this.Top);
                //set to this field if ready
                this._htmlRenderBox = newFrRenderBox;
            }
            switch (this._waitingContentKind)
            {
                default:
                case WaitingContentKind.NoWaitingContent:
                    break;
                case WaitingContentKind.HtmlDocument:
                    {
                        LoadHtmlDom(this._waitingHtmlDoc);
                    }
                    break;
                case WaitingContentKind.HtmlFragmentString:
                    {
                        LoadHtmlFragmentString(this._waitingHtmlString);
                    }
                    break;
                case WaitingContentKind.HtmlString:
                    {
                        LoadHtmlString(this._waitingHtmlString);
                    }
                    break;
            }

            return _htmlRenderBox;
        }
        //-----------------------------------------------------------------------------------------------------
        void ClearWaitingContent()
        {
            this._waitingHtmlDoc = null;
            this._waitingHtmlString = null;
            _waitingContentKind = WaitingContentKind.NoWaitingContent;
        }
        public void LoadHtmlDom(HtmlDocument htmldoc)
        {
            if (_htmlRenderBox == null)
            {
                this._waitingContentKind = WaitingContentKind.HtmlDocument;
                this._waitingHtmlDoc = htmldoc;
            }
            else
            {
                //just parse content and load 
                this._htmlVisualRoot = HtmlHostExtensions.CreateHtmlVisualRoot(this._htmlhost, htmldoc, _htmlRenderBox);
                SetHtmlContainerEventHandlers();
                ClearWaitingContent();
                RaiseLayoutFinished();
            }
        }
        public void LoadHtmlString(string htmlString)
        {
            if (_htmlRenderBox == null)
            {
                this._waitingContentKind = WaitingContentKind.HtmlString;
                this._waitingHtmlString = htmlString;
            }
            else
            {
                //just parse content and load 
                this._htmlVisualRoot = HtmlHostExtensions.CreateHtmlVisualRootFromFullHtml(this._htmlhost, htmlString, _htmlRenderBox);
                SetHtmlContainerEventHandlers();
                ClearWaitingContent();
            }
        }
        public void LoadHtmlFragmentString(string fragmentHtmlString)
        {
            if (_htmlRenderBox == null)
            {
                this._waitingContentKind = WaitingContentKind.HtmlFragmentString;
                this._waitingHtmlString = fragmentHtmlString;
            }
            else
            {
                //just parse content and load 
                this._htmlVisualRoot = HtmlHostExtensions.CreateHtmlVisualRootFromFragmentHtml(this._htmlhost, fragmentHtmlString, _htmlRenderBox);
                SetHtmlContainerEventHandlers();
                ClearWaitingContent();
            }
        }


        void SetHtmlContainerEventHandlers()
        {
            _htmlVisualRoot.AttachEssentialHandlers(
                //1.
                (s, e) => this.InvalidateGraphics(),
                //2.
                (s, e) =>
                {
                    //---------------------------
                    if (_htmlRenderBox == null) return;
                    //--------------------------- 
                    _htmlhost.GetRenderTreeBuilder().RefreshCssTree(_htmlVisualRoot.RootElement);
                    LayoutVisitor lay = this._htmlhost.GetSharedHtmlLayoutVisitor(_htmlVisualRoot);
                    _htmlVisualRoot.PerformLayout(lay);
                    this._htmlhost.ReleaseHtmlLayoutVisitor(lay);
                },
                //3.
                (s, e) => this.InvalidateGraphics(),
                //4
                (s, e) => { this.RaiseLayoutFinished(); });
        }
        public WebDom.IHtmlDocument HtmlDoc
        {
            get
            {
                return this._htmlVisualRoot.WebDocument as WebDom.IHtmlDocument;
            }
        }

        public override void SetViewport(int x, int y, object reqBy)
        {
            base.SetViewport(x, y, reqBy);
            if (_htmlRenderBox != null)
            {
                _htmlRenderBox.SetViewport(x, y);
            }
        }
        public override int InnerWidth
        {
            get
            {
                return this._htmlRenderBox.HtmlWidth;
            }
        }
        public override int InnerHeight
        {
            get
            {
                return this._htmlRenderBox.HtmlHeight;
            }
        }

        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "htmlbox");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}





