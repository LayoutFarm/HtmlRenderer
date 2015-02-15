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
        List<LayoutFarm.Composers.CustomCssBoxGenerator> generators = new List<LayoutFarm.Composers.CustomCssBoxGenerator>();

        HtmlContainerUpdateHandler htmlContainerUpdateHandler;

        EventHandler<ImageRequestEventArgs> requestImage;
        EventHandler<TextRequestEventArgs> requestStyleSheet;


        GraphicsPlatform gfxplatform;
        HtmlDocument commonHtmlDoc;


        public HtmlHost(GraphicsPlatform gfxplatform, WebDom.CssActiveSheet activeSheet)
        {

            this.gfxplatform = gfxplatform;
            this.BaseStylesheet = activeSheet;

            this.commonHtmlDoc = new HtmlDocument();
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

        public FragmentHtmlDocument CreateNewFragmentHtml()
        {
            return new FragmentHtmlDocument(this.commonHtmlDoc);
        }

        //------------------------         
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

        public HtmlInputEventAdapter GetNewInputEventAdapter()
        {
            return new HtmlInputEventAdapter(this.gfxplatform.SampleIFonts);
        }
        public LayoutFarm.Composers.RenderTreeBuilder GetRenderTreeBuilder()
        {
            if (this.renderTreeBuilder == null)
            {
                renderTreeBuilder = new Composers.RenderTreeBuilder(this);
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

        //--------------------------------------------------- 

        public bool HasRegisterCssBoxGenerator(Type t)
        {
            for (int i = generators.Count - 1; i >= 0; --i)
            {
                if (generators[i].GetType() == t)
                {
                    return true;
                }
            }
            return false;
        }
        public void RegisterCssBoxGenerator(LayoutFarm.Composers.CustomCssBoxGenerator cssBoxGenerator)
        {
            this.generators.Add(cssBoxGenerator);
        }

        public CssBox CreateCustomBox(CssBox parent, LayoutFarm.WebDom.DomElement tag, LayoutFarm.Css.BoxSpec boxspec, RootGraphic rootgfx)
        {

            for (int i = generators.Count - 1; i >= 0; --i)
            {
                var newbox = generators[i].CreateCssBox(tag, parent, boxspec, rootgfx);
                if (newbox != null)
                {
                    return newbox;
                }
            }
            return null;
        }
        //---------------------------------------------------
    }

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
                _currentSelectionRange.ClearSelectionStatus();
                _currentSelectionRange = null;
            }
        }
        public override void SetSelection(SelectionRange selRange)
        {
            if (selRange == null)
            {
            }
            this._currentSelectionRange = selRange;
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
