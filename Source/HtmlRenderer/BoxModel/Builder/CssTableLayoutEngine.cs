//BSD 2014, WinterCore
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
    sealed partial class CssTableLayoutEngine
    {

        #region Fields and Consts

        /// <summary>
        /// the main box of the table
        /// </summary>
        readonly CssBox _tableBox;

        readonly List<CssBox> _allRowBoxes = new List<CssBox>();

        TableColumnCollection columnCollection = new TableColumnCollection();

        float[] _columnWidths;
        float[] _columnMinWidths;
        byte[] _columnWidthsStatus;



        const int MAX_COL_AT_THIS_VERSION = 20;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="tableBox"></param>
        private CssTableLayoutEngine(CssBox tableBox)
        {
            _tableBox = tableBox;
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
            var table = new CssTableLayoutEngine(tableBox);
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
        void Layout(IGraphics g)
        {
            S1_MeasureWords(_tableBox, g);

            // get the table boxes into the proper fields
            // Insert EmptyBoxes for vertical cell spanning.  

            var userColumnList = S2_PrepareBoxes();

            // Determine Row and Column Count, and ColumnWidths

            float availableWidth = S3_CalculateCountAndWidth(userColumnList);

            S4_DetermineMissingColumnWidths(availableWidth);

            //
            S5_CalculateColumnMinWidths();

            // Check for minimum sizes (increment widths if necessary)
            S6_EnforceMinimumSize();

            // While table width is larger than it should, and width is reducible
            S7_EnforceMaximumSize();

            // Ensure there's no padding             
            _tableBox.PaddingLeft = _tableBox.PaddingTop = _tableBox.PaddingRight = _tableBox.PaddingBottom = CssLength.ZeroPx;

            //Actually layout cells!
            S8_LayoutCells(g);
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
                    S1_MeasureWords(childBox, g); //recursive
                }
            }
        }
        /// <summary>
        /// Get the table boxes into the proper fields.
        /// </summary>
        List<CssBox> S2_PrepareBoxes()
        {
            //===========================================================
            //part 1: assign boxes into proper fields             

            List<CssBox> userDefinedColBoxes = new List<CssBox>();
            List<CssBox> bodyrows = new List<CssBox>();
            CssBox headerBox = null;
            CssBox footerBox = null;
            CssBox caption = null;

            foreach (var box in _tableBox.GetChildBoxIter())
            {
                switch (box.CssDisplay)
                {
                    case CssDisplay.TableCaption:
                        caption = box;
                        break;
                    case CssDisplay.TableRow:
                        bodyrows.Add(box);
                        break;
                    case CssDisplay.TableRowGroup:
                        foreach (CssBox childBox in box.GetChildBoxIter())
                        {
                            if (childBox.CssDisplay == CssDisplay.TableRow)
                            {
                                bodyrows.Add(childBox);
                            }
                        }
                        break;
                    case CssDisplay.TableFooterGroup:
                        if (footerBox != null)
                        {
                            bodyrows.Add(box);
                        }
                        else
                        {
                            footerBox = box;
                        }
                        break;
                    case CssDisplay.TableHeaderGroup:
                        if (headerBox != null)
                        {
                            bodyrows.Add(box);
                        }
                        else
                        {
                            headerBox = box;
                        }
                        break;
                    case CssDisplay.TableColumn:
                        {

                            for (int i = GetSpan(box) - 1; i >= 0; --i)
                            {
                                //duplicate box*** for colspan
                                userDefinedColBoxes.Add(box);
                            }

                        } break;
                    case CssDisplay.TableColumnGroup:
                        {
                            if (box.ChildCount == 0)
                            {
                                for (int i = GetSpan(box) - 1; i >= 0; --i)
                                {
                                    //duplicate box*** for colspan
                                    userDefinedColBoxes.Add(box);
                                }
                            }
                            else
                            {
                                foreach (CssBox childBox in box.GetChildBoxIter())
                                {
                                    for (int i = GetSpan(childBox) - 1; i >= 0; --i)
                                    {
                                        //duplicate box*** for colspan
                                        userDefinedColBoxes.Add(childBox);
                                    }
                                }
                            }
                        } break;
                }
            }

            if (headerBox != null)
            {
                _allRowBoxes.AddRange(headerBox.GetChildBoxIter());
            }

            _allRowBoxes.AddRange(bodyrows);

            if (footerBox != null)
            {
                _allRowBoxes.AddRange(footerBox.GetChildBoxIter());
            }

            //===========================================================
            //part 2: insert empty cell ,           
            if (!_tableBox._tableFixed) //fix once !!!
            {
                int rIndex = 0;
                int bodyRowCount = bodyrows.Count;
                foreach (CssBox row in bodyrows)
                {
                    //'row' loop 
                    int grid_index = 0;
                    for (int c = 0; c < row.ChildCount; ++c)
                    {
                        //'cell' in 'row' loop 
                        CssBox cellBox = row.GetChildBox(c);
                        int rowspan = cellBox.RowSpan;

                        if (rowspan > 1)
                        {

                            for (int i = rIndex + 1; (i < rIndex + rowspan) && (i < bodyRowCount); ++i)
                            {
                                //fill rowspan (vertical expand down) 
                                int insertAt;
                                CssBox lowerRow = bodyrows[i];
                                if (FindVerticalCellSpacingBoxInsertionPoint(lowerRow, grid_index, out insertAt))
                                {
                                    lowerRow.InsertChild(insertAt, new CssVerticalCellSpacingBox(_tableBox, cellBox, rIndex));
                                }
                            }
                        }

                        grid_index += cellBox.ColSpan;
                    }
                    rIndex++;//***
                }
                _tableBox._tableFixed = true;
            }
            //===========================================================  

            return userDefinedColBoxes;
        }

        /// <summary>
        /// Determine Row and Column Count, and ColumnWidths
        /// </summary>
        /// <returns></returns>
        float S3_CalculateCountAndWidth(List<CssBox> userDefinedColumnBoxes)
        {
            //-----------------------------------------------------------
            //1. count columns
            int columnCount = 0;
            if (userDefinedColumnBoxes.Count > 0)
            {
                //mode 1: user defined columns
                columnCount = userDefinedColumnBoxes.Count;
            }
            else
            {   //mode 2:  anonymous column definitions, 
                int cellcount = 0;
                //find max column count in the table 
                //each row may contain different number of cell 
                for (int i = _allRowBoxes.Count - 1; i >= 0; --i)
                {
                    if ((cellcount = _allRowBoxes[i].ChildCount) > columnCount)
                    {
                        columnCount = cellcount;
                    }
                }
            }
            //-------------------------------------------------------------
            //2. Initialize column widths array with NaNs
            this._columnWidths = new float[columnCount];
            this._columnWidthsStatus = new byte[columnCount];

            for (int i = columnCount - 1; i >= 0; --i)
            {
                _columnWidths[i] = float.NaN;
            }


            float availbleWidthForAllCells = GetAvailableTableWidth() - (GetHorizontalSpacing(_tableBox) * (columnCount + 1)) - _tableBox.ActualBorderLeftWidth - _tableBox.ActualBorderRightWidth;
            //-------------------------------------------------------------
            //3. 
            if (userDefinedColumnBoxes.Count > 0)
            {
                //mode 1: user defined columns  
                // Fill ColumnWidths array by scanning column widths
                for (int i = userDefinedColumnBoxes.Count - 1; i >= 0; --i)
                {
                    CssLength colWidth = userDefinedColumnBoxes[i].Width; //Get specified width

                    if (colWidth.Number > 0) //If some width specified
                    {
                        switch (colWidth.Unit)
                        {
                            case CssUnit.Percent:
                                {
                                    _columnWidths[i] = CssValueParser.ParseNumber(userDefinedColumnBoxes[i].Width, availbleWidthForAllCells);
                                } break;
                            case CssUnit.Pixels:
                            case CssUnit.None:
                                {
                                    _columnWidths[i] = colWidth.Number; //Get width as an absolute-pixel value
                                } break;
                        }
                    }
                }
            }
            else
            {
                //mode 2:  anonymous column definitions, 

                // Fill ColumnWidths array by scanning width in table-cell definitions
                foreach (CssBox row in _allRowBoxes)
                {
                    //Check for column width in table-cell definitions

                    int col_limit = columnCount > MAX_COL_AT_THIS_VERSION ? MAX_COL_AT_THIS_VERSION : columnCount;


                    for (int i = 0; i < col_limit; i++)// limit column width check
                    {
                        if (float.IsNaN(_columnWidths[i]))
                        {
                            if (i < row.ChildCount)
                            {
                                var childBox = row.GetChildBox(i);
                                if (childBox.CssDisplay == CssDisplay.TableCell)
                                {
                                    float cellBoxWidth = CssValueParser.ParseLength(childBox.Width, availbleWidthForAllCells, childBox);
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
            return availbleWidthForAllCells;
        }
        float S3_CalculateCountAndWidth()
        {
            //-----------------------------------------------------------
            //1. count columns
            int columnCount = 0;
            int cellcount = 0;
            //find max column count in the table 
            //each row may contain different number of cell 
            for (int i = _allRowBoxes.Count - 1; i >= 0; --i)
            {
                if ((cellcount = _allRowBoxes[i].ChildCount) > columnCount)
                {
                    columnCount = cellcount;
                }
            }
            //-------------------------------------------------------------
            //2. Initialize column widths array with NaNs
            _columnWidths = new float[columnCount];
            for (int i = columnCount - 1; i >= 0; --i)
            {
                _columnWidths[i] = float.NaN;
            }
            float availbleWidthForAllCells = GetAvailableTableWidth() - (GetHorizontalSpacing(_tableBox) * (columnCount + 1)) - _tableBox.ActualBorderLeftWidth - _tableBox.ActualBorderRightWidth;




            //  anonymous column definitions,  
            // Fill ColumnWidths array by scanning width in table-cell definitions
            foreach (CssBox row in _allRowBoxes)
            {
                //Check for column width in table-cell definitions

                int col_limit = columnCount > MAX_COL_AT_THIS_VERSION ? MAX_COL_AT_THIS_VERSION : columnCount;


                for (int i = 0; i < col_limit; i++)// limit column width check
                {
                    if (float.IsNaN(_columnWidths[i]))
                    {
                        if (i < row.ChildCount)
                        {
                            var childBox = row.GetChildBox(i);
                            if (childBox.CssDisplay == CssDisplay.TableCell)
                            {
                                float cellBoxWidth = CssValueParser.ParseLength(childBox.Width, availbleWidthForAllCells, childBox);
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

            return availbleWidthForAllCells;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="availCellSpace"></param>
        void S4_DetermineMissingColumnWidths(float availCellSpace)
        {


            if (_tableBox.Width.Number > 0) //If a width was specified,
            {
                float occupiedSpace = 0f;
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
                        occupiedSpace += colWidth;
                    }
                }

                int orgNumOfNans = numOfNans;

                float[] orgColWidths = null;

                if (numOfNans < _columnWidths.Length)
                {
                    //backup 
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
                            float nanWidth = (availCellSpace - occupiedSpace) / numOfNans;
                            if (float.IsNaN(_columnWidths[i]) && nanWidth > maxFullWidths[i])
                            {
                                _columnWidths[i] = maxFullWidths[i];
                                numOfNans--;
                                occupiedSpace += maxFullWidths[i];
                            }
                        }

                    } while (oldNumOfNans != numOfNans);


                    if (numOfNans > 0)
                    {
                        // Determine width that will be assigned to un assigned widths
                        float nanWidth = (availCellSpace - occupiedSpace) / numOfNans;
                        for (int i = colWidthCount - 1; i >= 0; --i)
                        {
                            if (float.IsNaN(_columnWidths[i]))
                            {
                                _columnWidths[i] = nanWidth;
                            }
                        }
                    }
                }

                if (numOfNans == 0 && occupiedSpace < availCellSpace)
                {
                    int colWidthCount = _columnWidths.Length;
                    if (orgNumOfNans > 0)
                    {
                        // spread extra width between all non width specified columns
                        float extWidth = (availCellSpace - occupiedSpace) / orgNumOfNans;
                        if (orgColWidths == null)
                        {
                            for (int i = colWidthCount - 1; i >= 0; --i)
                            {
                                _columnWidths[i] += extWidth;
                            }
                        }
                        else
                        {
                            for (int i = colWidthCount - 1; i >= 0; --i)
                            {
                                if (float.IsNaN(orgColWidths[i]))
                                {
                                    _columnWidths[i] += extWidth;
                                }
                            }
                        }
                    }
                    else
                    {
                        // spread extra width between all columns with respect to relative sizes 
                        for (int i = colWidthCount - 1; i >= 0; --i)
                        {
                            _columnWidths[i] += (availCellSpace - occupiedSpace) * (_columnWidths[i] / occupiedSpace);
                        }
                    }
                }
            }
            else
            {
                float occupiedSpace = 0f;
                //Get the minimum and maximum full length of NaN boxes
                float[] minFullWidths, maxFullWidths;

                CalculateColumnsMinMaxWidthByContent(true, out minFullWidths, out maxFullWidths);

                int colWidthCount = _columnWidths.Length;

                float c_width = 0;
                for (int i = colWidthCount - 1; i >= 0; --i)
                {
                    c_width = _columnWidths[i];
                    if (float.IsNaN(c_width))
                    {
                        _columnWidths[i] = c_width = minFullWidths[i];
                    }
                    occupiedSpace += c_width;
                }

                // spread extra width between all columns
                for (int i = 0; i < colWidthCount; i++)
                {
                    if ((c_width = _columnWidths[i]) < maxFullWidths[i])
                    {
                        _columnWidths[i] = c_width = Math.Min(c_width + (availCellSpace - occupiedSpace) / Convert.ToSingle(colWidthCount - i), maxFullWidths[i]);
                        occupiedSpace += c_width;
                    }
                }
            }
        }

        /// <summary>
        /// While table width is larger than it should, and width is reductable.<br/>
        /// If table max width is limited by we need to lower the columns width even if it will result in clipping<br/>
        /// </summary>
        void S7_EnforceMaximumSize()
        {

            var widthSum = CalculateWidthSum();

            if (widthSum > this.GetAvailableTableWidth())
            {
                //try reduce...
                int cIndex = 0;
                int foundAt;
                while (FindFirstReducibleColumnWidth(cIndex, out foundAt))
                {
                    _columnWidths[foundAt] -= 1f;
                    cIndex = foundAt + 1;
                    if (cIndex >= _columnWidths.Length)
                    {
                        cIndex = 0; //retry again 
                    }
                }
            }

            //--------------------------------------------------

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
                        for (int a = 0; a < 15 && maxWidth < widthSum - 0.1; a++)
                        {
                            // limit iteration so bug won't create infinite loop

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
                        for (int a = 0; a < 15 && maxWidth > widthSum + 0.1; a++)
                        {
                            // limit iteration so bug won't create infinite loop

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
        void S6_EnforceMinimumSize()
        {
            int col_count = _columnWidths.Length;
            float[] col_min_widths = this._columnMinWidths;

            foreach (CssBox row in _allRowBoxes)
            {
                int grid_index = 0;
                foreach (CssBox cellBox in row.GetChildBoxIter())
                {
                    if (grid_index < col_count && _columnWidths[grid_index] < col_min_widths[grid_index])
                    {
                        int affect_col = grid_index + cellBox.ColSpan - 1;

                        _columnWidths[affect_col] = col_min_widths[affect_col];
                        if (grid_index < col_count - 1)
                        {
                            _columnWidths[grid_index + 1] -= (col_min_widths[grid_index] - _columnWidths[grid_index]);
                        }
                    }

                    //------------------------------
                    grid_index += cellBox.ColSpan; //****
                }
            }
        }

        /// <summary>
        /// Layout the cells by the calculated table layout
        /// </summary>
        /// <param name="g"></param>
        void S8_LayoutCells(IGraphics g)
        {
            float vertical_spacing = GetVerticalSpacing(_tableBox);
            float horizontal_spacing = GetHorizontalSpacing(_tableBox);

            float startx = Math.Max(_tableBox.ClientLeft + horizontal_spacing, 0);
            float starty = Math.Max(_tableBox.ClientTop + vertical_spacing, 0);
            float cury = starty;
            float maxRight = startx;
            float maxBottom = 0f;
            int currentRow = 0;
            int col_count = _columnWidths.Length;

            for (int i = 0; i < _allRowBoxes.Count; i++)
            {
                var row = _allRowBoxes[i];
                float curx = startx;

                int cIndex = 0;
                int grid_index = 0;

                foreach (CssBox cell in row.GetChildBoxIter())
                {
                    if (cIndex >= col_count)
                    {
                        break;
                    }
                    else
                    {
                        int colspan = cell.ColSpan;
                        float width = GetCellWidth(grid_index, colspan, horizontal_spacing);

                        cell.SetLocation(curx, cury);
                        cell.Size = new SizeF(width, 0f);
                        cell.PerformLayout(g); //That will automatically set the bottom of the cell

                        //Alter max bottom only if row is cell's row + cell's rowspan - 1
                        CssVerticalCellSpacingBox sb = cell as CssVerticalCellSpacingBox;
                        if (sb != null)
                        {
                            if (sb.EndRow == currentRow)
                            {
                                maxBottom = Math.Max(maxBottom, sb.ExtendedBox.ActualBottom);
                            }
                        }
                        else if (cell.RowSpan == 1)
                        {
                            maxBottom = Math.Max(maxBottom, cell.ActualBottom);
                        }

                        maxRight = Math.Max(maxRight, cell.ActualRight);

                        curx = cell.ActualRight + horizontal_spacing;

                        //-------------------------
                        cIndex++;
                        grid_index += colspan;
                    }
                }

                foreach (CssBox cell in row.GetChildBoxIter())
                {
                    CssVerticalCellSpacingBox spacer = cell as CssVerticalCellSpacingBox;

                    if (spacer == null)
                    {
                        if (cell.RowSpan == 1)
                        {
                            cell.ActualBottom = maxBottom;
                            ApplyCellVerticalAlignment(g, cell);
                        }
                    }
                    else
                    {
                        if (spacer.EndRow == currentRow)
                        {
                            spacer.ExtendedBox.ActualBottom = maxBottom;
                            ApplyCellVerticalAlignment(g, spacer.ExtendedBox);
                        }
                    }
                }

                cury = maxBottom + vertical_spacing;
                currentRow++;
            }

            maxRight = Math.Max(maxRight, _tableBox.LocationX + _tableBox.ActualWidth);
            _tableBox.ActualRight = maxRight + horizontal_spacing + _tableBox.ActualBorderRightWidth;
            _tableBox.ActualBottom = Math.Max(maxBottom, starty) + vertical_spacing + _tableBox.ActualBorderBottomWidth;
        }


        /// <summary>
        /// Get the table cells spacing for all the cells in the table.<br/>
        /// Used to calculate the spacing the table has in addition to regular padding and borders.
        /// </summary>
        /// <param name="tableBox">the table box to calculate the spacing for</param>
        /// <returns>the calculated spacing</returns>
        static float CalculateTableSpacing(CssBox tableBox)
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
                if (count > MAX_COL_AT_THIS_VERSION)
                {
                    break;
                }
            }

            // +1 columns because padding is between the cell and table borders
            return (columns + 1) * GetHorizontalSpacing(tableBox);
        }


        /// <summary>
        /// Applies special vertical alignment for table-cells
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cell"></param>
        static void ApplyCellVerticalAlignment(IGraphics g, CssBox cell)
        {
            ArgChecker.AssertArgNotNull(g, "g");
            ArgChecker.AssertArgNotNull(cell, "cell");

            float dist = 0f;
            switch (cell.VerticalAlign)
            {
                case CssVerticalAlign.Bottom:
                    dist = cell.ClientBottom - cell.CalculateMaximumBottom(cell, 0f);
                    break;
                case CssVerticalAlign.Middle:
                    dist = (cell.ClientBottom - cell.CalculateMaximumBottom(cell, 0f)) / 2;
                    break;
                default:
                    return;
            }

            foreach (CssBox b in cell.GetChildBoxIter())
            {
                b.OffsetTop(dist);
            }

        }

        /// <summary>
        /// Gets the spanned width of a cell (With of all columns it spans minus one).
        /// </summary>
        static float GetSpannedMinWidth(float[] min_widths, int rowCellCount, int realcolindex, int colspan)
        {
            float w = 0f;
            int min_widths_len = min_widths.Length;
            for (int i = realcolindex; (i < min_widths_len) && (i < rowCellCount || i < realcolindex + colspan - 1); ++i)
            {
                w += min_widths[i];
            }

            return w;
        }
        /// <summary>
        /// Gets the cells width, taking colspan and being in the specified column
        /// </summary>
        /// <param name="cIndex"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        float GetCellWidth(int cIndex, int colspan, float horizontal_spacing)
        {
            if (colspan == 1)
            {
                return _columnWidths[cIndex];
            }
            else
            {
                float sum = 0f;
                int col_count = _columnWidths.Length;
                for (int i = cIndex; (i < cIndex + colspan) && (i < col_count); ++i)
                {
                    sum += _columnWidths[i];
                }
                sum += (colspan - 1) * horizontal_spacing;
                return sum;
            }
        }

        bool FindFirstReducibleColumnWidth(int startAtIndex, out int foundAtIndex)
        {

            int col_count = _columnWidths.Length;
            for (int i = startAtIndex; i < col_count; ++i)
            {
                if (_columnWidths[i] > _columnMinWidths[i])
                {
                    foundAtIndex = i;
                    return true;
                }
            }
            foundAtIndex = -1;
            return false;
        }
        static bool FindVerticalCellSpacingBoxInsertionPoint(CssBox curRow, int cIndex, out int insertAt)
        {
            int colcount = 0;
            int cellCount = curRow.ChildCount;
            for (int n = 0; n < cellCount; ++n)
            {
                //all cell in this row
                if (colcount == cIndex)
                {
                    //insert new spacing box for table 
                    //at 'colcount' index                        
                    insertAt = colcount;
                    return true;//found
                }
                else
                {
                    colcount++;
                    cIndex -= (curRow.GetChildBox(n)).RowSpan - 1;
                }
            }
            insertAt = -1;
            return false;
        }

        /// <summary>
        /// Gets the available width for the whole table.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The table's width can be larger than the result of this method, because of the minimum 
        /// size that individual boxes.
        /// </remarks>
        /// 
        private float GetAvailableTableWidth()
        {
            CssLength tblen = _tableBox.Width;
            if (tblen.Number > 0)
            {
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
            var tblen = _tableBox.MaxWidth;
            if (tblen.Number > 0)
            {

                return CssValueParser.ParseLength(_tableBox.MaxWidth, _tableBox.ParentBox.AvailableWidth, _tableBox);
            }
            else
            {
                return CssBox.MAX_TABLE_WIDTH;
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

            int col_count = _columnWidths.Length;
            maxFullWidths = new float[col_count];
            minFullWidths = new float[col_count];

            if (onlyNans)
            {
                foreach (CssBox row in _allRowBoxes)
                {
                    int gridIndex = 0;
                    foreach (CssBox cellBox in row.GetChildBoxIter())
                    {

                        int cIndex = gridIndex < col_count ? gridIndex : col_count - 1;
                        if (float.IsNaN(_columnWidths[cIndex]))
                        {
                            float minWidth, maxWidth;
                            CalculateMinMaxWidth(cellBox, out minWidth, out maxWidth);
                            int colSpan = cellBox.ColSpan;
                            if (colSpan == 1)
                            {
                                minFullWidths[cIndex] = Math.Max(minFullWidths[cIndex], minWidth);
                                maxFullWidths[cIndex] = Math.Max(maxFullWidths[cIndex], maxWidth);
                            }
                            else
                            {
                                minWidth /= colSpan;
                                maxWidth /= colSpan;
                                for (int n = 0; n < colSpan; n++)
                                {
                                    minFullWidths[cIndex + n] = Math.Max(minFullWidths[cIndex + n], minWidth);
                                    maxFullWidths[cIndex + n] = Math.Max(maxFullWidths[cIndex + n], maxWidth);
                                }
                            }
                        }
                        //----
                        gridIndex += cellBox.ColSpan;
                    }
                }
            }
            else
            {
                foreach (CssBox row in _allRowBoxes)
                {
                    int gridIndex = 0;
                    foreach (CssBox cellBox in row.GetChildBoxIter())
                    {
                        int cIndex = gridIndex < col_count ? gridIndex : col_count - 1;

                        float minWidth, maxWidth;
                        CalculateMinMaxWidth(cellBox, out minWidth, out maxWidth);
                        int colSpan = cellBox.ColSpan;
                        if (colSpan == 1)
                        {
                            minFullWidths[cIndex] = Math.Max(minFullWidths[cIndex], minWidth);
                            maxFullWidths[cIndex] = Math.Max(maxFullWidths[cIndex], maxWidth);
                        }
                        else
                        {
                            minWidth /= colSpan;
                            maxWidth /= colSpan;
                            for (int n = 0; n < colSpan; n++)
                            {
                                minFullWidths[cIndex + n] = Math.Max(minFullWidths[cIndex + n], minWidth);
                                maxFullWidths[cIndex + n] = Math.Max(maxFullWidths[cIndex + n], maxWidth);
                            }
                        }
                        //----
                        gridIndex += cellBox.ColSpan;
                    }
                }
            }



        }
        /// <summary>
        /// Get the <paramref name="minWidth"/> and <paramref name="maxWidth"/> width of the box content.<br/>
        /// </summary>
        /// <param name="minWidth">The minimum width the content must be so it won't overflow (largest word + padding).</param>
        /// <param name="maxWidth">The total width the content can take without line wrapping (with padding).</param>
        static void CalculateMinMaxWidth(CssBox box, out float minWidth, out float maxWidth)
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
            //recursive

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
                paddingSum += CssTableLayoutEngine.CalculateTableSpacing(box);
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

                    float msum = childBox.ActualMarginLeft + childBox.ActualMarginRight;
                    marginSum += msum;

                    //recursive
                    CalculateMinMaxSumWords(childBox, ref min, ref maxSum, ref paddingSum, ref marginSum);

                    marginSum -= msum;
                }
            }

            // max sum is max of all the lines in the box
            if (oldSum.HasValue)
            {
                maxSum = Math.Max(maxSum, oldSum.Value);
            }
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
            f += GetHorizontalSpacing(_tableBox) * (_columnWidths.Length + 1);

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
        void S5_CalculateColumnMinWidths()
        {

            int col_count = _columnWidths.Length;
            float[] col_min_widths = this._columnMinWidths = new float[col_count];
            float horizontal_spacing = GetHorizontalSpacing(this._tableBox);

            foreach (CssBox row in _allRowBoxes)
            {
                int gridIndex = 0;
                foreach (CssBox cellBox in row.GetChildBoxIter())
                {
                    int colspan = cellBox.ColSpan;
                    int affect_col = Math.Min(gridIndex + colspan, col_count) - 1;
                    if (colspan == 1)
                    {
                        float spanned_width = (colspan - 1) * horizontal_spacing;
                        col_min_widths[affect_col] = Math.Max(col_min_widths[affect_col], cellBox.CalculateMinimumWidth() - spanned_width);
                    }
                    else
                    {
                        float spanned_width = GetSpannedMinWidth(col_min_widths, row.ChildCount, gridIndex, colspan) + (colspan - 1) * horizontal_spacing;
                        col_min_widths[affect_col] = Math.Max(col_min_widths[affect_col], cellBox.CalculateMinimumWidth() - spanned_width);
                    }
                    gridIndex += cellBox.ColSpan;
                }
            }

        }



        /// <summary>
        /// Gets the actual horizontal spacing of the table
        /// </summary>
        static float GetHorizontalSpacing(CssBox tableBox)
        {
            return tableBox.IsBorderCollapse ? -1f : tableBox.ActualBorderSpacingHorizontal;
        }

        /// <summary>
        /// Gets the actual vertical spacing of the table
        /// </summary>
        static float GetVerticalSpacing(CssBox tableBox)
        {
            return tableBox.IsBorderCollapse ? -1f : tableBox.ActualBorderSpacingVertical;
        }


        #endregion
    }
}
