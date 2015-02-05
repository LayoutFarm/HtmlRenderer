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
    public class HtmlImageRequestEventArgs : EventArgs
    {
        public ImageBinder binder;
        public object requestBy;

    }
    public delegate void HtmlIslandUpdated(HtmlIsland island);

    public class HtmlIslandHost
    {
        HtmlIslandUpdated islandUpdateHandler;
        public event EventHandler<HtmlImageRequestEventArgs> RequestImage;
        public event EventHandler<TextLoadRequestEventArgs> RequestStyleSheet;

        SelectionRange _currentSelectionRange;
        GraphicsPlatform gfxplatform;
        Composers.HtmlDocument commonHtmlDoc;


        public HtmlIslandHost(GraphicsPlatform gfxplatform, WebDom.CssActiveSheet activeSheet)
        {

            this.gfxplatform = gfxplatform;
            this.commonHtmlDoc = new Composers.HtmlDocument();

            this.BaseStylesheet = activeSheet;
            this.commonHtmlDoc.CssActiveSheet = activeSheet;
        }
        public HtmlIslandHost(GraphicsPlatform gfxplatform)
            : this(gfxplatform, LayoutFarm.Composers.CssParserHelper.ParseStyleSheet(null, true))
        {
            //use default style sheet
        }
        public void SetHtmlIslandUpdateHandler(HtmlIslandUpdated islandUpdateHandler)
        {
            this.islandUpdateHandler = islandUpdateHandler;
        }


        public GraphicsPlatform GfxPlatform { get { return this.gfxplatform; } }
        public WebDom.CssActiveSheet BaseStylesheet { get; private set; }

        public void ChildRequestImage(ImageBinder binder, HtmlIsland reqIsland, object reqFrom, bool _sync)
        {
            if (this.RequestImage != null)
            {
                HtmlImageRequestEventArgs resReq = new HtmlImageRequestEventArgs();
                resReq.binder = binder;
                resReq.requestBy = reqFrom;
                RequestImage(this, resReq);
            }
        }
      
        /// <summary>
        /// Get stylesheet by given key.
        /// </summary>
        static string GetDefaultStyleSheet(string src)
        {
            if (src == "StyleSheet")
            {
                return @"h1, h2, h3 { color: navy; font-weight:normal; }
                    h1 { margin-bottom: .47em }
                    h2 { margin-bottom: .3em }
                    h3 { margin-bottom: .4em }
                    ul { margin-top: .5em }
                    ul li {margin: .25em}
                    body { font:10pt Tahoma }
		            pre  { border:solid 1px gray; background-color:#eee; padding:1em }
                    a:link { text-decoration: none; }
                    a:hover { text-decoration: underline; }
                    .gray    { color:gray; }
                    .example { background-color:#efefef; corner-radius:5px; padding:0.5em; }
                    .whitehole { background-color:white; corner-radius:10px; padding:15px; }
                    .caption { font-size: 1.1em }
                    .comment { color: green; margin-bottom: 5px; margin-left: 3px; }
                    .comment2 { color: green; }";
            }
            return null;
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
        public LayoutFarm.Composers.RenderTreeBuilder GetRenderTreeBuilder()
        {
            if (this.renderTreeBuilder == null)
            {
                renderTreeBuilder = new Composers.RenderTreeBuilder(this.gfxplatform);
                this.renderTreeBuilder.RequestStyleSheet += (e) =>
                {
                   
                    //---------------------------
                    var stylesheet = GetDefaultStyleSheet(e.Src);
                    if (stylesheet != null)
                    {
                        e.SetStyleSheet = stylesheet;
                    }
                    else if (RequestStyleSheet != null)
                    {
                        var req = new TextLoadRequestEventArgs(e.Src);
                        RequestStyleSheet(this, req);
                        e.SetStyleSheet = req.SetStyleSheet;
                    }
                    //---------------------------
                    
                };
            }
            return renderTreeBuilder;
        }
        internal void NotifyIslandUpdate(HtmlIsland island)
        {
            if (islandUpdateHandler != null)
            {
                islandUpdateHandler(island);
            }
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
                        this.islandHost.NotifyIslandUpdate(this);
                    });
                }
            }
        }

        public bool RefreshIfNeed()
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
            //send request to host
            if (binder.State == ImageBinderState.Unload)
            {
                this.islandHost.ChildRequestImage(binder, this, reqFrom, _sync);
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
