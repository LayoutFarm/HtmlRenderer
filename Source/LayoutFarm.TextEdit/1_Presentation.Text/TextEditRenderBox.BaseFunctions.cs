//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

using LayoutFarm;
using LayoutFarm.Text;

namespace LayoutFarm.Text
{

    public sealed partial class TextEditRenderBox : RenderBoxBase
    {

        EditableTextFlowLayer textLayer;
        InternalTextLayerController internalTextLayerController;
        int verticalExpectedCharIndex;
        bool isMultiLine = false;
        bool isInVerticalPhase = false;

        public TextEditRenderBox(RootGraphic rootgfx, int width, int height, bool isMultiLine)
            : base(rootgfx, width, height)
        {
            //RegisterNativeEvent((1 << UIEventIdentifier.NE_DRAG_START)
            //    | (1 << UIEventIdentifier.NE_DRAGING)
            //    | (1 << UIEventIdentifier.NE_DRAG_STOP)
            //    | (1 << UIEventIdentifier.NE_MOUSE_DOWN)
            //    | (1 << UIEventIdentifier.NE_MOUSE_MOVE)
            //    | (1 << UIEventIdentifier.NE_MOUSE_HOVER)
            //    | (1 << UIEventIdentifier.NE_MOUSE_UP)
            //    | (1 << UIEventIdentifier.NE_DBLCLICK)
            //    | (1 << UIEventIdentifier.NE_KEY_DOWN)
            //    | (1 << UIEventIdentifier.NE_KEY_PRESS));


            textLayer = new EditableTextFlowLayer(this);
            this.Layers = new VisualLayerCollection();
            this.Layers.AddLayer(textLayer);
            this.NeedSystemCaret = true;

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
        }

        public TextMan TextMan
        {
            get
            {
                return this.internalTextLayerController.TextMan;
            }
        }
        public static void NotifyTextContentSizeChanged(TextEditRenderBox ts)
        {
            ts.BoxEvaluateScrollBar();
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


        public void OnKeyPress(UIKeyPressEventArgs e)
        {

            if (!e.IsControlKey)
            {

                char c = e.KeyChar;
                e.CancelBubbling = true;
                if (internalTextLayerController.SelectionRange != null
                    && internalTextLayerController.SelectionRange.IsValid)
                {
                    InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
                }

                if (textSurfaceEventListener != null && !TextSurfaceEventListener.NotifyPreviewKeydown(textSurfaceEventListener, c))
                {
                    internalTextLayerController.UpdateSelectionRange();

                }
                if (internalTextLayerController.SelectionRange != null)
                {

                    internalTextLayerController.AddCharToCurrentLine(c);
                    if (textSurfaceEventListener != null)
                    {
                        TextSurfaceEventListener.NotifyCharactersReplaced(textSurfaceEventListener, e.KeyChar);
                    }
                }
                else
                {
                    internalTextLayerController.AddCharToCurrentLine(c);
                    if (textSurfaceEventListener != null)
                    {
                        TextSurfaceEventListener.NotifyCharacterAdded(textSurfaceEventListener, e.KeyChar);
                    }
                }
                EnsureCaretVisible();

            }
        }

        void InvalidateGraphicOfCurrentLineArea()
        {
#if DEBUG
            Rectangle c_lineArea = this.internalTextLayerController.CurrentParentLineArea;
#endif
            InvalidateGraphicLocalArea(this, this.internalTextLayerController.CurrentParentLineArea);

        }


        public void OnMouseDown(UIMouseEventArgs e)
        {
            if (e.Button == UIMouseButtons.Left)
            {

                InvalidateGraphicOfCurrentLineArea(); internalTextLayerController.CaretPos = e.Location; if (internalTextLayerController.SelectionRange != null)
                {
                    Rectangle r = GetSelectionUpdateArea(); internalTextLayerController.CancelSelect();
                    InvalidateGraphicLocalArea(this, r);
                }
                else
                {
                    InvalidateGraphicOfCurrentLineArea();
                }

            }

        }
        public void OnDoubleClick(UIMouseEventArgs e)
        {

            internalTextLayerController.CancelSelect();
            EditableTextSpan textRun = this.CurrentTextRun;
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
        public void OnDrag(UIDragEventArgs e)
        {

            if ((UIMouseButtons)e.Button == UIMouseButtons.Left)
            {
                internalTextLayerController.CaretPos = e.Location; internalTextLayerController.EndSelect();

                this.InvalidateGraphic();

            }

        }
        public void OnDragStart(UIDragEventArgs e)
        {
            if ((UIMouseButtons)e.Button == UIMouseButtons.Left)
            {
                internalTextLayerController.CaretPos = e.Location;
                internalTextLayerController.StartSelect();
                internalTextLayerController.EndSelect();

                this.InvalidateGraphic();


            }
        }
        public void OnDragStop(UIDragEventArgs e)
        {
            if ((UIMouseButtons)e.Button == UIMouseButtons.Left)
            {
                internalTextLayerController.CaretPos = e.Location;
                internalTextLayerController.EndSelect();


                this.InvalidateGraphic();

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
        public void OnMouseUp(UIMouseEventArgs e)
        {

        }
        public void OnKeyDown(UIKeyEventArgs e)
        {

            if (!e.HasKeyData)
            {
                return;
            }
            UIKeys keycode = (UIKeys)e.KeyData;

            switch (keycode)
            {
                case UIKeys.Back:
                    {



                        if (internalTextLayerController.SelectionRange != null)
                        {

                            InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
                        }
                        else
                        {

                            InvalidateGraphicOfCurrentLineArea();
                        }
                        if (textSurfaceEventListener == null)
                        {

                            internalTextLayerController.DoBackspace();
                        }
                        else
                        {
                            if (!TextSurfaceEventListener.NotifyPreviewBackSpace(textSurfaceEventListener) &&
                                internalTextLayerController.DoBackspace())
                            {

                                TextDomEventArgs textdomE = new TextDomEventArgs(internalTextLayerController.updateJustCurrentLine);
                                TextSurfaceEventListener.NotifyCharactersRemoved(textSurfaceEventListener, textdomE);
                            }
                        }

                        EnsureCaretVisible();

                    } break;
                case UIKeys.Home:
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

                        EnsureCaretVisible();

                    } break;
                case UIKeys.End:
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

                        EnsureCaretVisible();

                    } break;
                case UIKeys.Delete:
                    {


                        if (internalTextLayerController.SelectionRange != null)
                        {
                            InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
                        }
                        else
                        {
                            InvalidateGraphicOfCurrentLineArea();
                        }
                        if (textSurfaceEventListener == null)
                        {

                            internalTextLayerController.DoDelete();
                        }
                        else
                        {

                            VisualSelectionRangeSnapShot delpart = internalTextLayerController.DoDelete();
                            TextSurfaceEventListener.NotifyCharactersRemoved(textSurfaceEventListener,
                                new TextDomEventArgs(internalTextLayerController.updateJustCurrentLine));

                        }

                        EnsureCaretVisible();


                    } break;
                default:
                    {
                        if (textSurfaceEventListener != null)
                        {

                            if (keycode >= UIKeys.F1 && keycode <= UIKeys.F12)
                            {

                                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
                                TextSurfaceEventListener.NotifyFunctionKeyDown(textSurfaceEventListener, keycode);
                                EnsureCaretVisible();

                            }
                        }

                    } break;
            }

            if (e.HasKeyData && e.Control)
            {

                switch (keycode)
                {
                    case UIKeys.C:
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
                    case UIKeys.V:
                        {

                            if (Clipboard.ContainUnicodeText())
                            {

                                internalTextLayerController.AddTextRunsToCurrentLine(
                                    new EditableTextSpan[]{ 
                                        new EditableTextSpan( 
                                            Clipboard.GetUnicodeText())
                                           });
                                EnsureCaretVisible();

                            }

                        } break;
                    case UIKeys.X:
                        {

                            if (internalTextLayerController.SelectionRange != null)
                            {

                                if (internalTextLayerController.SelectionRange != null)
                                {

                                    InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
                                }
                                StringBuilder stBuilder = GetFreeStringBuilder();
                                internalTextLayerController.CopySelectedTextToPlainText(stBuilder);
                                if (stBuilder != null)
                                {
                                    Clipboard.SetText(stBuilder.ToString());
                                }

                                internalTextLayerController.DoDelete();


                                EnsureCaretVisible();
                                ReleaseStringBuilder(stBuilder);

                            }

                        } break;

                    case UIKeys.Z:
                        {

                            internalTextLayerController.UndoLastAction();
                            EnsureCaretVisible();

                        } break;

                    case UIKeys.Y:
                        {


                            internalTextLayerController.ReverseLastUndoAction();
                            EnsureCaretVisible();


                        } break;
                    case UIKeys.B:
                        {
                            TextSpanSytle defaultBeh1 = internalTextLayerController.GetFirstTextStyleInSelectedRange();

                            TextSpanSytle textStyle = null;
                            if (defaultBeh1 != null)
                            {
                                TextSpanSytle defaultBeh = ((TextSpanSytle)defaultBeh1);
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


                            internalTextLayerController.DoFormatSelection(textStyle);

                            if (internalTextLayerController.updateJustCurrentLine)
                            {

                                InvalidateGraphicOfCurrentLineArea();
                            }
                            else
                            {
                                InvalidateGraphic();

                            }

                        } break;

                }
            }

        }

        public bool OnProcessDialogKey(UIKeyEventArgs e)
        {
            UIKeys keyData = (UIKeys)e.KeyData;


            if (isInVerticalPhase && (keyData != UIKeys.Up || keyData != UIKeys.Down))
            {
                isInVerticalPhase = false;
            }
            switch (keyData)
            {
                case UIKeys.Return:
                case UIKeys.Return | UIKeys.Shift:
                    {

                        if (textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewEnter(textSurfaceEventListener))
                        {
                            return true;
                        }
                        if (isMultiLine)
                        {

                            if (internalTextLayerController.SelectionRange != null)
                            {
                                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
                            }
                            internalTextLayerController.SplitCurrentLineIntoNewLine();


                            if (textSurfaceEventListener != null)
                            {


                                TextSurfaceEventListener.NofitySplitNewLine(textSurfaceEventListener, e);

                            }

                            Rectangle lineArea = internalTextLayerController.CurrentLineArea;
                            if (lineArea.Bottom > this.ViewportBottom)
                            {

                                ScrollBy(0, lineArea.Bottom - this.ViewportBottom);
                            }
                            else
                            {
                                InvalidateGraphicOfCurrentLineArea();
                            }
                            EnsureCaretVisible();

                            return true;
                        }
                        return true;
                    }
                case UIKeys.Left:
                    {
                        if (textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewArrow(textSurfaceEventListener, keyData))
                        {
                            return true;
                        }

                        InvalidateGraphicOfCurrentLineArea();
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
                        EnsureCaretVisible();
                        if (textSurfaceEventListener != null)
                        {
                            TextSurfaceEventListener.NotifyArrowKeyCaretPosChanged(textSurfaceEventListener, keyData);
                        }

                        return true;
                    }
                case UIKeys.Right:
                    {
                        if (textSurfaceEventListener != null &&
                            TextSurfaceEventListener.NotifyPreviewArrow(textSurfaceEventListener, keyData))
                        {

                            return true;
                        }

                        InvalidateGraphicOfCurrentLineArea();
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

                        EnsureCaretVisible();
                        if (textSurfaceEventListener != null)
                        {
                            TextSurfaceEventListener.NotifyArrowKeyCaretPosChanged(textSurfaceEventListener, keyData);
                        }

                        return true;
                    }
                case UIKeys.Down:
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
                                ScrollBy(0, lineArea.Bottom - this.ViewportBottom);
                            }
                            else
                            {
                                InvalidateGraphicOfCurrentLineArea();
                            }

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
                case UIKeys.Up:
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

                            if (lineArea.Top < ViewportY)
                            {
                                ScrollBy(0, lineArea.Top - ViewportY);
                            }
                            else
                            {   //
                                InvalidateGraphicOfCurrentLineArea();
                            }

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
                case UIKeys.Tab:
                    {

                        DoTab();


                        return true;
                    }
                default:
                    {

                        return false;
                    }
            }
        }
         
        //public override Point CaretPosition
        //{
        //    get
        //    {
        //        Point textManCaretPos = internalTextLayerController.CaretPos;
        //        textManCaretPos.Offset(-ViewportX, -ViewportY);
        //        return textManCaretPos;
        //    }
        //}

        //public Point GlobalCaretPosition
        //{
        //    get
        //    {
        //        Point caretPos = this.CaretPosition;
        //        Point globalCaret = this.GetGlobalLocation();
        //        caretPos.Offset(globalCaret.X, globalCaret.Y);
        //        return caretPos;
        //    }
        //}

        void EnsureCaretVisible()
        {

            Point textManCaretPos = internalTextLayerController.CaretPos;

            textManCaretPos.Offset(-ViewportX, -ViewportY);
            this.Root.SetCarentPosition(textManCaretPos, this);

            //RootGraphic.SetCarentPosition(textManCaretPos, this);

            if (textManCaretPos.X >= this.Width)
            {
                if (!isMultiLine)
                {

                    Rectangle r = internalTextLayerController.CurrentParentLineArea;
                    if (r.Width >= this.Width)
                    {
#if DEBUG
                        vinv_dbug_SetInitObject(this);
                        vinv_dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.ArtVisualTextSurafce_EnsureCaretVisible);

#endif

                        InnerDoTopDownReCalculateContentSize(this);
                        this.BoxEvaluateScrollBar();
                        RefreshSnapshotCanvas();

#if DEBUG
                        vinv_dbug_EndLayoutTrace();
#endif

                    }
                }
                else
                {
                }

                ScrollBy(textManCaretPos.X - this.Width, 0);
            }
            else if (textManCaretPos.X < 0)
            {
                ScrollBy(textManCaretPos.X - this.X, 0);
            }
            if (internalTextLayerController.updateJustCurrentLine)
            {
                InvalidateGraphicOfCurrentLineArea();
            }
            else
            {
                InvalidateGraphic();
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
        public void ScrollToCurrentLine()
        {
            this.ScrollTo(0, internalTextLayerController.CaretPos.Y);
        }

        public void DoTab()
        {
            if (internalTextLayerController.SelectionRange != null)
            {
                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
            }

            internalTextLayerController.AddCharToCurrentLine(' ');
            internalTextLayerController.AddCharToCurrentLine(' ');
            internalTextLayerController.AddCharToCurrentLine(' ');
            internalTextLayerController.AddCharToCurrentLine(' ');
            internalTextLayerController.AddCharToCurrentLine(' ');

            if (textSurfaceEventListener != null)
            {
                TextSurfaceEventListener.NotifyCharacterAdded(textSurfaceEventListener, '\t');
            }
            InvalidateGraphicOfCurrentLineArea();
        }

        public void DoTyping(string text)
        {


            if (internalTextLayerController.SelectionRange != null)
            {
                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
            }

            char[] charBuff = text.ToCharArray();
            int j = charBuff.Length;
            for (int i = 0; i < j; ++i)
            {
                internalTextLayerController.AddCharToCurrentLine(charBuff[i]);
            }
            InvalidateGraphicOfCurrentLineArea();
        }
    }
}
