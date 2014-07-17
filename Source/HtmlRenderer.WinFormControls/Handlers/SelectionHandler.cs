// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Drawing;
using System.Windows.Forms;
using HtmlRenderer.Boxes;
using HtmlRenderer.Diagnostics;
using HtmlRenderer.Drawing;

namespace HtmlRenderer.Handlers
{
    /// <summary>
    /// Handler for text selection in the html.
    /// </summary>
    internal sealed class SelectionHandler :  IDisposable
    {
        #region Fields and Consts


        BoxHitChain _latestMouseDownHitChain = null;
        Point _mouseDownLocation;


        /// <summary>
        /// the root of the handled html tree
        /// </summary>
        private readonly CssBox _root;

        /// <summary>
        /// handler for showing context menu on right click
        /// </summary>
        private readonly ContextMenuHandler _contextMenuHandler;

        /// <summary>
        /// the mouse location when selection started used to ignore small selections
        /// </summary>
        private Point _selectionStartPoint;

        /// <summary>
        /// the starting word of html selection<br/>
        /// where the user started the selection, if the selection is backwards then it will be the last selected word.
        /// </summary>
        private CssRun _selectionStart;

        /// <summary>
        /// the ending word of html selection<br/>
        /// where the user ended the selection, if the selection is backwards then it will be the first selected word.
        /// </summary>
        private CssRun _selectionEnd;

        /// <summary>
        /// the selection start index if the first selected word is partially selected (-1 if not selected or fully selected)
        /// </summary>
        private int _selectionStartIndex = -1;

        /// <summary>
        /// the selection end index if the last selected word is partially selected (-1 if not selected or fully selected)
        /// </summary>
        private int _selectionEndIndex = -1;

        /// <summary>
        /// the selection start offset if the first selected word is partially selected (-1 if not selected or fully selected)
        /// </summary>
        private float _selectionStartOffset = -1;

        /// <summary>
        /// the selection end offset if the last selected word is partially selected (-1 if not selected or fully selected)
        /// </summary>
        private float _selectionEndOffset = -1;

        /// <summary>
        /// is the selection goes backward in the html, the starting word comes after the ending word in DFS traversing.<br/>
        /// </summary>
        private bool _backwardSelection;

        /// <summary>
        /// used to ignore mouse up after selection
        /// </summary>
        private bool _inSelection;

        /// <summary>
        /// current selection process is after double click (full word selection)
        /// </summary>
        private bool _isDoubleClickSelect;

        /// <summary>
        /// used to know if selection is in the control or started outside so it needs to be ignored
        /// </summary>
        private bool _mouseDownInControl;

        /// <summary>
        /// used to handle drag & drop
        /// </summary>
        private bool _mouseDownOnSelectedWord;

        /// <summary>
        /// is the cursor on the control has been changed by the selection handler
        /// </summary>
        private bool _cursorChanged;

        /// <summary>
        /// used to know if double click selection is requested
        /// </summary>
        private DateTime _lastMouseDown;

        /// <summary>
        /// used to know if drag & drop was already started not to execute the same operation over
        /// </summary>
        private DataObject _dragDropData;

        #endregion

        HtmlContainerImpl container;
        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="root">the root of the handled html tree</param>
        public SelectionHandler(CssBox root, HtmlContainerImpl container)
        {

            this.container = container;
            _root = root;
            _contextMenuHandler = new ContextMenuHandler(this, container);
        }

        /// <summary>
        /// Select all the words in the html.
        /// </summary>
        /// <param name="control">the control hosting the html to invalidate</param>
        public void SelectAll(Control control)
        {
            if (container.IsSelectionEnabled)
            {
                ClearSelection();
                SelectAllWords(_root);
                control.Invalidate();
            }
        }

        /// <summary>
        /// Select the word at the given location if found.
        /// </summary>
        /// <param name="control">the control hosting the html to invalidate</param>
        /// <param name="loc">the location to select word at</param>
        public void SelectWord(Control control, Point loc)
        {
            throw new NotSupportedException("wait of another technique");


            //if (_root.HtmlContainer.IsSelectionEnabled)
            //{
            //    var word = DomUtils.GetCssBoxWord(_root, loc);
            //    if (word != null)
            //    {
            //        word.Selection = this;
            //        _selectionStartPoint = loc;
            //        _selectionStart = _selectionEnd = word;
            //        control.Invalidate();
            //    }
            //}
        }

    

        /// <summary>
        /// Handle mouse down to handle selection.
        /// </summary>
        /// <param name="parent">the control hosting the html to invalidate</param>
        /// <param name="loc">the location of the mouse on the html</param>
        /// <param name="isMouseInContainer"> </param>
        public void HandleMouseDown(Control parent, Point loc, bool isMouseInContainer)
        {
            bool clear = !isMouseInContainer;
            _mouseDownLocation = loc;

            if (isMouseInContainer)
            {
                if (this.container.SelectionRange != null)
                {
                    //has existing selection
                    this.container.SelectionRange = null;
                    clear = true;
                }
                _mouseDownInControl = true;
                _isDoubleClickSelect = (DateTime.Now - _lastMouseDown).TotalMilliseconds < 400;
                _lastMouseDown = DateTime.Now;
                _mouseDownOnSelectedWord = false;

                if (container.IsSelectionEnabled && (Control.MouseButtons & MouseButtons.Left) != 0)
                {
                    BoxHitChain hitChain = new BoxHitChain();
                    hitChain.SetRootGlobalPosition(loc.X, loc.Y);

                    BoxUtils.HitTest(_root, loc.X, loc.Y, hitChain);

                    _latestMouseDownHitChain = hitChain;
                    //HitInfo hitInfo = hitChain.GetLastHit();
                    //switch (hitInfo.hitObjectKind)
                    //{
                    //    case HitObjectKind.Run:
                    //        {
                    //        } break;
                    //    case HitObjectKind.CssBox:
                    //        {
                    //        } break;
                    //}

                    ////get cssword or cssbox at location ***
                    //var word = DomUtils.GetCssBoxWord(_root, loc);
                    //if (word != null)
                    //{ 
                    //    //found word 
                    //}
                    //if (word != null && word.Selected)
                    //{
                    //    _mouseDownOnSelectedWord = true;
                    //}
                    //else
                    //{
                    //    clear = true;
                    //}
                }
                else if ((Control.MouseButtons & MouseButtons.Right) != 0)
                {

                    //link click



                    //var rect = DomUtils.GetCssBoxWord(_root, loc);
                    //var link = DomUtils.GetLinkBox(_root, loc);
                    //if (_root.HtmlContainer.IsContextMenuEnabled)
                    //{
                    //    _contextMenuHandler.ShowContextMenu(parent, rect, link);
                    //}
                    //clear = rect == null || !rect.Selected;
                }
            }

            if (clear)
            {
                ClearSelection();
                parent.Invalidate();
            }
        }

        /// <summary>
        /// Handle mouse up to handle selection and link click.
        /// </summary>
        /// <param name="parent">the control hosting the html to invalidate</param>
        /// <param name="button">the mouse button that has been released</param>
        /// <returns>is the mouse up should be ignored</returns>
        public bool HandleMouseUp(Control parent, MouseButtons button)
        {
            bool ignore = false;
            _mouseDownInControl = false;
            if (container.IsSelectionEnabled)
            {
                ignore = _inSelection;
                if (!_inSelection && (button & MouseButtons.Left) != 0 && _mouseDownOnSelectedWord)
                {
                    ClearSelection();
                    parent.Invalidate();
                }

                _mouseDownOnSelectedWord = false;
                _inSelection = false;
            }
            ignore = ignore || (DateTime.Now - _lastMouseDown > TimeSpan.FromSeconds(1));
            return ignore;
        }

        /// <summary>
        /// Handle mouse move to handle hover cursor and text selection.
        /// </summary>
        /// <param name="parent">the control hosting the html to set cursor and invalidate</param>
        /// <param name="loc">the location of the mouse on the html</param>
        public void HandleMouseMove(Control parent, Point loc)
        {
            if (container.IsSelectionEnabled &&
                _mouseDownInControl && (Control.MouseButtons & MouseButtons.Left) != 0)
            {
                if (loc.X != _mouseDownLocation.X || loc.Y != _mouseDownLocation.Y)
                {
                    //drag  
                    using (var g = new WinGraphics(parent.CreateGraphics(), false))
                    {
                        HandleDragSelection(g, loc, !_isDoubleClickSelect);
                    }
                    parent.Invalidate();
                }

                //if (_mouseDownOnSelectedWord)
                //{
                //    // make sure not to start drag-drop on click but when it actually moves as it fucks mouse-up
                //    if ((DateTime.Now - _lastMouseDown).TotalMilliseconds > 200)
                //    {
                //        StartDragDrop(parent);
                //    }
                //}
                //else
                //{
                //    HandleSelection(parent, loc, !_isDoubleClickSelect);
                //    _inSelection = _selectionStart != null && _selectionEnd != null && (_selectionStart != _selectionEnd || _selectionStartIndex != _selectionEndIndex);
                //}
            }
            else
            {
                //// Handle mouse hover over the html to change the cursor depending if hovering word, link of other.
                //var link = DomUtils.GetLinkBox(_root, loc);
                //if (link != null)
                //{
                //    _cursorChanged = true;
                //    parent.Cursor = Cursors.Hand;
                //}
                //else if (_root.HtmlContainer.IsSelectionEnabled)
                //{
                //    //var word = DomUtils.GetCssBoxWord(_root, loc);
                //    //_cursorChanged = word != null && !word.IsImage && !(word.Selected && (word.SelectedStartIndex < 0 || word.Left + word.SelectedStartOffset <= loc.X) && (word.SelectedEndOffset < 0 || word.Left + word.SelectedEndOffset >= loc.X));
                //    //parent.Cursor = _cursorChanged ? Cursors.IBeam : Cursors.Default;
                //}
                //else if (_cursorChanged)
                //{
                //    parent.Cursor = Cursors.Default;
                //}
            }
        }

        /// <summary>
        /// On mouse leave change the cursor back to default.
        /// </summary>
        /// <param name="parent">the control hosting the html to set cursor and invalidate</param>
        public void HandleMouseLeave(Control parent)
        {
            if (_cursorChanged)
            {
                _cursorChanged = false;
                parent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Copy the currently selected html segment to clipboard.<br/>
        /// Copy rich html text and plain text.
        /// </summary>
        public void CopySelectedHtml()
        {
            if (container.IsSelectionEnabled)
            {
                //var html = BoxUtils.GenerateHtml(_root, HtmlGenerationStyle.Inline, true);
                //var plainText = BoxUtils.GetSelectedPlainText(_root);
                //if (!string.IsNullOrEmpty(plainText))
                //    HtmlClipboardUtils.CopyToClipboard(html, plainText);
            }
        }

        /// <summary>
        /// Get the currently selected text segment in the html.<br/>
        /// </summary>
        public string GetSelectedText()
        {
            //wait for another techinque
            return ""; 
        }

        /// <summary>
        /// Copy the currently selected html segment with style.<br/>
        /// </summary>
        public string GetSelectedHtml()
        {
            //wait for another technique
            return "";
            //return container.IsSelectionEnabled ? BoxUtils.GenerateHtml(_root, HtmlGenerationStyle.Inline, true) : null;
        }

        /// <summary>
        /// The selection start index if the first selected word is partially selected (-1 if not selected or fully selected)<br/>
        /// if the given word is not starting or ending selection word -1 is returned as full word selection is in place.
        /// </summary>
        /// <remarks>
        /// Handles backward selecting by returning the selection end data instead of start.
        /// </remarks>
        /// <param name="word">the word to return the selection start index for</param>
        /// <returns>data value or -1 if not applicable</returns>
        public int GetSelectingStartIndex(CssRun word)
        {
            return word == (_backwardSelection ? _selectionEnd : _selectionStart) ? (_backwardSelection ? _selectionEndIndex : _selectionStartIndex) : -1;
        }

        /// <summary>
        /// The selection end index if the last selected word is partially selected (-1 if not selected or fully selected)<br/>
        /// if the given word is not starting or ending selection word -1 is returned as full word selection is in place.
        /// </summary>
        /// <remarks>
        /// Handles backward selecting by returning the selection end data instead of start.
        /// </remarks>
        /// <param name="word">the word to return the selection end index for</param>
        public int GetSelectedEndIndexOffset(CssRun word)
        {
            return word == (_backwardSelection ? _selectionStart : _selectionEnd) ? (_backwardSelection ? _selectionStartIndex : _selectionEndIndex) : -1;
        }

        /// <summary>
        /// The selection start offset if the first selected word is partially selected (-1 if not selected or fully selected)<br/>
        /// if the given word is not starting or ending selection word -1 is returned as full word selection is in place.
        /// </summary>
        /// <remarks>
        /// Handles backward selecting by returning the selection end data instead of start.
        /// </remarks>
        /// <param name="word">the word to return the selection start offset for</param>
        public float GetSelectedStartOffset(CssRun word)
        {
            return word == (_backwardSelection ? _selectionEnd : _selectionStart) ? (_backwardSelection ? _selectionEndOffset : _selectionStartOffset) : -1;
        }

        /// <summary>
        /// The selection end offset if the last selected word is partially selected (-1 if not selected or fully selected)<br/>
        /// if the given word is not starting or ending selection word -1 is returned as full word selection is in place.
        /// </summary>
        /// <remarks>
        /// Handles backward selecting by returning the selection end data instead of start.
        /// </remarks>
        /// <param name="word">the word to return the selection end offset for</param>
        public float GetSelectedEndOffset(CssRun word)
        {
            return word == (_backwardSelection ? _selectionStart : _selectionEnd) ? (_backwardSelection ? _selectionStartOffset : _selectionEndOffset) : -1;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            _contextMenuHandler.Dispose();
        }


        #region Private methods

        /// <summary>
        /// Handle html text selection by mouse move over the html with left mouse button pressed.<br/>
        /// Calculate the words in the selected range and set their selected property.
        /// </summary>
        /// <param name="control">the control hosting the html to invalidate</param>
        /// <param name="loc">the mouse location</param>
        /// <param name="allowPartialSelect">true - partial word selection allowed, false - only full words selection</param>
        private void HandleDragSelection(IGraphics g, Point loc, bool allowPartialSelect)
        {
            // get the line under the mouse or nearest from the top  
            BoxHitChain hitChain = new BoxHitChain();
            hitChain.SetRootGlobalPosition(loc.X, loc.Y);

            BoxUtils.HitTest(_root, loc.X, loc.Y, hitChain);

            //create selection range  
            if (this.container.SelectionRange != null)
            {
                this.container.SelectionRange = null;
            }
            this.container.SelectionRange = new Boxes.SelectionRange(_latestMouseDownHitChain, hitChain, g);
        }


        /// <summary>
        /// Clear the current selection.
        /// </summary>
        private void ClearSelection()
        {
            // clear drag and drop
            _dragDropData = null;
            // this.container.SelectionRange = null;

            ClearSelection(_root);

            _selectionStartOffset = -1;
            _selectionStartIndex = -1;
            _selectionEndOffset = -1;
            _selectionEndIndex = -1;

            _selectionStartPoint = Point.Empty;
            _selectionStart = null;
            _selectionEnd = null;
        }

        /// <summary>
        /// Clear the selection from all the words in the css box recursively.
        /// </summary>
        /// <param name="box">the css box to selectionStart clear at</param>
        private static void ClearSelection(CssBox box)
        {
            //foreach (var word in box.GetRunIter())
            //{
            //    word.Selection = null;
            //}
            //foreach (var childBox in box.GetChildBoxIter())
            //{
            //    ClearSelection(childBox);
            //}
        }

        /// <summary>
        /// Start drag-n-drop operation on the currently selected html segment.
        /// </summary>
        /// <param name="control">the control to start the drag and drop on</param>
        private void StartDragDrop(Control control)
        {
            if (_dragDropData == null)
            {
                //var html = BoxUtils.GenerateHtml(_root, HtmlGenerationStyle.Inline, true);
                //var plainText = BoxUtils.GetSelectedPlainText(_root);
                //_dragDropData = HtmlClipboardUtils.GetDataObject(html, plainText);
            }
            control.DoDragDrop(_dragDropData, DragDropEffects.Copy);
        }

        /// <summary>
        /// Select all the words that are under <paramref name="box"/> DOM hierarchy.<br/>
        /// </summary>
        /// <param name="box">the box to start select all at</param>
        public void SelectAllWords(CssBox box)
        {
            //foreach (var word in box.GetRunIter())
            //{
            //    word.Selection = this;
            //}

            //foreach (var childBox in box.GetChildBoxIter())
            //{
            //    SelectAllWords(childBox);
            //}
        }

        /// <summary>
        /// Check if the current selection is non empty, has some selection data.
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="allowPartialSelect">true - partial word selection allowed, false - only full words selection</param>
        /// <returns>true - is non empty selection, false - empty selection</returns>
        private bool CheckNonEmptySelection(Point loc, bool allowPartialSelect)
        {
            // full word selection is never empty
            if (!allowPartialSelect)
                return true;

            // if end selection location is near starting location then the selection is empty
            if (Math.Abs(_selectionStartPoint.X - loc.X) <= 1 && Math.Abs(_selectionStartPoint.Y - loc.Y) < 5)
                return false;

            // selection is empty if on same word and same index
            return _selectionStart != _selectionEnd || _selectionStartIndex != _selectionEndIndex;
        }

        ///// <summary>
        ///// Select all the words that are between <paramref name="selectionStart"/> word and <paramref name="selectionEnd"/> word in the DOM hierarchy.<br/>
        ///// </summary>
        ///// <param name="root">the root of the DOM sub-tree the selection is in</param>
        ///// <param name="selectionStart">selection start word limit</param>
        ///// <param name="selectionEnd">selection end word limit</param>
        //private void SelectWordsInRange(CssBox root, CssRun selectionStart, CssRun selectionEnd)
        //{
        //    bool inSelection = false;
        //    SelectWordsInRange(root, selectionStart, selectionEnd, ref inSelection);
        //}

        ///// <summary>
        ///// Select all the words that are between <paramref name="selectionStart"/> word and <paramref name="selectionEnd"/> word in the DOM hierarchy.
        ///// </summary>
        ///// <param name="box">the current traversal node</param>
        ///// <param name="selectionStart">selection start word limit</param>
        ///// <param name="selectionEnd">selection end word limit</param>
        ///// <param name="inSelection">used to know the traversal is currently in selected range</param>
        ///// <returns></returns>
        //private bool SelectWordsInRange(CssBox box, CssRun selectionStart, CssRun selectionEnd, ref bool inSelection)
        //{
        //    //foreach (var boxWord in box.GetRunIter())
        //    //{
        //    //    if (!inSelection && boxWord == selectionStart)
        //    //    {
        //    //        inSelection = true;
        //    //    }
        //    //    if (inSelection)
        //    //    {
        //    //        boxWord.Selection = this;

        //    //        if (selectionStart == selectionEnd || boxWord == selectionEnd)
        //    //        {
        //    //            return true;
        //    //        }
        //    //    }
        //    //}

        //    //foreach (var childBox in box.GetChildBoxIter())
        //    //{
        //    //    if (SelectWordsInRange(childBox, selectionStart, selectionEnd, ref inSelection))
        //    //    {
        //    //        return true;
        //    //    }
        //    //}

        //    return false;
        //}

      

        

        #endregion
    }
}