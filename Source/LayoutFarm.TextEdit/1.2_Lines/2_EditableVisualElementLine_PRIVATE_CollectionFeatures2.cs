//2014,2015 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;




namespace LayoutFarm.Text
{



    partial class EditableVisualElementLine
    {
        void AddLineBreakAfter(EditableTextSpan afterTextRun)
        {
            if (afterTextRun == null)
            {
                this.EndWithLineBreak = true; EditableVisualElementLine newline = editableFlowLayer.InsertNewLine(currentLineNumber + 1);
                if (editableFlowLayer.LineCount - 1 != newline.LineNumber)
                {
                    newline.EndWithLineBreak = true;
                }
                return;
            }
            if (afterTextRun.NextTextRun == null)
            {
                this.EndWithLineBreak = true; EditableVisualElementLine newline = editableFlowLayer.InsertNewLine(currentLineNumber + 1);
                if (editableFlowLayer.LineCount - 1 != newline.LineNumber)
                {
                    newline.EndWithLineBreak = true;
                }
            }
            else
            {
                List<EditableTextSpan> tempTextRuns = new List<EditableTextSpan>(this.Count);
                if (afterTextRun != null)
                {
                    foreach (EditableTextSpan t in GetVisualElementForward(afterTextRun.NextTextRun))
                    {
                        tempTextRuns.Add(t);
                    }
                }

                this.EndWithLineBreak = true; this.LocalSuspendLineReArrange(); EditableVisualElementLine newTextline = editableFlowLayer.InsertNewLine(currentLineNumber + 1);

                int j = tempTextRuns.Count;


                newTextline.LocalSuspendLineReArrange(); int cx = 0;

                for (int i = 0; i < j; ++i)
                {
                    EditableTextSpan t = tempTextRuns[i];
                    this.Remove(t); newTextline.AddLast(t); RenderElement.DirectSetVisualElementLocation(t, cx, 0);
                    cx += t.Width;
                }

                newTextline.LocalResumeLineReArrange(); this.LocalResumeLineReArrange();
            }

        }
        void AddLineBreakBefore(EditableTextSpan beforeTextRun)
        {
            if (beforeTextRun == null)
            {
                this.EndWithLineBreak = true;
                editableFlowLayer.InsertNewLine(currentLineNumber + 1);
            }
            else
            {
                List<EditableTextSpan> tempTextRuns = new List<EditableTextSpan>();
                if (beforeTextRun != null)
                {
                    foreach (EditableTextSpan t in GetVisualElementForward(beforeTextRun))
                    {
                        tempTextRuns.Add(t);
                    }
                }
                this.EndWithLineBreak = true;
                EditableVisualElementLine newTextline = editableFlowLayer.InsertNewLine(currentLineNumber + 1);

                this.LocalSuspendLineReArrange();
                newTextline.LocalSuspendLineReArrange();
                int j = tempTextRuns.Count;
                for (int i = 0; i < j; ++i)
                {
                    EditableTextSpan t = tempTextRuns[i];
                    this.Remove(t);
                    newTextline.AddLast(t);

                }
                this.LocalResumeLineReArrange();
                newTextline.LocalResumeLineReArrange();
            }
        }

        void RemoveLeft(EditableTextSpan t)
        {
            if (t != null)
            {
                if (t.IsLineBreak)
                {
                    throw new NotSupportedException();
                }

                LinkedList<EditableTextSpan> tobeRemoveTextRuns = CollectLeftRuns(t);

                LinkedListNode<EditableTextSpan> curNode = tobeRemoveTextRuns.First;
                LocalSuspendLineReArrange();
                while (curNode != null)
                {
                    Remove(curNode.Value);
                    curNode = curNode.Next;
                }
                LocalResumeLineReArrange();
            }
        }
        void RemoveRight(EditableTextSpan t)
        {
            if (t.IsLineBreak)
            {
                throw new NotSupportedException();
            }

            LinkedList<EditableTextSpan> tobeRemoveTextRuns = CollectRightRuns(t);

            LinkedListNode<EditableTextSpan> curNode = tobeRemoveTextRuns.First;
            LocalSuspendLineReArrange();
            while (curNode != null)
            {
                Remove(curNode.Value);
                curNode = curNode.Next;
            }
            LocalResumeLineReArrange();

        }
        LinkedList<EditableTextSpan> CollectLeftRuns(EditableTextSpan t)
        {

            if (t.IsLineBreak)
            {
                throw new NotSupportedException();
            }

            LinkedList<EditableTextSpan> colllectRun = new LinkedList<EditableTextSpan>();
            foreach (EditableTextSpan r in GetVisualElementForward(this.FirstRun, t))
            {
                colllectRun.AddLast(r);
            }
            return colllectRun;
        }
        LinkedList<EditableTextSpan> CollectRightRuns(EditableTextSpan t)
        {
            if (t.IsLineBreak)
            {
                throw new NotSupportedException();
            }
            LinkedList<EditableTextSpan> colllectRun = new LinkedList<EditableTextSpan>();
            foreach (EditableTextSpan r in editableFlowLayer.TextRunForward(t, this.LastRun))
            {
                colllectRun.AddLast(r);
            }
            return colllectRun;
        }
        public void ReplaceAll(IEnumerable<EditableTextSpan> textRuns)
        {
            this.Clear();
            this.LocalSuspendLineReArrange();
            foreach (EditableTextSpan r in textRuns)
            {
                this.AddLast(r);
            }
            this.LocalResumeLineReArrange();

        }
    }

}