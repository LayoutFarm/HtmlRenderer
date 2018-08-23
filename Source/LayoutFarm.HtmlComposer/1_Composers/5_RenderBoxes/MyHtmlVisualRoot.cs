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

        WebDocument webdoc;
        HtmlHost htmlhost;
        SelectionRange _currentSelectionRange;
        int lastDomUpdateVersion;
        EventHandler domVisualRefresh;
        EventHandler domRequestRebuild;
        EventHandler containerInvalidateGfxHandler;
        EventHandler domFinished;
        Rectangle _currentSelectionArea;
        bool hasSomeSelectedArea;
        public MyHtmlVisualRoot(HtmlHost htmlhost)
        {
            this.htmlhost = htmlhost;
        }

        public void AttachEssentialHandlers(EventHandler domVisualRefreshHandler,
            EventHandler domRequestRebuildHandler,
            EventHandler containerInvalidateGfxHanlder,
            EventHandler domFinished)
        {
            this.domVisualRefresh = domVisualRefreshHandler;
            this.domRequestRebuild = domRequestRebuildHandler;
            this.containerInvalidateGfxHandler = containerInvalidateGfxHanlder;
            this.domFinished = domFinished;
        }
        public void DetachEssentialHandlers()
        {
            this.domVisualRefresh =
                this.domRequestRebuild =
                this.containerInvalidateGfxHandler =
                 this.domFinished = null;
        }
        protected override void OnLayoutFinished()
        {
            if (this.domFinished != null)
            {
                this.domFinished(this, EventArgs.Empty);
            }
        }

        public DomElement RootElement
        {
            get { return webdoc.RootNode; }
        }
        public WebDocument WebDocument
        {
            get { return this.webdoc; }
            set
            {
                var htmldoc = this.webdoc as LayoutFarm.Composers.HtmlDocument;
                if (htmldoc != null)
                {
                    //clear
                    htmldoc.SetDomUpdateHandler(null);
                }
                //------------------------------------
                this.webdoc = value;
                //when attach  
                htmldoc = value as LayoutFarm.Composers.HtmlDocument;
                if (htmldoc != null)
                {
                    //attach monitor
                    htmldoc.SetDomUpdateHandler((s, e) =>
                    {
                        //when update
                        //add to update queue
                        this.htmlhost.NotifyHtmlVisualRootUpdate(this);
                    });
                }
            }
        }

        public override bool RefreshDomIfNeed()
        {
            if (webdoc == null) return false;
            //----------------------------------

            int latestDomUpdateVersion = webdoc.DomUpdateVersion;
            if (this.lastDomUpdateVersion != latestDomUpdateVersion)
            {
                this.lastDomUpdateVersion = latestDomUpdateVersion;
                //reset 
                this.NeedLayout = false;
                if (domVisualRefresh != null)
                {
                    domVisualRefresh(this, EventArgs.Empty);
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
            if (this._currentSelectionRange != null)
            {
                _currentSelectionRange.ClearSelection();
                _currentSelectionRange = null;
                this.RootCssBox.InvalidateGraphics(this._currentSelectionArea);
                _currentSelectionArea = Rectangle.Empty;
            }
            hasSomeSelectedArea = false;
        }
        public override void SetSelection(SelectionRange selRange)
        {
            //find gross area of the selection range
            //then invalidate that area

            if (selRange != null)
            {
                _currentSelectionArea = (hasSomeSelectedArea) ?
                            Rectangle.Union(_currentSelectionArea, selRange.SnapSelectionArea) :
                            selRange.SnapSelectionArea;
                hasSomeSelectedArea = true;
            }
            else
            {
                hasSomeSelectedArea = false;
            }
            this._currentSelectionRange = selRange;

            this.RootCssBox.InvalidateGraphics(_currentSelectionArea);
        }
        public override void CopySelection(StringBuilder stbuilder)
        {
            if (this._currentSelectionRange != null)
            {
                this._currentSelectionRange.CopyText(stbuilder);
            }
        }
        public bool NeedLayout
        {
            get;
            private set;
        }
        public override void ContainerInvalidateGraphics()
        {
            containerInvalidateGfxHandler(this, EventArgs.Empty);
        }
        protected override void OnRequestImage(ImageBinder binder, object reqFrom, bool _sync)
        {
            //send request to host
            if (binder.State == BinderState.Unload)
            {
                this.htmlhost.ChildRequestImage(binder, this, reqFrom, _sync);
            }
        }
        protected override void OnRequestScrollView(CssBox box)
        {
            //RootGraphic rootgfx = (RootGraphic)box.RootGfx;
            //rootgfx.AddToElementUpdateQueue(box);
            this.htmlhost.EnqueueCssUpdate(box);

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
            if (webdoc != null &&
                webdoc.DocumentState == DocumentState.ChangedAfterIdle
                && domRequestRebuild != null)
            {
                domRequestRebuild(this, EventArgs.Empty);
            }
        }


        protected override void OnRootDisposed()
        {
            base.OnRootDisposed();
        }
        protected override void OnRootCreated(CssBox root)
        {
            //this._selectionHandler = new SelectionHandler(root, this);
            base.OnRootCreated(root);
        }
        protected override void OnAllDisposed()
        {
        }

    }
}
