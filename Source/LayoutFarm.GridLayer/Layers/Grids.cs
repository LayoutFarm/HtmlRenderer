//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LayoutFarm.Presentation.Grids
{
    public enum GridType
    {
        Free,
        UniformWidth,
        UniformHeight,
        UniformCell
    }
    public enum GridNeighborType
    {
        Left,
        Right,
        Up,
        Down
    }

    public class GridColumn
    {

        GridColumnCollection parentColumnCollection;
        int col_index = -1;
        int left = 0;
        int columnWidth = 0;
        List<GridItem> cells = new List<GridItem>();
        int calculatedWidth = 0;
        int desiredHeight = 0;
        int columnFlags = 0;

        const int COLUMN_FIXED_SIZE = 1 << (1 - 1);
        const int COLUMN_HAS_CUSTOM_SIZE = 1 << (2 - 1);

        public GridColumn(int columnWidth)
        {
            this.columnWidth = columnWidth;
        }
        internal void SetParentColumnCollection(GridColumnCollection parentColumnCollection)
        {
            this.parentColumnCollection = parentColumnCollection;
        }
        public int Width
        {
            get { return columnWidth; }
            set { columnWidth = value; }
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
            GridItem removedGridItem = cells[rowid];
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
        internal GridItem CreateGridItemForRow(GridRow row)
        {
            GridItem gridItem = new GridItem(
                this,
                row);
            cells.Add(gridItem);
            return gridItem;
        }
        internal void AddRowRange(IEnumerable<GridRow> rows, int count)
        {

            GridItem[] newGrids = new GridItem[count];
            int i = 0;
            foreach (GridRow row in rows)
            {
                GridItem gridItem = new GridItem(this, row);
                newGrids[i] = gridItem;
                i++;
            }
            cells.AddRange(newGrids);
        }
        internal void InsertAfter(int index, GridRow row)
        {
            GridItem gridItem = new GridItem(
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
            GridItem fromGridItem = cells[fromRow.RowIndex];
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
        public GridItem GetCell(int rowIndex)
        {
            return cells[rowIndex];
        }

        public IEnumerable<RenderElement> GetTopDownVisualElementIter()
        {

            int j = cells.Count;
            for (int i = 0; i < j; ++i)
            {
                RenderElement v = cells[i].contentElement;
                if (v != null)
                {
                    yield return v;
                }
            }
        }
        public IEnumerable<RenderElement> GetTopDownVisualElementReverseIter()
        {

            for (int i = cells.Count - 1; i > -1; --i)
            {
                RenderElement v = cells[i].contentElement;
                if (v != null)
                {
                    yield return v;
                }
            }
        }

        public void ReCalculateColumnSize()
        {
            int j = cells.Count;
            if (j > 0)
            {
                desiredHeight = 0;
                bool firstFoundContentCell = false;
                int local_desired_width = 0;
                for (int i = 0; i < j; i++)
                {
                    GridItem cell = cells[i];

                    cell.ReCalculateContentSize();
                    int cellDesiredWidth = cell.DesiredWidth;
                    desiredHeight += cell.DesiredHeight;

                    if (!firstFoundContentCell)
                    {

                        firstFoundContentCell = (cell.contentElement != null);
                    }
                    if (cellDesiredWidth > local_desired_width)
                    {
                        if (firstFoundContentCell)
                        {

                            if (cell.contentElement != null)
                            {
                                local_desired_width = cellDesiredWidth;

                            }

                        }
                        else
                        {
                            local_desired_width = cellDesiredWidth;
                        }

                    }
                }
                this.calculatedWidth = local_desired_width;
            }
            else
            {
                this.calculatedWidth = this.Width;
            }
        }

        public int DesiredWidth
        {
            get
            {
                return calculatedWidth;
            }
        }

        public int DesiredHeight
        {
            get
            {
                return desiredHeight;
            }
        }
        public GridLayer OwnerGridLayer
        {

            get
            {
                if (parentColumnCollection != null)
                {
                    return parentColumnCollection.OwnerGridLayer;
                }
                else
                {
                    return null;
                }

            }
        }


        internal void SetLeftAndPerformArrange(int left)
        {


            int prevWidth = this.columnWidth;
            if (!this.HasCustomSize)
            {

                this.columnWidth = this.calculatedWidth;
            }
            this.left = left;

            int j = cells.Count;
            int dW = this.columnWidth;
            for (int i = 0; i < j; ++i)
            {
                RenderElement content = cells[i].contentElement;
                if (content != null)
                {
                    //RenderElement.DirectSetVisualElementWidth(content, dW);
                    //if (content.IsVisualContainerBase)
                    //{

                    //    ArtVisualContainerBase vscont = (ArtVisualContainerBase)content;
                    //    vscont.InvalidateContentArrangementFromContainerSizeChanged();
                    //    vscont.TopDownReArrangeContentIfNeed(vinv);
                    //}

                }
            }
        }

#if DEBUG
        public override string ToString()
        {
            return "left=" + left + ",width=" + Width;
        }
#endif
    }

    public class GridRow
    {

        GridRowCollection parentRowCollection;

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
        internal void SetOwnerParentRowCollection(GridRowCollection parentRowCollection)
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

        public GridLayer OwnerGridLayer
        {

            get
            {
                return parentRowCollection.OwnerGridLayer;
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


        public IEnumerable<GridItem> GetGridItemIter()
        {
            return parentRowCollection.GetCellIter(this);
        }


        public GridItem GetGridItem(int columnId)
        {

            return parentRowCollection.OwnerGridLayer.GetCell(RowIndex, columnId);

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

    public class GridItem
    {

        internal GridRow row;
        internal GridColumn column;
        RenderElement content;
        internal GridItem(GridColumn column, GridRow row)
        {
            this.row = row;
            this.column = column;
        }
        public RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, System.Drawing.Point point)
        {
            return null;
        }
        public bool MayHasOverlapChild
        {
            get
            {
                return false;
            }
        }
        public void PerformLayout()
        {
        }

        public void Unlink(RenderElement ve)
        {

            this.content = null;
        }
        public bool ControlChildPosition
        {
            get
            {
                return true;
            }
        }
        public void AdjustParentLocation(ref System.Drawing.Point p)
        {
        }

#if DEBUG
        public string dbugGetLinkInfo()
        {
            return "grid-link";
        }
#endif
        public RenderElement NotifyParentToInvalidate(out bool goToFinalExit
#if DEBUG
, RenderElement ve
#endif
)
        {
            goToFinalExit = false;
            return this.OwnerGridLayer.InvalidateArrangement();

        }
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

        public GridLayer OwnerGridLayer
        {
            get
            {
                return column.OwnerGridLayer;
            }
        }
        public RenderElement OwnerVisualElement
        {
            get
            {
                return column.OwnerGridLayer.ownerVisualElement;
            }
        }


        internal RenderElement contentElement
        {
            get
            {
                return this.content;
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

        public RenderElement ContentElement
        {
            get
            {
                return contentElement;
            }
            set
            {

                this.content = value;
                RenderElement.SetVisualElementAsChildOfOther(value, null);

            }
        }

        internal void DrawToThisPage(CanvasBase canvasPage, InternalRect updateArea)
        {

            if (canvasPage.PushClipArea(this.Width, this.Height, updateArea))
            {
                contentElement.DrawToThisPage(canvasPage, updateArea);
            }

            canvasPage.PopClipArea();

        }
        internal bool PrepareDrawingChain(VisualDrawingChain chain)
        {
            return this.contentElement.PrepareDrawingChain(chain);
        }

        public GridItem GetNeighborGrid(GridNeighborType nb)
        {

            switch (nb)
            {
                case GridNeighborType.Left:
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
                case GridNeighborType.Right:
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
                case GridNeighborType.Up:
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
                case GridNeighborType.Down:
                    {
                        if (row.RowIndex < row.OwnerGridLayer.RowCount - 1)
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


        internal void ReCalculateContentSize()
        {

            if (contentElement != null && !contentElement.HasCalculatedSize)
            {
                contentElement.TopDownReCalculateContentSize();
            }
        }

        internal int DesiredWidth
        {
            get
            {
                if (contentElement != null)
                {
                    int content_desired_width = contentElement.ElementDesiredWidth;
                    if (content_desired_width > column.Width)
                    {
                        return content_desired_width;
                    }
                    else
                    {
                        return column.Width;
                    }

                }
                else
                {
                    return column.Width;

                }
            }
        }
        internal int DesiredHeight
        {
            get
            {
                if (contentElement != null)
                {
                    int content_desired_height = contentElement.ElementDesiredHeight;
                    if (content_desired_height > row.Height)
                    {
                        return content_desired_height;
                    }
                    else
                    {
                        return row.Height;
                    }
                }
                else
                {
                    return row.Height;
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



    class GridColumnCollection
    {


        GridLayer _ownerGridLayer; 
        List<GridColumn> cols = new List<GridColumn>();

        public GridColumnCollection(GridLayer ownerGridLayer)
        {
            this._ownerGridLayer = ownerGridLayer;
        }
        internal GridLayer OwnerGridLayer
        {

            get { return this._ownerGridLayer; }
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

            OwnerGridLayer.ownerVisualElement.InvalidateLayoutAndStartBubbleUp();

#if DEBUG
            //contArrVisitor.dbug_EndLayoutTrace();
#endif

            //--------------------------------------------
        }

        public void Insert(int index, GridColumn coldef)
        {

            cols.Insert(index, coldef);

            int j = cols.Count;


            for (int i = index + 1; i < j; i++)
            {
                cols[i].ColumnIndex = i;
            }


            foreach (GridRow rowdef in OwnerGridLayer.Rows.RowIter)
            {
                coldef.CreateGridItemForRow(rowdef);
            }
            //--------------------------------------------
            //                ContentArrangementVisitor contArrVisitor = new ContentArrangementVisitor(ownerGridLayer);
            //#if DEBUG
            //                //contArrVisitor.dbug_StartLayoutTrace("GridColumnCollection::Insert");
            //                contArrVisitor.dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.GridColumnCollection_Insert);
            //#endif

            OwnerGridLayer.ownerVisualElement.InvalidateLayoutAndStartBubbleUp();


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

            OwnerGridLayer.ownerVisualElement.InvalidateLayoutAndStartBubbleUp();


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

            OwnerGridLayer.ownerVisualElement.InvalidateLayoutAndStartBubbleUp();

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
        public IEnumerable<GridColumn> ColumnIter
        {
            get
            {
                int j = cols.Count;
                for (int i = 0; i < j; ++i)
                {
                    yield return cols[i];
                }
            }
        }
        public IEnumerable<GridColumn> ColumnReverseIter
        {
            get
            {
                for (int i = cols.Count - 1; i > -1; --i)
                {
                    yield return cols[i];
                }
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

    class GridRowCollection
    {


        GridLayer ownerGridLayer;
        List<GridRow> rows = new List<GridRow>();
        internal GridRowCollection(GridLayer ownerGridLayer)
        {
            this.ownerGridLayer = ownerGridLayer;

        }
        public void MoveRowAfter(GridRow fromRow, GridRow toRow)
        {

            int toRowIndex = toRow.RowIndex;
            if (fromRow.RowIndex < toRowIndex)
            {
                toRowIndex -= 1;
            }

            foreach (GridColumn col in ownerGridLayer.gridCols.ColumnIter)
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
        internal IEnumerable<GridItem> GetCellIter(GridRow rowdef)
        {

            int rowId = rowdef.RowIndex;
            if (rowId > -1 && rowId < rows.Count)
            {

                foreach (GridColumn coldef in ownerGridLayer.gridCols.ColumnIter)
                {
                    yield return coldef.GetCell(rowId);
                }
            }
        }


        public IEnumerable<GridRow> RowIter
        {
            get
            {
                foreach (GridRow rowdef in rows)
                {
                    yield return rowdef;
                }
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

                foreach (GridColumn column in ownerGridLayer.gridCols.ColumnIter)
                {

                    column.CreateGridItemForRow(row);
                }
            }
            row.SetOwnerParentRowCollection(this);

            ownerGridLayer.ownerVisualElement.InvalidateLayoutAndStartBubbleUp();


        }

        public void Remove(int rowid)
        {

            GridRow removedRow = rows[rowid];
            foreach (GridColumn coldef in ownerGridLayer.gridCols.ColumnIter)
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


            ownerGridLayer.ownerVisualElement.InvalidateLayoutAndStartBubbleUp();

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

            foreach (GridColumn coldef in ownerGridLayer.gridCols.ColumnIter)
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

            foreach (GridColumn coldef in ownerGridLayer.gridCols.ColumnIter)
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
        public GridLayer OwnerGridLayer
        {
            get
            {
                return ownerGridLayer;
            }
        }
    }

}