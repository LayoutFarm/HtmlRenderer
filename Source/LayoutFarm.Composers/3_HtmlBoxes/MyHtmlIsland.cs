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

        public HtmlIslandHost()
        { 
        }
        public WebDom.CssActiveSheet BaseStylesheet { get; set; }
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
    }

    public sealed class MyHtmlIsland : HtmlIsland
    {

        HtmlIslandHost islandHost;
        WebDocument doc;

        public event EventHandler DomVisualRefresh;
        public event EventHandler DomRequestRebuild;

        public MyHtmlIsland(HtmlIslandHost islandHost)
        {
            this.islandHost = islandHost;
        }

        public WebDocument Document
        {
            get { return this.doc; }
            set { this.doc = value; }
        }

        public bool RefreshIfNeed()
        {

            //not need to store that binder 
            //(else if you want to debug) 
            if (this.newUpdateImageCount > 0)
            {
                //reset
                this.newUpdateImageCount = 0;
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
            //            if (recentUpdateImageBinders.Count > 0)
            //            {
            //                recentUpdateImageBinders.Clear();
            //                this.RequestRefresh(false);
            //#if DEBUG
            //                dbugCount02++;
            //                //Console.WriteLine(dd);
            //#endif
            //                return true;
            //            }
            //            return false;
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
            if (doc != null &&
                doc.DocumentState == DocumentState.ChangedAfterIdle
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
