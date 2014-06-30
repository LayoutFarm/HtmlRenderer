//BSD 2014, WinterCore


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
using System.Drawing.Drawing2D;
using HtmlRenderer.Dom;
using HtmlRenderer.Entities;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Handlers
{


    /// <summary>
    /// Contains all the complex paint code to paint different style borders.
    /// </summary>
    static class BordersDrawHandler
    {

        /// <summary>
        /// Draws all the border of the box with respect to style, width, etc.
        /// </summary>
        /// <param name="g">the device to draw into</param>
        /// <param name="box">the box to draw borders for</param>
        /// <param name="rect">the bounding rectangle to draw in</param>
        /// <param name="isFirst">is it the first rectangle of the element</param>
        /// <param name="isLast">is it the last rectangle of the element</param>
        public static void DrawBoxBorders(PaintVisitor p, CssBox box, RectangleF rect, bool isFirst, bool isLast)
        {
            if (rect.Width > 0 && rect.Height > 0)
            {
<<<<<<< HEAD

=======
>>>>>>> FETCH_HEAD
                var dd = box.dbugId;
                if (box.BorderTopStyle >= CssBorderStyle.Visible &&
                    box.ActualBorderTopWidth > 0)
                {
                    DrawBorder(Border.Top, box, p, rect, isFirst, isLast);
                }


                if (isFirst &&
                    box.BorderLeftStyle >= CssBorderStyle.Visible &&
                    box.ActualBorderLeftWidth > 0)
                {
                    DrawBorder(Border.Left, box, p, rect, true, isLast);
                }

                if (box.BorderBottomStyle >= CssBorderStyle.Visible &&
                    box.ActualBorderBottomWidth > 0)
                {
                    DrawBorder(Border.Bottom, box, p, rect, isFirst, isLast);
                }

                if (isLast &&
                    box.BorderRightStyle >= CssBorderStyle.Visible &&
                    box.ActualBorderRightWidth > 0)
                {
                    DrawBorder(Border.Right, box, p, rect, isFirst, true);
                }
            }
        }

        /// <summary>
        /// Draw simple border.
        /// </summary>
        /// <param name="border">Desired border</param>
        /// <param name="g">the device to draw to</param>
        /// <param name="box">Box which the border corresponds</param>
        /// <param name="brush">the brush to use</param>
        /// <param name="rectangle">the bounding rectangle to draw in</param>
        /// <returns>Beveled border path, null if there is no rounded corners</returns>
        public static void DrawBorder(Border border, IGraphics g, CssBox box, Brush brush, RectangleF rectangle)
        {
            PointF[] borderPts = new PointF[4];
            SetInOutsetRectanglePoints(border, box, rectangle, true, true, borderPts);
            g.FillPolygon(brush, borderPts);
        }

        #region Private methods

        /// <summary>
        /// Draw specific border (top/bottom/left/right) with the box data (style/width/rounded).<br/>
        /// </summary>
        /// <param name="border">desired border to draw</param>
        /// <param name="box">the box to draw its borders, contain the borders data</param>
        /// <param name="g">the device to draw into</param>
        /// <param name="rect">the rectangle the border is enclosing</param>
        /// <param name="isLineStart">Specifies if the border is for a starting line (no bevel on left)</param>
        /// <param name="isLineEnd">Specifies if the border is for an ending line (no bevel on right)</param>
        static void DrawBorder(Border border, CssBox box, PaintVisitor p, RectangleF rect, bool isLineStart, bool isLineEnd)
        {

            CssBorderStyle style = GetStyle(border, box);
            var color = GetColor(border, box, style);

            var borderPath = GetRoundedBorderPath(border, box, rect);

            //if (box.CssDisplay == CssDisplay.Table)
            //{

            //}

            //if (box.CssDisplay == CssDisplay.Table)
            //{
            //}
            //else if (box.CssDisplay == CssDisplay.TableCell)
            //{
            //}
            

            IGraphics g = p.Gfx;
            if (borderPath != null)
            {
                // rounded border need special path

                var smooth = g.SmoothingMode;
                if (!p.AvoidGeometryAntialias && box.HasRoundCorner)
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                }

                var pen = GetPen(style, color, GetWidth(border, box));
                using (borderPath)
                {
                    g.DrawPath(pen, borderPath);
                }

                g.SmoothingMode = smooth;
            }
            else
            {
                // non rounded border

                if (style == CssBorderStyle.Inset || style == CssBorderStyle.Outset)
                {
                    // inset/outset border needs special rectangle
                    PointF[] borderPnts = new PointF[4];
                    SetInOutsetRectanglePoints(border, box, rect, isLineStart, isLineEnd, borderPnts);
                    g.FillPolygon(RenderUtils.GetSolidBrush(color), borderPnts);
                }
                else
                {
                    // solid/dotted/dashed border draw as simple line
                    var pen = GetPen(style, color, GetWidth(border, box));
                    switch (border)
                    {
                        case Border.Top:
                            g.DrawLine(pen, (float)Math.Ceiling(rect.Left), rect.Top + box.ActualBorderTopWidth / 2, rect.Right - 1, rect.Top + box.ActualBorderTopWidth / 2);
                            break;
                        case Border.Left:
                            g.DrawLine(pen, rect.Left + box.ActualBorderLeftWidth / 2, (float)Math.Ceiling(rect.Top), rect.Left + box.ActualBorderLeftWidth / 2, (float)Math.Floor(rect.Bottom));
                            break;
                        case Border.Bottom:
                            g.DrawLine(pen, (float)Math.Ceiling(rect.Left), rect.Bottom - box.ActualBorderBottomWidth / 2, rect.Right - 1, rect.Bottom - box.ActualBorderBottomWidth / 2);
                            break;
                        case Border.Right:
                            g.DrawLine(pen, rect.Right - box.ActualBorderRightWidth / 2, (float)Math.Ceiling(rect.Top), rect.Right - box.ActualBorderRightWidth / 2, (float)Math.Floor(rect.Bottom));
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Set rectangle for inset/outset border as it need diagonal connection to other borders.
        /// </summary>
        /// <param name="border">Desired border</param>
        /// <param name="b">Box which the border corresponds</param>
        /// <param name="r">the rectangle the border is enclosing</param>
        /// <param name="isLineStart">Specifies if the border is for a starting line (no bevel on left)</param>
        /// <param name="isLineEnd">Specifies if the border is for an ending line (no bevel on right)</param>
        /// <returns>Beveled border path, null if there is no rounded corners</returns>
        static void SetInOutsetRectanglePoints(Border border, CssBox b, RectangleF r, bool isLineStart, bool isLineEnd,
            PointF[] _borderPts)
        {
            switch (border)
            {
                case Border.Top:
                    _borderPts[0] = new PointF(r.Left, r.Top);
                    _borderPts[1] = new PointF(r.Right, r.Top);
                    _borderPts[2] = new PointF(r.Right, r.Top + b.ActualBorderTopWidth);
                    _borderPts[3] = new PointF(r.Left, r.Top + b.ActualBorderTopWidth);
                    if (isLineEnd)
                        _borderPts[2].X -= b.ActualBorderRightWidth;
                    if (isLineStart)
                        _borderPts[3].X += b.ActualBorderLeftWidth;
                    break;
                case Border.Right:
                    _borderPts[0] = new PointF(r.Right - b.ActualBorderRightWidth, r.Top + b.ActualBorderTopWidth);
                    _borderPts[1] = new PointF(r.Right, r.Top);
                    _borderPts[2] = new PointF(r.Right, r.Bottom);
                    _borderPts[3] = new PointF(r.Right - b.ActualBorderRightWidth, r.Bottom - b.ActualBorderBottomWidth);
                    break;
                case Border.Bottom:
                    _borderPts[0] = new PointF(r.Left, r.Bottom - b.ActualBorderBottomWidth);
                    _borderPts[1] = new PointF(r.Right, r.Bottom - b.ActualBorderBottomWidth);
                    _borderPts[2] = new PointF(r.Right, r.Bottom);
                    _borderPts[3] = new PointF(r.Left, r.Bottom);
                    if (isLineStart)
                        _borderPts[0].X += b.ActualBorderLeftWidth;
                    if (isLineEnd)
                        _borderPts[1].X -= b.ActualBorderRightWidth;
                    break;
                case Border.Left:
                    _borderPts[0] = new PointF(r.Left, r.Top);
                    _borderPts[1] = new PointF(r.Left + b.ActualBorderLeftWidth, r.Top + b.ActualBorderTopWidth);
                    _borderPts[2] = new PointF(r.Left + b.ActualBorderLeftWidth, r.Bottom - b.ActualBorderBottomWidth);
                    _borderPts[3] = new PointF(r.Left, r.Bottom);
                    break;
            }
        }

        /// <summary>
        /// Makes a border path for rounded borders.<br/>
        /// To support rounded dotted/dashed borders we need to use arc in the border path.<br/>
        /// Return null if the border is not rounded.<br/>
        /// </summary>
        /// <param name="border">Desired border</param>
        /// <param name="b">Box which the border corresponds</param>
        /// <param name="r">the rectangle the border is enclosing</param>
        /// <returns>Beveled border path, null if there is no rounded corners</returns>
        static GraphicsPath GetRoundedBorderPath(Border border, CssBox b, RectangleF r)
        {
            GraphicsPath path = null;

            switch (border)
            {
                case Border.Top:
                    if (b.ActualCornerNW > 0 || b.ActualCornerNE > 0)
                    {
                        path = new GraphicsPath();

                        if (b.ActualCornerNW > 0)
                            path.AddArc(r.Left + b.ActualBorderLeftWidth / 2, r.Top + b.ActualBorderTopWidth / 2, b.ActualCornerNW * 2, b.ActualCornerNW * 2, 180f, 90f);
                        else
                            path.AddLine(r.Left + b.ActualBorderLeftWidth / 2, r.Top + b.ActualBorderTopWidth / 2, r.Left + b.ActualBorderLeftWidth, r.Top + b.ActualBorderTopWidth / 2);

                        if (b.ActualCornerNE > 0)
                            path.AddArc(r.Right - b.ActualCornerNE * 2 - b.ActualBorderRightWidth / 2, r.Top + b.ActualBorderTopWidth / 2, b.ActualCornerNE * 2, b.ActualCornerNE * 2, 270f, 90f);
                        else
                            path.AddLine(r.Right - b.ActualCornerNE * 2 - b.ActualBorderRightWidth, r.Top + b.ActualBorderTopWidth / 2, r.Right - b.ActualBorderRightWidth / 2, r.Top + b.ActualBorderTopWidth / 2);
                    }
                    break;
                case Border.Bottom:
                    if (b.ActualCornerSW > 0 || b.ActualCornerSE > 0)
                    {
                        path = new GraphicsPath();

                        if (b.ActualCornerSE > 0)
                            path.AddArc(r.Right - b.ActualCornerNE * 2 - b.ActualBorderRightWidth / 2, r.Bottom - b.ActualCornerSE * 2 - b.ActualBorderBottomWidth / 2, b.ActualCornerSE * 2, b.ActualCornerSE * 2, 0f, 90f);
                        else
                            path.AddLine(r.Right - b.ActualBorderRightWidth / 2, r.Bottom - b.ActualBorderBottomWidth / 2, r.Right - b.ActualBorderRightWidth / 2, r.Bottom - b.ActualBorderBottomWidth / 2 - .1f);

                        if (b.ActualCornerSW > 0)
                            path.AddArc(r.Left + b.ActualBorderLeftWidth / 2, r.Bottom - b.ActualCornerSW * 2 - b.ActualBorderBottomWidth / 2, b.ActualCornerSW * 2, b.ActualCornerSW * 2, 90f, 90f);
                        else
                            path.AddLine(r.Left + b.ActualBorderLeftWidth / 2 + .1f, r.Bottom - b.ActualBorderBottomWidth / 2, r.Left + b.ActualBorderLeftWidth / 2, r.Bottom - b.ActualBorderBottomWidth / 2);
                    }
                    break;
                case Border.Right:
                    if (b.ActualCornerNE > 0 || b.ActualCornerSE > 0)
                    {
                        path = new GraphicsPath();

                        if (b.ActualCornerNE > 0 && b.BorderTopStyle >= CssBorderStyle.Visible)
                        //(b.BorderTopStyle == CssConstants.None || b.BorderTopStyle == CssConstants.Hidden))
                        {
                            path.AddArc(r.Right - b.ActualCornerNE * 2 - b.ActualBorderRightWidth / 2, r.Top + b.ActualBorderTopWidth / 2, b.ActualCornerNE * 2, b.ActualCornerNE * 2, 270f, 90f);
                        }
                        else
                        {
                            path.AddLine(r.Right - b.ActualBorderRightWidth / 2, r.Top + b.ActualCornerNE + b.ActualBorderTopWidth / 2, r.Right - b.ActualBorderRightWidth / 2, r.Top + b.ActualCornerNE + b.ActualBorderTopWidth / 2 + .1f);
                        }

                        if (b.ActualCornerSE > 0 &&
                            b.BorderBottomStyle >= CssBorderStyle.Visible)
                        //(b.BorderBottomStyle == CssConstants.None || b.BorderBottomStyle == CssConstants.Hidden))
                        {
                            path.AddArc(r.Right - b.ActualCornerSE * 2 - b.ActualBorderRightWidth / 2, r.Bottom - b.ActualCornerSE * 2 - b.ActualBorderBottomWidth / 2, b.ActualCornerSE * 2, b.ActualCornerSE * 2, 0f, 90f);
                        }
                        else
                        {
                            path.AddLine(r.Right - b.ActualBorderRightWidth / 2, r.Bottom - b.ActualCornerSE - b.ActualBorderBottomWidth / 2 - .1f, r.Right - b.ActualBorderRightWidth / 2, r.Bottom - b.ActualCornerSE - b.ActualBorderBottomWidth / 2);
                        }
                    }
                    break;
                case Border.Left:
                    if (b.ActualCornerNW > 0 || b.ActualCornerSW > 0)
                    {
                        path = new GraphicsPath();

                        if (b.ActualCornerSW > 0 && b.BorderTopStyle >= CssBorderStyle.Visible)//(b.BorderTopStyle == CssConstants.None || b.BorderTopStyle == CssConstants.Hidden))
                        {
                            path.AddArc(r.Left + b.ActualBorderLeftWidth / 2, r.Bottom - b.ActualCornerSW * 2 - b.ActualBorderBottomWidth / 2, b.ActualCornerSW * 2, b.ActualCornerSW * 2, 90f, 90f);
                        }
                        else
                        {
                            path.AddLine(r.Left + b.ActualBorderLeftWidth / 2, r.Bottom - b.ActualCornerSW - b.ActualBorderBottomWidth / 2, r.Left + b.ActualBorderLeftWidth / 2, r.Bottom - b.ActualCornerSW - b.ActualBorderBottomWidth / 2 - .1f);
                        }

                        if (b.ActualCornerNW > 0 &&
                            b.BorderBottomStyle >= CssBorderStyle.Visible)
                        {
                            path.AddArc(r.Left + b.ActualBorderLeftWidth / 2, r.Top + b.ActualBorderTopWidth / 2, b.ActualCornerNW * 2, b.ActualCornerNW * 2, 180f, 90f);
                        }
                        else
                        {
                            path.AddLine(r.Left + b.ActualBorderLeftWidth / 2, r.Top + b.ActualCornerNW + b.ActualBorderTopWidth / 2 + .1f, r.Left + b.ActualBorderLeftWidth / 2, r.Top + b.ActualCornerNW + b.ActualBorderTopWidth / 2);
                        }
                    }
                    break;
            }

            return path;
        }

        /// <summary>
        /// Get pen to be used for border draw respecting its style.
        /// </summary>
        static Pen GetPen(CssBorderStyle style, Color color, float width)
        {
            var p = RenderUtils.GetPen(color);
            p.Width = width;
            switch (style)
            {
                case CssBorderStyle.Solid:// "solid":
                    p.DashStyle = DashStyle.Solid;
                    break;
                case CssBorderStyle.Dotted:// "dotted":
                    p.DashStyle = DashStyle.Dot;

                    break;
                case CssBorderStyle.Dashed:// "dashed":
                    p.DashStyle = DashStyle.Dash;
                    if (p.Width < 2)
                        p.DashPattern = new[] { 4, 4f }; // better looking
                    break;
            }
            return p;
        }

        /// <summary>
        /// Get the border color for the given box border.
        /// </summary>
        static Color GetColor(Border border, CssBoxBase box, CssBorderStyle style)
        {
            switch (border)
            {
                case Border.Top:
                    return style == CssBorderStyle.Inset ? Darken(box.BorderTopColor) : box.BorderTopColor;
                case Border.Right:
                    return style == CssBorderStyle.Outset ? Darken(box.BorderRightColor) : box.BorderRightColor;
                case Border.Bottom:
                    return style == CssBorderStyle.Outset ? Darken(box.BorderBottomColor) : box.BorderBottomColor;
                case Border.Left:
                    return style == CssBorderStyle.Inset ? Darken(box.BorderLeftColor) : box.BorderLeftColor;
                default:
                    throw new ArgumentOutOfRangeException("border");
            }
        }

        /// <summary>
        /// Get the border width for the given box border.
        /// </summary>
        static float GetWidth(Border border, CssBox box)
        {
            switch (border)
            {
                case Border.Top:
                    return box.ActualBorderTopWidth;
                case Border.Right:
                    return box.ActualBorderRightWidth;
                case Border.Bottom:
                    return box.ActualBorderBottomWidth;
                case Border.Left:
                    return box.ActualBorderLeftWidth;
                default:
                    throw new ArgumentOutOfRangeException("border");
            }
        }

        /// <summary>
        /// Get the border style for the given box border.
        /// </summary>
        static CssBorderStyle GetStyle(Border border, CssBoxBase box)
        {
            switch (border)
            {
                case Border.Top:
                    return box.BorderTopStyle;
                case Border.Right:
                    return box.BorderRightStyle;
                case Border.Bottom:
                    return box.BorderBottomStyle;
                case Border.Left:
                    return box.BorderLeftStyle;
                default:
                    throw new ArgumentOutOfRangeException("border");
            }
        }

        /// <summary>
        /// Makes the specified color darker for inset/outset borders.
        /// </summary>
        static Color Darken(Color c)
        {
            return Color.FromArgb(c.R / 2, c.G / 2, c.B / 2);
        }

        #endregion
    }
}
