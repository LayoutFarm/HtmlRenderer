//MIT, 2014-2017, WinterDev
//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//----------------------------------- 

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PixelFarm.Drawing.Fonts
{
    class NativeFontGlyph : FontGlyph
    {
        /// <summary>
        /// original 8bpp image buffer
        /// </summary>
        public byte[] glyImgBuffer8;
        public IntPtr nativeOutlinePtr;
        public IntPtr nativeBmpPtr;
    }

    class NativeFontFace : FontFace
    {
        /// <summary>
        /// store font file content in unmanaged memory
        /// </summary>
        IntPtr unmanagedMem;
        /// <summary>
        /// free type handle (unmanaged mem)
        /// </summary>
        IntPtr ftFaceHandle;
        int currentFacePixelSize = 0;

        IntPtr hb_font;
        ExportFace exportFace = new ExportFace();
        string name;
        string fontPath;
        FontStyle fontStyle;
        internal NativeFontFace(IntPtr unmanagedMem, IntPtr ftFaceHandle, string name, string fontPath, FontStyle fontStyle)
        {
            this.name = name;
            this.fontPath = fontPath;
            this.fontStyle = fontStyle;

            this.unmanagedMem = unmanagedMem;
            this.ftFaceHandle = ftFaceHandle;
            //get face information             
            unsafe
            {
                fixed (ExportFace* face_h = &this.exportFace)
                {
                    NativeMyFontsLib.MyFtGetFaceData(unmanagedMem, face_h);
                }
            }
        }
        public override float GetScale(float pointSize)
        {
            return 1 / 64;
        }
        public override int AscentInDzUnit
        {
            get { return exportFace.ascender << 6; }
        }
        public override int DescentInDzUnit
        {
            get { return exportFace.descender << 6; }
        }
        public override int LineGapInDzUnit
        {
            //?
            //TODO: review here
            get { return (int)(AscentInDzUnit * 0.20); }
        }
        public override string Name
        {
            get { return name; }
        }
        public override string FontPath
        {
            get { return fontPath; }
        }
        ~NativeFontFace()
        {
            Dispose();
        }
        public override ActualFont GetFontAtPointsSize(float fontPointSize)
        {
            currentFacePixelSize = (int)RequestFont.ConvEmSizeInPointsToPixels(fontPointSize);
            //----------------------------------
            //set current fontface size             
            NativeMyFontsLib.MyFtSetPixelSizes(this.ftFaceHandle, currentFacePixelSize);
            //create font size 
            return new NativeFont(this, this.name, FontStyle.Regular, currentFacePixelSize);
        }
        /// <summary>
        /// free typpe handler
        /// </summary>
        public IntPtr Handle
        {
            get { return this.ftFaceHandle; }
        }
        public string LoadFromFilename
        {
            get;
            set;
        }

        protected override void OnDispose()
        {
            if (this.ftFaceHandle != IntPtr.Zero)
            {
                NativeMyFontsLib.MyFtDoneFace(this.ftFaceHandle);
                ftFaceHandle = IntPtr.Zero;
            }
            if (unmanagedMem != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(unmanagedMem);
                unmanagedMem = IntPtr.Zero;
            }

        }
        /// <summary>
        /// ascent in font unit
        /// </summary>
        public int Ascent
        {
            get
            {
                return exportFace.ascender;
            }
        }

        /// <summary>
        /// descent in font unit
        /// </summary>
        public int Descent
        {
            get
            {
                return exportFace.descender;
            }
        }
        public int UnitPerEm
        {
            get { return exportFace.units_per_EM; }
        }
        //---------------------------
        //for font shaping engine
        //--------------------------- 
        internal IntPtr HBFont
        {
            get { return this.hb_font; }
            set { this.hb_font = value; }
        }

        public override object GetInternalTypeface()
        {
            throw new NotImplementedException();
        }

        //internal NativeFont GetFontAtPixelSize(int pixelSize)
        //{
        //    NativeFont found;
        //    if (!fonts.TryGetValue(pixelSize, out found))
        //    {
        //        //----------------------------------
        //        //set current fontface size
        //        currentFacePixelSize = pixelSize;
        //        NativeMyFontsLib.MyFtSetPixelSizes(this.ftFaceHandle, pixelSize);
        //        //create font size 
        //        NativeFont f = new NativeFont(this, this.name, FontStyle.Regular, pixelSize);
        //        fonts.Add(pixelSize, f);
        //        //------------------------------------
        //        return f;
        //    }
        //    return found;
        //}
        //internal NativeFont GetFontAtPointSize(float fontPointSize)
        //{
        //    //convert from point size to pixelsize ***              
        //    NativeFont nativeFont = GetFontAtPixelSize((int)RequestFont.ConvEmSizeInPointsToPixels(fontPointSize));

        //    return nativeFont;
        //}

        internal FontGlyph ReloadGlyphFromIndex(uint glyphIndex, int pixelSize)
        {
            if (currentFacePixelSize != pixelSize)
            {
                currentFacePixelSize = pixelSize;
                NativeMyFontsLib.MyFtSetPixelSizes(this.ftFaceHandle, pixelSize);
            }

            //--------------------------------------------------

            var fontGlyph = new NativeFontGlyph();
            NativeGlyphMatrix nativeGlyphMatrix;
            NativeMyFontsLib.MyFtLoadGlyph(ftFaceHandle, glyphIndex, out nativeGlyphMatrix);
            fontGlyph.glyphMatrix = nativeGlyphMatrix.matrixData;
            BuildOutlineGlyph(fontGlyph, pixelSize);
            return fontGlyph;
        }
        internal FontGlyph ReloadGlyphFromChar(char unicodeChar, int pixelSize)
        {
            if (currentFacePixelSize != pixelSize)
            {
                currentFacePixelSize = pixelSize;
                NativeMyFontsLib.MyFtSetPixelSizes(this.ftFaceHandle, pixelSize);
            }
            //-------------------------------------------------- 
            var fontGlyph = new NativeFontGlyph();
            NativeGlyphMatrix nativeGlyphMatrix;
            NativeMyFontsLib.MyFtLoadChar(ftFaceHandle, unicodeChar, out nativeGlyphMatrix);
            fontGlyph.glyphMatrix = nativeGlyphMatrix.matrixData;
            fontGlyph.nativeBmpPtr = nativeGlyphMatrix.bitmap;
            fontGlyph.nativeOutlinePtr = nativeGlyphMatrix.outline;
            //-------------------------------------------------- 
            BuildOutlineGlyph(fontGlyph, pixelSize);
            return fontGlyph;
        }
        void BuildBitmapGlyph(NativeFontGlyph fontGlyph, int pxsize)
        {
            NativeFontGlyphBuilder.CopyGlyphBitmap(fontGlyph);
        }
        void BuildOutlineGlyph(NativeFontGlyph fontGlyph, int pxsize)
        {
            NativeFontGlyphBuilder.BuildGlyphOutline(fontGlyph);
            Agg.VertexStore vxs = new Agg.VertexStore();
            NativeFontGlyphBuilder.FlattenVxs(fontGlyph.originalVxs, vxs);
            fontGlyph.flattenVxs = vxs;
        }
    }


    /// <summary>
    /// cross platform font
    /// </summary>
    class NativeFont : ActualFont
    {
        NativeFontFace ownerFace;
        float emSizeInPoints;
        int fontSizeInPixelUnit;
        float fontFaceAscentInPx;
        float fontFaceDescentInPx;
        float lineGapInPx;
        string fontName;
        FontStyle fontStyle;
        /// <summary>
        /// glyph
        /// </summary>
        Dictionary<char, FontGlyph> dicGlyphs = new Dictionary<char, FontGlyph>();
        Dictionary<uint, FontGlyph> dicGlyphs2 = new Dictionary<uint, FontGlyph>();
        internal NativeFont(NativeFontFace ownerFace, string fontName, FontStyle fontStyle, int pixelSize)
        {
            this.fontName = fontName;
            this.fontStyle = fontStyle;
            //store unmanage font file information
            this.ownerFace = ownerFace;
            this.fontSizeInPixelUnit = pixelSize;
            this.fontStyle = fontStyle;
            int ascentEmSize = ownerFace.Ascent / ownerFace.UnitPerEm;
            fontFaceAscentInPx = RequestFont.ConvEmSizeInPointsToPixels(ascentEmSize);

            int descentEmSize = ownerFace.Descent / ownerFace.UnitPerEm;
            fontFaceDescentInPx = RequestFont.ConvEmSizeInPointsToPixels(descentEmSize);


            int lineGap = ownerFace.LineGapInDzUnit / ownerFace.UnitPerEm;
            lineGapInPx = RequestFont.ConvEmSizeInPointsToPixels(lineGap);
        }

        public override string FontName
        {
            get { return fontName; }
        }
        public override FontStyle FontStyle
        {
            get { return fontStyle; }
        }
        protected override void OnDispose()
        {
            //TODO: clear resource here 

        }
        public override FontGlyph GetGlyph(char c)
        {
            FontGlyph found;
            if (!dicGlyphs.TryGetValue(c, out found))
            {
                found = ownerFace.ReloadGlyphFromChar(c, fontSizeInPixelUnit);
                this.dicGlyphs.Add(c, found);
            }
            return found;
        }
        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            FontGlyph found;
            if (!dicGlyphs2.TryGetValue(glyphIndex, out found))
            {
                //not found glyph 
                found = ownerFace.ReloadGlyphFromIndex(glyphIndex, fontSizeInPixelUnit);
                this.dicGlyphs2.Add(glyphIndex, found);
            }
            return found;
        }
        internal void SetEmSizeInPoint(float emSizeInPoints)
        {
            this.emSizeInPoints = emSizeInPoints;
        }
        /// <summary>
        /// owner font face
        /// </summary>
        public override FontFace FontFace
        {
            get { return this.ownerFace; }
        }
        internal NativeFontFace NativeFontFace
        {
            get { return this.ownerFace; }
        }

        public override float AscentInPixels
        {
            get { return fontFaceAscentInPx; }
        }

        public override float DescentInPixels
        {
            get { return fontFaceDescentInPx; }
        }
        public override float SizeInPoints { get { return this.emSizeInPoints; } }
        public override float SizeInPixels { get { return fontSizeInPixelUnit; } }
        public override float LineGapInPixels
        {
            get { return lineGapInPx; }
        }
        public override float RecommendedLineSpacingInPixels
        {
            get { return AscentInPixels - DescentInPixels + LineGapInPixels; }
        }



    }
}