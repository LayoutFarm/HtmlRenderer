using System;
using System.Collections.Generic;

using System.Text;
using System.Diagnostics;
using LayoutFarm.Drawing;
using HtmlRenderer.WebDom;

using HtmlRenderer.ContentManagers;
using HtmlRenderer.Diagnostics;

using HtmlRenderer.Boxes;
using LayoutFarm.UI;

namespace HtmlRenderer.Composers
{
    public class HtmlResourceRequestEventArgs : EventArgs
    {
        public ImageBinder binder;
        public object requestBy;
        public IUpdateStateChangedListener updateChangeListener;
    }

    public class MyHtmlIsland : HtmlIsland, IUpdateStateChangedListener
    {

        WebDocument doc;
        CssActiveSheet activeCssSheet;

        /// <summary>
        /// Raised when an error occurred during html rendering.<br/>
        /// </summary>
        /// <remarks>
        /// There is no guarantee that the event will be raised on the main thread, it can be raised on thread-pool thread.
        /// </remarks>
        //public event EventHandler<HtmlRenderErrorEventArgs> RenderError;
        public event EventHandler<HtmlRefreshEventArgs> Refresh;
        public event EventHandler<HtmlResourceRequestEventArgs> RequestResource;
        public event EventHandler<EventArgs> NeedUpdateDom;
        List<ImageBinder> requestImageBinderUpdates = new List<ImageBinder>();



        //----------------------------------------------------------- 
        public MyHtmlIsland()
        {
            this.IsSelectionEnabled = true;
        }

        public WebDom.CssActiveSheet BaseStylesheet
        {
            get;
            set;

        }
        public void InternalRefreshRequest()
        {
            if (requestImageBinderUpdates.Count > 0)
            {
                requestImageBinderUpdates.Clear();
                this.RequestRefresh(false);
#if DEBUG
                dbugCount02++;
                //Console.WriteLine(dd);
#endif
            }
        }

        public override void AddRequestImageBinderUpdate(ImageBinder binder)
        {
            this.requestImageBinderUpdates.Add(binder);
        }
        protected override void RequestRefresh(bool layout)
        {
            if (this.Refresh != null)
            {
                this.Refresh(this, new HtmlRefreshEventArgs(layout));
            }


        }
        protected override void OnRequestImage(ImageBinder binder, CssBox requestBox, bool _sync)
        {

            //manage image loading 
            if (this.RequestResource != null)
            {
                if (binder.State == ImageBinderState.Unload)
                {
                    HtmlResourceRequestEventArgs resReq = new HtmlResourceRequestEventArgs();
                    resReq.binder = binder;
                    resReq.requestBy = requestBox;
                    resReq.updateChangeListener = this;
                    RequestResource(this, resReq);
                }

            }
        }

        public void SetHtmlDoc(WebDocument doc)
        {
            this.doc = doc;
        }
        public void SetRootCssBox(CssBox rootBox, CssActiveSheet activeCss)
        {
            this.activeCssSheet = activeCss;
            base.SetRootCssBox(rootBox);
        }
        public void CheckDocUpdate()
        {
            if (doc != null &&
                doc.DocumentState == DocumentState.ChangedAfterIdle &&
                NeedUpdateDom != null)
            {
                NeedUpdateDom(this, EventArgs.Empty);
            }
        }
        public void PerformPaint(LayoutFarm.Drawing.Canvas canvas)
        {
            if (doc == null) return;
            base.PerformPaint(canvas.GetIGraphics());
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
