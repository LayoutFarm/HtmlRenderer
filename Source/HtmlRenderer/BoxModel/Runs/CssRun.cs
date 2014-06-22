//BSD 2014,WinterCore

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
using System;
using System.Drawing;
using HtmlRenderer.Handlers;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{
    public enum CssRunKind : byte
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
    public abstract class CssRun
    {


        /// <summary>
        /// the CSS box owner of the word
        /// </summary>
        readonly CssBox _ownerBox;
        readonly CssRunKind _runKind;
        CssLineBox _hostline;

        /// <summary>
        /// Rectangle
        /// </summary>         
        float _x;
        float _y;
        float _width;
        float _height;

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="owner">the CSS box owner of the word</param>
        protected CssRun(CssBox owner, CssRunKind rectKind)
        {
            this._ownerBox = owner;
            this._runKind = rectKind;
        }

        internal CssLineBox HostLine
        {
            get { return this._hostline; }
        }
        internal static void SetHostLine(CssRun run, CssLineBox hostline)
        {
            run._hostline = hostline;
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
        public CssRunKind Kind
        {
            get
            {
                return this._runKind;
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
            get { return new RectangleF(this._x, this._y, this._width, this._height); }

        }

        /// <summary>
        /// Left of the rectangle
        /// </summary>
        public float Left
        {
            get { return this._x; }
            set { this._x = value; }
        }

        /// <summary>
        /// Top of the rectangle
        /// </summary>
        public float Top
        {
            get { return this._y; }
            set { this._y = value; }
        }
        internal void SetLocation(float x, float y)
        {
            this._x = x;
            this._y = y;
        }
        internal void Offset(float xdiff, float ydiff)
        {
            this._x += xdiff;
            this._y += ydiff;
        }
        internal void OffsetY(float ydiff)
        {
            this._y += ydiff;
        }
        internal void SetSize(float w, float h)
        {
            this._width = w;
            this._height = h;
        }
        /// <summary>
        /// Width of the rectangle
        /// </summary>
        public float Width
        {
            get { return this._width; }
            set { this._width = value; }
        }



        /// <summary>
        /// Height of the rectangle
        /// </summary>
        public float Height
        {
            get { return this._height; }
            set { this._height = value; }
        }

        /// <summary>
        /// Gets or sets the right of the rectangle. When setting, it only affects the Width of the rectangle.
        /// </summary>
        public float Right
        {
            get { return this._x + this._width; }
        }

        /// <summary>
        /// Gets or sets the bottom of the rectangle. When setting, it only affects the Height of the rectangle.
        /// </summary>
        public float Bottom
        {
            get { return this._y + this._height; }
        }
        /// <summary>
        /// Gets the image this words represents (if one exists)
        /// </summary>
        public virtual Image Image
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// Gets if the word represents an image.
        /// </summary>
        public bool IsImage
        {
            get { return this._runKind == CssRunKind.Image; }
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
                return this._runKind >= CssRunKind.SingleSpace;
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
                return this._runKind == CssRunKind.LineBreak;
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
            string txt = this.Text;
            return string.Format("{0} ({1} char{2})",
                txt.Replace(' ', '-').Replace("\n", "\\n"), txt.Length, txt.Length != 1 ? "s" : string.Empty);
        }

        public void FindSelectionPoint(IGraphics g,
            int offset, bool inclusive, out int selectionIndex,
            out int selectionOffset)
        {

            int charFit;
            int charFitWidth;
            var maxWidth = offset + (inclusive ? 0 : 1.5f * this.LeftGlyphPadding);

            //return new string(ownerTextBuff, this._textStartIndex, this._textLength); 
            //g.MeasureString(word.Text, font, maxWidth, out charFit, out charFitWidth);  
            //between
            switch (this.Kind)
            {
                case CssRunKind.Image:
                    {
                        // not a text word - set full selection
                        selectionIndex = -1;
                        selectionOffset = -1;
                    } break;
                case CssRunKind.Text:
                    {
                        char[] ownerTextBuff = CssBox.UnsafeGetTextBuffer(this.OwnerBox);
                        CssTextRun textRun = (CssTextRun)this;
                        g.MeasureString2(ownerTextBuff, textRun.TextStartIndex, textRun.TextLength,
                            this.OwnerBox.ActualFont, maxWidth, out charFit, out charFitWidth);
                        selectionIndex = charFit;
                        selectionOffset = charFitWidth;
                    } break;
                case CssRunKind.Space:
                    {
                        throw new NotSupportedException();
                    } break;
                case CssRunKind.SingleSpace:
                    {

                        if (offset > this.Width / 2)
                        {
                            selectionIndex = -1;
                            selectionOffset = 0;
                        }
                        else
                        {
                            selectionIndex = 0;
                            selectionOffset = (int)this.Width;
                        }
                    } break;
                default:
                    {
                        throw new NotSupportedException();
                    }
            }

        }
    }
}
