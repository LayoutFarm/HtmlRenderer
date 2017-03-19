//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.Text
{
    partial class EditableTextLine
    {
        public static void InnerDoJoinWithNextLine(EditableTextLine line)
        {
            line.JoinWithNextLine();
        }
        internal void JoinWithNextLine()
        {
            if (!IsLastLine)
            {
                EditableTextLine lowerLine = editableFlowLayer.GetTextLine(currentLineNumber + 1);
                this.LocalSuspendLineReArrange();
                int cx = 0;
                EditableRun lastTextRun = (EditableRun)this.LastRun;
                if (lastTextRun != null)
                {
                    cx = lastTextRun.Right;
                }

                foreach (EditableRun r in lowerLine)
                {
                    this.AddLast(r);
                    EditableRun.DirectSetLocation(r, cx, 0);
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
        public void Copy(LinkedList<EditableRun> output)
        {
            LinkedListNode<EditableRun> curNode = this.First;
            while (curNode != null)
            {
                output.AddLast(curNode.Value.Clone());
                curNode = curNode.Next;
            }
        }

        public void Copy(VisualSelectionRange selectionRange, LinkedList<EditableRun> output)
        {
            EditableVisualPointInfo startPoint = selectionRange.StartPoint;
            EditableVisualPointInfo endPoint = selectionRange.EndPoint;
            if (startPoint.TextRun != null)
            {
                if (startPoint.TextRun == endPoint.TextRun)
                {
                    EditableRun elem =
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
                    EditableTextLine startLine = null;
                    EditableTextLine stopLine = null;
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
                        EditableRun postCutTextRun = startPoint.TextRun.Copy(startPoint.LocalSelectedIndex + 1);
                        if (postCutTextRun != null)
                        {
                            output.AddLast(postCutTextRun);
                        }
                        if (startPoint.TextRun.NextTextRun != endPoint.TextRun)
                        {
                            foreach (EditableRun t in editableFlowLayer.TextRunForward(startPoint.TextRun.NextTextRun, endPoint.TextRun.PrevTextRun))
                            {
                                output.AddLast(t.Clone());
                            }
                        }

                        EditableRun preCutTextRun = endPoint.TextRun.LeftCopy(endPoint.LocalSelectedIndex);
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
                            output.AddLast(new EditableTextRun(this.Root, '\n', this.CurrentTextSpanStyle));
                            EditableTextLine line = editableFlowLayer.GetTextLine(i);
                            line.Copy(output);
                        }
                        if (endPoint.LineCharIndex > -1)
                        {
                            output.AddLast(new EditableTextRun(this.Root, '\n', this.CurrentTextSpanStyle));
                            stopLine.LeftCopy(endPoint, output);
                        }
                    }
                }
            }
            else
            {
                EditableTextLine startLine = null;
                EditableTextLine stopLine = null;
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
                        foreach (EditableRun t in editableFlowLayer.TextRunForward(startPoint.TextRun, endPoint.TextRun.PrevTextRun))
                        {
                            output.AddLast(t.Clone());
                        }
                        EditableRun postCutTextRun = endPoint.TextRun.Copy(endPoint.LocalSelectedIndex + 1);
                        if (postCutTextRun != null)
                        {
                            output.AddLast(postCutTextRun);
                        }
                    }
                    else
                    {
                        EditableRun postCutTextRun = startPoint.TextRun.Copy(startPoint.LocalSelectedIndex + 1);
                        if (postCutTextRun != null)
                        {
                            output.AddLast(postCutTextRun);
                        }

                        foreach (EditableRun t in editableFlowLayer.TextRunForward(startPoint.TextRun.NextTextRun, endPoint.TextRun.PrevTextRun))
                        {
                            output.AddLast(t.Clone());
                        }

                        EditableRun preCutTextRun = endPoint.TextRun.LeftCopy(startPoint.LocalSelectedIndex);
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
                        output.AddLast(new EditableTextRun(this.Root, '\n', this.CurrentTextSpanStyle));
                        EditableTextLine line = editableFlowLayer.GetTextLine(i);
                        line.Copy(output);
                    }
                    stopLine.LeftCopy(endPoint, output);
                }
            }
        }
        internal TextSpanStyle CurrentTextSpanStyle
        {
            get
            {
                return this.OwnerFlowLayer.CurrentTextSpanStyle;
            }
        }
        internal void Remove(VisualSelectionRange selectionRange)
        {
            EditableVisualPointInfo startPoint = selectionRange.StartPoint;
            EditableVisualPointInfo endPoint = selectionRange.EndPoint;
            if (startPoint.TextRun != null)
            {
                if (startPoint.TextRun == endPoint.TextRun)
                {
                    EditableRun removedRun = (EditableRun)startPoint.TextRun;
                    EditableRun.InnerRemove(removedRun,
                                    startPoint.LocalSelectedIndex + 1,
                                    endPoint.LineCharIndex - startPoint.LineCharIndex, false);
                    if (removedRun.CharacterCount == 0)
                    {
                        if (startPoint.LineId == this.currentLineNumber)
                        {
                            this.Remove(removedRun);
                        }
                        else
                        {
                            EditableTextLine line = editableFlowLayer.GetTextLine(startPoint.LineId);
                            line.Remove(removedRun);
                        }
                    }
                }
                else
                {
                    EditableVisualPointInfo newStartPoint = null;
                    EditableVisualPointInfo newStopPoint = null;
                    EditableTextLine startLine = null;
                    EditableTextLine stopLine = null;
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
                            LinkedList<EditableRun> tobeRemoveRuns = new LinkedList<EditableRun>();
                            if (newStartPoint.LineCharIndex == -1)
                            {
                                foreach (EditableRun t in editableFlowLayer.TextRunForward(
                                    (EditableRun)newStartPoint.TextRun,
                                    (EditableRun)newStopPoint.TextRun))
                                {
                                    tobeRemoveRuns.AddLast(t);
                                }
                            }
                            else
                            {
                                foreach (EditableRun t in editableFlowLayer.TextRunForward(
                                     newStartPoint.TextRun.NextTextRun,
                                    (EditableRun)newStopPoint.TextRun))
                                {
                                    tobeRemoveRuns.AddLast(t);
                                }
                            }
                            startLine.LocalSuspendLineReArrange();
                            foreach (EditableRun t in tobeRemoveRuns)
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
                            stopLine.RemoveLeft((EditableRun)newStopPoint.TextRun);
                        }
                        for (int i = stopLineId - 1; i > startLineId; i--)
                        {
                            EditableTextLine line = editableFlowLayer.GetTextLine(i);
                            line.Clear();
                            line.JoinWithNextLine();
                        }
                        if (newStartPoint.LineCharIndex == -1)
                        {
                            startLine.RemoveRight((EditableRun)newStartPoint.TextRun);
                        }
                        else
                        {
                            EditableRun nextRun = ((EditableRun)newStartPoint.TextRun).NextTextRun;
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
                EditableTextLine startLine = null;
                EditableTextLine stopLine = null;
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
                        LinkedList<EditableRun> tobeRemoveRuns = new LinkedList<EditableRun>();
                        if (newStartPoint.LineCharIndex == -1)
                        {
                            foreach (EditableRun t in editableFlowLayer.TextRunForward(
                                (EditableRun)newStartPoint.TextRun,
                                (EditableRun)newStopPoint.TextRun))
                            {
                                tobeRemoveRuns.AddLast(t);
                            }
                        }
                        else
                        {
                            foreach (EditableRun t in editableFlowLayer.TextRunForward(
                                newStartPoint.TextRun.NextTextRun,
                                (EditableRun)newStopPoint.TextRun))
                            {
                                tobeRemoveRuns.AddLast(t);
                            }
                        }
                        foreach (EditableRun t in tobeRemoveRuns)
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
                        stopLine.RemoveLeft((EditableRun)newStopPoint.TextRun);
                    }
                    for (int i = stopLineId - 1; i > startLineId; i--)
                    {
                        EditableTextLine line = editableFlowLayer.GetTextLine(i);
                        line.Clear();
                        line.JoinWithNextLine();
                    }
                    if (newStartPoint.LineCharIndex == -1)
                    {
                        if (newStartPoint.TextRun != null)
                        {
                            startLine.RemoveRight((EditableRun)newStartPoint.TextRun);
                        }
                    }
                    else
                    {
                        EditableRun nextRun = newStartPoint.TextRun.NextTextRun;
                        if (nextRun != null && !nextRun.IsLineBreak)
                        {
                            startLine.RemoveRight(nextRun);
                        }
                    }
                    startLine.JoinWithNextLine();
                }
            }
        }

        EditableRun GetPrevTextRun(EditableRun run)
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
                    EditableRun prevTextRun = run.PrevTextRun;
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
        EditableRun GetNextTextRun(EditableRun run)
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
                    EditableRun nextTextRun = run.NextTextRun;
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
                EditableRun toBeCutTextRun = startPoint.TextRun;
                EditableRun preCutTextRun = (EditableRun)toBeCutTextRun.LeftCopy(startPoint.LocalSelectedIndex);
                EditableRun middleCutTextRun = (EditableRun)toBeCutTextRun.Copy(startPoint.LocalSelectedIndex + 1, endPoint.LineCharIndex - startPoint.LineCharIndex);
                EditableRun postCutTextRun = (EditableRun)toBeCutTextRun.Copy(endPoint.LocalSelectedIndex + 1);
                EditableVisualPointInfo newStartRangePointInfo = null;
                EditableVisualPointInfo newEndRangePointInfo = null;
                EditableTextLine line = this;
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
                    EditableRun prevTxtRun = GetPrevTextRun((EditableRun)startPoint.TextRun);
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
                    EditableRun nextTxtRun = GetNextTextRun((EditableRun)endPoint.TextRun);
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
                EditableTextLine workingLine = this;
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

            EditableRun tobeCutRun = (EditableRun)pointInfo.TextRun;
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
            EditableRun preCutTextRun = (EditableRun)tobeCutRun.LeftCopy(pointInfo.LocalSelectedIndex);
            EditableRun postCutTextRun = (EditableRun)tobeCutRun.Copy(pointInfo.LocalSelectedIndex + 1);
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
                EditableRun infoTextRun = null;
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
        void RightCopy(VisualPointInfo pointInfo, LinkedList<EditableRun> output)
        {
            if (pointInfo.LineId != currentLineNumber)
            {
                throw new NotSupportedException();
            }
            EditableRun tobeCutRun = pointInfo.TextRun;
            if (tobeCutRun == null)
            {
                return;
            }
            EditableRun postCutTextRun = (EditableRun)tobeCutRun.Copy(pointInfo.LocalSelectedIndex + 1);
            if (postCutTextRun != null)
            {
                output.AddLast(postCutTextRun);
            }
            foreach (EditableRun t in GetVisualElementForward(tobeCutRun.NextTextRun, this.LastRun))
            {
                output.AddLast(t.Clone());
            }
        }

        void LeftCopy(VisualPointInfo pointInfo, LinkedList<EditableRun> output)
        {
            if (pointInfo.LineId != currentLineNumber)
            {
                throw new NotSupportedException();
            }
            EditableRun tobeCutRun = pointInfo.TextRun;
            if (tobeCutRun == null)
            {
                return;
            }

            foreach (EditableRun t in this)
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
            EditableRun preCutTextRun = tobeCutRun.LeftCopy(pointInfo.LocalSelectedIndex);
            if (preCutTextRun != null)
            {
                output.AddLast(preCutTextRun);
            }
        }



        EditableVisualPointInfo CreateTextPointInfo(int lineId, int lineCharIndex, int caretPixelX,
            EditableRun onTextRun, int textRunCharOffset, int textRunPixelOffset)
        {
            EditableVisualPointInfo textPointInfo = new EditableVisualPointInfo(this, lineCharIndex);
            textPointInfo.SetAdditionVisualInfo(onTextRun, textRunCharOffset, caretPixelX, textRunPixelOffset);
            return textPointInfo;
        }
        public VisualPointInfo GetTextPointInfoFromCaretPoint(int caretX)
        {
            int accTextRunWidth = 0; int accTextRunCharCount = 0;
            EditableRun lastestTextRun = null;
            foreach (EditableRun t in this)
            {
                lastestTextRun = t;
                int thisTextRunWidth = t.Width;
                if (accTextRunWidth + thisTextRunWidth > caretX)
                {
                    EditableRunCharLocation localPointInfo = t.GetCharacterFromPixelOffset(caretX - thisTextRunWidth);
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
            EditableRun lastestRun = null;
            foreach (EditableRun r in this)
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


        internal EditableTextLine SplitToNewLine(EditableRun startVisualElement)
        {
            LinkedListNode<EditableRun> curNode = GetLineLinkedNode(startVisualElement);
            EditableTextLine newSplitedLine = editableFlowLayer.InsertNewLine(this.currentLineNumber + 1);
            newSplitedLine.LocalSuspendLineReArrange();
            while (curNode != null)
            {
                LinkedListNode<EditableRun> tobeRemovedNode = curNode;
                curNode = curNode.Next;
                if (tobeRemovedNode.List != null)
                {
                    EditableRun tmpv = tobeRemovedNode.Value;
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