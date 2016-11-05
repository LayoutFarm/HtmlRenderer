//MIT, 2014-2016, WinterDev
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

            var fontGlyph = new FontGlyph();
            NativeMyFontsLib.MyFtLoadGlyph(ftFaceHandle, glyphIndex, out fontGlyph.glyphMatrix);
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
            var fontGlyph = new FontGlyph();
            NativeMyFontsLib.MyFtLoadChar(ftFaceHandle, unicodeChar, out fontGlyph.glyphMatrix);
            BuildOutlineGlyph(fontGlyph, pixelSize);
            return fontGlyph;
        }
        void BuildBitmapGlyph(FontGlyph fontGlyph, int pxsize)
        {
            NativeFontGlyphBuilder.CopyGlyphBitmap(fontGlyph);
        }
        void BuildOutlineGlyph(FontGlyph fontGlyph, int pxsize)
        {
            NativeFontGlyphBuilder.BuildGlyphOutline(fontGlyph);
            fontGlyph.flattenVxs = NativeFontGlyphBuilder.FlattenVxs(fontGlyph.originalVxs);
        }
    }
}