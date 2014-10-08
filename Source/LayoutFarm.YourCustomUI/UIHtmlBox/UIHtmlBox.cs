//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HtmlRenderer;
using HtmlRenderer.ContentManagers;
using HtmlRenderer.Boxes;
using HtmlRenderer.Composers;

using LayoutFarm.UI;
using LayoutFarm.Boxes;

namespace LayoutFarm.SampleControls
{

    public class UIHtmlBox : UIElement
    {
        HtmlRenderBox myCssBoxWrapper;
        int _width, _height;
        MyHtmlIsland myHtmlIsland;

        HtmlRenderer.WebDom.WebDocument currentdoc;
        HtmlRenderer.Composers.HtmlInputEventBridge _htmlEventBridge;

        public event EventHandler<TextLoadRequestEventArgs> RequestStylesheet;
        public event EventHandler<ImageRequestEventArgs> RequestImage;

        System.Timers.Timer tim = new System.Timers.Timer();
        bool hasWaitingDocToLoad;
        HtmlRenderer.WebDom.CssActiveSheet waitingCssData;

        static UIHtmlBox()
        {
            HtmlRenderer.Composers.BridgeHtml.BoxCreator.RegisterCustomCssBoxGenerator(
               new HtmlRenderer.Boxes.LeanBoxCreator());
        }

        public UIHtmlBox(int width, int height)
        {
            this._width = width;
            this._height = height;

            myHtmlIsland = new MyHtmlIsland();
            myHtmlIsland.BaseStylesheet = HtmlRenderer.Composers.CssParserHelper.ParseStyleSheet(null, true);
            myHtmlIsland.Refresh += OnRefresh;
            myHtmlIsland.NeedUpdateDom += myHtmlIsland_NeedUpdateDom;
            myHtmlIsland.RequestResource += myHtmlIsland_RequestResource;

            tim.Interval = 30;
            tim.Elapsed += new System.Timers.ElapsedEventHandler(tim_Elapsed);
        }

        void tim_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.myHtmlIsland != null)
            {
                this.myHtmlIsland.InternalRefreshRequest();
            }
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
        void myHtmlIsland_NeedUpdateDom(object sender, EventArgs e)
        {
            hasWaitingDocToLoad = true;
            //---------------------------
            if (myCssBoxWrapper == null) return;
            //---------------------------

            var builder = new HtmlRenderer.Composers.RenderTreeBuilder(myCssBoxWrapper.Root);
            builder.RequestStyleSheet += (e2) =>
            {
                if (this.RequestStylesheet != null)
                {
                    var req = new TextLoadRequestEventArgs(e2.Src);
                    RequestStylesheet(this, req);
                    e2.SetStyleSheet = req.SetStyleSheet;
                }
            };
            var rootBox2 = builder.RefreshCssTree(this.currentdoc,
                LayoutFarm.Drawing.CurrentGraphicPlatform.P.SampleIGraphics,
                this.myHtmlIsland);

            this.myHtmlIsland.PerformLayout(LayoutFarm.Drawing.CurrentGraphicPlatform.P.SampleIGraphics);

        }
        /// <summary>
        /// Handle html renderer invalidate and re-layout as requested.
        /// </summary>
        void OnRefresh(object sender, HtmlRenderer.WebDom.HtmlRefreshEventArgs e)
        {
            this.InvalidateGraphic();

            //if (e.Layout)
            //{
            //    if (InvokeRequired)
            //        Invoke(new MethodInvoker(PerformLayout));
            //    elsedf 
            //        PerformLayout();
            //}
            //if (InvokeRequired)
            //    Invoke(new MethodInvoker(Invalidate));
            //else
            //    Invalidate();
        }
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            //mouse down on html box
            this._htmlEventBridge.MouseDown(e.X, e.Y, (int)e.Button);

        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            this._htmlEventBridge.MouseUp(e.X, e.Y, (int)e.Button);
        }
        protected override void OnKeyDown(UIKeyEventArgs e)
        {
            this._htmlEventBridge.KeyDown('a');
        }
        protected override void OnKeyPress(UIKeyPressEventArgs e)
        {

            this._htmlEventBridge.KeyDown(e.KeyChar);
        }
        protected override void OnKeyUp(UIKeyEventArgs e)
        {
            base.OnKeyUp(e);
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (myCssBoxWrapper == null)
            {
                _htmlEventBridge = new HtmlInputEventBridge();
                _htmlEventBridge.Bind(myHtmlIsland, rootgfx.SampleIFonts);
                myCssBoxWrapper = new HtmlRenderBox(rootgfx, _width, _height, myHtmlIsland);
                myCssBoxWrapper.SetController(this);
                myCssBoxWrapper.HasSpecificSize = true;
            }

            if (this.hasWaitingDocToLoad)
            {
                UpdateWaitingHtmlDoc(this.myCssBoxWrapper.Root);
            }
            return myCssBoxWrapper;
        }
        void UpdateWaitingHtmlDoc(RootGraphic rootgfx)
        {
            var builder = new HtmlRenderer.Composers.RenderTreeBuilder(rootgfx);
            builder.RequestStyleSheet += (e) =>
            {
                if (this.RequestStylesheet != null)
                {
                    var req = new TextLoadRequestEventArgs(e.Src);
                    RequestStylesheet(this, req);
                    e.SetStyleSheet = req.SetStyleSheet;
                }
            };

            //build rootbox from htmldoc
            var rootBox = builder.BuildCssRenderTree(this.currentdoc,
                LayoutFarm.Drawing.CurrentGraphicPlatform.P.SampleIFonts,
                this.myHtmlIsland,
                this.waitingCssData,
                this.myCssBoxWrapper);

            var htmlIsland = this.myHtmlIsland;
            htmlIsland.SetHtmlDoc(this.currentdoc);
            htmlIsland.SetRootCssBox(rootBox, this.waitingCssData);
            htmlIsland.MaxSize = new LayoutFarm.Drawing.SizeF(this._width, 0);
            htmlIsland.PerformLayout(rootgfx.SampleIGraphics);
        }
        void SetHtml(MyHtmlIsland htmlIsland, string html, HtmlRenderer.WebDom.CssActiveSheet cssData)
        {
            var htmldoc = HtmlRenderer.Composers.WebDocumentParser.ParseDocument(
                             new HtmlRenderer.WebDom.Parser.TextSnapshot(html.ToCharArray()));
            this.currentdoc = htmldoc;
            this.hasWaitingDocToLoad = true;
            this.waitingCssData = cssData;
            //---------------------------
            if (myCssBoxWrapper == null) return;
            //---------------------------
            UpdateWaitingHtmlDoc(this.myCssBoxWrapper.Root);

        }
        public void LoadHtmlText(string html)
        {
            //myHtmlBox.LoadHtmlText(html);
            this.tim.Enabled = false;
            SetHtml(myHtmlIsland, html, myHtmlIsland.BaseStylesheet);
            this.tim.Enabled = true;
            if (this.myCssBoxWrapper != null)
            {
                myCssBoxWrapper.InvalidateGraphic();
            }
        }


        public override void InvalidateGraphic()
        {
            if (this.myCssBoxWrapper != null)
            {
                myCssBoxWrapper.InvalidateGraphic();
            }
        }
    }
}





