//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.Text
{
    partial class EditableTextLine
    {
        void AddLineBreakAfter(EditableRun afterTextRun)
        {
            if (afterTextRun == null)
            {
                this.EndWithLineBreak = true; EditableTextLine newline = editableFlowLayer.InsertNewLine(currentLineNumber + 1);
                if (editableFlowLayer.LineCount - 1 != newline.LineNumber)
                {
                    newline.EndWithLineBreak = true;
                }
                return;
            }
            if (afterTextRun.NextTextRun == null)
            {
                this.EndWithLineBreak = true; EditableTextLine newline = editableFlowLayer.InsertNewLine(currentLineNumber + 1);
                if (editableFlowLayer.LineCount - 1 != newline.LineNumber)
                {
                    newline.EndWithLineBreak = true;
                }
            }
            else
            {
                List<EditableRun> tempTextRuns = new List<EditableRun>(this.Count);
                if (afterTextRun != null)
                {
                    foreach (EditableRun t in GetVisualElementForward(afterTextRun.NextTextRun))
                    {
                        tempTextRuns.Add(t);
                    }
                }

                this.EndWithLineBreak = true; this.LocalSuspendLineReArrange(); EditableTextLine newTextline = editableFlowLayer.InsertNewLine(currentLineNumber + 1);
                int j = tempTextRuns.Count;
                newTextline.LocalSuspendLineReArrange(); int cx = 0;
                for (int i = 0; i < j; ++i)
                {
                    EditableRun t = tempTextRuns[i];
                    this.Remove(t); newTextline.AddLast(t); RenderElement.DirectSetLocation(t, cx, 0);
                    cx += t.Width;
                }

                newTextline.LocalResumeLineReArrange(); this.LocalResumeLineReArrange();
            }
        }
        void AddLineBreakBefore(EditableRun beforeTextRun)
        {
            if (beforeTextRun == null)
            {
                this.EndWithLineBreak = true;
                editableFlowLayer.InsertNewLine(currentLineNumber + 1);
            }
            else
            {
                List<EditableRun> tempTextRuns = new List<EditableRun>();
                if (beforeTextRun != null)
                {
                    foreach (EditableRun t in GetVisualElementForward(beforeTextRun))
                    {
                        tempTextRuns.Add(t);
                    }
                }
                this.EndWithLineBreak = true;
                EditableTextLine newTextline = editableFlowLayer.InsertNewLine(currentLineNumber + 1);
                this.LocalSuspendLineReArrange();
                newTextline.LocalSuspendLineReArrange();
                int j = tempTextRuns.Count;
                for (int i = 0; i < j; ++i)
                {
                    EditableRun t = tempTextRuns[i];
                    this.Remove(t);
                    newTextline.AddLast(t);
                }
                this.LocalResumeLineReArrange();
                newTextline.LocalResumeLineReArrange();
            }
        }

        void RemoveLeft(EditableRun t)
        {
            if (t != null)
            {
                if (t.IsLineBreak)
                {
                    throw new NotSupportedException();
                }

                LinkedList<EditableRun> tobeRemoveTextRuns = CollectLeftRuns(t);
                LinkedListNode<EditableRun> curNode = tobeRemoveTextRuns.First;
                LocalSuspendLineReArrange();
                while (curNode != null)
                {
                    Remove(curNode.Value);
                    curNode = curNode.Next;
                }
                LocalResumeLineReArrange();
            }
        }
        void RemoveRight(EditableRun t)
        {
            if (t.IsLineBreak)
            {
                throw new NotSupportedException();
            }

            LinkedList<EditableRun> tobeRemoveTextRuns = CollectRightRuns(t);
            LinkedListNode<EditableRun> curNode = tobeRemoveTextRuns.First;
            LocalSuspendLineReArrange();
            while (curNode != null)
            {
                Remove(curNode.Value);
                curNode = curNode.Next;
            }
            LocalResumeLineReArrange();
        }
        LinkedList<EditableRun> CollectLeftRuns(EditableRun t)
        {
            if (t.IsLineBreak)
            {
                throw new NotSupportedException();
            }

            LinkedList<EditableRun> colllectRun = new LinkedList<EditableRun>();
            foreach (EditableRun r in GetVisualElementForward(this.FirstRun, t))
            {
                colllectRun.AddLast(r);
            }
            return colllectRun;
        }
        LinkedList<EditableRun> CollectRightRuns(EditableRun t)
        {
            if (t.IsLineBreak)
            {
                throw new NotSupportedException();
            }
            LinkedList<EditableRun> colllectRun = new LinkedList<EditableRun>();
            foreach (EditableRun r in editableFlowLayer.TextRunForward(t, this.LastRun))
            {
                colllectRun.AddLast(r);
            }
            return colllectRun;
        }
        public void ReplaceAll(IEnumerable<EditableRun> textRuns)
        {
            this.Clear();
            this.LocalSuspendLineReArrange();
            foreach (EditableRun r in textRuns)
            {
                this.AddLast(r);
            }
            this.LocalResumeLineReArrange();
        }
    }
}