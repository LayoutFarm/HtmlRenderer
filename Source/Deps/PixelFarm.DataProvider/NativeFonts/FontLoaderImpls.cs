//MIT, 2014-2016, WinterDev
//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//-----------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NOpenType;
using System.IO;
namespace PixelFarm.Drawing.Fonts
{
    public static class FreeTypeFontLoader
    {
        static FreeTypeFontLoader()
        {
#if DEBUG
            int major, minor, revision;
            NativeMyFontsLib.MyFtLibGetFullVersion(out major, out minor, out revision);
#endif
        }
        public static FontFace LoadFont(string fontfile, string lang, HBDirection direction, int defaultScriptCode = 0)
        {
            return LoadFont(FontPreview.GetFontDetails(fontfile), lang, direction, defaultScriptCode);
        }
        public static FontFace LoadFont(InstalledFont installedFont, string lang, HBDirection direction, int defaultScriptCode = 0)
        {

            NativeFontFace fontFace = LoadFreeTypeFontFace(installedFont);
            SetShapingEngine(fontFace, lang, direction, defaultScriptCode);
            return fontFace;
        }
        static NativeFontFace LoadFreeTypeFontFace(InstalledFont installedFont)
        {
            //if not found
            //then load it
            byte[] fontFileContent = File.ReadAllBytes(installedFont.FontPath);
            int filelen = fontFileContent.Length;
            IntPtr unmanagedMem = Marshal.AllocHGlobal(filelen);
            Marshal.Copy(fontFileContent, 0, unmanagedMem, filelen);
            //---------------------------------------------------
            //convert font point size to pixel size 
            //---------------------------------------------------
            //load font from memory
            IntPtr faceHandle = NativeMyFontsLib.MyFtNewMemoryFace(unmanagedMem, filelen);
            if (faceHandle == IntPtr.Zero)
            {
                //error
                //load font error
                Marshal.FreeHGlobal(unmanagedMem);
                return null;
            }
            //------------------------------------------------------- 
            NativeFontFace fontFace = new NativeFontFace(unmanagedMem, faceHandle, installedFont.FontName,
                installedFont.FontPath, FontStyle.Regular);
            fontFace.LoadFromFilename = installedFont.FontPath;
            ExportTypeFaceInfo exportTypeInfo = new ExportTypeFaceInfo();
            NativeMyFontsLib.MyFtGetFaceInfo(faceHandle, ref exportTypeInfo);
            fontFace.HasKerning = exportTypeInfo.hasKerning;
            //------------------------------------------------------- 
            return fontFace;
        }

        //static ActualFont LoadFont(InstalledFont installedFont,
        //    float sizeInPoints,
        //    string lang,
        //    HBDirection direction,
        //    int scriptCode)
        //{
        //    //load font from specific file  

        //    NativeFontFace fontFace;
        //    //if not found
        //    //then load it
        //    byte[] fontFileContent = File.ReadAllBytes(installedFont.FontPath);
        //    int filelen = fontFileContent.Length;
        //    IntPtr unmanagedMem = Marshal.AllocHGlobal(filelen);
        //    Marshal.Copy(fontFileContent, 0, unmanagedMem, filelen);
        //    //---------------------------------------------------
        //    //convert font point size to pixel size 
        //    //---------------------------------------------------
        //    //load font from memory
        //    IntPtr faceHandle = NativeMyFontsLib.MyFtNewMemoryFace(unmanagedMem, filelen);
        //    if (faceHandle != IntPtr.Zero)
        //    {

        //        //ok pass 
        //        //-------------------
        //        //test change font size
        //        //NativeMyFontsLib.MyFtSetCharSize(faceHandle,
        //        //    0, //char_width in 1/64th of points, value 0 => same as height
        //        //    16 * 64,//16 pt //char_height in 1*64 of ppoints
        //        //    96,//horizontal device resolution (eg screen resolution 96 dpi)
        //        //    96);// vertical device resolution  
        //        //------------------- 
        //        //TODO: review here
        //        fontFace = new NativeFontFace(unmanagedMem, faceHandle, installedFont.FontName, FontStyle.Regular);
        //        fontFace.LoadFromFilename = installedFont.FontPath;
        //        ExportTypeFaceInfo exportTypeInfo = new ExportTypeFaceInfo();
        //        NativeMyFontsLib.MyFtGetFaceInfo(faceHandle, ref exportTypeInfo);
        //        fontFace.HasKerning = exportTypeInfo.hasKerning;
        //        //for shaping engine***
        //        //SetShapingEngine(fontFace,
        //        //    lang,
        //        //    defaultHbDirection,
        //        //    defaultScriptCode);

        //        //------------------- 
        //        //uint glyphIndex1;
        //        //int char1 = NativeMyFontsLib.MyFtGetFirstChar(faceHandle, out glyphIndex1);
        //        //List<CharAndGlyphMap> charMap = new List<CharAndGlyphMap>(); 
        //        //while (char1 != 0)
        //        //{
        //        //    char c = (char)char1;
        //        //    charMap.Add(new CharAndGlyphMap(glyphIndex1, c));
        //        //    char1 = NativeMyFontsLib.MyFtGetNextChar(faceHandle, char1, out glyphIndex1);
        //        //}
        //        //------------------- 

        //        //load glyph map
        //    }
        //    else
        //    {
        //        //load font error
        //        Marshal.FreeHGlobal(unmanagedMem);
        //    }

        //    //-------------------------------------------------
        //    //get font that specific size from found font face
        //    //-------------------------------------------------
        //    NativeFont nativeFont = fontFace.GetFontAtPointSize(sizeInPoints);
        //    //TODO: review here again, not hard code
        //    var fontKey = new FontKey(installedFont.FontName, sizeInPoints, FontStyle.Regular);
        //    registerFonts.Add(fontKey, nativeFont);
        //    return nativeFont;
        //}
        static void SetShapingEngine(NativeFontFace fontFace, string lang, HBDirection hb_direction, int hb_scriptcode)
        {
            ExportTypeFaceInfo exportTypeInfo = new ExportTypeFaceInfo();
            NativeMyFontsLib.MyFtSetupShapingEngine(fontFace.Handle,
               lang,
               lang.Length,
               hb_direction,
               hb_scriptcode,
               ref exportTypeInfo);
            fontFace.HBFont = exportTypeInfo.hb_font;
        }
    }

    public static class NOpenTypeFontLoader
    {
        public static FontFace LoadFont(string fontfile, string lang, HBDirection direction, int defaultScriptCode = 0)
        {
            //read font file
            OpenTypeReader openTypeReader = new OpenTypeReader();
            Typeface typeface = null;
            using (FileStream fs = new FileStream(fontfile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                typeface = openTypeReader.Read(fs);
                if (typeface == null)
                {
                    return null;
                }
            }
            //TODO:...
            //set shape engine *** 
            return new NOpenTypeFontFace(typeface, typeface.Name, fontfile);
        }
    }




    //    /// <summary>
    //    /// load font use myft lib,
    //    /// </summary>
    //    public class NativeFontStore
    //    {
    //        Dictionary<InstalledFont, NativeFontFace> fonts = new Dictionary<InstalledFont, NativeFontFace>();
    //        Dictionary<FontKey, NativeFont> registerFonts = new Dictionary<FontKey, NativeFont>();
    //        //--------------------------------------------------
    //        InstalledFontCollection installFonts;
    //        string defaultLang = "en";
    //        HBDirection defaultHbDirection = HBDirection.HB_DIRECTION_LTR;
    //        int defaultScriptCode = 0;
    //        public NativeFontStore()
    //        {
    //#if DEBUG
    //            int major, minor, revision;
    //            NativeMyFontsLib.MyFtLibGetFullVersion(out major, out minor, out revision);
    //#endif

    //        }
    //        public void LoadFonts(IInstalledFontProvider provider)
    //        {
    //            installFonts = new InstalledFontCollection();
    //            installFonts.LoadInstalledFont(provider.GetInstalledFontIter());
    //        }
    //        static void SetShapingEngine(NativeFontFace fontFace, string lang, HBDirection hb_direction, int hb_scriptcode)
    //        {
    //            ExportTypeFaceInfo exportTypeInfo = new ExportTypeFaceInfo();
    //            NativeMyFontsLib.MyFtSetupShapingEngine(fontFace.Handle,
    //               lang,
    //               lang.Length,
    //               hb_direction,
    //               hb_scriptcode,
    //               ref exportTypeInfo);
    //            fontFace.HBFont = exportTypeInfo.hb_font;
    //        }
    //        public ActualFont LoadFont(string fontName, string filename, float fontSizeInPoint)
    //        {
    //            InstalledFont found = installFonts.LoadFont(fontName, InstalledFontStyle.Regular);
    //            if (found == null)
    //            {
    //                return null;
    //            }
    //            return LoadFont(found, fontSizeInPoint);
    //        }
    //        public ActualFont LoadFont(string fontName, float fontSizeInPoint)
    //        {
    //            //find if we have this font
    //            InstalledFont found = installFonts.LoadFont(fontName, InstalledFontStyle.Regular);
    //            if (found == null)
    //            {
    //                return null;
    //            }
    //            //-----------------
    //            return LoadFont(found, fontSizeInPoint);
    //        }

    //        ActualFont LoadFont(InstalledFont installedFont, float sizeInPoints)
    //        {
    //            //load font from specific file  

    //            NativeFontFace fontFace;
    //            if (!fonts.TryGetValue(installedFont, out fontFace))
    //            {
    //                //if not found
    //                //then load it
    //                byte[] fontFileContent = File.ReadAllBytes(installedFont.FontPath);
    //                int filelen = fontFileContent.Length;
    //                IntPtr unmanagedMem = Marshal.AllocHGlobal(filelen);
    //                Marshal.Copy(fontFileContent, 0, unmanagedMem, filelen);
    //                //---------------------------------------------------
    //                //convert font point size to pixel size 
    //                //---------------------------------------------------
    //                //load font from memory
    //                IntPtr faceHandle = NativeMyFontsLib.MyFtNewMemoryFace(unmanagedMem, filelen);
    //                if (faceHandle != IntPtr.Zero)
    //                {

    //                    //ok pass 
    //                    //-------------------
    //                    //test change font size
    //                    //NativeMyFontsLib.MyFtSetCharSize(faceHandle,
    //                    //    0, //char_width in 1/64th of points, value 0 => same as height
    //                    //    16 * 64,//16 pt //char_height in 1*64 of ppoints
    //                    //    96,//horizontal device resolution (eg screen resolution 96 dpi)
    //                    //    96);// vertical device resolution  
    //                    //------------------- 
    //                    fontFace = new NativeFontFace(unmanagedMem, faceHandle, installedFont.FontName, FontStyle.Regular);
    //                    fontFace.LoadFromFilename = installedFont.FontPath;
    //                    ExportTypeFaceInfo exportTypeInfo = new ExportTypeFaceInfo();
    //                    NativeMyFontsLib.MyFtGetFaceInfo(faceHandle, ref exportTypeInfo);
    //                    fontFace.HasKerning = exportTypeInfo.hasKerning;
    //                    //for shaping engine***
    //                    SetShapingEngine(fontFace,
    //                        defaultLang,
    //                        defaultHbDirection,
    //                        defaultScriptCode);
    //                    fonts.Add(installedFont, fontFace);
    //                    //------------------- 
    //                    //uint glyphIndex1;
    //                    //int char1 = NativeMyFontsLib.MyFtGetFirstChar(faceHandle, out glyphIndex1);
    //                    //List<CharAndGlyphMap> charMap = new List<CharAndGlyphMap>(); 
    //                    //while (char1 != 0)
    //                    //{
    //                    //    char c = (char)char1;
    //                    //    charMap.Add(new CharAndGlyphMap(glyphIndex1, c));
    //                    //    char1 = NativeMyFontsLib.MyFtGetNextChar(faceHandle, char1, out glyphIndex1);
    //                    //}
    //                    //------------------- 

    //                    //load glyph map
    //                }
    //                else
    //                {
    //                    //load font error
    //                    Marshal.FreeHGlobal(unmanagedMem);
    //                }
    //            }
    //            //-------------------------------------------------
    //            //get font that specific size from found font face
    //            //-------------------------------------------------
    //            NativeFont nativeFont = fontFace.GetFontAtPointSize(sizeInPoints);
    //            //TODO: review here again, not hard code
    //            var fontKey = new FontKey(installedFont.FontName, sizeInPoints, FontStyle.Regular);
    //            registerFonts.Add(fontKey, nativeFont);
    //            return nativeFont;
    //        }
    //        //public NativeFont GetResolvedNativeFont(FontKey fontKey)
    //        //{
    //        //    NativeFont found;
    //        //    registerFonts.TryGetValue(fontKey, out found);
    //        //    return found;
    //        //}
    //        public NativeFont GetResolvedNativeFont(RequestFont reqFont)
    //        {
    //            NativeFont found;
    //            registerFonts.TryGetValue(reqFont.FontKey, out found);
    //            return found;
    //        }
    //    }


}
