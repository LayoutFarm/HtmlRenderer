//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace LayoutFarm.Presentation.Text
{


    partial class InternalTextLayerController
    {

        public VisualSelectionRangeSnapShot DoDelete(LayoutPhaseVisitor vinv)
        {

#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                dbugTextManRecorder.WriteInfo("TxLMan::DoDelete");
                dbugTextManRecorder.BeginContext();
            }
#endif

            VisualSelectionRangeSnapShot removedRange = this.RemoveSelectedText(vinv);

            if (removedRange.IsEmpty())
            {
                updateJustCurrentLine = true;
                char deletedChar = textLineWriter.DoDelete(vinv);
                if (deletedChar == '\0')
                {
                    undoActionCollection.AddDocAction(new DocActionJoinWithNextLine(
textLineWriter.LineNumber, textLineWriter.CharIndex));

                    JoinWithNextLine();
                    updateJustCurrentLine = false;
                }
                else
                {
                    undoActionCollection.AddDocAction(
new DocActionDeleteChar(deletedChar, textLineWriter.LineNumber, textLineWriter.CharIndex));

                    char nextChar = textLineWriter.NextChar;

                    if (nextChar != '\0' && textLineWriter.NextCharWidth < 1)
                    {
                        DoDelete(vinv);
                    }
                }
            }
#if DEBUG
            if (dbugEnableTextManRecorder) dbugTextManRecorder.EndContext();
#endif

            return removedRange;
        }
#if DEBUG
        int dbug_BackSpaceCount = 0;
#endif
        public bool DoBackspace(LayoutPhaseVisitor vinv)
        {


#if DEBUG

            if (dbugEnableTextManRecorder)
            {

                dbug_BackSpaceCount++;

                dbugTextManRecorder.WriteInfo("TxLMan::DoBackSpace");
                dbugTextManRecorder.BeginContext();

            }
#endif

            VisualSelectionRangeSnapShot removeSelRange = this.RemoveSelectedText(vinv);
            if (!removeSelRange.IsEmpty())
            {
                CancelSelect();
#if DEBUG
                if (dbugEnableTextManRecorder) dbugTextManRecorder.EndContext();
#endif
                return true;
            }
            else
            {
                updateJustCurrentLine = true;
                char deletedChar = textLineWriter.DoBackspace(vinv);
                if (deletedChar == '\0')
                {
                    if (!IsOnFirstLine)
                    {
                        CurrentLineNumber--; DoEnd(); undoActionCollection.AddDocAction(new DocActionJoinWithNextLine(
textLineWriter.LineNumber, textLineWriter.CharIndex));

                        JoinWithNextLine();
                    }
#if DEBUG
                    if (dbugEnableTextManRecorder) dbugTextManRecorder.EndContext();
#endif
                    return false;
                }
                else
                {
                    undoActionCollection.AddDocAction(
new DocActionDeleteChar(deletedChar, textLineWriter.LineNumber, textLineWriter.CharIndex));
#if DEBUG
                    if (dbugEnableTextManRecorder) dbugTextManRecorder.EndContext();
#endif
                    return true;
                }
            }
        }
        public void DoEnd()
        {
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                dbugTextManRecorder.WriteInfo("TxLMan::DoEnd");
                dbugTextManRecorder.BeginContext();
            }
#endif
            textLineWriter.CharIndex = textLineWriter.CharCount - 1;
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                dbugTextManRecorder.EndContext();
            }
#endif
        }
        public void DoHome()
        {
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                dbugTextManRecorder.WriteInfo("TxLMan::DoHome");
                dbugTextManRecorder.BeginContext();
            }
#endif
            textLineWriter.CharIndex = -1;
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                dbugTextManRecorder.EndContext();
            }
#endif
        }
    }
}