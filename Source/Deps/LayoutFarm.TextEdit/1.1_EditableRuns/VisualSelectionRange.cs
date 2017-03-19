//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
namespace LayoutFarm.Text
{
    public struct VisualSelectionRangeSnapShot
    {
        public readonly int startLineNum;
        public readonly int startColumnNum;
        public readonly int endLineNum;
        public readonly int endColumnNum;
        public VisualSelectionRangeSnapShot(int startLineNum, int startColumnNum, int endLineNum, int endColumnNum)
        {
            this.startLineNum = startLineNum;
            this.startColumnNum = startColumnNum;
            this.endLineNum = endLineNum;
            this.endColumnNum = endColumnNum;
        }
        public bool IsEmpty()
        {
            return startLineNum == 0 && startColumnNum == 0
                && endLineNum == 0 && endColumnNum == 0;
        }
        public static readonly VisualSelectionRangeSnapShot Empty = new VisualSelectionRangeSnapShot();
    }

    class VisualSelectionRange
    {
        EditableVisualPointInfo startPoint = null;
        EditableVisualPointInfo endPoint = null;
        public VisualSelectionRange(EditableVisualPointInfo startPoint, EditableVisualPointInfo endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }
        public EditableVisualPointInfo StartPoint
        {
            get
            {
                return startPoint;
            }
            set
            {
                startPoint = value;
            }
        }
        public EditableVisualPointInfo EndPoint
        {
            get
            {
                return endPoint;
            }
            set
            {
                if (startPoint != null)
                {
                    endPoint = value;
                }
                else
                {
                    endPoint = null;
                }
            }
        }
        public bool IsOnTheSameLine
        {
            get
            {
                return startPoint.LineId == endPoint.LineId;
            }
        }


        public void SwapIfUnOrder()
        {
            if (IsOnTheSameLine)
            {
                if (startPoint.LineCharIndex > endPoint.LineCharIndex)
                {
                    EditableVisualPointInfo tmpPoint = startPoint;
                    startPoint = endPoint;
                    endPoint = tmpPoint;
                }
            }
            else
            {
                if (startPoint.LineId > endPoint.LineId)
                {
                    EditableVisualPointInfo tmp = startPoint;
                    startPoint = endPoint;
                    endPoint = tmp;
                }
            }
        }

        public bool IsValid
        {
            get
            {
                if (startPoint != null && endPoint != null)
                {
                    if ((startPoint.TextRun != null && !startPoint.TextRun.HasParent) ||
                        (endPoint.TextRun != null && !endPoint.TextRun.HasParent))
                    {
                        throw new NotSupportedException("text range err");
                    }
                    if ((startPoint.LineCharIndex == endPoint.LineCharIndex) &&
                    (startPoint.LineId == endPoint.LineId))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        public EditableVisualPointInfo TopEnd
        {
            get
            {
                switch (startPoint.LineId.CompareTo(endPoint.LineId))
                {
                    case -1:
                        return startPoint;
                    case 0:
                        if (startPoint.X <= endPoint.X)
                        {
                            return startPoint;
                        }
                        else
                        {
                            return endPoint;
                        }
                }
                return endPoint;
            }
        }
        public EditableVisualPointInfo BottomEnd
        {
            get
            {
                switch (startPoint.LineId.CompareTo(endPoint.LineId))
                {
                    case -1:
                        return endPoint;
                    case 0:
                        if (endPoint.X > startPoint.X)
                        {
                            return endPoint;
                        }
                        else
                        {
                            return startPoint;
                        }
                }
                return startPoint;
            }
        }
        public void Draw(Canvas destPage, Rectangle updateArea)
        {
            if (IsOnTheSameLine)
            {
                VisualPointInfo topEndPoint = TopEnd;
                VisualPointInfo bottomEndPoint = BottomEnd;
                int linetop = topEndPoint.LineTop;
                destPage.FillRectangle(Color.LightGray, topEndPoint.X, linetop,
                    bottomEndPoint.X - topEndPoint.X, topEndPoint.ActualLineHeight);
            }
            else
            {
                EditableVisualPointInfo topEndPoint = TopEnd;
                int lineYPos = topEndPoint.LineTop;
                destPage.FillRectangle(Color.LightGray, topEndPoint.X, lineYPos,
                    topEndPoint.CurrentWidth - topEndPoint.X,
                    topEndPoint.ActualLineHeight);
                int topLineId = topEndPoint.LineId;
                int bottomLineId = BottomEnd.LineId;
                if (bottomLineId - topLineId > 1)
                {
                    EditableTextLine adjacentStartLine = topEndPoint.EditableLine.Next;
                    while (adjacentStartLine != BottomEnd.Line)
                    {
                        destPage.FillRectangle(Color.LightGray, 0,
                            adjacentStartLine.LineTop,
                            adjacentStartLine.CurrentWidth,
                            adjacentStartLine.ActualLineHeight);
                        adjacentStartLine = adjacentStartLine.Next;
                    }
                    EditableTextLine adjacentStopLine = BottomEnd.Line.Prev;
                }
                VisualPointInfo bottomEndPoint = BottomEnd;
                lineYPos = bottomEndPoint.LineTop;
                destPage.FillRectangle(Color.LightGray, 0, lineYPos, bottomEndPoint.X,
                     bottomEndPoint.ActualLineHeight);
            }
        }
        public void UpdateSelectionRange()
        {
            if (startPoint.TextRun != null && !startPoint.TextRun.HasParent)
            {
                EditableTextLine startLine = startPoint.EditableLine;
                startPoint = startLine.GetTextPointInfoFromCharIndex(startPoint.LineCharIndex);
            }
            if (endPoint.TextRun != null && !endPoint.TextRun.HasParent)
            {
                EditableTextLine stopLine = endPoint.EditableLine;
                endPoint = stopLine.GetTextPointInfoFromCharIndex(endPoint.LineCharIndex);
            }
        }

        public IEnumerable<EditableRun> GetPrintableTextRunIter()
        {
            EditableRun startRun = null;
            if (startPoint.TextRun == null)
            {
                EditableTextLine line = startPoint.EditableLine;
                startRun = line.FirstRun;
            }
            else
            {
                startRun = startPoint.TextRun.NextTextRun;
            }

            EditableTextFlowLayer layer = startRun.OwnerEditableLine.editableFlowLayer;
            foreach (EditableRun t in layer.GetDrawingIter(startRun, endPoint.TextRun))
            {
                if (!t.IsLineBreak)
                {
                    yield return t;
                }
            }
        }
        public VisualSelectionRangeSnapShot GetSelectionRangeSnapshot()
        {
            return new VisualSelectionRangeSnapShot(
                startPoint.LineNumber,
                startPoint.LineCharIndex,
                endPoint.LineNumber,
                endPoint.LineCharIndex);
        }
#if DEBUG
        public override string ToString()
        {
            StringBuilder stBuilder = new StringBuilder();
            if (this.IsValid)
            {
                stBuilder.Append("sel");
            }
            else
            {
                stBuilder.Append("!sel");
            }
            stBuilder.Append(startPoint.ToString());
            stBuilder.Append(',');
            stBuilder.Append(endPoint.ToString());
            return stBuilder.ToString();
        }
#endif
    }
}

