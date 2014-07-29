using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;



namespace LayoutFarm.Presentation
{
    partial class ArtVisualElement
    {



        public bool HasSolidBackground
        {
            get
            {
                if (Beh != null)
                {
                    BoxStyle beh = (BoxStyle)Beh;
                    ArtColorBrush colorBrush = beh.SharedBgColorBrush;

                    return colorBrush != null;
                }
                return false;
            }
        }
        protected static void DrawBackground(ArtVisualElement visualElement, ArtCanvas canvasPage, InternalRect updateArea)
        {
            ArtColorBrush colorBrush = new ArtSolidBrush(Color.White);
            colorBrush.myBrush = Brushes.White;

            if (colorBrush == null)
            {
                return;
            }
            int j = 0;
            int i = 0;
            canvasPage.FillRectangle(colorBrush, 0, 0, visualElement.uiWidth, visualElement.uiHeight);

        }




        public abstract void CustomDrawToThisPage(ArtCanvas canvasPage, InternalRect updateArea);



        public bool PrepareDrawingChain(VisualDrawingChain drawingChain)
        {
            if ((uiFlags & HIDDEN) == HIDDEN)
            {
                return false;
            }

            if (this.IntersectsWith(drawingChain.CurrentClipRect))
            {
                bool containAll = this.ContainRect(drawingChain.CurrentClipRect);

                switch ((VisualElementNature)(uiCombineFlags & 0xF))
                {
                    case VisualElementNature.CssBox:
                        {
                            drawingChain.AddVisualElement(this, containAll);
                        } break;
                    case VisualElementNature.Shapes:
                        {
                            drawingChain.AddVisualElement(this, containAll);
                        } break;
                    case VisualElementNature.TextRun:
                        {

                        } break;

                    default:
                        {
                            drawingChain.AddVisualElement(this, containAll);

                            int x = this.uiLeft;
                            int y = this.uiTop;

                            if (this.IsScrollable)
                            {
                                ArtVisualContainerBase scContainer = (ArtVisualContainerBase)this;
                                x -= scContainer.ViewportX;
                                y -= scContainer.ViewportY;
                            }


                            drawingChain.OffsetCanvasOrigin(x, y);
                            ((ArtVisualContainerBase)this).PrepareOriginalChildContentDrawingChain(drawingChain);
                            drawingChain.OffsetCanvasOrigin(-x, -y);


                        } break;
                }


            }

            return false;







        }
        public void DrawToThisPage(ArtCanvas canvasPage, InternalRect updateArea)
        {

            if ((uiFlags & HIDDEN) == HIDDEN)
            {
                return;
            }
#if DEBUG
            dbugVRoot.dbug_drawLevel++;
#endif

            if (canvasPage.PushClipArea(uiWidth, uiHeight, updateArea))
            {
#if DEBUG
                if (dbugVRoot.dbug_RecordDrawingChain)
                {
                    dbugVRoot.dbug_AddDrawElement(this, canvasPage);
                }
#endif
                switch ((VisualElementNature)(uiCombineFlags & 0xF))
                {
                    case VisualElementNature.HtmlContainer:
                        {
                            throw new NotSupportedException();
                        } break;
                    case VisualElementNature.CssBox:
                        {

                            throw new NotSupportedException();


                        } break;
                    case VisualElementNature.Shapes:
                        {
                        } break;
                    case VisualElementNature.TextRun:
                        {
                            this.CustomDrawToThisPage(canvasPage, updateArea);
                        } break;
                    default:
                        {
                            if (this.HasDoubleScrollableSurface)
                            {
                                ((ArtVisualContainerBase)this).ScrollableDrawContent(canvasPage, updateArea);
                            }
                            else
                            {
                                ((ArtVisualContainerBase)this).ContainerDrawOriginalContent(canvasPage, updateArea);
                            }
                        } break;
                }

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
                return this.ElementNature == VisualElementNature.TextEditContainer;
            }
        }
        public bool IsWindowRoot
        {
            get
            {
                return this.ElementNature == VisualElementNature.WindowRoot;
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
                return this.ElementNature >= VisualElementNature.SimpleContainer;
            }
        }



    }
}