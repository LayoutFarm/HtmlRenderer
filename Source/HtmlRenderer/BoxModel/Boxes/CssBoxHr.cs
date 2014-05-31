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

using System.Drawing;
using HtmlRenderer.Entities;
using HtmlRenderer.Handlers;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{
    /// <summary>
    /// CSS box for hr element.
    /// </summary>
    internal sealed class CssBoxHr : CssBox
    {
        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="parent">the parent box of this box</param>
        /// <param name="tag">the html tag data of this box</param>
        public CssBoxHr(CssBox parent, IHtmlTag tag)
            : base(parent, tag)
        {
            //Display = CssConstants.Block;
            this.CssDisplay = CssDisplay.Block;
        }

        /// <summary>
        /// Measures the bounds of box and children, recursively.<br/>
        /// Performs layout of the DOM structure creating lines by set bounds restrictions.
        /// </summary>
        /// <param name="g">Device context to use</param>
        protected override void PerformLayoutImp(IGraphics g)
        {
            //if (Display == CssConstants.None)
            if (this.CssDisplay == CssDisplay.None)
            {
                return;
            }

            ResetSummaryBound();

            var prevSibling = CssBox.GetPreviousSibling(this);
            this.LocationX = ContainingBlock.LocationX + ContainingBlock.ActualPaddingLeft + ActualMarginLeft + ContainingBlock.ActualBorderLeftWidth;
            float top = this.LocationY = (prevSibling == null && ParentBox != null ? ParentBox.ClientTop : ParentBox == null ? LocationY : 0) + MarginTopCollapse(prevSibling) + (prevSibling != null ? prevSibling.ActualBottom + prevSibling.ActualBorderBottomWidth : 0);

            //Location = new PointF(left, top);
            ActualBottom = top;

            //width at 100% (or auto)
            float minwidth = GetMinimumWidth();
            float width = ContainingBlock.Size.Width
                          - ContainingBlock.ActualPaddingLeft - ContainingBlock.ActualPaddingRight
                          - ContainingBlock.ActualBorderLeftWidth - ContainingBlock.ActualBorderRightWidth
                          - ActualMarginLeft - ActualMarginRight - ActualBorderLeftWidth - ActualBorderRightWidth;

            //Check width if not auto
            if (!this.Width.IsAuto && !this.Width.IsEmpty)
            {
                width = CssValueParser.ParseLength(Width, width, this);

            }
            //if (Width != CssConstants.Auto && !string.IsNullOrEmpty(Width))
            //{
            //    width = CssValueParser.ParseLength(Width, width, this);
            //}

            if (width < minwidth || width >= 9999)
            {
                width = minwidth;
            }

            float height = ActualHeight;
            if (height < 1)
            {
                height = Size.Height + ActualBorderTopWidth + ActualBorderBottomWidth;
            }
            if (height < 1)
            {
                height = 2;
            }
            if (height <= 2 && ActualBorderTopWidth < 1 && ActualBorderBottomWidth < 1)
            {
                BorderTopStyle = BorderBottomStyle = CssBorderStyle.Solid; //CssConstants.Solid;
                BorderTopWidth = CssLength.MakePixelLength(1); //"1px";
                BorderBottomWidth = CssLength.MakePixelLength(1);
            }

            Size = new SizeF(width, height);

            ActualBottom = this.LocationY + ActualPaddingTop + ActualPaddingBottom + height;
        }

        /// <summary>
        /// Paints the fragment
        /// </summary>
        /// <param name="g">the device to draw to</param>
        protected override void PaintImp(IGraphics g, PaintingArgs args)
        {
            var offset = HtmlContainer != null ? HtmlContainer.ScrollOffset : PointF.Empty;
            var rect = new RectangleF(Bounds.X + offset.X, Bounds.Y + offset.Y, Bounds.Width, Bounds.Height);

            if (rect.Height > 2 && RenderUtils.IsColorVisible(ActualBackgroundColor))
            {
                g.FillRectangle(RenderUtils.GetSolidBrush(ActualBackgroundColor), rect.X, rect.Y, rect.Width, rect.Height);
            }

            var b1 = RenderUtils.GetSolidBrush(ActualBorderTopColor);
            BordersDrawHandler.DrawBorder(Border.Top, g, this, b1, rect);

            if (rect.Height > 1)
            {
                var b2 = RenderUtils.GetSolidBrush(ActualBorderLeftColor);
                BordersDrawHandler.DrawBorder(Border.Left, g, this, b2, rect);

                var b3 = RenderUtils.GetSolidBrush(ActualBorderRightColor);
                BordersDrawHandler.DrawBorder(Border.Right, g, this, b3, rect);

                var b4 = RenderUtils.GetSolidBrush(ActualBorderBottomColor);
                BordersDrawHandler.DrawBorder(Border.Bottom, g, this, b4, rect);
            }
        }
    }
}