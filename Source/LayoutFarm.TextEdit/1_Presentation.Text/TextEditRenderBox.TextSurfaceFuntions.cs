//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

using System.IO;

using LayoutFarm.Presentation;
using LayoutFarm.Presentation.Text;



namespace LayoutFarm.Presentation.Text
{

    partial class TextEditRenderBox
    {

        bool IsMultiLine
        {
            get
            {
                return isMultiLine;
            }
        }
        TextSurfaceEventListener textSurfaceEventListener;
        public TextSurfaceEventListener TextDomListener
        {
            get
            {
                return textSurfaceEventListener;
            }
            set
            {
                textSurfaceEventListener = value;
                if (value != null)
                {
                    textSurfaceEventListener.SetMonitoringTextSurface(this);
                }
            }
        }


        public override void ClearAllChildren()
        {
            internalTextLayerController.Clear();
            this.Layers.ClearAllContentInEachLayer();
        }

        public int Column
        {
            get
            {
                if (internalTextLayerController != null)
                {
                    return internalTextLayerController.CharIndex;
                }
                else
                {
                    return -1;
                }
            }
            set
            {
                if (internalTextLayerController != null)
                {
                    internalTextLayerController.CharIndex = value;
                }
            }
        }

        static Stack<StringBuilder> stringBuilderPool = new Stack<StringBuilder>();
        static StringBuilder GetFreeStringBuilder()
        {
            if (stringBuilderPool.Count > 0)
            {
                return stringBuilderPool.Pop();
            }
            else
            {
                return new StringBuilder();
            }
        }
        static void ReleaseStringBuilder(StringBuilder stBuilder)
        {
            stBuilder.Length = 0; stringBuilderPool.Push(stBuilder);
        }
        public string Text
        {
            get
            {
                StringBuilder stBuilder = GetFreeStringBuilder();
                CopyContentToStringBuilder(stBuilder);
                string output = stBuilder.ToString();
                ReleaseStringBuilder(stBuilder);
                return output;
            }
        }



        //public void SetTextContent(string value, VisualElementArgs vinv)
        //{

        //    internalTextLayerController.Clear();
        //    if (!string.IsNullOrEmpty(value))
        //    {
        //        StringReader reader = new StringReader(value);
        //        string line = reader.ReadLine();
        //        int lineCount = 0;
        //        while (line != null)
        //        {
        //            if (lineCount > 0)
        //            {
        //                internalTextLayerController.SplitCurrentLineIntoNewLine(vinv);
        //            }
        //            lineCount++;
        //            internalTextLayerController.AddTextRunsToCurrentLine(
        //                new ArtEditableVisualTextRun[] { 
        //                    new ArtEditableVisualTextRun(line) }, vinv);
        //            line = reader.ReadLine();
        //        }
        //        internalTextLayerController.DoEnd();
        //    }

        //    EnsureCaretVisible(vinv);

        //    if (textSurfaceEventListener != null)
        //    {
        //        TextSurfaceEventListener.NotifyReplaceAll(textSurfaceEventListener, new TextDomEventArgs(false));
        //    }
        //}
        public int LineCount
        {
            get
            {
                return internalTextLayerController.LineCount;
            }
        }
        public void ReplaceCurrentTextRunContent(int nBackspace, string t)
        {
            VisualElementArgs vinv = GetVInv();

            internalTextLayerController.ReplaceCurrentTextRunContent(nBackspace, t, vinv);

            FreeVInv(vinv);
        }
        public void LoadTextRun(IEnumerable<EditableVisualTextRun> textRuns)
        {
            internalTextLayerController.LoadTextRun(textRuns);
        }
        public void ReplaceCurrentLineTextRuns(IEnumerable<EditableVisualTextRun> textRuns)
        {
            internalTextLayerController.ReplaceCurrentLineTextRun(textRuns);
        }
        public void ReplaceLine(int lineNum, IEnumerable<EditableVisualTextRun> textRuns)
        {
            internalTextLayerController.ReplaceLine(lineNum, textRuns);
        }
        public void CopyCurrentLine(StringBuilder output)
        {
            internalTextLayerController.CopyCurrentLine(output);
        }
        public void CopyLine(int lineNum, StringBuilder output)
        {
            internalTextLayerController.CopyLine(lineNum, output);
        }
        public void CopyContentToStringBuilder(StringBuilder stBuilder)
        {

            internalTextLayerController.CopyAllToPlainText(stBuilder);
        }

        public EditableVisualTextRun CurrentTextRun
        {
            get
            {
                return internalTextLayerController.CurrentTextRun;
            }
        }
        public void GetSelectedText(StringBuilder output)
        {
            internalTextLayerController.CopySelectedTextToPlainText(output);
        }
    }
}