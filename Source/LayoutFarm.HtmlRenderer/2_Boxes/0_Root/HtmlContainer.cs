//BSD, 2014-2017, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

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
using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    public abstract class HtmlContainer : IDisposable
    {
        ITextService _textService;
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
        public abstract void CopySelection(System.Text.StringBuilder stbuilder);
        public HtmlContainer()
        {
            //TODO: review here again
            _textService = PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.GetIFonts();
        }
        public ITextService GetTextService()
        {
            return _textService;
        }
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
        public abstract bool RefreshDomIfNeed();

        public bool HasRootBox { get { return this._rootBox != null; } }

        /// <summary>
        /// The actual size of the rendered html (after layout)
        /// </summary>
        public SizeF ActualSize
        {
            get { return new SizeF(this._actualWidth, this._actualHeight); }
        }
        public float ActualWidth
        {
            get { return (int)this._actualWidth; }
        }
        public float ActualHeight
        {
            get { return (int)this._actualHeight; }
        }
        public void SetMaxSize(float maxWidth, float maxHeight)
        {
            this._maxWidth = maxWidth;
            this._maxHeight = maxHeight;
        }
        int layoutVersion;
        public int LayoutVersion
        {
            get
            {
                return this.layoutVersion;
            }
        }
        public void PerformLayout(LayoutVisitor lay)
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
            _rootBox.SetVisualSize(this._maxWidth > 0 ? this._maxWidth : MAX_WIDTH, 0);
            CssBox.ValidateComputeValues(_rootBox);
            //----------------------- 
            //LayoutVisitor layoutArgs = new LayoutVisitor(this.GraphicsPlatform, this);
            lay.PushContaingBlock(_rootBox);
            //----------------------- 

            _rootBox.PerformLayout(lay);
            if (this._maxWidth <= 0.1)
            {
                // in case the width is not restricted we need to double layout, first will find the width so second can layout by it (center alignment)
                _rootBox.SetVisualWidth((int)Math.Ceiling(this._actualWidth));
                _actualWidth = _actualHeight = 0;
                _rootBox.PerformLayout(lay);
            }
            lay.PopContainingBlock();
            //----------------------- 
            //TODO: review here again
            FloatingContextStack floatStack = lay.GetFloatingContextStack();
            List<FloatingContext> totalContexts = floatStack.GetTotalContexts();
            int j = totalContexts.Count;
            for (int i = 0; i < j; ++i)
            {
                FloatingContext floatingContext = totalContexts[i];
                int floatBoxCount = floatingContext.FloatBoxCount;
                if (floatBoxCount == 0) { continue; }


                CssBox floatingOwner = floatingContext.Owner;
                float rfx, rfy;
                floatingOwner.GetGlobalLocation(out rfx, out rfy);
                CssBox prevParent = null;
                //TODO: review here again
                float extraAdjustX = 0; //temp fixed
                for (int n = 0; n < floatBoxCount; ++n)
                {
                    float bfx, bfy;
                    CssBox box = floatingContext.GetBox(n);
                    box.GetGlobalLocation(out bfx, out bfy);
                    //diff
                    float nx = bfx - rfx;
                    float ny = bfy - rfy;
                    if (prevParent != null && prevParent != box.ParentBox)
                    {
                        if (n > 0)
                        {
                            CssBox prevFloatChild = floatingContext.GetBox(n - 1);
                            //TODO: review here again
                            //temp fix
                            extraAdjustX = prevFloatChild.ActualMarginRight + box.ActualMarginLeft;
                            ny += box.ActualMarginTop;
                        }
                    }
                    box.SetLocation(nx + extraAdjustX, ny);
                    prevParent = box.ParentBox;
                    floatingOwner.AppendToAbsoluteLayer(box);
                }
            }

            OnLayoutFinished();
            //----------------------- 
            unchecked { layoutVersion++; }
            //----------------------- 
        }
#if DEBUG
        void dbugAddToProperContainer(CssBox box)
        {
            var rectChild = new RectangleF(box.LocalX, box.LocalY,
                box.InnerContentWidth,
                box.InnerContentHeight);
            CssBox parent = box.ParentBox;
            bool found = false;
            while (parent != null)
            {
                var rectParent = new RectangleF(0, 0, parent.VisualWidth, parent.VisualHeight);
                if (rectParent.Contains(rectChild))
                {
                    found = true;
                    //add to here
                    float bfx, bfy;
                    box.GetGlobalLocation(out bfx, out bfy);
                    float rfx, rfy;
                    parent.GetGlobalLocation(out rfx, out rfy);
                    //diff
                    float nx = bfx - rfx;
                    float ny = bfy - rfy;
                    box.SetLocation(nx, ny);
                    parent.AppendToAbsoluteLayer(box);
                    break;
                }
                else
                {
                    rectChild.Offset(parent.LocalX, parent.LocalY);
                    parent = parent.ParentBox;
                }
            }
            if (!found)
            {
                //add to root top 
                float bfx, bfy;
                box.GetGlobalLocation(out bfx, out bfy);
                float rfx, rfy;
                this._rootBox.GetGlobalLocation(out rfx, out rfy);
                //diff
                float nx = bfx - rfx;
                float ny = bfy - rfy;
                box.SetLocation(nx, ny);
                this._rootBox.AppendToAbsoluteLayer(box);
            }
        }
#endif
        public bool IsInUpdateQueue { get; set; }
        protected virtual void OnLayoutFinished()
        {
        }
        public void PerformPaint(PaintVisitor p)
        {
            if (_rootBox == null)
            {
                return;
            }

            p.PushContaingBlock(_rootBox);
#if DEBUG
            p.dbugEnableLogRecord = false;
            p.dbugResetLogRecords();
#endif
            _rootBox.Paint(p);
            p.PopContainingBlock();
#if DEBUG
            if (p.dbugEnableLogRecord)
            {
                //var logs = p.logRecords;
                //System.Text.StringBuilder stbuilder = new System.Text.StringBuilder();
                //foreach (var str in logs)
                //{
                //    stbuilder.AppendLine(str);
                //}
                //System.IO.File.AppendAllText("drawLogs.txt", stbuilder.ToString());
            }
#endif

        }

        //------------------------------------------------------------------
        protected abstract void OnRequestImage(ImageBinder binder,
            object reqFrom, bool _sync);
        internal void RaiseImageRequest(
            ImageBinder binder,
            object reqBy,
            bool _sync)
        {
            //TODO: sync or async?
            OnRequestImage(binder, reqBy, false);
        }

        protected abstract void OnRequestScrollView(CssBox box);
        internal void RequestScrollView(CssBox box)
        {
            OnRequestScrollView(box);
        }
        public abstract void ContainerInvalidateGraphics();
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