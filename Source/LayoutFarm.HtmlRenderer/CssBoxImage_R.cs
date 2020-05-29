//MIT, 2018-present, WinterDev
//BSD, 2014
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
using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    partial class CssBoxImage
    {
        void DrawWithTempTransitionImage(PaintVisitor p, RectangleF r)
        {

            Image img;
            if ((img = (Image)_tmpTransitionImgBinder.LocalImage) != null) //assign and test
            {
                if (this.VisualWidth != 0)
                {
                    //TODO: review here

                    if (_imgRun.ImageRectangle == Rectangle.Empty)
                    {
                        p.DrawImage(img,
                              r.Left, r.Top,
                              this.VisualWidth, this.VisualHeight);
                    }
                    else
                    {
                        p.DrawImage(img, _imgRun.ImageRectangle);
                    }
                }
                else
                {
                    if (_imgRun.ImageRectangle == Rectangle.Empty)
                    {
                        p.DrawImage(img,
                              r.Left, r.Top,
                              img.Width, img.Height);
                    }
                    else
                    {
                        p.DrawImage(img, _imgRun.ImageRectangle);
                    }
                }
            }

        }
        public override void Paint(PaintVisitor p, RectangleF rect)
        {
            Color bgColorHint = p.CurrentSolidBackgroundColorHint;//save

            PaintBackground(p, rect, true, true);

            if (this.HasSomeVisibleBorder)
            {
                p.PaintBorders(this, rect, true, true);
            }
            //--------------------------------------------------------- 
            RectangleF r = _imgRun.Rectangle;
            r.Height -= ActualBorderTopWidth + ActualBorderBottomWidth + ActualPaddingTop + ActualPaddingBottom;
            r.Location = new PointF((float)Math.Floor(r.Left), (float)Math.Floor(r.Top + ActualBorderTopWidth + ActualPaddingTop));

            bool tryLoadOnce = false;
        EVAL_STATE:
            switch (_imgRun.ImageBinder.State)
            {
                case BinderState.Unload:
                    {
                        if (_tmpTransitionImgBinder != null)
                        {
                            DrawWithTempTransitionImage(p, rect);
                        }


                        //async request image
                        if (!tryLoadOnce)
                        {
                            p.RequestImageAsync(_imgRun.ImageBinder, _imgRun, this);
                            //retry again
                            tryLoadOnce = true;
                            goto EVAL_STATE;
                        }
                    }
                    break;
                case BinderState.Loading:
                    {
                        if (_tmpTransitionImgBinder != null)
                        {
                            DrawWithTempTransitionImage(p, rect);
                        }

                        //RenderUtils.DrawImageLoadingIcon(g, r);
                    }
                    break;
                case BinderState.Loaded:
                    {
                        if (_tmpTransitionImgBinder != null)
                        {
                            //*** clear tmp transition img after new image is loaded
                            _tmpTransitionImgBinder = null;
                        }
                        Image img;
                        if ((img = (Image)_imgRun.ImageBinder.LocalImage) != null) //assign and test
                        {
                            if (this.VisualWidth != 0)
                            {
                                //TODO: review here

                                if (_imgRun.ImageRectangle == Rectangle.Empty)
                                {
                                    p.DrawImage(img,
                                          r.Left, r.Top,
                                          this.VisualWidth, this.VisualHeight);
                                }
                                else
                                {
                                    p.DrawImage(img, _imgRun.ImageRectangle);
                                }
                            }
                            else
                            {
                                if (_imgRun.ImageRectangle == Rectangle.Empty)
                                {
                                    p.DrawImage(img,
                                          r.Left, r.Top,
                                          img.Width, img.Height);
                                }
                                else
                                {
                                    p.DrawImage(img, _imgRun.ImageRectangle);
                                }
                            }

                        }
                        else
                        {
                            RenderUtils.DrawImageLoadingIcon(p.InnerDrawBoard, r);
                            if (r.Width > 19 && r.Height > 19)
                            {
                                p.DrawRectangle(KnownColors.LightGray, r.Left, r.Top, r.Width, r.Height);
                            }
                        }
                    }
                    break;
                case BinderState.Blank:
                    {
                    }
                    break;
                case BinderState.Error:
                    {
                        RenderUtils.DrawImageErrorIcon(p.InnerDrawBoard, r);
                    }
                    break;
            }

            //#if DEBUG
            //            p.FillRectangle(Color.Red, rect.X, rect.Y, rect.Width, rect.Height);
            //#endif
             
            p.CurrentSolidBackgroundColorHint = bgColorHint;//restore
        }

        /// <summary>
        /// Paints the fragment
        /// </summary>
        /// <param name="g">the device to draw to</param>
        protected override void PaintImp(PaintVisitor p)
        {
            // load image if it is in visible rectangle  
            //1. single image can't be splited  
#if DEBUG
            p.dbugEnterNewContext(this, PaintVisitor.PaintVisitorContextName.Init);
#endif
            Paint(p, new RectangleF(0, 0, this.VisualWidth, this.VisualHeight));
#if DEBUG
            p.dbugExitContext();
#endif
        }

    }
}