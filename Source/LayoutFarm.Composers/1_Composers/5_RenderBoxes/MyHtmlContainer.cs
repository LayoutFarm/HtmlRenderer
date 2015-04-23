//BSD 2014-2015 ,WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;

using System.Text;
using System.Diagnostics;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.ContentManagers;
using LayoutFarm.UI;

namespace LayoutFarm.HtmlBoxes
{   

    public delegate void HtmlContainerUpdateHandler(HtmlContainer htmlCont);

    public sealed class MyHtmlContainer : HtmlContainer
    {
        WebDocument webdoc;
        HtmlHost htmlhost;
        SelectionRange _currentSelectionRange;
        int lastDomUpdateVersion;
        EventHandler domVisualRefresh;
        EventHandler domRequestRebuild;
        EventHandler containerInvalidateGfxHandler;
        EventHandler domFinished;

        public MyHtmlContainer(HtmlHost htmlhost)
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
        public bool IsInUpdateQueue
        {
            get;
            set;
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

                var htmldoc = this.webdoc as HtmlDocument;
                if (htmldoc != null)
                {
                    //clear
                    htmldoc.SetDomUpdateHandler(null);
                }
                //------------------------------------
                this.webdoc = value;
                //when attach  
                htmldoc = value as HtmlDocument;
                if (htmldoc != null)
                {
                    //attach monitor
                    htmldoc.SetDomUpdateHandler((s, e) =>
                    {
                        //when update
                        //add to update queue
                        this.htmlhost.NotifyHtmlContainerUpdate(this);
                    });
                }
            }
        }

        public bool RefreshDomIfNeed()
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
            }
        }
        public override void SetSelection(SelectionRange selRange)
        {
            this._currentSelectionRange = selRange;
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
            if (binder.State == ImageBinderState.Unload)
            {
                this.htmlhost.ChildRequestImage(binder, this, reqFrom, _sync);
            }
        }
        protected override void OnRequestScrollView(CssBox box)
        {
            RootGraphic rootgfx = (RootGraphic)box.RootGfx;
            rootgfx.AddToElementUpdateQueue(box);
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

        public void GetHtml(StringBuilder stbuilder)
        {
            throw new NotSupportedException();
        }

    }
}
