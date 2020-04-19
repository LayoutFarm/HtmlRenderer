//MIT, 2020, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;


namespace PixelFarm.Drawing
{


    [Flags]
    public enum FontStyle : byte
    {
        Regular = 0,
        Bold = 1,
        Italic = 1 << 1,
        Underline = 1 << 2,
        Strikeout = 1 << 3
    }


    /// <summary>
    /// user request font specification
    /// </summary>
    public sealed class RequestFont
    {
        //each platform/canvas has its own representation of this Font 
        //this is just a request for specficic font presentation at a time
        //----- 

        public RequestFont(string facename, float fontSizeInPts, FontStyle style = FontStyle.Regular)
            : this(facename, Len.Pt(fontSizeInPts), style)
        {
        }
        public RequestFont(string facename, Len fontSize, FontStyle style = FontStyle.Regular)
        {

            //Lang = "en";//default
            Name = facename;
            Size = fontSize; //store user font size here

            Style = style;
            float fontSizeInPts = SizeInPoints = fontSize.ToPoints();
            FontKey = CalculateFontKey(facename, fontSizeInPts, style);
        }
        public Len Size { get; private set; }
        //
        public int FontKey { get; private set; }
        /// <summary>
        /// font's face name
        /// </summary>
        public string Name { get; private set; }
        public FontStyle Style { get; private set; }

        /// <summary>
        /// emheight in point unit
        /// </summary>
        public float SizeInPoints { get; private set; }

        public static int CalculateFontKey(string facename, float fontSizeInPts, FontStyle style)
        {
            return (new InternalFontKey(facename, fontSizeInPts, style)).GetHashCode();
        }

        struct InternalFontKey
        {

            public readonly int FontNameIndex;
            public readonly float FontSize;
            public readonly FontStyle FontStyle;

            public InternalFontKey(string fontname, float fontSize, FontStyle fs)
            {
                //font name/ not filename
                this.FontNameIndex = RegisterFontName(fontname.ToLower());
                this.FontSize = fontSize;
                this.FontStyle = fs;
            }

            static Dictionary<string, int> s_registerFontNames = new Dictionary<string, int>();

            static InternalFontKey()
            {
                RegisterFontName(""); //blank font name
            }
            static int RegisterFontName(string fontName)
            {
                //TODO: review here again
                fontName = fontName.ToUpper();
                return 1;
                //if (!s_registerFontNames.TryGetValue(fontName, out int found))
                //{
                //    int nameCrc32 = CRC32Calculator.CalculateCrc32(fontName);
                //    s_registerFontNames.Add(fontName, nameCrc32);
                //    return nameCrc32;
                //}
                //return found;
            }
            public override int GetHashCode()
            {
                return CalculateGetHasCode(this.FontNameIndex, this.FontSize, (int)this.FontStyle);
            }
            static int CalculateGetHasCode(int nameIndex, float fontSize, int fontstyle)
            {
                //modified from https://stackoverflow.com/questions/1646807/quick-and-simple-hash-code-combinations
                unchecked
                {
                    int hash = 17;
                    hash = hash * 31 + nameIndex.GetHashCode();
                    hash = hash * 31 + fontSize.GetHashCode();
                    hash = hash * 31 + fontstyle.GetHashCode();
                    return hash;
                }
            }
        }

        //------------------ 
        //caching ...

        internal int _platform_id;//resolve by system id
        internal object _latestResolved; //result of the actual font
        internal int _whitespace_width;
        internal int _generalLineSpacingInPx;


        //TODO: review here again
        internal float _sizeInPx;
        internal float _descentInPx;
        internal float _ascentInPx;
        internal float _lineGapInPx;

        public float SizeInPixels => _sizeInPx;
        public float DescentInPixels => _descentInPx;
        public float AscentInPixels => _ascentInPx;
        public float LineGapInPixels => _lineGapInPx;

        /// <summary>
        /// already in pixels
        /// </summary>
        public int LineSpacingInPixels => _generalLineSpacingInPx;
#if DEBUG
        public override string ToString()
        {
            return Name + "," + SizeInPoints + "," + Style;
        }
#endif
    }


    public partial class Image
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }


    public interface ITextService
    {

        float MeasureWhitespace(RequestFont f);
        float MeasureBlankLineHeight(RequestFont f);
        //
        bool SupportsWordBreak { get; }

        ILineSegmentList BreakToLineSegments(in TextBufferSpan textBufferSpan);
        //
        Size MeasureString(in TextBufferSpan textBufferSpan, RequestFont font);

        void MeasureString(in TextBufferSpan textBufferSpan, RequestFont font, int maxWidth, out int charFit, out int charFitWidth);

        void CalculateUserCharGlyphAdvancePos(in TextBufferSpan textBufferSpan,
                RequestFont font,
                ref TextSpanMeasureResult result);
        void CalculateUserCharGlyphAdvancePos(in TextBufferSpan textBufferSpan, ILineSegmentList lineSegs,
                RequestFont font,
                ref TextSpanMeasureResult result);
    }
    public interface ILineSegmentList : System.IDisposable
    {
        int Count { get; }
        ILineSegment this[int index] { get; }
    }
    public interface ILineSegment
    {
        int Length { get; }
        int StartAt { get; }
    }

    public struct TextBufferSpan
    {
        public readonly int start;
        public readonly int len;

        char[] _rawString;
        public TextBufferSpan(char[] rawCharBuffer)
        {
            _rawString = rawCharBuffer;
            this.len = rawCharBuffer.Length;
            this.start = 0;
        }
        public TextBufferSpan(char[] rawCharBuffer, int start, int len)
        {
            this.start = start;
            this.len = len;
            _rawString = rawCharBuffer;
        }

        public override string ToString()
        {
            return start + ":" + len;
        }


        public char[] GetRawCharBuffer() => _rawString;
    }

    public struct TextSpanMeasureResult
    {
        public int[] outputXAdvances;
        public int outputTotalW;
        public ushort lineHeight;

        public bool hasSomeExtraOffsetY;
        public short minOffsetY;
        public short maxOffsetY;
    }


    public delegate void LoadImageFunc(ImageBinder binder);




    public class ImageBinder
    {

        /// <summary>
        /// local img cached
        /// </summary>
        PixelFarm.Drawing.Image _localImg;
        bool _isLocalImgOwner;
        LoadImageFunc _lazyLoadImgFunc;
        int _previewImgWidth = 16; //default ?
        int _previewImgHeight = 16;
        bool _isAtlasImg;
#if DEBUG
        static int dbugTotalId;
        public int dbugId = dbugTotalId++;
#endif

        protected ImageBinder() { }
        public Image LocalImage { get; set; }
        public ImageBinder(string imgSource, bool isMemBmpOwner = false)
        {
            ImageSource = imgSource;
            _isLocalImgOwner = isMemBmpOwner; //if true=> this binder will release a local cahed img
        }

        //        public ImageBinder(PixelFarm.CpuBlit.MemBitmap memBmp, bool isMemBmpOwner = false)
        //        {
        //#if DEBUG
        //            if (memBmp == null)
        //            {
        //                throw new NotSupportedException();
        //            }
        //#endif
        //            //binder to image
        //            _localImg = memBmp;
        //            _isLocalImgOwner = isMemBmpOwner; //if true=> this binder will release a local cahed img
        //            this.State = BinderState.Loaded;
        //        }
        public ImageBinder(PixelFarm.Drawing.Image otherImg, bool isMemBmpOwner = false)
        {
#if DEBUG
            if (otherImg == null)
            {
                throw new NotSupportedException();
            }
#endif
            //binder to image
            _localImg = otherImg;
            _isLocalImgOwner = isMemBmpOwner; //if true=> this binder will release a local cahed img
            this.State = BinderState.Loaded;
        }

        public event System.EventHandler ImageChanged;

        public virtual void RaiseImageChanged()
        {
            try
            {
                ImageChanged?.Invoke(this, System.EventArgs.Empty);
            }
            catch (Exception ex)
            {

            }
        }

        //#if DEBUG
        //        public override void dbugNotifyUsage()
        //        {
        //        }
        //#endif
        //        public override void ReleaseLocalBitmapIfRequired()
        //        {

        //        }
        /// <summary>
        /// preview img size is an expected(assume) img of original img, 
        /// but it may not equal to the actual size after img is loaded.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public void SetPreviewImageSize(int w, int h)
        {
            _previewImgWidth = w;
            _previewImgHeight = h;
        }

        /// <summary>
        /// reference to original 
        /// </summary>
        public string ImageSource { get; set; }

        /// <summary>
        /// current loading/binding state
        /// </summary>
        public BinderState State { get; set; }

        ///// <summary>
        ///// read already loaded img
        /////// </summary>
        //public PixelFarm.Drawing.Image LocalImage => _localImg;

        public void ClearLocalImage()
        {
            this.State = BinderState.Unloading;//reset this to unload?

            //if (_localImg != null)
            //{
            //    if (_isLocalImgOwner)
            //    {
            //        _localImg.Dispose();
            //    }
            //    _localImg = null;
            //}

            //TODO: review here
            this.State = BinderState.Unload;//reset this to unload?
        }
        //public override void Dispose()
        //{
        //    if (this.State == BinderState.Loaded)
        //    {
        //        ClearLocalImage();
        //    }
        //}

        public int Width => (_localImg != null) ? _localImg.Width : _previewImgWidth; //default?

        public int Height => (_localImg != null) ? _localImg.Height : _previewImgHeight;

        /// <summary>
        /// set local loaded image
        /// </summary>
        /// <param name="image"></param>
        public virtual void SetLocalImage(PixelFarm.Drawing.Image image, bool raiseEvent = true)
        {
            //set image to this binder
            if (image != null)
            {
                _localImg = image;
                this.State = BinderState.Loaded;


                if (raiseEvent)
                {
                    RaiseImageChanged();
                }
                else
                {
                    //eg. when we setLocalImage 
                    //from other thread  
                    //don't call raise image changed directly here
                    //please use 'main thread queue' to invoke this
                }
            }
            else
            {
                //if set to null 
            }
        }

        public bool HasLazyFunc => _lazyLoadImgFunc != null;

        /// <summary>
        /// set lazy img loader
        /// </summary>
        /// <param name="lazyLoadFunc"></param>
        public void SetImageLoader(LoadImageFunc lazyLoadFunc)
        {
            _lazyLoadImgFunc = lazyLoadFunc;
        }
        public void LazyLoadImage()
        {
            _lazyLoadImgFunc?.Invoke(this);
        }
        //public override IntPtr GetRawBufferHead()
        //{

        //    PixelFarm.CpuBlit.MemBitmap bmp = _localImg as PixelFarm.CpuBlit.MemBitmap;
        //    if (bmp != null)
        //    {
        //        return PixelFarm.CpuBlit.MemBitmap.GetBufferPtr(bmp).Ptr;
        //    }

        //    return IntPtr.Zero;
        //}
        //public override void ReleaseBufferHead()
        //{

        //}

        //public override bool IsYFlipped => false;
        //
        public static readonly ImageBinder NoImage = new NoImageImageBinder();
        public virtual bool IsAtlasImage => false;

        class NoImageImageBinder : ImageBinder
        {
            public NoImageImageBinder()
            {
                this.State = BinderState.Blank;
            }
            //public override IntPtr GetRawBufferHead() => IntPtr.Zero; 
            //public override void ReleaseBufferHead()
            //{
            //}
        }
    }

    public enum BinderState : byte
    {
        Unload,
        Loaded,
        Loading,
        Unloading,
        Error,
        Blank
    }

}

namespace LayoutFarm.Css
{
    static class FontDefaultConfig
    {

        internal static string DEFAULT_FONT_NAME = "Tahoma";
        /// <summary>
        /// Default font size in points. Change this value to modify the default font size.
        /// </summary>
        public const float DEFAULT_FONT_SIZE = 10f;
    }

}

namespace LayoutFarm.HtmlBoxes
{
    partial class CssBox
    {
        public void InvalidateGraphics() { }
        public void InvalidateGraphics(RectangleF r) { }
    }

}