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
    public class HtmlResourceRequestEventArgs : EventArgs
    {
        public ImageBinder binder;
        public object requestBy;

    }

    public class HtmlIslandHost
    {
        public event EventHandler<HtmlResourceRequestEventArgs> RequestResource;
        SelectionRange _currentSelectionRange;
        GraphicsPlatform gfxplatform;

        Composers.HtmlDocument commonHtmlDoc;


        public HtmlIslandHost(GraphicsPlatform gfxplatform, WebDom.CssActiveSheet activeSheet)
        {
            this.gfxplatform = gfxplatform;
            this.commonHtmlDoc = new Composers.HtmlDocument();
            this.BaseStylesheet = activeSheet;
            this.commonHtmlDoc.ActiveCssTemplate = new Composers.ActiveCssTemplate(activeSheet);
        }
        public HtmlIslandHost(GraphicsPlatform gfxplatform)
            : this(gfxplatform, LayoutFarm.Composers.CssParserHelper.ParseStyleSheet(null, true))
        {
            //use default style sheet
        }
        public GraphicsPlatform GfxPlatform { get { return this.gfxplatform; } }
        public WebDom.CssActiveSheet BaseStylesheet { get; private set; }

        public virtual void RequestImage(ImageBinder binder, HtmlIsland reqIsland, object reqFrom, bool _sync)
        {
            if (this.RequestResource != null)
            {
                HtmlResourceRequestEventArgs resReq = new HtmlResourceRequestEventArgs();
                resReq.binder = binder;
                resReq.requestBy = reqFrom;
                RequestResource(this, resReq);
            }
        }
        public virtual void RequestStyleSheet(TextLoadRequestEventArgs e)
        {

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

        public LayoutFarm.HtmlBoxes.LayoutVisitor GetSharedHtmlLayoutVisitor(HtmlIsland island)
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
            lay.Bind(island);
            return lay;
        }
        public void ReleaseHtmlLayoutVisitor(LayoutFarm.HtmlBoxes.LayoutVisitor lay)
        {
            lay.UnBind();
            this.htmlLayoutVisitorStock.Enqueue(lay);
        }
        public HtmlInputEventAdapter GetSharedInputEventAdapter(HtmlIsland island)
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
            adapter.Bind(island);
            return adapter;
        }
        public void ReleaseSharedInputEventAdapter(HtmlInputEventAdapter adapter)
        {
            adapter.Unbind();
            this.inputEventAdapterStock.Enqueue(adapter);
        }
        public LayoutFarm.Composers.RenderTreeBuilder GetRenderTreeBuilder(RootGraphic rootgfx)
        {
            if (this.renderTreeBuilder == null)
            {
                renderTreeBuilder = new Composers.RenderTreeBuilder(rootgfx);
                this.renderTreeBuilder.RequestStyleSheet += (e) =>
                {
                    var req = new TextLoadRequestEventArgs(e.Src);
                    this.RequestStyleSheet(req);
                    e.SetStyleSheet = req.SetStyleSheet;

                };
            }
            return renderTreeBuilder;
        }





    }





    public sealed class MyHtmlIsland : HtmlIsland
    {
        WebDocument webdoc;
        HtmlIslandHost islandHost;

        int lastDomUpdateVersion;
        public event EventHandler DomVisualRefresh;
        public event EventHandler DomRequestRebuild;

        public MyHtmlIsland(HtmlIslandHost islandHost)
        {
            this.islandHost = islandHost;
        }

        public DomElement RootElement
        {
            get { return webdoc.RootNode; }
        }
        public WebDocument WebDocument
        {
            get { return this.webdoc; }
            set { this.webdoc = value; }
        }

        public override bool RefreshIfNeed()
        {
            if (webdoc == null) return false;
            //----------------------------------

            int latestDomUpdateVersion = webdoc.DomUpdateVersion;
            if (this.lastDomUpdateVersion != latestDomUpdateVersion)
            {
                this.lastDomUpdateVersion = latestDomUpdateVersion;
                //reset 
                this.NeedLayout = false;

                if (DomVisualRefresh != null)
                {
                    DomVisualRefresh(this, EventArgs.Empty);
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
            this.islandHost.ClearPreviousSelection();
        }
        public override void SetSelection(SelectionRange selRange)
        {
            this.islandHost.SelectionRange = selRange;
        }


        public bool NeedLayout
        {
            get;
            private set;
        }
        protected override void OnRequestImage(ImageBinder binder, object reqFrom, bool _sync)
        {

            //manage image loading 
            if (binder.State == ImageBinderState.Unload)
            {
                this.islandHost.RequestImage(binder, this, reqFrom, _sync);
            }

        }
        /// <summary>
        /// check if dom update
        /// </summary>
        public void CheckDocUpdate()
        {
            if (webdoc != null &&
                webdoc.DocumentState == DocumentState.ChangedAfterIdle
                && DomRequestRebuild != null)
            {
                DomRequestRebuild(this, EventArgs.Empty);
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
