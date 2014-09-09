//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;




namespace LayoutFarm.Presentation.Text
{



    partial class EditableVisualElementLine
    {
        void AddLineBreakAfter(EditableVisualTextRun afterTextRun)
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
                List<EditableVisualTextRun> tempTextRuns = new List<EditableVisualTextRun>(this.Count);
                if (afterTextRun != null)
                {
                    foreach (EditableVisualTextRun t in GetVisualElementForward(afterTextRun.NextTextRun))
                    {
                        tempTextRuns.Add(t);
                    }
                }

                this.EndWithLineBreak = true; this.LocalSuspendLineReArrange(); EditableVisualElementLine newTextline = editableFlowLayer.InsertNewLine(currentLineNumber + 1);

                int j = tempTextRuns.Count;


                newTextline.LocalSuspendLineReArrange(); int cx = 0;

                for (int i = 0; i < j; ++i)
                {
                    EditableVisualTextRun t = tempTextRuns[i];
                    this.Remove(t); newTextline.AddLast(t); RenderElement.DirectSetVisualElementLocation(t, cx, 0);
                    cx += t.Width;
                }

                newTextline.LocalResumeLineReArrange(); this.LocalResumeLineReArrange();
            }

        }
        void AddLineBreakBefore(EditableVisualTextRun beforeTextRun)
        {
            if (beforeTextRun == null)
            {
                this.EndWithLineBreak = true;
                editableFlowLayer.InsertNewLine(currentLineNumber + 1);
            }
            else
            {
                List<EditableVisualTextRun> tempTextRuns = new List<EditableVisualTextRun>();
                if (beforeTextRun != null)
                {
                    foreach (EditableVisualTextRun t in GetVisualElementForward(beforeTextRun))
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
                    EditableVisualTextRun t = tempTextRuns[i];
                    this.Remove(t);
                    newTextline.AddLast(t);

                }
                this.LocalResumeLineReArrange();
                newTextline.LocalResumeLineReArrange();
            }
        }

        void RemoveLeft(EditableVisualTextRun t)
        {
            if (t != null)
            {
                if (t.IsLineBreak)
                {
                    throw new NotSupportedException();
                }

                LinkedList<EditableVisualTextRun> tobeRemoveTextRuns = CollectLeftRuns(t);

                LinkedListNode<EditableVisualTextRun> curNode = tobeRemoveTextRuns.First;
                LocalSuspendLineReArrange();
                while (curNode != null)
                {
                    Remove(curNode.Value);
                    curNode = curNode.Next;
                }
                LocalResumeLineReArrange();
            }
        }
        void RemoveRight(EditableVisualTextRun t)
        {
            if (t.IsLineBreak)
            {
                throw new NotSupportedException();
            }

            LinkedList<EditableVisualTextRun> tobeRemoveTextRuns = CollectRightRuns(t);

            LinkedListNode<EditableVisualTextRun> curNode = tobeRemoveTextRuns.First;
            LocalSuspendLineReArrange();
            while (curNode != null)
            {
                Remove(curNode.Value);
                curNode = curNode.Next;
            }
            LocalResumeLineReArrange();

        }
        LinkedList<EditableVisualTextRun> CollectLeftRuns(EditableVisualTextRun t)
        {

            if (t.IsLineBreak)
            {
                throw new NotSupportedException();
            }

            LinkedList<EditableVisualTextRun> colllectRun = new LinkedList<EditableVisualTextRun>();
            foreach (EditableVisualTextRun r in GetVisualElementForward(this.FirstRun, t))
            {
                colllectRun.AddLast(r);
            }
            return colllectRun;
        }
        LinkedList<EditableVisualTextRun> CollectRightRuns(EditableVisualTextRun t)
        {
            if (t.IsLineBreak)
            {
                throw new NotSupportedException();
            }
            LinkedList<EditableVisualTextRun> colllectRun = new LinkedList<EditableVisualTextRun>();
            foreach (EditableVisualTextRun r in editableFlowLayer.TextRunForward(t, this.LastRun))
            {
                colllectRun.AddLast(r);
            }
            return colllectRun;
        }
        public void ReplaceAll(IEnumerable<EditableVisualTextRun> textRuns)
        {
            this.Clear();
            this.LocalSuspendLineReArrange();
            foreach (EditableVisualTextRun r in textRuns)
            {
                this.AddLast(r);
            }
            this.LocalResumeLineReArrange();

        }
    }

}