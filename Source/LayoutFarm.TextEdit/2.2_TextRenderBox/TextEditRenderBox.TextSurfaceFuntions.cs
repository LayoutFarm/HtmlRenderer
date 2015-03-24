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

        public void SplitCurrentLineToNewLine()
        {
            this.internalTextLayerController.SplitCurrentLineIntoNewLine();
        }
        public void AddTextRun(EditableTextSpan textspan)
        {
            internalTextLayerController.AddRun(textspan);
        }
        public EditableTextSpan CreateNewTextSpan(string str)
        {
            return new EditableTextSpan(this.Root, str, this.currentSpanStyle);
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