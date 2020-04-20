//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

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


namespace LayoutFarm.HtmlBoxes
{
    partial class CssTableLayoutEngine
    {
        /// <summary>
        /// Used to make space on vertical cell combination
        /// </summary>
        sealed class CssVerticalCellSpacingBox : CssBox
        {
            readonly CssBox _extendedBox;
            /// <summary>
            /// the index of the row where box ends
            /// </summary>
            readonly int _endRow;
            public CssVerticalCellSpacingBox(CssBox extendedBox, int startRow)
                : base(s_specForVCell, extendedBox.RootGfx)
            {
                _extendedBox = extendedBox;
                this.SetRowSpanAndColSpan(1, 1);
                _endRow = startRow + extendedBox.RowSpan - 1;
            }

            public CssBox ExtendedBox => _extendedBox;

            /// <summary>
            /// Gets the index of the row where box ends
            /// </summary>
            public int EndRow => _endRow;

            //=========================================================
            static Css.BoxSpec s_specForVCell = new Css.BoxSpec();
            static CssVerticalCellSpacingBox()
            {
                s_specForVCell.Freeze();
            }
        }
    }
}