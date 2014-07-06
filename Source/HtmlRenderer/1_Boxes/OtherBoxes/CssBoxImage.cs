//BSD 2014,
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
using System.Drawing; 
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{
    /// <summary>
    /// CSS box for image element.
    /// </summary>
    sealed class CssBoxImage : CssBox
    {
        #region Fields and Consts

        /// <summary>
        /// the image word of this image box
        /// </summary>
        private readonly CssImageRun _imageWord;

        /////// <summary>
        /////// handler used for image loading by source
        /////// </summary>
        //private ImageLoadHandler _imageLoadHandler;

        ImageBinder _imgBinder;
        /// <summary>
        /// is image load is finished, used to know if no image is found
        /// </summary>
        private bool _imageLoadingComplete;

        #endregion

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="parent">the parent box of this box</param>
        /// <param name="tag">the html tag data of this box</param>
        public CssBoxImage(CssBox parent, BridgeHtmlElement tag, BoxSpec boxSpec)
            : base(parent, tag, boxSpec)
        {

            this._imageWord = new CssImageRun(); 
            this.SetTextContent(new RunCollection(_imageWord));
        }

        /// <summary>
        /// Get the image of this image box.
        /// </summary>
        public Image Image
        {
            get { return _imageWord.Image; }
        }
        void OnImageBinderLoadingComplete()
        {
            //when binder state changed 
            switch (_imgBinder.State)
            {
                case ImageBinderState.Loaded:
                    {
                        //get image from binder
                        //-----------------------
                        var img = _imgBinder.Image;
                        _imageWord.Image = img;
                        _imageWord.ImageRectangle = new Rectangle(0, 0, img.Width, img.Height);
                        _imageLoadingComplete = true;
                        this.RunSizeMeasurePass = false;
                        //-----------------------

                        //if (_imageLoadingComplete && image == null)
                        //{
                        //    SetErrorBorder();
                        //} 
                        //if (!HtmlContainer.AvoidImagesLateLoading || async)
                        //{
                        //    var width = this.Width;//new CssLength(Width);
                        //    var height = this.Height;// new CssLength(Height);
                        //    var layout = (width.Number <= 0 || width.Unit != CssUnit.Pixels) || (height.Number <= 0 || height.Unit != CssUnit.Pixels);
                        //    HtmlContainer.RequestRefresh(layout);
                        //}

                    } break;
            }
        }
        internal void PaintImage(IGraphics g, RectangleF rect, PaintVisitor p)
        {

            if (_imgBinder == null)
            {
                _imgBinder = new ImageBinder(GetAttribute("src"));
            }

            p.PushLocalClipArea(rect.Width, rect.Height);

            PaintBackground(p, rect, true, true);
            p.PaintBorders(this, rect, true, true);

            RectangleF r = _imageWord.Rectangle;
            r.Height -= ActualBorderTopWidth + ActualBorderBottomWidth + ActualPaddingTop + ActualPaddingBottom;
            r.Y += ActualBorderTopWidth + ActualPaddingTop;
            r.X = (float)Math.Floor(r.X);
            r.Y = (float)Math.Floor(r.Y);

            switch (_imgBinder.State)
            {
                case ImageBinderState.Unload:
                    {

                        p.RequestImage(_imgBinder, this, OnImageBinderLoadingComplete);

                        RenderUtils.DrawImageErrorIcon(g, r);
                    } break;
                case ImageBinderState.Loaded:
                    {

                        if (_imageWord.Image != null)
                        {
                            if (_imageWord.ImageRectangle == Rectangle.Empty)
                            {
                                g.DrawImage(_imageWord.Image, Rectangle.Round(r));
                            }
                            else
                            {
                                g.DrawImage(_imageWord.Image, Rectangle.Round(r), _imageWord.ImageRectangle);
                            }
                        }
                        else if (_imageLoadingComplete)
                        {
                            if (_imageLoadingComplete && r.Width > 19 && r.Height > 19)
                            {
                                RenderUtils.DrawImageErrorIcon(g, r);
                            }
                        }
                        else
                        {
                            RenderUtils.DrawImageLoadingIcon(g, r);
                            if (r.Width > 19 && r.Height > 19)
                            {
                                g.DrawRectangle(Pens.LightGray, r.X, r.Y, r.Width, r.Height);
                            }
                        }
                    } break;
                case ImageBinderState.NoImage:
                    {

                    } break;
                case ImageBinderState.Error:
                    {
                        RenderUtils.DrawImageErrorIcon(g, r);

                    } break;
            }

            p.PopLocalClipArea();

        }
        /// <summary>
        /// Paints the fragment
        /// </summary>
        /// <param name="g">the device to draw to</param>
        protected override void PaintImp(IGraphics g, PaintVisitor p)
        {
            // load image iff it is in visible rectangle  
            //1. single image can't be splited  
            PaintImage(g, new RectangleF(0, 0, this.SizeWidth, this.SizeHeight), p);
        }

        /// <summary>
        /// Assigns words its width and height
        /// </summary>
        /// <param name="g">the device to use</param>
        internal override void MeasureRunsSize(LayoutVisitor lay)
        {
            if (!this.RunSizeMeasurePass)
            {

                if (_imgBinder == null && lay.AvoidImageAsyncLoadOrLateBind)
                {
                    _imgBinder = new ImageBinder(GetAttribute("src"));
                    lay.RequestImage(_imgBinder, this, OnImageBinderLoadingComplete);
                }

                MeasureWordSpacing(lay);
                this.RunSizeMeasurePass = true;

            }

            CssLayoutEngine.MeasureImageSize(_imageWord);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            //if (_imageLoadHandler != null)
            //    _imageLoadHandler.Dispose();
            base.Dispose();
        }


        #region Private methods

        ///// <summary>
        ///// Set error image border on the image box.
        ///// </summary>
        //private void SetErrorBorder()
        //{

        //    this.SetAllBorders(
        //        CssBorderStyle.Solid, CssLength.MakePixelLength(2),
        //        System.Drawing.Color.FromArgb(0xA0, 0xA0, 0xA0));

        //    BorderRightColor = BorderBottomColor = System.Drawing.Color.FromArgb(0xE3, 0xE3, 0xE3);// "#E3E3E3";
        //}



        #endregion
    }
}