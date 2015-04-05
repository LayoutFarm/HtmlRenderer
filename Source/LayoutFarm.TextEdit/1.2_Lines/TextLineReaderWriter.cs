// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.Text
{
    class TextLineWriter : TextLineReader
    {
        BackGroundTextLineWriter backgroundWriter;


        public TextLineWriter(EditableTextFlowLayer textLayer)
            : base(textLayer)
        {

        }
        public TextSpanStyle CurrentSpanStyle
        {
            get
            {
                return this.TextLayer.CurrentTextSpanStyle;
            }
        }
        internal BackGroundTextLineWriter GetBackgroundWriter()
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
        public void Reload(IEnumerable<EditableTextSpan> runs)
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
            if (run == null || !run.HasParent)
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
        public void RemoveSelectedTextRuns(VisualSelectionRange selectionRange)
        {
            int precutIndex = selectionRange.StartPoint.LineCharIndex; 
            CurrentLine.Remove(selectionRange);

            CurrentLine.TextLineReCalculateActualLineSize();
            CurrentLine.RefreshInlineArrange();

            EnsureCurrentTextRun(precutIndex);

        }

        public void ReplaceCurrentLine(IEnumerable<EditableTextSpan> textRuns)
        {
            int currentCharIndex = CharIndex;

            CurrentLine.ReplaceAll(textRuns);

            CurrentLine.TextLineReCalculateActualLineSize();
            CurrentLine.RefreshInlineArrange();
            
            EnsureCurrentTextRun(currentCharIndex);
        } 
        public void JoinWithNextLine()
        {
            EditableTextLine.InnerDoJoinWithNextLine(this.CurrentLine);
            EnsureCurrentTextRun();
        }
        char Delete()
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
                EditableTextSpan removingTextRun = CurrentTextRun;
                int removeIndex = CurrentTextRunCharIndex;
                CharIndex--;
                EditableTextSpan.InnerRemove(removingTextRun, removeIndex, 1, false);
                if (removingTextRun.CharacterCount == 0)
                {
                    CurrentLine.Remove(removingTextRun);
                    EnsureCurrentTextRun();
                }
                CurrentLine.TextLineReCalculateActualLineSize();
                CurrentLine.RefreshInlineArrange();

                return toBeRemovedChar;
            }
        }
        public EditableTextSpan GetCurrentTextRun()
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

        public void AddCharacter(char c)
        {
            if (CurrentLine.IsBlankLine)
            {
                //1. new 
                EditableTextSpan t = new EditableTextSpan(this.Root,
                    c,
                    this.CurrentSpanStyle);

                var owner = this.FlowLayer.OwnerRenderElement;

                CurrentLine.AddLast(t);

                SetCurrentTextRun(t);
            }
            else
            {
                EditableTextSpan cRun = CurrentTextRun;
                if (cRun != null)
                {   
                    if (cRun.IsInsertable)
                    {
                        cRun.InsertAfter(CurrentTextRunCharIndex, c);
                    }
                    else
                    { 
                        AddTextSpan(new EditableTextSpan(this.Root, c, this.CurrentSpanStyle));
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
        public void AddTextSpan(EditableTextSpan textRun)
        {
            if (CurrentLine.IsBlankLine)
            {

                CurrentLine.AddLast(textRun);
                SetCurrentTextRun(textRun);
                CurrentLine.TextLineReCalculateActualLineSize();
                CurrentLine.RefreshInlineArrange();
                CharIndex += textRun.CharacterCount;

            }
            else
            {
                if (CurrentTextRun != null)
                {
                    VisualPointInfo newPointInfo = CurrentLine.Split(GetCurrentPointInfo());
                    if (newPointInfo.IsOnTheBeginOfLine)
                    {
                        CurrentLine.AddBefore((EditableTextSpan)newPointInfo.TextRun, textRun);
                    }
                    else
                    {
                        CurrentLine.AddAfter((EditableTextSpan)newPointInfo.TextRun, textRun);
                    }
                    CurrentLine.TextLineReCalculateActualLineSize();
                    CurrentLine.RefreshInlineArrange();
                    EnsureCurrentTextRun(CharIndex + textRun.CharacterCount);
                    
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
        public void ReplaceAllLineContent(EditableTextSpan[] runs)
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

        public char DoBackspace()
        {
            return Delete();
        }
        public char DoDelete()
        {
            if (CharIndex < CurrentLine.CharCount - 1)
            {
                CharIndex++;
                return Delete();
            }
            else
            {
                return '\0';
            }
        }

        public void SplitToNewLine()
        {

            EditableTextSpan lineBreakRun = new EditableTextSpan(this.Root, '\n', this.CurrentSpanStyle);
            EditableTextSpan currentRun = CurrentTextRun;
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

                    EditableTextSpan rightSplitedPart = EditableTextSpan.InnerRemove(currentRun,
                        CurrentTextRunCharIndex + 1, true);
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


            this.TextLayer.TopDownReCalculateContentSize();
            EnsureCurrentTextRun();
        }
        public EditableVisualPointInfo[] SplitSelectedText(VisualSelectionRange selectionRange)
        {

            EditableVisualPointInfo[] newPoints = CurrentLine.Split(selectionRange);
            EnsureCurrentTextRun();
            return newPoints;
        }
    }

    abstract class TextLineReader
    {

#if DEBUG
        static int dbugTotalId;
        int dbug_MyId;
        public dbugMultiTextManRecorder dbugTextManRecorder;
#endif

        EditableTextFlowLayer visualFlowLayer;
        EditableTextLine currentLine;

        int currentLineY = 0;
        EditableTextSpan currentTextRun;
        int charIndex = -1;
        int caretXPos = 0; 
        int rCharOffset = 0; 
        int rPixelOffset = 0;
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
        protected RootGraphic Root
        {
            get { return this.visualFlowLayer.Root; }
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
        protected EditableTextLine CurrentLine
        {
            get
            {
                return currentLine;
            }
        }

        protected EditableTextSpan CurrentTextRun
        {
            get
            {
                return currentTextRun;
            }
        }
        protected void SetCurrentTextRun(EditableTextSpan r)
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


            EditableTextSpan nextTextRun = currentTextRun.NextTextRun;
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

            currentTextRun = (EditableTextSpan)currentLine.FirstRun;
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
                        EditableTextSpan nextRun = currentTextRun.NextTextRun;

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
            if (currentTextRun != null && !currentTextRun.HasParent)
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
                            VisualLocationInfo foundLocation = EditableTextSpan.InnerGetCharacterFromPixelOffset(currentTextRun, value - rPixelOffset);
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
                    } while (MoveToNextTextRun());
                    caretXPos = rPixelOffset + currentTextRun.Width;
                    charIndex = rCharOffset + currentTextRun.CharacterCount - 1;
                    return;
                }
                else if (pixDiff < 0)
                {
                    do
                    {
                        if (value >= rPixelOffset)
                        {


                            VisualLocationInfo foundLocation = EditableTextSpan.InnerGetCharacterFromPixelOffset(currentTextRun, value - rPixelOffset);
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
                                            charIndex = value;
                                            caretXPos = rPixelOffset + currentTextRun.GetRunWidth(charIndex - rCharOffset + 1);

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

        internal EditableTextLine GetTextLine(int lineId)
        {
            return TextLayer.GetTextLine(lineId);
        }

        internal EditableTextLine GetTextLineAtPos(int y)
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
        public LinkedList<EditableTextSpan> CopySelectedTextRuns(VisualSelectionRange selectionRange)
        {
            LinkedList<EditableTextSpan> output = new LinkedList<EditableTextSpan>();
            currentLine.Copy(selectionRange, output);
            return output;
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


        internal EditableTextFlowLayer TextLayer
        {
            get
            {
                return currentLine.OwnerFlowLayer;
            }
        }
        #endregion
    }
    class BackGroundTextLineWriter : TextLineWriter
    {
        public BackGroundTextLineWriter(EditableTextFlowLayer visualElementLayer)
            : base(visualElementLayer)
        {
        }
    }

}