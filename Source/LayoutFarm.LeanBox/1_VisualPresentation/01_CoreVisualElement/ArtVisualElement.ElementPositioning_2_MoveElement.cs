//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

using LayoutFarm.Presentation;


namespace LayoutFarm.Presentation
{

    partial class ArtVisualElement
    {

        protected static Point MoveCareParentBound(ArtVisualElement visualElement, int dx, int dy, VisualElementArgs vinv)
        {
            ArtVisualElement parentElement = visualElement.ParentVisualElement;
            if (parentElement == null)
            {
                return Point.Empty;
            }
            else
            {
                visualElement.BeforeBoundChangedInvalidateGraphics(vinv);

                if (dx < 0)
                {
                    if (visualElement.uiLeft + dx > -1)
                    {
                        visualElement.uiLeft += dx;
                    }
                    else
                    {
                        dx = 0;
                    }
                }
                else
                {
                    if (visualElement.Right + dx < (parentElement.Right - parentElement.uiLeft))
                    {
                        visualElement.uiLeft += dx;
                    }
                    else
                    {
                        dx = 0;
                    }
                }
                if (dy < 0)
                {
                    if (visualElement.uiTop + dy > -1)
                    {
                        visualElement.uiTop += dy;
                    }
                    else
                    {
                        dy = 0;
                    }
                }
                else
                {
                    if (visualElement.Bottom + dy < (parentElement.Bottom - parentElement.uiTop))
                    {
                        visualElement.uiTop += dy;

                    }
                    else
                    {
                        dy = 0;
                    }
                }
                visualElement.AfterBoundChangedInvalidateGraphics(vinv);
                return new Point(dx, dy);
            }

        }

        public void SetLocation(int x, int y, VisualElementArgs vinv)
        {
            if (vinv == null)
            {
                uiLeft = x;
                uiTop = y;
            }
            else
            {
                if (uiLeft == x && uiTop == y)
                {
                    return;
                }

                this.BeforeBoundChangedInvalidateGraphics(vinv);
                uiLeft = x;
                uiTop = y;
                this.AfterBoundChangedInvalidateGraphics(vinv);
            }


        }
        public void SetLocation(Point p, VisualElementArgs vinv)
        {
            SetLocation(p.X, p.Y, vinv);
        }

        public void SetLeft(int x, VisualElementArgs vinv)
        {
            if (visualParentLink == null)
            {
                uiLeft = x;
            }
            else if (uiLeft != x)
            {
                this.BeforeBoundChangedInvalidateGraphics(vinv);
                uiLeft = x;
                this.AfterBoundChangedInvalidateGraphics(vinv);
            }
        }
        public void SetTop(int y, VisualElementArgs vinv)
        {
            if (visualParentLink == null)
            {
                uiTop = y;
            }
            else if (uiTop != y)
            {
                this.BeforeBoundChangedInvalidateGraphics(vinv);
                uiTop = y;
                this.AfterBoundChangedInvalidateGraphics(vinv);
            }
        }

        public void Offset(int dx, int dy, VisualElementArgs vinv)
        {
            if (visualParentLink == null)
            {
                uiLeft += dx;
                uiTop += dy;
            }
            else if (!(dx == 0 && dy == 0))
            {
                this.BeforeBoundChangedInvalidateGraphics(vinv);
                uiLeft += dx;
                uiTop += dy;
                this.AfterBoundChangedInvalidateGraphics(vinv);
            }
        }

        public void Move(int dx, int dy, VisualElementArgs vinv)
        {
            if (visualParentLink == null)
            {
                uiLeft += dx;
                uiTop += dy;
            }
            else if (!(dx == 0 && dy == 0))
            {
                this.BeforeBoundChangedInvalidateGraphics(vinv);
                uiLeft += dx;
                uiTop += dy;
                this.AfterBoundChangedInvalidateGraphics(vinv);
            }

        }

    }
}