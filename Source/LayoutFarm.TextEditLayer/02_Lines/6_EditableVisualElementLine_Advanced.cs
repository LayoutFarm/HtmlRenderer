//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace LayoutFarm.Presentation.Text
{


    partial class EditableVisualElementLine
    {
        public static void InnerDoJoinWithNextLine(EditableVisualElementLine line)
        {
            line.JoinWithNextLine();
        }
        internal void JoinWithNextLine()
        {
            if (!IsLastLine)
            {
                EditableVisualElementLine lowerLine = editableFlowLayer.GetTextLine(currentLineNumber + 1);

                this.LocalSuspendLineReArrange();
                int cx = 0;
                ArtEditableVisualTextRun lastTextRun = (ArtEditableVisualTextRun)this.LastRun;
                if (lastTextRun != null)
                {
                    cx = lastTextRun.Right;
                }

                foreach (ArtEditableVisualTextRun r in lowerLine)
                {
                    this.AddLast(r);

                    ArtEditableVisualTextRun.DirectSetVisualElementLocation(r, cx, 0);
                    cx += r.Width;
                }
                this.LocalResumeLineReArrange();
                this.EndWithLineBreak = lowerLine.EndWithLineBreak;

                editableFlowLayer.Remove(lowerLine.currentLineNumber);
            }
        }
        internal void UnsafeDetachFromFlowLayer()
        {
            this.editableFlowLayer = null;
        }
        public void Copy(LinkedList<ArtEditableVisualTextRun> output)
        {

            LinkedListNode<ArtEditableVisualTextRun> curNode = this.First;
            while (curNode != null)
            {
                output.AddLast(curNode.Value.Clone());
                curNode = curNode.Next;
            }
        }
        public void Copy(VisualSelectionRange selectionRange, LinkedList<ArtEditableVisualTextRun> output)
        {
            EditableVisualPointInfo startPoint = selectionRange.StartPoint;
            EditableVisualPointInfo endPoint = selectionRange.EndPoint;
            if (startPoint.TextRun != null)
            {
                if (startPoint.TextRun == endPoint.TextRun)
                {

                    ArtEditableVisualTextRun elem =
                      startPoint.TextRun.Copy(
                        startPoint.LocalSelectedIndex + 1,
                        endPoint.LineCharIndex - startPoint.LineCharIndex);
                    if (elem != null)
                    {
                        output.AddLast(elem);
                    }
                }
                else
                {

                    EditableVisualElementLine startLine = null;
                    EditableVisualElementLine stopLine = null;

                    if (startPoint.LineId == currentLineNumber)
                    {
                        startLine = this;
                    }
                    else
                    {
                        startLine = editableFlowLayer.GetTextLine(startPoint.LineId);
                    }
                    if (endPoint.LineId == currentLineNumber)
                    {
                        stopLine = this;
                    }
                    else
                    {
                        stopLine = editableFlowLayer.GetTextLine(endPoint.LineId);
                    }
                    if (startLine == stopLine)
                    {
                        ArtEditableVisualTextRun postCutTextRun = startPoint.TextRun.Copy(startPoint.LocalSelectedIndex + 1);
                        if (postCutTextRun != null)
                        {
                            output.AddLast(postCutTextRun);
                        }
                        if (startPoint.TextRun.NextTextRun != endPoint.TextRun)
                        {
                            foreach (ArtEditableVisualTextRun t in editableFlowLayer.TextRunForward(startPoint.TextRun.NextTextRun, endPoint.TextRun.PrevTextRun))
                            {
                                output.AddLast(new ArtEditableVisualTextRun(t.Text));
                            }
                        }

                        ArtEditableVisualTextRun preCutTextRun = endPoint.TextRun.LeftCopy(endPoint.LocalSelectedIndex);
                        if (preCutTextRun != null)
                        {
                            output.AddLast(preCutTextRun);

                        }
                    }
                    else
                    {
                        int startLineId = startPoint.LineId;
                        int stopLineId = endPoint.LineId;
                        startLine.RightCopy(startPoint, output);
                        for (int i = startLineId + 1; i < stopLineId; i++)
                        {
                            output.AddLast(new ArtEditableVisualTextRun('\n'));
                            EditableVisualElementLine line = editableFlowLayer.GetTextLine(i);
                            line.Copy(output);
                        }
                        if (endPoint.LineCharIndex > -1)
                        {
                            output.AddLast(new ArtEditableVisualTextRun('\n'));
                            stopLine.LeftCopy(endPoint, output);
                        }

                    }
                }
            }
            else
            {

                EditableVisualElementLine startLine = null;
                EditableVisualElementLine stopLine = null;

                if (startPoint.LineId == currentLineNumber)
                {
                    startLine = this;
                }
                else
                {
                    startLine = editableFlowLayer.GetTextLine(startPoint.LineId);
                }

                if (endPoint.LineId == currentLineNumber)
                {
                    stopLine = this;
                }
                else
                {
                    stopLine = editableFlowLayer.GetTextLine(endPoint.LineId);
                }


                if (startLine == stopLine)
                {
                    if (startPoint.LineCharIndex == -1)
                    {

                        foreach (ArtEditableVisualTextRun t in editableFlowLayer.TextRunForward(startPoint.TextRun, endPoint.TextRun.PrevTextRun))
                        {
                            output.AddLast(t.Clone());
                        }
                        ArtEditableVisualTextRun postCutTextRun = endPoint.TextRun.Copy(endPoint.LocalSelectedIndex + 1);
                        if (postCutTextRun != null)
                        {
                            output.AddLast(postCutTextRun);
                        }

                    }
                    else
                    {

                        ArtEditableVisualTextRun postCutTextRun = startPoint.TextRun.Copy(startPoint.LocalSelectedIndex + 1);

                        if (postCutTextRun != null)
                        {
                            output.AddLast(postCutTextRun);
                        }

                        foreach (ArtEditableVisualTextRun t in editableFlowLayer.TextRunForward(startPoint.TextRun.NextTextRun, endPoint.TextRun.PrevTextRun))
                        {
                            output.AddLast(t.Clone());
                        }

                        ArtEditableVisualTextRun preCutTextRun = endPoint.TextRun.LeftCopy(startPoint.LocalSelectedIndex);
                        if (preCutTextRun != null)
                        {
                            output.AddLast(preCutTextRun);
                        }
                    }
                }
                else
                {
                    int startLineId = startPoint.LineId;
                    int stopLineId = endPoint.LineId;
                    startLine.RightCopy(startPoint, output);
                    for (int i = startLineId + 1; i < stopLineId; i++)
                    {
                        output.AddLast(new ArtEditableVisualTextRun('\n'));
                        EditableVisualElementLine line = editableFlowLayer.GetTextLine(i);
                        line.Copy(output);
                    }
                    stopLine.LeftCopy(endPoint, output);
                }
            }
        }

        internal void Remove(VisualSelectionRange selectionRange, VisualElementArgs vinv)
        {
            EditableVisualPointInfo startPoint = selectionRange.StartPoint;
            EditableVisualPointInfo endPoint = selectionRange.EndPoint;

            if (startPoint.TextRun != null)
            {
                if (startPoint.TextRun == endPoint.TextRun)
                {

                    ArtEditableVisualTextRun removedRun = (ArtEditableVisualTextRun)startPoint.TextRun;
                    ArtEditableVisualTextRun.InnerRemove(removedRun,
    startPoint.LocalSelectedIndex + 1,
endPoint.LineCharIndex - startPoint.LineCharIndex, false, vinv);

                    if (removedRun.CharacterCount == 0)
                    {
                        if (startPoint.LineId == this.currentLineNumber)
                        {
                            this.Remove(removedRun);
                        }
                        else
                        {
                            EditableVisualElementLine line = editableFlowLayer.GetTextLine(startPoint.LineId);
                            line.Remove(removedRun);
                        }
                    }
                }
                else
                {
                    EditableVisualPointInfo newStartPoint = null;
                    EditableVisualPointInfo newStopPoint = null;
                    EditableVisualElementLine startLine = null;
                    EditableVisualElementLine stopLine = null;

                    if (startPoint.LineId == currentLineNumber)
                    {
                        startLine = this;
                    }
                    else
                    {
                        startLine = editableFlowLayer.GetTextLine(startPoint.LineId);
                    }
                    newStartPoint = startLine.Split(startPoint);
                    if (endPoint.LineId == currentLineNumber)
                    {
                        stopLine = this;
                    }
                    else
                    {
                        stopLine = editableFlowLayer.GetTextLine(endPoint.LineId);
                    }

                    newStopPoint = stopLine.Split(endPoint);

                    if (startLine == stopLine)
                    {
                        if (newStartPoint.TextRun != null)
                        {
                            LinkedList<ArtEditableVisualTextRun> tobeRemoveRuns = new LinkedList<ArtEditableVisualTextRun>();

                            if (newStartPoint.LineCharIndex == -1)
                            {
                                foreach (ArtEditableVisualTextRun t in editableFlowLayer.TextRunForward(
                                    (ArtEditableVisualTextRun)newStartPoint.TextRun,
                                    (ArtEditableVisualTextRun)newStopPoint.TextRun))
                                {
                                    tobeRemoveRuns.AddLast(t);
                                }
                            }
                            else
                            {
                                foreach (ArtEditableVisualTextRun t in editableFlowLayer.TextRunForward(
                                     newStartPoint.TextRun.NextTextRun,
                                    (ArtEditableVisualTextRun)newStopPoint.TextRun))
                                {
                                    tobeRemoveRuns.AddLast(t);
                                }
                            }
                            startLine.LocalSuspendLineReArrange();
                            foreach (ArtEditableVisualTextRun t in tobeRemoveRuns)
                            {
                                startLine.Remove(t);
                            }
                            startLine.LocalResumeLineReArrange();


                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }
                    else
                    {
                        int startLineId = newStartPoint.LineId;
                        int stopLineId = newStopPoint.LineId;

                        if (newStopPoint.LineCharIndex > -1)
                        {
                            stopLine.RemoveLeft((ArtEditableVisualTextRun)newStopPoint.TextRun);
                        }
                        for (int i = stopLineId - 1; i > startLineId; i--)
                        {
                            EditableVisualElementLine line = editableFlowLayer.GetTextLine(i);
                            line.Clear();
                            line.JoinWithNextLine();
                        }
                        if (newStartPoint.LineCharIndex == -1)
                        {
                            startLine.RemoveRight((ArtEditableVisualTextRun)newStartPoint.TextRun);
                        }
                        else
                        {
                            ArtEditableVisualTextRun nextRun = ((ArtEditableVisualTextRun)newStartPoint.TextRun).NextTextRun;
                            if (nextRun != null && !nextRun.IsLineBreak)
                            {
                                startLine.RemoveRight(nextRun);
                            }
                        }
                        startLine.JoinWithNextLine();
                    }
                }
            }
            else
            {
                VisualPointInfo newStartPoint = null;
                VisualPointInfo newStopPoint = null;
                EditableVisualElementLine startLine = null;
                EditableVisualElementLine stopLine = null;
                if (startPoint.LineId == this.currentLineNumber)
                {
                    startLine = this;
                }
                else
                {
                    startLine = editableFlowLayer.GetTextLine(startPoint.LineId);
                }
                newStartPoint = startLine.Split(startPoint);
                if (endPoint.LineId == currentLineNumber)
                {
                    stopLine = this;
                }
                else
                {
                    stopLine = editableFlowLayer.GetTextLine(endPoint.LineId);
                }
                newStopPoint = stopLine.Split(endPoint);

                if (startLine == stopLine)
                {
                    if (newStartPoint.TextRun != null)
                    {
                        LinkedList<ArtEditableVisualTextRun> tobeRemoveRuns = new LinkedList<ArtEditableVisualTextRun>();

                        if (newStartPoint.LineCharIndex == -1)
                        {
                            foreach (ArtEditableVisualTextRun t in editableFlowLayer.TextRunForward(
                                (ArtEditableVisualTextRun)newStartPoint.TextRun,
                                (ArtEditableVisualTextRun)newStopPoint.TextRun))
                            {
                                tobeRemoveRuns.AddLast(t);
                            }
                        }
                        else
                        {
                            foreach (ArtEditableVisualTextRun t in editableFlowLayer.TextRunForward(
                                newStartPoint.TextRun.NextTextRun,
                                (ArtEditableVisualTextRun)newStopPoint.TextRun))
                            {
                                tobeRemoveRuns.AddLast(t);
                            }
                        }
                        foreach (ArtEditableVisualTextRun t in tobeRemoveRuns)
                        {
                            startLine.Remove(t);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException();

                    }
                }
                else
                {

                    int startLineId = newStartPoint.LineId;
                    int stopLineId = newStopPoint.LineId;

                    if (newStopPoint.LineCharIndex > -1)
                    {
                        stopLine.RemoveLeft((ArtEditableVisualTextRun)newStopPoint.TextRun);
                    }
                    for (int i = stopLineId - 1; i > startLineId; i--)
                    {
                        EditableVisualElementLine line = editableFlowLayer.GetTextLine(i);
                        line.Clear();
                        line.JoinWithNextLine();
                    }
                    if (newStartPoint.LineCharIndex == -1)
                    {
                        if (newStartPoint.TextRun != null)
                        {
                            startLine.RemoveRight((ArtEditableVisualTextRun)newStartPoint.TextRun);
                        }
                    }
                    else
                    {
                        ArtEditableVisualTextRun nextRun = newStartPoint.TextRun.NextTextRun;
                        if (nextRun != null && !nextRun.IsLineBreak)
                        {
                            startLine.RemoveRight(nextRun);
                        }
                    }
                    startLine.JoinWithNextLine();
                }
            }

        }

        ArtEditableVisualTextRun GetPrevTextRun(ArtEditableVisualTextRun run)
        {
            if (IsSingleLine)
            {
                return run.PrevTextRun;
            }
            else
            {
                if (IsFirstLine)
                {
                    return run.PrevTextRun;
                }
                else
                {
                    ArtEditableVisualTextRun prevTextRun = run.PrevTextRun;
                    if (prevTextRun.IsLineBreak)
                    {
                        return null;
                    }
                    else
                    {
                        return prevTextRun;
                    }
                }
            }
        }
        ArtEditableVisualTextRun GetNextTextRun(ArtEditableVisualTextRun run)
        {
            if (IsSingleLine)
            {
                return run.NextTextRun;
            }
            else
            {
                if (IsLastLine)
                {
                    return run.NextTextRun;
                }
                else
                {
                    ArtEditableVisualTextRun nextTextRun = run.NextTextRun;
                    if (nextTextRun.IsLineBreak)
                    {
                        return null;
                    }
                    else
                    {
                        return nextTextRun;
                    }
                }
            }
        }
        internal EditableVisualPointInfo[] Split(VisualSelectionRange selectionRange)
        {
            selectionRange.SwapIfUnOrder();
            EditableVisualPointInfo startPoint = selectionRange.StartPoint;
            EditableVisualPointInfo endPoint = selectionRange.EndPoint;
            if (startPoint.TextRun == endPoint.TextRun)
            {
                ArtEditableVisualTextRun toBeCutTextRun = startPoint.TextRun;
                ArtEditableVisualTextRun preCutTextRun = (ArtEditableVisualTextRun)toBeCutTextRun.LeftCopy(startPoint.LocalSelectedIndex);
                ArtEditableVisualTextRun middleCutTextRun = (ArtEditableVisualTextRun)toBeCutTextRun.Copy(startPoint.LocalSelectedIndex + 1, endPoint.LineCharIndex - startPoint.LineCharIndex);
                ArtEditableVisualTextRun postCutTextRun = (ArtEditableVisualTextRun)toBeCutTextRun.Copy(endPoint.LocalSelectedIndex + 1);


                EditableVisualPointInfo newStartRangePointInfo = null;
                EditableVisualPointInfo newEndRangePointInfo = null;

                EditableVisualElementLine line = this;
                if (startPoint.LineId != currentLineNumber)
                {
                    line = editableFlowLayer.GetTextLine(startPoint.LineId);
                }
                line.LocalSuspendLineReArrange();

                if (preCutTextRun != null)
                {
                    line.AddBefore(toBeCutTextRun, preCutTextRun);
                    newStartRangePointInfo = CreateTextPointInfo(
                        startPoint.LineId, startPoint.LineCharIndex, startPoint.X,
                        preCutTextRun, startPoint.TextRunCharOffset, startPoint.TextRunPixelOffset);
                }
                else
                {

                    ArtEditableVisualTextRun prevTxtRun = GetPrevTextRun((ArtEditableVisualTextRun)startPoint.TextRun);
                    if (prevTxtRun != null)
                    {
                        newStartRangePointInfo = CreateTextPointInfo(
                            startPoint.LineId, startPoint.LineCharIndex, startPoint.X, prevTxtRun, startPoint.TextRunCharOffset - preCutTextRun.CharacterCount,
                            startPoint.TextRunPixelOffset - prevTxtRun.Width);
                    }
                    else
                    {
                        newStartRangePointInfo = CreateTextPointInfo(
                            startPoint.LineId,
                            startPoint.LineCharIndex,
                            0,
                            null,
                            0, 0);
                    }
                }

                if (postCutTextRun != null)
                {
                    line.AddAfter(toBeCutTextRun, postCutTextRun);
                    newEndRangePointInfo =
                        CreateTextPointInfo(
                            endPoint.LineId,
                            endPoint.LineCharIndex,
                            endPoint.X,
                            middleCutTextRun,
                            startPoint.TextRunCharOffset + middleCutTextRun.CharacterCount,
                            startPoint.TextRunPixelOffset + middleCutTextRun.Width);
                }
                else
                {
                    ArtEditableVisualTextRun nextTxtRun = GetNextTextRun((ArtEditableVisualTextRun)endPoint.TextRun);
                    if (nextTxtRun != null)
                    {
                        newEndRangePointInfo = CreateTextPointInfo(
                            endPoint.LineId,
                            endPoint.LineCharIndex,
                            endPoint.X,
                            nextTxtRun,
                            endPoint.TextRunPixelOffset + endPoint.TextRun.CharacterCount,
                            endPoint.TextRunPixelOffset + endPoint.TextRun.Width);
                    }
                    else
                    {
                        newEndRangePointInfo = CreateTextPointInfo(
                            endPoint.LineId,
                            endPoint.LineCharIndex,
                            endPoint.X,
                            middleCutTextRun,
                            endPoint.TextRunCharOffset,
                            endPoint.TextRunPixelOffset);
                    }
                }

                if (middleCutTextRun != null)
                {
                    line.AddAfter(toBeCutTextRun, middleCutTextRun);
                }
                else
                {
                    throw new NotSupportedException();
                }
                line.Remove(toBeCutTextRun);

                line.LocalResumeLineReArrange();

                return new EditableVisualPointInfo[] { newStartRangePointInfo, newEndRangePointInfo };
            }
            else
            {
                EditableVisualElementLine workingLine = this;
                if (startPoint.LineId != currentLineNumber)
                {
                    workingLine = editableFlowLayer.GetTextLine(startPoint.LineId);
                }
                EditableVisualPointInfo newStartPoint = workingLine.Split(startPoint);
                workingLine = this;
                if (endPoint.LineId != currentLineNumber)
                {
                    workingLine = editableFlowLayer.GetTextLine(endPoint.LineId);
                }
                EditableVisualPointInfo newEndPoint = workingLine.Split(endPoint);
                return new EditableVisualPointInfo[] { newStartPoint, newEndPoint };
            }
        }

        internal EditableVisualPointInfo Split(EditableVisualPointInfo pointInfo)
        {
            if (pointInfo.LineId != currentLineNumber)
            {
                throw new NotSupportedException();
            }

            ArtEditableVisualTextRun tobeCutRun = (ArtEditableVisualTextRun)pointInfo.TextRun;
            if (tobeCutRun == null)
            {
                return CreateTextPointInfo(
   pointInfo.LineId,
   pointInfo.LineCharIndex,
   pointInfo.X,
   null,
   pointInfo.TextRunCharOffset,
   pointInfo.TextRunPixelOffset);

            }

            this.LocalSuspendLineReArrange();
            EditableVisualPointInfo result = null;
            ArtEditableVisualTextRun preCutTextRun = (ArtEditableVisualTextRun)tobeCutRun.LeftCopy(pointInfo.LocalSelectedIndex);
            ArtEditableVisualTextRun postCutTextRun = (ArtEditableVisualTextRun)tobeCutRun.Copy(pointInfo.LocalSelectedIndex + 1);


            if (preCutTextRun != null)
            {
                this.AddBefore(tobeCutRun, preCutTextRun);
                if (postCutTextRun != null)
                {
                    this.AddAfter(tobeCutRun, postCutTextRun);
                }

                result = CreateTextPointInfo(
                    pointInfo.LineId,
                    pointInfo.LineCharIndex,
                    pointInfo.X,
                    preCutTextRun,
                    pointInfo.TextRunCharOffset,
                    pointInfo.TextRunPixelOffset);

            }
            else
            {
                if (postCutTextRun != null)
                {
                    this.AddAfter(tobeCutRun, postCutTextRun);
                }
                ArtEditableVisualTextRun infoTextRun = null;
                if (IsSingleLine)
                {
                    if (tobeCutRun.PrevTextRun != null)
                    {
                        infoTextRun = tobeCutRun.PrevTextRun;
                    }
                    else
                    {
                        infoTextRun = tobeCutRun.NextTextRun;
                    }
                }
                else
                {
                    if (IsFirstLine)
                    {
                        if (tobeCutRun.PrevTextRun != null)
                        {
                            infoTextRun = tobeCutRun.PrevTextRun;
                        }
                        else
                        {
                            if (tobeCutRun.NextTextRun.IsLineBreak)
                            {
                                infoTextRun = null;
                            }
                            else
                            {
                                infoTextRun = tobeCutRun.NextTextRun;
                            }
                        }
                    }
                    else if (IsLastLine)
                    {
                        if (tobeCutRun.PrevTextRun.IsLineBreak)
                        {
                            if (tobeCutRun.NextTextRun != null)
                            {
                                infoTextRun = tobeCutRun.NextTextRun;
                            }
                            else
                            {
                                infoTextRun = null;
                            }
                        }
                        else
                        {
                            infoTextRun = tobeCutRun.PrevTextRun;
                        }
                    }
                    else
                    {
                        if (!tobeCutRun.NextTextRun.IsLineBreak)
                        {
                            infoTextRun = tobeCutRun.NextTextRun;
                        }
                        else
                        {
                            infoTextRun = null;
                        }
                    }
                }
                result = CreateTextPointInfo(
                    pointInfo.LineId,
                    pointInfo.LineCharIndex,
                    pointInfo.X,
                    infoTextRun,
                    pointInfo.TextRunCharOffset,
                    pointInfo.TextRunPixelOffset);

            }

            this.Remove(tobeCutRun);
            this.LocalResumeLineReArrange();

            return result;
        }
        void RightCopy(VisualPointInfo pointInfo, LinkedList<ArtEditableVisualTextRun> output)
        {

            if (pointInfo.LineId != currentLineNumber)
            {
                throw new NotSupportedException();
            }
            ArtEditableVisualTextRun tobeCutRun = pointInfo.TextRun;
            if (tobeCutRun == null)
            {
                return;
            }
            ArtEditableVisualTextRun postCutTextRun = (ArtEditableVisualTextRun)tobeCutRun.Copy(pointInfo.LocalSelectedIndex + 1);
            if (postCutTextRun != null)
            {
                output.AddLast(postCutTextRun);
            }
            foreach (ArtEditableVisualTextRun t in GetVisualElementForward(tobeCutRun.NextTextRun, this.LastRun))
            {
                output.AddLast(t.Clone());
            }
        }

        void LeftCopy(VisualPointInfo pointInfo, LinkedList<ArtEditableVisualTextRun> output)
        {


            if (pointInfo.LineId != currentLineNumber)
            {
                throw new NotSupportedException();
            }
            ArtEditableVisualTextRun tobeCutRun = pointInfo.TextRun;
            if (tobeCutRun == null)
            {
                return;
            }

            foreach (ArtEditableVisualTextRun t in this)
            {
                if (t != tobeCutRun)
                {
                    output.AddLast(t.Clone());
                }
                else
                {
                    break;
                }
            }
            ArtEditableVisualTextRun preCutTextRun = tobeCutRun.LeftCopy(pointInfo.LocalSelectedIndex);
            if (preCutTextRun != null)
            {
                output.AddLast(preCutTextRun);
            }

        }



        EditableVisualPointInfo CreateTextPointInfo(int lineId, int lineCharIndex, int caretPixelX,
ArtEditableVisualTextRun onTextRun, int textRunCharOffset, int textRunPixelOffset)
        {
            EditableVisualPointInfo textPointInfo = new EditableVisualPointInfo(this, lineCharIndex);
            textPointInfo.SetAdditionVisualInfo(onTextRun, textRunCharOffset, caretPixelX, textRunPixelOffset);
            return textPointInfo;
        }
        public VisualPointInfo GetTextPointInfoFromCaretPoint(int caretX)
        {
            int accTextRunWidth = 0; int accTextRunCharCount = 0;
            ArtEditableVisualTextRun lastestTextRun = null;
            foreach (ArtEditableVisualTextRun t in this)
            {
                lastestTextRun = t;
                int thisTextRunWidth = t.Width;
                if (accTextRunWidth + thisTextRunWidth > caretX)
                {
                    VisualLocationInfo localPointInfo = t.GetCharacterFromPixelOffset(caretX - thisTextRunWidth);
                    EditableVisualPointInfo pointInfo =
                        new EditableVisualPointInfo(this, accTextRunCharCount + localPointInfo.charIndex);
                    pointInfo.SetAdditionVisualInfo(t, accTextRunCharCount, caretX, accTextRunWidth);
                    return pointInfo;
                }
                else
                {
                    accTextRunWidth += thisTextRunWidth;
                    accTextRunCharCount += t.CharacterCount;
                }
            }
            if (lastestTextRun != null)
            {
                return null;
            }
            else
            {
                EditableVisualPointInfo pInfo = new EditableVisualPointInfo(this, -1);
                pInfo.SetAdditionVisualInfo(null, accTextRunCharCount, caretX, accTextRunWidth);
                return pInfo;
            }
        }

        public EditableVisualPointInfo GetTextPointInfoFromCharIndex(int charIndex)
        {

            int limit = CharCount - 1;
            if (charIndex > limit)
            {
                charIndex = limit;
            }

            EditableVisualPointInfo textPointInfo = new EditableVisualPointInfo(this, charIndex);
            int rCharOffset = 0;
            int rPixelOffset = 0;
            ArtEditableVisualTextRun lastestRun = null;
            foreach (ArtEditableVisualTextRun r in this)
            {

                lastestRun = r;
                int thisCharCount = lastestRun.CharacterCount;
                if (thisCharCount + rCharOffset > charIndex)
                {

                    int localCharOffset = charIndex - rCharOffset;

                    int pixelOffset = lastestRun.GetRunWidth(localCharOffset);

                    textPointInfo.SetAdditionVisualInfo(lastestRun,
                        localCharOffset, rPixelOffset + pixelOffset
                        , rPixelOffset);

                    return textPointInfo;
                }
                else
                {
                    rCharOffset += thisCharCount;
                    rPixelOffset += r.Width;
                }
            }
            textPointInfo.SetAdditionVisualInfo(lastestRun, rCharOffset - lastestRun.CharacterCount, rPixelOffset, rPixelOffset - lastestRun.Width);
            return textPointInfo;
        }


        internal EditableVisualElementLine SplitToNewLine(ArtEditableVisualTextRun startVisualElement)
        {

            LinkedListNode<ArtEditableVisualTextRun> curNode = GetLineLinkedNode(startVisualElement);
            EditableVisualElementLine newSplitedLine = editableFlowLayer.InsertNewLine(this.currentLineNumber + 1);
            newSplitedLine.LocalSuspendLineReArrange();
            while (curNode != null)
            {
                LinkedListNode<ArtEditableVisualTextRun> tobeRemovedNode = curNode;
                curNode = curNode.Next;
                if (tobeRemovedNode.List != null)
                {
                    ArtEditableVisualTextRun tmpv = tobeRemovedNode.Value;
                    base.Remove(tobeRemovedNode);
                    newSplitedLine.AddLast(tmpv);
                }
                else
                {

                }
            }
            newSplitedLine.LocalResumeLineReArrange();
            return newSplitedLine;
        }
    }

}