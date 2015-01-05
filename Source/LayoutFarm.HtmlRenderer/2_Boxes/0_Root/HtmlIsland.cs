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


        public float MaxWidth { get { return this._maxHeight; } }
        public abstract void ClearPreviousSelection();
        public abstract void SetSelection(SelectionRange selRange);

#if DEBUG
        public static int dbugCount02 = 0;
#endif
        public CssBox RootCssBox
        {
            get { return this._rootBox; }
            set
            {
                if (_rootBox != null)
                {

                    _rootBox = null;
                    //---------------------------
                    this.OnRootDisposed();
                }
                _rootBox = value;
                if (value != null)
                {
                    this.OnRootCreated(_rootBox);
                }
            }
        }
        public bool HasRootBox { get { return this._rootBox != null; } }


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


        public void PerformLayout(LayoutVisitor layoutArgs)
        {

            if (this._rootBox == null)
            {
                return;
            }
            //----------------------- 
            //reset
            _actualWidth = _actualHeight = 0;
            // if width is not restricted we set it to large value to get the actual later    
            _rootBox.SetLocation(0, 0);
            _rootBox.SetSize(this._maxWidth > 0 ? this._maxWidth : MAX_WIDTH, 0);

            CssBox.ValidateComputeValues(_rootBox);
            //----------------------- 
            //LayoutVisitor layoutArgs = new LayoutVisitor(this.GraphicsPlatform, this);
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

        public void PerformPaint(Painter p)
        {
            if (_rootBox == null)
            {
                return;
            }
            p.PushContaingBlock(_rootBox);
            _rootBox.Paint(p);
            p.PopContainingBlock();
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

        internal void UpdateSizeIfWiderOrHigher(float newWidth, float newHeight)
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

        protected virtual void OnRootDisposed()
        {

        }
        protected virtual void OnRootCreated(CssBox root)
        {
        }
        protected virtual void OnAllDisposed()
        {
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void Dispose(bool all)
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


    }
}