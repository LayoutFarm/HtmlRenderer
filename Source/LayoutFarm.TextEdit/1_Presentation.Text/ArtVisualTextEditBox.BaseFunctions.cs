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

    public sealed partial class ArtVisualTextEditBox : ArtVisualContainerBase
    {

        EditableTextFlowLayer textLayer;

        InternalTextLayerController internalTextLayerController;
        int verticalExpectedCharIndex; bool isMultiLine = false;
        bool isInVerticalPhase = false;

         

        public ArtVisualTextEditBox(int width, int height, bool isMultiLine) :
            base(width, height, VisualElementNature.TextEditContainer)
        {
            RegisterNativeEvent((1 << ArtEventIdentifier.NE_DRAG_START)
                | (1 << ArtEventIdentifier.NE_DRAGING)
                | (1 << ArtEventIdentifier.NE_DRAG_STOP)
                | (1 << ArtEventIdentifier.NE_MOUSE_DOWN)
                | (1 << ArtEventIdentifier.NE_MOUSE_MOVE)
                | (1 << ArtEventIdentifier.NE_MOUSE_HOVER)
                | (1 << ArtEventIdentifier.NE_MOUSE_UP)
                | (1 << ArtEventIdentifier.NE_DBLCLICK)
                | (1 << ArtEventIdentifier.NE_KEY_DOWN)
                | (1 << ArtEventIdentifier.NE_KEY_PRESS));


            textLayer = new EditableTextFlowLayer(this);

            InnerSetHasSubGroupLayer(this, true);
            internalTextLayerController = new InternalTextLayerController(this, textLayer);

            this.isMultiLine = isMultiLine;
            if (isMultiLine)
            {
                textLayer.SetUseDoubleCanvas(false, true);

            }
            else
            {
                textLayer.SetUseDoubleCanvas(true, false);
            }
            this.IsBlockElement = false;

            this.IsScrollable = true;
        }

        protected override bool HasGroundLayer()
        {
            return true;
        }
        protected override VisualLayer GetGroundLayer()
        {
            return textLayer;
        }

        public TextMan TextMan
        {
            get
            {
                return this.internalTextLayerController.TextMan;
            }
        }
        public static void NotifyTextContentSizeChanged(ArtVisualTextEditBox ts)
        {
            ts.ContainerBaseEvaluateScrollBar();
        }

        public Rectangle GetRectAreaOf(int beginlineNum, int beginColumnNum, int endLineNum, int endColumnNum)
        {

            EditableTextFlowLayer flowLayer = this.textLayer;
            EditableVisualElementLine beginLine = flowLayer.GetTextLineAtPos(beginlineNum); if (beginLine == null)
            {
                return Rectangle.Empty;
            }
            if (beginlineNum == endLineNum)
            {
                VisualPointInfo beginPoint = beginLine.GetTextPointInfoFromCharIndex(beginColumnNum);
                VisualPointInfo endPoint = beginLine.GetTextPointInfoFromCharIndex(endColumnNum);
                return new Rectangle(beginPoint.X, beginLine.Top, endPoint.X, beginLine.ActualLineHeight);
            }
            else
            {
                VisualPointInfo beginPoint = beginLine.GetTextPointInfoFromCharIndex(beginColumnNum);

                EditableVisualElementLine endLine = flowLayer.GetTextLineAtPos(endLineNum); VisualPointInfo endPoint = endLine.GetTextPointInfoFromCharIndex(endColumnNum);
                return new Rectangle(beginPoint.X, beginLine.Top, endPoint.X, beginLine.ActualLineHeight);
            }

        }

        protected override void DrawSubGround(ArtCanvas canvasPage, InternalRect updateArea)
        {
            if (internalTextLayerController.SelectionRange != null)
            {
                internalTextLayerController.SelectionRange.Draw(canvasPage, updateArea);
            }
            //if (SubgroundPaint != null)
            //{
            //    SubgroundPaint(this, new ArtVisualPaintEventArgs(canvasPage, updateArea));
            //}
        }



        public void OnKeyPress(ArtKeyPressEventArgs e)
        {

            if (!e.IsControlKey)
            {
                VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();

                char c = e.KeyChar;
                e.CancelBubbling = true;
                if (internalTextLayerController.SelectionRange != null
                    && internalTextLayerController.SelectionRange.IsValid)
                {
                    InvalidateGraphicLocalArea(this, GetSelectionUpdateArea(), vinv);
                }

                if (textSurfaceEventListener != null && !TextSurfaceEventListener.NotifyPreviewKeydown(textSurfaceEventListener, c))
                {
                    internalTextLayerController.UpdateSelectionRange();

                }
                if (internalTextLayerController.SelectionRange != null)
                {

                    internalTextLayerController.AddCharToCurrentLine(c, vinv);
                    if (textSurfaceEventListener != null)
                    {
                        TextSurfaceEventListener.NotifyCharactersReplaced(textSurfaceEventListener, e.KeyChar);
                    }
                }
                else
                {
                    internalTextLayerController.AddCharToCurrentLine(c, vinv);
                    if (textSurfaceEventListener != null)
                    {
                        TextSurfaceEventListener.NotifyCharacterAdded(textSurfaceEventListener, e.KeyChar);
                    }
                }
                EnsureCaretVisible(vinv);
                e.FreeVisualInvalidateCanvasArgs(vinv);
            }
        }

        void InvalidateGraphicOfCurrentLineArea(VisualElementArgs vinv)
        {
#if DEBUG
            Rectangle c_lineArea = this.internalTextLayerController.CurrentParentLineArea;
#endif
            InvalidateGraphicLocalArea(this, this.internalTextLayerController.CurrentParentLineArea, vinv);

        }


        public void OnMouseDown(ArtMouseEventArgs e)
        {
            if (e.Button == ArtMouseButtons.Left)
            {
                VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                InvalidateGraphicOfCurrentLineArea(vinv); internalTextLayerController.CaretPos = e.Location; if (internalTextLayerController.SelectionRange != null)
                {
                    Rectangle r = GetSelectionUpdateArea(); internalTextLayerController.CancelSelect();
                    InvalidateGraphicLocalArea(this, r, vinv);
                }
                else
                {
                    InvalidateGraphicOfCurrentLineArea(vinv);
                }
                e.FreeVisualInvalidateCanvasArgs(vinv);
            }

        }
        public void OnDoubleClick(ArtMouseEventArgs e)
        {

            internalTextLayerController.CancelSelect();
            ArtEditableVisualTextRun textRun = this.CurrentTextRun;
            if (textRun != null)
            {
                VisualPointInfo pointInfo = internalTextLayerController.GetCurrentPointInfo();
                int lineCharacterIndex = pointInfo.LineCharIndex;
                int localselIndex = pointInfo.LocalSelectedIndex;
                internalTextLayerController.CharIndex = lineCharacterIndex - localselIndex - 1;

                internalTextLayerController.StartSelect();
                internalTextLayerController.CharIndex += textRun.CharacterCount;
                internalTextLayerController.EndSelect();
            }

        }
        public void OnDrag(ArtDragEventArgs e)
        {



            if ((ArtMouseButtons)e.Button == ArtMouseButtons.Left)
            {

                internalTextLayerController.CaretPos = e.Location; internalTextLayerController.EndSelect();
                VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                this.InvalidateGraphic(vinv);
                e.FreeVisualInvalidateCanvasArgs(vinv);
            }

        }
        public void OnDragStart(ArtDragEventArgs e)
        {
            if ((ArtMouseButtons)e.Button == ArtMouseButtons.Left)
            {
                internalTextLayerController.CaretPos = e.Location;
                internalTextLayerController.StartSelect();
                internalTextLayerController.EndSelect();
                VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                this.InvalidateGraphic(vinv);
                e.FreeVisualInvalidateCanvasArgs(vinv);

            }
        }
        public void OnDragStop(ArtDragEventArgs e)
        {
            if ((ArtMouseButtons)e.Button == ArtMouseButtons.Left)
            {
                internalTextLayerController.CaretPos = e.Location;
                internalTextLayerController.EndSelect();

                VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                this.InvalidateGraphic(vinv);
                e.FreeVisualInvalidateCanvasArgs(vinv);
            }
        }



        Rectangle GetSelectionUpdateArea()
        {
            VisualSelectionRange selectionRange = internalTextLayerController.SelectionRange;

            if (selectionRange != null && selectionRange.IsValid)
            {

                return Rectangle.FromLTRB(0,
                    selectionRange.TopEnd.LineTop,
                    Width,
                    selectionRange.BottomEnd.Line.LineBottom);
            }
            else
            {
                return Rectangle.Empty;
            }
        }
        public void OnMouseUp(ArtMouseEventArgs e)
        {

        }
        public void OnKeyDown(ArtKeyEventArgs e)
        {

            if (!e.HasKeyData)
            {
                return;
            }
            ArtKeys keycode = (ArtKeys)e.KeyData;

            switch (keycode)
            {
                case ArtKeys.Back:
                    {

                        VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();

                        if (internalTextLayerController.SelectionRange != null)
                        {

                            InvalidateGraphicLocalArea(this, GetSelectionUpdateArea(), vinv);
                        }
                        else
                        {

                            InvalidateGraphicOfCurrentLineArea(vinv);
                        }
                        if (textSurfaceEventListener == null)
                        {

                            internalTextLayerController.DoBackspace(vinv);
                        }
                        else
                        {
                            if (!TextSurfaceEventListener.NotifyPreviewBackSpace(textSurfaceEventListener) &&
                                internalTextLayerController.DoBackspace(vinv))
                            {

                                TextDomEventArgs textdomE = new TextDomEventArgs(internalTextLayerController.updateJustCurrentLine);
                                TextSurfaceEventListener.NotifyCharactersRemoved(textSurfaceEventListener, textdomE);
                            }
                        }

                        EnsureCaretVisible(vinv);
                        e.FreeVisualInvalidateCanvasArgs(vinv);
                    } break;
                case ArtKeys.Home:
                    {

                        if (!e.IsShiftKeyDown)
                        {
                            internalTextLayerController.DoHome();
                            internalTextLayerController.CancelSelect();
                        }
                        else
                        {
                            if (internalTextLayerController.SelectionRange == null)
                            {

                                internalTextLayerController.StartSelect();
                            }
                            internalTextLayerController.DoHome();
                            internalTextLayerController.EndSelect();
                        }
                        VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                        EnsureCaretVisible(vinv);
                        e.FreeVisualInvalidateCanvasArgs(vinv);
                    } break;
                case ArtKeys.End:
                    {
                        if (!e.IsShiftKeyDown)
                        {
                            internalTextLayerController.DoEnd();
                            internalTextLayerController.CancelSelect();

                        }
                        else
                        {

                            if (internalTextLayerController.SelectionRange == null)
                            {

                                internalTextLayerController.StartSelect();
                            }

                            internalTextLayerController.DoEnd();
                            internalTextLayerController.EndSelect();

                        }
                        VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                        EnsureCaretVisible(vinv);
                        e.FreeVisualInvalidateCanvasArgs(vinv);
                    } break;
                case ArtKeys.Delete:
                    {

                        VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                        if (internalTextLayerController.SelectionRange != null)
                        {
                            InvalidateGraphicLocalArea(this, GetSelectionUpdateArea(), vinv);
                        }
                        else
                        {
                            InvalidateGraphicOfCurrentLineArea(vinv);
                        }
                        if (textSurfaceEventListener == null)
                        {

                            internalTextLayerController.DoDelete(vinv);
                        }
                        else
                        {

                            VisualSelectionRangeSnapShot delpart = internalTextLayerController.DoDelete(vinv);
                            TextSurfaceEventListener.NotifyCharactersRemoved(textSurfaceEventListener,
                                new TextDomEventArgs(internalTextLayerController.updateJustCurrentLine));

                        }

                        EnsureCaretVisible(vinv);
                        e.FreeVisualInvalidateCanvasArgs(vinv);

                    } break;
                default:
                    {
                        if (textSurfaceEventListener != null)
                        {

                            if (keycode >= ArtKeys.F1 && keycode <= ArtKeys.F12)
                            {
                                VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea(), vinv);
                                TextSurfaceEventListener.NotifyFunctionKeyDown(textSurfaceEventListener, keycode);
                                EnsureCaretVisible(vinv);
                                e.FreeVisualInvalidateCanvasArgs(vinv);
                            }
                        }

                    } break;
            }

            if (e.HasKeyData && e.Control)
            {

                switch (keycode)
                {
                    case ArtKeys.C:
                        {
                            StringBuilder stBuilder = GetFreeStringBuilder();
                            internalTextLayerController.CopySelectedTextToPlainText(stBuilder);
                            if (stBuilder != null)
                            {
                                if (stBuilder.Length == 0)
                                {
                                    Clipboard.Clear();
                                }
                                else
                                {
                                    Clipboard.SetText(stBuilder.ToString());
                                }
                            }
                            ReleaseStringBuilder(stBuilder);

                        } break;
                    case ArtKeys.V:
                        {

                            if (Clipboard.ContainUnicodeText())
                            {
                                VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();

                                internalTextLayerController.AddTextRunsToCurrentLine(
                                    new ArtEditableVisualTextRun[]{ 
                                        new ArtEditableVisualTextRun( 
                                            Clipboard.GetUnicodeText())
                                           }, vinv);
                                EnsureCaretVisible(vinv);
                                e.FreeVisualInvalidateCanvasArgs(vinv);
                            }

                        } break;
                    case ArtKeys.X:
                        {

                            if (internalTextLayerController.SelectionRange != null)
                            {
                                VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                                if (internalTextLayerController.SelectionRange != null)
                                {

                                    InvalidateGraphicLocalArea(this, GetSelectionUpdateArea(), vinv);
                                }
                                StringBuilder stBuilder = GetFreeStringBuilder();
                                internalTextLayerController.CopySelectedTextToPlainText(stBuilder);
                                if (stBuilder != null)
                                {
                                    Clipboard.SetText(stBuilder.ToString());
                                }

                                internalTextLayerController.DoDelete(vinv);


                                EnsureCaretVisible(vinv);
                                ReleaseStringBuilder(stBuilder);
                                e.FreeVisualInvalidateCanvasArgs(vinv);
                            }

                        } break;

                    case ArtKeys.Z:
                        {
                            VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                            internalTextLayerController.UndoLastAction(vinv);
                            EnsureCaretVisible(vinv);
                            e.FreeVisualInvalidateCanvasArgs(vinv);
                        } break;

                    case ArtKeys.Y:
                        {

                            VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                            internalTextLayerController.ReverseLastUndoAction(vinv);
                            EnsureCaretVisible(vinv);
                            e.FreeVisualInvalidateCanvasArgs(vinv);

                        } break;
                    case ArtKeys.B:
                        {
                            BoxStyle defaultBeh1 = internalTextLayerController.GetFirstTextStyleInSelectedRange();

                            BoxStyle textStyle = null;
                            if (defaultBeh1 != null)
                            {
                                BoxStyle defaultBeh = ((BoxStyle)defaultBeh1);
                                if (defaultBeh.FontBold)
                                {
                                    textStyle = StyleHelper.CreateNewStyle(Color.Black);
                                }
                                else
                                {
                                    textStyle = StyleHelper.CreateNewStyle(Color.Blue);
                                }
                            }
                            else
                            {
                                textStyle = StyleHelper.CreateNewStyle(Color.Blue);

                            }

                            VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                            internalTextLayerController.DoFormatSelection(textStyle, vinv);

                            if (internalTextLayerController.updateJustCurrentLine)
                            {

                                InvalidateGraphicOfCurrentLineArea(vinv);
                            }
                            else
                            {
                                InvalidateGraphic(vinv);

                            }
                            e.FreeVisualInvalidateCanvasArgs(vinv);
                        } break;

                }
            }

        }

        public bool OnProcessDialogKey(ArtKeyEventArgs e)
        {
            ArtKeys keyData = (ArtKeys)e.KeyData;


            if (isInVerticalPhase && (keyData != ArtKeys.Up || keyData != ArtKeys.Down))
            {
                isInVerticalPhase = false;
            }
            switch (keyData)
            {
                case ArtKeys.Return:
                case ArtKeys.Return | ArtKeys.Shift:
                    {

                        if (textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewEnter(textSurfaceEventListener))
                        {
                            return true;
                        }
                        if (isMultiLine)
                        {
                            VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();

                            if (internalTextLayerController.SelectionRange != null)
                            {
                                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea(), vinv);
                            }
                            internalTextLayerController.SplitCurrentLineIntoNewLine(vinv);


                            if (textSurfaceEventListener != null)
                            {

                                e.FreeVisualInvalidateCanvasArgs(vinv);
                                TextSurfaceEventListener.NofitySplitNewLine(textSurfaceEventListener, e);
                                vinv = e.GetVisualInvalidateCanvasArgs();
                            }


                            Rectangle lineArea = internalTextLayerController.CurrentLineArea;

                            if (lineArea.Bottom > this.ViewportBottom)
                            {

                                ScrollBy(0, lineArea.Bottom - this.ViewportBottom, vinv);
                            }
                            else
                            {
                                InvalidateGraphicOfCurrentLineArea(vinv);
                            }
                            EnsureCaretVisible(vinv);
                            e.FreeVisualInvalidateCanvasArgs(vinv);
                            return true;
                        }
                        return true;
                    }
                case ArtKeys.Left:
                    {
                        if (textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewArrow(textSurfaceEventListener, keyData))
                        {
                            return true;
                        }
                        VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                        InvalidateGraphicOfCurrentLineArea(vinv);
                        if (!e.IsShiftKeyDown)
                        {
                            internalTextLayerController.CancelSelect();
                        }
                        else
                        {
                            if (internalTextLayerController.SelectionRange == null)
                            {
                                internalTextLayerController.StartSelect();
                            }
                        }

                        Point currentCaretPos = Point.Empty;
                        if (!isMultiLine)
                        {
                            while (!internalTextLayerController.IsOnStartOfLine)
                            {

                                Point prvCaretPos = internalTextLayerController.CaretPos;
                                internalTextLayerController.CharIndex--;
                                currentCaretPos = internalTextLayerController.CaretPos;
                                if (currentCaretPos.X != prvCaretPos.X)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {

                            if (internalTextLayerController.IsOnStartOfLine)
                            {
                                internalTextLayerController.CharIndex--;
                                currentCaretPos = internalTextLayerController.CaretPos;
                            }
                            else
                            {
                                while (!internalTextLayerController.IsOnStartOfLine)
                                {

                                    Point prvCaretPos = internalTextLayerController.CaretPos;
                                    internalTextLayerController.CharIndex--;
                                    currentCaretPos = internalTextLayerController.CaretPos;
                                    if (currentCaretPos.X != prvCaretPos.X)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        EnsureCaretVisible(vinv);
                        if (textSurfaceEventListener != null)
                        {
                            TextSurfaceEventListener.NotifyArrowKeyCaretPosChanged(textSurfaceEventListener, keyData);
                        }
                        e.FreeVisualInvalidateCanvasArgs(vinv);
                        return true;
                    }
                case ArtKeys.Right:
                    {
                        if (textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewArrow(textSurfaceEventListener, keyData))
                        {

                            return true;
                        }
                        VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                        InvalidateGraphicOfCurrentLineArea(vinv);
                        if (!e.IsShiftKeyDown)
                        {
                            internalTextLayerController.CancelSelect();
                        }
                        else
                        {

                            if (internalTextLayerController.SelectionRange == null)
                            {
                                internalTextLayerController.StartSelect();
                            }
                        }


                        Point currentCaretPos = Point.Empty;
                        if (!isMultiLine)
                        {
                            while (!internalTextLayerController.IsOnEndOfLine)
                            {
                                Point prvCaretPos = internalTextLayerController.CaretPos;
                                internalTextLayerController.CharIndex++;
                                currentCaretPos = internalTextLayerController.CaretPos;
                                if (currentCaretPos.X != prvCaretPos.X)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {

                            if (internalTextLayerController.IsOnEndOfLine)
                            {
                                internalTextLayerController.CharIndex++;
                                currentCaretPos = internalTextLayerController.CaretPos;
                            }
                            else
                            {
                                while (!internalTextLayerController.IsOnEndOfLine)
                                {
                                    Point prvCaretPos = internalTextLayerController.CaretPos;
                                    internalTextLayerController.CharIndex++;
                                    currentCaretPos = internalTextLayerController.CaretPos;
                                    if (currentCaretPos.X != prvCaretPos.X)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        EnsureCaretVisible(vinv);
                        if (textSurfaceEventListener != null)
                        {
                            TextSurfaceEventListener.NotifyArrowKeyCaretPosChanged(textSurfaceEventListener, keyData);
                        }
                        e.FreeVisualInvalidateCanvasArgs(vinv);
                        return true;
                    }
                case ArtKeys.Down:
                    {
                        if (textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewArrow(textSurfaceEventListener, keyData))
                        {

                            return true;
                        }
                        if (isMultiLine)
                        {
                            if (!isInVerticalPhase)
                            {

                                isInVerticalPhase = true;

                                verticalExpectedCharIndex = internalTextLayerController.CharIndex;
                            }
                            VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                            internalTextLayerController.CurrentLineNumber++;

                            if (verticalExpectedCharIndex > internalTextLayerController.CurrentLineCharCount - 1)
                            {
                                internalTextLayerController.CharIndex = internalTextLayerController.CurrentLineCharCount - 1;
                            }
                            else
                            {
                                internalTextLayerController.CharIndex = verticalExpectedCharIndex;
                            }

                            Rectangle lineArea = internalTextLayerController.CurrentLineArea;
                            if (lineArea.Bottom > this.ViewportBottom)
                            {
                                ScrollBy(0, lineArea.Bottom - this.ViewportBottom, vinv);
                            }
                            else
                            {
                                InvalidateGraphicOfCurrentLineArea(vinv);
                            }
                            e.FreeVisualInvalidateCanvasArgs(vinv);
                        }
                        else
                        {
                        }
                        if (textSurfaceEventListener != null)
                        {
                            TextSurfaceEventListener.NotifyArrowKeyCaretPosChanged(textSurfaceEventListener, keyData);
                        }
                        return true;

                    }
                case ArtKeys.Up:
                    {
                        if (textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewArrow(textSurfaceEventListener, keyData))
                        {
                            return true;
                        }

                        if (isMultiLine)
                        {
                            if (!isInVerticalPhase)
                            {

                                isInVerticalPhase = true;

                                verticalExpectedCharIndex = internalTextLayerController.CharIndex;
                            }
                            internalTextLayerController.CurrentLineNumber--;

                            if (verticalExpectedCharIndex > internalTextLayerController.CurrentLineCharCount - 1)
                            {
                                internalTextLayerController.CharIndex = internalTextLayerController.CurrentLineCharCount - 1;
                            }
                            else
                            {
                                internalTextLayerController.CharIndex = verticalExpectedCharIndex;
                            }

                            Rectangle lineArea = internalTextLayerController.CurrentLineArea;
                            VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                            if (lineArea.Top < ViewportY)
                            {
                                ScrollBy(0, lineArea.Top - ViewportY, vinv);
                            }
                            else
                            {   //
                                InvalidateGraphicOfCurrentLineArea(vinv);
                            }
                            e.FreeVisualInvalidateCanvasArgs(vinv);
                        }
                        else
                        {
                        }
                        if (textSurfaceEventListener != null)
                        {
                            TextSurfaceEventListener.NotifyArrowKeyCaretPosChanged(textSurfaceEventListener, keyData);

                        }
                        return true;
                    }
                case ArtKeys.Tab:
                    {
                        VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                        DoTab(vinv);
                        e.FreeVisualInvalidateCanvasArgs(vinv);

                        return true;
                    }
                default:
                    {

                        return false;
                    }
            }
        }


        public override Point CaretPosition
        {
            get
            {
                Point textManCaretPos = internalTextLayerController.CaretPos;
                textManCaretPos.Offset(-ViewportX, -ViewportY);
                return textManCaretPos;
            }
        }

        public Point GlobalCaretPosition
        {
            get
            {
                Point caretPos = this.CaretPosition;
                Point globalCaret = this.GetGlobalLocation();
                caretPos.Offset(globalCaret.X, globalCaret.Y);
                return caretPos;
            }
        }

        void EnsureCaretVisible(VisualElementArgs vinv)
        {
            Point textManCaretPos = internalTextLayerController.CaretPos;
            textManCaretPos.Offset(-ViewportX, -ViewportY);

            if (textManCaretPos.X >= this.Width)
            {
                if (!isMultiLine)
                {

                    Rectangle r = internalTextLayerController.CurrentParentLineArea;
                    if (r.Width >= this.Width)
                    {
#if DEBUG
                        vinv.dbug_SetInitObject(this);
                        vinv.dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.ArtVisualTextSurafce_EnsureCaretVisible);

#endif

                        InnerDoTopDownReCalculateContentSize(this, vinv);
                        this.ContainerBaseEvaluateScrollBar();
                        RefreshSnapshotCanvas();

#if DEBUG
                        vinv.dbug_EndLayoutTrace();
#endif

                    }
                }
                else
                {
                }

                ScrollBy(textManCaretPos.X - this.Width, 0, vinv);
            }
            else if (textManCaretPos.X < 0)
            {
                ScrollBy(textManCaretPos.X - this.X, 0, vinv);
            }
            if (internalTextLayerController.updateJustCurrentLine)
            {
                InvalidateGraphicOfCurrentLineArea(vinv);
            }
            else
            {
                InvalidateGraphic(vinv);
            }
        }
        void RefreshSnapshotCanvas()
        {

        }
        public bool OnlyCurrentlineUpdated
        {
            get
            {
                return internalTextLayerController.updateJustCurrentLine;
            }
        }

        public int CurrentLineCharIndex
        {
            get
            {

                return internalTextLayerController.CurrentLineCharIndex;
            }
        }
        public int CurrentTextRunCharIndex
        {
            get
            {
                return internalTextLayerController.CurrentTextRunCharIndex;
            }
        }
        public int CurrentLineNumber
        {
            get
            {
                return internalTextLayerController.CurrentLineNumber;
            }
            set
            {
                internalTextLayerController.CurrentLineNumber = value;
            }
        }
        public void ScrollToCurrentLine(VisualElementArgs vinv)
        {
            this.ScrollTo(0, internalTextLayerController.CaretPos.Y, vinv);
        }

        public void DoTab(VisualElementArgs vinv)
        {
            if (internalTextLayerController.SelectionRange != null)
            {
                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea(), vinv);
            }

            internalTextLayerController.AddCharToCurrentLine(' ', vinv);
            internalTextLayerController.AddCharToCurrentLine(' ', vinv);
            internalTextLayerController.AddCharToCurrentLine(' ', vinv);
            internalTextLayerController.AddCharToCurrentLine(' ', vinv);
            internalTextLayerController.AddCharToCurrentLine(' ', vinv);

            if (textSurfaceEventListener != null)
            {
                TextSurfaceEventListener.NotifyCharacterAdded(textSurfaceEventListener, '\t');
            }
            InvalidateGraphicOfCurrentLineArea(vinv);
        }

        public void DoTyping(string text, VisualElementArgs vinv)
        {


            if (internalTextLayerController.SelectionRange != null)
            {
                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea(), vinv);
            }

            char[] charBuff = text.ToCharArray();
            int j = charBuff.Length;
            for (int i = 0; i < j; ++i)
            {
                internalTextLayerController.AddCharToCurrentLine(charBuff[i], vinv);
            }
            InvalidateGraphicOfCurrentLineArea(vinv);
        }
    }
}
