//2014 Apache2, WinterDev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace LayoutFarm.Presentation.Text
{
    public class TextMan
    {
        InternalTextLayerController innerTextMan;
        TextEditRenderBox visualTextSurface;
        internal TextMan(InternalTextLayerController innerTextMan, TextEditRenderBox visualTextSurface)
        {
            this.innerTextMan = innerTextMan;
            this.visualTextSurface = visualTextSurface;
        }
        public void AttachTextDomListener(TextSurfaceEventListener listener)
        {
            this.visualTextSurface.TextDomListener = listener;
        }
        public void AddTextRunsToCurrentLine(IEnumerable<EditableVisualTextRun> textRuns)
        {
            this.innerTextMan.AddTextRunsToCurrentLine(textRuns);
        }
        public void SplitCurrentLineIntoNewLine()
        {
            this.innerTextMan.SplitCurrentLineIntoNewLine(); 
        }
        public LayoutPhaseVisitor GetVInv()
        {
            return this.visualTextSurface.GetVInv();
        }
        public void FreeVinv()
        {
            this.visualTextSurface.FreeVInv();
        }
        public EditableVisualTextRun CurrentTextRun
        {
            get
            {
                return this.innerTextMan.CurrentTextRun;
            }
        }
        public int CurrentTextRunCharIndex
        {
            get
            {
                return this.innerTextMan.CurrentTextRunCharIndex;
            }
        }
    }


    partial class InternalTextLayerController
    {


        VisualSelectionRange selectionRange;

        internal bool updateJustCurrentLine = true;

        bool enableUndoHistoryRecording = true;

        DocumentCommandCollection undoActionCollection;


        BackGroundTextLineWriter backGroundTextLineWriter;
        TextLineWriter textLineWriter;
        TextEditRenderBox visualTextSurface;

        TextMan textMan;

#if DEBUG
        dbugMultiTextManRecorder dbugTextManRecorder;
        internal bool dbugEnableTextManRecorder = false;
#endif

        public InternalTextLayerController(
            TextEditRenderBox visualTextSurface,
            EditableTextFlowLayer textLayer)
        {


            this.visualTextSurface = visualTextSurface;
            textLineWriter = new TextLineWriter(textLayer);
            backGroundTextLineWriter = textLineWriter.GetBackgroundWriter();
            undoActionCollection = new DocumentCommandCollection(this);

            this.textMan = new TextMan(this, visualTextSurface);

#if DEBUG
            if (dbugEnableTextManRecorder)
            {

                dbugTextManRecorder = new dbugMultiTextManRecorder();
                textLineWriter.dbugTextManRecorder = dbugTextManRecorder;
                dbugTextManRecorder.Start();
            }
#endif
        }
        public TextMan TextMan
        {
            get
            {
                return this.textMan;
            }
        }

        public bool EnableUndoHistoryRecording
        {
            get
            {
                return enableUndoHistoryRecording;
            }
            set
            {
                enableUndoHistoryRecording = value;
            }
        }


        public void AddCharToCurrentLine(char c)
        {
            updateJustCurrentLine = true;
            bool passRemoveSelectedText = false;
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                dbugTextManRecorder.WriteInfo("TxLMan::AddCharToCurrentLine " + c);
                dbugTextManRecorder.BeginContext();
            }
#endif
            if (SelectionRange != null)
            {

#if DEBUG
                if (dbugEnableTextManRecorder)
                {

                    dbugTextManRecorder.WriteInfo(SelectionRange);
                }
#endif
                RemoveSelectedText(); passRemoveSelectedText = true;
            }
            if (passRemoveSelectedText && c == ' ')
            {

            }
            else
            {
                undoActionCollection.AddDocAction(
                  new DocActionCharTyping(c, textLineWriter.LineNumber, textLineWriter.CharIndex));
            }

            textLineWriter.Add(c);


#if DEBUG
            if (dbugEnableTextManRecorder)
            {

                dbugTextManRecorder.EndContext();
            }
#endif
        }
        public EditableVisualTextRun CurrentTextRun
        {
            get
            {
                return textLineWriter.GetCurrentTextRun();
            }
        }

        VisualSelectionRangeSnapShot RemoveSelectedText()
        {

#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                dbugTextManRecorder.WriteInfo("TxLMan::RemoveSelectedText");
                dbugTextManRecorder.BeginContext();
            }
#endif

            if (selectionRange == null)
            {
#if DEBUG
                if (dbugEnableTextManRecorder)
                {
                    dbugTextManRecorder.WriteInfo("NO_SEL_RANGE");
                    dbugTextManRecorder.EndContext();
                }
#endif
                return VisualSelectionRangeSnapShot.Empty;
            }
            else if (!selectionRange.IsValid)
            {
#if DEBUG
                if (dbugEnableTextManRecorder)
                {
                    dbugTextManRecorder.WriteInfo("!RANGE_ON_SAME_POINT");
                }
#endif
                CancelSelect();

#if DEBUG
                if (dbugEnableTextManRecorder)
                {
                    dbugTextManRecorder.EndContext();
                }
#endif
                return VisualSelectionRangeSnapShot.Empty;
            }
            selectionRange.SwapIfUnOrder();

            VisualSelectionRangeSnapShot selSnapshot = selectionRange.GetSelectionRangeSnapshot();

            VisualPointInfo startPoint = selectionRange.StartPoint;
            CurrentLineNumber = startPoint.LineId;
            int preCutIndex = startPoint.LineCharIndex;
            textLineWriter.CharIndex = startPoint.LineCharIndex;
            if (selectionRange.IsOnTheSameLine)
            {
                LinkedList<EditableVisualTextRun> tobeDeleteTextRun = textLineWriter.CopySelectedTextRuns(selectionRange);
                if (tobeDeleteTextRun != null)
                {

                    undoActionCollection.AddDocAction(
                    new DocActionDeleteRange(tobeDeleteTextRun,
                        selSnapshot.startLineNum,
                        selSnapshot.startColumnNum,
                        selSnapshot.endLineNum,
                        selSnapshot.endColumnNum));

                    textLineWriter.RemoveSelectedTextRuns(selectionRange);
                    updateJustCurrentLine = true;
                }
            }
            else
            {
                int startPointLindId = startPoint.LineId;
                int startPointCharIndex = startPoint.LineCharIndex;

                LinkedList<EditableVisualTextRun> tobeDeleteTextRun = textLineWriter.CopySelectedTextRuns(selectionRange);
                if (tobeDeleteTextRun != null)
                {
                    undoActionCollection.AddDocAction(
                    new DocActionDeleteRange(tobeDeleteTextRun,
                        selSnapshot.startLineNum,
                        selSnapshot.startColumnNum,
                        selSnapshot.endLineNum,
                        selSnapshot.endColumnNum));

                    textLineWriter.RemoveSelectedTextRuns(selectionRange);
                    updateJustCurrentLine = false;
                    textLineWriter.MoveToLine(startPointLindId);
                    textLineWriter.CharIndex = startPointCharIndex;
                }
            }
            CancelSelect(); TextEditRenderBox.NotifyTextContentSizeChanged(visualTextSurface);

#if DEBUG
            if (dbugEnableTextManRecorder)
            {

                dbugTextManRecorder.EndContext();
            }
#endif
            return selSnapshot;

        }
        void SplitSelectedText()
        {
            VisualSelectionRange selRange = SelectionRange;
            if (selRange != null)
            {
                EditableVisualPointInfo[] newPoints = textLineWriter.SplitSelectedText(selRange);
                if (newPoints != null)
                {
                    selRange.StartPoint = newPoints[0];
                    selRange.EndPoint = newPoints[1];
                    return;
                }
                else
                {
                    selectionRange = null;
                }
            }

        }
        public void SplitCurrentLineIntoNewLine()
        {
            RemoveSelectedText();

            undoActionCollection.AddDocAction(
    new DocActionSplitToNewLine(textLineWriter.LineNumber, textLineWriter.CharIndex));

            textLineWriter.SplitToNewLine();

            CurrentLineNumber++;

            updateJustCurrentLine = false;

            TextEditRenderBox.NotifyTextContentSizeChanged(visualTextSurface);
        }


        public TextRunStyle GetFirstTextStyleInSelectedRange()
        {
            VisualSelectionRange selRange = SelectionRange;
            if (selRange != null)
            {
                if (selectionRange.StartPoint.TextRun != null)
                {
                    return selectionRange.StartPoint.TextRun.MyBoxStyle;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return null;
            }
        }
        public void DoFormatSelection(TextRunStyle textStyle)
        {
            int startLineNum = textLineWriter.LineNumber;
            int startCharIndex = textLineWriter.CharIndex;
            SplitSelectedText();


            VisualSelectionRange selRange = SelectionRange;
            if (selRange != null)
            {

                foreach (EditableVisualTextRun r in selRange.GetPrintableTextRunIter())
                {
                    r.SetStyle(textStyle);
                }

                this.updateJustCurrentLine = selectionRange.IsOnTheSameLine;

                CancelSelect();

                CharIndex++;
                CharIndex--;
            }
        }


        public int CurrentLineCharCount
        {
            get
            {
                return textLineWriter.CharCount;
            }
        }

        public int LineCount
        {
            get
            {
                return textLineWriter.LineCount;
            }
        }
        public int CurrentLineCharIndex
        {
            get
            {
                return textLineWriter.CharIndex;
            }
        }
        public int CurrentTextRunCharIndex
        {
            get
            {
                return textLineWriter.CurrentTextRunCharIndex;
            }
        }
        public int CurrentLineNumber
        {
            get
            {
                return textLineWriter.LineNumber;
            }
            set
            {
                int diff = value - textLineWriter.LineNumber;

                switch (diff)
                {
                    case 0:
                        {
                            return;
                        }
                    case 1:
                        {

                            if (textLineWriter.HasNextLine)
                            {
                                textLineWriter.MoveToNextLine();
                                DoHome();
                            }

                        } break;
                    case -1:
                        {
                            if (textLineWriter.HasPrevLine)
                            {
                                textLineWriter.MoveToPrevLine();
                                DoEnd();
                            }
                        } break;
                    default:
                        {
                            if (diff > 1)
                            {
                                textLineWriter.MoveToLine(value);

                            }
                            else
                            {
                                if (value < -1)
                                {
                                    textLineWriter.MoveToLine(value);
                                }
                                else
                                {
                                    textLineWriter.MoveToLine(value);

                                }

                            }
                        } break;
                }
            }
        }

        public VisualSelectionRange SelectionRange
        {
            get
            {
                return selectionRange;
            }

        }
        public void UpdateSelectionRange()
        {
            if (selectionRange != null)
            {
                selectionRange.UpdateSelectionRange();
            }
        }

        public EditableVisualPointInfo GetCurrentPointInfo()
        {
            return textLineWriter.GetCurrentPointInfo();
        }
        public int CharIndex
        {
            get
            {
                return textLineWriter.CharIndex;
            }
            set
            {

                if (textLineWriter.CharIndex < 0 && value < -1)
                {
                    if (textLineWriter.HasPrevLine)
                    {
                        textLineWriter.MoveToPrevLine();
                        DoEnd();
                    }
                }
                else
                {
                    int lineLength = textLineWriter.CharCount;
                    if (textLineWriter.CharIndex >= lineLength - 1 && value > lineLength - 1)
                    {
                        if (textLineWriter.HasNextLine)
                        {
                            textLineWriter.MoveToNextLine();
                        }
                    }
                    else
                    {
                        textLineWriter.CharIndex = value;
                    }
                }
            }
        }
        public bool IsOnEndOfLine
        {
            get { return textLineWriter.IsOnEndOfLine; }
        }
        public bool IsOnStartOfLine
        {
            get
            {
                return textLineWriter.IsOnStartOfLine;
            }
        }

        public Point CaretPos
        {
            get
            {
                return this.textLineWriter.CaretPosition;
            }
            set
            {
                int j = textLineWriter.LineCount;
                if (j > 0)
                {
                    EditableVisualElementLine line = textLineWriter.GetTextLineAtPos(value.Y);
                    int calculatedLineId = 0;
                    if (line != null)
                    {
                        calculatedLineId = line.LineNumber;

                    }

                    this.CurrentLineNumber = calculatedLineId;
                    this.textLineWriter.CaretXPos = value.X;

                   
                }
            }
        }
        public Rectangle CurrentLineArea
        {
            get
            {
                return textLineWriter.LineArea;
            }
        }
        public Rectangle CurrentParentLineArea
        {
            get
            {
                return textLineWriter.ParentLineArea;
            }
        }
        public bool IsOnFirstLine
        {
            get
            {
                return !textLineWriter.HasPrevLine;
            }
        }

        void JoinWithNextLine()
        {
            textLineWriter.JoinWithNextLine();
            TextEditRenderBox.NotifyTextContentSizeChanged(visualTextSurface);
        }
        public void UndoLastAction()
        {
            undoActionCollection.UndoLastAction();
        }
        public void ReverseLastUndoAction()
        {
            undoActionCollection.ReverseLastUndoAction();
        }

    }




}