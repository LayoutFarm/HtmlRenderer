//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo


namespace LayoutFarm.HtmlBoxes
{
    partial class CssTableLayoutEngine
    {
        enum ColumnSpecificWidthLevel : byte
        {
            None,
            StartAtMin,
            Adjust,
            ExpandToMax,
            FromCellConstraint
        }

        class TableColumn
        {
            int _index;
            ColumnSpecificWidthLevel _spWidthLevel;
            float _actualWidth;
            float _minContentWidth;
            float _maxContentWidth;
            public TableColumn(int index)
            {
                this._index = index;
            }
            public float Width => this._actualWidth;

            public void SetWidth(float width, ColumnSpecificWidthLevel specificWidthLevel)
            {
#if DEBUG
                if (width > 360)
                {

                }
#endif
                this.SpecificWidthLevel = specificWidthLevel;
                this._actualWidth = width;
            }
            public float MinContentWidth => this._minContentWidth;

            public float MaxContentWidth => this._maxContentWidth;

            public bool HasSpecificWidth => this._spWidthLevel == ColumnSpecificWidthLevel.FromCellConstraint;

            public ColumnSpecificWidthLevel SpecificWidthLevel
            {
                get => this._spWidthLevel;
                private set
                {
                    //if (this.index == 1 && value == ColumnSpecificWidthLevel.FromCellConstraint)
                    //{ 
                    //}
                    if (this._spWidthLevel == ColumnSpecificWidthLevel.FromCellConstraint)
                    {
                    }
                    else
                    {
                        this._spWidthLevel = value;
                    }
                }
            }
            //-----------------------------------------------------
            public void S3_UpdateIfWider(float newWidth, ColumnSpecificWidthLevel level)
            {
                //called at state3 only
                if (newWidth > this._actualWidth)
                {
                    this._actualWidth = newWidth;
                    this.SpecificWidthLevel = level;
                }
            }
            public void S5_SetMinWidth(float minWidth)
            {
                this._minContentWidth = minWidth;
            }
            public void UpdateMinMaxContentWidthIfWider(float newMinContentWidth, float newMaxContentWidth)
            {
                if (newMinContentWidth > this._minContentWidth)
                {
                    this._minContentWidth = newMinContentWidth;
                }

                if (newMaxContentWidth > this._maxContentWidth)
                {
                    this._maxContentWidth = newMaxContentWidth;
                }
            }
            public bool TouchLowerLimit => this._actualWidth <= this._minContentWidth;

            public bool TouchUpperLimit => return this._actualWidth >= this._maxContentWidth; 
            

            public void AddMoreWidthValue(float offset, ColumnSpecificWidthLevel level)
            {
                this._actualWidth += offset;
                this.SpecificWidthLevel = level;
            }
            public void UseMinWidth()
            {
                this._actualWidth = this._minContentWidth;
                this.SpecificWidthLevel = ColumnSpecificWidthLevel.StartAtMin;
            }
            public void UseMaxWidth()
            {
                this._actualWidth = this.MaxContentWidth;
                this.SpecificWidthLevel = ColumnSpecificWidthLevel.ExpandToMax;
            }
            public void AdjustDecrByOne()
            {
                this._actualWidth--;
                this.SpecificWidthLevel = ColumnSpecificWidthLevel.Adjust;
            }
        }
        class TableColumnCollection
        {
            TableColumn[] _columns;
            public TableColumnCollection(int columnCount)
            {
                this._columns = new TableColumn[columnCount];
                for (int i = columnCount - 1; i >= 0; --i)
                {
                    this._columns[i] = new TableColumn(i);
                }
            }
            public void SetColumnWidth(int colIndex, float width)
            {
                _columns[colIndex].SetWidth(width, ColumnSpecificWidthLevel.FromCellConstraint);
            }
            public TableColumn this[int index] => this._columns[index];


            public void CountUnspecificWidthColumnAndOccupiedSpace(out int numOfUnspecificColWidth, out float occupiedSpace)
            {
                numOfUnspecificColWidth = 0;
                occupiedSpace = 0f;
                for (int i = _columns.Length - 1; i >= 0; --i)
                {
                    TableColumn col = _columns[i];
                    switch (col.SpecificWidthLevel)
                    {
                        case ColumnSpecificWidthLevel.None:
                            {
                                numOfUnspecificColWidth++;
                            }
                            break;
                        default:
                            {
                                occupiedSpace += col.Width;
                            }
                            break;
                    }
                    //if (!col.HasSpecificWidth)
                    //{
                    //    numOfUnspecificColWidth++;
                    //}
                    //else
                    //{
                    //    occupiedSpace += col.Width;
                    //}
                }
            }

            public bool FindFirstReducibleColumnWidth(int startAtIndex, out int foundAtIndex)
            {
                int col_count = _columns.Length;
                for (int i = startAtIndex; i < col_count; ++i)
                {
                    if (!_columns[i].TouchLowerLimit)
                    {
                        foundAtIndex = i;
                        return true;
                    }
                }
                foundAtIndex = -1;
                return false;
            }
            public int Count => this._columns.Length;


            public float CalculateTotalWidth()
            {
                float total = 0;
                for (int i = _columns.Length - 1; i >= 0; --i)
                {
                    total += _columns[i].Width;
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

            public void S4_AddMoreWidthToColumns(bool onlyNonspeicificWidth, float value)
            {
                if (onlyNonspeicificWidth)
                {
                    for (int i = _columns.Length - 1; i >= 0; --i)
                    {
                        TableColumn col = _columns[i];
                        if (!col.HasSpecificWidth)
                        {
                            col.AddMoreWidthValue(value, ColumnSpecificWidthLevel.Adjust);
                        }
                    }
                }
                else
                {
                    for (int i = _columns.Length - 1; i >= 0; --i)
                    {
                        _columns[i].AddMoreWidthValue(value, ColumnSpecificWidthLevel.Adjust);
                    }
                }
            }
            public void LowerAllColumnToMinWidth()
            {
                for (int i = _columns.Length - 1; i >= 0; --i)
                {
                    _columns[i].UseMinWidth();
                }
            }
            /// <summary>
            /// Gets the spanned width of a cell (With of all columns it spans minus one).
            /// </summary>
            public float GetSpannedMinWidth(int rowCellCount, int realcolindex, int colspan)
            {
                float w = 0f;
                int min_widths_len = this._columns.Length;
                for (int i = realcolindex; (i < min_widths_len) && (i < rowCellCount || i < realcolindex + colspan - 1); ++i)
                {
                    w += this._columns[i].MinContentWidth;
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
                    return this._columns[cIndex].Width;
                }
                else
                {
                    float sum = 0f;
                    int col_count = this._columns.Length;
                    for (int i = cIndex; (i < cIndex + colspan) && (i < col_count); ++i)
                    {
                        sum += this._columns[i].Width;
                    }
                    sum += (colspan - 1) * horizontal_spacing;
                    return sum;
                }
            }
        }
    }
}