//Apache2, 2014-present, WinterDev

using System;
using System.Text;
using LayoutFarm.Composers;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.UI;
using LayoutFarm.UI.ForImplementator;

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
            _htmlhost = htmlHost;
        }

#if DEBUG
        bool debug_PreferSoftwareRenderer;
        public bool dbugPreferSoftwareRenderer
        {
            get => debug_PreferSoftwareRenderer;
            set
            {
                debug_PreferSoftwareRenderer = value;
                if (_htmlRenderBox != null)
                {

                    _htmlRenderBox.dbugPreferSoftwareRenderer = value;
                }
            }
        }
#endif
        //
        internal HtmlHost HtmlHost => _htmlhost;
        //
        public string BaseUrl
        {
            get => _htmlhost.BaseUrl;
            set => _htmlhost.BaseUrl = value;

        }
        protected override void OnContentLayout()
        {
            this.PerformContentLayout();
        }
        public override void PerformContentLayout()
        {
            this.RaiseLayoutFinished();
        }
        //
        public override RenderElement CurrentPrimaryRenderElement => _htmlRenderBox;
        //
        HtmlInputEventAdapter GetInputEventAdapter()
        {
            if (_inputEventAdapter == null)
            {
                _inputEventAdapter = new HtmlInputEventAdapter();
                _inputEventAdapter.Bind(_htmlVisualRoot);
            }
            return _inputEventAdapter;
        }

        void IEventPortal.PortalMouseUp(UIMouseUpEventArgs e)
        {

            e.SetCurrentContextElement(this);//?
            GetInputEventAdapter().MouseUp(e, _htmlRenderBox.CssBox);
        }
        void IEventPortal.PortalMouseDown(UIMouseDownEventArgs e)
        {
            this.Focus();
            e.SetCurrentContextElement(this);//?
            GetInputEventAdapter().MouseDown(e, _htmlRenderBox.CssBox);

            if (e.Shift && !e.CancelBubbling)
            {
                //simulate html selection
                if (_htmlVisualRoot.CurrentSelectionRange != null)
                {
                    //extend from existing selection
                    SelectionRange selRange = _htmlVisualRoot.CurrentSelectionRange;

#if DEBUG
                    PixelFarm.Drawing.Rectangle existingArea = _htmlVisualRoot.CurrentSelectionRange.SnapSelectionArea;
#endif
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
        void IEventPortal.PortalMouseMove(UIMouseMoveEventArgs e)
        {
            //TODO: review this again
            //this set current context element should be automatically set
            e.SetCurrentContextElement(this);

            GetInputEventAdapter().MouseMove(e, _htmlRenderBox.CssBox);
        }
        void IEventPortal.PortalMouseWheel(UIMouseWheelEventArgs e)
        {
            e.SetCurrentContextElement(this);
        }
        void IEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            e.SetCurrentContextElement(this);
            GetInputEventAdapter().KeyDown(e, _htmlRenderBox.CssBox);
        }
        void IEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            e.SetCurrentContextElement(this);
            GetInputEventAdapter().KeyPress(e, _htmlRenderBox.CssBox);
        }
        void IEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            e.SetCurrentContextElement(this);
            GetInputEventAdapter().KeyUp(e, _htmlRenderBox.CssBox);
        }
        bool IEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            e.SetCurrentContextElement(this);
            return GetInputEventAdapter().ProcessDialogKey(e, _htmlRenderBox.CssBox);
        }
        void IEventPortal.PortalGotFocus(UIFocusEventArgs e)
        {
            e.SetCurrentContextElement(this);
        }
        void IEventPortal.PortalLostFocus(UIFocusEventArgs e)
        {
            e.SetCurrentContextElement(this);
        }


        readonly UIMouseDownEventArgs _sel_MouseDownArgs = new UIMouseDownEventArgs();
        readonly UIMouseMoveEventArgs _sel_MouseMoveArgs = new UIMouseMoveEventArgs();
        readonly UIMouseUpEventArgs _selMouseUpArgs = new UIMouseUpEventArgs();

        void SimulateMouseSelection(int startX, int startY, int endX, int endY)
        {
            //TODO: review here again***
            HtmlInputEventAdapter evAdapter = GetInputEventAdapter();
            {

                _sel_MouseDownArgs.SetLocation(startX, startY);
                evAdapter.MouseDown(_sel_MouseDownArgs);
            }

            {
                _sel_MouseMoveArgs.IsDragging = true;
                _sel_MouseMoveArgs.SetLocation(endX, endY);
                evAdapter.MouseMove(_sel_MouseMoveArgs);
            }
            {

                _selMouseUpArgs.SetLocation(endX, endY);
                evAdapter.MouseUp(_selMouseUpArgs);
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
                            _htmlVisualRoot.CopySelection(stbuilder);
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

            //??? review here
            e.SetCurrentContextElement(this);
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_htmlRenderBox == null)
            {
                var newHtmlRenderBox = new HtmlRenderBox(rootgfx, this.Width, this.Height);
                newHtmlRenderBox.SetController(this);
                newHtmlRenderBox.HasSpecificWidthAndHeight = true;
                newHtmlRenderBox.SetLocation(this.Left, this.Top);
#if DEBUG
                newHtmlRenderBox.dbugPreferSoftwareRenderer = this.dbugPreferSoftwareRenderer;
#endif
                //set to this field if ready
                _htmlRenderBox = newHtmlRenderBox;
            }
            switch (_waitingContentKind)
            {
                default:
                case WaitingContentKind.NoWaitingContent:
                    break;
                case WaitingContentKind.HtmlDocument:
                    {
                        LoadHtmlDom(_waitingHtmlDoc);
                    }
                    break;
                case WaitingContentKind.HtmlFragmentString:
                    {
                        LoadHtmlFragmentString(_waitingHtmlString);
                    }
                    break;
                case WaitingContentKind.HtmlString:
                    {
                        LoadHtmlString(_waitingHtmlString);
                    }
                    break;
            }

            return _htmlRenderBox;
        }
        //-----------------------------------------------------------------------------------------------------
        void ClearWaitingContent()
        {
            _waitingHtmlDoc = null;
            _waitingHtmlString = null;
            _waitingContentKind = WaitingContentKind.NoWaitingContent;
        }
        public void LoadHtmlDom(HtmlDocument htmldoc)
        {
            if (_htmlRenderBox == null)
            {
                _waitingContentKind = WaitingContentKind.HtmlDocument;
                _waitingHtmlDoc = htmldoc;
            }
            else
            {
                //just parse content and load 
                _htmlVisualRoot = HtmlHostExtensions.CreateHtmlVisualRoot(_htmlhost, htmldoc, _htmlRenderBox);
                SetHtmlContainerEventHandlers();
                ClearWaitingContent();
                RaiseLayoutFinished();
            }
        }

        string _orgHtmlString;

        public string GetOrgHtmlString()
        {
            //temp!
            return _orgHtmlString;
        }
        public void LoadHtmlString(string htmlString)
        {

            if (_htmlRenderBox == null)
            {
                _waitingContentKind = WaitingContentKind.HtmlString;
                _orgHtmlString = _waitingHtmlString = htmlString;
            }
            else
            {
                //just parse content and load 
                _htmlVisualRoot = HtmlHostExtensions.CreateHtmlVisualRootFromFullHtml(_htmlhost, _orgHtmlString = htmlString, _htmlRenderBox);
                SetHtmlContainerEventHandlers();
                ClearWaitingContent();
                RaiseLayoutFinished();
                _htmlRenderBox.InvalidateGraphics();
            }
        }
        public void LoadHtml(ExternalHtmlTreeWalker externalHtmlTreeWalker)
        {

            if (_htmlRenderBox == null)
            {
                _waitingContentKind = WaitingContentKind.HtmlString;
                _waitingHtmlString = "";
            }
            else
            {
                //just parse content and load 
                _htmlVisualRoot = HtmlHostExtensions.CreateHtmlVisualRootFromFullHtml(_htmlhost, externalHtmlTreeWalker, _htmlRenderBox);
                SetHtmlContainerEventHandlers();
                ClearWaitingContent();
                RaiseLayoutFinished();
                _htmlRenderBox.InvalidateGraphics();
            }

        }
        public void LoadHtmlFragmentString(string fragmentHtmlString)
        {
            if (_htmlRenderBox == null)
            {
                _waitingContentKind = WaitingContentKind.HtmlFragmentString;
                _orgHtmlString = _waitingHtmlString = fragmentHtmlString;
            }
            else
            {
                //just parse content and load 
                _htmlVisualRoot = HtmlHostExtensions.CreateHtmlVisualRootFromFragmentHtml(_htmlhost, _orgHtmlString = fragmentHtmlString, _htmlRenderBox);
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
                    LayoutVisitor lay = _htmlhost.GetSharedHtmlLayoutVisitor(_htmlVisualRoot);
                    _htmlVisualRoot.PerformLayout(lay);
                    _htmlhost.ReleaseHtmlLayoutVisitor(lay);
                },
                //3.
                (s, e) => this.InvalidateGraphics(),
                //4
                (s, e) => { this.RaiseLayoutFinished(); });
        }
        //
        public WebDom.IHtmlDocument HtmlDoc => (_htmlVisualRoot != null) ? (_htmlVisualRoot.WebDocument as WebDom.IHtmlDocument) : null;
        public bool HasHtmlVisualRoot => _htmlVisualRoot != null;

        public override void SetViewport(int x, int y, object reqBy)
        {
            base.SetViewport(x, y, reqBy);
            if (_htmlRenderBox != null)
            {
                _htmlRenderBox.SetViewport(x, y);
            }
        }
        //

        public override int InnerWidth => _htmlRenderBox.HtmlWidth;
        public override int InnerHeight => _htmlRenderBox.HtmlHeight;


    }
}





