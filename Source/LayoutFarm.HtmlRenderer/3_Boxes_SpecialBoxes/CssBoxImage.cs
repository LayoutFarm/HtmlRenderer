//BSD 2014,WinterDev
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
using LayoutFarm.Drawing;

namespace LayoutFarm.HtmlBoxes
{
    /// <summary>
    /// CSS box for image element.
    /// </summary>
    public sealed class CssBoxImage : CssBox
    {

        /// <summary>
        /// the image word of this image box
        /// </summary>
        readonly CssImageRun _imgRun;

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="parent">the parent box of this box</param>
        /// <param name="controller">the html tag data of this box</param>
        public CssBoxImage(object controller, Css.BoxSpec boxSpec, ImageBinder binder)
            : base(controller, boxSpec)
        {

            this._imgRun = new CssImageRun();
            this._imgRun.ImageBinder = binder;
            this._imgRun.SetOwner(this);


            var runlist = new List<CssRun>(1);

            runlist.Add(_imgRun);

            CssBox.UnsafeSetContentRuns(this, runlist, false);

        }

        ///// <summary>
        ///// Get the image of this image box.
        ///// </summary>
        //public Image Image
        //{
        //    get { return this._imgRun.ImageBinder.Image; }
        //}
        //void OnImageBinderLoadingComplete()
        //{
        //    //when binder state changed 
        //    switch (_imgBinder.State)
        //    {
        //        case ImageBinderState.Loaded:
        //            {
        //                //get image from binder
        //                //-----------------------
        //                var img = _imgBinder.Image;
        //                _imageWord.Image = img;
        //                _imageWord.ImageRectangle = new Rectangle(0, 0, img.Width, img.Height);
        //                _imageLoadingComplete = true;
        //                this.RunSizeMeasurePass = false;
        //                //-----------------------

        //                //if (_imageLoadingComplete && image == null)
        //                //{
        //                //    SetErrorBorder();
        //                //} 
        //                //if (!HtmlContainer.AvoidImagesLateLoading || async)
        //                //{
        //                //    var width = this.Width;//new CssLength(Width);
        //                //    var height = this.Height;// new CssLength(Height);
        //                //    var layout = (width.Number <= 0 || width.Unit != CssUnit.Pixels) || (height.Number <= 0 || height.Unit != CssUnit.Pixels);
        //                //    HtmlContainer.RequestRefresh(layout);
        //                //}

        //            } break;
        //    }
        //}
        internal void PaintImage(BoxPainter p, RectangleF rect)
        {

            PaintBackground(p, rect, true, true);

            if (this.HasSomeVisibleBorder)
            {
                p.PaintBorders(this, rect, true, true);
            }
            //--------------------------------------------------------- 
            RectangleF r = _imgRun.Rectangle;

            r.Height -= ActualBorderTopWidth + ActualBorderBottomWidth + ActualPaddingTop + ActualPaddingBottom;
            r.Y += ActualBorderTopWidth + ActualPaddingTop;
            r.X = (float)Math.Floor(r.X);
            r.Y = (float)Math.Floor(r.Y);

            bool tryLoadOnce = false;

        EVAL_STATE:
            switch (_imgRun.ImageBinder.State)
            {
                case ImageBinderState.Unload:
                    {
                        //async request image
                        if (!tryLoadOnce)
                        {

                            p.RequestImageAsync(_imgRun.ImageBinder, this._imgRun, this);
                            //retry again
                            tryLoadOnce = true;
                            goto EVAL_STATE;
                        }
                    } break;
                case ImageBinderState.Loading:
                    {
                        //RenderUtils.DrawImageLoadingIcon(g, r);
                    } break;
                case ImageBinderState.Loaded:
                    {

                        Bitmap img;
                        if ((img = (Bitmap)_imgRun.ImageBinder.Image) != null)
                        {

                            if (_imgRun.ImageRectangle == Rectangle.Empty)
                            {
                                p.DrawImage(img,
                                      r.Left, r.Top,
                                      img.Width, img.Height);
                                // g.DrawImage(img, Rectangle.Round(r));
                            }
                            else
                            {
                                //
                                p.DrawImage(img, _imgRun.ImageRectangle);
                                //g.DrawImage(_imageWord.Image, Rectangle.Round(r), _imageWord.ImageRectangle);
                            }
                        }
                        else
                        {
                            RenderUtils.DrawImageLoadingIcon(p.InnerCanvas, r);
                            if (r.Width > 19 && r.Height > 19)
                            {
                                p.DrawRectangle(Color.LightGray, r.X, r.Y, r.Width, r.Height);
                            }
                        }
                    } break;
                case ImageBinderState.NoImage:
                    {

                    } break;
                case ImageBinderState.Error:
                    {
                        RenderUtils.DrawImageErrorIcon(p.InnerCanvas, r);
                    } break;
            }

            //p.PopLocalClipArea();

        }
        /// <summary>
        /// Paints the fragment
        /// </summary>
        /// <param name="g">the device to draw to</param>
        protected override void PaintImp(BoxPainter p)
        {
            // load image iff it is in visible rectangle  
            //1. single image can't be splited  
            PaintImage(p, new RectangleF(0, 0, this.SizeWidth, this.SizeHeight));
        }

        /// <summary>
        /// Assigns words its width and height
        /// </summary>
        /// <param name="g">the device to use</param>
        internal override void MeasureRunsSize(LayoutVisitor lay)
        {
            if (!this.RunSizeMeasurePass)
            {
                this.RunSizeMeasurePass = true;
            }
            CssLayoutEngine.MeasureImageSize(_imgRun, lay);
        }
        #region Private methods




        #endregion
    }
}