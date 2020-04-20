//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

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
    /// <summary>
    /// CSS box for image element.
    /// </summary>
    public partial class CssBoxImage : CssBox
    {
        ImageBinder _tmpTransitionImgBinder;
        /// <summary>
        /// the image word of this image box
        /// </summary>
        readonly CssImageRun _imgRun;
        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="parent">the parent box of this box</param>
        /// <param name="controller">the html tag data of this box</param>
        public CssBoxImage(Css.BoxSpec boxSpec,
            IRootGraphics rootgfx, ImageBinder binder)
            : base(boxSpec, rootgfx)
        {
            _imgRun = new CssImageRun();
            _imgRun.ImageBinder = binder;
            binder.ImageChanged += Binder_ImageChanged;
            _imgRun.SetOwner(this);
            //
            var runlist = new List<CssRun>(1);
            runlist.Add(_imgRun);
            CssBox.UnsafeSetContentRuns(this, runlist, false);
        }

        private void Binder_ImageChanged(object sender, EventArgs e)
        {
            //TODO: ...

            float newW = this.Width.IsEmptyOrAuto ?
                            _imgRun.ImageBinder.Width :
                            LayoutFarm.WebDom.Parser.CssLengthExt.ConvertToPx(Width, VisualWidth, this);

            float newH = this.Height.IsEmptyOrAuto ?
                           _imgRun.ImageBinder.Height :
                           LayoutFarm.WebDom.Parser.CssLengthExt.ConvertToPx(Width, VisualWidth, this);


            SetVisualSize(newW, newH);
            //SetVisualSize(
            // _imgRun.ImageBinder.ImageWidth,
            // _imgRun.ImageBinder.ImageHeight); 
            //------------------------
            _imgRun.SetSize(
                 _imgRun.ImageBinder.Width,
                 _imgRun.ImageBinder.Height);
            //------------------------
            _imgRun.InvalidateGraphics();

        }

        public override void Clear()
        {
            base.Clear();
            var runlist = new List<CssRun>(1);
            runlist.Add(_imgRun);
            CssBox.UnsafeSetContentRuns(this, runlist, false);
        }

        public ImageBinder ImageBinder
        {
            get => _imgRun.ImageBinder;
            set
            {
                _imgRun.ImageBinder = value;
                this.RunSizeMeasurePass = false;
                if (value != null)
                {
                    switch (value.State)
                    {
                        case BinderState.Error:
                        case BinderState.Loaded:
                            break;
                        default:
                            //wait for img changed
                            value.ImageChanged += Binder_ImageChanged;
                            break;
                    }
                }
            }
        }
        public ImageBinder TempTranstionImageBinder
        {
            get => _tmpTransitionImgBinder;
            set => _tmpTransitionImgBinder = value;
        }

     
        /// <summary>
        /// Assigns words its width and height
        /// </summary>
        /// <param name="g">the device to use</param>
        public override void MeasureRunsSize(LayoutVisitor lay)
        {
            if (this.RunSizeMeasurePass)
            {
                return;
            }

            this.RunSizeMeasurePass = true;
            MeasureImageSize(_imgRun, lay);
        }

        /// <summary>
        /// Measure image box size by the width\height set on the box and the actual rendered image size.<br/>
        /// If no image exists for the box error icon will be set.
        /// </summary>
        /// <param name="imgRun">the image word to measure</param>
        static void MeasureImageSize(CssImageRun imgRun, LayoutVisitor lay)
        {
            var width = imgRun.OwnerBox.Width;
            var height = imgRun.OwnerBox.Height;
            bool hasImageTagWidth = width.Number > 0 && width.UnitOrNames == Css.CssUnitOrNames.Pixels;
            bool hasImageTagHeight = height.Number > 0 && height.UnitOrNames == Css.CssUnitOrNames.Pixels;
            bool scaleImageHeight = false;
            if (hasImageTagWidth)
            {
                imgRun.Width = width.Number;
            }
            else if (width.Number > 0 && width.IsPercentage)
            {
                imgRun.Width = width.Number * lay.LatestContainingBlock.VisualWidth;
                scaleImageHeight = true;
            }
            else if (imgRun.HasUserImageContent)
            {
                imgRun.Width = imgRun.ImageRectangle == Rectangle.Empty ? imgRun.OriginalImageWidth : imgRun.ImageRectangle.Width;
            }
            else
            {
                imgRun.Width = hasImageTagHeight ? height.Number / 1.14f : 20;
            }

            var maxWidth = imgRun.OwnerBox.MaxWidth;// new CssLength(imageWord.OwnerBox.MaxWidth);
            if (maxWidth.Number > 0)
            {
                float maxWidthVal = -1;
                switch (maxWidth.UnitOrNames)
                {
                    case Css.CssUnitOrNames.Percent:
                        {
                            maxWidthVal = maxWidth.Number * lay.LatestContainingBlock.VisualWidth;
                        }
                        break;
                    case Css.CssUnitOrNames.Pixels:
                        {
                            maxWidthVal = maxWidth.Number;
                        }
                        break;
                }


                if (maxWidthVal > -1 && imgRun.Width > maxWidthVal)
                {
                    imgRun.Width = maxWidthVal;
                    scaleImageHeight = !hasImageTagHeight;
                }
            }

            if (hasImageTagHeight)
            {
                imgRun.Height = height.Number;
            }
            else if (imgRun.HasUserImageContent)
            {
                imgRun.Height = imgRun.ImageRectangle == Rectangle.Empty ? imgRun.OriginalImageHeight : imgRun.ImageRectangle.Height;
            }
            else
            {
                imgRun.Height = imgRun.Width > 0 ? imgRun.Width * 1.14f : 22.8f;
            }

            if (imgRun.HasUserImageContent)
            {
                // If only the width was set in the html tag, ratio the height.
                if ((hasImageTagWidth && !hasImageTagHeight) || scaleImageHeight)
                {
                    // Divide the given tag width with the actual image width, to get the ratio.
                    float ratio = imgRun.Width / imgRun.OriginalImageWidth;
                    imgRun.Height = imgRun.OriginalImageHeight * ratio;
                }
                // If only the height was set in the html tag, ratio the width.
                else if (hasImageTagHeight && !hasImageTagWidth)
                {
                    // Divide the given tag height with the actual image height, to get the ratio.
                    float ratio = imgRun.Height / imgRun.OriginalImageHeight;
                    imgRun.Width = imgRun.OriginalImageWidth * ratio;
                }
            }
            //imageWord.Height += imageWord.OwnerBox.ActualBorderBottomWidth + imageWord.OwnerBox.ActualBorderTopWidth + imageWord.OwnerBox.ActualPaddingTop + imageWord.OwnerBox.ActualPaddingBottom;
        }
    }
}