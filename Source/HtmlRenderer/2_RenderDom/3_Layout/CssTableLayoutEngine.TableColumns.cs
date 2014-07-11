//BSD 2014, WinterDev
//ArthurHub 
 

namespace HtmlRenderer.RenderDom
{
    partial class CssTableLayoutEngine
    {
        class TableColumn
        {
            int index;
            bool hasSpecificWidth;
            float actualWidth;
            float minWidth;
            float maxWidth;

            public TableColumn(int index)
            {
                this.index = index;
            }
            public float Width
            {
                get { return this.actualWidth; }
                set
                {
                    hasSpecificWidth = true;
                    this.actualWidth = value;
                }
            }
            public bool HasSpecificWidth
            {
                get { return this.hasSpecificWidth; }
            }
            public void UpdateIfWider(float newWidth)
            {
                if (newWidth >= this.actualWidth)
                {
                    this.Width = newWidth;
                }
            }
            public float MinWidth
            {
                get { return this.minWidth; }
                set { this.minWidth = value; }
            }

            public float MaxWidth
            {
                get { return this.maxWidth; }
                set { this.maxWidth = value; }
            }
            public void UpdateMinMaxWidthIfWider(float newMinWidth, float newMaxWidth)
            {
                if (newMinWidth > this.minWidth)
                {
                    this.minWidth = newMinWidth;
                }

                if (newMaxWidth > this.maxWidth)
                {
                    this.maxWidth = newMaxWidth;
                }
            }
            public bool TouchLowerLimit
            {
                get { return this.actualWidth <= this.minWidth; }
            }
            public bool TouchUpperLimit
            {
                get { return this.actualWidth >= this.maxWidth; }
            }

            public void AddMoreWidthValue(float offset)
            {
                this.actualWidth += offset;
            }
            public void UserMinWidth()
            {
                this.actualWidth = this.minWidth;
            }
        }
        class TableColumnCollection
        {
            TableColumn[] columns;
            public TableColumnCollection(int columnCount)
            {
                this.columns = new TableColumn[columnCount];
                for (int i = columnCount - 1; i >= 0; --i)
                {
                    this.columns[i] = new TableColumn(i);
                }
            }
            public void SetColumnWidth(int colIndex, float width)
            {
                columns[colIndex].Width = width;
            }
            public TableColumn this[int index]
            {
                get
                {
                    return this.columns[index];
                }
            }
            public void CountUnspecificWidthColumnAndOccupiedSpace(out int numOfUnspecificColWidth, out float occupiedSpace)
            {
                numOfUnspecificColWidth = 0;
                occupiedSpace = 0f;
                for (int i = columns.Length - 1; i >= 0; --i)
                {
                    var col = columns[i];
                    if (!col.HasSpecificWidth)
                    {
                        numOfUnspecificColWidth++;
                    }
                    else
                    {
                        occupiedSpace += col.Width;
                    }
                }
            }

            public bool FindFirstReducibleColumnWidth(int startAtIndex, out int foundAtIndex)
            {

                int col_count = columns.Length;
                for (int i = startAtIndex; i < col_count; ++i)
                {
                    if (!columns[i].TouchLowerLimit)
                    {
                        foundAtIndex = i;
                        return true;
                    }

                }
                foundAtIndex = -1;
                return false;
            }
            public int Count
            {
                get
                {
                    return this.columns.Length;
                }
            }
            public float CalculateTotalWidth()
            {
                float total = 0;
                for (int i = columns.Length - 1; i >= 0; --i)
                {
                    total += columns[i].Width;
                    //float t = columns[i];
                    //if (float.IsNaN(t))
                    //{
                    //    throw new Exception("CssTable Algorithm error: There's a NaN in column widths");
                    //}
                    //else
                    //{
                    //    f += t;
                    //}
                }
                return total;

            }

            public void AddMoreWidthToColumns(bool onlyNonspeicificWidth, float value)
            {
                if (onlyNonspeicificWidth)
                {
                    for (int i = columns.Length - 1; i >= 0; --i)
                    {
                        var col = columns[i];
                        if (!col.HasSpecificWidth)
                        {
                            col.AddMoreWidthValue(value);
                        }
                    }
                }
                else
                {
                    for (int i = columns.Length - 1; i >= 0; --i)
                    {
                        columns[i].AddMoreWidthValue(value);
                    }
                }
            }
            public void LowerAllColumnToMinWidth()
            {
                for (int i = columns.Length - 1; i >= 0; --i)
                {
                    columns[i].UserMinWidth();
                }
            }
            /// <summary>
            /// Gets the spanned width of a cell (With of all columns it spans minus one).
            /// </summary>
            public float GetSpannedMinWidth(int rowCellCount, int realcolindex, int colspan)
            {
                float w = 0f;
                int min_widths_len = this.columns.Length;
                for (int i = realcolindex; (i < min_widths_len) && (i < rowCellCount || i < realcolindex + colspan - 1); ++i)
                {
                    w += this.columns[i].MinWidth;
                }
                return w;
            }
            /// <summary>
            /// Gets the cells width, taking colspan and being in the specified column
            /// </summary>
            /// <param name="cIndex"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public float GetCellWidth(int cIndex, int colspan, float horizontal_spacing)
            {
                if (colspan == 1)
                {
                    return this.columns[cIndex].Width;
                }
                else
                {
                    float sum = 0f;
                    int col_count = this.columns.Length;
                    for (int i = cIndex; (i < cIndex + colspan) && (i < col_count); ++i)
                    {
                        sum += this.columns[i].Width;
                    }
                    sum += (colspan - 1) * horizontal_spacing;
                    return sum;
                }
            }

        }
    }
}