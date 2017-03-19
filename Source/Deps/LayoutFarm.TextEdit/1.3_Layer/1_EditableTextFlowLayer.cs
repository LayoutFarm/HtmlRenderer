//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.Text
{
    partial class EditableTextFlowLayer : RenderElementLayer
    {
        object lineCollection;
        public event EventHandler Reflow;
        TextEditRenderBox owner;
        public EditableTextFlowLayer(TextEditRenderBox owner)
            : base(owner)
        {
            this.owner = owner;
            //start with single line per layer
            //and can change to multiline
            lineCollection = new EditableTextLine(this);
        }
        public TextSpanStyle CurrentTextSpanStyle
        {
            get { return this.owner.CurrentTextSpanStyle; }
        }
        public void SetUseDoubleCanvas(bool useWithWidth, bool useWithHeight)
        {
            this.SetDoubleCanvas(useWithWidth, useWithHeight);
        }

        public bool FlowLayerHasMultiLines
        {
            get
            {
                return (layerFlags & FLOWLAYER_HAS_MULTILINE) != 0;
            }
            private set
            {
                if (value)
                {
                    layerFlags |= FLOWLAYER_HAS_MULTILINE;
                }
                else
                {
                    layerFlags &= ~FLOWLAYER_HAS_MULTILINE;
                }
            }
        }

        internal IEnumerable<EditableRun> GetDrawingIter(EditableRun start, EditableRun stop)
        {
            if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
            {
                List<EditableTextLine> lines = (List<EditableTextLine>)lineCollection;
                int j = lines.Count;
                for (int i = 0; i < j; ++i)
                {
                    LinkedListNode<EditableRun> curNode = lines[i].Last;
                    while (curNode != null)
                    {
                        yield return curNode.Value;
                        curNode = curNode.Previous;
                    }
                }
            }
            else
            {
                EditableTextLine onlyLine = (EditableTextLine)lineCollection;
                LinkedListNode<EditableRun> curNode = onlyLine.Last;
                while (curNode != null)
                {
                    yield return curNode.Value;
                    curNode = curNode.Previous;
                }
            }
        }

        public int LineCount
        {
            get
            {
                if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
                {
                    return ((List<EditableTextLine>)lineCollection).Count;
                }
                else
                {
                    return 1;
                }
            }
        }

        public override void DrawChildContent(Canvas canvas, Rectangle updateArea)
        {
            if ((layerFlags & IS_LAYER_HIDDEN) != 0)
            {
                return;
            }

            this.BeginDrawingChildContent();
            if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
            {
                List<EditableTextLine> lines = (List<EditableTextLine>)lineCollection;
                int renderAreaTop = updateArea.Top;
                int renderAreaBottom = updateArea.Bottom;
                bool foundFirstLine = false;
                int j = lines.Count;
                for (int i = 0; i < j; ++i)
                {
                    EditableTextLine line = lines[i];
#if DEBUG
                    if (this.OwnerRenderElement is RenderBoxBase)
                    {
                        debug_RecordLineInfo((RenderBoxBase)OwnerRenderElement, line);
                    }
#endif

                    int y = line.Top;
                    LinkedListNode<EditableRun> curNode = line.First;
                    if (!foundFirstLine)
                    {
                        if (y + line.ActualLineHeight < renderAreaTop)
                        {
                            continue;
                        }
                        else
                        {
                            foundFirstLine = true;
                        }
                    }
                    else
                    {
                        if (y > renderAreaBottom)
                        {
                            break;
                        }
                    }

                    updateArea.OffsetY(-y);
                    canvas.OffsetCanvasOriginY(y);
                    while (curNode != null)
                    {
                        EditableRun child = curNode.Value;
                        if (child.IntersectOnHorizontalWith(ref updateArea))
                        {
                            int x = child.X;
                            canvas.OffsetCanvasOriginX(x);
                            updateArea.OffsetX(-x);
                            child.DrawToThisCanvas(canvas, updateArea);
                            canvas.OffsetCanvasOriginX(-x);
                            updateArea.OffsetX(x);
                        }
                        curNode = curNode.Next;
                    }
                    canvas.OffsetCanvasOriginY(-y);
                    updateArea.OffsetY(y);
                }
            }
            else
            {
                EditableTextLine line = (EditableTextLine)lineCollection;
#if DEBUG
                if (OwnerRenderElement is RenderBoxBase)
                {
                    debug_RecordLineInfo((RenderBoxBase)OwnerRenderElement, line);
                }
#endif

                LinkedListNode<EditableRun> curNode = line.First;
                if (curNode != null)
                {
                    int y = line.Top;
                    canvas.OffsetCanvasOriginY(y);
                    updateArea.OffsetY(-y);
                    while (curNode != null)
                    {
                        EditableRun child = curNode.Value;
                        if (child.IntersectOnHorizontalWith(ref updateArea))
                        {
                            int x = child.X;
                            canvas.OffsetCanvasOriginX(x);
                            updateArea.OffsetX(-x);
                            child.DrawToThisCanvas(canvas, updateArea);
                            canvas.OffsetCanvasOriginX(-x);
                            updateArea.OffsetX(x);
                        }
                        curNode = curNode.Next;
                    }
                    canvas.OffsetCanvasOriginY(-y);
                    updateArea.OffsetY(y);
                }
            }
            this.FinishDrawingChildContent();
        }

        public override bool HitTestCore(HitChain hitChain)
        {
            if ((layerFlags & IS_LAYER_HIDDEN) == 0)
            {
                if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
                {
                    List<EditableTextLine> lines = (List<EditableTextLine>)lineCollection;
                    int j = lines.Count;
                    int testYPos = hitChain.TestPoint.Y;
                    for (int i = 0; i < j; ++i)
                    {
                        EditableTextLine line = lines[i];
                        if (line.LineBottom < testYPos)
                        {
                            continue;
                        }
                        else if (line.HitTestCore(hitChain))
                        {
                            return true;
                        }
                        else if (line.LineTop > testYPos)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    EditableTextLine onlyLine = (EditableTextLine)lineCollection;
                    return onlyLine.HitTestCore(hitChain);
                }
            }
            return false;
        }

        static Size ReCalculateContentSizeHorizontalFlow(EditableTextFlowLayer layer)
        {
            if (layer.lineCollection == null)
            {
                return Size.Empty;
            }

            FlowFeeder flowFeeder = FlowFeeder.GetNewFlowFeeder();
            flowFeeder.Load(layer);
            int curX = 0;
            int curY = 0;
            bool lastestIsBlock = false;
            int maxHeightInRow = 0;
            int maxWidth = 0;
            int curY_fromTop = 0;
            bool isFirstRunOfLine = true;
            int childCount = 0;
            EditableRun lastNotNullElement = null;
            while (flowFeeder.Read())
            {
                EditableRun currentRun = flowFeeder.CurrentRun;
                if (currentRun != null)
                {
#if DEBUG
                    vinv_dbug_BeginSetElementBound(currentRun);
#endif

                    lastNotNullElement = currentRun;
                    childCount++;
                    if (lastestIsBlock || currentRun.IsBlockElement)
                    {
                        curX = 0;
                        if (!isFirstRunOfLine)
                        {
                            if (maxWidth < curX)
                            {
                                maxWidth = curX;
                            }

                            curY = curY_fromTop + maxHeightInRow;
                            curY_fromTop = curY;
                            maxHeightInRow = 0;
                        }
                        lastestIsBlock = currentRun.IsBlockElement;
                        if (lastestIsBlock)
                        {
                        }
                        if (!currentRun.HasCalculatedSize)
                        {
                            EditableRun.InnerTextRunTopDownReCalculateContentSize(currentRun);
                        }
#if DEBUG
                        else
                        {
                            vinv_dbug_WriteInfo(dbugVisitorMessage.SKIP, currentRun);
                        }
#endif
                        int v_ds_height = currentRun.Height;
                        if (v_ds_height > maxHeightInRow)
                        {
                            maxHeightInRow = v_ds_height;
                        }
                        curX += currentRun.Width;
                        if (curX > maxWidth)
                        {
                            maxWidth = curX;
                        }
                        isFirstRunOfLine = false;
                    }
                    else
                    {
                        lastestIsBlock = currentRun.IsBlockElement;
                        if (!currentRun.HasCalculatedSize)
                        {
                            EditableRun.InnerTextRunTopDownReCalculateContentSize(currentRun);
                        }
#if DEBUG
                        else
                        {
                            vinv_dbug_WriteInfo(dbugVisitorMessage.SKIP, currentRun);
                        }
#endif
                        int v_ds_height = currentRun.Height;
                        if (v_ds_height > maxHeightInRow)
                        {
                            maxHeightInRow = v_ds_height;
                        }
                        curX += currentRun.Width;
                        if (curX > maxWidth)
                        {
                            maxWidth = curX;
                        }
                        isFirstRunOfLine = false;
                    }
#if DEBUG
                    vinv_dbug_EndSetElementBound(currentRun);
#endif
                }
                else
                {
                    if (flowFeeder.ReadState == 1)
                    {
                        curX = 0;
                        curY = curY_fromTop + maxHeightInRow;
                        curY_fromTop = curY;
                        maxHeightInRow = 0;
                    }
                    else
                    {
                    }
                }
            }


            if (curX > maxWidth)
            {
                maxWidth = curX;
            }

            int finalHeight = curY_fromTop + maxHeightInRow;
            FlowFeeder.FreeFlowFeeder(flowFeeder);
            return new Size(maxWidth, finalHeight);
        }

        public override void TopDownReArrangeContent()
        {
            //vinv_IsInTopDownReArrangePhase = true;
#if DEBUG
            vinv_dbug_EnterLayerReArrangeContent(this);
#endif
            //this.BeginLayerLayoutUpdate();

            RenderBoxBase container = this.OwnerRenderElement as RenderBoxBase;
            if (container != null)
            {
                PerformHorizontalFlowArrange(0, container.Width, 0);
            }

            if (Reflow != null)
            {
                Reflow(this, EventArgs.Empty);
            }

            //this.EndLayerLayoutUpdate();
#if DEBUG
            vinv_dbug_ExitLayerReArrangeContent();
#endif
        }


        public override void TopDownReCalculateContentSize()
        {
#if DEBUG

            vinv_dbug_EnterLayerReCalculateContent(this);
#endif
            if (this.LineCount > 1)
            {
                List<EditableTextLine> lines = (List<EditableTextLine>)this.lineCollection;
                EditableTextLine lastline = lines[lines.Count - 1];
                SetPostCalculateLayerContentSize(lastline.ActualLineWidth, lastline.ActualLineHeight + lastline.LineTop);
            }
            else
            {
                SetPostCalculateLayerContentSize(ReCalculateContentSizeHorizontalFlow(this));
            }
#if DEBUG
            vinv_dbug_ExitLayerReCalculateContent();
#endif
        }


        internal EditableTextLine GetTextLine(int lineId)
        {
            List<EditableTextLine> lines = lineCollection as List<EditableTextLine>;
            if (lines != null)
            {
                if (lineId < lines.Count)
                {
                    return lines[lineId];
                }
            }
            else if (lineId == 0)
            {
                return (EditableTextLine)lineCollection;
            }

            return null;
        }



        internal EditableTextLine GetTextLineAtPos(int y)
        {
            if (lineCollection != null)
            {
                if (lineCollection is List<EditableTextLine>)
                {
                    List<EditableTextLine> lines = lineCollection as List<EditableTextLine>;
                    if (lines != null)
                    {
                        int j = lines.Count;
                        for (int i = 0; i < j; ++i)
                        {
                            EditableTextLine line = lines[i];
                            if (line.IntersectsWith(y))
                            {
                                return line;
                            }
                        }
                    }
                }
                else
                {
                    EditableTextLine line = (EditableTextLine)lineCollection;
                    if (line.IntersectsWith(y))
                    {
                        return line;
                    }
                }
            }
            return null;
        }
        void AppendLine(EditableTextLine line)
        {
            if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
            {
                List<EditableTextLine> lines = (List<EditableTextLine>)lineCollection;
                int lineCount = lines.Count;
                EditableTextLine lastLine = lines[lineCount - 1];
                line.SetLineNumber(lineCount);
                line.SetTop(lastLine.Top + lastLine.ActualLineHeight);
                lines.Add(line);
            }
            else
            {
                EditableTextLine onlyLine = (EditableTextLine)lineCollection;
                List<EditableTextLine> newLineList = new List<EditableTextLine>();
                newLineList.Add(onlyLine);
                line.SetTop(onlyLine.ActualLineHeight);
                line.SetLineNumber(1);
                newLineList.Add(line);
                lineCollection = newLineList;
                FlowLayerHasMultiLines = true;
            }
        }
        void PerformHorizontalFlowArrangeForMultilineText(
            int ownerClientLeft, int ownerClientWidth,
            int ownerClientTop)
        {
#if DEBUG
            long startTick = DateTime.Now.Ticks;
#endif

            List<EditableTextLine> lines = (List<EditableTextLine>)this.lineCollection;
            int ownerClientRight = ownerClientLeft + ownerClientWidth;
            int curX = 0;
            int curY = 0;
            bool lastestIsBlock = false;
            int maxWidth = 0;
            int curY_fromTop = ownerClientTop;
            int maxHeightInRow = EditableTextLine.DEFAULT_LINE_HEIGHT;
            int lineCount = lines.Count;
            for (int i = 0; i < lineCount; ++i)
            {
                EditableTextLine line = lines[i];
                curX = ownerClientLeft;
                lastestIsBlock = false;
                line.SetTop(curY_fromTop);
                if (!line.NeedArrange)
                {
                    maxHeightInRow = line.ActualLineHeight;
                    if (line.ActualLineWidth > maxWidth)
                    {
                        maxWidth = line.ActualLineWidth;
                    }
                }
                else
                {
                    maxHeightInRow = EditableTextLine.DEFAULT_LINE_HEIGHT;
                    EditableTextLine newLine = null;
                    line.ValidateContentArrangement();
                    bool isFirstRunInThisLine = true;
                    foreach (EditableRun currentRun in line)
                    {
#if DEBUG
                        vinv_dbug_BeginSetElementBound(currentRun);
#endif
                        int v_desired_width = currentRun.Width;
                        int v_desired_height = currentRun.Height;
                        if (isFirstRunInThisLine)
                        {
                            lastestIsBlock = currentRun.IsBlockElement;
                            if (v_desired_height > maxHeightInRow)
                            {
                                maxHeightInRow = v_desired_height;
                            }
                            EditableRun.DirectSetLocation(currentRun, curX, 0);
                            if (v_desired_height > maxHeightInRow)
                            {
                                maxHeightInRow = v_desired_height;
                            }
                            if (lastestIsBlock)
                            {
                                v_desired_width = ownerClientWidth;
                            }

                            EditableRun.DirectSetSize(currentRun,
                                    v_desired_width, v_desired_height);
                            currentRun.MarkValidContentArrangement();
                            curX += v_desired_width;
                            isFirstRunInThisLine = false;
                        }
                        else
                        {
                            if (lastestIsBlock || currentRun.IsBlockElement ||
                            (curX + v_desired_width > ownerClientRight))
                            {
                                newLine = new EditableTextLine(this);
                                newLine.AddLast(currentRun);
                                curY = curY_fromTop + maxHeightInRow;
                                curY_fromTop = curY;
                                maxHeightInRow = EditableTextLine.DEFAULT_LINE_HEIGHT;
                                EditableRun nextR = currentRun.NextTextRun;
                                while (nextR != null)
                                {
                                    line.UnsafeRemoveVisualElement(nextR);
                                    newLine.AddLast(nextR);
                                    nextR = nextR.NextTextRun;
                                }
                                if (i + 1 == lineCount)
                                {
                                    lines.Add(newLine);
                                }
                                else
                                {
                                    lines.Insert(i + 1, newLine);
                                }
                                lineCount++;
                                break;
                            }
                            else
                            {
                                lastestIsBlock = currentRun.IsBlockElement;
                                if (v_desired_height > maxHeightInRow)
                                {
                                    maxHeightInRow = v_desired_height;
                                }
                                EditableRun.DirectSetLocation(currentRun, curX, 0);
                                EditableRun.DirectSetSize(currentRun,
                                       v_desired_width, v_desired_height);
                                currentRun.MarkValidContentArrangement();
                                curX += v_desired_width;
                            }
                        }

#if DEBUG
                        vinv_dbug_EndSetElementBound(currentRun);
#endif

                    }
                    if (curX > maxWidth)
                    {
                        maxWidth = curX;
                    }
                }
                line.SetPostArrangeLineSize(maxWidth, maxHeightInRow);
                curY = curY_fromTop + maxHeightInRow;
                curY_fromTop = curY;
            }

            ValidateArrangement();
        }


        void PerformHorizontalFlowArrange(
            int ownerClientLeft, int ownerClientWidth,
            int ownerClientTop)
        {
            if (lineCollection == null)
            {
                return;
            }
            if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
            {
                PerformHorizontalFlowArrangeForMultilineText(ownerClientLeft,
                ownerClientWidth, ownerClientTop);
                return;
            }
            int ownerClientRight = ownerClientLeft + ownerClientWidth;
            int default_line_height = EditableTextLine.DEFAULT_LINE_HEIGHT;
            int curX = 0;
            int curY = 0;
            bool lastestIsBlock = false;
            int maxHeightInRow = 2;
            int maxWidth = 0;
            int curY_fromTop = ownerClientTop;
#if DEBUG
            long startTick = DateTime.Now.Ticks;
#endif


            FlowReLocator flowRelocator = FlowReLocator.GetNewFlowRelocator();
            flowRelocator.Load(this);
            EditableRun lastNotNullElement = null;
            int childCount = 0;
            while (flowRelocator.ReadNextRun())
            {
                EditableRun currentRun = flowRelocator.CurrentRun;
                if (currentRun != null)
                {
                    lastNotNullElement = currentRun;
                    childCount++;
#if DEBUG
                    vinv_dbug_BeginSetElementBound(currentRun);
#endif

                    int v_desired_width = currentRun.Width;
                    int v_desired_height = currentRun.Height;
                    if (lastestIsBlock || currentRun.IsBlockElement ||
                       (curX + v_desired_width > ownerClientRight))
                    {
                        if (!flowRelocator.IsFirstRunOfLine)
                        {
                            flowRelocator.CloseCurrentLine(curX, maxHeightInRow);
                            if (maxWidth < curX)
                            {
                                maxWidth = curX;
                            }
                            curY = curY_fromTop + maxHeightInRow;
                            curY_fromTop = curY;
                            maxHeightInRow = default_line_height;
                            flowRelocator.SetCurrentLineTop(curY);
                        }

                        curX = ownerClientLeft;
                        flowRelocator.Accept();
                        lastestIsBlock = currentRun.IsBlockElement;
                        if (v_desired_height > maxHeightInRow)
                        {
                            maxHeightInRow = v_desired_height;
                        }
                        EditableRun.DirectSetLocation(currentRun, curX, 0);
                        if (lastestIsBlock)
                        {
                            v_desired_width = flowRelocator.OwnerElementWidth;
                        }

                        EditableRun.DirectSetSize(currentRun,
                            v_desired_width, v_desired_height);
                        curX += v_desired_width;
                    }
                    else
                    {
                        flowRelocator.Accept();
                        lastestIsBlock = currentRun.IsBlockElement;
                        if (v_desired_height > maxHeightInRow)
                        {
                            maxHeightInRow = v_desired_height;
                        }
                        EditableRun.DirectSetLocation(currentRun, curX, 0);
                        EditableRun.DirectSetSize(
                            currentRun,
                            v_desired_width,
                            v_desired_height);
                        curX += v_desired_width;
                    }
#if DEBUG
                    vinv_dbug_EndSetElementBound(currentRun);
#endif
                }
                else
                {
                    switch (flowRelocator.FeederState)
                    {
                        case 1:
                            {
                                if (maxWidth < curX)
                                {
                                    maxWidth = curX;
                                }
                                flowRelocator.CloseCurrentLineWithLineBreak(curX, maxHeightInRow);
                                curX = ownerClientLeft;
                                curY = curY_fromTop + maxHeightInRow;
                                curY_fromTop = curY;
                                maxHeightInRow = default_line_height;
                                flowRelocator.SetCurrentLineTop(curY);
                            }
                            break;
                        case 4:
                            {
                                if (maxWidth < curX)
                                {
                                    maxWidth = curX;
                                }
                                flowRelocator.SetCurrentLineSize(curX, maxHeightInRow);
                                curX = ownerClientLeft;
                            }
                            break;
                    }
                }
            }


            if (curX > maxWidth)
            {
                maxWidth = curX;
            }


            int finalHeight = curY_fromTop + maxHeightInRow;
            ValidateArrangement();
            FlowReLocator.FreeFlowRelocator(flowRelocator);
        }


        internal EditableTextLine InsertNewLine(int insertAt)
        {
            EditableTextLine newLine = new EditableTextLine(this);
            this.InsertLine(insertAt, newLine);
            return newLine;
        }
        void InsertLine(int insertAt, EditableTextLine textLine)
        {
            if (insertAt < 0)
            {
                throw new NotSupportedException();
            }

            List<EditableTextLine> lines = lineCollection as List<EditableTextLine>;
            if (lines != null)
            {
                int j = lines.Count;
                if (insertAt > j - 1)
                {
                    AppendLine(textLine);
                }
                else
                {
                    EditableTextLine line = lines[insertAt];
                    int cy = line.Top;
                    textLine.SetTop(cy);
                    textLine.SetLineNumber(insertAt);
                    cy += line.ActualLineHeight;
                    for (int i = insertAt; i < j; i++)
                    {
                        line = lines[i];
                        line.SetTop(cy);
                        line.SetLineNumber(i + 1);
                        cy += line.ActualLineHeight;
                    }
                    textLine.editableFlowLayer = this;
                    lines.Insert(insertAt, textLine);
                }
            }
            else
            {
                lines = new List<EditableTextLine>();
                lines.Add((EditableTextLine)lineCollection);
                lineCollection = lines;
                FlowLayerHasMultiLines = true;
                int j = lines.Count;
                if (insertAt > j - 1)
                {
                    AppendLine(textLine);
                }
                else
                {
                    EditableTextLine line = lines[insertAt];
                    int cy = line.Top;
                    textLine.SetTop(cy);
                    textLine.SetLineNumber(insertAt);
                    cy += line.ActualLineHeight;
                    for (int i = insertAt; i < j; i++)
                    {
                        line = lines[i];
                        line.SetTop(cy);
                        line.SetLineNumber(i + 1);
                        cy += line.ActualLineHeight;
                    }
                    textLine.editableFlowLayer = this;
                    lines.Insert(insertAt, textLine);
                }
            }
        }



        public void CopyContentToStringBuilder(StringBuilder stBuilder)
        {
            List<EditableTextLine> lines = lineCollection as List<EditableTextLine>;
            if (lines != null)
            {
                int j = lines.Count;
                int n = j - 1;
                for (int i = 0; i < j; ++i)
                {
                    lines[i].CopyLineContent(stBuilder);
                    if (i < n)
                    {
                        stBuilder.Append('\n');
                    }
                }
            }
            else
            {
                ((EditableTextLine)lineCollection).CopyLineContent(stBuilder);
            }
        }

        internal IEnumerable<EditableRun> TextRunForward(EditableRun startRun, EditableRun stopRun)
        {
            EditableTextLine currentLine = startRun.OwnerEditableLine;
            EditableTextLine stopLine = stopRun.OwnerEditableLine;
            if (currentLine == stopLine)
            {
                foreach (EditableRun r in currentLine.GetVisualElementForward(startRun, stopRun))
                {
                    yield return r;
                }
            }
            else
            {
                foreach (EditableRun r in currentLine.GetVisualElementForward(startRun))
                {
                    yield return r;
                }
                currentLine = currentLine.Next;
                while (currentLine != null)
                {
                    if (currentLine == stopLine)
                    {
                        foreach (EditableRun r in currentLine)
                        {
                            if (r == stopRun)
                            {
                                break;
                            }
                            else
                            {
                                yield return r;
                            }
                        }
                        break;
                    }
                    else
                    {
                        foreach (EditableRun r in currentLine)
                        {
                            yield return r;
                        }
                        currentLine = currentLine.Next;
                    }
                }
            }
        }
        internal void Reload(IEnumerable<EditableRun> runs)
        {
            Clear();
            foreach (EditableRun run in runs)
            {
                AddTop(run);
            }
        }

#if DEBUG
        void debug_RecordLineInfo(RenderBoxBase owner, EditableTextLine line)
        {
            RootGraphic visualroot = this.dbugVRoot;
            if (visualroot.dbug_RecordDrawingChain)
            {
            }
        }

        public override void dbug_DumpElementProps(dbugLayoutMsgWriter writer)
        {
            writer.Add(new dbugLayoutMsg(
                this, this.ToString()));
            writer.EnterNewLevel();
            foreach (EditableRun child in this.dbugGetDrawingIter2())
            {
                child.dbug_DumpVisualProps(writer);
            }
            writer.LeaveCurrentLevel();
        }
        public override string ToString()
        {
            return "editable flow layer " + "(L" + dbug_layer_id + this.dbugLayerState + ") postcal:" +
                this.PostCalculateContentSize.ToString() + " of " + this.OwnerRenderElement.dbug_FullElementDescription();
        }
        public IEnumerable<EditableRun> dbugGetDrawingIter2()
        {
            if ((layerFlags & FLOWLAYER_HAS_MULTILINE) != 0)
            {
                List<EditableTextLine> lines = (List<EditableTextLine>)lineCollection;
                int j = lines.Count;
                for (int i = 0; i < j; ++i)
                {
                    LinkedListNode<EditableRun> curNode = lines[i].First;
                    while (curNode != null)
                    {
                        yield return curNode.Value;
                        curNode = curNode.Next;
                    }
                }
            }
            else
            {
                EditableTextLine onlyLine = (EditableTextLine)lineCollection;
                LinkedListNode<EditableRun> curNode = onlyLine.First;
                while (curNode != null)
                {
                    yield return curNode.Value;
                    curNode = curNode.Next;
                }
            }
        }

#endif
    }
}