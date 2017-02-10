//Apache2, 2014-2017, WinterDev

using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.Text
{
    public sealed partial class TextEditRenderBox : RenderBoxBase
    {
        CaretRenderElement myCaret;
        EditableTextFlowLayer textLayer;
        InternalTextLayerController internalTextLayerController;
        int verticalExpectedCharIndex;
        bool isMultiLine = false;
        bool isInVerticalPhase = false;
        bool isFocus = false;
        bool stateShowCaret = false;
        bool isDragBegin;
        TextSpanStyle currentSpanStyle;
        public TextEditRenderBox(RootGraphic rootgfx,
            int width, int height,
            bool isMultiLine)
            : base(rootgfx, width, height)
        {
            GlobalCaretController.RegisterCaretBlink(rootgfx);            
            myCaret = new CaretRenderElement(rootgfx, 2, 17);
            myCaret.TransparentForAllEvents = true;
            this.MayHasViewport = true;
            this.BackgroundColor = Color.White;// Color.Transparent;
            this.currentSpanStyle = new TextSpanStyle();
            this.currentSpanStyle.FontInfo = rootgfx.DefaultTextEditFontInfo;
            textLayer = new EditableTextFlowLayer(this);
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

        public TextSpanStyle CurrentTextSpanStyle
        {
            get { return this.currentSpanStyle; }
            set { this.currentSpanStyle = value; }
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

        public void DoHome(bool pressShitKey)
        {
            if (!pressShitKey)
            {
                internalTextLayerController.DoHome();
                internalTextLayerController.CancelSelect();
            }
            else
            {
                internalTextLayerController.StartSelectIfNoSelection();
                internalTextLayerController.DoHome();
                internalTextLayerController.EndSelect();
            }

            EnsureCaretVisible();
        }
        public void DoEnd(bool pressShitKey)
        {
            if (!pressShitKey)
            {
                internalTextLayerController.DoEnd();
                internalTextLayerController.CancelSelect();
            }
            else
            {
                internalTextLayerController.StartSelectIfNoSelection();
                internalTextLayerController.DoEnd();
                internalTextLayerController.EndSelect();
            }

            EnsureCaretVisible();
        }


        public Rectangle GetRectAreaOf(int beginlineNum, int beginColumnNum, int endLineNum, int endColumnNum)
        {
            EditableTextFlowLayer flowLayer = this.textLayer;
            EditableTextLine beginLine = flowLayer.GetTextLineAtPos(beginlineNum);
            if (beginLine == null)
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
                EditableTextLine endLine = flowLayer.GetTextLineAtPos(endLineNum);
                VisualPointInfo endPoint = endLine.GetTextPointInfoFromCharIndex(endColumnNum);
                return new Rectangle(beginPoint.X, beginLine.Top, endPoint.X, beginLine.ActualLineHeight);
            }
        }
        public void OnKeyPress(UIKeyEventArgs e)
        {
            this.SetCaretState(true);
            //------------------------
            if (e.IsControlCharacter)
            {
                OnKeyDown(e);
                return;
            }

            char c = e.KeyChar;
            e.CancelBubbling = true;
            if (internalTextLayerController.SelectionRange != null
                && internalTextLayerController.SelectionRange.IsValid)
            {
                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
            }
            bool preventDefault = false;
            if (textSurfaceEventListener != null &&
                !(preventDefault = TextSurfaceEventListener.NotifyPreviewKeydown(textSurfaceEventListener, c)))
            {
                internalTextLayerController.UpdateSelectionRange();
            }
            if (preventDefault)
            {
                return;
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
            if (textSurfaceEventListener != null)
            {
                TextSurfaceEventListener.NotifyKeyDown(textSurfaceEventListener, e.KeyCode);
            }
        }
        void InvalidateGraphicOfCurrentLineArea()
        {
#if DEBUG
            Rectangle c_lineArea = this.internalTextLayerController.CurrentParentLineArea;
#endif
            InvalidateGraphicLocalArea(this, this.internalTextLayerController.CurrentParentLineArea);
        }


        internal void SwapCaretState()
        {
            this.stateShowCaret = !stateShowCaret;
            this.InvalidateGraphics();
            //int swapcount = dbugCaretSwapCount++;
            //if (stateShowCaret)
            //{
            //    Console.WriteLine(">>on " + swapcount);
            //    this.InvalidateGraphics();
            //    Console.WriteLine("<<on " + swapcount);
            //}
            //else
            //{
            //    Console.WriteLine(">>off " + swapcount);
            //    this.InvalidateGraphics();
            //    Console.WriteLine("<<off " + swapcount);
            //}

        }
        internal void SetCaretState(bool visible)
        {
            this.stateShowCaret = visible;
            this.InvalidateGraphics();
        }
        public void Focus()
        {
            GlobalCaretController.CurrentTextEditBox = this;
            this.SetCaretState(true);
            this.isFocus = true;
        }
        public void Blur()
        {
            GlobalCaretController.CurrentTextEditBox = null;
            this.SetCaretState(false);
            this.isFocus = false;
        }
        public bool IsFocused
        {
            get
            {
                return this.isFocus;
            }
        }

        public void OnMouseDown(UIMouseEventArgs e)
        {
            if (e.Button == UIMouseButtons.Left)
            {
                InvalidateGraphicOfCurrentLineArea();
                internalTextLayerController.SetCaretPos(e.X, e.Y);
                if (internalTextLayerController.SelectionRange != null)
                {
                    Rectangle r = GetSelectionUpdateArea();
                    internalTextLayerController.CancelSelect();
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
            EditableRun textRun = this.CurrentTextRun;
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
        public void OnDrag(UIMouseEventArgs e)
        {
            if (!isDragBegin)
            {
                //dbugMouseDragBegin++;
                //first time
                isDragBegin = true;
                if ((UIMouseButtons)e.Button == UIMouseButtons.Left)
                {
                    internalTextLayerController.SetCaretPos(e.X, e.Y);
                    internalTextLayerController.StartSelect();
                    internalTextLayerController.EndSelect();
                    this.InvalidateGraphics();
                }
            }
            else
            {
                //dbugMouseDragging++;
                if ((UIMouseButtons)e.Button == UIMouseButtons.Left)
                {
                    internalTextLayerController.StartSelectIfNoSelection();
                    internalTextLayerController.SetCaretPos(e.X, e.Y);
                    internalTextLayerController.EndSelect();
                    this.InvalidateGraphics();
                }
            }
        }
        public void OnDragEnd(UIMouseEventArgs e)
        {
            isDragBegin = false;
            if ((UIMouseButtons)e.Button == UIMouseButtons.Left)
            {
                internalTextLayerController.StartSelectIfNoSelection();
                internalTextLayerController.SetCaretPos(e.X, e.Y);
                internalTextLayerController.EndSelect();
                this.InvalidateGraphics();
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
        public void OnKeyUp(UIKeyEventArgs e)
        {
            this.SetCaretState(true);
        }
        public void OnKeyDown(UIKeyEventArgs e)
        {
            this.SetCaretState(true);
            if (!e.HasKeyData)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case UIKeys.Home:
                    {
                        this.DoHome(e.Shift);
                    }
                    break;
                case UIKeys.End:
                    {
                        this.DoEnd(e.Shift);
                    }
                    break;
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
                                TextSurfaceEventListener.NotifyCharactersRemoved(textSurfaceEventListener,
                                    new TextDomEventArgs(internalTextLayerController.updateJustCurrentLine));
                            }
                        }

                        EnsureCaretVisible();
                    }
                    break;
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
                    }
                    break;
                default:
                    {
                        if (textSurfaceEventListener != null)
                        {
                            UIKeys keycode = e.KeyCode;
                            if (keycode >= UIKeys.F1 && keycode <= UIKeys.F12)
                            {
                                InvalidateGraphicLocalArea(this, GetSelectionUpdateArea());
                                TextSurfaceEventListener.NotifyFunctionKeyDown(textSurfaceEventListener, keycode);
                                EnsureCaretVisible();
                            }
                        }
                    }
                    break;
            }

            if (e.HasKeyData && e.Ctrl)
            {
                switch (e.KeyCode)
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
                        }
                        break;
                    case UIKeys.V:
                        {
                            if (Clipboard.ContainUnicodeText())
                            {
                                internalTextLayerController.AddTextRunsToCurrentLine(
                                    new EditableRun[]{
                                        new EditableTextRun(this.Root,
                                            Clipboard.GetUnicodeText(), this.CurrentTextSpanStyle)
                                           });
                                EnsureCaretVisible();
                            }
                        }
                        break;
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
                        }
                        break;
                    case UIKeys.Z:
                        {
                            internalTextLayerController.UndoLastAction();
                            EnsureCaretVisible();
                        }
                        break;
                    case UIKeys.Y:
                        {
                            internalTextLayerController.ReverseLastUndoAction();
                            EnsureCaretVisible();
                        }
                        break;
                    case UIKeys.B:
                        {
                            //TextSpanStyle style = internalTextLayerController.GetFirstTextStyleInSelectedRange(); 
                            //TextSpanStyle textStyle = null;

                            ////test only ***
                            ////TODO: make this more configurable
                            //if (style != null)
                            //{
                            //    TextSpanStyle defaultBeh = ((TextSpanStyle)style);
                            //    if (defaultBeh.FontBold)
                            //    {
                            //        textStyle = StyleHelper.CreateNewStyle(Color.Black);
                            //    }
                            //    else
                            //    {
                            //        textStyle = StyleHelper.CreateNewStyle(Color.Blue);
                            //    }
                            //}
                            //else
                            //{
                            //    textStyle = StyleHelper.CreateNewStyle(Color.Blue); 
                            //} 

                            //internalTextLayerController.DoFormatSelection(textStyle);

                            //if (internalTextLayerController.updateJustCurrentLine)
                            //{

                            //    InvalidateGraphicOfCurrentLineArea();
                            //}
                            //else
                            //{
                            //    InvalidateGraphics(); 
                            //}

                        }
                        break;
                }
            }
            if (textSurfaceEventListener != null)
            {
                TextSurfaceEventListener.NotifyKeyDown(textSurfaceEventListener, e.KeyCode);
            }
        }
        public Point CurrentCaretPos
        {
            get { return this.internalTextLayerController.CaretPos; }
        }
        public bool OnProcessDialogKey(UIKeyEventArgs e)
        {
            UIKeys keyData = (UIKeys)e.KeyData;
            SetCaretState(true);
            if (isInVerticalPhase && (keyData != UIKeys.Up || keyData != UIKeys.Down))
            {
                isInVerticalPhase = false;
            }

            switch (e.KeyCode)
            {
                case UIKeys.Home:
                    {
                        OnKeyDown(e);
                        return true;
                    }
                case UIKeys.Return:
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
                        if (!e.Shift)
                        {
                            internalTextLayerController.CancelSelect();
                        }
                        else
                        {
                            internalTextLayerController.StartSelectIfNoSelection();
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
                        //-------------------
                        if (e.Shift)
                        {
                            internalTextLayerController.EndSelectIfNoSelection();
                        }
                        //-------------------

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
                        if (!e.Shift)
                        {
                            internalTextLayerController.CancelSelect();
                        }
                        else
                        {
                            internalTextLayerController.StartSelectIfNoSelection();
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
                                    int nextCharWidth = internalTextLayerController.GetNextCharacterWidth();
                                    if (nextCharWidth > 0)
                                    {
                                        break;
                                    }
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
                                        //forward check next caret
                                        int nextCharWidth = internalTextLayerController.GetNextCharacterWidth();
                                        if (nextCharWidth > 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        //-------------------
                        if (e.Shift)
                        {
                            internalTextLayerController.EndSelectIfNoSelection();
                        }
                        //-------------------

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

                            //----------------------------                          
                            if (!e.Shift)
                            {
                                internalTextLayerController.CancelSelect();
                            }
                            else
                            {
                                internalTextLayerController.StartSelectIfNoSelection();
                            }
                            //---------------------------- 

                            internalTextLayerController.CurrentLineNumber++;
                            if (verticalExpectedCharIndex > internalTextLayerController.CurrentLineCharCount - 1)
                            {
                                internalTextLayerController.CharIndex = internalTextLayerController.CurrentLineCharCount - 1;
                            }
                            else
                            {
                                internalTextLayerController.CharIndex = verticalExpectedCharIndex;
                            }
                            //----------------------------

                            if (e.Shift)
                            {
                                internalTextLayerController.EndSelectIfNoSelection();
                            }
                            //----------------------------
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

                            //----------------------------                          
                            if (!e.Shift)
                            {
                                internalTextLayerController.CancelSelect();
                            }
                            else
                            {
                                internalTextLayerController.StartSelectIfNoSelection();
                            }
                            //----------------------------

                            internalTextLayerController.CurrentLineNumber--;
                            if (verticalExpectedCharIndex > internalTextLayerController.CurrentLineCharCount - 1)
                            {
                                internalTextLayerController.CharIndex = internalTextLayerController.CurrentLineCharCount - 1;
                            }
                            else
                            {
                                internalTextLayerController.CharIndex = verticalExpectedCharIndex;
                            }

                            //----------------------------
                            if (e.Shift)
                            {
                                internalTextLayerController.EndSelectIfNoSelection();
                            }

                            Rectangle lineArea = internalTextLayerController.CurrentLineArea;
                            if (lineArea.Top < ViewportY)
                            {
                                ScrollBy(0, lineArea.Top - ViewportY);
                            }
                            else
                            {
                                EnsureCaretVisible();
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
        public override Size InnerContentSize
        {
            get
            {
                return internalTextLayerController.CurrentLineArea.Size;
            }
        }
        void EnsureCaretVisible()
        {
            //----------------------
            Point textManCaretPos = internalTextLayerController.CaretPos;
            textManCaretPos.Offset(-ViewportX, -ViewportY);
            //----------------------  
            if (textManCaretPos.X >= this.Width)
            {
                if (!isMultiLine)
                {
                    var r = internalTextLayerController.CurrentLineArea;
                    //Rectangle r = internalTextLayerController.CurrentParentLineArea;
                    if (r.Width >= this.Width)
                    {
#if DEBUG
                        dbug_SetInitObject(this);
                        dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.ArtVisualTextSurafce_EnsureCaretVisible);
#endif
                        //SetCalculatedSize(this, r.Width, r.Height);
                        //InnerDoTopDownReCalculateContentSize(this);
                        this.BoxEvaluateScrollBar();
                        RefreshSnapshotCanvas();
#if DEBUG
                        dbug_EndLayoutTrace();
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
                InvalidateGraphics();
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
