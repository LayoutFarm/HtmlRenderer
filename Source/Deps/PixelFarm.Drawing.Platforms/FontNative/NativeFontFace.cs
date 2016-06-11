// 2015,2014 ,MIT, WinterDev
//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//----------------------------------- 

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace PixelFarm.Agg.Fonts
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
        Dictionary<int, Font> fonts = new Dictionary<int, Font>();
        IntPtr hb_font;
        Font px64Font;
        internal NativeFontFace(IntPtr unmanagedMem, IntPtr ftFaceHandle)
        {
            this.unmanagedMem = unmanagedMem;
            this.ftFaceHandle = ftFaceHandle;
            //---------------------------------
            //for master font at 64px
            px64Font = GetFontAtPixelSize(64);
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


        internal Font GetFontAtPixelSize(int pixelSize)
        {
            Font found;
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
        internal Font GetFontAtPointSize(float fontPointSize)
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
            unsafe
            {
                ExportGlyph exportTypeFace = new ExportGlyph();
                NativeMyFontsLib.MyFtLoadGlyph(ftFaceHandle, glyphIndex, ref exportTypeFace);
                FontGlyph fontGlyph = new FontGlyph();
                BuildGlyph(fontGlyph, &exportTypeFace, pixelSize);
                return fontGlyph;
            }
        }
        internal FontGlyph ReloadGlyphFromChar(char unicodeChar, int pixelSize)
        {
            if (currentFacePixelSize != pixelSize)
            {
                currentFacePixelSize = pixelSize;
                NativeMyFontsLib.MyFtSetPixelSizes(this.ftFaceHandle, pixelSize);
            }
            //--------------------------------------------------
            unsafe
            {
                ExportGlyph exportTypeFace = new ExportGlyph();
                NativeMyFontsLib.MyFtLoadChar(ftFaceHandle, unicodeChar, ref exportTypeFace);
                FontGlyph fontGlyph = new FontGlyph();
                BuildGlyph(fontGlyph, &exportTypeFace, pixelSize);
                return fontGlyph;
            }
        }

        unsafe void BuildGlyph(FontGlyph fontGlyph, ExportGlyph* exportTypeFace, int pxsize)
        {
            //------------------------------------------
            //copy font metrics
            fontGlyph.exportGlyph = *(exportTypeFace);
            //------------------------------------------
            //copy raw image 
            NativeFontGlyphBuilder.CopyGlyphBitmap(fontGlyph, exportTypeFace);
            //outline version
            //------------------------------------------
            if (px64Font != null)
            {
                if (pxsize < 64)
                {
                    NativeFontGlyphBuilder.BuildGlyphOutline(fontGlyph, exportTypeFace);
                }
                else
                {
                    NativeFontGlyphBuilder.BuildGlyphOutline(fontGlyph, exportTypeFace);
                }
            }
            else
            {
                NativeFontGlyphBuilder.BuildGlyphOutline(fontGlyph, exportTypeFace);
            }
        }
    }
}