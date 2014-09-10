//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;



namespace LayoutFarm.Presentation
{
    partial class RenderElement
    {
        public bool HasSolidBackground
        {
            get
            {
                if (MyBoxStyle != null)
                {
                    BoxStyle beh = (BoxStyle)MyBoxStyle;
                    ArtColorBrush colorBrush = beh.SharedBgColorBrush;

                    return colorBrush != null;
                }
                return false;
            }
        }
        protected static void DrawBackground(RenderElement visualElement, CanvasBase canvasPage, InternalRect updateArea)
        {
            ArtColorBrush colorBrush = new ArtSolidBrush(Color.White);
            colorBrush.myBrush = Brushes.White;

            if (colorBrush == null)
            {
                return;
            }

            canvasPage.FillRectangle(colorBrush, 0, 0, visualElement.b_width, visualElement.b_Height);

        }

        public abstract void CustomDrawToThisPage(CanvasBase canvasPage, InternalRect updateArea);

        public bool PrepareDrawingChain(VisualDrawingChain drawingChain)
        {
            if ((uiFlags & HIDDEN) == HIDDEN)
            {
                return false;
            }

            if (this.IntersectsWith(drawingChain.CurrentClipRect))
            {
                bool containAll = this.ContainRect(drawingChain.CurrentClipRect);

                switch ((ElementNature)(uiCombineFlags & 0xF))
                {

                    case ElementNature.Shapes:
                        {
                            drawingChain.AddVisualElement(this, containAll);
                        } break;
                    case ElementNature.TextRun:
                        {

                        } break;
                    default:
                        {
                            drawingChain.AddVisualElement(this, containAll);

                            int x = this.b_left;
                            int y = this.b_top;

                            if (this.IsScrollable)
                            {
                                MultiLayerRenderBox scContainer = (MultiLayerRenderBox)this;
                                x -= scContainer.ViewportX;
                                y -= scContainer.ViewportY;
                            }


                            drawingChain.OffsetCanvasOrigin(x, y);
                            ((MultiLayerRenderBox)this).PrepareOriginalChildContentDrawingChain(drawingChain);
                            drawingChain.OffsetCanvasOrigin(-x, -y);


                        } break;
                }
            }
            return false;
        }
        public void DrawToThisPage(CanvasBase canvasPage, InternalRect updateArea)
        {

            if ((uiFlags & HIDDEN) == HIDDEN)
            {
                return;
            }
#if DEBUG
            dbugVRoot.dbug_drawLevel++;
#endif

            if (canvasPage.PushClipArea(b_width, b_Height, updateArea))
            {
#if DEBUG
                if (dbugVRoot.dbug_RecordDrawingChain)
                {
                    dbugVRoot.dbug_AddDrawElement(this, canvasPage);
                }
#endif
                //------------------------------------------
                this.CustomDrawToThisPage(canvasPage, updateArea);

                //switch ((ElementNature)(uiCombineFlags & 0xF))
                //{
                //    case ElementNature.Shapes:
                //    case ElementNature.CustomContainer:
                //    case ElementNature.TextRun:
                //        {
                //            this.CustomDrawToThisPage(canvasPage, updateArea);
                //        } break;
                //    default:
                //        {
                //            if (this.HasDoubleScrollableSurface)
                //            {
                //                ((MultiLayerRenderBox)this).ScrollableDrawContent(canvasPage, updateArea);
                //            }
                //            else
                //            {
                //                ((MultiLayerRenderBox)this).ContainerDrawOriginalContent(canvasPage, updateArea);
                //            }
                //        } break;
                //}

                //------------------------------------------
                uiFlags |= IS_GRAPHIC_VALID;
#if DEBUG
                debug_RecordPostDrawInfo(canvasPage);
#endif
            }

            canvasPage.PopClipArea();
#if DEBUG
            dbugVRoot.dbug_drawLevel--;
#endif
        }

        public bool IsTextEditContainer
        {
            get
            {
                return this.ElementNature == ElementNature.TextEditContainer;
            }
        }
        public bool IsWindowRoot
        {
            get
            {
                return this.ElementNature == ElementNature.WindowRoot;
            }
        }
        public bool IsScrollable
        {
            get
            {
                return (this.uiFlags & IS_SCROLLABLE) != 0;
            }
            protected set
            {
                if (value)
                {
                    this.uiFlags |= IS_SCROLLABLE;
                }
                else
                {
                    this.uiFlags &= ~IS_SCROLLABLE;
                }
            }
        }
        public bool HasDoubleScrollableSurface
        {
            get
            {
                return (this.uiFlags & HAS_DOUBLE_SCROLL_SURFACE) != 0;
            }
            protected set
            {
                if (value)
                {
                    this.uiFlags |= HAS_DOUBLE_SCROLL_SURFACE;
                }
                else
                {
                    this.uiFlags &= ~HAS_DOUBLE_SCROLL_SURFACE;
                }
            }
        }

        public bool IsVisualContainerBase
        {
            get
            {
                return this.ElementNature >= ElementNature.WindowRoot;
            }
        }



    }
}