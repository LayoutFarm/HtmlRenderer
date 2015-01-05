//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HtmlRenderer;
using HtmlRenderer.ContentManagers;
using HtmlRenderer.Boxes;
using HtmlRenderer.Composers;

using LayoutFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.Boxes;

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

        HtmlRenderer.WebDom.WebDocument currentdoc;
        public event EventHandler<TextLoadRequestEventArgs> RequestStylesheet;
        public event EventHandler<ImageRequestEventArgs> RequestImage;

        bool hasWaitingDocToLoad;
        HtmlRenderer.WebDom.CssActiveSheet waitingCssData;
        HtmlInputEventAdapter inputEventAdapter;
        object uiHtmlTask = new object();


        HtmlRenderer.Composers.RenderTreeBuilder renderTreeBuilder = null;
        static HtmlBox()
        {
            HtmlRenderer.Composers.BridgeHtml.BoxCreator.RegisterCustomCssBoxGenerator(
               new HtmlRenderer.Boxes.LeanBoxCreator());
        }
        public HtmlBox(int width, int height)
        {
            this._width = width;
            this._height = height;

            myHtmlIsland = new MyHtmlIsland();
            myHtmlIsland.BaseStylesheet = HtmlRenderer.Composers.CssParserHelper.ParseStyleSheet(null, true);
            myHtmlIsland.Refresh += myHtmlIsland_Refresh;
            myHtmlIsland.NeedUpdateDom += myHtmlIsland_NeedUpdateDom;
            myHtmlIsland.RequestResource += myHtmlIsland_RequestResource;

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
        void myHtmlIsland_RequestResource(object sender, HtmlResourceRequestEventArgs e)
        {
            if (this.RequestImage != null)
            {
                RequestImage(this, new ImageRequestEventArgs(e.binder));
            }
        }
        void CreateRenderTreeBuilder()
        {
            this.renderTreeBuilder = new HtmlRenderer.Composers.RenderTreeBuilder(htmlRenderBox.Root);
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

            var rootBox2 = renderTreeBuilder.RefreshCssTree(this.currentdoc);
            this.myHtmlIsland.PerformLayout(htmlLayoutVisitor);

        }
        /// <summary>
        /// Handle html renderer invalidate and re-layout as requested.
        /// </summary>
        void myHtmlIsland_Refresh(object sender, HtmlRenderer.WebDom.HtmlRefreshEventArgs e)
        {
            this.InvalidateGraphic();
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
            rootgfx.RequestGraphicsIntervalTask(uiHtmlTask,
                 TaskIntervalPlan.Animation, 25,
                 (s, e) =>
                 {

                     if (this.myHtmlIsland.InternalRefreshRequest())
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
            var rootBox = renderTreeBuilder.BuildCssRenderTree(this.currentdoc,
                rootgfx.SampleIFonts,
                this.waitingCssData,
                this.htmlRenderBox);

            //update htmlIsland
            var htmlIsland = this.myHtmlIsland;
            htmlIsland.Document = this.currentdoc;
            htmlIsland.RootCssBox = rootBox;
            //htmlIsland.MaxSize = new LayoutFarm.Drawing.SizeF(this._width, 0);
            htmlIsland.SetMaxSize(this._width, 0);
            htmlIsland.PerformLayout(this.htmlLayoutVisitor);
        }
        void SetHtml(MyHtmlIsland htmlIsland, string html, HtmlRenderer.WebDom.CssActiveSheet cssData)
        {
            var htmldoc = HtmlRenderer.Composers.WebDocumentParser.ParseDocument(
                             new HtmlRenderer.WebDom.Parser.TextSnapshot(html.ToCharArray()));
            this.currentdoc = htmldoc;
            this.hasWaitingDocToLoad = true;
            this.waitingCssData = cssData;
            //---------------------------
            if (htmlRenderBox == null) return;
            //---------------------------
            UpdateWaitingHtmlDoc(this.htmlRenderBox.Root);
        }
        public void LoadHtmlText(string html)
        {
            //myHtmlBox.LoadHtmlText(html);
            //this.tim.Enabled = false;
            SetHtml(myHtmlIsland, html, myHtmlIsland.BaseStylesheet);
            //this.tim.Enabled = true;
            if (this.htmlRenderBox != null)
            {
                htmlRenderBox.InvalidateGraphic();
            }
        }
        public override void InvalidateGraphic()
        {
            if (this.htmlRenderBox != null)
            {
                htmlRenderBox.InvalidateGraphic();
            }
        }
    }
}





