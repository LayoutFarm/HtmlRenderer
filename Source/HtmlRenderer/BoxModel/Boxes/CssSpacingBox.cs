//BSD 2014,
//ArthurHub

using System;
using System.Collections.Generic;
using HtmlRenderer.Entities;

namespace HtmlRenderer.Dom
{
    /// <summary>
    /// Used to make space on vertical cell combination
    /// </summary>
    sealed class CssSpacingBox : CssBox
    {
        #region Fields and Consts

        private readonly CssBox _extendedBox;

        /// <summary>
        /// the index of the row where box starts
        /// </summary>
        //private readonly int _startRow;

        /// <summary>
        /// the index of the row where box ends
        /// </summary>
        private readonly int _endRow;
        #endregion


        public CssSpacingBox(CssBox tableBox, CssBox extendedBox, int startRow)
            : base(tableBox, null)
        {
            _extendedBox = extendedBox;
            this.ColSpan = 1;
            this.CssDisplay = CssDisplay.None;
            //_endRow = startRow + Int32.Parse(extendedBox.GetAttribute("rowspan", "1")) - 1;
            _endRow = startRow + extendedBox.RowSpan - 1;
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
    }
}