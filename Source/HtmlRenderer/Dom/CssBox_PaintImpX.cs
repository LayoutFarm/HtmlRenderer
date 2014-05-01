//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Globalization;
//using HtmlRenderer.Entities;
//using HtmlRenderer.Handlers;
//using HtmlRenderer.Parse;
//using HtmlRenderer.Utils;

//namespace HtmlRenderer.Dom
//{


//    partial class CssBox
//    {
//        /// <summary>
//        /// Paints the fragment
//        /// </summary>
//        /// <param name="g">the device to draw to</param>
//        void PaintImpX(IGraphics g, PaintingArgs args)
//        {

//            if (this.CssDisplay != CssBoxDisplayType.None &&
//               (this.CssDisplay != CssBoxDisplayType.TableCell ||
//                 EmptyCells != CssConstants.Hide || !IsSpaceOrEmpty))
//            {
//                var prevClip = RenderUtils.ClipGraphicsByOverflow(g, this);

//                //area to draw ?
//                int rectCount = this.Rectangles.Count;
//                PointF offset = HtmlContainer.ScrollOffset;
//                int i = 0;
//                int lim = rectCount - 1;

//                if (rectCount == 0)
//                {
//                    RectangleF actualRect = this.Bounds;
//                    actualRect.Offset(offset);
//                    PaintBackground(g, actualRect, true, true);
//                    BordersDrawHandler.DrawBoxBorders(g, this, actualRect, true, true);
//                    i++;
//                }
//                else
//                {
//                    foreach (RectangleF actualRect in this.Rectangles.Values)
//                    {
//                        actualRect.Offset(offset);
//                        PaintBackground(g, actualRect, i == 0, i == lim);
//                        //BordersDrawHandler.DrawBoxBorders(g, this, actualRect, i == 0, i == lim);
//                        i++;
//                    }
//                }

//                //-----------------------------------
//                PaintWords(g, offset);
//                //-----------------------------------
//                i = 0;//reset
//                if (rectCount == 0)
//                {
//                    RectangleF actualRect = this.Bounds;
//                    actualRect.Offset(offset);
//                    //PaintDecoration(g, actualRect, i == 0, i == lim);
//                    i++;
//                }
//                else
//                {
//                    foreach (RectangleF actualRect in this.Rectangles.Values)
//                    {
//                        actualRect.Offset(offset);
//                        //PaintDecoration(g, actualRect, i == 0, i == lim);
//                        i++;
//                    }
//                }
//                if (this.HasContainingBlockProperty)
//                {
//                    args.PushContainingBox(this);
//                    // split paint to handle z-order
//                    foreach (CssBox b in Boxes)
//                    {
//                        //if (b.Position != CssConstants.Absolute)
//                        if (!b.IsAbsolutePosition)
//                        {
//                            b.Paint(g, args);
//                        }
//                    }
//                    foreach (CssBox b in Boxes)
//                    {
//                        if (b.IsAbsolutePosition)
//                        { //b.Position == CssConstants.Absolute)
//                            b.Paint(g, args);
//                        }
//                    }
//                    args.PopContainingBox();
//                }
//                else
//                {
//                    foreach (CssBox b in Boxes)
//                    {
//                        //if (b.Position != CssConstants.Absolute)
//                        if (!b.IsAbsolutePosition)
//                        {
//                            b.Paint(g, args);
//                        }
//                    }
//                    foreach (CssBox b in Boxes)
//                    {
//                        if (b.IsAbsolutePosition)
//                        { //b.Position == CssConstants.Absolute)
//                            b.Paint(g, args);
//                        }
//                    }
//                }


//                RenderUtils.ReturnClip(g, prevClip);
//                if (_listItemBox != null)
//                {
//                    _listItemBox.Paint(g, args);
//                }
//            }
//        }
//    }
//}