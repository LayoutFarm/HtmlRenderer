//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.Text
{
    abstract class DocumentAction
    {
        protected int startLineNumber;
        protected int startCharIndex;
        public DocumentAction(int lineNumber, int charIndex)
        {
            this.startLineNumber = lineNumber;
            this.startCharIndex = charIndex;
        }

        public abstract void InvokeUndo(InternalTextLayerController textdom);
        public abstract void InvokeRedo(InternalTextLayerController textdom);
    }

    class DocActionCharTyping : DocumentAction
    {
        char c;
        public DocActionCharTyping(char c, int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
            this.c = c;
        }

        public override void InvokeUndo(InternalTextLayerController textdom)
        {
            textdom.CurrentLineNumber = startLineNumber;
            textdom.CharIndex = startCharIndex;
            textdom.DoBackspace();
        }
        public override void InvokeRedo(InternalTextLayerController textdom)
        {
            textdom.CurrentLineNumber = startLineNumber;
            textdom.CharIndex = startCharIndex;
            textdom.AddCharToCurrentLine(c);
        }
    }

    class DocActionSplitToNewLine : DocumentAction
    {
        public DocActionSplitToNewLine(int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
        }
        public override void InvokeUndo(InternalTextLayerController textdom)
        {
            textdom.CurrentLineNumber = startLineNumber;
            textdom.DoEnd();
            textdom.DoDelete();
        }
        public override void InvokeRedo(InternalTextLayerController textdom)
        {
            textdom.CurrentLineNumber = startLineNumber;
            textdom.CharIndex = startCharIndex;
            textdom.SplitCurrentLineIntoNewLine();
        }
    }
    class DocActionJoinWithNextLine : DocumentAction
    {
        public DocActionJoinWithNextLine(int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
        }
        public override void InvokeUndo(InternalTextLayerController textdom)
        {
            textdom.CurrentLineNumber = startLineNumber;
            textdom.CharIndex = startCharIndex;
            textdom.SplitCurrentLineIntoNewLine();
        }
        public override void InvokeRedo(InternalTextLayerController textdom)
        {
            textdom.CurrentLineNumber = startLineNumber;
            textdom.CharIndex = startCharIndex;
            textdom.DoDelete();
        }
    }


    class DocActionDeleteChar : DocumentAction
    {
        char c;
        public DocActionDeleteChar(char c, int lineNumber, int charIndex)
            : base(lineNumber, charIndex)
        {
            this.c = c;
        }
        public override void InvokeUndo(InternalTextLayerController textdom)
        {
            textdom.CurrentLineNumber = startLineNumber;
            textdom.CharIndex = startCharIndex;
            textdom.AddCharToCurrentLine(c);
        }
        public override void InvokeRedo(InternalTextLayerController textdom)
        {
            textdom.CurrentLineNumber = startLineNumber;
            textdom.CharIndex = startCharIndex;
            textdom.DoDelete();
        }
    }
    class DocActionDeleteRange : DocumentAction
    {
        LinkedList<EditableRun> deletedTextRuns;
        int endLineNumber;
        int endCharIndex;
        public DocActionDeleteRange(LinkedList<EditableRun> deletedTextRuns, int startLineNum, int startColumnNum,
            int endLineNum, int endColumnNum)
            : base(startLineNum, startColumnNum)
        {
            this.deletedTextRuns = deletedTextRuns;
            this.endLineNumber = endLineNum;
            this.endCharIndex = endColumnNum;
        }

        public override void InvokeUndo(InternalTextLayerController textdom)
        {
            textdom.CancelSelect();
            textdom.AddTextRunsToCurrentLine(deletedTextRuns);
        }
        public override void InvokeRedo(InternalTextLayerController textdom)
        {
            textdom.CurrentLineNumber = startLineNumber;
            textdom.CharIndex = startCharIndex;
            textdom.StartSelect();
            textdom.CurrentLineNumber = endLineNumber;
            textdom.CharIndex = endCharIndex;
            textdom.EndSelect();
            textdom.DoDelete();
        }
    }

    class DocActionInsertRuns : DocumentAction
    {
        EditableRun singleInsertTextRun;
        IEnumerable<EditableRun> insertingTextRuns;
        int endLineNumber;
        int endCharIndex;
        public DocActionInsertRuns(IEnumerable<EditableRun> insertingTextRuns,
            int startLineNumber, int startCharIndex, int endLineNumber, int endCharIndex)
            : base(startLineNumber, startCharIndex)
        {
            this.insertingTextRuns = insertingTextRuns;
            this.endLineNumber = endLineNumber;
            this.endCharIndex = endCharIndex;
        }
        public DocActionInsertRuns(EditableRun insertingTextRun,
           int startLineNumber, int startCharIndex, int endLineNumber, int endCharIndex)
            : base(startLineNumber, startCharIndex)
        {
            this.singleInsertTextRun = insertingTextRun;
            this.endLineNumber = endLineNumber;
            this.endCharIndex = endCharIndex;
        }
        public override void InvokeUndo(InternalTextLayerController textdom)
        {
            textdom.CurrentLineNumber = startLineNumber;
            textdom.CharIndex = startCharIndex;
            textdom.StartSelect();
            textdom.CurrentLineNumber = endLineNumber;
            textdom.CharIndex = endCharIndex;
            textdom.EndSelect();
            textdom.DoDelete();
        }
        public override void InvokeRedo(InternalTextLayerController textdom)
        {
            textdom.CurrentLineNumber = startLineNumber;
            textdom.CharIndex = startCharIndex;
            if (singleInsertTextRun != null)
            {
                textdom.AddTextRunToCurrentLine(singleInsertTextRun);
            }
            else
            {
                textdom.AddTextRunsToCurrentLine(insertingTextRuns);
            }
        }
    }
    class DocActionFormatting : DocumentAction
    {
        int endLineNumber;
        int endCharIndex;
        TextSpanStyle textStyle;
        public DocActionFormatting(TextSpanStyle textStyle, int startLineNumber, int startCharIndex, int endLineNumber, int endCharIndex)
            : base(startLineNumber, startCharIndex)
        {
            this.textStyle = textStyle;
            this.endLineNumber = endLineNumber;
            this.endCharIndex = endCharIndex;
        }


        public override void InvokeUndo(InternalTextLayerController textMan)
        {
            textMan.CurrentLineNumber = startLineNumber;
            textMan.CharIndex = startCharIndex;
            textMan.StartSelect();
            textMan.CurrentLineNumber = endLineNumber;
            textMan.CharIndex = endCharIndex;
            textMan.EndSelect();
        }
        public override void InvokeRedo(InternalTextLayerController textdom)
        {
        }
    }

    class DocumentCommandCollection
    {
        LinkedList<DocumentAction> undoList = new LinkedList<DocumentAction>();
        Stack<DocumentAction> reverseUndoAction = new Stack<DocumentAction>();
        int maxCommandsCount = 20;
        InternalTextLayerController textdom;
        public DocumentCommandCollection(InternalTextLayerController textdomManager)
        {
            this.textdom = textdomManager;
        }

        public void Clear()
        {
            undoList.Clear();
            reverseUndoAction.Clear();
        }

        public int MaxCommandCount
        {
            get
            {
                return maxCommandsCount;
            }
            set
            {
                maxCommandsCount = value;
                if (undoList.Count > maxCommandsCount)
                {
                    int diff = undoList.Count - maxCommandsCount;
                    for (int i = 0; i < diff; i++)
                    {
                        undoList.RemoveFirst();
                    }
                }
            }
        }

        public void AddDocAction(DocumentAction docAct)
        {
            if (textdom.EnableUndoHistoryRecording)
            {
                undoList.AddLast(docAct);
                EnsureCapacity();
            }
        }

        void EnsureCapacity()
        {
            if (undoList.Count > maxCommandsCount)
            {
                undoList.RemoveFirst();
            }
        }
        public void UndoLastAction()
        {
            DocumentAction docAction = PopUndoCommand();
            if (docAction != null)
            {
                textdom.EnableUndoHistoryRecording = false;
                docAction.InvokeUndo(textdom);
                textdom.EnableUndoHistoryRecording = true;
                reverseUndoAction.Push(docAction);
            }
        }
        public void ReverseLastUndoAction()
        {
            if (reverseUndoAction.Count > 0)
            {
                textdom.EnableUndoHistoryRecording = false;
                DocumentAction docAction = reverseUndoAction.Pop();
                textdom.EnableUndoHistoryRecording = true;
                docAction.InvokeRedo(textdom);
                undoList.AddLast(docAction);
            }
        }
        public DocumentAction PeekCommand
        {
            get
            {
                return undoList.Last.Value;
            }
        }
        public int Count
        {
            get
            {
                return undoList.Count;
            }
        }
        public DocumentAction PopUndoCommand()
        {
            if (undoList.Count > 0)
            {
                DocumentAction lastCmd = undoList.Last.Value;
                undoList.RemoveLast();
                return lastCmd;
            }
            else
            {
                return null;
            }
        }
    }
}