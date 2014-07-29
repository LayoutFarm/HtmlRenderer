using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;




namespace LayoutFarm.Presentation.Text
{

        

    partial class EditableVisualElementLine
    {                                  void AddLineBreakAfter(ArtEditableVisualTextRun afterTextRun)
        {
                        if (afterTextRun == null)
            {
                this.EndWithLineBreak = true;                EditableVisualElementLine newline = editableFlowLayer.InsertNewLine(currentLineNumber + 1);
                if (editableFlowLayer.LineCount - 1 != newline.LineNumber)
                {                       newline.EndWithLineBreak = true;
                }
                return;
            }
                        if (afterTextRun.NextTextRun == null)
            {
                                this.EndWithLineBreak = true;                EditableVisualElementLine newline = editableFlowLayer.InsertNewLine(currentLineNumber + 1);
                                if (editableFlowLayer.LineCount - 1 != newline.LineNumber)
                {                       newline.EndWithLineBreak = true;
                }
            }
            else
            {
                                List<ArtEditableVisualTextRun> tempTextRuns = new List<ArtEditableVisualTextRun>(this.Count);
                if (afterTextRun != null)
                {
                    foreach (ArtEditableVisualTextRun t in GetVisualElementForward(afterTextRun.NextTextRun))
                    {
                        tempTextRuns.Add(t);
                    }
                }
                                                                                                
                this.EndWithLineBreak = true;                this.LocalSuspendLineReArrange();                 EditableVisualElementLine newTextline = editableFlowLayer.InsertNewLine(currentLineNumber + 1);

                                int j = tempTextRuns.Count;


                newTextline.LocalSuspendLineReArrange();                 int cx = 0;

                for (int i = 0; i < j; ++i)
                {
                    ArtEditableVisualTextRun t = tempTextRuns[i];
                    this.Remove(t);                    newTextline.AddLast(t);                    ArtVisualElement.DirectSetVisualElementLocation(t, cx, 0);
                    cx += t.Width;
                }

                newTextline.LocalResumeLineReArrange();                 this.LocalResumeLineReArrange();
                            }

        }
                                        void AddLineBreakBefore(ArtEditableVisualTextRun beforeTextRun)
        {
            if (beforeTextRun == null)
            {
                                this.EndWithLineBreak = true;
                editableFlowLayer.InsertNewLine(currentLineNumber + 1);
            }
            else
            {
                                                List<ArtEditableVisualTextRun> tempTextRuns = new List<ArtEditableVisualTextRun>();
                if (beforeTextRun != null)
                {
                    foreach (ArtEditableVisualTextRun t in GetVisualElementForward(beforeTextRun))
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
                                        ArtEditableVisualTextRun t = tempTextRuns[i];
                    this.Remove(t);
                                        newTextline.AddLast(t);

                }
                                this.LocalResumeLineReArrange();
                newTextline.LocalResumeLineReArrange();
            }
        }

                                        void RemoveLeft(ArtEditableVisualTextRun t)
        {
            if (t != null)
            {
                if (t.IsLineBreak)
                {
                    throw new NotSupportedException();
                }

                LinkedList<ArtEditableVisualTextRun> tobeRemoveTextRuns = CollectLeftRuns(t);

                LinkedListNode<ArtEditableVisualTextRun> curNode = tobeRemoveTextRuns.First;
                LocalSuspendLineReArrange();
                while (curNode != null)
                {
                    Remove(curNode.Value);
                    curNode = curNode.Next;
                }
                LocalResumeLineReArrange();
            }
        }
                                        void RemoveRight(ArtEditableVisualTextRun t)
        {
                        if (t.IsLineBreak)
            {
                throw new NotSupportedException();
            }

            LinkedList<ArtEditableVisualTextRun> tobeRemoveTextRuns = CollectRightRuns(t);

            LinkedListNode<ArtEditableVisualTextRun> curNode = tobeRemoveTextRuns.First;
            LocalSuspendLineReArrange();
            while (curNode != null)
            {
                Remove(curNode.Value);
                curNode = curNode.Next;
            }
            LocalResumeLineReArrange();

        }
                                                LinkedList<ArtEditableVisualTextRun> CollectLeftRuns(ArtEditableVisualTextRun t)
        {

            if (t.IsLineBreak)
            {
                throw new NotSupportedException();
            }

            LinkedList<ArtEditableVisualTextRun> colllectRun = new LinkedList<ArtEditableVisualTextRun>();
            foreach (ArtEditableVisualTextRun r in GetVisualElementForward(this.FirstRun, t))
            {
                colllectRun.AddLast(r);
            }
            return colllectRun;
        }
                                                LinkedList<ArtEditableVisualTextRun> CollectRightRuns(ArtEditableVisualTextRun t)
        {               if (t.IsLineBreak)
            {
                throw new NotSupportedException();
            }
            LinkedList<ArtEditableVisualTextRun> colllectRun = new LinkedList<ArtEditableVisualTextRun>();
            foreach (ArtEditableVisualTextRun r in editableFlowLayer.TextRunForward(t, this.LastRun))
            {
                colllectRun.AddLast(r);
            }
            return colllectRun;
        }
                                        public void ReplaceAll(IEnumerable<ArtEditableVisualTextRun> textRuns)
        {
            this.Clear();
            this.LocalSuspendLineReArrange();
            foreach (ArtEditableVisualTextRun r in textRuns)
            {
                this.AddLast(r);
            }
            this.LocalResumeLineReArrange();

        }
    }

}