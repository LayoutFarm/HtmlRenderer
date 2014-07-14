//BSD 2014, WinterDev
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


namespace HtmlRenderer.Boxes
{
    partial class CssTableLayoutEngine
    {
        /// <summary>
        /// Used to make space on vertical cell combination
        /// </summary>
        sealed class CssVerticalCellSpacingBox : CssBox
        {


            private readonly CssBox _extendedBox;

            /// <summary>
            /// the index of the row where box ends
            /// </summary>
            private readonly int _endRow;

            public CssVerticalCellSpacingBox(CssBox tableBox, 
                HtmlRenderer.Drawing.IFontPool fontPool,
                CssBox extendedBox, int startRow)
                : base(tableBox, null, specForVCell)
            {
                _extendedBox = extendedBox;
                this.SetRowColSpan(1, 1);
                _endRow = startRow + extendedBox.RowSpan - 1;

                ReEvaluateComputedValues(fontPool, tableBox);
                ChangeDisplayType(this, Css.CssDisplay.None);
            }

            public CssBox ExtendedBox
            {
                get { return _extendedBox; }
            }
            /// <summary>
            /// Gets the index of the row where box ends
            /// </summary>
            public int EndRow
            {
                get { return _endRow; }
            }
            //=========================================================
            static Css.BoxSpec specForVCell = new Css.BoxSpec();
            static CssVerticalCellSpacingBox()
            {
                specForVCell.Freeze();
            }
        }

    }
}