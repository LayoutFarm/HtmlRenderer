// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PixelFarm.Drawing;
using LayoutFarm.Text;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{
    public class UIGridBox : UIBox
    {
        CustomRenderBox gridBox;
        GridTable gridTable = new GridTable();
        CellSizeStyle cellSizeStyle;
        GridLayer gridLayer;

        public UIGridBox(int width, int height)
            : base(width, height)
        {

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
                GridRow row = new GridRow(1);//create with default height
                rows.Add(row);
            }
        }
        public override void SetSize(int width, int height)
        {
            //readjust cellsize
            base.SetSize(width, height);


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
            if (this.gridBox == null) { return; }
            var gridLayer = gridBox.Layers.GetLayer(0) as GridLayer;
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
        public void AddUI(UIElement ui, int rowIndex, int colIndex)
        {
            if (rowIndex < gridTable.RowCount && colIndex < gridTable.ColumnCount)
            {
                gridTable.GetCell(rowIndex, colIndex).ContentElement = ui;
                if (this.HasReadyRenderElement)
                {
                    gridLayer.GetCell(rowIndex, colIndex).ContentElement = ui.GetPrimaryRenderElement(gridLayer.Root);
                }
            }
        }
        public CellSizeStyle CellSizeStyle
        {
            get { return this.cellSizeStyle; }
            set { this.cellSizeStyle = value; }
        }
        protected override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.gridBox; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this.gridBox != null; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (gridBox == null)
            {
                var myGridBox = new CustomRenderBox(rootgfx, this.Width, this.Height);
                RenderElement.DirectSetVisualElementLocation(myGridBox, this.Left, this.Top);
                this.gridBox = myGridBox;

                var layers = new VisualLayerCollection();
                gridBox.Layers = layers;
                //create layers
                int nrows = this.gridTable.RowCount;
                int ncols = this.gridTable.ColumnCount;
                //----------------------------------------        
                gridLayer = new GridLayer(gridBox, ncols, nrows, this.cellSizeStyle);
                //add grid content
                for (int c = 0; c < ncols; ++c)
                {
                    for (int r = 0; r < nrows; ++r)
                    {
                        var gridCell = gridTable.GetCell(r, c);
                        var content = gridCell.ContentElement as UIElement;
                        if (content != null)
                        {
                            RenderElement re = content.GetPrimaryRenderElement(rootgfx);
                            gridLayer.GetCell(r, c).ContentElement = re;
                        }
                    }
                }

                layers.AddLayer(gridLayer);
            }
            return gridBox;
        }
    }




}