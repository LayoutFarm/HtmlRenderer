using System;
using System.Collections.Generic;

using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

using HtmlRenderer.Boxes;
using HtmlRenderer.WebDom;
using HtmlRenderer.Handlers;
using HtmlRenderer.Drawing;
using HtmlRenderer.ContentManagers;
using HtmlRenderer.Diagnostics;

namespace HtmlRenderer
{

    public class HtmlContainerImpl : HtmlContainer
    {


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

        public HtmlContainerImpl()
        {

            this.IsSelectionEnabled = true;
            imageContentManager = new ImageContentManager(this);
            textContentManager = new TextContentManager(this);

        }
        protected override void RequestRefresh(bool layout)
        {
            if (this.Refresh != null)
            {
                HtmlRefreshEventArgs arg = new HtmlRefreshEventArgs(layout);
                this.Refresh(this, arg);
            }
        }
        protected override void OnRequestImage(ImageBinder binder, CssBox requestBox, bool _sync)
        {

            //manage image loading 
            if (imageContentManager != null)
            {
                if (binder.State == ImageBinderState.Unload)
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
        protected override void OnRequestStyleSheet(string hrefSource,
            out string stylesheet,
            out WebDom.CssActiveSheet stylesheetData)
        {
            if (textContentManager != null)
            {
                textContentManager.AddStyleSheetRequest(hrefSource,
                    out stylesheet,
                    out stylesheetData);
            }
            else
            {
                stylesheet = null;
                stylesheetData = null;
            }

        }
        public void PerformPaint(Graphics g)
        {
            using (var gfx = new WinGraphics(g, this.UseGdiPlusTextRendering))
            {
                Region prevClip = null;
                if (this.MaxSize.Height > 0)
                {
                    prevClip = g.Clip;
                    g.SetClip(new RectangleF(this.Location, this.MaxSize));
                }

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
                using (var gfx = new WinGraphics(g, this.UseGdiPlusTextRendering))
                {
                    this.PerformLayout(gfx);
                }
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
        ///// <summary>
        ///// Handle mouse down to handle selection.
        ///// </summary>
        ///// <param name="parent">the control hosting the html to invalidate</param>
        ///// <param name="e">the mouse event args</param>
        //public void HandleMouseDown(Control parent, MouseEventArgs e)
        //{


        //    //try
        //    //{

        //    //    //mouse down  
        //    //    if (_selectionHandler != null)
        //    //    {
        //    //        _selectionHandler.HandleMouseDown(parent, OffsetByScroll(e.Location),
        //    //            IsMouseInContainer(e.Location));
        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    ReportError(HtmlRenderer.Diagnostics.HtmlRenderErrorType.KeyboardMouse, "Failed mouse down handle", ex);
        //    //}
        //}
        ///// <summary>
        ///// Handle link clicked going over <see cref="LinkClicked"/> event and using <see cref="Process.Start()"/> if not canceled.
        ///// </summary>
        ///// <param name="parent">the control hosting the html to invalidate</param>
        ///// <param name="e">the mouse event args</param>
        ///// <param name="link">the link that was clicked</param>
        //internal void HandleLinkClicked(Control parent, MouseEventArgs e, CssBox link)
        //{

        //    //wait for another technique
        //    //var element = link.HtmlElement;
        //    //var href = element.TryGetAttribute("href", null);

        //    //if (LinkClicked != null)
        //    //{

        //    //    if (href != null)
        //    //    {
        //    //        var args = new HtmlLinkClickedEventArgs(href, link.HtmlElement);
        //    //        try
        //    //        {
        //    //            LinkClicked(this, args);
        //    //        }
        //    //        catch (Exception ex)
        //    //        {
        //    //            throw new HtmlLinkClickedException("Error in link clicked intercept", ex);
        //    //        }
        //    //        if (args.Handled)
        //    //        {
        //    //            return;
        //    //        }
        //    //    }
        //    //}

        //    //if (!string.IsNullOrEmpty(href))
        //    //{
        //    //    if (href.StartsWith("#") && href.Length > 1)
        //    //    {
        //    //        if (ScrollChange != null)
        //    //        {
        //    //            var rect = GetElementRectangle(href.Substring(1));
        //    //            if (rect.HasValue)
        //    //            {
        //    //                ScrollChange(this, new HtmlScrollEventArgs(Point.Round(rect.Value.Location)));
        //    //                HandleMouseMove(parent, e);
        //    //            }
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        var nfo = new ProcessStartInfo(href);
        //    //        nfo.UseShellExecute = true;
        //    //        Process.Start(nfo);
        //    //    }
        //    //}
        //}

        ///// <summary>
        ///// Handle mouse up to handle selection and link click.
        ///// </summary>
        ///// <param name="parent">the control hosting the html to invalidate</param>
        ///// <param name="e">the mouse event args</param>
        //public void HandleMouseUp(Control parent, MouseEventArgs e)
        //{

        //    //try
        //    //{
        //    //    if (_selectionHandler != null && IsMouseInContainer(e.Location))
        //    //    {
        //    //        var ignore = _selectionHandler.HandleMouseUp(parent, e.Button);
        //    //        if (!ignore && (e.Button & MouseButtons.Left) != 0)
        //    //        {
        //    //            //var loc = OffsetByScroll(e.Location);
        //    //            //var link = DomUtils.GetLinkBox(_root, loc);
        //    //            //if (link != null)
        //    //            //{
        //    //            //    HandleLinkClicked(parent, e, link);
        //    //            //}
        //    //        }
        //    //    }
        //    //}
        //    //catch (HtmlLinkClickedException)
        //    //{
        //    //    throw;
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    ReportError(HtmlRenderErrorType.KeyboardMouse, "Failed mouse up handle", ex);
        //    //}
        //}

        ///// <summary>
        ///// Handle mouse double click to select word under the mouse.
        ///// </summary>
        ///// <param name="parent">the control hosting the html to set cursor and invalidate</param>
        ///// <param name="e">mouse event args</param>
        //public void HandleMouseDoubleClick(Control parent, MouseEventArgs e)
        //{

        //    //try
        //    //{
        //    //    if (_selectionHandler != null && IsMouseInContainer(e.Location))
        //    //        _selectionHandler.SelectWord(parent, OffsetByScroll(e.Location));
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    ReportError(HtmlRenderErrorType.KeyboardMouse, "Failed mouse double click handle", ex);
        //    //}
        //}

        /////// <summary>
        ///// Handle mouse move to handle hover cursor and text selection.
        ///// </summary>
        ///// <param name="parent">the control hosting the html to set cursor and invalidate</param>
        ///// <param name="e">the mouse event args</param>
        //public void HandleMouseMove(Control parent, MouseEventArgs e)
        //{


        //    //try
        //    //{
        //    var loc = OffsetByScroll(e.Location);
        //    if (_selectionHandler != null && IsMouseInContainer(e.Location))
        //        _selectionHandler.HandleMouseMove(parent, loc);

        //    /*
        //    if( _hoverBoxes != null )
        //    {
        //        bool refresh = false;
        //        foreach(var hoverBox in _hoverBoxes)
        //        {
        //            foreach(var rect in hoverBox.Item1.Rectangles.Values)
        //            {
        //                if( rect.Contains(loc) )
        //                {
        //                    //hoverBox.Item1.Color = "gold";
        //                    refresh = true;
        //                }
        //            }
        //        }

        //        if(refresh)
        //            RequestRefresh(true);
        //    }
        //     */
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    ReportError(HtmlRenderErrorType.KeyboardMouse, "Failed mouse move handle", ex);
        //    //}
        //}

        ///// <summary>
        ///// Handle mouse leave to handle hover cursor.
        ///// </summary>
        ///// <param name="parent">the control hosting the html to set cursor and invalidate</param>
        //public void HandleMouseLeave(Control parent)
        //{
        //    try
        //    {
        //        if (_selectionHandler != null)
        //            _selectionHandler.HandleMouseLeave(parent);
        //    }
        //    catch (Exception ex)
        //    {
        //        ReportError(HtmlRenderErrorType.KeyboardMouse, "Failed mouse leave handle", ex);
        //    }
        //}

        /// <summary>
        /// Handle key down event for selection and copy.
        /// </summary>
        /// <param name="parent">the control hosting the html to invalidate</param>
        /// <param name="e">the pressed key</param>
        public void HandleKeyDown(Control parent, KeyEventArgs e)
        {

            //try
            //{
            //    if (e.Control && _selectionHandler != null)
            //    {
            //        // select all
            //        if (e.KeyCode == Keys.A)
            //        {
            //            _selectionHandler.SelectAll(parent);
            //        }

            //        // copy currently selected text
            //        if (e.KeyCode == Keys.C)
            //        {
            //            _selectionHandler.CopySelectedHtml();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ReportError(HtmlRenderErrorType.KeyboardMouse, "Failed key down handle", ex);
            //}
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
