//BSD 2014-2015 ,WinterDev
//ArthurHub

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
     
    public class HtmlHost
    {
        HtmlContainerUpdateHandler htmlContainerUpdateHandler;
        EventHandler<ImageRequestEventArgs> requestImage;
        EventHandler<TextRequestEventArgs> requestStyleSheet;

        SelectionRange _currentSelectionRange;
        GraphicsPlatform gfxplatform;
        Composers.HtmlDocument commonHtmlDoc;


        public HtmlHost(GraphicsPlatform gfxplatform, WebDom.CssActiveSheet activeSheet)
        {

            this.gfxplatform = gfxplatform;
            this.BaseStylesheet = activeSheet;

            this.commonHtmlDoc = new Composers.HtmlDocument();
            this.commonHtmlDoc.CssActiveSheet = activeSheet;
        }
        public HtmlHost(GraphicsPlatform gfxplatform)
            : this(gfxplatform, LayoutFarm.Composers.CssParserHelper.ParseStyleSheet(null, true))
        {
            //use default style sheet
        }
        public void AttachEssentailHandlers(
            EventHandler<ImageRequestEventArgs> reqImageHandler,
            EventHandler<TextRequestEventArgs> reqStyleSheetHandler)
        {
            this.requestImage = reqImageHandler;
            this.requestStyleSheet = reqStyleSheetHandler;
        }
        public void DetachEssentailHanlders()
        {
            this.requestImage = null;
            this.requestStyleSheet = null;
        }
        public void SetHtmlContainerUpdateHandler(HtmlContainerUpdateHandler htmlContainerUpdateHandler)
        {
            this.htmlContainerUpdateHandler = htmlContainerUpdateHandler;
        }


        public GraphicsPlatform GfxPlatform { get { return this.gfxplatform; } }
        public WebDom.CssActiveSheet BaseStylesheet { get; private set; }

        public void ChildRequestImage(ImageBinder binder, HtmlContainer htmlCont, object reqFrom, bool _sync)
        {
            if (this.requestImage != null)
            {
                ImageRequestEventArgs resReq = new ImageRequestEventArgs(binder); 
                resReq.requestBy = reqFrom;
                requestImage(this, resReq);
            }
        } 
        internal SelectionRange SelectionRange
        {
            get { return this._currentSelectionRange; }
            set { this._currentSelectionRange = value; }
        }
        internal void ClearPreviousSelection()
        {
            if (_currentSelectionRange != null)
            {
                _currentSelectionRange.ClearSelectionStatus();
                _currentSelectionRange = null;
            }
        }

        public Composers.FragmentHtmlDocument CreateNewFragmentHtml()
        {

            return new Composers.FragmentHtmlDocument(this.commonHtmlDoc);
        }

        //------------------------
        Queue<HtmlInputEventAdapter> inputEventAdapterStock = new Queue<HtmlInputEventAdapter>();
        Queue<LayoutFarm.HtmlBoxes.LayoutVisitor> htmlLayoutVisitorStock = new Queue<LayoutVisitor>();
        LayoutFarm.Composers.RenderTreeBuilder renderTreeBuilder;

        public LayoutFarm.HtmlBoxes.LayoutVisitor GetSharedHtmlLayoutVisitor(HtmlContainer htmlCont)
        {
            LayoutFarm.HtmlBoxes.LayoutVisitor lay = null;
            if (htmlLayoutVisitorStock.Count == 0)
            {
                lay = new LayoutVisitor(this.gfxplatform);
            }
            else
            {
                lay = this.htmlLayoutVisitorStock.Dequeue();
            }
            lay.Bind(htmlCont);
            return lay;
        }
        public void ReleaseHtmlLayoutVisitor(LayoutFarm.HtmlBoxes.LayoutVisitor lay)
        {
            lay.UnBind();
            this.htmlLayoutVisitorStock.Enqueue(lay);
        }
        public HtmlInputEventAdapter GetSharedInputEventAdapter(HtmlContainer htmlCont)
        {
            HtmlInputEventAdapter adapter = null;
            if (inputEventAdapterStock.Count == 0)
            {
                adapter = new HtmlInputEventAdapter(this.gfxplatform.SampleIFonts);
            }
            else
            {
                adapter = this.inputEventAdapterStock.Dequeue();
            }
            adapter.Bind(htmlCont);
            return adapter;
        }
        public void ReleaseSharedInputEventAdapter(HtmlInputEventAdapter adapter)
        {
            adapter.Unbind();
            this.inputEventAdapterStock.Enqueue(adapter);
        }
        public LayoutFarm.Composers.RenderTreeBuilder GetRenderTreeBuilder()
        {
            if (this.renderTreeBuilder == null)
            {
                renderTreeBuilder = new Composers.RenderTreeBuilder(this.gfxplatform);
                this.renderTreeBuilder.RequestStyleSheet += (e) =>
                {
                    if (requestStyleSheet != null)
                    {   
                        requestStyleSheet(this, e); 
                    }
                };
            }
            return renderTreeBuilder;
        }
        internal void NotifyHtmlContainerUpdate(HtmlContainer htmlCont)
        {
            if (htmlContainerUpdateHandler != null)
            {
                htmlContainerUpdateHandler(htmlCont);
            }
        }
    }

    public delegate void HtmlContainerUpdateHandler(HtmlContainer htmlCont);

    public sealed class MyHtmlContainer : HtmlContainer
    {
        WebDocument webdoc;
        HtmlHost htmlhost;

        int lastDomUpdateVersion;
        EventHandler domVisualRefresh;
        EventHandler domRequestRebuild;
        EventHandler containerInvalidateHanlder;

        public MyHtmlContainer(HtmlHost htmlhost)
        {
            this.htmlhost = htmlhost;
        }

        public void AttachEssentialHandlers(EventHandler domVisualRefreshHandler,
            EventHandler domRequestRebuildHandler,
            EventHandler containerInvalidateHandler)
        {
            this.domVisualRefresh = domVisualRefreshHandler;
            this.domRequestRebuild = domRequestRebuildHandler;
            this.containerInvalidateHanlder = containerInvalidateHandler;
        }
        public void DetachEssentialHandlers()
        {
            this.domVisualRefresh =
                this.domRequestRebuild =
                this.containerInvalidateHanlder = null;
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

                var htmldoc = this.webdoc as Composers.HtmlDocument;
                if (htmldoc != null)
                {
                    //clear
                    htmldoc.SetDomUpdateHandler(null);
                }
                //------------------------------------
                this.webdoc = value;
                //when attach  
                htmldoc = value as Composers.HtmlDocument;
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
            this.htmlhost.ClearPreviousSelection();
        }
        public override void SetSelection(SelectionRange selRange)
        {
            this.htmlhost.SelectionRange = selRange;
        }
        public bool NeedLayout
        {
            get;
            private set;
        }
        public override void ContainerInvalidateGraphics()
        {
            containerInvalidateHanlder(this, EventArgs.Empty);
        }
        protected override void OnRequestImage(ImageBinder binder, object reqFrom, bool _sync)
        {
            //send request to host
            if (binder.State == ImageBinderState.Unload)
            {
                this.htmlhost.ChildRequestImage(binder, this, reqFrom, _sync);
            }

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
