// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PixelFarm.Drawing;



namespace LayoutFarm.HtmlBoxes
{

    //    class GridBoxRenderElement
    //    {
    //        GridLayer gridLayer;
    //        public void BuildGrid(int nCols, int nRows, CellSizeStyle cellSizeStyle)
    //        {
    //            this.gridLayer = new GridLayer(this, nCols, nRows, cellSizeStyle);
    //        }
    //        public GridLayer GridLayer
    //        {
    //            get { return this.gridLayer; }
    //        }
    //        public void SetContent(int r, int c, CssBox box)
    //        {
    //            gridLayer.GetCell(r, c).ContentElement = box;
    //        }
    //        protected void DrawContent(Canvas canvas, Rectangle updateArea)
    //        {

    //#if DEBUG
    //            if (this.dbugBreak)
    //            {

    //            }
    //#endif
    //            //sample bg   
    //            //canvas.FillRectangle(BackColor, updateArea.Left, updateArea.Top, updateArea.Width, updateArea.Height);
    //            canvas.FillRectangle(BackColor, 0, 0, this.Width, this.Height);

    //            gridLayer.DrawChildContent(canvas, updateArea);

    //            if (this.HasDefaultLayer)
    //            {
    //                this.DrawDefaultLayer(canvas, ref updateArea);
    //            }
    //#if DEBUG
    //            //canvas.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
    //            //    new Rectangle(0, 0, this.Width, this.Height));

    //            //canvas.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
    //            //   new Rectangle(updateArea.Left, updateArea.Top, updateArea.Width, updateArea.Height));
    //#endif
    //        }
    //    }

    public class GridBox
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

        //public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        //{
        //    if (gridBox == null)
        //    {
        //        var myGridBox = new GridBoxRenderElement(rootgfx, this.Width, this.Height);
        //        myGridBox.SetLocation(this.Left, this.Top);
        //        this.SetPrimaryRenderElement(myGridBox);
        //        this.gridBox = myGridBox;
        //        //create layers
        //        int nrows = this.gridTable.RowCount;
        //        int ncols = this.gridTable.ColumnCount;
        //        //----------------------------------------        

        //        myGridBox.BuildGrid(ncols, nrows, this.CellSizeStyle);
        //        //add grid content
        //        for (int c = 0; c < ncols; ++c)
        //        {
        //            for (int r = 0; r < nrows; ++r)
        //            {
        //                var gridCell = gridTable.GetCell(r, c);
        //                var content = gridCell.ContentElement as UIElement;
        //                if (content != null)
        //                {
        //                    myGridBox.SetContent(r, c, content);
        //                }
        //            }
        //        }
        //    }
        //    return gridBox;
        //}


    }




}