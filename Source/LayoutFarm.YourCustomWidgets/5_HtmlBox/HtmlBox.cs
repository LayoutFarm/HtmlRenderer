// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm;
using LayoutFarm.ContentManagers;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.Composers;

using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{

    public class HtmlBox : UIElement, IUserEventPortal
    {
        RootGraphic rootgfx;
        HtmlRenderBox htmlRenderBox;
        int _width;
        int _height;
        MyHtmlIsland myHtmlIsland;
        LayoutVisitor htmlLayoutVisitor;

        LayoutFarm.WebDom.WebDocument currentdoc;

        public event EventHandler<TextLoadRequestEventArgs> RequestStylesheet;
        public event EventHandler<ImageRequestEventArgs> RequestImage;

        bool hasWaitingDocToLoad;
        LayoutFarm.WebDom.CssActiveSheet waitingCssData;
        HtmlInputEventAdapter inputEventAdapter;
        object uiHtmlTask = new object();


        LayoutFarm.Composers.RenderTreeBuilder renderTreeBuilder = null;
        HtmlIslandHost islandHost;
        static HtmlBox()
        {
            LayoutFarm.Composers.BoxCreator.RegisterCustomCssBoxGenerator(
               new MyCssBoxGenerator());
        }
        public HtmlBox(int width, int height)
        {
            this._width = width;
            this._height = height;

            this.islandHost = new HtmlIslandHost();
            this.islandHost.BaseStylesheet = LayoutFarm.Composers.CssParserHelper.ParseStyleSheet(null, true);
            this.islandHost.RequestResource += (s, e) =>
            {
                if (this.RequestImage != null)
                {
                    RequestImage(this, new ImageRequestEventArgs(e.binder));
                }
            };

            myHtmlIsland = new MyHtmlIsland(islandHost);
            myHtmlIsland.DomVisualRefresh += (s, e) => this.InvalidateGraphics();
            myHtmlIsland.DomRequestRebuild += myHtmlIsland_NeedUpdateDom;
            //request ui timer *** 
            //tim.Interval = 30;
            //tim.Elapsed += new System.Timers.ElapsedEventHandler(tim_Elapsed);
        }
        //--------------------------------------------------------------------

        void IUserEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {

            inputEventAdapter.MouseUp(e);
        }
        void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {
            e.CurrentContextElement = this;
            inputEventAdapter.MouseDown(e);
        }
        void IUserEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {
            inputEventAdapter.MouseMove(e);

        }
        void IUserEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {

        }

        void IUserEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            inputEventAdapter.KeyDown(e);
        }
        void IUserEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            inputEventAdapter.KeyPress(e);
        }
        void IUserEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            inputEventAdapter.KeyUp(e);
        }
        bool IUserEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            return this.inputEventAdapter.ProcessDialogKey(e);
        }
        void IUserEventPortal.PortalGotFocus(UIFocusEventArgs e)
        {
        }
        void IUserEventPortal.PortalLostFocus(UIFocusEventArgs e)
        {
        }
        internal MyHtmlIsland HtmlIsland
        {
            get { return this.myHtmlIsland; }
        }

        void CreateRenderTreeBuilder()
        {
            this.renderTreeBuilder = new LayoutFarm.Composers.RenderTreeBuilder(htmlRenderBox.Root);
            this.renderTreeBuilder.RequestStyleSheet += (e2) =>
            {
                if (this.RequestStylesheet != null)
                {
                    var req = new TextLoadRequestEventArgs(e2.Src);
                    RequestStylesheet(this, req);
                    e2.SetStyleSheet = req.SetStyleSheet;
                }
            };

        }
        void myHtmlIsland_NeedUpdateDom(object sender, EventArgs e)
        {
            hasWaitingDocToLoad = true;
            //---------------------------
            if (htmlRenderBox == null) return;
            //--------------------------- 
            if (this.renderTreeBuilder == null) CreateRenderTreeBuilder();

            //var builder = new HtmlRenderer.Composers.RenderTreeBuilder(htmlRenderBox.Root);
            //builder.RequestStyleSheet += (e2) =>
            //{
            //    if (this.RequestStylesheet != null)
            //    {
            //        var req = new TextLoadRequestEventArgs(e2.Src);
            //        RequestStylesheet(this, req);
            //        e2.SetStyleSheet = req.SetStyleSheet;
            //    }
            //};

            renderTreeBuilder.RefreshCssTree(this.currentdoc.RootNode);
            this.myHtmlIsland.PerformLayout(htmlLayoutVisitor);

        }

        protected override void OnKeyUp(UIKeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (htmlRenderBox == null)
            {
                this.rootgfx = rootgfx;
                htmlRenderBox = new HtmlRenderBox(rootgfx, _width, _height, myHtmlIsland);
                htmlRenderBox.SetController(this);
                htmlRenderBox.HasSpecificSize = true;

                inputEventAdapter = new HtmlInputEventAdapter(rootgfx.SampleIFonts);
                inputEventAdapter.Bind(this.myHtmlIsland);

                htmlLayoutVisitor = new LayoutVisitor(rootgfx.P);
                htmlLayoutVisitor.Bind(this.myHtmlIsland);
            }
            //-------------------------
            rootgfx.SubscribeGraphicsIntervalTask(uiHtmlTask,
                 LayoutFarm.RenderBoxes.TaskIntervalPlan.Animation, 25,
                 (s, e) =>
                 {
                     if (this.myHtmlIsland.RefreshIfNeed())
                     {
                         e.NeedUpdate = 1;
                     }
                 });
            //-------------------------


            if (this.hasWaitingDocToLoad)
            {
                UpdateWaitingHtmlDoc(this.htmlRenderBox.Root);
            }
            return htmlRenderBox;
        }
        void UpdateWaitingHtmlDoc(RootGraphic rootgfx)
        {
            if (this.renderTreeBuilder == null) CreateRenderTreeBuilder();
            //------------------------------------------------------------


            //build rootbox from htmldoc
            var rootBox = renderTreeBuilder.BuildCssRenderTree((HtmlDocument)this.currentdoc,
                rootgfx.SampleIFonts,
                this.waitingCssData,
                this.htmlRenderBox);

            //update htmlIsland
            var htmlIsland = this.myHtmlIsland;
            htmlIsland.RootElement = this.currentdoc.RootNode;
            htmlIsland.RootCssBox = rootBox;
            //htmlIsland.MaxSize = new PixelFarm.Drawing.SizeF(this._width, 0);
            htmlIsland.SetMaxSize(this._width, 0);
            htmlIsland.PerformLayout(this.htmlLayoutVisitor);
        }
        void SetHtml(MyHtmlIsland htmlIsland, string html, LayoutFarm.WebDom.CssActiveSheet cssData)
        {
            var htmldoc = LayoutFarm.Composers.WebDocumentParser.ParseDocument(
                             new LayoutFarm.WebDom.Parser.TextSnapshot(html.ToCharArray()));
            this.currentdoc = htmldoc;
            this.hasWaitingDocToLoad = true;
            this.waitingCssData = cssData;
            //---------------------------
            if (htmlRenderBox == null) return;
            //---------------------------
            UpdateWaitingHtmlDoc(this.htmlRenderBox.Root);
            htmlRenderBox.InvalidateGraphics();
        }
        public void LoadHtmlText(string html)
        {
            SetHtml(myHtmlIsland, html, this.islandHost.BaseStylesheet);
        }
        public override void InvalidateGraphics()
        {
            this.htmlRenderBox.InvalidateGraphics();
        }
        protected override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.htmlRenderBox; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this.htmlRenderBox != null; }
        }
    }
}





