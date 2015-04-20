// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PixelFarm.Drawing;



namespace LayoutFarm.HtmlBoxes
{


    class GridBox
    {

        GridLayer gridLayer;
        GridTable gridTable = new GridTable();
        CellSizeStyle cellSizeStyle;
        int gridBoxWidth;
        int gridBoxHeight;

        public GridBox(int width, int height)
        {
            //has special grid layer

        }
        public void BuildGrid(int ncols, int nrows, CellSizeStyle cellSizeStyle)
        {
            this.cellSizeStyle = cellSizeStyle;
            //1. create cols
            var cols = gridTable.Columns;
            for (int n = 0; n < ncols; ++n)
            {
                //create with defatul width
                GridColumn col = new GridColumn(1);
                cols.Add(col);
            }
            //2. create rows
            var rows = gridTable.Rows;
            for (int n = 0; n < nrows; ++n)
            {

                rows.Add(new GridRow(1));
            }
        }
        public void SetSize(int width, int height)
        {
            //readjust cellsize
            this.gridBoxWidth = width;
            this.gridBoxHeight = height;

            //----------------------------------
            var cols = gridTable.Columns;
            int ncols = cols.Count;

            //each col width
            int eachColWidth = width / ncols;
            int colLeft = 0;
            for (int n = 0; n < ncols; ++n)
            {
                //create with defatul width
                var col = cols[n];
                col.Width = eachColWidth;
                col.Left = colLeft;
                colLeft += eachColWidth;
            }

            var rows = gridTable.Rows;
            int nrows = rows.Count;
            int eachRowHeight = height / nrows;
            int rowTop = 0;
            for (int n = 0; n < nrows; ++n)
            {
                var row = rows[n];
                row.Height = eachRowHeight;
                row.Top = rowTop; ;
                rowTop += eachRowHeight;
            }
            //----------------------------------
            //if (this.gridBox == null) { return; }
            //var gridLayer = gridBox.GridLayer;
            colLeft = 0;
            for (int n = 0; n < ncols; ++n)
            {
                var col = gridLayer.GetColumn(n);
                col.Width = eachColWidth;
                col.Left = colLeft;
                colLeft += eachColWidth;
            }
            rowTop = 0;
            for (int n = 0; n < nrows; ++n)
            {
                var row = gridLayer.GetRow(n);
                row.Height = eachRowHeight;
                row.Top = rowTop; ;
                rowTop += eachRowHeight;
            }
        }
        public void AddUI(object ui, int rowIndex, int colIndex)
        {
            //if (rowIndex < gridTable.RowCount && colIndex < gridTable.ColumnCount)
            //{
            //    gridTable.GetCell(rowIndex, colIndex).ContentElement = ui;
            //    if (this.gridBox != null)
            //    {
            //        gridBox.SetContent(rowIndex, colIndex, ui.GetPrimaryRenderElement(gridBox.Root));
            //    }
            //}
        }
        public CellSizeStyle CellSizeStyle
        {
            get { return this.cellSizeStyle; }
            set { this.cellSizeStyle = value; }
        }


    }




}