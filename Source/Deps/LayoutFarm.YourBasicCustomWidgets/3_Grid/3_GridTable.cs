//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.UI
{
    public enum CellSizeStyle
    {
        ColumnAndRow,//depends on owner column and row
        UniformWidth,
        UniformHeight,
        UniformCell
    }

    enum CellNeighbor
    {
        Left,
        Right,
        Up,
        Down
    }

    class GridColumn
    {
        GridTable.GridColumnCollection parentColumnCollection;
        int col_index = -1;
        int left = 0;
        int columnWidth = 0;
        List<GridCell> cells = new List<GridCell>();
        int calculatedWidth = 0;
        int desiredHeight = 0;
        int columnFlags = 0;
        const int COLUMN_FIXED_SIZE = 1 << (1 - 1);
        const int COLUMN_HAS_CUSTOM_SIZE = 1 << (2 - 1);
        public GridColumn(int columnWidth)
        {
            this.columnWidth = columnWidth;
        }
        internal void SetParentColumnCollection(GridTable.GridColumnCollection parentColumnCollection)
        {
            this.parentColumnCollection = parentColumnCollection;
        }
        public int Width
        {
            get { return columnWidth; }
            set { columnWidth = value; }
        }
        public int CalculatedWidth
        {
            get { return this.calculatedWidth; }
            set { this.calculatedWidth = value; }
        }
        public int ColumnIndex
        {
            get { return col_index; }
            internal set { this.col_index = value; }
        }
        public int Right
        {
            get { return columnWidth + left; }
        }
        public bool HasCustomSize
        {
            get
            {
                return (columnFlags & COLUMN_HAS_CUSTOM_SIZE) != 0;
            }
            set
            {
                if (value)
                {
                    columnFlags |= COLUMN_HAS_CUSTOM_SIZE;
                }
                else
                {
                    columnFlags &= ~COLUMN_HAS_CUSTOM_SIZE;
                }
            }
        }

        public int Left
        {
            get { return left; }
            set
            {
                int leftdiff = left - value;
                left = value;
            }
        }

        internal void RemoveRow(GridRow row)
        {
            if (cells.Count == 0)
            {
                return;
            }
            int rowid = row.RowIndex;
            GridCell removedGridItem = cells[rowid];
            cells.RemoveAt(rowid);
        }

        internal void ClearAllRows()
        {
            if (cells.Count == 0)
            {
                return;
            }
            cells.Clear();
        }
        internal GridCell CreateGridItemForRow(GridRow row)
        {
            GridCell gridItem = new GridCell(
                this,
                row);
            cells.Add(gridItem);
            return gridItem;
        }
        internal void AddRowRange(IEnumerable<GridRow> rows, int count)
        {
            GridCell[] newGrids = new GridCell[count];
            int i = 0;
            foreach (GridRow row in rows)
            {
                GridCell gridItem = new GridCell(this, row);
                newGrids[i] = gridItem;
                i++;
            }
            cells.AddRange(newGrids);
        }
        internal void InsertAfter(int index, GridRow row)
        {
            GridCell gridItem = new GridCell(
                this, row);
            cells.Insert(index + 1, gridItem);
        }

        internal void MoveRowAfter(GridRow fromRow, GridRow toRow)
        {
            int destRowIndex = toRow.RowIndex;
            if (destRowIndex > fromRow.RowIndex)
            {
                destRowIndex -= 1;
            }
            GridCell fromGridItem = cells[fromRow.RowIndex];
            cells.RemoveAt(fromRow.RowIndex);
            cells.Insert(destRowIndex, fromGridItem);
        }
        public GridColumn PrevColumn
        {
            get
            {
                if (ColumnIndex > 0)
                {
                    return parentColumnCollection[ColumnIndex - 1];
                }
                else
                {
                    return null;
                }
            }
        }
        public GridColumn NextColumn
        {
            get
            {
                if (ColumnIndex < parentColumnCollection.Count - 1)
                {
                    return parentColumnCollection[ColumnIndex + 1];
                }
                else
                {
                    return null;
                }
            }
        }
        public GridCell GetCell(int rowIndex)
        {
            return cells[rowIndex];
        }
        public int CellCount
        {
            get { return this.cells.Count; }
        }
        public IEnumerable<GridCell> GetTopDownGridCellIter()
        {
            int j = cells.Count;
            for (int i = 0; i < j; ++i)
            {
                yield return cells[i];
            }
        }

        public int DesiredWidth
        {
            get
            {
                return calculatedWidth;
            }
            set
            {
                this.calculatedWidth = value;
            }
        }

        public int DesiredHeight
        {
            get
            {
                return desiredHeight;
            }
            set
            {
                this.desiredHeight = value;
            }
        }

#if DEBUG
        public override string ToString()
        {
            return "left=" + left + ",width=" + Width;
        }
#endif
    }

    class GridRow
    {
        GridTable.GridRowCollection parentRowCollection;
        int row_Index = -1;
        int top = 0;
        int rowHeight = 0;
        int desiredHeight;
        int rowFlags = 0;
        const int FIXED_HEIGHT = 1 << (1 - 1);
        const int HAS_CALCULATE_HEIGHT = 1 << (2 - 1);
        public GridRow(int initRowHeight)
        {
            this.rowHeight = initRowHeight;
        }

        public int RowIndex
        {
            get { return this.row_Index; }
            internal set { this.row_Index = value; }
        }
        internal void SetOwnerParentRowCollection(GridTable.GridRowCollection parentRowCollection)
        {
            this.parentRowCollection = parentRowCollection;
        }
        public int DesiredHeight
        {
            get
            {
                if ((rowFlags & HAS_CALCULATE_HEIGHT) != 0)
                {
                    return desiredHeight;
                }
                else
                {
                    return rowHeight;
                }
            }
        }
        public bool IsBoundToGrid
        {
            get
            {
                return parentRowCollection != null;
            }
        }


        public int OwnerGridRowCount
        {
            get
            {
                return this.parentRowCollection.GridTable.RowCount;
            }
        }
        public int Height
        {
            get
            {
                return rowHeight;
            }
            set
            {
                rowHeight = value;
            }
        }
        public int Top
        {
            get
            {
                return top;
            }
            set
            {
                top = value;
            }
        }
        public int Bottom
        {
            get
            {
                return top + rowHeight;
            }
        }
        public int RowId
        {
            get
            {
                return RowIndex;
            }
        }
        public GridRow PrevRow
        {
            get
            {
                if (RowIndex > 0)
                {
                    return parentRowCollection[RowIndex - 1];
                }
                else
                {
                    return null;
                }
            }
        }
        public GridRow NextRow
        {
            get
            {
                if (RowIndex < parentRowCollection.Count - 1)
                {
                    return parentRowCollection[RowIndex + 1];
                }
                else
                {
                    return null;
                }
            }
        }
        public void AdjustBottom(int bottomPos)
        {
            //2010-09-04
            //int i = 0;
            //foreach (ArtUIGridItem affectedBox in parentRowCollection.GetCellIter(this))
            //{
            //    if (affectedBox != null)
            //    {
            //        Rectangle oldRect = affectedBox.Rect;
            //        int ydiff = bottomPos - oldRect.Bottom;
            //        if (ydiff != 0) 
            //        {
            //           
            //            affectedBox.SetRectBound(oldRect.X, oldRect.Y, oldRect.Width, oldRect.Height += ydiff);
            //            affectedBox.ReArrangeContent();  
            //            //affectedBox.PerformLayout(); 
            //        }
            //        if (i == 0)
            //        {
            //            rowHeight = affectedBox.Height;
            //        }
            //    }
            //    i++;
            //}
        }
        public void AdjustTop(int topPos)
        {
            //ArtUIGridItem[] children = parentRowCollection.GetGridItems(this);//neighbors
            if ((rowFlags & FIXED_HEIGHT) != 0)
            {
                int i = 0;
                //foreach (ArtUIGridItem gridItem in parentRowCollection.GetCellIter(this))
                //{

                //    Rectangle oldRect = gridItem.Rect;
                //    int oldY = oldRect.Y;
                //    int ydiff = oldY - topPos;
                //    if (ydiff != 0)
                //    {
                //        gridItem.SetRectBound(oldRect.X, topPos, oldRect.Width, oldRect.Height += ydiff);
                //        
                //        gridItem.ReCalculateContentSize(); 
                //        gridItem.ReArrangeContent(); 
                //        //gridItem.PerformLayout();  
                //    }
                //    if (i == 0)
                //    {
                //        rowHeight = gridItem.Height;
                //        top = gridItem.Y;
                //    }
                //    i++;
                //}
            }
            else
            {
                int i = 0;
                //foreach (ArtUIGridItem gridItem in parentRowCollection.GetCellIter(this))
                //{
                //    Point oldPos = gridItem.Location;
                //    gridItem.SetLocation(oldPos.X, topPos);
                //    if (i == 0)
                //    {
                //        rowHeight = gridItem.Height;
                //        top = gridItem.Y;
                //    }
                //    i++;
                //}
            }

            //rowHeight = children[0].Height;
            //top = children[0].Y;
        }
        public void SetTopAndHeight(int top, int height)
        {
            this.top = top;
            rowHeight = height;
            //foreach (ArtUIGridItem cell in parentRowCollection.GetCellIter(this))
            //{
            //    Rectangle oldrect = cell.Rect;
            //    oldrect.Y = top;
            //    oldrect.Height = height;
            //    cell.SetRectBound(oldrect);
            //}
        }
        public void CalculateRowHeight()
        {
            this.desiredHeight = this.rowHeight;
            rowFlags |= HAS_CALCULATE_HEIGHT;
        }
        internal void AcceptDesiredHeight(int currentTop)
        {
            if ((rowFlags & HAS_CALCULATE_HEIGHT) != 0)
            {
                this.rowHeight = desiredHeight;
            }
            else
            {
            }
            this.top = currentTop;
        }
#if DEBUG
        public override string ToString()
        {
            return "top=" + top + ",height=" + Height;
        }
#endif

    }

    class GridCell
    {
        internal GridRow row;
        internal GridColumn column;
        object content;
        internal GridCell(GridColumn column, GridRow row)
        {
            this.row = row;
            this.column = column;
        }
        public bool ControlChildPosition
        {
            get
            {
                return true;
            }
        }

#if DEBUG
        public string dbugGetLinkInfo()
        {
            return "grid-link";
        }
#endif

        public GridRow Row
        {
            get
            {
                return row;
            }
        }
        public GridColumn Column
        {
            get
            {
                return column;
            }
        }
        public Rectangle Rect
        {
            get
            {
                return new Rectangle(column.Left, row.Top, column.Width, row.Height);
            }
        }

        public bool HasContent
        {
            get { return this.content != null; }
        }
        public object ContentElement
        {
            get
            {
                return this.content;
            }
            set
            {
                //set content to that cell
                this.content = value;
            }
        }
        public int X
        {
            get
            {
                return column.Left;
            }
        }
        public int Y
        {
            get
            {
                return row.Top;
            }
        }
        public int Right
        {
            get
            {
                return column.Right;
            }
        }
        public int Bottom
        {
            get
            {
                return row.Bottom;
            }
        }
        public int Width
        {
            get
            {
                return column.Width;
            }
        }
        public int Height
        {
            get
            {
                return row.Height;
            }
        }

        public Point RightBottomCorner
        {
            get
            {
                return new Point(column.Right, row.Bottom);
            }
        }
        public Point RightTopCorner
        {
            get
            {
                return new Point(column.Right, row.Top);
            }
        }

        public GridCell GetNeighborGrid(CellNeighbor nb)
        {
            switch (nb)
            {
                case CellNeighbor.Left:
                    {
                        GridColumn prevColumn = column.PrevColumn;
                        if (prevColumn != null)
                        {
                            return prevColumn.GetCell(row.RowIndex);
                        }
                        else
                        {
                            return null;
                        }
                    }
                case CellNeighbor.Right:
                    {
                        GridColumn nextColumn = column.NextColumn;
                        if (nextColumn != null)
                        {
                            return nextColumn.GetCell(row.RowIndex);
                        }
                        else
                        {
                            return null;
                        }
                    }
                case CellNeighbor.Up:
                    {
                        if (row.RowIndex > 0)
                        {
                            return column.GetCell(row.RowIndex - 1);
                        }
                        else
                        {
                            return null;
                        }
                    }
                case CellNeighbor.Down:
                    {
                        if (row.RowIndex < row.OwnerGridRowCount - 1)
                        {
                            return column.GetCell(row.RowIndex + 1);
                        }
                        else
                        {
                            return null;
                        }
                    }
                default:
                    {
#if DEBUG
                        throw new NotSupportedException();
#else
                            return null;
#endif
                    }
            }
        }

#if DEBUG
        public override string ToString()
        {
            return column.ColumnIndex.ToString() + "," + row.RowIndex.ToString() + " " + base.ToString();
        }
#endif

    }


    partial class GridTable
    {
        GridColumnCollection cols;
        GridRowCollection rows;
        public GridTable()
        {
            this.cols = new GridColumnCollection(this);
            this.rows = new GridRowCollection(this);
        }
        public void Clear()
        {
            this.cols.Clear();
            this.rows.ClearAll();
        }

        public IEnumerable<GridColumn> GetColumnIter()
        {
            return this.cols.GetColumnIter();
        }
        public IEnumerable<GridRow> GetRowIter()
        {
            return this.rows.GetRowIter();
        }
        public GridCell GetCell(int rowId, int columnId)
        {
            return this.cols[columnId].GetCell(rowId);
        }
        public int RowCount
        {
            get
            {
                return this.rows.Count;
            }
        }
        public int ColumnCount
        {
            get
            {
                return this.cols.Count;
            }
        }
        public GridColumnCollection Columns
        {
            get { return this.cols; }
        }
        public GridRowCollection Rows
        {
            get { return this.rows; }
        }
    }

    partial class GridTable
    {
        public class GridColumnCollection
        {
            GridTable table;
            List<GridColumn> cols = new List<GridColumn>();
            internal GridColumnCollection(GridTable table)
            {
                this.table = table;
            }
            public void Clear()
            {
                cols.Clear();
            }

            public void Add(GridColumn newColumnDef)
            {
                int j = cols.Count;
                if (j == 0)
                {
                    newColumnDef.Left = 0;
                    newColumnDef.ColumnIndex = 0;
                }
                else
                {
                    newColumnDef.Left = cols[j - 1].Right + 1;
                    newColumnDef.ColumnIndex = j;
                }
                newColumnDef.SetParentColumnCollection(this);
                cols.Add(newColumnDef);
#if DEBUG
                //contArrVisitor.dbug_StartLayoutTrace("GridCollection::Add(GridColumn)");
#endif

                InvalidateGraphicAndStartBubbleUp();
#if DEBUG
                //contArrVisitor.dbug_EndLayoutTrace();
#endif

                //--------------------------------------------
            }
            void InvalidateGraphicAndStartBubbleUp()
            {
            }
            public void Insert(int index, GridColumn coldef)
            {
                cols.Insert(index, coldef);
                int j = cols.Count;
                for (int i = index + 1; i < j; i++)
                {
                    cols[i].ColumnIndex = i;
                }


                foreach (GridRow rowdef in this.table.GetRowIter())
                {
                    coldef.CreateGridItemForRow(rowdef);
                }
                //--------------------------------------------
                //                ContentArrangementVisitor contArrVisitor = new ContentArrangementVisitor(ownerGridLayer);
                //#if DEBUG
                //                //contArrVisitor.dbug_StartLayoutTrace("GridColumnCollection::Insert");
                //                contArrVisitor.dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.GridColumnCollection_Insert);
                //#endif

                InvalidateGraphicAndStartBubbleUp();
                //OwnerGridLayer.OwnerInvalidateGraphicAndStartBubbleUp();


                //#if DEBUG
                //                contArrVisitor.dbug_EndLayoutTrace();
                //#endif
                //--------------------------------------------
            }

            public void Remove(int columnid)
            {
                GridColumn col = cols[columnid];
                int removedColumnWidth = col.Width;
                col.ClearAllRows();
                int j = cols.Count;
                for (int i = columnid + 1; i < j; i++)
                {
                    col = cols[i];
                    col.ColumnIndex -= 1;
                    col.Left -= removedColumnWidth;
                }
                cols.RemoveAt(columnid);
                OwnerInvalidateGraphicAndStartBubbleUp();
            }
            void OwnerInvalidateGraphicAndStartBubbleUp()
            {
            }
            public GridColumn First
            {
                get
                {
                    if (cols.Count > 0)
                    {
                        return cols[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            public GridColumn Last
            {
                get
                {
                    if (cols.Count > 0)
                    {
                        return cols[cols.Count - 1];
                    }
                    else
                    {
                        return null;
                    }
                }
            }


            public void MoveColumnAfter(GridColumn tobeMovedColumn, GridColumn afterColumn)
            {
                //---------------------------------------------------------

                int toTargetColumnIndex = afterColumn.ColumnIndex;
                if (tobeMovedColumn.ColumnIndex < toTargetColumnIndex)
                {
                    toTargetColumnIndex -= 1;
                }
                cols.RemoveAt(tobeMovedColumn.ColumnIndex);
                cols.Insert(afterColumn.ColumnIndex, tobeMovedColumn);
                UpdateColumnIndex(Math.Min(afterColumn.ColumnIndex, toTargetColumnIndex));
                this.OwnerInvalidateGraphicAndStartBubbleUp();
            }

            void UpdateColumnIndex(int startIndex)
            {
                int j = cols.Count;
                if (startIndex < j)
                {
                    for (int i = startIndex; i < j; i++)
                    {
                        cols[i].ColumnIndex = i;
                    }
                }
            }

            public GridColumn this[int index]
            {
                get
                {
                    return cols[index];
                }
            }
            public IEnumerable<GridColumn> GetColumnIter()
            {
                int j = cols.Count;
                for (int i = 0; i < j; ++i)
                {
                    yield return cols[i];
                }
            }
            public IEnumerable<GridColumn> GetColumnReverseIter()
            {
                for (int i = cols.Count - 1; i > -1; --i)
                {
                    yield return cols[i];
                }
            }

            public GridColumn GetColumnAtPosition(int x)
            {
                foreach (GridColumn coldef in cols)
                {
                    if (coldef.Right >= x)
                    {
                        return coldef;
                    }
                }
                return null;
            }
            public int Count
            {
                get
                {
                    return cols.Count;
                }
            }
        }

        public class GridRowCollection
        {
            GridTable table;
            List<GridRow> rows = new List<GridRow>();
            internal GridRowCollection(GridTable table)
            {
                this.table = table;
            }
            public void MoveRowAfter(GridRow fromRow, GridRow toRow)
            {
                int toRowIndex = toRow.RowIndex;
                if (fromRow.RowIndex < toRowIndex)
                {
                    toRowIndex -= 1;
                }

                foreach (GridColumn col in table.GetColumnIter())
                {
                    col.MoveRowAfter(fromRow, toRow);
                }

                rows.RemoveAt(fromRow.RowIndex);
                rows.Insert(toRowIndex, fromRow);
                UpdateRowIndex(fromRow, toRow);
            }

            void UpdateRowIndex(GridRow row1, GridRow row2)
            {
                if (row1.RowIndex < row2.RowIndex)
                {
                    int stopRowIndex = row2.RowIndex;
                    for (int i = row1.RowIndex; i <= stopRowIndex; i++)
                    {
                        rows[i].RowIndex = i;
                    }
                }
                else
                {
                    int stopRowIndex = row1.RowIndex;
                    for (int i = row2.RowIndex; i <= stopRowIndex; i++)
                    {
                        rows[i].RowIndex = i;
                    }
                }
            }

            public GridRow First
            {
                get
                {
                    if (rows.Count > 0)
                    {
                        return rows[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            public GridRow Last
            {
                get
                {
                    if (rows.Count > 0)
                    {
                        return rows[rows.Count - 1];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            internal IEnumerable<GridCell> GetCellIter(GridRow rowdef)
            {
                int rowId = rowdef.RowIndex;
                if (rowId > -1 && rowId < rows.Count)
                {
                    foreach (GridColumn coldef in table.GetColumnIter())
                    {
                        yield return coldef.GetCell(rowId);
                    }
                }
            }


            public IEnumerable<GridRow> GetRowIter()
            {
                foreach (GridRow rowdef in rows)
                {
                    yield return rowdef;
                }
            }
            public GridRow this[int index]
            {
                get
                {
                    return rows[index];
                }
            }
            public int Count
            {
                get
                {
                    return rows.Count;
                }
            }

            public void Add(GridRow row)
            {
                int lastcount = rows.Count;
                row.RowIndex = lastcount;
                if (lastcount > 0)
                {
                    row.Top = rows[lastcount - 1].Bottom;
                }

                rows.Add(row);
                if (!row.IsBoundToGrid)
                {
                    foreach (GridColumn column in table.GetColumnIter())
                    {
                        column.CreateGridItemForRow(row);
                    }
                }
                row.SetOwnerParentRowCollection(this);
                OwnerInvalidateGraphicAndStartBubbleUp();
            }
            void OwnerInvalidateGraphicAndStartBubbleUp()
            {
            }
            public void Remove(int rowid)
            {
                GridRow removedRow = rows[rowid];
                foreach (GridColumn coldef in table.GetColumnIter())
                {
                    coldef.RemoveRow(removedRow);//
                }

                rows.RemoveAt(rowid);
                int j = rows.Count;
                int removeRowHeight = removedRow.Height;
                for (int i = rowid; i < j; i++)
                {
                    GridRow r = rows[i];
                    r.RowIndex--;
                    r.Top -= removeRowHeight;
                }


                this.OwnerInvalidateGraphicAndStartBubbleUp();
            }
            public GridRow AddRow(int rowHeight)
            {
                return AddRowAfter(rows.Count - 1, rowHeight);
            }

            public GridRow AddRowAfter(int afterRowId, int rowHeight)
            {
                int newrowId = afterRowId + 1;
                GridRow newGridRow = null;
                if (afterRowId == -1)
                {
                    newGridRow = new GridRow(rowHeight);
                    newGridRow.Top = 0;
                    Add(newGridRow);
                }
                else
                {
                    GridRow refRowDefinition = rows[afterRowId];
                    newGridRow = new GridRow(rowHeight);
                    newGridRow.Top = refRowDefinition.Top + refRowDefinition.Height;
                    InsertAfter(afterRowId, newGridRow);
                }

                return newGridRow;
            }
            public void ClearAll()
            {
                foreach (GridColumn coldef in table.GetColumnIter())
                {
                    coldef.ClearAllRows();
                }
                rows.Clear();
            }

            internal void InsertAfter(int afterRowId, GridRow row)
            {
                int newRowHeight = row.Height;
                row.SetOwnerParentRowCollection(this);
                row.RowIndex = afterRowId + 1;
                rows.Insert(afterRowId + 1, row);
                foreach (GridColumn coldef in table.GetColumnIter())
                {
                    coldef.InsertAfter(afterRowId, row);
                }

                int j = rows.Count;
                for (int i = afterRowId + 2; i < j; i++)
                {
                    GridRow r = rows[i];
                    r.RowIndex = i;
                }
            }
            public void InsertAfter(GridRow afterThisRow, GridRow row)
            {
                InsertAfter(afterThisRow.RowIndex, row);
            }

            public GridRow GetRowAtPos(int y)
            {
                int j = rows.Count;
                for (int i = 0; i < j; ++i)
                {
                    if (rows[i].Bottom >= y)
                    {
                        return rows[i];
                    }
                }
                return null;
            }
            public GridTable GridTable
            {
                get
                {
                    return this.table;
                }
            }
        }
    }
}