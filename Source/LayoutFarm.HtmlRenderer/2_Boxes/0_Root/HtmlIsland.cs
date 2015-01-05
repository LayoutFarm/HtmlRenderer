//BSD 2014, WinterDev
//ArthurHub

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
using System.Collections.Generic;
using LayoutFarm.Drawing;
using HtmlRenderer.Boxes;
using HtmlRenderer.Diagnostics;
using HtmlRenderer.Css;

namespace HtmlRenderer.Boxes
{
    /// <summary>
    /// layout and render the html fragment
    /// </summary>
    public abstract class HtmlIsland : IDisposable
    {
        /// <summary>
        /// the root css box of the parsed html
        /// </summary>
        CssBox _rootBox;
        /// <summary>
        /// The actual size of the rendered html (after layout)
        /// </summary>
        float _actualWidth;
        float _actualHeight;

        float _maxWidth;
        float _maxHeight;
        /// <summary>
        /// 99999
        /// </summary>
        const int MAX_WIDTH = 99999;

        
        SelectionRange _currentSelectionRange;
        GraphicsPlatform gfxPlatform;

        public HtmlIsland(GraphicsPlatform gfxPlatforms)
        {
            this.gfxPlatform = gfxPlatforms;
        }
        public float MaxWidth { get { return this._maxHeight; } }
        protected GraphicsPlatform CurrentGfxPlatform
        {
            get { return this.gfxPlatform; }
        }
        public void ClearPreviousSelection()
        {
            if (_currentSelectionRange != null)
            {
                _currentSelectionRange.ClearSelectionStatus();
                _currentSelectionRange = null;
            }
        }
        public void SetSelection(SelectionRange selRange)
        {
            _currentSelectionRange = selRange;
        }
#if DEBUG
        public static int dbugCount02 = 0;
#endif


        public abstract void AddRequestImageBinderUpdate(ImageBinder binder);


        /// <summary>
        /// Gets or sets a value indicating if anti-aliasing should be avoided for geometry like backgrounds and borders (default - false).
        /// </summary>
        public bool AvoidGeometryAntialias
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating if image asynchronous loading should be avoided (default - false).<br/>
        /// True - images are loaded synchronously during html parsing.<br/>
        /// False - images are loaded asynchronously to html parsing when downloaded from URL or loaded from disk.<br/>
        /// </summary>
        /// <remarks>
        /// Asynchronously image loading allows to unblock html rendering while image is downloaded or loaded from disk using IO 
        /// ports to achieve better performance.<br/>
        /// Asynchronously image loading should be avoided when the full html content must be available during render, like render to image.
        /// </remarks>
        public bool AvoidAsyncImagesLoading
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating if image loading only when visible should be avoided (default - false).<br/>
        /// True - images are loaded as soon as the html is parsed.<br/>
        /// False - images that are not visible because of scroll location are not loaded until they are scrolled to.
        /// </summary>
        /// <remarks>
        /// Images late loading improve performance if the page contains image outside the visible scroll area, especially if there is large 
        /// amount of images, as all image loading is delayed (downloading and loading into memory).<br/>
        /// Late image loading may effect the layout and actual size as image without set size will not have actual size until they are loaded
        /// resulting in layout change during user scroll.<br/>
        /// Early image loading may also effect the layout if image without known size above the current scroll location are loaded as they
        /// will push the html elements down.
        /// </remarks>
        public bool AvoidImagesLateLoading
        {
            get;
            set;
        }

        ///// <summary>
        ///// Use GDI+ text rendering to measure/draw text.<br/>
        ///// </summary>
        ///// <remarks>
        ///// <para>
        ///// GDI+ text rendering is less smooth than GDI text rendering but it natively supports alpha channel
        ///// thus allows creating transparent images.
        ///// </para>
        ///// <para>
        ///// While using GDI+ text rendering you can control the text rendering using <see cref="Graphics.TextRenderingHint"/>, note that
        ///// using <see cref="TextRenderingHint.ClearTypeGridFit"/> doesn't work well with transparent background.
        ///// </para>
        ///// </remarks>
        //public bool UseGdiPlusTextRendering
        //{
        //    get { return _useGdiPlusTextRendering; }
        //    set
        //    {
        //        if (_useGdiPlusTextRendering != value)
        //        {
        //            _useGdiPlusTextRendering = value;
        //            RequestRefresh(true);
        //        }
        //    }
        //}


        /// <summary>
        /// The scroll offset of the html.<br/>
        /// This will adjust the rendered html by the given offset so the content will be "scrolled".<br/>
        /// </summary>
        /// <example>
        /// Element that is rendered at location (50,100) with offset of (0,200) will not be rendered as it
        /// will be at -100 therefore outside the client rectangle.
        /// </example>
        public PointF ScrollOffset
        {
            get;
            set;

        }

        public void SetMaxSize(float maxWidth, float maxHeight)
        {
            this._maxWidth = maxWidth;
            this._maxHeight = maxHeight;
        }
        /// <summary>
        /// The actual size of the rendered html (after layout)
        /// </summary>
        public SizeF ActualSize
        {
            get { return new SizeF(this._actualWidth, this._actualHeight); }
        }
        internal void UpdateSizeIfWiderOrHeigher(float newWidth, float newHeight)
        {
            if (newWidth > this._actualWidth)
            {
                this._actualWidth = newWidth;
            }
            if (newHeight > this._actualHeight)
            {
                this._actualHeight = newHeight;
            }
        }
        public CssBox GetRootCssBox()
        {
            return this._rootBox;
        }
        public void SetRootCssBox(CssBox rootBox)
        {
            if (_rootBox != null)
            {
                _rootBox = null;
                //---------------------------
                this.OnRootDisposed();
            }
            _rootBox = rootBox;
            if (rootBox != null)
            {
                this.OnRootCreated(_rootBox);
            }
        }

        protected virtual void OnRootDisposed()
        {

        }
        protected virtual void OnRootCreated(CssBox root)
        {
        }
        protected virtual void OnAllDisposed()
        {
        }


        public void PerformLayout()
        {

            if (this._rootBox == null)
            {
                return;
            }
            //----------------------- 
            _actualWidth = _actualHeight = 0;
            // if width is not restricted we set it to large value to get the actual later    
            _rootBox.SetLocation(0, 0);
            _rootBox.SetSize(this._maxWidth > 0 ? this._maxWidth : MAX_WIDTH, 0);

            CssBox.ValidateComputeValues(_rootBox);
            //----------------------- 
            LayoutVisitor layoutArgs = new LayoutVisitor(this.CurrentGfxPlatform, this);
            layoutArgs.PushContaingBlock(_rootBox);
            //----------------------- 
            _rootBox.PerformLayout(layoutArgs);
            if (this._maxWidth <= 0.1)
            {
                // in case the width is not restricted we need to double layout, first will find the width so second can layout by it (center alignment)

                _rootBox.SetWidth((int)Math.Ceiling(this._actualWidth));
                _actualWidth = _actualHeight = 0;
                _rootBox.PerformLayout(layoutArgs);
            }
            layoutArgs.PopContainingBlock();

        }
        public RectangleF PhysicalViewportBound
        {
            get;
            set;
        }

        /// <summary>
        /// Render the html using the given device.
        /// </summary>
        /// <param name="g"></param>
        protected void PerformPaint(Canvas canvas)
        {
            if (_rootBox == null)
            {
                return;
            }

            Painter p = new Painter(this, canvas);


            var physicalViewportSize = this.PhysicalViewportBound.Size;

            int ox = canvas.CanvasOriginX;
            int oy = canvas.CanvasOriginY;

            int scX = (int)this.ScrollOffset.X;
            int scY = (int)this.ScrollOffset.Y;


            canvas.SetCanvasOrigin(ox + scX, oy + scY);

            p.PushContaingBlock(_rootBox);
            p.SetPhysicalViewportBound(0, oy + scY, physicalViewportSize.Width, physicalViewportSize.Height);


            _rootBox.Paint(p);


            p.PopContainingBlock();

            canvas.SetCanvasOrigin(ox, oy);
        }



        //------------------------------------------------------------------
        protected abstract void OnRequestImage(ImageBinder binder,
            object reqFrom, bool _sync);

        internal static void RaiseRequestImage(HtmlIsland htmlIsland,
            ImageBinder binder,
            object reqBy,
            bool _sync)
        {

            htmlIsland.OnRequestImage(binder, reqBy, false);
        }
        //------------------------------------------------------------------ 
        protected abstract void RequestRefresh(bool layout);
        /// <summary>
        /// Report error in html render process.
        /// </summary>
        /// <param name="type">the type of error to report</param>
        /// <param name="message">the error message</param>
        /// <param name="exception">optional: the exception that occured</param>
        public void ReportError(HtmlRenderErrorType type, string message, Exception exception = null)
        {
            //try
            //{
            //    if (RenderError != null)
            //    {
            //        RenderError(this, new HtmlRenderErrorEventArgs(type, message, exception));
            //    }
            //}
            //catch
            //{
            //}
        }



        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
        }


        #region Private methods

        /// <summary>
        /// Adjust the offset of the given location by the current scroll offset.
        /// </summary>
        /// <param name="location">the location to adjust</param>
        /// <returns>the adjusted location</returns>
        protected Point OffsetByScroll(Point location)
        {
            location.Offset(-(int)ScrollOffset.X, -(int)ScrollOffset.Y);
            return location;
        }

        ///// <summary>
        ///// Check if the mouse is currently on the html container.<br/>
        ///// Relevant if the html container is not filled in the hosted control (location is not zero and the size is not the full size of the control).
        ///// </summary>
        //protected bool IsMouseInContainer(Point location)
        //{
        //    return location.X >= _location.X &&
        //        location.X <= _location.X + _actualWidth &&
        //        location.Y >= _location.Y + ScrollOffset.Y &&
        //        location.Y <= _location.Y + ScrollOffset.Y + _actualHeight;
        //}


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        private void Dispose(bool all)
        {
            try
            {

                if (all)
                {
                    this.OnAllDisposed();

                    //RenderError = null;
                    //StylesheetLoadingRequest = null;
                    //ImageLoadingRequest = null;
                }


                if (_rootBox != null)
                {

                    _rootBox = null;
                    this.OnRootDisposed();
                }


                //if (_selectionHandler != null)
                //    _selectionHandler.Dispose();
                //_selectionHandler = null;
            }
            catch
            { }
        }

        #endregion
    }
}