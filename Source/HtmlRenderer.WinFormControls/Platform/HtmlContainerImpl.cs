using System;
using System.Collections.Generic;
 
using System.Text;
using System.Drawing;

using System.Diagnostics;
using HtmlRenderer.Dom;
using HtmlRenderer.Entities;
using HtmlRenderer.Handlers;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;
using System.Windows.Forms;


namespace HtmlRenderer
{

    public class HtmlContainerImpl : HtmlContainer
    {
        /// <summary>
        /// Handler for text selection in the html. 
        /// </summary>
        private SelectionHandler _selectionHandler;
        /// <summary>
        /// Raised when the user clicks on a link in the html.<br/>
        /// Allows canceling the execution of the link.
        /// </summary>
        public event EventHandler<HtmlLinkClickedEventArgs> LinkClicked;

        /// <summary>
        /// Raised when Html Renderer request scroll to specific location.<br/>
        /// This can occur on document anchor click.
        /// </summary>
        public event EventHandler<HtmlScrollEventArgs> ScrollChange;
        CssBox _root;

        public HtmlContainerImpl()
        {
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
        }
        public void PerformLayout(Graphics g)
        {
            using (var gfx = new WinGraphics(g, this.UseGdiPlusTextRendering))
            {
                this.PerformLayout(gfx);
            }
        }

        protected override void OnRootDisposed()
        {

            if (_selectionHandler != null)
            {
                _selectionHandler.Dispose();
            }
            _selectionHandler = null;
            this._root = null;
            base.OnRootDisposed();
        }
        protected override void OnRootCreated(CssBox root)
        {
            this._root = root;
            this._selectionHandler = new SelectionHandler(root);
            base.OnRootCreated(root);
        }
        protected override void OnAllDisposed()
        {
            this.LinkClicked = null;
        }
        /// <summary>
        /// Get the currently selected text segment in the html.
        /// </summary>
        public override string SelectedText
        {
            get { return _selectionHandler.GetSelectedText(); }
        }

        /// <summary>
        /// Copy the currently selected html segment with style.
        /// </summary>
        public override string SelectedHtml
        {
            get { return _selectionHandler.GetSelectedHtml(); }
        }

        /// <summary>
        /// Handle mouse down to handle selection.
        /// </summary>
        /// <param name="parent">the control hosting the html to invalidate</param>
        /// <param name="e">the mouse event args</param>
        public void HandleMouseDown(Control parent, MouseEventArgs e)
        {
            ArgChecker.AssertArgNotNull(parent, "parent");
            ArgChecker.AssertArgNotNull(e, "e");

            try
            {
                if (_selectionHandler != null)
                {
                    _selectionHandler.HandleMouseDown(parent, OffsetByScroll(e.Location), IsMouseInContainer(e.Location));
                }
            }
            catch (Exception ex)
            {
                ReportError(HtmlRenderErrorType.KeyboardMouse, "Failed mouse down handle", ex);
            }
        }
        /// <summary>
        /// Handle link clicked going over <see cref="LinkClicked"/> event and using <see cref="Process.Start()"/> if not canceled.
        /// </summary>
        /// <param name="parent">the control hosting the html to invalidate</param>
        /// <param name="e">the mouse event args</param>
        /// <param name="link">the link that was clicked</param>
        internal void HandleLinkClicked(Control parent, MouseEventArgs e, CssBox link)
        {
            if (LinkClicked != null)
            {
                var args = new HtmlLinkClickedEventArgs(link.HrefLink, link.HtmlTag.Attributes);
                try
                {
                    LinkClicked(this, args);
                }
                catch (Exception ex)
                {
                    throw new HtmlLinkClickedException("Error in link clicked intercept", ex);
                }
                if (args.Handled)
                    return;
            }

            if (!string.IsNullOrEmpty(link.HrefLink))
            {
                if (link.HrefLink.StartsWith("#") && link.HrefLink.Length > 1)
                {
                    if (ScrollChange != null)
                    {
                        var rect = GetElementRectangle(link.HrefLink.Substring(1));
                        if (rect.HasValue)
                        {
                            ScrollChange(this, new HtmlScrollEventArgs(Point.Round(rect.Value.Location)));
                            HandleMouseMove(parent, e);
                        }
                    }
                }
                else
                {
                    var nfo = new ProcessStartInfo(link.HrefLink);
                    nfo.UseShellExecute = true;
                    Process.Start(nfo);
                }
            }
        }

        /// <summary>
        /// Handle mouse up to handle selection and link click.
        /// </summary>
        /// <param name="parent">the control hosting the html to invalidate</param>
        /// <param name="e">the mouse event args</param>
        public void HandleMouseUp(Control parent, MouseEventArgs e)
        {
            ArgChecker.AssertArgNotNull(parent, "parent");
            ArgChecker.AssertArgNotNull(e, "e");

            try
            {
                if (_selectionHandler != null && IsMouseInContainer(e.Location))
                {
                    var ignore = _selectionHandler.HandleMouseUp(parent, e.Button);
                    if (!ignore && (e.Button & MouseButtons.Left) != 0)
                    {
                        var loc = OffsetByScroll(e.Location);
                        var link = DomUtils.GetLinkBox(_root, loc);
                        if (link != null)
                        {
                            HandleLinkClicked(parent, e, link);
                        }
                    }
                }
            }
            catch (HtmlLinkClickedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ReportError(HtmlRenderErrorType.KeyboardMouse, "Failed mouse up handle", ex);
            }
        }

        /// <summary>
        /// Handle mouse double click to select word under the mouse.
        /// </summary>
        /// <param name="parent">the control hosting the html to set cursor and invalidate</param>
        /// <param name="e">mouse event args</param>
        public void HandleMouseDoubleClick(Control parent, MouseEventArgs e)
        {
            ArgChecker.AssertArgNotNull(parent, "parent");
            ArgChecker.AssertArgNotNull(e, "e");

            try
            {
                if (_selectionHandler != null && IsMouseInContainer(e.Location))
                    _selectionHandler.SelectWord(parent, OffsetByScroll(e.Location));
            }
            catch (Exception ex)
            {
                ReportError(HtmlRenderErrorType.KeyboardMouse, "Failed mouse double click handle", ex);
            }
        }

        /// <summary>
        /// Handle mouse move to handle hover cursor and text selection.
        /// </summary>
        /// <param name="parent">the control hosting the html to set cursor and invalidate</param>
        /// <param name="e">the mouse event args</param>
        public void HandleMouseMove(Control parent, MouseEventArgs e)
        {
            ArgChecker.AssertArgNotNull(parent, "parent");
            ArgChecker.AssertArgNotNull(e, "e");

            try
            {
                var loc = OffsetByScroll(e.Location);
                if (_selectionHandler != null && IsMouseInContainer(e.Location))
                    _selectionHandler.HandleMouseMove(parent, loc);

                /*
                if( _hoverBoxes != null )
                {
                    bool refresh = false;
                    foreach(var hoverBox in _hoverBoxes)
                    {
                        foreach(var rect in hoverBox.Item1.Rectangles.Values)
                        {
                            if( rect.Contains(loc) )
                            {
                                //hoverBox.Item1.Color = "gold";
                                refresh = true;
                            }
                        }
                    }

                    if(refresh)
                        RequestRefresh(true);
                }
                 */
            }
            catch (Exception ex)
            {
                ReportError(HtmlRenderErrorType.KeyboardMouse, "Failed mouse move handle", ex);
            }
        }

        /// <summary>
        /// Handle mouse leave to handle hover cursor.
        /// </summary>
        /// <param name="parent">the control hosting the html to set cursor and invalidate</param>
        public void HandleMouseLeave(Control parent)
        {
            ArgChecker.AssertArgNotNull(parent, "parent");

            try
            {
                if (_selectionHandler != null)
                    _selectionHandler.HandleMouseLeave(parent);
            }
            catch (Exception ex)
            {
                ReportError(HtmlRenderErrorType.KeyboardMouse, "Failed mouse leave handle", ex);
            }
        }

        /// <summary>
        /// Handle key down event for selection and copy.
        /// </summary>
        /// <param name="parent">the control hosting the html to invalidate</param>
        /// <param name="e">the pressed key</param>
        public void HandleKeyDown(Control parent, KeyEventArgs e)
        {
            ArgChecker.AssertArgNotNull(parent, "parent");
            ArgChecker.AssertArgNotNull(e, "e");

            try
            {
                if (e.Control && _selectionHandler != null)
                {
                    // select all
                    if (e.KeyCode == Keys.A)
                    {
                        _selectionHandler.SelectAll(parent);
                    }

                    // copy currently selected text
                    if (e.KeyCode == Keys.C)
                    {
                        _selectionHandler.CopySelectedHtml();
                    }
                }
            }
            catch (Exception ex)
            {
                ReportError(HtmlRenderErrorType.KeyboardMouse, "Failed key down handle", ex);
            }
        }




    }
}
