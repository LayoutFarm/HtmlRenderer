//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace LayoutFarm.Text
{

    partial class EditableVisualElementLine
    {
        public void RefreshInlineArrange()
        {
            EditableTextSpan r = this.FirstRun;
            int lastestX = 0;
            while (r != null)
            {
                RenderElement.DirectSetVisualElementLocation(
                        r,
                        lastestX,
                        r.Y);
                lastestX += r.Width;
                r = r.NextTextRun;
            }

        }
        internal void SetPostArrangeLineSize(int lineWidth, int lineHeight)
        {
            this.actualLineWidth = lineWidth;
            this.actualLineHeight = lineHeight;
        }



        public void LocalSuspendLineReArrange()
        {
            lineFlags |= LOCAL_SUSPEND_LINE_REARRANGE;
        }
        public void LocalResumeLineReArrange()
        {
            lineFlags &= ~LOCAL_SUSPEND_LINE_REARRANGE;

            LinkedListNode<EditableTextSpan> curNode = this.First;
            int cx = 0;
            while (curNode != null)
            {
                EditableTextSpan ve = curNode.Value;
                EditableTextSpan.DirectSetVisualElementLocation(ve, cx, 0);
                cx += ve.Width;
                curNode = curNode.Next;
            }
        }

    }

}