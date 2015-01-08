// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using System.Diagnostics;
using LayoutFarm.RenderBoxes;
 
namespace LayoutFarm.Text
{



#if DEBUG
    [DebuggerDisplay("ELN {dbugShortLineInfo}")]
#endif
    public sealed partial class EditableVisualElementLine : LinkedList<EditableTextSpan>
    {

        int currentLineNumber;
        internal EditableTextFlowLayer editableFlowLayer;

        int actualLineHeight;
        int actualLineWidth;
        int lineTop;
        int lineFlags;

        const int LINE_CONTENT_ARRANGED = 1 << (1 - 1);
        const int LINE_SIZE_VALID = 1 << (2 - 1);
        const int LOCAL_SUSPEND_LINE_REARRANGE = 1 << (3 - 1);
        const int END_WITH_LINE_BREAK = 1 << (4 - 1);
        public const int DEFAULT_LINE_HEIGHT = 17;

#if DEBUG
        static int dbugLineTotalCount = 0;
        internal int dbugLineId;
#endif
        internal EditableVisualElementLine(EditableTextFlowLayer ownerFlowLayer)
        {

            this.editableFlowLayer = ownerFlowLayer;
            this.actualLineHeight = DEFAULT_LINE_HEIGHT;
#if DEBUG
            this.dbugLineId = dbugLineTotalCount;
            dbugLineTotalCount++;
#endif
        }
        RootGraphic Root
        {
            get { return this.OwnerElement.Root; }
        }
        internal EditableTextSpan LastRun
        {
            get
            {
                if (this.Count > 0)
                {
                    return this.Last.Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public void TextLineReCalculateActualLineSize()
        {
            EditableTextSpan r = this.FirstRun;
            int maxHeight = 2;
            int lw = 0;
            while (r != null)
            {
                if (r.Height > maxHeight)
                {
                    maxHeight = r.Height;
                }
                lw += r.Width;
                r = r.NextTextRun;
            }
            this.actualLineWidth = lw;
            this.actualLineHeight = maxHeight;
        }


        internal bool PrepareRenderingChain(VisualDrawingChain chain)
        {
            if (this.Count == 0)
            {
                return false;
            }
            else
            {
                LinkedListNode<EditableTextSpan> cnode = this.First;
                chain.OffsetCanvasOriginY(this.lineTop);

                while (cnode != null)
                {
                    if (cnode.Value.PrepareDrawingChain(chain))
                    {
                        chain.OffsetCanvasOriginY(-this.lineTop);
                        return true;
                    }
                    cnode = cnode.Next;
                }

                chain.OffsetCanvasOriginY(-this.lineTop);
                return false;
            }
        }

        internal bool HitTestCore(HitChain hitChain)
        {

            int testX;
            int testY;
            hitChain.GetTestPoint(out testX, out testY);

            if (this.Count == 0)
            {
                return false;
            }
            else
            {


                LinkedListNode<EditableTextSpan> cnode = this.First;

                int curLineTop = this.lineTop;
                hitChain.OffsetTestPoint(0, -curLineTop);
                while (cnode != null)
                {
                    if (cnode.Value.HitTestCore(hitChain))
                    {
                        hitChain.OffsetTestPoint(0, curLineTop);
                        return true;
                    }
                    cnode = cnode.Next;
                }
                hitChain.OffsetTestPoint(0, curLineTop);
                return false;
            }
        }

        public RenderElement OwnerElement
        {
            get
            {
                if (editableFlowLayer != null)
                {
                    return editableFlowLayer.OwnerRenderElement;
                }
                else
                {
                    return null;
                }
            }
        }
        public EditableTextFlowLayer OwnerFlowLayer
        {
            get
            {
                return this.editableFlowLayer;
            }
        }
        public bool EndWithLineBreak
        {
            get
            {
                return (lineFlags & END_WITH_LINE_BREAK) != 0;
            }
            set
            {
                if (value)
                {
                    lineFlags |= END_WITH_LINE_BREAK;
                }
                else
                {
                    lineFlags &= ~END_WITH_LINE_BREAK;
                }
            }
        }

        public bool IntersectsWith(int y)
        {
            return y >= lineTop && y < (lineTop + actualLineHeight);
        }
        public int LineTop
        {
            get
            {
                return lineTop;
            }
        }
        public int ActualLineHeight
        {
            get
            {
                return actualLineHeight;
            }
        }
        public int ActualLineWidth
        {
            get
            {
                return actualLineWidth;
            }
        }
        public Rectangle ActualLineArea
        {
            get
            {
                return new Rectangle(0, lineTop, actualLineWidth, actualLineHeight);
            }
        }
        public Rectangle ParentLineArea
        {
            get
            {
                return new Rectangle(0, lineTop, this.editableFlowLayer.OwnerRenderElement.Width, 17);
            }
        }
        internal IEnumerable<EditableTextSpan> GetVisualElementForward(EditableTextSpan startVisualElement)
        {
            if (startVisualElement != null)
            {
                yield return startVisualElement;
                var curRun = startVisualElement.NextTextRun;
                while (curRun != null)
                {
                    yield return curRun;
                    curRun = curRun.NextTextRun;
                }
            }
        }
        internal IEnumerable<EditableTextSpan> GetVisualElementForward(EditableTextSpan startVisualElement, EditableTextSpan stopVisualElement)
        {

            if (startVisualElement != null)
            {

                LinkedListNode<EditableTextSpan> lexnode = GetLineLinkedNode(startVisualElement);

                while (lexnode != null)
                {
                    yield return lexnode.Value;
                    if (lexnode.Value == stopVisualElement)
                    {
                        break;
                    }
                    lexnode = lexnode.Next;
                }
            }
        }



        public int CharCount
        {
            get
            {
                int charCount = 0;
                foreach (EditableTextSpan r in this)
                {
                    charCount += r.CharacterCount;
                }
                return charCount;
            }
        }

        public int LineBottom
        {
            get
            {
                return lineTop + actualLineHeight;
            }
        }

        internal int CurrentWidth
        {
            get
            {
                var lastRun = this.LastRun;
                if (lastRun == null)
                {
                    return 0;
                }
                else
                {
                    return lastRun.Right;
                }

            }
        }

        public int Top
        {
            get
            {
                return lineTop;
            }
        }

        public void SetTop(int linetop)
        {
            this.lineTop = linetop;
        }
#if DEBUG
        public override string ToString()
        {
            return this.dbugShortLineInfo;
        }
        public string dbugShortLineInfo
        {
            get
            {
                return "LINE[" + dbugLineId + "]:" + this.currentLineNumber + "{T:" + lineTop.ToString() + ",W:" +
                   actualLineWidth + ",H:" + this.actualLineHeight + "}";
            }
        }
#endif
        public int LineNumber
        {
            get
            {
                return currentLineNumber;
            }
        }
        internal void SetLineNumber(int value)
        {
            this.currentLineNumber = value;
        }
        bool IsFirstLine
        {
            get
            {
                return currentLineNumber == 0;
            }
        }
        bool IsLastLine
        {
            get
            {

                return currentLineNumber == editableFlowLayer.LineCount - 1;
            }
        }
        bool IsSingleLine
        {
            get
            {

                return IsFirstLine && IsLastLine;
            }
        }
        public bool IsBlankLine
        {
            get
            {
                return Count == 0;
            }
        }
        public EditableVisualElementLine Next
        {
            get
            {
                if (currentLineNumber < editableFlowLayer.LineCount - 1)
                {
                    return editableFlowLayer.GetTextLine(currentLineNumber + 1);
                }
                else
                {
                    return null;
                }

            }
        }
        public EditableVisualElementLine Prev
        {
            get
            {
                if (currentLineNumber > 0)
                {
                    return editableFlowLayer.GetTextLine(currentLineNumber - 1);
                }
                else
                {
                    return null;
                }
            }
        }

        public EditableTextSpan FirstRun
        {
            get
            {
                if (this.Count > 0)
                {
                    return this.First.Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public bool NeedArrange
        {
            get
            {
                return (lineFlags & LINE_CONTENT_ARRANGED) == 0;
            }
        }
        internal void ValidateContentArrangement()
        {
            lineFlags |= LINE_CONTENT_ARRANGED;
        }
        public static void InnerCopyLineContent(EditableVisualElementLine line, StringBuilder stBuilder)
        {
            line.CopyLineContent(stBuilder);
        }
        public void CopyLineContent(StringBuilder stBuilder)
        {
            LinkedListNode<EditableTextSpan> curNode = this.First;
            while (curNode != null)
            {
                EditableTextSpan v = curNode.Value;
                v.CopyContentToStringBuilder(stBuilder);
                curNode = curNode.Next;
            }
        }
        internal bool IsLocalSuspendLineRearrange
        {
            get
            {
                return (this.lineFlags & LOCAL_SUSPEND_LINE_REARRANGE) != 0;
            }
        }

        internal void InvalidateLineLayout()
        {
            this.lineFlags &= ~LINE_SIZE_VALID;
            this.lineFlags &= ~LINE_CONTENT_ARRANGED;
        }



    }



}