//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{
    public class CustomImageRenderBox : CustomRenderBox
    {
#if DEBUG
        public bool dbugBreak;
#endif
        ImageBinder imageBinder;
        public CustomImageRenderBox(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.BackColor = Color.LightGray;
        }
        public override void ClearAllChildren()
        {
        }

        public ImageBinder ImageBinder
        {
            get { return this.imageBinder; }
            set { this.imageBinder = value; }
        }
        protected override void DrawBoxContent(Canvas canvas, Rectangle updateArea)
        {
            if (this.imageBinder != null)
            {
                switch (imageBinder.State)
                {
                    case ImageBinderState.Loaded:
                        {
                            canvas.DrawImage(imageBinder.Image,
                                new RectangleF(0, 0, this.Width, this.Height));
                        }
                        break;
                    case ImageBinderState.Unload:
                        {
                            if (this.imageBinder.HasLazyFunc)
                            {
                                this.imageBinder.LazyLoadImage();
                            }
                        }
                        break;
                }
            }
            else
            {
                //when no image
                //canvasPage.FillRectangle(BackColor, updateArea._left, updateArea._top, updateArea.Width, updateArea.Height);
            }
#if DEBUG
            //canvasPage.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //    new Rectangle(0, 0, this.Width, this.Height));
#endif
        }
    }
}