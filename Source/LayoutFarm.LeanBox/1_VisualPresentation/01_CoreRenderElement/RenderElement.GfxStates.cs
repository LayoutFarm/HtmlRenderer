//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace LayoutFarm.Presentation
{
    partial class RenderElement
    {

        object basicStyle;

        public BoxStyle MyBoxStyle
        {
            get
            {
                if (this.basicStyle != null)
                {
                    return (BoxStyle)this.basicStyle;

                }
                return null;
            }
        }

        public virtual void SetStyle(BoxStyle newbeh, VisualElementArgs vinv)
        {

            BoxStyle beh = (BoxStyle)newbeh;
            if (newbeh == null)
            {
                return;
            }

            if ((uiFlags & USE_ANIMATOR) == 0)
            {
                if (vinv != null)
                {
                    this.InvalidateGraphic(vinv);
                }

                this.basicStyle = beh;
                if (beh.positionWidth > -1)
                {
                    this.SetWidth(beh.positionWidth, vinv);
                }
                if (beh.positionHeight > -1)
                {
                    this.SetHeight(beh.positionHeight, vinv);
                }
                if (vinv != null)
                {
                    this.InvalidateGraphic(vinv);
                }
            }
            else
            {

            }
        }




        public bool TransparentForAllEvents
        {
            get
            {
                return (uiFlags & TRANSPARENT_FOR_ALL_EVENTS) != 0;

            }
            set
            {
                if (value)
                {
                    uiFlags |= TRANSPARENT_FOR_ALL_EVENTS;
                }
                else
                {
                    uiFlags &= ~TRANSPARENT_FOR_ALL_EVENTS;
                }

            }
        }

    }
}