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
        /// <summary>
        /// store font glyph for each px size
        /// </summary>
        Dictionary<int, Drawing.Font> fonts = new Dictionary<int, Drawing.Font>();
        IntPtr hb_font;
        internal NativeFontFace(IntPtr unmanagedMem, IntPtr ftFaceHandle)
        {
            this.unmanagedMem = unmanagedMem;
            this.ftFaceHandle = ftFaceHandle;
        }

        ~NativeFontFace()
        {
            Dispose();
        }

        /// <summary>
        /// free typpe handler
        /// </summary>
        internal IntPtr Handle
        {
            get { return this.ftFaceHandle; }
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
            if (fonts != null)
            {
                fonts.Clear();
                fonts = null;
            }
        }


        //---------------------------
        //for font shaping engine
        //--------------------------- 
        internal IntPtr HBFont
        {
            get { return this.hb_font; }
            set { this.hb_font = value; }
        }


        internal Drawing.Font GetFontAtPixelSize(int pixelSize)
        {
            Drawing.Font found;
            if (!fonts.TryGetValue(pixelSize, out found))
            {
                //----------------------------------
                //set current fontface size
                currentFacePixelSize = pixelSize;
                NativeMyFontsLib.MyFtSetPixelSizes(this.ftFaceHandle, pixelSize);
                //create font size
                NativeFont f = new NativeFont(this, pixelSize);
                fonts.Add(pixelSize, f);
                //------------------------------------
                return f;
            }
            return found;
        }
        internal Drawing.Font GetFontAtPointSize(float fontPointSize)
        {
            //convert from point size to pixelsize ***              
            return GetFontAtPixelSize(NativeFontStore.ConvertFromPointUnitToPixelUnit(fontPointSize));
        }

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