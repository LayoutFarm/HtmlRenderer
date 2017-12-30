// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutFarm.WebDom;
using LayoutFarm.Composers;
using LayoutFarm.HtmlBoxes;
namespace LayoutFarm.Demo
{
    /// <summary>
    /// Provides HTML rendering using the text property.<br/>
    /// WinForms control that will render html content in it's client rectangle.<br/>
    /// If <see cref="AutoScroll"/> is true and the layout of the html resulted in its content beyond the client bounds 
    /// of the panel it will show scrollbars (horizontal/vertical) allowing to scroll the content.<br/>
    /// If <see cref="AutoScroll"/> is false html content outside the client bounds will be clipped.<br/>
    /// The control will handle mouse and keyboard events on it to support html text selection, copy-paste and mouse clicks.<br/>
    /// <para>
    /// The major differential to use HtmlPanel or HtmlLabel is size and scrollbars.<br/>
    /// If the size of the control depends on the html content the HtmlLabel should be used.<br/>
    /// If the size is set by some kind of layout then HtmlPanel is more suitable, also shows scrollbars if the html contents is larger than the control client rectangle.<br/>
    /// </para>
    /// <para>
    /// <h4>AutoScroll:</h4>
    /// Allows showing scrollbars if html content is placed outside the visible boundaries of the panel.
    /// </para>
    /// <para>
    /// <h4>LinkClicked event:</h4>
    /// Raised when the user clicks on a link in the html.<br/>
    /// Allows canceling the execution of the link.
    /// </para>
    /// <para>
    /// <h4>StylesheetLoad event:</h4>
    /// Raised when a stylesheet is about to be loaded by file path or URI by link element.<br/>
    /// This event allows to provide the stylesheet manually or provide new source (file or uri) to load from.<br/>
    /// If no alternative data is provided the original source will be used.<br/>
    /// </para>
    /// <para>
    /// <h4>ImageLoad event:</h4>
    /// Raised when an image is about to be loaded by file path or URI.<br/>
    /// This event allows to provide the image manually, if not handled the image will be loaded from file or download from URI.
    /// </para>
    /// <para>
    /// <h4>RenderError event:</h4>
    /// Raised when an error occurred during html rendering.<br/>
    /// </para>
    /// </summary>
    public class HtmlPanel : ScrollableControl
    {
        List<MyHtmlContainer> waitingUpdateList = new List<MyHtmlContainer>();
        LayoutFarm.WebDom.WebDocument currentDoc;
        LayoutFarm.HtmlBoxes.HtmlHost htmlhost;
        LayoutFarm.HtmlBoxes.MyHtmlContainer htmlContainer;
        HtmlInputEventAdapter _htmlInputEventAdapter;
        /// <summary>
        /// the raw base stylesheet data used in the control
        /// </summary>
        string _baseRawCssData;
        /// <summary>
        /// the base stylesheet data used in the control
        /// </summary>
        WebDom.CssActiveSheet _baseCssData;
        Timer timer01 = new Timer();
        LayoutFarm.HtmlBoxes.LayoutVisitor htmlLayoutVisitor;
        PixelFarm.Drawing.DrawBoard renderCanvas;


        /// <summary>
        /// Creates a new HtmlPanel and sets a basic css for it's styling.
        /// </summary>
        public HtmlPanel()
        {
            AutoScroll = true;
            BackColor = SystemColors.Window;
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //this.gfxPlatform = p;

            //this.renderCanvas = gfxPlatform.CreateCanvas(0, 0,
            //    this.canvasW = w, this.canvasH = h);
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            //create actual render canvas
            base.OnHandleCreated(e);
        }
        public HtmlHost HtmlHost
        {
            get { return this.htmlhost; }
        }
        public void SetHtmlHost(HtmlHost htmlhost)
        {
            this.htmlhost = htmlhost;//***
            htmlhost.SetHtmlContainerUpdateHandler(htmlCont =>
            {
                var updatedHtmlCont = htmlCont as MyHtmlContainer;
                if (updatedHtmlCont != null && !updatedHtmlCont.IsInUpdateQueue)
                {
                    updatedHtmlCont.IsInUpdateQueue = true;
                    waitingUpdateList.Add(updatedHtmlCont);
                }
            });
            htmlContainer = new MyHtmlContainer(htmlhost);
            htmlContainer.AttachEssentialHandlers(
                OnRefresh,
                myHtmlContainer_NeedUpdateDom,
                OnRefresh,
                null);
            //------------------------------------------------------- 
            htmlLayoutVisitor = new LayoutVisitor(htmlhost.GetTextService());
            htmlLayoutVisitor.Bind(htmlContainer);
            //------------------------------------------------------- 
            timer01.Interval = 20;//20ms?
            timer01.Tick += (s, e) =>
            {
                //----------------------
                //TODO: not to run this
                //----------------------
                //clear waiting
                int j = waitingUpdateList.Count;
                for (int i = 0; i < j; ++i)
                {
                    var htmlCont = waitingUpdateList[i];
                    htmlCont.IsInUpdateQueue = false;
                    htmlCont.RefreshDomIfNeed();
                }
                for (int i = j - 1; i >= 0; --i)
                {
                    waitingUpdateList.RemoveAt(i);
                }
            };
            timer01.Enabled = true;
            //-------------------------------------------
            _htmlInputEventAdapter = new HtmlInputEventAdapter();
            _htmlInputEventAdapter.Bind(htmlContainer);
            //-------------------------------------------
        }


        void myHtmlContainer_NeedUpdateDom(object sender, EventArgs e)
        {
            this.htmlhost.GetRenderTreeBuilder().RefreshCssTree(this.currentDoc.RootNode);
            this.htmlContainer.PerformLayout(this.htmlLayoutVisitor);
        }


        ///// <summary>
        ///// Raised when a stylesheet is about to be loaded by file path or URI by link element.<br/>
        ///// This event allows to provide the stylesheet manually or provide new source (file or uri) to load from.<br/>
        ///// If no alternative data is provided the original source will be used.<br/>
        ///// </summary>
        //public event EventHandler<TextLoadRequestEventArgs> StylesheetLoad;

        ///// <summary>
        ///// Raised when an image is about to be loaded by file path or URI.<br/>
        ///// This event allows to provide the image manually, if not handled the image will be loaded from file or download from URI.
        ///// </summary>
        //public event EventHandler<LayoutFarm.ContentManagers.ImageRequestEventArgs> ImageLoad;

        /// <summary>
        /// Gets or sets a value indicating if anti-aliasing should be avoided for geometry like backgrounds and borders (default - false).
        /// </summary>
        public bool AvoidGeometryAntialias
        {
            get;
            set;
        }


        public bool AvoidImagesLateLoading
        {
            get;
            set;
        }

        /// <summary>
        /// Is content selection is enabled for the rendered html (default - true).<br/>
        /// If set to 'false' the rendered html will be static only with ability to click on links.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Is content selection is enabled for the rendered html.")]
        public bool IsSelectionEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Is the build-in context menu enabled and will be shown on mouse right click (default - true)
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Is the build-in context menu enabled and will be shown on mouse right click.")]
        public bool IsContextMenuEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Set base stylesheet to be used by html rendered in the panel.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Set base stylesheet to be used by html rendered in the control.")]
        public string BaseStylesheet
        {
            get { return _baseRawCssData; }
            set
            {
                _baseRawCssData = value;
                _baseCssData = LayoutFarm.WebDom.Parser.CssParserHelper.ParseStyleSheet(value,
                    LayoutFarm.Composers.CssDefaults.DefaultCssData,
                    true);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the container enables the user to scroll to any controls placed outside of its visible boundaries. 
        /// </summary>
        [Browsable(true)]
        [Description("Sets a value indicating whether the container enables the user to scroll to any controls placed outside of its visible boundaries.")]
        public override bool AutoScroll
        {
            get { return base.AutoScroll; }
            set { base.AutoScroll = value; }
        }

        /// <summary>
        /// Gets or sets the text of this panel
        /// </summary>
        [Browsable(true)]
        [Description("Sets the html of this control.")]
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                if (!IsDisposed)
                {
                    VerticalScroll.Value = VerticalScroll.Minimum;
                    SetHtml(htmlContainer, Text, _baseCssData);
                    PerformLayout();
                    Invalidate();
                }
            }
        }
        void SetHtml(LayoutFarm.HtmlBoxes.MyHtmlContainer htmlContainer, string html, CssActiveSheet cssData)
        {
            //-----------------------------------------------------------------
            var htmldoc = this.currentDoc =
                LayoutFarm.Composers.WebDocumentParser.ParseDocument(
                    new WebDom.Parser.TextSource(html.ToCharArray()));
            //build rootbox from htmldoc
            var rootBox = this.htmlhost.GetRenderTreeBuilder().BuildCssRenderTree(htmldoc,
                cssData,
                null);
            htmlContainer.WebDocument = htmldoc;
            htmlContainer.RootCssBox = rootBox;
        }

        public void LoadHtmlDom(LayoutFarm.WebDom.WebDocument doc, string defaultCss)
        {
            _baseRawCssData = defaultCss;
            _baseCssData = LayoutFarm.WebDom.Parser.CssParserHelper.ParseStyleSheet(defaultCss,
                CssDefaults.DefaultCssData,
                true);
            //-----------------  
            htmlContainer.WebDocument = (this.currentDoc = doc);
            BuildCssBoxTree(htmlContainer, _baseCssData);
            //---------------------
            PerformLayout();
            Invalidate();
        }

        void BuildCssBoxTree(MyHtmlContainer htmlCont, CssActiveSheet cssData)
        {
            var rootBox = this.htmlhost.GetRenderTreeBuilder().BuildCssRenderTree(
                this.currentDoc,
                cssData,
                null);
            htmlCont.RootCssBox = rootBox;
        }
        public void ForceRefreshHtmlDomChange(LayoutFarm.WebDom.WebDocument doc)
        {
            //RefreshHtmlDomChange(_baseCssData);
            myHtmlContainer_NeedUpdateDom(this, EventArgs.Empty);
            this.PaintMe();
        }
        public HtmlContainer GetHtmlContainer()
        {
            return this.htmlContainer;
        }


        /// <summary>
        /// Get html from the current DOM tree with inline style.
        /// </summary>
        /// <returns>generated html</returns>
        public string GetHtml()
        {
            if (htmlContainer == null)
            {
                return null;
            }
            else
            {
                System.Text.StringBuilder stbuilder = new System.Text.StringBuilder();
                htmlContainer.GetHtml(stbuilder);
                return stbuilder.ToString();
            }
        }


        ///// <summary>
        ///// Adjust the scrollbar of the panel on html element by the given id.<br/>
        ///// The top of the html element rectangle will be at the top of the panel, if there
        ///// is not enough height to scroll to the top the scroll will be at maximum.<br/>
        ///// </summary>
        ///// <param name="elementId">the id of the element to scroll to</param>
        //public void ScrollToElement(string elementId)
        //{
        //    if (_htmlContainer != null)
        //    {
        //        var rect = _htmlContainer.GetElementRectangle(elementId);
        //        if (rect.HasValue)
        //        {
        //            UpdateScroll(Point.Round(rect.Value.Location));
        //            _htmlContainer.HandleMouseMove(this, new MouseEventArgs(MouseButtons, 0, MousePosition.X, MousePosition.Y, 0));
        //        }
        //    }
        //}

        #region Private methods

        /// <summary>
        /// Perform the layout of the html in the control.
        /// </summary>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            PerformHtmlLayout();
            base.OnLayout(levent);
            // to handle if vertical scrollbar is appearing or disappearing
            if (htmlContainer != null && Math.Abs(htmlContainer.MaxWidth - ClientSize.Width) > 0.1)
            {
                PerformHtmlLayout();
                base.OnLayout(levent);
            }
        }

        /// <summary>
        /// Perform html container layout by the current panel client size.
        /// </summary>
        void PerformHtmlLayout()
        {
            if (htmlContainer != null)
            {
                //htmlContainer.MaxSize = new PixelFarm.Drawing.SizeF(ClientSize.Width, 0);
                htmlContainer.SetMaxSize(ClientSize.Width, 0);
                htmlContainer.PerformLayout(this.htmlLayoutVisitor);
                var asize = htmlContainer.ActualSize;
                AutoScrollMinSize = Size.Round(new SizeF(asize.Width, asize.Height));
            }
        }
        int count01;
        /// <summary>
        /// Perform paint of the html in the control.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            PaintMe(e);
        }
        void PaintMe()
        {
            PaintMe(null);
        }
        void PaintMe(PaintEventArgs e)
        {
            if (htmlContainer != null)
            {
                var bounds = this.Bounds;
                htmlContainer.CheckDocUpdate();
                var painter = GetSharedPainter(htmlContainer, renderCanvas);
                renderCanvas.Clear(PixelFarm.Drawing.Color.White);
                var scrollPos = AutoScrollPosition;
                painter.SetViewportSize(bounds.Width, bounds.Height);
                painter.OffsetCanvasOrigin(scrollPos.X, scrollPos.Y);
                htmlContainer.PerformPaint(painter);
                painter.OffsetCanvasOrigin(-scrollPos.X, -scrollPos.Y);
                ReleaseSharedPainter(painter);
                //------------------------------------------------------------

                //Win32 specific code
                IntPtr hdc = GetDC(this.Handle);
                renderCanvas.RenderTo(hdc, 0, 0, new PixelFarm.Drawing.Rectangle(0, 0, 800, 600));
                ReleaseDC(this.Handle, hdc);
                // call mouse move to handle paint after scroll or html change affecting mouse cursor.
                //var mp = PointToClient(MousePosition);
                //_htmlContainer.HandleMouseMove(this, new MouseEventArgs(MouseButtons.None, 0, mp.X, mp.Y, 0));
            }

            if (e != null)
            {
                e.Graphics.DrawString(count01.ToString(), this.Font, Brushes.Black, new PointF(0, 0));
            }
            count01++;
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hdc);
        /// <summary>
        /// Set focus on the control for keyboard scrrollbars handling.
        /// </summary>
        protected override void OnClick(EventArgs e)
        {
            this.isMouseDown = this.isDragging = false;
            base.OnClick(e);
            Focus();
        }


        /// <summary>
        /// Handle mouse leave to handle cursor change.
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            //if (_htmlContainer != null)
            //    _htmlContainer.HandleMouseLeave(this);
        }

        bool isMouseDown = false;
        bool isDragging = false;
        /// <summary>
        /// Handle mouse down to handle selection. 
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.isMouseDown = true;
            this.isDragging = false;
            base.OnMouseDown(e);
            this._htmlInputEventAdapter.MouseDown(CreateMouseEventArg(e));
            PaintMe(null);
            //this.Invalidate();
        }
        /// <summary>
        /// Handle mouse move to handle hover cursor and text selection. 
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.isDragging = this.isMouseDown;
            var mouseE = CreateMouseEventArg(e);
            _htmlInputEventAdapter.MouseMove(mouseE);
            PaintMe(null);
        }

        /// <summary>
        /// Handle mouse up to handle selection and link click. 
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            //get mouseE before reset isMouseDown and isDragging
            var mouseE = CreateMouseEventArg(e);
            this.isMouseDown = this.isDragging = false;
            this._htmlInputEventAdapter.MouseUp(mouseE);
            PaintMe(null);
            this.isMouseDown = this.isDragging = false;
            // this.Invalidate();
        }
        LayoutFarm.UI.UIMouseEventArgs CreateMouseEventArg(MouseEventArgs e)
        {
            var mouseE = new LayoutFarm.UI.UIMouseEventArgs();
            mouseE.SetEventInfo(
                e.X, e.Y,
                GetUIMouseButton(e.Button),
                e.Clicks,
                e.Delta);
            return mouseE;
        }

        static LayoutFarm.UI.UIMouseButtons GetUIMouseButton(MouseButtons mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButtons.Right:
                    return LayoutFarm.UI.UIMouseButtons.Right;
                case MouseButtons.Middle:
                    return LayoutFarm.UI.UIMouseButtons.Middle;
                case MouseButtons.None:
                    return LayoutFarm.UI.UIMouseButtons.None;
                default:
                    return LayoutFarm.UI.UIMouseButtons.Left;
            }
        }
        ///// <summary>
        ///// Handle mouse double click to select word under the mouse. 
        ///// </summary>
        //protected override void OnMouseDoubleClick(MouseEventArgs e)
        //{
        //    base.OnMouseDoubleClick(e);
        //    this._htmlEventBridge.MouseDoubleClick(CreateMouseEventArg(e));

        //    //if (_htmlContainer != null)
        //    //    _htmlContainer.HandleMouseDoubleClick(this, e);
        //}

        /// <summary>
        /// Handle key down event for selection, copy and scrollbars handling.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            //if (_htmlContainer != null)
            //    _htmlContainer.HandleKeyDown(this, e);
            //if (e.KeyCode == Keys.Up)
            //{
            //    VerticalScroll.Value = Math.Max(VerticalScroll.Value - 70, VerticalScroll.Minimum);
            //    PerformLayout();
            //}
            //else if (e.KeyCode == Keys.Down)
            //{
            //    VerticalScroll.Value = Math.Min(VerticalScroll.Value + 70, VerticalScroll.Maximum);
            //    PerformLayout();
            //}
            //else if (e.KeyCode == Keys.PageDown)
            //{
            //    VerticalScroll.Value = Math.Min(VerticalScroll.Value + 400, VerticalScroll.Maximum);
            //    PerformLayout();
            //}
            //else if (e.KeyCode == Keys.PageUp)
            //{
            //    VerticalScroll.Value = Math.Max(VerticalScroll.Value - 400, VerticalScroll.Minimum);
            //    PerformLayout();
            //}
            //else if (e.KeyCode == Keys.End)
            //{
            //    VerticalScroll.Value = VerticalScroll.Maximum;
            //    PerformLayout();
            //}
            //else if (e.KeyCode == Keys.Home)
            //{
            //    VerticalScroll.Value = VerticalScroll.Minimum;
            //    PerformLayout();
            //}
        }

        ///// <summary>
        ///// Propagate the LinkClicked event from root container.
        ///// </summary>
        //private void OnLinkClicked(object sender, HtmlLinkClickedEventArgs e)
        //{
        //    if (LinkClicked != null)
        //    {
        //        LinkClicked(this, e);
        //    }
        //}

        ///// <summary>
        ///// Propagate the Render Error event from root container.
        ///// </summary>
        //private void OnRenderError(object sender, HtmlRenderErrorEventArgs e)
        //{
        //    //if (RenderError != null)
        //    //{
        //    //    if (InvokeRequired)
        //    //        Invoke(RenderError, this, e);
        //    //    else
        //    //        RenderError(this, e);
        //    //}
        //}

        ///// <summary>
        ///// Propagate the stylesheet load event from root container.
        ///// </summary>
        //private void OnStylesheetLoad(object sender, TextLoadRequestEventArgs e)
        //{
        //    if (StylesheetLoad != null)
        //    {
        //        StylesheetLoad(this, e);
        //    }
        //}

        ///// <summary>
        ///// Propagate the image load event from root container.
        ///// </summary>
        //private void OnImageLoad(object sender, LayoutFarm.ContentManagers.ImageRequestEventArgs e)
        //{
        //    if (ImageLoad != null)
        //    {
        //        ImageLoad(this, e);
        //    }
        //}

        /// <summary>
        /// Handle html renderer invalidate and re-layout as requested.
        /// </summary>
        private void OnRefresh(object sender, EventArgs e)
        {
            MyHtmlContainer htmlCont = (MyHtmlContainer)sender;
            if (htmlCont.NeedLayout)
            {
                if (InvokeRequired)
                    Invoke(new MethodInvoker(PerformLayout));
                else
                    PerformLayout();
            }
            if (InvokeRequired)
                Invoke(new MethodInvoker(this.PaintMe));
            else
                this.PaintMe();
            //Invalidate();
        }

        /// <summary>
        /// On html renderer scroll request adjust the scrolling of the panel to the requested location.
        /// </summary>
        private void OnScrollChange(object sender, HtmlScrollEventArgs e)
        {
            UpdateScroll(e.Location);
        }

        /// <summary>
        /// Adjust the scrolling of the panel to the requested location.
        /// </summary>
        /// <param name="location">the location to adjust the scroll to</param>
        private void UpdateScroll(Point location)
        {
            AutoScrollPosition = location;
        }

        /// <summary>
        /// Used to add arrow keys to the handled keys in <see cref="OnKeyDown"/>.
        /// </summary>
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    return true;
                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        /// <summary>
        /// Release the html container resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (htmlContainer != null)
            {
                this.timer01.Stop();
                htmlContainer.DetachEssentialHandlers();
                htmlContainer.Dispose();
                htmlContainer = null;
            }
            base.Dispose(disposing);
        }

        #region Hide not relevant properties from designer

        /// <summary>
        /// Not applicable.
        /// </summary>
        [Browsable(false)]
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }

        /// <summary>
        /// Not applicable.
        /// </summary>
        [Browsable(false)]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }

        /// <summary>
        /// Not applicable.
        /// </summary>
        [Browsable(false)]
        public override bool AllowDrop
        {
            get { return base.AllowDrop; }
            set { base.AllowDrop = value; }
        }

        /// <summary>
        /// Not applicable.
        /// </summary>
        [Browsable(false)]
        public override RightToLeft RightToLeft
        {
            get { return base.RightToLeft; }
            set { base.RightToLeft = value; }
        }

        /// <summary>
        /// Not applicable.
        /// </summary>
        [Browsable(false)]
        public override Cursor Cursor
        {
            get { return base.Cursor; }
            set { base.Cursor = value; }
        }

        /// <summary>
        /// Not applicable.
        /// </summary>
        [Browsable(false)]
        public new bool UseWaitCursor
        {
            get { return base.UseWaitCursor; }
            set { base.UseWaitCursor = value; }
        }

        #endregion

        #endregion


        static PaintVisitor GetSharedPainter(LayoutFarm.HtmlBoxes.HtmlContainer htmlCont, PixelFarm.Drawing.DrawBoard canvas)
        {
            PaintVisitor painter = null;
            if (painterStock.Count == 0)
            {
                painter = new PaintVisitor();
            }
            else
            {
                painter = painterStock.Dequeue();
            }

            painter.Bind(htmlCont, canvas);
            return painter;
        }
        static void ReleaseSharedPainter(PaintVisitor p)
        {
            p.UnBind();
            painterStock.Enqueue(p);
        }
        static Queue<PaintVisitor> painterStock = new Queue<PaintVisitor>();
    }
}