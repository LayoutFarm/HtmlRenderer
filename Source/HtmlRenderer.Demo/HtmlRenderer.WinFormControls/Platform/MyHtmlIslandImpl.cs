using System;
using System.Collections.Generic;

using System.Text;
using System.Drawing;
using System.Diagnostics;

using HtmlRenderer.Boxes;
using HtmlRenderer.WebDom;
using HtmlRenderer.Drawing;
using HtmlRenderer.ContentManagers;
using HtmlRenderer.Diagnostics;




namespace HtmlRenderer
{

    public class MyHtmlIslandImpl : HtmlIsland
    {


        WebDocument doc;
        CssActiveSheet activeCssSheet;
        ImageContentManager imageContentManager;
        TextContentManager textContentManager;

        /// <summary>
        /// Raised when Html Renderer request scroll to specific location.<br/>
        /// This can occur on document anchor click.
        /// </summary>
        public event EventHandler<HtmlScrollEventArgs> ScrollChange;

        bool isRootCreated;

        /// <summary>
        /// Raised when an error occurred during html rendering.<br/>
        /// </summary>
        /// <remarks>
        /// There is no guarantee that the event will be raised on the main thread, it can be raised on thread-pool thread.
        /// </remarks>
        public event EventHandler<HtmlRenderErrorEventArgs> RenderError;
        public event EventHandler<HtmlRefreshEventArgs> Refresh;



        public MyHtmlIslandImpl()
        {

            this.IsSelectionEnabled = true;

            imageContentManager = new ImageContentManager(this);
            textContentManager = new TextContentManager(this);
        }
        /// <summary>
        /// connect to box composer 
        /// </summary>
        public Composers.BoxComposer BoxComposer
        {
            get;
            set;
        }



        protected override void RequestRefresh(bool layout)
        {
            if (this.Refresh != null)
            {
                HtmlRefreshEventArgs arg = new HtmlRefreshEventArgs(layout);
                this.Refresh(this, arg);
            }
        }
        protected override void OnRequestImage(LayoutFarm.Drawing.ImageBinder binder, CssBox requestBox, bool _sync)
        {

            //manage image loading 
            if (imageContentManager != null)
            {
                if (binder.State == LayoutFarm.Drawing.ImageBinderState.Unload)
                {
                    imageContentManager.AddRequestImage(new ImageContentRequest(binder, requestBox));
                }
            }
        }
        public ImageContentManager ImageContentMan
        {
            get
            {
                return this.imageContentManager;
            }
        }
        public TextContentManager TextContentMan
        {
            get
            {
                return this.textContentManager;
            }
        }

        public void SetHtmlDoc(HtmlRenderer.WebDom.WebDocument doc)
        {
            this.doc = doc;
        }
        public void SetRootCssBox(CssBox rootBox, HtmlRenderer.WebDom.CssActiveSheet activeCss)
        {
            this.activeCssSheet = activeCss;
            base.SetRootCssBox(rootBox);
        }
        public void PerformPaint(Graphics g)
        {
            if (doc == null)
            {
                return;
            }

            using (var gfx = LayoutFarm.Drawing.CurrentGraphicPlatform.P.CreateIGraphics(g))
            {
                Region prevClip = null;
                if (this.MaxSize.Height > 0)
                {

                    prevClip = g.Clip;
                    g.SetClip(new System.Drawing.RectangleF(
                       LayoutFarm.Drawing.Conv.ToPointF(this.Location),
                       LayoutFarm.Drawing.Conv.ToSizeF(this.MaxSize)));
                }
                //------------------------------------------------------

                if (doc.DocumentState == DocumentState.ChangedAfterIdle)
                {

                    WinHtmlRootVisualBoxExtension.RefreshHtmlDomChange(
                        this,
                        doc,
                        this.activeCssSheet);
                    this.PerformLayout(gfx);
                }
                //------------------------------------------------------ 

                this.PerformPaint(gfx);

                if (prevClip != null)
                {
                    g.SetClip(prevClip, System.Drawing.Drawing2D.CombineMode.Replace);
                }
            }

            //=============
            //System.Diagnostics.Stopwatch sw = new Stopwatch();
            //for (int i = 0; i < 200; ++i)
            //{
            //    dbugCounter.ResetPaintCount();
            //    long ticks = dbugCounter.Snap(sw, () =>
            //    {
            //        using (var gfx = new WinGraphics(g, this.UseGdiPlusTextRendering))
            //        {
            //            Region prevClip = null;
            //            if (this.MaxSize.Height > 0)
            //            {
            //                prevClip = g.Clip;
            //                g.SetClip(new RectangleF(this.Location, this.MaxSize));
            //            }

            //            this.PerformPaint(gfx);

            //            if (prevClip != null)
            //            {
            //                g.SetClip(prevClip, System.Drawing.Drawing2D.CombineMode.Replace);
            //            }
            //        }

            //    });
            //    //Console.WriteLine(string.Format("boxes{0}, lines{1}, runs{2}", dbugCounter.dbugBoxPaintCount, dbugCounter.dbugLinePaintCount, dbugCounter.dbugRunPaintCount));
            //    Console.WriteLine(ticks);
            //}
            //System.Diagnostics.Debugger.Break();
        }
        public void PerformLayout(Graphics g)
        {
            if (this.isRootCreated)
            {
                this.PerformLayout(LayoutFarm.Drawing.CurrentGraphicPlatform.P.SampleIGraphics);
            }
        }

        protected override void OnRootDisposed()
        {

            this.isRootCreated = false;
            base.OnRootDisposed();
        }
        protected override void OnRootCreated(CssBox root)
        {
            this.isRootCreated = true;
            //this._selectionHandler = new SelectionHandler(root, this);
            base.OnRootCreated(root);
        }
        protected override void OnAllDisposed()
        {

        }

        public string GetHtml()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Is content selection is enabled for the rendered html (default - true).<br/>
        /// If set to 'false' the rendered html will be static only with ability to click on links.
        /// </summary>
        public bool IsSelectionEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Is the build-in context menu enabled and will be shown on mouse right click (default - true)
        /// </summary>
        public bool IsContextMenuEnabled
        {
            get;
            set;
        }

    }
}
