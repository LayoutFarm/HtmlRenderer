//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LayoutFarm.Presentation.Text
{



    public class TextLineWriter : TextLineReader
    {
        BackGroundTextLineWriter backgroundWriter;
        

        public TextLineWriter(EditableTextFlowLayer textLayer)
            : base(textLayer)
        {
            
        }
        public BackGroundTextLineWriter GetBackgroundWriter()
        {
            if (backgroundWriter == null)
            {
                backgroundWriter = new BackGroundTextLineWriter(this.TextLayer);

#if DEBUG
                backgroundWriter.dbugTextManRecorder = this.dbugTextManRecorder;
#endif
            }
            return backgroundWriter;
        }
        public void Reload(IEnumerable<EditableVisualTextRun> runs)
        {
            this.TextLayer.Reload(runs);
        }

        public void Clear()
        {
            this.MoveToLine(0);
            CurrentLine.Clear();
            EnsureCurrentTextRun();
        }
        public void EnsureCurrentTextRun(int index)
        {
            var run = CurrentTextRun;
            if (run == null || run.IsFreeElement)
            {
                CharIndex = -1;
                if (index != -1)
                {
                    int limit = CurrentLine.CharCount - 1;
                    if (index > limit)
                    {
                        index = limit;
                    }
                    CharIndex = index;
                }
            }
        }
        public void EnsureCurrentTextRun()
        {
            EnsureCurrentTextRun(CharIndex);
        }
        public void RemoveSelectedTextRuns(VisualSelectionRange selectionRange, VisualElementArgs vinv)
        {
            int precutIndex = selectionRange.StartPoint.LineCharIndex; CurrentLine.Remove(selectionRange, vinv);
            EnsureCurrentTextRun(precutIndex);

        }

        public void ReplaceCurrentLine(IEnumerable<EditableVisualTextRun> textRuns)
        {
            int currentCharIndex = CharIndex;
            CurrentLine.ReplaceAll(textRuns);
            EnsureCurrentTextRun(currentCharIndex);
        }

        public void JoinWithNextLine()
        {
            EditableVisualElementLine.InnerDoJoinWithNextLine(this.CurrentLine);
            EnsureCurrentTextRun();
        }
        char Delete(VisualElementArgs vinv)
        {

            if (CurrentTextRun == null)
            {
                return '\0';
            }
            else
            {
                if (CharIndex < 0)
                {
                    return '\0';
                }

                char toBeRemovedChar = CurrentChar;
                EditableVisualTextRun removingTextRun = CurrentTextRun;
                int removeIndex = CurrentTextRunCharIndex;
                CharIndex--;
                EditableVisualTextRun.InnerRemove(removingTextRun, removeIndex, 1, false, vinv);
                if (removingTextRun.CharacterCount == 0)
                {
                    CurrentLine.Remove(removingTextRun);
                    EnsureCurrentTextRun();
                }
                else
                {
                    removingTextRun.StartBubbleUpLayoutInvalidState();
                }

                return toBeRemovedChar;
            }
        }
        public EditableVisualTextRun GetCurrentTextRun()
        {
            if (CurrentLine.IsBlankLine)
            {
                return null;
            }
            else
            {
                return CurrentTextRun;

            }
        }

        public void Add(char c, VisualElementArgs vinv)
        {
            if (CurrentLine.IsBlankLine)
            {
                EditableVisualTextRun t = new EditableVisualTextRun(c );

                var owner = this.FlowLayer.ownerVisualElement;

                if (owner.MyBoxStyle != null)
                {
                    t.SetStyle(owner.MyBoxStyle, vinv);
                }
                CurrentLine.AddLast(t);

                SetCurrentTextRun(t);
            }
            else
            {
                EditableVisualTextRun cRun = CurrentTextRun;
                if (cRun != null)
                {

                    if (cRun.IsInsertable)
                    {
                        cRun.InsertAfter(CurrentTextRunCharIndex, c, vinv);

                    }
                    else
                    {
                        Add(new EditableVisualTextRun(c));
                        return;
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            CurrentLine.TextLineReCalculateActualLineSize();
            CurrentLine.RefreshInlineArrange();

            CharIndex++;
        }
        public void Add(EditableVisualTextRun textRun)
        {
            if (CurrentLine.IsBlankLine)
            {

                CurrentLine.AddLast(textRun);
                SetCurrentTextRun(textRun);
                CurrentLine.TextLineReCalculateActualLineSize();
                CharIndex += textRun.CharacterCount;

            }
            else
            {
                if (CurrentTextRun != null)
                {
                    VisualPointInfo newPointInfo = CurrentLine.Split(GetCurrentPointInfo());
                    if (newPointInfo.IsOnTheBeginOfLine)
                    {
                        CurrentLine.AddBefore((EditableVisualTextRun)newPointInfo.TextRun, textRun);
                    }
                    else
                    {
                        CurrentLine.AddAfter((EditableVisualTextRun)newPointInfo.TextRun, textRun);
                    }
                    CurrentLine.TextLineReCalculateActualLineSize();
                    EnsureCurrentTextRun(CharIndex + textRun.CharacterCount);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
        public void ReplaceAllLineContent(EditableVisualTextRun[] runs)
        {
            int charIndex = CharIndex;
            CurrentLine.Clear();
            int j = runs.Length;
            for (int i = 0; i < j; ++i)
            {
                CurrentLine.AddLast(runs[i]);
            }

            EnsureCurrentTextRun(charIndex);
        }

        public char DoBackspace(VisualElementArgs vinv)
        {
            return Delete(vinv);
        }
        public char DoDelete(VisualElementArgs vinv)
        {
            if (CharIndex < CurrentLine.CharCount - 1)
            {
                CharIndex++;
                return Delete(vinv);
            }
            else
            {
                return '\0';
            }
        }

        public void SplitToNewLine(VisualElementArgs vinv)
        {

            EditableVisualTextRun lineBreakRun = new EditableVisualTextRun('\n');
            EditableVisualTextRun currentRun = CurrentTextRun;
            if (CurrentLine.IsBlankLine)
            {

                CurrentLine.AddLast(lineBreakRun);

            }
            else
            {
                if (CharIndex == -1)
                {

                    CurrentLine.AddFirst(lineBreakRun);
                    SetCurrentTextRun(null);
                }
                else
                {

                    EditableVisualTextRun rightSplitedPart = EditableVisualTextRun.InnerRemove(currentRun,
                        CurrentTextRunCharIndex + 1, true, vinv);
                    if (rightSplitedPart != null)
                    {
                        CurrentLine.AddAfter(currentRun, rightSplitedPart);
                    }
                    CurrentLine.AddAfter(currentRun, lineBreakRun);

                    if (currentRun.CharacterCount == 0)
                    {
                        CurrentLine.Remove(currentRun);
                    }
                }
            }


            this.TextLayer.TopDownReCalculateContentSize(vinv);
            EnsureCurrentTextRun();
        }
        public EditableVisualPointInfo[] SplitSelectedText(VisualSelectionRange selectionRange)
        {

            EditableVisualPointInfo[] newPoints = CurrentLine.Split(selectionRange);
            EnsureCurrentTextRun();
            return newPoints;
        }
    }

    public abstract class TextLineReader
    {

#if DEBUG
        static int dbugTotalId;
        int dbug_MyId;

        public dbugMultiTextManRecorder dbugTextManRecorder;
#endif

        EditableTextFlowLayer visualFlowLayer;
        EditableVisualElementLine currentLine;

        int currentLineY = 0;
        EditableVisualTextRun currentTextRun;
        int charIndex = -1;
        int caretXPos = 0; int rCharOffset = 0; int rPixelOffset = 0;
        public TextLineReader(EditableTextFlowLayer flowlayer)
        {

#if DEBUG
            this.dbug_MyId = dbugTotalId;
            dbugTotalId++;
#endif

            this.visualFlowLayer = flowlayer;
            flowlayer.Reflow += new EventHandler(flowlayer_Reflow);

            currentLine = flowlayer.GetTextLine(0);

            if (currentLine.FirstRun != null)
            {
                currentTextRun = currentLine.FirstRun;
            }
        }
        public EditableTextFlowLayer FlowLayer
        {
            get
            {
                return this.visualFlowLayer;
            }
        }
        void flowlayer_Reflow(object sender, EventArgs e)
        {
            int prevCharIndex = charIndex;
            this.CharIndex = -1;
            this.CharIndex = prevCharIndex;

        }
        protected EditableVisualElementLine CurrentLine
        {
            get
            {
                return currentLine;
            }
        }

        protected EditableVisualTextRun CurrentTextRun
        {
            get
            {
                return currentTextRun;
            }
        }
        protected void SetCurrentTextRun(EditableVisualTextRun r)
        {
            currentTextRun = r;
        }
        bool MoveToPreviousTextRun()
        {
#if DEBUG
            if (currentTextRun.IsLineBreak)
            {
                throw new NotSupportedException();
            }
#endif
            if (currentTextRun.PrevTextRun != null)
            {
                currentTextRun = currentTextRun.PrevTextRun; rCharOffset -= currentTextRun.CharacterCount;
                rPixelOffset -= currentTextRun.Width;
                charIndex = rCharOffset + currentTextRun.CharacterCount - 1;
                caretXPos = rPixelOffset + currentTextRun.Width;
                return true;

            }
            return false;
        }

        bool MoveToNextTextRun()
        {

#if DEBUG
            if (currentTextRun.IsLineBreak)
            {
                throw new NotSupportedException();
            }
#endif


            EditableVisualTextRun nextTextRun = currentTextRun.NextTextRun;
            if (nextTextRun != null && !nextTextRun.IsLineBreak)
            {
                rCharOffset += currentTextRun.CharacterCount;
                rPixelOffset += currentTextRun.Width;
                currentTextRun = nextTextRun;
                charIndex = rCharOffset;
                caretXPos = rPixelOffset + currentTextRun.GetCharWidth(0);

                return true;
            }
            return false;
        }

        public void MoveToLine(int lineNumber)
        {
            currentLine = visualFlowLayer.GetTextLine(lineNumber);

            currentLineY = currentLine.Top;

            currentTextRun = (EditableVisualTextRun)currentLine.FirstRun;
            rCharOffset = 0;
            rPixelOffset = 0;
            charIndex = -1;
            caretXPos = 0;

        }
        public void CopyContentToStrignBuilder(StringBuilder stBuilder)
        {
            visualFlowLayer.CopyContentToStringBuilder(stBuilder);
        }
        public char NextChar
        {
            get
            {
                if (currentTextRun != null)
                {
                    if (charIndex < 0)
                    {

                        return '\0';
                    }
                    if (charIndex == rCharOffset + currentTextRun.CharacterCount - 1)
                    {
                        if (currentTextRun.NextTextRun != null)
                        {
                            return (currentTextRun.NextTextRun)[0];
                        }
                        else
                        {
                            return '\0';
                        }
                    }
                    else
                    {
                        return currentTextRun[charIndex - rCharOffset + 1];
                    }
                }
                else
                {
                    return '\0';
                }
            }
        }
        public int NextCharWidth
        {
            get
            {
                if (currentTextRun != null)
                {
                    if (charIndex < 0)
                    {

                        return 0;
                    }
                    if (charIndex == rCharOffset + currentTextRun.CharacterCount - 1)
                    {
                        EditableVisualTextRun nextRun = currentTextRun.NextTextRun;

                        if (nextRun != null)
                        {
                            return nextRun.GetCharWidth(0);
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return currentTextRun.GetCharWidth(charIndex - rCharOffset + 1);

                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        public EditableVisualPointInfo GetCurrentPointInfo()
        {
#if DEBUG
            if (currentTextRun != null && currentTextRun.IsFreeElement)
            {
                throw new NotSupportedException();
            }
            if (currentTextRun == null)
            {
            }
#endif


            EditableVisualPointInfo textPointInfo =
                new EditableVisualPointInfo(currentLine, charIndex);
            textPointInfo.SetAdditionVisualInfo(currentTextRun,
                rCharOffset, caretXPos, rPixelOffset);

            return textPointInfo;
        }
        public Point CaretPosition
        {
            get
            {
                return new Point(caretXPos, currentLineY);
            }
        }

        public char CurrentChar
        {
            get
            {
                if (charIndex == -1)
                {
                    return '\0';
                }
                else
                {
                    return currentTextRun[charIndex - rCharOffset];
                }
            }
        }
        public int CurrentTextRunCharOffset
        {
            get
            {
                return rCharOffset;
            }
        }
        public int CurrentTextRunPixelOffset
        {
            get
            {
                return rPixelOffset;
            }
        }
        public int CurrentTextRunCharIndex
        {
            get
            {
                return charIndex - rCharOffset;
            }
        }
        public int CaretXPos
        {
            get
            {
                return caretXPos;
            }
            set
            {

                if (currentTextRun == null)
                {
                    charIndex = -1;
                    caretXPos = 0;
                    rCharOffset = 0;
                    rPixelOffset = 0;

                    return;
                }
                int pixDiff = value - caretXPos; if (pixDiff > 0)
                {

                    do
                    {
                        int thisTextRunPixelLength = currentTextRun.Width;
                        if (rPixelOffset + thisTextRunPixelLength > value)
                        {
                            VisualLocationInfo foundLocation =
EditableVisualTextRun.InnerGetCharacterFromPixelOffset(currentTextRun, value - rPixelOffset);
                            if (foundLocation.charIndex == -1)
                            {
                                if (!(MoveToPreviousTextRun()))
                                {
                                    charIndex = -1;
                                    caretXPos = 0;
                                }
                            }
                            else
                            {
                                caretXPos = rPixelOffset + foundLocation.pixelOffset; charIndex = rCharOffset + foundLocation.charIndex;
                            }
                            return;
                        }
                    } while (MoveToNextTextRun()); caretXPos = rPixelOffset + currentTextRun.Width;
                    charIndex = rCharOffset + currentTextRun.CharacterCount - 1;
                    return;
                }
                else if (pixDiff < 0)
                {
                    do
                    {
                        if (value >= rPixelOffset)
                        {


                            VisualLocationInfo foundLocation = EditableVisualTextRun.InnerGetCharacterFromPixelOffset(currentTextRun, value - rPixelOffset);
                            if (foundLocation.charIndex == -1)
                            {
                                if (!MoveToPreviousTextRun())
                                {
                                    charIndex = -1;
                                    caretXPos = 0;
                                }
                            }
                            else
                            {
                                caretXPos = rPixelOffset + foundLocation.pixelOffset; charIndex = rCharOffset + foundLocation.charIndex;
                            }
                            return;
                        }
                    } while (MoveToPreviousTextRun());//
                    caretXPos = 0;
                    charIndex = -1;
                    return;
                }
            }
        }



        public int CharIndex
        {
            get
            {
                return charIndex;
            }
            set
            {

#if DEBUG
                if (dbugTextManRecorder != null)
                {
                    dbugTextManRecorder.WriteInfo("TextLineReader::CharIndex_set=" + value);
                    dbugTextManRecorder.BeginContext();
                }
#endif          
                if (value < -1 || value > currentLine.CharCount - 1)
                {
                    throw new NotSupportedException("index out of range");
                }

                if (value == -1)
                {
                    charIndex = -1;
                    caretXPos = 0;
                    rCharOffset = 0;
                    rPixelOffset = 0;
                    currentTextRun = currentLine.FirstRun;
                }
                else
                {
                    int diff = value - charIndex;
                    switch (diff)
                    {
                        case 0:
                            {
                                return;
                            }
                        case 1:
                            {
                                if (charIndex + 1 >= rCharOffset + currentTextRun.CharacterCount)
                                {
                                    MoveToNextTextRun();
                                }
                                else
                                {
                                    charIndex++;
                                    caretXPos += currentTextRun.GetCharWidth(charIndex - rCharOffset);
                                }

                            } break;
                        case -1:
                            {
                                if (charIndex - 1 < rCharOffset)
                                {
                                    MoveToPreviousTextRun();
                                }
                                else
                                {
                                    if (charIndex > -1)
                                    {
                                        caretXPos -= currentTextRun.GetCharWidth(charIndex - rCharOffset);
                                        charIndex--;

                                    }
                                }
                            } break;
                        default:
                            {
                                if (diff > 1)
                                {
                                    do
                                    {
                                        if (rCharOffset + currentTextRun.CharacterCount > value)
                                        {
                                            charIndex = value; caretXPos = rPixelOffset + currentTextRun.GetRunWidth(charIndex - rCharOffset + 1);

#if DEBUG
                                            if (dbugTextManRecorder != null)
                                            {
                                                dbugTextManRecorder.EndContext();
                                            }
#endif

                                            return;
                                        }
                                    } while (MoveToNextTextRun());
                                    charIndex = rCharOffset + currentTextRun.CharacterCount;
                                    caretXPos = rPixelOffset + currentTextRun.Width;
                                    return;
                                }
                                else
                                {
                                    do
                                    {
                                        if (rCharOffset - 1 < value)
                                        {
                                            charIndex = value; caretXPos = rPixelOffset + currentTextRun.GetRunWidth(charIndex - rCharOffset + 1);

#if DEBUG
                                            if (dbugTextManRecorder != null)
                                            {
                                                dbugTextManRecorder.EndContext();
                                            }
#endif
                                            return;
                                        }
                                    } while (MoveToPreviousTextRun());
                                    charIndex = -1;
                                    caretXPos = 0;
                                }
                            } break;
                    }
                }
#if DEBUG
                if (dbugTextManRecorder != null)
                {
                    dbugTextManRecorder.EndContext();
                }
#endif
            }
        }


        #region Secondary Method
        public bool IsOnEndOfLine
        {
            get
            {
                return (charIndex == currentLine.CharCount - 1);
            }
        }
        public EditableVisualElementLine GetTextLine(int lineId)
        {
            return TextLayer.GetTextLine(lineId);
        }
        public EditableVisualElementLine GetTextLineAtPos(int y)
        {
            return this.TextLayer.GetTextLineAtPos(y);
        }
        public int LineCount
        {
            get
            {
                return TextLayer.LineCount;
            }
        }

        public bool HasNextLine
        {
            get
            {
                return currentLine.Next != null;
            }
        }
        public bool HasPrevLine
        {
            get
            {
                return currentLine.Prev != null;
            }
        }
        public bool IsOnStartOfLine
        {
            get
            {
                return CharIndex == -1;
            }
        }
        public int CharCount
        {
            get
            {
                return currentLine.CharCount;
            }
        }
        public void CopyLineContent(StringBuilder stBuilder)
        {
            currentLine.CopyLineContent(stBuilder);
        }
        public LinkedList<EditableVisualTextRun> CopySelectedTextRuns(VisualSelectionRange selectionRange)
        {
            LinkedList<EditableVisualTextRun> output = new LinkedList<EditableVisualTextRun>();
            currentLine.Copy(selectionRange, output);
            return output;
        }
        internal EditableTextFlowLayer TextLayer
        {
            get
            {
                return currentLine.OwnerFlowLayer;
            }
        }
        public int LineNumber
        {
            get
            {
                return currentLine.LineNumber;
            }
        }
        public void MoveToNextLine()
        {
            MoveToLine(currentLine.LineNumber + 1);
        }
        public void MoveToPrevLine()
        {
            MoveToLine(currentLine.LineNumber - 1);

        }
        public Rectangle LineArea
        {
            get
            {
                return currentLine.ActualLineArea;
            }
        }
        public Rectangle ParentLineArea
        {
            get
            {
                return currentLine.ParentLineArea;
            }
        }
        #endregion
    }
    public class BackGroundTextLineWriter : TextLineWriter
    {
        public BackGroundTextLineWriter(EditableTextFlowLayer visualElementLayer)
            : base(visualElementLayer)
        {
        }
    }

}