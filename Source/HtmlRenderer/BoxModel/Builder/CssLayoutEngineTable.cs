//BSD 2014,
//ArthurHub

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
using System.Collections.Generic;
using System.Drawing;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{
    /// <summary>
    /// Layout engine for tables executing the complex layout of tables with rows/columns/headers/etc.
    /// </summary>
    sealed class CssLayoutEngineTable
    {






        #region Fields and Consts

        /// <summary>
        /// the main box of the table
        /// </summary>
        readonly CssBox _tableBox;

        CssBox _caption;

        CssBox _headerBox;

        CssBox _footerBox;


        /// <summary>
        /// collection of all columns boxes
        /// </summary>
        readonly List<CssBox> _columns = new List<CssBox>();


        readonly List<CssBox> _allRows = new List<CssBox>();

        private int _columnCount;

        private bool _widthSpecified;


        float[] _columnWidths;
        float[] _columnMinWidths;
        byte[] _columnWidthsStatus;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="tableBox"></param>
        private CssLayoutEngineTable(CssBox tableBox)
        {
            _tableBox = tableBox;
        }

        /// <summary>
        /// Get the table cells spacing for all the cells in the table.<br/>
        /// Used to calculate the spacing the table has in addition to regular padding and borders.
        /// </summary>
        /// <param name="tableBox">the table box to calculate the spacing for</param>
        /// <returns>the calculated spacing</returns>
        static float GetTableSpacing(CssBox tableBox)
        {
            int count = 0;
            int columns = 0;
            foreach (var box in tableBox.GetChildBoxIter())
            {

                switch (box.CssDisplay)
                {
                    case CssDisplay.TableColumn:
                        {
                            columns += GetSpan(box);
                        } break;
                    case CssDisplay.TableRowGroup:
                        {
                            foreach (CssBox cr in tableBox.GetChildBoxIter())
                            {
                                count++;
                                if (cr.CssDisplay == CssDisplay.TableRow)
                                {
                                    columns = Math.Max(columns, cr.ChildCount);
                                }
                            }
                        } break;
                    case CssDisplay.TableRow:
                        {
                            count++;
                            columns = Math.Max(columns, box.ChildCount);
                        } break;

                }
                // limit the amount of rows to process for performance ?
                if (count > 30)
                {
                    break;
                }
            }

            // +1 columns because padding is between the cell and table borders
            return (columns + 1) * GetHorizontalSpacing(tableBox);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="tableBox"> </param>
        public static void PerformLayout(IGraphics g, CssBox tableBox)
        {
            ArgChecker.AssertArgNotNull(g, "g");
            ArgChecker.AssertArgNotNull(tableBox, "tableBox");

            //try
            //{
            var table = new CssLayoutEngineTable(tableBox);
            table.Layout(g);
            //}
            //catch (Exception ex)
            //{
            //    tableBox.HtmlContainer.ReportError(HtmlRenderErrorType.Layout, "Failed table layout", ex);
            //}
        }


        #region Private Methods

        /// <summary>
        /// Analyzes the Table and assigns values to this CssTable object.
        /// To be called from the constructor
        /// </summary>
        private void Layout(IGraphics g)
        {
            S1_MeasureWords(_tableBox, g);

            // get the table boxes into the proper fields
            // Insert EmptyBoxes for vertical cell spanning. 

            S2_PrepareBoxes();


            // Determine Row and Column Count, and ColumnWidths
            var availCellSpace = S3_CalculateCountAndWidth();

            S4_DetermineMissingColumnWidths(availCellSpace);

            // Check for minimum sizes (increment widths if necessary)
            S5_EnforceMinimumSize();

            // While table width is larger than it should, and width is reducible
            S6_EnforceMaximumSize();

            // Ensure there's no padding             
            _tableBox.PaddingLeft = _tableBox.PaddingTop = _tableBox.PaddingRight = _tableBox.PaddingBottom = CssLength.ZeroPx;

            //Actually layout cells!
            S7_LayoutCells(g);
        }

        /// <summary>
        /// Get the table boxes into the proper fields.
        /// </summary>
        private void S2_PrepareBoxes()
        {
            //===========================================================
            //part 1: analysis box 
            List<CssBox> _bodyrows = new List<CssBox>();

            foreach (var box in _tableBox.GetChildBoxIter())
            {
                switch (box.CssDisplay)
                {
                    case CssDisplay.TableCaption:
                        _caption = box;
                        break;
                    case CssDisplay.TableRow:
                        _bodyrows.Add(box);
                        break;
                    case CssDisplay.TableRowGroup:
                        foreach (CssBox childBox in box.GetChildBoxIter())
                        {
                            if (childBox.CssDisplay == CssDisplay.TableRow)
                            {
                                _bodyrows.Add(childBox);
                            }
                        }
                        break;
                    case CssDisplay.TableFooterGroup:
                        if (_footerBox != null)
                        {
                            _bodyrows.Add(box);
                        }
                        else
                        {
                            _footerBox = box;
                        }
                        break;
                    case CssDisplay.TableHeaderGroup:
                        if (_headerBox != null)
                        {
                            _bodyrows.Add(box);
                        }
                        else
                        {
                            _headerBox = box;
                        }
                        break;
                    case CssDisplay.TableColumn:
                        {

                            for (int i = GetSpan(box) - 1; i >= 0; --i)
                            {
                                _columns.Add(box);
                            }

                        } break;
                    case CssDisplay.TableColumnGroup:
                        {
                            if (box.ChildCount == 0)
                            {

                                for (int i = GetSpan(box) - 1; i >= 0; --i)
                                {
                                    _columns.Add(box);
                                }

                            }
                            else
                            {
                                foreach (CssBox childBox in box.GetChildBoxIter())
                                {

                                    for (int i = GetSpan(childBox) - 1; i >= 0; --i)
                                    {
                                        _columns.Add(childBox);
                                    }
                                }
                            }
                        } break;
                }
            }

            if (_headerBox != null)
            {
                _allRows.AddRange(_headerBox.GetChildBoxIter());
            }

            _allRows.AddRange(_bodyrows);

            if (_footerBox != null)
            {
                _allRows.AddRange(_footerBox.GetChildBoxIter());
            }

            //===========================================================
            //part 2: analysis box 
            //insert empty cell
            if (!_tableBox._tableFixed) //fix once !!!
            {
                int currow = 0;
                List<CssBox> rows = _bodyrows;
                foreach (CssBox row in rows)
                {
                    for (int c = 0; c < row.ChildCount; ++c)
                    {

                        CssBox cell = row.GetChildBox(c);

                        int rowspan = cell.RowSpan;

                        int realcol = GetCellRealColumnIndex(row, cell);

                        for (int i = currow + 1; i < currow + rowspan; ++i)
                        {
                            //expand row (top-down) 
                            if (i < rows.Count)
                            {
                                //if this is not last row
                                int colcount = 0;
                                var curRow = rows[i];
                                for (int n = 0; n < curRow.ChildCount; ++n)
                                {
                                    //all cell in this row
                                    if (colcount == realcol)
                                    {
                                        //insert new spacing box for table 
                                        //at 'colcount' index  

                                        //curRow is modified , then break to 
                                        curRow.InsertChild(colcount, new CssSpacingBox(_tableBox, cell, currow));
                                        break;
                                    }
                                    colcount++;
                                    realcol -= (curRow.GetChildBox(n)).RowSpan - 1;
                                }
                            }
                        }
                    }
                    currow++;
                }

                _tableBox._tableFixed = true;
            }
            //=========================================================== 
        }

        /// <summary>
        /// Determine Row and Column Count, and ColumnWidths
        /// </summary>
        /// <returns></returns>
        float S3_CalculateCountAndWidth()
        {
            //Columns
            if (_columns.Count > 0)
            {
                _columnCount = _columns.Count;
            }
            else
            {
                for (int i = _allRows.Count - 1; i >= 0; --i)
                {
                    var b = _allRows[i];
                    _columnCount = Math.Max(_columnCount, b.ChildCount);
                }
            }

            //Initialize column widths array with NaNs
            _columnWidths = new float[_columnCount];

            for (int i = _columnWidths.Length - 1; i >= 0; --i)
            {
                _columnWidths[i] = float.NaN;
            }

            float availSpaceForAllCells = GetAvailableWidthForAllCells();

            if (_columns.Count > 0)
            {


                // Fill ColumnWidths array by scanning column widths
                for (int i = _columns.Count - 1; i >= 0; --i)
                {
                    CssLength len = _columns[i].Width; //Get specified width

                    if (len.Number > 0) //If some width specified
                    {
                        switch (len.Unit)
                        {
                            case CssUnit.Percent:
                                {
                                    _columnWidths[i] = CssValueParser.ParseNumber(_columns[i].Width, availSpaceForAllCells);
                                } break;
                            case CssUnit.Pixels:
                            case CssUnit.None:
                                {
                                    _columnWidths[i] = len.Number; //Get width as an absolute-pixel value
                                } break;
                        }

                    }
                }
            }
            else
            {



                // Fill ColumnWidths array by scanning width in table-cell definitions
                foreach (CssBox row in _allRows)
                {
                    //Check for column width in table-cell definitions
                    for (int i = 0; i < _columnCount; i++)
                    {
                        if (i < 20 || float.IsNaN(_columnWidths[i])) // limit column width check
                        {
                            if (i < row.ChildCount)
                            {
                                var childBox = row.GetChildBox(i);
                                if (childBox.CssDisplay == CssDisplay.TableCell)
                                {
                                    float cellBoxWidth = CssValueParser.ParseLength(childBox.Width, availSpaceForAllCells, childBox);
                                    if (cellBoxWidth > 0) //If some width specified
                                    {
                                        int colspan = childBox.ColSpan;
                                        cellBoxWidth /= Convert.ToSingle(colspan);
                                        for (int n = i; n < i + colspan; n++)
                                        {
                                            _columnWidths[n] =
                                                float.IsNaN(_columnWidths[n]) ?
                                                    cellBoxWidth :
                                                    Math.Max(_columnWidths[n], cellBoxWidth);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return availSpaceForAllCells;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="availCellSpace"></param>
        private void S4_DetermineMissingColumnWidths(float availCellSpace)
        {
            float occupedSpace = 0f;
            if (_widthSpecified) //If a width was specified,
            {
                //Assign NaNs equally with space left after gathering not-NaNs
                int numOfNans = 0;

                //Calculate number of NaNs and occupied space
                for (int i = _columnWidths.Length - 1; i >= 0; --i)
                {
                    float colWidth = _columnWidths[i];
                    if (float.IsNaN(colWidth))
                    {
                        numOfNans++;
                    }
                    else
                    {
                        occupedSpace += colWidth;
                    }
                }

                int orgNumOfNans = numOfNans;

                float[] orgColWidths = null;

                if (numOfNans < _columnWidths.Length)
                {
                    orgColWidths = new float[_columnWidths.Length];
                    for (int i = _columnWidths.Length - 1; i >= 0; --i)
                    {
                        orgColWidths[i] = _columnWidths[i];
                    }
                }

                if (numOfNans > 0)
                {
                    // Determine the max width for each column
                    float[] minFullWidths, maxFullWidths;
                    CalculateColumnsMinMaxWidthByContent(true, out minFullWidths, out maxFullWidths);

                    // set the columns that can fulfill by the max width in a loop because it changes the nanWidth
                    int oldNumOfNans;
                    int colWidthCount = _columnWidths.Length;
                    do
                    {
                        oldNumOfNans = numOfNans;
                        for (int i = 0; i < colWidthCount; i++)
                        {
                            float nanWidth = (availCellSpace - occupedSpace) / numOfNans;
                            if (float.IsNaN(_columnWidths[i]) && nanWidth > maxFullWidths[i])
                            {
                                _columnWidths[i] = maxFullWidths[i];
                                numOfNans--;
                                occupedSpace += maxFullWidths[i];
                            }
                        }

                    } while (oldNumOfNans != numOfNans);


                    if (numOfNans > 0)
                    {
                        // Determine width that will be assigned to un assigned widths
                        float nanWidth = (availCellSpace - occupedSpace) / numOfNans;

                        for (int i = colWidthCount - 1; i >= 0; --i)
                        {
                            if (float.IsNaN(_columnWidths[i]))
                            {
                                _columnWidths[i] = nanWidth;
                            }
                        }
                    }
                }

                if (numOfNans == 0 && occupedSpace < availCellSpace)
                {
                    int colWidthCount = _columnWidths.Length;
                    if (orgNumOfNans > 0)
                    {
                        // spread extra width between all non width specified columns
                        float extWidth = (availCellSpace - occupedSpace) / orgNumOfNans;

                        for (int i = colWidthCount - 1; i >= 0; --i)
                        {
                            if (orgColWidths == null || float.IsNaN(orgColWidths[i]))
                            {
                                _columnWidths[i] += extWidth;
                            }
                        }
                    }
                    else
                    {
                        // spread extra width between all columns with respect to relative sizes

                        for (int i = colWidthCount - 1; i >= 0; --i)
                        {
                            _columnWidths[i] += (availCellSpace - occupedSpace) * (_columnWidths[i] / occupedSpace);
                        }
                    }
                }
            }
            else
            {
                //Get the minimum and maximum full length of NaN boxes
                float[] minFullWidths, maxFullWidths;
                CalculateColumnsMinMaxWidthByContent(true, out minFullWidths, out maxFullWidths);

                int colWidthCount = _columnWidths.Length;
                for (int i = colWidthCount - 1; i >= 0; --i)
                {
                    if (float.IsNaN(_columnWidths[i]))
                    {
                        _columnWidths[i] = minFullWidths[i];
                    }
                    occupedSpace += _columnWidths[i];
                }

                // spread extra width between all columns
                for (int i = 0; i < colWidthCount; i++)
                {
                    if (maxFullWidths[i] > _columnWidths[i])
                    {
                        var temp = _columnWidths[i];
                        _columnWidths[i] = Math.Min(_columnWidths[i] + (availCellSpace - occupedSpace) / Convert.ToSingle(_columnWidths.Length - i), maxFullWidths[i]);
                        occupedSpace = occupedSpace + _columnWidths[i] - temp;
                    }
                }
            }
        }

        /// <summary>
        /// While table width is larger than it should, and width is reductable.<br/>
        /// If table max width is limited by we need to lower the columns width even if it will result in clipping<br/>
        /// </summary>
        private void S6_EnforceMaximumSize()
        {
            int curCol = 0;
            var widthSum = CalculateWidthSum();
            while (widthSum > GetAvailableTableWidth() && CanReduceWidth())
            {
                while (!CanReduceWidth(curCol))
                {
                    curCol++;
                }

                _columnWidths[curCol] -= 1f;

                curCol++;

                if (curCol >= _columnWidths.Length)
                {
                    curCol = 0;
                }
            }

            // if table max width is limited by we need to lower the columns width even if it will result in clipping
            var maxWidth = GetMaxTableWidth();
            if (maxWidth < CssBox.MAX_RIGHT)
            {
                widthSum = CalculateWidthSum();
                if (maxWidth < widthSum)
                {
                    //Get the minimum and maximum full length of NaN boxes
                    float[] minFullWidths, maxFullWidths;
                    CalculateColumnsMinMaxWidthByContent(false, out minFullWidths, out maxFullWidths);

                    // lower all the columns to the minimum
                    for (int i = _columnWidths.Length - 1; i >= 0; --i)
                    {
                        _columnWidths[i] = minFullWidths[i];
                    }

                    // either min for all column is not enought and we need to lower it more resulting in clipping
                    // or we now have extra space so we can give it to columns than need it
                    widthSum = CalculateWidthSum();
                    if (maxWidth < widthSum)
                    {
                        // lower the width of columns starting from the largest one until the max width is satisfied
                        for (int a = 0; a < 15 && maxWidth < widthSum - 0.1; a++) // limit iteration so bug won't create infinite loop
                        {
                            int nonMaxedColumns = 0;
                            float largeWidth = 0f, secLargeWidth = 0f;
                            for (int i = 0; i < _columnWidths.Length; i++)
                            {
                                if (_columnWidths[i] > largeWidth + 0.1)
                                {
                                    secLargeWidth = largeWidth;
                                    largeWidth = _columnWidths[i];
                                    nonMaxedColumns = 1;
                                }
                                else if (_columnWidths[i] > largeWidth - 0.1)
                                {
                                    nonMaxedColumns++;
                                }
                            }

                            float decrease = secLargeWidth > 0 ? largeWidth - secLargeWidth : (widthSum - maxWidth) / _columnWidths.Length;
                            if (decrease * nonMaxedColumns > widthSum - maxWidth)
                            {
                                decrease = (widthSum - maxWidth) / nonMaxedColumns;
                            }
                            for (int i = 0; i < _columnWidths.Length; i++)
                            {
                                if (_columnWidths[i] > largeWidth - 0.1)
                                {
                                    _columnWidths[i] -= decrease;
                                }
                            }
                            widthSum = CalculateWidthSum();
                        }
                    }
                    else
                    {
                        // spread extra width to columns that didn't reached max width where trying to spread it between all columns
                        for (int a = 0; a < 15 && maxWidth > widthSum + 0.1; a++) // limit iteration so bug won't create infinite loop
                        {
                            int nonMaxedColumns = 0;
                            for (int i = 0; i < _columnWidths.Length; i++)
                            {
                                if (_columnWidths[i] + 1 < maxFullWidths[i])
                                {
                                    nonMaxedColumns++;
                                }
                            }
                            if (nonMaxedColumns == 0)
                            {
                                nonMaxedColumns = _columnWidths.Length;
                            }

                            bool hit = false;
                            float minIncrement = (maxWidth - widthSum) / nonMaxedColumns;

                            for (int i = 0; i < _columnWidths.Length; i++)
                            {
                                if (_columnWidths[i] + 0.1 < maxFullWidths[i])
                                {
                                    minIncrement = Math.Min(minIncrement, maxFullWidths[i] - _columnWidths[i]);
                                    hit = true;
                                }
                            }

                            for (int i = 0; i < _columnWidths.Length; i++)
                            {
                                if (!hit || _columnWidths[i] + 1 < maxFullWidths[i])
                                {
                                    _columnWidths[i] += minIncrement;
                                }
                            }
                            widthSum = CalculateWidthSum();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check for minimum sizes (increment widths if necessary)
        /// </summary>
        private void S5_EnforceMinimumSize()
        {
            foreach (CssBox row in _allRows)
            {
                foreach (CssBox cell in row.GetChildBoxIter())
                {
                    int colspan = cell.ColSpan;
                    int col = GetCellRealColumnIndex(row, cell);
                    int affectcol = col + colspan - 1;

                    if (_columnWidths.Length > col && _columnWidths[col] < CalculateColumnMinWidths()[col])
                    {
                        float diff = CalculateColumnMinWidths()[col] - _columnWidths[col];
                        _columnWidths[affectcol] = CalculateColumnMinWidths()[affectcol];

                        if (col < _columnWidths.Length - 1)
                        {
                            _columnWidths[col + 1] -= diff;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Layout the cells by the calculated table layout
        /// </summary>
        /// <param name="g"></param>
        private void S7_LayoutCells(IGraphics g)
        {
            float startx = Math.Max(_tableBox.ClientLeft + GetHorizontalSpacing(), 0);
            float starty = Math.Max(_tableBox.ClientTop + GetVerticalSpacing(), 0);
            float cury = starty;
            float maxRight = startx;
            float maxBottom = 0f;
            int currentrow = 0;

            for (int i = 0; i < _allRows.Count; i++)
            {
                var row = _allRows[i];
                float curx = startx;
                int curCol = 0;

                foreach (var cell in row.GetChildBoxIter())
                {
                    if (curCol >= _columnWidths.Length) break;

                    int rowspan = cell.RowSpan;
                    var columnIndex = GetCellRealColumnIndex(row, cell);
                    float width = GetCellWidth(columnIndex, cell);

                    cell.SetLocation(curx, cury);
                    cell.Size = new SizeF(width, 0f);
                    cell.PerformLayout(g); //That will automatically set the bottom of the cell

                    //Alter max bottom only if row is cell's row + cell's rowspan - 1
                    CssSpacingBox sb = cell as CssSpacingBox;
                    if (sb != null)
                    {
                        if (sb.EndRow == currentrow)
                        {
                            maxBottom = Math.Max(maxBottom, sb.ExtendedBox.ActualBottom);
                        }
                    }
                    else if (rowspan == 1)
                    {
                        maxBottom = Math.Max(maxBottom, cell.ActualBottom);
                    }
                    maxRight = Math.Max(maxRight, cell.ActualRight);
                    curCol++;
                    curx = cell.ActualRight + GetHorizontalSpacing();
                }

                foreach (CssBox cell in row.GetChildBoxIter())
                {
                    CssSpacingBox spacer = cell as CssSpacingBox;

                    if (spacer == null && cell.RowSpan == 1)
                    {
                        cell.ActualBottom = maxBottom;
                        CssLayoutEngine.ApplyCellVerticalAlignment(g, cell);
                    }
                    else if (spacer != null && spacer.EndRow == currentrow)
                    {
                        spacer.ExtendedBox.ActualBottom = maxBottom;
                        CssLayoutEngine.ApplyCellVerticalAlignment(g, spacer.ExtendedBox);
                    }
                }

                cury = maxBottom + GetVerticalSpacing();
                currentrow++;
            }

            maxRight = Math.Max(maxRight, _tableBox.LocationX + _tableBox.ActualWidth);
            _tableBox.ActualRight = maxRight + GetHorizontalSpacing() + _tableBox.ActualBorderRightWidth;
            _tableBox.ActualBottom = Math.Max(maxBottom, starty) + GetVerticalSpacing() + _tableBox.ActualBorderBottomWidth;
        }

        /// <summary>
        /// Gets the spanned width of a cell (With of all columns it spans minus one).
        /// </summary>
        float GetSpannedMinWidth(CssBox row, CssBox cell, int realcolindex, int colspan)
        {
            float w = 0f;
            for (int i = realcolindex; i < row.ChildCount || i < realcolindex + colspan - 1; ++i)
            {
                if (i < CalculateColumnMinWidths().Length)
                {
                    w += CalculateColumnMinWidths()[i];
                }
            }
            return w;
        }

        /// <summary>
        /// Gets the cell column index checking its position and other cells colspans
        /// </summary>
        /// <param name="row"></param>
        /// <param name="cell"></param>
        /// <returns></returns>
        static int GetCellRealColumnIndex(CssBox row, CssBox cell)
        {
            int i = 0;
            foreach (CssBox b in row.GetChildBoxIter())
            {
                if (b == cell)
                {
                    break;
                }
                i += b.ColSpan;
            }
            return i;
        }

        /// <summary>
        /// Gets the cells width, taking colspan and being in the specified column
        /// </summary>
        /// <param name="column"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        float GetCellWidth(int column, CssBox b)
        {
            int colspan = b.ColSpan;
            float sum = 0f;
            int col_count = _columnWidths.Length;
            if (column < col_count)
            {
                for (int i = column; (i < column + colspan) && (i < col_count); ++i)
                {
                    sum += _columnWidths[i];
                }
            }
            sum += (colspan - 1) * GetHorizontalSpacing();

            return sum; // -b.ActualBorderLeftWidth - b.ActualBorderRightWidth - b.ActualPaddingRight - b.ActualPaddingLeft;
        }
        /// <summary>
        /// Recursively measures words inside the box
        /// </summary>
        /// <param name="box">the box to measure</param>
        /// <param name="g">Device to use</param>
        static void S1_MeasureWords(CssBox box, IGraphics g)
        {
            //recursive
            if (box != null)
            {
                foreach (var childBox in box.GetChildBoxIter())
                {
                    childBox.MeasureRunsSize(g);
                    S1_MeasureWords(childBox, g);
                }
            }
        }

        /// <summary>
        /// Tells if the columns widths can be reduced,
        /// by checking the minimum widths of all cells
        /// </summary>
        /// <returns></returns>
        private bool CanReduceWidth()
        {

            for (int i = _columnWidths.Length - 1; i >= 0; --i)
            {
                if (CanReduceWidth(i))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tells if the specified column can be reduced,
        /// by checking its minimum width
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private bool CanReduceWidth(int columnIndex)
        {
            if (_columnWidths.Length >= columnIndex || CalculateColumnMinWidths().Length >= columnIndex)
            {
                return false;
            }
            return _columnWidths[columnIndex] > CalculateColumnMinWidths()[columnIndex];
        }

        /// <summary>
        /// Gets the available width for the whole table.
        /// It also sets the value of WidthSpecified
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The table's width can be larger than the result of this method, because of the minimum 
        /// size that individual boxes.
        /// </remarks>
        private float GetAvailableTableWidth()
        {
            CssLength tblen = _tableBox.Width; //new CssLength(_tableBox.Width);

            if (tblen.Number > 0)
            {
                _widthSpecified = true;
                return CssValueParser.ParseLength(_tableBox.Width, _tableBox.ParentBox.AvailableWidth, _tableBox);
            }
            else
            {
                return _tableBox.ParentBox.AvailableWidth;
            }
        }

        /// <summary>
        /// Gets the available width for the whole table.
        /// It also sets the value of WidthSpecified
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The table's width can be larger than the result of this method, because of the minimum 
        /// size that individual boxes.
        /// </remarks>
        private float GetMaxTableWidth()
        {
            var tblen = _tableBox.MaxWidth;// new CssLength(_tableBox.MaxWidth);
            if (tblen.Number > 0)
            {
                _widthSpecified = true;
                return CssValueParser.ParseLength(_tableBox.MaxWidth, _tableBox.ParentBox.AvailableWidth, _tableBox);
            }
            else
            {
                return 9999f;
            }
        }

        /// <summary>
        /// Calculate the min and max width for each column of the table by the content in all rows.<br/>
        /// the min width possible without clipping content<br/>
        /// the max width the cell content can take without wrapping<br/>
        /// </summary>
        /// <param name="onlyNans">if to measure only columns that have no calculated width</param>
        /// <param name="minFullWidths">return the min width for each column - the min width possible without clipping content</param>
        /// <param name="maxFullWidths">return the max width for each column - the max width the cell content can take without wrapping</param>
        private void CalculateColumnsMinMaxWidthByContent(bool onlyNans, out float[] minFullWidths, out float[] maxFullWidths)
        {
            maxFullWidths = new float[_columnWidths.Length];
            minFullWidths = new float[_columnWidths.Length];

            foreach (CssBox row in _allRows)
            {
                int childCount = row.ChildCount;
                int i = 0;
                foreach (var childBox in row.GetChildBoxIter())
                {
                    int col = GetCellRealColumnIndex(row, childBox);
                    col = _columnWidths.Length > col ? col : _columnWidths.Length - 1;

                    if ((!onlyNans || float.IsNaN(_columnWidths[col])) && i < row.ChildCount)
                    {
                        float minWidth, maxWidth;
                        CalculateMinMaxWidth(childBox, out minWidth, out maxWidth);
                        var colSpan = childBox.ColSpan;
                        minWidth = minWidth / colSpan;
                        maxWidth = maxWidth / colSpan;
                        for (int j = 0; j < colSpan; j++)
                        {
                            minFullWidths[col + j] = Math.Max(minFullWidths[col + j], minWidth);
                            maxFullWidths[col + j] = Math.Max(maxFullWidths[col + j], maxWidth);
                        }
                    }

                    i++;
                }

            }
        }
        /// <summary>
        /// Get the <paramref name="minWidth"/> and <paramref name="maxWidth"/> width of the box content.<br/>
        /// </summary>
        /// <param name="minWidth">The minimum width the content must be so it won't overflow (largest word + padding).</param>
        /// <param name="maxWidth">The total width the content can take without line wrapping (with padding).</param>
        internal void CalculateMinMaxWidth(CssBox box, out float minWidth, out float maxWidth)
        {
            float min = 0f;
            float maxSum = 0f;
            float paddingSum = 0f;
            float marginSum = 0f;
            CalculateMinMaxSumWords(box, ref min, ref maxSum, ref paddingSum, ref marginSum);

            maxWidth = paddingSum + maxSum;
            minWidth = paddingSum + (min < CssBox.MAX_RIGHT ? min : 0);
        }

        /// <summary>
        /// Get the <paramref name="min"/> and <paramref name="maxSum"/> of the box words content and <paramref name="paddingSum"/>.<br/>
        /// </summary>
        /// <param name="box">the box to calculate for</param>
        /// <param name="min">the width that allows for each word to fit (width of the longest word)</param>
        /// <param name="maxSum">the max width a single line of words can take without wrapping</param>
        /// <param name="paddingSum">the total amount of padding the content has </param>
        /// <param name="marginSum"></param>
        /// <returns></returns>
        static void CalculateMinMaxSumWords(CssBox box, ref float min, ref float maxSum, ref float paddingSum, ref float marginSum)
        {
            float? oldSum = null;

            // not inline (block) boxes start a new line so we need to reset the max sum 
            if (box.CssDisplay != CssDisplay.Inline &&
                box.CssDisplay != CssDisplay.TableCell &&
                box.WhiteSpace != CssWhiteSpace.NoWrap)
            {
                oldSum = maxSum;
                maxSum = marginSum;
            }

            // add the padding 
            paddingSum += box.ActualBorderLeftWidth + box.ActualBorderRightWidth + box.ActualPaddingRight + box.ActualPaddingLeft;


            // for tables the padding also contains the spacing between cells                
            if (box.CssDisplay == CssDisplay.Table)
            {
                paddingSum += CssLayoutEngineTable.GetTableSpacing(box);
            }

            if (box.HasRuns)
            {
                // calculate the min and max sum for all the words in the box 
                foreach (CssRun run in box.GetRunIter())
                {
                    maxSum += run.Width;
                    min = Math.Max(min, run.Width);
                }
            }
            else
            {
                // recursively on all the child boxes
                foreach (CssBox childBox in box.GetChildBoxIter())
                {
                    marginSum += childBox.ActualMarginLeft + childBox.ActualMarginRight;
                    CalculateMinMaxSumWords(childBox, ref min, ref maxSum, ref paddingSum, ref marginSum);

                    marginSum -= childBox.ActualMarginLeft + childBox.ActualMarginRight;
                }
            }

            // max sum is max of all the lines in the box
            if (oldSum.HasValue)
            {
                maxSum = Math.Max(maxSum, oldSum.Value);
            }
        }

        /// <summary>
        /// Gets the width available for cells
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// It takes away the cell-spacing from <see cref="GetAvailableTableWidth"/>
        /// </remarks>
        private float GetAvailableWidthForAllCells()
        {
            return GetAvailableTableWidth() - (GetHorizontalSpacing() * (_columnCount + 1)) - _tableBox.ActualBorderLeftWidth - _tableBox.ActualBorderRightWidth;
        }

        /// <summary>
        /// Gets the current sum of column widths
        /// </summary>
        /// <returns></returns>
        private float CalculateWidthSum()
        {
            float f = 0f;

            for (int i = _columnWidths.Length - 1; i >= 0; --i)
            {
                float t = _columnWidths[i];
                if (float.IsNaN(t))
                {
                    throw new Exception("CssTable Algorithm error: There's a NaN in column widths");
                }
                else
                {
                    f += t;
                }
            }

            //Take cell-spacing
            f += GetHorizontalSpacing() * (_columnWidths.Length + 1);

            //Take table borders
            f += _tableBox.ActualBorderLeftWidth + _tableBox.ActualBorderRightWidth;

            return f;
        }

        /// <summary>
        /// Gets the span attribute of the tag of the specified box
        /// </summary>
        /// <param name="b"></param>
        static int GetSpan(CssBox b)
        {
            //span attr contain number of column that element should span
            string spanValue = b.GetAttribute("span");

            if (spanValue != string.Empty)
            {
                int result;
                if (int.TryParse(spanValue, out result))
                {
                    if (result < 0)
                    {
                        return -result;
                    }
                    return result;
                }
            }
            return 1;
        }

        /// <summary>
        /// Gets the minimum width of each column
        /// </summary>
        private float[] CalculateColumnMinWidths()
        {
            if (_columnMinWidths == null)
            {

                _columnMinWidths = new float[_columnWidths.Length];

                foreach (CssBox row in _allRows)
                {
                    foreach (CssBox cell in row.GetChildBoxIter())
                    {
                        int colspan = cell.ColSpan;
                        int col = GetCellRealColumnIndex(row, cell);
                        int affectcol = Math.Min(col + colspan, _columnMinWidths.Length) - 1;
                        float spannedwidth = GetSpannedMinWidth(row, cell, col, colspan) + (colspan - 1) * GetHorizontalSpacing();

                        _columnMinWidths[affectcol] = Math.Max(_columnMinWidths[affectcol], cell.CalculateMinimumWidth() - spannedwidth);
                    }
                }
            }

            return _columnMinWidths;
        }

        /// <summary>
        /// Gets the actual horizontal spacing of the table
        /// </summary>
        private float GetHorizontalSpacing()
        {
            return _tableBox.IsBorderCollapse ? -1f : _tableBox.ActualBorderSpacingHorizontal;
        }

        /// <summary>
        /// Gets the actual horizontal spacing of the table
        /// </summary>
        private static float GetHorizontalSpacing(CssBox box)
        {
            return box.IsBorderCollapse ? -1f : box.ActualBorderSpacingHorizontal;
        }

        /// <summary>
        /// Gets the actual vertical spacing of the table
        /// </summary>
        private float GetVerticalSpacing()
        {
            return _tableBox.IsBorderCollapse ? -1f : _tableBox.ActualBorderSpacingVertical;
        }


        #endregion
    }
}
