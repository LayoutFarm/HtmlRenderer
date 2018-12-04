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

using System;
using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    public enum CssRunKind : byte
    {
        Unknown,
        Text,
        /// <summary>
        /// unsplitable content like image,input control etc.
        /// </summary>
        SolidContent,
        BlockRun,
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
        CssBox _owner;
        readonly CssRunKind _runKind;
        CssLineBox _hostline;
        /// <summary>
        /// Rectangle
        /// </summary>         
        float _x;
        float _y;
        float _width;
        float _height;
#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="owner">the CSS box owner of the word</param>
        protected CssRun(CssRunKind rectKind)
        {
            _runKind = rectKind;
        }
        public void SetOwner(CssBox owner)
        {
            _owner = owner;
        }
        /// <summary>
        /// Gets the Box where this word belongs.
        /// </summary>
        public CssBox OwnerBox
        {
            get { return _owner; }
        }
        internal CssLineBox HostLine
        {
            get { return _hostline; }
        }
        internal static void SetHostLine(CssRun run, CssLineBox hostline)
        {
            run._hostline = hostline;
        }

        internal void InvalidateGraphics()
        {
            switch (_runKind)
            {
                case CssRunKind.BlockRun:
                    {
                        //TODO: review here again

                        CssBlockRun blockRun = (CssBlockRun)this;
                        CssLineBox ownerLine = blockRun.HostLine;
                        Rectangle r = new Rectangle(
                           (int)(this.Left + blockRun.Left),
                           (int)(this.Top + blockRun.Top + ownerLine.CachedLineTop),
                           (int)this.Width,
                           (int)this.Height);
                        CssBox ownerBox = ownerLine.OwnerBox;
                        ownerBox.InvalidateGraphics(r);
                    }
                    break;
                default:
                    //fine owner
                    {
                        CssLineBox ownerLine = this.HostLine;
                        Rectangle r = new Rectangle(
                           (int)(this.Left),
                           (int)(this.Top + ownerLine.CachedLineTop),
                           (int)this.Width,
                           (int)this.Height);
                        CssBox ownerBox = ownerLine.OwnerBox;
                        ownerBox.InvalidateGraphics(r);
                    }
                    break;
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
        public CssRunKind Kind
        {
            get
            {
                return _runKind;
            }
        }


        /// <summary>
        /// Gets or sets the bounds of the rectangle
        /// </summary>
        public RectangleF Rectangle
        {
            get { return new RectangleF(_x, _y, _width, _height); }
        }

        /// <summary>
        /// Left of the rectangle
        /// </summary>
        public float Left
        {
            get { return _x; }
            set
            {
                _x = value;
            }
        }
        /// <summary>
        /// Top of the rectangle
        /// </summary>
        public float Top
        {
            get { return _y; }
            set { _y = value; }
        }
        internal void SetLocation(float x, float y)
        {
            _x = x;
            _y = y;
        }
        internal void Offset(float xdiff, float ydiff)
        {
            _x += xdiff;
            _y += ydiff;
        }
        internal void OffsetY(float ydiff)
        {
            _y += ydiff;
        }
        internal void SetSize(float w, float h)
        {
            _width = w;
            _height = h;
        }
        /// <summary>
        /// Width of the rectangle
        /// </summary>
        public float Width
        {
            get { return _width; }
            set
            {
                _width = value;
            }
        }

        /// <summary>
        /// Height of the rectangle
        /// </summary>
        public float Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// Gets or sets the right of the rectangle. When setting, it only affects the Width of the rectangle.
        /// </summary>
        public float Right
        {
            get { return _x + _width; }
        }

        /// <summary>
        /// Gets or sets the bottom of the rectangle. When setting, it only affects the Height of the rectangle.
        /// </summary>
        public float Bottom
        {
            get { return _y + _height; }
        }


        /// <summary>
        /// Gets if the word represents solid content like  image, input control
        /// </summary>
        public bool IsSolidContent
        {
            get { return _runKind == CssRunKind.SolidContent; }
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
                return _runKind >= CssRunKind.SingleSpace;
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
                return _runKind == CssRunKind.LineBreak;
            }
        }
        /// <summary>
        /// Gets the text of the word
        /// </summary>
        public virtual string Text
        {
            get { return null; }
        }

        public abstract void WriteContent(System.Text.StringBuilder stbuilder, int start, int length);
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

        internal void FindSelectionPoint(ITextService ifonts,
            int offset, out int selectionIndex,
            out int runSelectionOffsetPx)
        {
            int charFit;
            int charFitWidth;
            int maxWidth = offset;
            switch (this.Kind)
            {
                case CssRunKind.BlockRun:
                    {
                        //contains sub  
                        selectionIndex = -1;
                        runSelectionOffsetPx = 0;
                    }
                    break;
                case CssRunKind.SolidContent:
                    {
                        // not a text word - set full selection
                        selectionIndex = -1;
                        runSelectionOffsetPx = 0;
                    }
                    break;
                case CssRunKind.Text:
                    {
                        char[] ownerTextBuff = CssBox.UnsafeGetTextBuffer(this.OwnerBox);
                        CssTextRun textRun = (CssTextRun)this;
                        var textBuf = new TextBufferSpan(ownerTextBuff, textRun.TextStartIndex, textRun.TextLength);

                        ifonts.MeasureString(ref textBuf,
                            this.OwnerBox.ResolvedFont, maxWidth, out charFit, out charFitWidth);
                        selectionIndex = charFit;
                        runSelectionOffsetPx = charFitWidth;


                    }
                    break;
                case CssRunKind.Space:
                    {
                        char[] ownerTextBuff = CssBox.UnsafeGetTextBuffer(this.OwnerBox);
                        CssTextRun textRun = (CssTextRun)this;
                        var textBuf = new TextBufferSpan(ownerTextBuff, textRun.TextStartIndex, textRun.TextLength);

                        ifonts.MeasureString(ref textBuf,
                            this.OwnerBox.ResolvedFont, maxWidth, out charFit, out charFitWidth);
                        selectionIndex = charFit;
                        runSelectionOffsetPx = charFitWidth;


                    }
                    break;
                case CssRunKind.SingleSpace:
                    {
                        if (offset > this.Width / 2)
                        {
                            selectionIndex = -1;
                            runSelectionOffsetPx = 0;
                        }
                        else
                        {
                            selectionIndex = 0;
                            runSelectionOffsetPx = (int)this.Width;
                        }
                    }
                    break;
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }


        //public CssBox GetGlobalLocation(out float globalX, out float globalY)
        //{
        //    //get global location              
        //    float x2, y2;
        //    var root = _hostline.OwnerBox.GetGlobalLocation(out x2, out y2);
        //    globalX = x2 + _x;
        //    globalY = y2 + _y;
        //    return root;
        //}
    }
}
