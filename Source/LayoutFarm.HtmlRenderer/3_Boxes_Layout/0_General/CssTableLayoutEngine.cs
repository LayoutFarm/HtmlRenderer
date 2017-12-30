//BSD, 2014-2017, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

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
using LayoutFarm.Css;
using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    /// <summary>
    /// Layout engine for tables executing the complex layout of tables with rows/columns/headers/etc.
    /// </summary>
    sealed partial class CssTableLayoutEngine
    {
        /// <summary>
        /// the main box of the table
        /// </summary>
        readonly CssBox _tableBox;
        readonly List<CssBox> _allRowBoxes = new List<CssBox>();
        ITextService _tmpIFonts;
        TableColumnCollection columnCollection;
        const int MAX_COL_AT_THIS_VERSION = 20;
        float hostAvaliableWidth;
        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="tableBox"></param>
        private CssTableLayoutEngine(CssBox tableBox, float hostAvaliableWidth)
        {
            _tableBox = tableBox;
            this.hostAvaliableWidth = hostAvaliableWidth;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="tableBox"> </param>
        public static void PerformLayout(CssBox tableBox, float hostAvailableWidth, LayoutVisitor lay)
        {
            var table = new CssTableLayoutEngine(tableBox, hostAvailableWidth);
            table._tmpIFonts = lay.SampleIFonts;
            table.Layout(lay);
            table._tmpIFonts = null;
        }


        #region Private Methods

        /// <summary>
        /// Analyzes the Table and assigns values to this CssTable object.
        /// To be called from the constructor
        /// </summary>
        void Layout(LayoutVisitor lay)
        {
            S1_RecursiveMeasureRunContentSize(_tableBox, lay);
            //------------------------------------------------ 
            // get the table boxes into the proper fields
            // Insert EmptyBoxes for vertical cell spanning.  

            List<CssBox> userColumnList = S2_PrepareTableCellBoxes();
            // Determine Row and Column Count, and ColumnWidths 
            float availableWidthForAllCells = S3_CalculateCountAndWidth(userColumnList);
            S4_DetermineMissingColumnWidths(availableWidthForAllCells);
            //
            S5_CalculateColumnMinWidths(lay.EpisodeId);
            // Check for minimum sizes (increment widths if necessary)
            S6_EnforceMinimumSize();
            // While table width is larger than it should, and width is reducible
            S7_EnforceMaximumSize();
            // Ensure there's no padding             
            //_tableBox.PaddingLeft = _tableBox.PaddingTop = _tableBox.PaddingRight = _tableBox.PaddingBottom = CssLength.ZeroPx;

            //Actually layout cells!
            S8_LayoutCells(lay);
        }
        /// <summary>
        /// Recursively measures run inside the box
        /// </summary>
        /// <param name="box">the box to measure</param>
        /// <param name="g">Device to use</param>
        static void S1_RecursiveMeasureRunContentSize(CssBox box, LayoutVisitor lay)
        {
            //recursive
            if (box != null)
            {
                float box_fontsize = box.ResolvedFont.SizeInPixels;
                var ifonts = lay.SampleIFonts;
                foreach (var childBox in box.GetChildBoxIter())
                {
                    childBox.ReEvaluateFont(ifonts, box_fontsize);
                    childBox.MeasureRunsSize(lay);
                    S1_RecursiveMeasureRunContentSize(childBox, lay); //recursive
                }
            }
        }
        /// <summary>
        /// Get the table boxes into the proper fields.
        /// </summary>
        List<CssBox> S2_PrepareTableCellBoxes()
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
                            //internal, use same field as TableCell

                            for (int i = box.ColSpan - 1; i >= 0; --i)
                            {
                                //duplicate box*** for colspan
                                userDefinedColBoxes.Add(box);
                            }
                        }
                        break;
                    case CssDisplay.TableColumnGroup:
                        {
                            if (box.ChildCount == 0)
                            {
                                for (int i = box.ColSpan - 1; i >= 0; --i)
                                {
                                    //duplicate box*** for colspan
                                    userDefinedColBoxes.Add(box);
                                }
                            }
                            else
                            {
                                foreach (CssBox childBox in box.GetChildBoxIter())
                                {
                                    for (int i = childBox.ColSpan - 1; i >= 0; --i)
                                    {
                                        //duplicate box*** for colspan
                                        userDefinedColBoxes.Add(childBox);
                                    }
                                }
                            }
                        }
                        break;
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
            if (!_tableBox.IsTableFixed) //fix once !!!
            {
                int rIndex = 0;
                int bodyRowCount = bodyrows.Count;
                var ifonts = _tmpIFonts;
                foreach (CssBox row in bodyrows)
                {
                    //'row' loop 
                    int grid_index = 0;
                    var cnode = row.GetFirstChild();
                    int c = 0;
                    while (cnode != null)
                    {
                        //'cell' in 'row' loop 
                        int rowspan = cnode.RowSpan;
                        if (rowspan > 1)
                        {
                            for (int i = rIndex + 1; (i < rIndex + rowspan) && (i < bodyRowCount); ++i)
                            {
                                //fill rowspan (vertical expand down) 
                                CssBox insertAt;
                                CssBox lowerRow = bodyrows[i];
                                if (FindVerticalCellSpacingBoxInsertionPoint(lowerRow, grid_index, out insertAt))
                                {
                                    var verticalCellSpacingBox = new CssVerticalCellSpacingBox(cnode, rIndex);
                                    verticalCellSpacingBox.ReEvaluateComputedValues(ifonts, _tableBox);
                                    CssBox.ChangeDisplayType(verticalCellSpacingBox, Css.CssDisplay.None);
                                    lowerRow.InsertChild(insertAt, verticalCellSpacingBox);
                                }
                            }
                        }

                        grid_index += cnode.ColSpan;
                        //-------------------------------
                        cnode = cnode.GetNextNode();
                        c++;
                    }

                    rIndex++;//***
                }

                _tableBox.IsTableFixed = true;
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
            //2. create column collection
            this.columnCollection = new TableColumnCollection(columnCount);
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
                        switch (colWidth.UnitOrNames)
                        {
                            case CssUnitOrNames.Percent:
                                {
                                    return (colWidth.Number / 100) * availbleWidthForAllCells;
                                }
                            case CssUnitOrNames.Pixels:
                            case CssUnitOrNames.EmptyValue:
                                {
                                    //Get width as an absolute-pixel value
                                    columnCollection.SetColumnWidth(i, colWidth.Number);
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                //mode 2:  anonymous column definitions, 

                // Fill ColumnWidths array by scanning width in table-cell definitions
                var tmpRows = this._allRowBoxes;
                int rowCount = tmpRows.Count;
                for (int rr = 0; rr < rowCount; ++rr)
                {
                    CssBox row = tmpRows[rr];
                    //Check for column width in table-cell definitions
                    int col_limit = columnCount > MAX_COL_AT_THIS_VERSION ? MAX_COL_AT_THIS_VERSION : columnCount;
                    var cnode = row.GetFirstChild();
                    int i = 0;
                    while (cnode != null)
                    {
                        if (!columnCollection[i].HasSpecificWidth)
                        {
                            if (i < row.ChildCount)
                            {
                                var childBox = cnode;
                                if (childBox.CssDisplay == CssDisplay.TableCell)
                                {
                                    CssLength cellWidth = childBox.Width;
                                    if (cellWidth.IsAuto)
                                    {
                                        //auto width - always 0 
                                    }
                                    else
                                    {
                                        float cellBoxWidth = CssValueParser.ConvertToPx(childBox.Width, availbleWidthForAllCells, childBox);
                                        if (cellBoxWidth > 0) //If some width specified
                                        {
                                            int colspan = childBox.ColSpan;
                                            cellBoxWidth /= colspan;
                                            for (int n = i; n < i + colspan; n++)
                                            {
                                                columnCollection[n].S3_UpdateIfWider(cellBoxWidth, ColumnSpecificWidthLevel.FromCellConstraint);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //--------------------------
                        cnode = cnode.GetNextNode();
                        i++;
                        if (i >= col_limit)
                        {
                            break;
                        }
                    }
                }
            }
            return availbleWidthForAllCells;
        }


        void S4_DetermineMissingColumnWidths(float availableWidthForAllCells)
        {
            float occupiedSpace = 0f;
            //Get the minimum and maximum full length of NaN boxes        
            S4S7_CalculateColumnsMinMaxWidthByContent(true);
            int colWidthCount = this.columnCollection.Count;
            //if no user specific column width
            //set each column to start at minimum width

            for (int i = colWidthCount - 1; i >= 0; --i)
            {
                TableColumn col = this.columnCollection[i];
                float c_width = col.Width;
                if (!col.HasSpecificWidth)
                {
                    c_width = col.MinContentWidth;
                    col.UseMinWidth();
                }
                occupiedSpace += c_width;
            }

            // spread extra width between all columns
            if ((availableWidthForAllCells - occupiedSpace) > 0)
            {
                for (int i = 0; i < colWidthCount; i++)
                {
                    TableColumn col = this.columnCollection[i];
                    float c_width = col.Width;
                    float remainingWidth = availableWidthForAllCells - occupiedSpace;
                    if (remainingWidth > 0)
                    {
                        if (!col.HasSpecificWidth)// && !col.TouchUpperLimit)
                        {
                            float newW = Math.Min(
                               c_width + (remainingWidth) / ((float)(colWidthCount - i)),
                               col.MaxContentWidth);
                            col.SetWidth(newW, ColumnSpecificWidthLevel.Adjust);
                            occupiedSpace += (newW - c_width);
                        }
                    }
                }
                //----------
                //if remaining width

            }
            if ((availableWidthForAllCells - occupiedSpace) > 0)
            {
                if (_tableBox.Width.Number > 0)
                {
                    //fill remaining space
                    if ((availableWidthForAllCells - occupiedSpace) > 0)
                    {
                        for (int i = 0; i < colWidthCount; i++)
                        {
                            TableColumn col = this.columnCollection[i];
                            float c_width = col.Width;
                            float remainingWidth = availableWidthForAllCells - occupiedSpace;
                            if (remainingWidth > 0)
                            {
                                if (!col.HasSpecificWidth)// && !col.TouchUpperLimit)
                                {
                                    float newW = c_width + (remainingWidth) / ((float)(colWidthCount - i));
                                    col.SetWidth(newW, ColumnSpecificWidthLevel.Adjust);
                                    occupiedSpace += (newW - c_width);
                                }
                            }
                        }
                    }
                    if ((availableWidthForAllCells - occupiedSpace) > 0)
                    {
                    }
                }
            }

            //------------------------------------------------------------------------------------------------
            //below here is old version

            //            if (_tableBox.Width.Number > 0) //If a width was specified,
            //            {
            //                float occupiedSpace;
            //                int numOfNonSpec;
            //                this.columnCollection.CountUnspecificWidthColumnAndOccupiedSpace(
            //                    out numOfNonSpec, out occupiedSpace);

            //                ///original number of nonspecific width
            //                int orgNumOfNonSpecificWidth = numOfNonSpec;
            //#if DEBUG
            //                if (orgNumOfNonSpecificWidth == 0)
            //                {
            //                    throw new NotSupportedException();
            //                }
            //#endif
            //                bool hasSomeNonSpecificWidth = numOfNonSpec < this.columnCollection.Count;

            //                if (numOfNonSpec > 0)
            //                {
            //                    // Determine the max width for each column
            //                    S4S7_CalculateColumnsMinMaxWidthByContent(true);

            //                    // set the columns that can fulfill by the max width in a loop because it changes the nanWidth
            //                    int oldNumOfNonSpecific;
            //                    int colWidthCount = columnCollection.Count;
            //                    do
            //                    {
            //                        oldNumOfNonSpecific = numOfNonSpec;
            //                        for (int i = 0; i < colWidthCount; i++)
            //                        {
            //                            //calculate every loop
            //                            float suggestedWidth = (availableWidthForAllCells - occupiedSpace) / numOfNonSpec;
            //                            TableColumn col = this.columnCollection[i];
            //                            switch (col.SpecificWidthLevel)
            //                            {
            //                                case ColumnSpecificWidthLevel.None:
            //                                    {
            //                                        if (col.MaxWidth < suggestedWidth)
            //                                        {
            //                                            col.UseMaxWidth();
            //                                            numOfNonSpec--;
            //                                            occupiedSpace += col.Width;
            //                                        }
            //                                    } break;
            //                                default:
            //                                    break;
            //                            }
            //                        }
            //                    } while (oldNumOfNonSpecific != numOfNonSpec);


            //                    if (numOfNonSpec > 0)
            //                    {
            //                        // Determine width that will be assigned to un assigned widths
            //                        float nanWidth = (availableWidthForAllCells - occupiedSpace) / numOfNonSpec;
            //                        this.columnCollection.S4_AddMoreWidthToColumns(true, nanWidth);
            //                    }
            //                }

            //                if (numOfNonSpec == 0 && occupiedSpace < availableWidthForAllCells)
            //                {
            //                    int colWidthCount = this.columnCollection.Count;

            //                    if (orgNumOfNonSpecificWidth > 0)
            //                    {
            //                        // spread extra width between all non width specified columns
            //                        float extWidth = (availableWidthForAllCells - occupiedSpace) / orgNumOfNonSpecificWidth;
            //                        this.columnCollection.S4_AddMoreWidthToColumns(hasSomeNonSpecificWidth, extWidth);
            //                    }
            //                    else
            //                    {
            //                        // spread extra width between all columns with respect to relative sizes  
            //                        for (int i = colWidthCount - 1; i >= 0; --i)
            //                        {
            //                            var col = this.columnCollection[i];
            //                            col.AddMoreWidthValue((availableWidthForAllCells - occupiedSpace) * (col.Width / occupiedSpace),
            //                                 ColumnSpecificWidthLevel.Adjust);
            //                        }
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                float occupiedSpace = 0f;
            //                //Get the minimum and maximum full length of NaN boxes        
            //                S4S7_CalculateColumnsMinMaxWidthByContent(true);

            //                int colWidthCount = this.columnCollection.Count;

            //                //if no user specific column width
            //                //set each column to start at minimum width

            //                for (int i = colWidthCount - 1; i >= 0; --i)
            //                {
            //                    float c_width = 0;
            //                    TableColumn col = this.columnCollection[i];
            //                    if (!col.HasSpecificWidth)
            //                    {
            //                        c_width = col.MinWidth;
            //                        col.UseMinWidth();
            //                    }
            //                    occupiedSpace += c_width;
            //                }

            //                // spread extra width between all columns
            //                if ((availableWidthForAllCells - occupiedSpace) > 0)
            //                {
            //                    for (int i = 0; i < colWidthCount; i++)
            //                    {

            //                        TableColumn col = this.columnCollection[i];
            //                        float c_width = col.Width;
            //                        float remainingWidth = availableWidthForAllCells - occupiedSpace;
            //                        if (remainingWidth > 0)
            //                        {
            //                            if (!col.HasSpecificWidth && !col.TouchUpperLimit)
            //                            {

            //                                float newW = Math.Min(
            //                                   c_width + (remainingWidth) / ((float)(colWidthCount - i)),
            //                                   col.MaxWidth);

            //                                col.SetWidth(newW, ColumnSpecificWidthLevel.Adjust);

            //                                occupiedSpace += (newW - c_width);
            //                            }
            //                        }

            //                    }
            //                }
            //            }
        }


        /// <summary>
        /// Gets the minimum width of each column
        /// </summary>
        void S5_CalculateColumnMinWidths(int layoutIdEpisode)
        {
            int col_count = this.columnCollection.Count;
            float horizontal_spacing = GetHorizontalSpacing(this._tableBox);
            foreach (CssBox row in _allRowBoxes)
            {
                int gridIndex = 0;
                int thisRowCellCount = row.ChildCount;
                foreach (CssBox cellBox in row.GetChildBoxIter())
                {
                    int colspan = cellBox.ColSpan;
                    int affect_col = Math.Min(gridIndex + colspan, col_count) - 1;
                    var col = this.columnCollection[affect_col];
                    float spanned_width = 0;
                    float minimumCellWidth = cellBox.CalculateMinimumWidth(layoutIdEpisode);
                    col.S5_SetMinWidth(Math.Max(col.MinContentWidth,
                        (colspan > 1) ?
                            minimumCellWidth - spanned_width :
                            Math.Max(col.MinContentWidth, minimumCellWidth)));
                    gridIndex += cellBox.ColSpan;
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
            var availableW = this.GetAvailableTableWidth();
            if (widthSum > this.GetAvailableTableWidth())
            {
                //try reduce...
                int cIndex = 0;
                int foundAt;
                int col_count = this.columnCollection.Count;
                while (this.columnCollection.FindFirstReducibleColumnWidth(cIndex, out foundAt))
                {
                    columnCollection[foundAt].AdjustDecrByOne();
                    cIndex = foundAt + 1;
                    if (cIndex >= col_count)
                    {
                        cIndex = 0; //retry again 
                    }
                }
            }

            //--------------------------------------------------

            // if table max width is limited by we need to lower the columns width even if it will result in clipping
            var maxWidth = GetMaxTableWidth();
            if (maxWidth < CssBoxConstConfig.BOX_MAX_RIGHT)
            {
                widthSum = CalculateWidthSum();
                if (maxWidth < widthSum)
                {
                    //Get the minimum and maximum full length of NaN boxes
                    S4S7_CalculateColumnsMinMaxWidthByContent(false);
                    // lower all the columns to the minimum
                    this.columnCollection.LowerAllColumnToMinWidth();
                    // either min for all column is not enought and we need to lower it more resulting in clipping
                    // or we now have extra space so we can give it to columns than need it
                    widthSum = CalculateWidthSum();
                    if (maxWidth < widthSum)
                    {
                        // lower the width of columns starting from the largest one until the max width is satisfied
                        int colCount = this.columnCollection.Count;
                        for (int a = 0; a < 15 && maxWidth < widthSum - 0.1; a++)
                        {
                            // limit iteration so bug won't create infinite loop

                            int nonMaxedColumns = 0;
                            float largeWidth = 0f, secLargeWidth = 0f;
                            for (int i = 0; i < colCount; i++)
                            {
                                var col = this.columnCollection[i];
                                if (col.Width > largeWidth + 0.1)
                                {
                                    secLargeWidth = largeWidth;
                                    largeWidth = col.Width;
                                    nonMaxedColumns = 1;
                                }
                                else if (col.Width > largeWidth - 0.1)
                                {
                                    nonMaxedColumns++;
                                }
                            }

                            float decrease = secLargeWidth > 0 ? largeWidth - secLargeWidth : (widthSum - maxWidth) / colCount;
                            if (decrease * nonMaxedColumns > widthSum - maxWidth)
                            {
                                decrease = (widthSum - maxWidth) / nonMaxedColumns;
                            }
                            for (int i = 0; i < colCount; i++)
                            {
                                var col = this.columnCollection[i];
                                if (col.Width > largeWidth - 0.1)
                                {
                                    col.AddMoreWidthValue(-decrease, ColumnSpecificWidthLevel.Adjust);
                                }
                            }

                            widthSum = CalculateWidthSum();
                        }
                    }
                    else
                    {
                        // spread extra width to columns that didn't reached max width where trying to spread it between all columns

                        int colCount = this.columnCollection.Count;
                        for (int a = 0; a < 15 && maxWidth > widthSum + 0.1; a++)
                        {
                            // limit iteration so bug won't create infinite loop

                            int nonMaxedColumns = 0;
                            for (int i = 0; i < colCount; i++)
                            {
                                var col = columnCollection[i];
                                if (col.Width + 1 < col.MaxContentWidth)
                                {
                                    nonMaxedColumns++;
                                }
                            }
                            if (nonMaxedColumns == 0)
                            {
                                nonMaxedColumns = colCount;
                            }

                            bool hit = false;
                            float minIncrement = (maxWidth - widthSum) / nonMaxedColumns;
                            for (int i = 0; i < colCount; i++)
                            {
                                var col = columnCollection[i];
                                if (col.Width + 0.1 < col.MaxContentWidth)
                                {
                                    minIncrement = Math.Min(minIncrement, col.MaxContentWidth - col.Width);
                                    hit = true;
                                }
                            }

                            for (int i = 0; i < colCount; i++)
                            {
                                var col = columnCollection[i];
                                if (!hit || col.Width + 1 < col.MaxContentWidth)
                                {
                                    col.AddMoreWidthValue(minIncrement, ColumnSpecificWidthLevel.Adjust);
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
            int col_count = this.columnCollection.Count;
            foreach (CssBox row in _allRowBoxes)
            {
                int grid_index = 0;
                foreach (CssBox cellBox in row.GetChildBoxIter())
                {
                    if (grid_index < col_count)
                    {
                        TableColumn col = this.columnCollection[grid_index];
                        if (col.Width < col.MinContentWidth)
                        {
                            int affect_col = grid_index + cellBox.ColSpan - 1;
                            this.columnCollection[affect_col].UseMinWidth();
                            if (grid_index < col_count - 1)
                            {
                                this.columnCollection[grid_index + 1].AddMoreWidthValue(-(col.Width - col.MinContentWidth), ColumnSpecificWidthLevel.Adjust);
                            }
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
        void S8_LayoutCells(LayoutVisitor lay)
        {
            //each cell relative to its owner

            float table_globalX = lay.ContainerBlockGlobalX;
            float table_globalY = lay.ContainerBlockGlobalY;
            float vertical_spacing = GetVerticalSpacing(_tableBox);
            float horizontal_spacing = GetHorizontalSpacing(_tableBox);
            float startx_global = Math.Max(table_globalX + _tableBox.GetClientLeft() + horizontal_spacing, 0);
            float starty_global = Math.Max(table_globalY + _tableBox.GetClientTop() + vertical_spacing, 0);
            float startx_local = startx_global - table_globalX;
            float starty_local = starty_global - table_globalY;
            float curY_local = starty_local;
            float maxRight_local = startx_local;
            float maxBottom_local = 0f;
            int currentRow = 0;
            int col_count = this.columnCollection.Count;
            for (int i = 0; i < _allRowBoxes.Count; i++)
            {
                var row = _allRowBoxes[i];
                float curX_local = startx_local;//reset
                int col_index = 0;
                int grid_index = 0;
                foreach (CssBox cell in row.GetChildBoxIter())
                {
                    if (col_index >= col_count)
                    {
                        break;
                    }
                    else
                    {
                        int colspan = cell.ColSpan;
                        float width = this.columnCollection.GetCellWidth(grid_index, colspan, horizontal_spacing);
                        //HtmlRenderer.Boxes.BoxUtils.ForEachTextRunDeep(cell, trun =>
                        //{
                        //    if (trun.Text.Contains("Cell1"))
                        //    {
                        //        cell.dbugMark = 20;
                        //        return true;
                        //    }
                        //    //else if (trun.Text.Contains("You1"))
                        //    //{
                        //    //    cell.dbugMark = 19;
                        //    //    return true;
                        //    //}
                        //    return false;
                        //});

                        //-----------------------------------------
                        cell.SetLocation(curX_local, curY_local);
                        cell.FreezeWidth = false;
                        cell.SetVisualSize(width, 0);
                        cell.FreezeWidth = true;
                        //-----------------------------------------

                        cell.PerformLayout(lay); //That will automatically set the bottom of the cell
                        //Alter max bottom only if row is cell's row + cell's rowspan - 1
                        CssVerticalCellSpacingBox sb = cell as CssVerticalCellSpacingBox;
                        if (sb != null)
                        {
                            if (sb.EndRow == currentRow)
                            {
                                maxBottom_local = Math.Max(maxBottom_local, sb.ExtendedBox.LocalVisualBottom);
                            }
                        }
                        else if (cell.RowSpan == 1)
                        {
                            maxBottom_local = Math.Max(maxBottom_local, cell.LocalVisualBottom);
                        }

                        maxRight_local = Math.Max(maxRight_local, cell.LocalVisualRight);
                        curX_local = cell.LocalVisualRight + horizontal_spacing;
                        //-------------------------
                        col_index++;
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
                            cell.SetVisualHeight(maxBottom_local - curY_local);
                            ApplyCellVerticalAlignment(cell, starty_local);
                        }
                    }
                    else
                    {
                        if (spacer.EndRow == currentRow)
                        {
                            spacer.ExtendedBox.SetVisualHeight(maxBottom_local - curY_local);
                            ApplyCellVerticalAlignment(spacer.ExtendedBox, starty_local);
                        }
                    }
                }

                curY_local = maxBottom_local + vertical_spacing;
                currentRow++;
            }

            maxRight_local = Math.Max(maxRight_local, _tableBox.ExpectedWidth);
            _tableBox.SetVisualWidth(maxRight_local + horizontal_spacing + _tableBox.ActualBorderRightWidth);
            float globalBottom = Math.Max((maxBottom_local + table_globalY), starty_global) + vertical_spacing + _tableBox.ActualBorderBottomWidth;
            _tableBox.SetVisualHeight(globalBottom - table_globalY);
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
                            columns += box.ColSpan;
                        }
                        break;
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
                        }
                        break;
                    case CssDisplay.TableRow:
                        {
                            count++;
                            columns = Math.Max(columns, box.ChildCount);
                        }
                        break;
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
        static void ApplyCellVerticalAlignment(CssBox cell, float tableBoxOffset)
        {
            float dist = 0f;
            switch (cell.VerticalAlign)
            {
                case CssVerticalAlign.Bottom:
                    dist = cell.GetClientHeight() - cell.CalculateInnerContentHeight();
                    break;
                case CssVerticalAlign.Middle:
                    dist = (cell.GetClientHeight() - cell.CalculateInnerContentHeight()) / 2;
                    break;
                default:
                    return;
            }
            if (dist > CssBoxConstConfig.TABLE_VERT_OFFSET_THESHOLD)
            {
                //more than our threshold
                if (cell.LineBoxCount > 0)
                {
                    var linebox = cell.GetFirstLineBox();
                    while (linebox != null)
                    {
                        linebox.OffsetTop(dist);
                        linebox = linebox.NextLine;
                    }
                }
                else
                {
                    foreach (CssBox b in cell.GetChildBoxIter())
                    {
                        b.OffsetLocalTop(dist);
                    }
                }
            }
        }
        static bool FindVerticalCellSpacingBoxInsertionPoint(CssBox curRow, int cIndex, out CssBox insertAt)
        {
            int colcount = 0;
            int cellCount = curRow.ChildCount;
            var cnode = curRow.GetFirstChild();
            int n = 0;
            while (cnode != null)
            {
                //all cell in this row
                if (colcount == cIndex)
                {
                    //insert new spacing box for table 
                    //at 'colcount' index                        
                    insertAt = cnode;
                    return true;//found
                }
                else
                {
                    colcount++;
                    cIndex -= cnode.RowSpan - 1;
                }

                //----------
                cnode = cnode.GetNextNode();
                n++;
            }
            insertAt = null;
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
        float GetAvailableTableWidth()
        {
            CssLength tblen = _tableBox.Width;
            if (tblen.Number > 0)
            {
                //has specific number
                return CssValueParser.ConvertToPx(_tableBox.Width, this.hostAvaliableWidth, _tableBox);
            }
            else
            {
                return this.hostAvaliableWidth;
                //return 700;
                //return _tableBox.ParentBox.GetClientWidth();
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
                return CssValueParser.ConvertToPx(_tableBox.MaxWidth, _tableBox.ParentBox.GetClientWidth(), _tableBox);
            }
            else
            {
                return CssBoxConstConfig.TABLE_MAX_WIDTH;
            }
        }

        /// <summary>
        /// Calculate the min and max width for each column of the table by the content in all rows.<br/>
        /// the min width possible without clipping content<br/>
        /// the max width the cell content can take without wrapping<br/>
        /// </summary>
        /// <param name="s4_onlyNonCalculated">if to measure only columns that have no calculated width</param>
        /// <param name="minFullWidths">return the min width for each column - the min width possible without clipping content</param>
        /// <param name="maxFullWidths">return the max width for each column - the max width the cell content can take without wrapping</param>
        void S4S7_CalculateColumnsMinMaxWidthByContent(bool s4_onlyNonCalculated)
        {
            int col_count = this.columnCollection.Count;
            if (s4_onlyNonCalculated)
            {
                var ifonts = _tmpIFonts;
                foreach (CssBox row in _allRowBoxes)
                {
                    int gridIndex = 0;
                    foreach (CssBox cellBox in row.GetChildBoxIter())
                    {
                        int cIndex = gridIndex < col_count ? gridIndex : col_count - 1;
                        TableColumn col = this.columnCollection[cIndex];
                        if (!col.HasSpecificWidth)
                        {
                            float minWidth, maxWidth;
                            CalculateMinMaxContentWidths(
                                cellBox,
                                _tableBox,
                                ifonts,
                                out minWidth,
                                out maxWidth);
                            int colSpan = cellBox.ColSpan;
                            if (colSpan == 1)
                            {
                                col.UpdateMinMaxContentWidthIfWider(minWidth, maxWidth);
                            }
                            else
                            {
                                minWidth /= colSpan;
                                maxWidth /= colSpan;
                                for (int n = 0; n < colSpan; n++)
                                {
                                    columnCollection[cIndex + n].UpdateMinMaxContentWidthIfWider(minWidth, maxWidth);
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
                var ifonts = _tmpIFonts;
                foreach (CssBox row in _allRowBoxes)
                {
                    int gridIndex = 0;
                    foreach (CssBox cellBox in row.GetChildBoxIter())
                    {
                        int cIndex = gridIndex < col_count ? gridIndex : col_count - 1;
                        float minContentWidth, maxContentWidth;
                        CalculateMinMaxContentWidths(cellBox, _tableBox, ifonts, out minContentWidth, out maxContentWidth);
                        int colSpan = cellBox.ColSpan;
                        if (colSpan == 1)
                        {
                            columnCollection[cIndex].UpdateMinMaxContentWidthIfWider(minContentWidth, maxContentWidth);
                        }
                        else
                        {
                            minContentWidth /= colSpan;
                            maxContentWidth /= colSpan;
                            for (int n = 0; n < colSpan; n++)
                            {
                                columnCollection[cIndex + n].UpdateMinMaxContentWidthIfWider(minContentWidth, maxContentWidth);
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
        static void CalculateMinMaxContentWidths(CssBox box,
            CssBox cbBox,
            ITextService iFonts,
            out float minWidth, out float maxWidth)
        {
            float min = 0f;
            float maxSum = 0f;
            float paddingSum = 0f;
            float marginSum = 0f;
            if (box.NeedComputedValueEvaluation) { box.ReEvaluateComputedValues(iFonts, cbBox); }
            CalculateMinMaxSumWords(box, cbBox, iFonts, ref min, ref maxSum, ref paddingSum, ref marginSum);
            maxWidth = paddingSum + maxSum;
            minWidth = paddingSum + (min < CssBoxConstConfig.BOX_MAX_RIGHT ? min : 0);
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
        static void CalculateMinMaxSumWords(
            CssBox box,
            CssBox cbBox,
            ITextService iFonts,
            ref float min,
            ref float maxSum,
            ref float paddingSum,
            ref float marginSum)
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

            if (box.HasOnlyRuns)
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
                //if this box has containing property 
                if (box.HasContainingBlockProperty)
                {
                    foreach (CssBox childBox in box.GetChildBoxIter())
                    {
                        if (childBox.NeedComputedValueEvaluation) { childBox.ReEvaluateComputedValues(iFonts, box); }

                        float msum = childBox.ActualMarginLeft + childBox.ActualMarginRight;
                        marginSum += msum;
                        //recursive                        
                        CalculateMinMaxSumWords(childBox, box, iFonts, ref min, ref maxSum, ref paddingSum, ref marginSum);
                        marginSum -= msum;
                    }
                }
                else
                {
                    foreach (CssBox childBox in box.GetChildBoxIter())
                    {
                        if (childBox.NeedComputedValueEvaluation) { childBox.ReEvaluateComputedValues(iFonts, cbBox); }

                        float msum = childBox.ActualMarginLeft + childBox.ActualMarginRight;
                        marginSum += msum;
                        //recursive
                        CalculateMinMaxSumWords(childBox, cbBox, iFonts, ref min, ref maxSum, ref paddingSum, ref marginSum);
                        marginSum -= msum;
                    }
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
            float f = this.columnCollection.CalculateTotalWidth();
            //Take cell-spacing
            f += GetHorizontalSpacing(_tableBox) * (columnCollection.Count + 1);
            //Take table borders
            f += _tableBox.ActualBorderLeftWidth + _tableBox.ActualBorderRightWidth;
            return f;
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
