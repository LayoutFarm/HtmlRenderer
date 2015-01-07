//2014,2015 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;


namespace LayoutFarm.Text
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
                EditableTextSpan lastTextRun = (EditableTextSpan)this.LastRun;
                if (lastTextRun != null)
                {
                    cx = lastTextRun.Right;
                }

                foreach (EditableTextSpan r in lowerLine)
                {
                    this.AddLast(r);

                    EditableTextSpan.DirectSetVisualElementLocation(r, cx, 0);
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
        public void Copy(LinkedList<EditableTextSpan> output)
        {

            LinkedListNode<EditableTextSpan> curNode = this.First;
            while (curNode != null)
            {
                output.AddLast(curNode.Value.Clone());
                curNode = curNode.Next;
            } 
        }
        
        public void Copy(VisualSelectionRange selectionRange, LinkedList<EditableTextSpan> output)
        {
            EditableVisualPointInfo startPoint = selectionRange.StartPoint;
            EditableVisualPointInfo endPoint = selectionRange.EndPoint;
            if (startPoint.TextRun != null)
            {
                if (startPoint.TextRun == endPoint.TextRun)
                {

                    EditableTextSpan elem =
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
                        EditableTextSpan postCutTextRun = startPoint.TextRun.Copy(startPoint.LocalSelectedIndex + 1);
                        if (postCutTextRun != null)
                        {
                            output.AddLast(postCutTextRun);
                        }
                        if (startPoint.TextRun.NextTextRun != endPoint.TextRun)
                        {
                            foreach (EditableTextSpan t in editableFlowLayer.TextRunForward(startPoint.TextRun.NextTextRun, endPoint.TextRun.PrevTextRun))
                            {
                                output.AddLast(new EditableTextSpan(this.Root, t.Text));
                            }
                        }

                        EditableTextSpan preCutTextRun = endPoint.TextRun.LeftCopy(endPoint.LocalSelectedIndex);
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
                            output.AddLast(new EditableTextSpan(this.Root, '\n'));
                            EditableVisualElementLine line = editableFlowLayer.GetTextLine(i);
                            line.Copy(output);
                        }
                        if (endPoint.LineCharIndex > -1)
                        {
                            output.AddLast(new EditableTextSpan(this.Root, '\n'));
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

                        foreach (EditableTextSpan t in editableFlowLayer.TextRunForward(startPoint.TextRun, endPoint.TextRun.PrevTextRun))
                        {
                            output.AddLast(t.Clone());
                        }
                        EditableTextSpan postCutTextRun = endPoint.TextRun.Copy(endPoint.LocalSelectedIndex + 1);
                        if (postCutTextRun != null)
                        {
                            output.AddLast(postCutTextRun);
                        }

                    }
                    else
                    {

                        EditableTextSpan postCutTextRun = startPoint.TextRun.Copy(startPoint.LocalSelectedIndex + 1);

                        if (postCutTextRun != null)
                        {
                            output.AddLast(postCutTextRun);
                        }

                        foreach (EditableTextSpan t in editableFlowLayer.TextRunForward(startPoint.TextRun.NextTextRun, endPoint.TextRun.PrevTextRun))
                        {
                            output.AddLast(t.Clone());
                        }

                        EditableTextSpan preCutTextRun = endPoint.TextRun.LeftCopy(startPoint.LocalSelectedIndex);
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
                        output.AddLast(new EditableTextSpan(this.Root, '\n'));
                        EditableVisualElementLine line = editableFlowLayer.GetTextLine(i);
                        line.Copy(output);
                    }
                    stopLine.LeftCopy(endPoint, output);
                }
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

                    EditableTextSpan removedRun = (EditableTextSpan)startPoint.TextRun;
                    EditableTextSpan.InnerRemove(removedRun,
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
                            LinkedList<EditableTextSpan> tobeRemoveRuns = new LinkedList<EditableTextSpan>();

                            if (newStartPoint.LineCharIndex == -1)
                            {
                                foreach (EditableTextSpan t in editableFlowLayer.TextRunForward(
                                    (EditableTextSpan)newStartPoint.TextRun,
                                    (EditableTextSpan)newStopPoint.TextRun))
                                {
                                    tobeRemoveRuns.AddLast(t);
                                }
                            }
                            else
                            {
                                foreach (EditableTextSpan t in editableFlowLayer.TextRunForward(
                                     newStartPoint.TextRun.NextTextRun,
                                    (EditableTextSpan)newStopPoint.TextRun))
                                {
                                    tobeRemoveRuns.AddLast(t);
                                }
                            }
                            startLine.LocalSuspendLineReArrange();
                            foreach (EditableTextSpan t in tobeRemoveRuns)
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
                            stopLine.RemoveLeft((EditableTextSpan)newStopPoint.TextRun);
                        }
                        for (int i = stopLineId - 1; i > startLineId; i--)
                        {
                            EditableVisualElementLine line = editableFlowLayer.GetTextLine(i);
                            line.Clear();
                            line.JoinWithNextLine();
                        }
                        if (newStartPoint.LineCharIndex == -1)
                        {
                            startLine.RemoveRight((EditableTextSpan)newStartPoint.TextRun);
                        }
                        else
                        {
                            EditableTextSpan nextRun = ((EditableTextSpan)newStartPoint.TextRun).NextTextRun;
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
                        LinkedList<EditableTextSpan> tobeRemoveRuns = new LinkedList<EditableTextSpan>();

                        if (newStartPoint.LineCharIndex == -1)
                        {
                            foreach (EditableTextSpan t in editableFlowLayer.TextRunForward(
                                (EditableTextSpan)newStartPoint.TextRun,
                                (EditableTextSpan)newStopPoint.TextRun))
                            {
                                tobeRemoveRuns.AddLast(t);
                            }
                        }
                        else
                        {
                            foreach (EditableTextSpan t in editableFlowLayer.TextRunForward(
                                newStartPoint.TextRun.NextTextRun,
                                (EditableTextSpan)newStopPoint.TextRun))
                            {
                                tobeRemoveRuns.AddLast(t);
                            }
                        }
                        foreach (EditableTextSpan t in tobeRemoveRuns)
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
                        stopLine.RemoveLeft((EditableTextSpan)newStopPoint.TextRun);
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
                            startLine.RemoveRight((EditableTextSpan)newStartPoint.TextRun);
                        }
                    }
                    else
                    {
                        EditableTextSpan nextRun = newStartPoint.TextRun.NextTextRun;
                        if (nextRun != null && !nextRun.IsLineBreak)
                        {
                            startLine.RemoveRight(nextRun);
                        }
                    }
                    startLine.JoinWithNextLine();
                }
            }

        }

        EditableTextSpan GetPrevTextRun(EditableTextSpan run)
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
                    EditableTextSpan prevTextRun = run.PrevTextRun;
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
        EditableTextSpan GetNextTextRun(EditableTextSpan run)
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
                    EditableTextSpan nextTextRun = run.NextTextRun;
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
                EditableTextSpan toBeCutTextRun = startPoint.TextRun;
                EditableTextSpan preCutTextRun = (EditableTextSpan)toBeCutTextRun.LeftCopy(startPoint.LocalSelectedIndex);
                EditableTextSpan middleCutTextRun = (EditableTextSpan)toBeCutTextRun.Copy(startPoint.LocalSelectedIndex + 1, endPoint.LineCharIndex - startPoint.LineCharIndex);
                EditableTextSpan postCutTextRun = (EditableTextSpan)toBeCutTextRun.Copy(endPoint.LocalSelectedIndex + 1);


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

                    EditableTextSpan prevTxtRun = GetPrevTextRun((EditableTextSpan)startPoint.TextRun);
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
                    EditableTextSpan nextTxtRun = GetNextTextRun((EditableTextSpan)endPoint.TextRun);
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

            EditableTextSpan tobeCutRun = (EditableTextSpan)pointInfo.TextRun;
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
            EditableTextSpan preCutTextRun = (EditableTextSpan)tobeCutRun.LeftCopy(pointInfo.LocalSelectedIndex);
            EditableTextSpan postCutTextRun = (EditableTextSpan)tobeCutRun.Copy(pointInfo.LocalSelectedIndex + 1);


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
                EditableTextSpan infoTextRun = null;
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
        void RightCopy(VisualPointInfo pointInfo, LinkedList<EditableTextSpan> output)
        {

            if (pointInfo.LineId != currentLineNumber)
            {
                throw new NotSupportedException();
            }
            EditableTextSpan tobeCutRun = pointInfo.TextRun;
            if (tobeCutRun == null)
            {
                return;
            }
            EditableTextSpan postCutTextRun = (EditableTextSpan)tobeCutRun.Copy(pointInfo.LocalSelectedIndex + 1);
            if (postCutTextRun != null)
            {
                output.AddLast(postCutTextRun);
            }
            foreach (EditableTextSpan t in GetVisualElementForward(tobeCutRun.NextTextRun, this.LastRun))
            {
                output.AddLast(t.Clone());
            }
        }

        void LeftCopy(VisualPointInfo pointInfo, LinkedList<EditableTextSpan> output)
        {


            if (pointInfo.LineId != currentLineNumber)
            {
                throw new NotSupportedException();
            }
            EditableTextSpan tobeCutRun = pointInfo.TextRun;
            if (tobeCutRun == null)
            {
                return;
            }

            foreach (EditableTextSpan t in this)
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
            EditableTextSpan preCutTextRun = tobeCutRun.LeftCopy(pointInfo.LocalSelectedIndex);
            if (preCutTextRun != null)
            {
                output.AddLast(preCutTextRun);
            }

        }



        EditableVisualPointInfo CreateTextPointInfo(int lineId, int lineCharIndex, int caretPixelX,
EditableTextSpan onTextRun, int textRunCharOffset, int textRunPixelOffset)
        {
            EditableVisualPointInfo textPointInfo = new EditableVisualPointInfo(this, lineCharIndex);
            textPointInfo.SetAdditionVisualInfo(onTextRun, textRunCharOffset, caretPixelX, textRunPixelOffset);
            return textPointInfo;
        }
        public VisualPointInfo GetTextPointInfoFromCaretPoint(int caretX)
        {
            int accTextRunWidth = 0; int accTextRunCharCount = 0;
            EditableTextSpan lastestTextRun = null;
            foreach (EditableTextSpan t in this)
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
            EditableTextSpan lastestRun = null;
            foreach (EditableTextSpan r in this)
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


        internal EditableVisualElementLine SplitToNewLine(EditableTextSpan startVisualElement)
        {

            LinkedListNode<EditableTextSpan> curNode = GetLineLinkedNode(startVisualElement);
            EditableVisualElementLine newSplitedLine = editableFlowLayer.InsertNewLine(this.currentLineNumber + 1);
            newSplitedLine.LocalSuspendLineReArrange();
            while (curNode != null)
            {
                LinkedListNode<EditableTextSpan> tobeRemovedNode = curNode;
                curNode = curNode.Next;
                if (tobeRemovedNode.List != null)
                {
                    EditableTextSpan tmpv = tobeRemovedNode.Value;
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