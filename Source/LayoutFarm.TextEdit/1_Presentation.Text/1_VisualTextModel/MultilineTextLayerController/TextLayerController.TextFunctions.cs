//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace LayoutFarm.Text
{

    partial class InternalTextLayerController
    {

        public void ReplaceCurrentLineTextRun(IEnumerable<EditableVisualTextRun> textruns)
        {
            textLineWriter.ReplaceCurrentLine(textruns);
        }


        public void ReplaceLine(int lineNum, IEnumerable<EditableVisualTextRun> textruns)
        {

            if (textLineWriter.LineNumber == backGroundTextLineWriter.LineNumber)
            {
                int prevIndex = textLineWriter.CharIndex;
                textLineWriter.ReplaceCurrentLine(textruns);
            }
            else
            {
                backGroundTextLineWriter.MoveToLine(lineNum);
                backGroundTextLineWriter.ReplaceCurrentLine(textruns);
            }
        }


        public void LoadTextRun(IEnumerable<EditableVisualTextRun> runs)
        {
            this.CancelSelect();
            textLineWriter.Clear();
            textLineWriter.Reload(runs);
            updateJustCurrentLine = false;
            textLineWriter.MoveToLine(0);
        }

        public void ReplaceCurrentTextRunContent(int nBackSpace, EditableVisualTextRun newTextRun)
        {
            if (newTextRun != null)
            {
                EnableUndoHistoryRecording = false; for (int i = 0; i < nBackSpace; i++)
                {
                    DoBackspace();
                }
                EnableUndoHistoryRecording = true;
                int startLineNum = textLineWriter.LineNumber;
                int startCharIndex = textLineWriter.CharIndex;


                textLineWriter.Add(newTextRun);
                textLineWriter.EnsureCurrentTextRun();

                undoActionCollection.AddDocAction(
                    new DocActionInsertRuns(new EditableVisualTextRun[] { newTextRun }, startLineNum, startCharIndex,
                        textLineWriter.LineNumber, textLineWriter.CharIndex));
            }

        }

        public void ReplaceCurrentTextRunContent(int nBackSpace, string content)
        {
            if (content != null)
            {
                for (int i = 0; i < nBackSpace; i++)
                {
                    DoBackspace();
                }
                int j = content.Length;
                if (j > 0)
                {
                    for (int i = 0; i < j; i++)
                    {
                        textLineWriter.Add(content[i]);
                    }
                }
            }
        }

        public void AddTextRunsToCurrentLine(IEnumerable<EditableVisualTextRun> textRuns)
        {
            RemoveSelectedText();
            int startLineNum = textLineWriter.LineNumber;
            int startCharIndex = textLineWriter.CharIndex;

            bool isRecordingHx = EnableUndoHistoryRecording;

            EnableUndoHistoryRecording = false; foreach (EditableVisualTextRun t in textRuns)
            {
                if (t.IsLineBreak)
                {
                    textLineWriter.SplitToNewLine();
                    CurrentLineNumber++;
                }
                else
                {
                    textLineWriter.Add(t);
                }
            }
            EnableUndoHistoryRecording = isRecordingHx;
            undoActionCollection.AddDocAction(
                new DocActionInsertRuns(textRuns, startLineNum, startCharIndex,
                    textLineWriter.LineNumber, textLineWriter.CharIndex));

            updateJustCurrentLine = false;
            TextEditRenderBox.NotifyTextContentSizeChanged(visualTextSurface);

        }

        public void CopyAllToPlainText(StringBuilder output)
        {
            textLineWriter.CopyContentToStrignBuilder(output);
        }
        public void Clear()
        {
            CancelSelect();
            textLineWriter.Clear();
            TextEditRenderBox.NotifyTextContentSizeChanged(visualTextSurface);
        }


        public void CopySelectedTextToPlainText(StringBuilder stBuilder)
        {

            if (selectionRange == null)
            {

            }
            else
            {

                selectionRange.SwapIfUnOrder();
                if (selectionRange.IsOnTheSameLine)
                {
                    LinkedList<EditableVisualTextRun> runs = textLineWriter.CopySelectedTextRuns(selectionRange);
                    foreach (EditableVisualTextRun t in runs)
                    {
                        t.CopyContentToStringBuilder(stBuilder);
                    }
                }
                else
                {
                    VisualPointInfo startPoint = selectionRange.StartPoint;
                    CurrentLineNumber = startPoint.LineId;
                    textLineWriter.CharIndex = startPoint.LineCharIndex;

                    LinkedList<EditableVisualTextRun> runs = textLineWriter.CopySelectedTextRuns(selectionRange);
                    foreach (EditableVisualTextRun t in runs)
                    {
                        t.CopyContentToStringBuilder(stBuilder);
                    }

                }

            }
        }
        public void CopyCurrentLine(StringBuilder output)
        {
            textLineWriter.CopyLineContent(output);
        }
        public void CopyLine(int lineNum, StringBuilder output)
        {
            backGroundTextLineWriter.MoveToLine(lineNum);
            backGroundTextLineWriter.CopyLineContent(output);
        }

        public void StartSelect()
        {
            if (textLineWriter != null)
            {


                selectionRange = new VisualSelectionRange(GetCurrentPointInfo(), GetCurrentPointInfo());
            }
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                dbugTextManRecorder.WriteInfo("TxLMan::StartSelect");

            }
#endif

        }
        public void EndSelect()
        {
            if (textLineWriter != null)
            {

#if DEBUG
                if (dbugEnableTextManRecorder)
                {
                    dbugTextManRecorder.WriteInfo("TxLMan::EndSelect");

                }
#endif
                selectionRange.EndPoint = GetCurrentPointInfo();
            }

        }
        public void CancelSelect()
        {
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                dbugTextManRecorder.WriteInfo("TxLMan::CancelSelect");

            }
#endif
            selectionRange = null;
        }

        public VisualPointInfo FindTextRunOnPosition(int x, int y)
        {
            if (y < 0)
            {
                return null;
            }
            else
            {
                int j = textLineWriter.LineCount;
                if (j > 0)
                {
                    EditableVisualElementLine textLine = textLineWriter.GetTextLineAtPos(y);
                    if (textLine != null)
                    {
                        return textLine.GetTextPointInfoFromCaretPoint(x);
                    }



                }
                return null;
            }
        }
    }
}