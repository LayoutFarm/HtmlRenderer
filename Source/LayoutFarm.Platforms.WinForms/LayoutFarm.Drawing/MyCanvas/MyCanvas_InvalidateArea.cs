//2014 BSD, WinterDev
//ArthurHub

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
using System.Text;
using LayoutFarm.Drawing;


namespace LayoutFarm
{
    partial class MyCanvas
    {
        public override bool DimensionInvalid
        {
            get
            {
                return (pageFlags & CANVAS_DIMEN_CHANGED) != 0;
            }
            set
            {
                if (value)
                {
                    pageFlags |= CANVAS_DIMEN_CHANGED;
                }
                else
                {
                    pageFlags &= ~CANVAS_DIMEN_CHANGED;
                }
            }
        }
        public void MarkAsFirstTimeInvalidateAndUpdateContent()
        {
            canvasFlags = FIRSTTIME_INVALID_AND_UPDATED_CONTENT;
        }
        public override Rect InvalidateArea
        {
            get
            {
                return invalidateArea;
            }
        }
        public override bool IsContentReady
        {
            get
            {
                return ((canvasFlags & UPDATED_CONTENT) == UPDATED_CONTENT);
            }
        }

        public override void Invalidate(Rect rect)
        {

            if ((canvasFlags & FIRSTTIME_INVALID) == FIRSTTIME_INVALID)
            {
                invalidateArea.LoadValues(rect);
            }
            else
            {
                invalidateArea.MergeRect(rect);
            }

            canvasFlags = WAIT_FOR_UPDATE;
        }


    }
}