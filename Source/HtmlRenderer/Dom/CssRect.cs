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

using System.Drawing;
using HtmlRenderer.Handlers;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{
    public enum CssRectKind : byte
    {
        Unknown,
        Text,
        Image,
        LineBreak,

        //------------
        //below here is space
        SingleSpace,
        Space,
    }
    /// <summary>
    /// Represents a word inside an inline box
    /// </summary>
    /// <remarks>
    /// Because of performance, words of text are the most atomic 
    /// element in the project. It should be characters, but come on,
    /// imagine the performance when drawing char by char on the device.<br/>
    /// It may change for future versions of the library.
    /// </remarks>
    public abstract class CssRect
    {
        #region Fields and Consts

        /// <summary>
        /// the CSS box owner of the word
        /// </summary>
        readonly CssBox _ownerBox;
        readonly CssRectKind _rectKind;

        /// <summary>
        /// Rectangle
        /// </summary>
        private RectangleF _rect;

        /// <summary>
        /// If the word is selected this points to the selection handler for more data
        /// </summary>
        private ISelectionHandler _selection;

        /// <summary>
        /// each word has only one owner linebox!
        /// </summary>
        CssLineBox myOwnerLineBox;


        #endregion

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="owner">the CSS box owner of the word</param>
        protected CssRect(CssBox owner, CssRectKind rectKind)
        {
            this._ownerBox = owner;
            this._rectKind = rectKind;
        }
        internal CssLineBox ownerLineBox
        {
            get
            {
                return this.myOwnerLineBox;
            }
            set
            {
#if DEBUG
                //if (value != null && this.myOwnerLineBox != null)
                //{
                //}
#endif

                this.myOwnerLineBox = value;
            }
        }
#if DEBUG
        //int dbugPaintCount;
        //int dbugSnapPass;
        public int debugPaintCount
        {
            get
            {
                return 0;
                //return this.dbugPaintCount;
            }
            set
            {
                //if (dbugCounter.dbugStartRecord
                //     && this.dbugPaintCount > 0)
                //{ 
                //}
                //this.dbugSnapPass = dbugCounter.dbugDrawStringCount;
                //this.dbugPaintCount = value; 
            }
        }
#endif
        public CssRectKind RectKind
        {
            get
            {
                return this._rectKind;
            }
        }
        /// <summary>
        /// Gets the Box where this word belongs.
        /// </summary>
        public CssBox OwnerBox
        {
            get { return _ownerBox; }
        }

        /// <summary>
        /// Gets or sets the bounds of the rectangle
        /// </summary>
        public RectangleF Rectangle
        {
            get { return _rect; }
            set { _rect = value; }
        }

        /// <summary>
        /// Left of the rectangle
        /// </summary>
        public float Left
        {
            get { return _rect.X; }
            set { _rect.X = value; }
        }

        /// <summary>
        /// Top of the rectangle
        /// </summary>
        public float Top
        {
            get { return _rect.Y; }
            set { _rect.Y = value; }
        }

        /// <summary>
        /// Width of the rectangle
        /// </summary>
        public float Width
        {
            get { return _rect.Width; }
            set { _rect.Width = value; }
        }

        /// <summary>
        /// Get the full width of the word including the spacing.
        /// </summary>
        public float FullWidth
        {
            get { return _rect.Width + ActualWordSpacing; }
        }

        /// <summary>
        /// Gets the actual width of whitespace between words.
        /// </summary>
        public float ActualWordSpacing
        {
            get { return (OwnerBox != null ? (HasSpaceAfter ? OwnerBox.ActualWordSpacing : 0) + (IsImage ? OwnerBox.ActualWordSpacing : 0) : 0); }
        }

        /// <summary>
        /// Height of the rectangle
        /// </summary>
        public float Height
        {
            get { return _rect.Height; }
            set { _rect.Height = value; }
        }

        /// <summary>
        /// Gets or sets the right of the rectangle. When setting, it only affects the Width of the rectangle.
        /// </summary>
        public float Right
        {
            get { return Rectangle.Right; }
            set { Width = value - Left; }
        }

        /// <summary>
        /// Gets or sets the bottom of the rectangle. When setting, it only affects the Height of the rectangle.
        /// </summary>
        public float Bottom
        {
            get { return Rectangle.Bottom; }
            set { Height = value - Top; }
        }

        /// <summary>
        /// If the word is selected this points to the selection handler for more data
        /// </summary>
        public ISelectionHandler Selection
        {
            get { return _selection; }
            set { _selection = value; }
        }

        /// <summary>
        /// was there a whitespace before the word chars (before trim)
        /// </summary>
        public virtual bool HasSpaceBefore
        {
            get { return false; }
        }

        /// <summary>
        /// was there a whitespace after the word chars (before trim)
        /// </summary>
        public virtual bool HasSpaceAfter
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the image this words represents (if one exists)
        /// </summary>
        public virtual Image Image
        {
            get { return null; }
            // ReSharper disable ValueParameterNotUsed
            set { }
            // ReSharper restore ValueParameterNotUsed
        }

        /// <summary>
        /// Gets if the word represents an image.
        /// </summary>
        public bool IsImage
        {
            get { return this._rectKind == CssRectKind.Image; }
        }


        /// <summary>
        /// Gets a bool indicating if this word is composed only by spaces.
        /// Spaces include tabs and line breaks
        /// </summary>
        public bool IsSpaces
        {
            get
            {
                //eval once
                return this._rectKind >= CssRectKind.SingleSpace;
            }
        }
        /// <summary>
        /// Gets if the word is composed by only a line break
        /// </summary>
        public bool IsLineBreak
        {
            get
            {
                //eval once
                return this._rectKind == CssRectKind.LineBreak;
            }
        }
        /// <summary>
        /// Gets the text of the word
        /// </summary>
        public virtual string Text
        {
            get { return null; }
        }

        /// <summary>
        /// is the word is currently selected
        /// </summary>
        public bool Selected
        {
            get { return _selection != null; }
        }

        /// <summary>
        /// the selection start index if the word is partially selected (-1 if not selected or fully selected)
        /// </summary>
        public int SelectedStartIndex
        {
            get { return _selection != null ? _selection.GetSelectingStartIndex(this) : -1; }
        }

        /// <summary>
        /// the selection end index if the word is partially selected (-1 if not selected or fully selected)
        /// </summary>
        public int SelectedEndIndexOffset
        {
            get { return _selection != null ? _selection.GetSelectedEndIndexOffset(this) : -1; }
        }

        /// <summary>
        /// the selection start offset if the word is partially selected (-1 if not selected or fully selected)
        /// </summary>
        public float SelectedStartOffset
        {
            get { return _selection != null ? _selection.GetSelectedStartOffset(this) : -1; }
        }

        /// <summary>
        /// the selection end offset if the word is partially selected (-1 if not selected or fully selected)
        /// </summary>
        public float SelectedEndOffset
        {
            get { return _selection != null ? _selection.GetSelectedEndOffset(this) : -1; }
        }

        /// <summary>
        /// Gets or sets an offset to be considered in measurements
        /// </summary>
        public float LeftGlyphPadding
        {
            get { return OwnerBox != null ? FontsUtils.GetFontLeftPadding(OwnerBox.ActualFont) : 0; }
        }

        /// <summary>
        /// Represents this word for debugging purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} ({1} char{2})", Text.Replace(' ', '-').Replace("\n", "\\n"), Text.Length, Text.Length != 1 ? "s" : string.Empty);
        }
    }
}
