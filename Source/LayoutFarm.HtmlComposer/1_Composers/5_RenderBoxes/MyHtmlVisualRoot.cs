//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

using System;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
namespace LayoutFarm.HtmlBoxes
{
    public delegate void HtmlVisualRootUpdateHandler(HtmlVisualRoot htmlVisualRoot);
    public sealed class MyHtmlVisualRoot : HtmlVisualRoot
    {

        WebDocument _webdoc;
        HtmlHost _htmlhost;
        SelectionRange _currentSelectionRange;
        int _lastDomUpdateVersion;
        EventHandler _domVisualRefresh;
        EventHandler _domRequestRebuild;
        EventHandler _containerInvalidateGfxHandler;
        EventHandler _domFinished;
        Rectangle _currentSelectionArea;
        bool _hasSomeSelectedArea;
        //
        public MyHtmlVisualRoot(HtmlHost htmlhost)
        {
            _htmlhost = htmlhost;
            _textService = htmlhost.GetTextService();

        }
        public void AttachEssentialHandlers(EventHandler domVisualRefreshHandler,
            EventHandler domRequestRebuildHandler,
            EventHandler containerInvalidateGfxHanlder,
            EventHandler domFinished)
        {
            _domVisualRefresh = domVisualRefreshHandler;
            _domRequestRebuild = domRequestRebuildHandler;
            _containerInvalidateGfxHandler = containerInvalidateGfxHanlder;
            _domFinished = domFinished;
        }
        public void DetachEssentialHandlers()
        {
            _domVisualRefresh =
                _domRequestRebuild =
                _containerInvalidateGfxHandler =
                 _domFinished = null;
        }
        protected override void OnLayoutFinished()
        {
            _domFinished?.Invoke(this, EventArgs.Empty);
        }
        //
        public DomElement RootElement => _webdoc.RootNode;
        //
        public WebDocument WebDocument
        {
            get => _webdoc;
            set
            {
                var htmldoc = _webdoc as LayoutFarm.Composers.HtmlDocument;
                if (htmldoc != null)
                {
                    //clear
                    htmldoc.SetDomUpdateHandler(null);
                }
                //------------------------------------
                _webdoc = value;
                //when attach  
                htmldoc = value as LayoutFarm.Composers.HtmlDocument;
                if (htmldoc != null)
                {
                    //attach monitor
                    htmldoc.SetDomUpdateHandler((s, e) =>
                    {
                        //when update
                        //add to update queue
                        _htmlhost.NotifyHtmlVisualRootUpdate(this);
                    });
                }
            }
        }

        public override bool RefreshDomIfNeed()
        {
            if (_webdoc == null) return false;
            //----------------------------------

            int latestDomUpdateVersion = _webdoc.DomUpdateVersion;
            if (_lastDomUpdateVersion != latestDomUpdateVersion)
            {
                _lastDomUpdateVersion = latestDomUpdateVersion;
                //reset 
                this.NeedLayout = false;
                if (_domVisualRefresh != null)
                {
                    _domVisualRefresh(this, EventArgs.Empty);
                    this.ContainerInvalidateGraphics();
                }
#if DEBUG
                //dbugCount02++;
                //Console.WriteLine(dd);
#endif
                return true;
            }
            return false;
        }
        public override void ClearPreviousSelection()
        {
            if (_currentSelectionRange != null)
            {
                _currentSelectionRange.ClearSelection();
                _currentSelectionRange = null;
                this.RootCssBox.InvalidateGraphics(_currentSelectionArea);
                _currentSelectionArea = Rectangle.Empty;
            }
            _hasSomeSelectedArea = false;
        }
        public override void SetSelection(SelectionRange selRange)
        {
            //find gross area of the selection range
            //then invalidate that area

            if (selRange != null)
            {
                _currentSelectionArea = (_hasSomeSelectedArea) ?
                            Rectangle.Union(_currentSelectionArea, selRange.SnapSelectionArea) :
                            selRange.SnapSelectionArea;
                _hasSomeSelectedArea = true;
            }
            else
            {
                _hasSomeSelectedArea = false;
            }
            _currentSelectionRange = selRange;

#if DEBUG
            //System.Diagnostics.Debug.WriteLine("html-sel:" + _currentSelectionArea.ToString());
#endif

            this.RootCssBox.InvalidateGraphics(_currentSelectionArea);
        }
        public override SelectionRange CurrentSelectionRange => _currentSelectionRange;

        public override void CopySelection(StringBuilder stbuilder)
        {
            if (_currentSelectionRange != null)
            {
                _currentSelectionRange.CopyText(stbuilder);
            }
        }
        public bool NeedLayout
        {
            get;
            private set;
        }
        public override void ContainerInvalidateGraphics()
        {
            _containerInvalidateGfxHandler(this, EventArgs.Empty);
        }
        protected override void OnRequestImage(ImageBinder binder, object reqFrom, bool _sync)
        {
            //send request to host
            if (binder.State == BinderState.Unload)
            {
                _htmlhost.ChildRequestImage(binder, this, reqFrom, _sync);
            }
        }
        protected override void OnRequestScrollView(CssBox box)
        {
            //RootGraphic rootgfx = (RootGraphic)box.RootGfx;
            //rootgfx.AddToElementUpdateQueue(box);
            _htmlhost.EnqueueCssUpdate(box);

            //    var renderE = this.elementUpdateQueue[i];
            //    var cssbox = renderE as HtmlBoxes.CssBox;
            //    if (cssbox != null)
            //    {
            //        var controller = HtmlBoxes.CssBox.UnsafeGetController(cssbox) as IEventListener;
            //        controller.HandleElementUpdate();
            //    }
            //    this.elementUpdateQueue.RemoveAt(i);
            //}
        }
        /// <summary>
        /// check if dom update
        /// </summary>
        public void CheckDocUpdate()
        {
            if (_webdoc != null &&
                _webdoc.DocumentState == DocumentState.ChangedAfterIdle
                && _domRequestRebuild != null)
            {
                _domRequestRebuild(this, EventArgs.Empty);
            }
        }


        protected override void OnRootDisposed()
        {
            base.OnRootDisposed();
        }
        protected override void OnRootCreated(CssBox root)
        {
            //_selectionHandler = new SelectionHandler(root, this);
            base.OnRootCreated(root);
        }
        protected override void OnAllDisposed()
        {
        }

    }
}
