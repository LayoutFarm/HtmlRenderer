// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.Text
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
        public TextSurfaceEventListener TextSurfaceListener
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
            this.MyLayers.ClearAllContentInEachLayer();
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
        public string GetTextContent()
        {    
            StringBuilder stBuilder = GetFreeStringBuilder();
            CopyContentToStringBuilder(stBuilder);
            string output = stBuilder.ToString();
            ReleaseStringBuilder(stBuilder);
            return output; 
        }
        public void SetTextContent(string textContent)
        {
            //clear existing content
            this.ClearAllChildren();
            if (textContent == null)
            {
                return;
            }

            System.IO.StringReader reader = new System.IO.StringReader(textContent);
            string line = reader.ReadLine();
            int lineCount = 0;
            while (line != null)
            {
                if (lineCount > 0)
                {
                    internalTextLayerController.SplitCurrentLineIntoNewLine();
                }
                lineCount++;
                internalTextLayerController.AddTextRunToCurrentLine(
                   new EditableTextSpan(Root, line, this.currentSpanStyle));
                line = reader.ReadLine();
            } 
        } 
        public int LineCount
        {
            get
            {
                return internalTextLayerController.LineCount;
            }
        }
        public void ReplaceCurrentTextRunContent(int nBackspace, string t)
        {
            internalTextLayerController.ReplaceCurrentTextRunContent(nBackspace, t);
        }
        public void LoadTextRun(IEnumerable<EditableTextSpan> textRuns)
        {
            internalTextLayerController.LoadTextRun(textRuns);
        }
        public void ReplaceCurrentLineTextRuns(IEnumerable<EditableTextSpan> textRuns)
        {
            internalTextLayerController.ReplaceCurrentLineTextRun(textRuns);
        }
        public void ReplaceLine(int lineNum, IEnumerable<EditableTextSpan> textRuns)
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

        public EditableTextSpan CurrentTextRun
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