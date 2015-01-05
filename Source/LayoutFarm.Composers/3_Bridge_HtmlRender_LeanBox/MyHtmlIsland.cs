//BSD 2014,WinterDev
//ArthurHub

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
        internal IUpdateChangeListener updateEventListener;
    }



    public class MyHtmlIsland : HtmlIsland, IUpdateChangeListener
    {

        WebDocument doc;
        public event EventHandler<HtmlRefreshEventArgs> Refresh;
        public event EventHandler<HtmlResourceRequestEventArgs> RequestResource;
        public event EventHandler<EventArgs> NeedUpdateDom;
        int newUpdateImageCount = 0;
        SelectionRange _currentSelectionRange;
        //List<ImageBinder> recentUpdateImageBinders = new List<ImageBinder>();
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
        public WebDocument Document
        {
            get { return this.doc; }
            set { this.doc = value; }
        }

        public bool NeedRefresh()
        {

            //not need to store that binder 
            //(else if you want to debug) 
            if (this.newUpdateImageCount > 0)
            {
                //reset
                this.newUpdateImageCount = 0;
                this.RequestRefresh(false);
#if DEBUG
                dbugCount02++;
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
            if (_currentSelectionRange != null)
            {
                _currentSelectionRange.ClearSelectionStatus();
                _currentSelectionRange = null;
            }
        }
        public override void SetSelection(SelectionRange selRange)
        {
            _currentSelectionRange = selRange;
        }
        void IUpdateChangeListener.AddUpdatedImageBinder(ImageBinder binder)
        {
            //not need to store that binder 
            //(else if you want to debug)

            newUpdateImageCount++;
            //this.recentUpdateImageBinders.Add(binder);
        }

        protected override void RequestRefresh(bool layout)
        {
            if (this.Refresh != null)
            {
                this.Refresh(this, new HtmlRefreshEventArgs(layout));
            }
        }
        protected override void OnRequestImage(ImageBinder binder, object reqFrom, bool _sync)
        {

            //manage image loading 
            if (this.RequestResource != null)
            {
                if (binder.State == ImageBinderState.Unload)
                {
                    HtmlResourceRequestEventArgs resReq = new HtmlResourceRequestEventArgs();
                    resReq.binder = binder;
                    resReq.requestBy = reqFrom;
                    resReq.updateEventListener = this;
                    RequestResource(this, resReq);
                }

            }
        }


        /// <summary>
        /// check if dom update
        /// </summary>
        public void CheckDocUpdate()
        {
            if (doc != null &&
                doc.DocumentState == DocumentState.ChangedAfterIdle &&
                NeedUpdateDom != null)
            {
                NeedUpdateDom(this, EventArgs.Empty);
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
