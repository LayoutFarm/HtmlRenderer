//BSD, 2014-2016, WinterDev 

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using PixelFarm.DrawingGL;

namespace PixelFarm.Drawing.Fonts
{
    //cross-platform font mx***
    class GLES2PlatformFontMx
    {
        //gdiplus platform can handle following font       
        //1. gdi font
        //2. vector font
        //3. opentype font
        //4. texture font

        //public WinGdiPlusFont ResolveForWinGdiPlusFont(RequestFont r)
        //{
        //    WinGdiPlusFont winGdiPlusFont = r.ActualFont as WinGdiPlusFont;
        //    if (winGdiPlusFont != null)
        //    {
        //        return winGdiPlusFont;
        //    }
        //    //check if 
        //    throw new NotSupportedException();
        //}

        ////---------
        //static GdiPlusPlatformFontMx s_gdiPlusFontMx = new GdiPlusPlatformFontMx();
        //public static GdiPlusPlatformFontMx Default { get { return s_gdiPlusFontMx; } }

        public ActualFont ResolveForGdiFont(RequestFont font)
        {
            return null;
        }
        public ActualFont ResolveForTextureFont(RequestFont font)
        {
            //check if we have texture font fot this font 
            TextureFont t = font.ActualFont as TextureFont;
            if (t != null)
            {
                return t;
            }
            //--------------------------------------------------------------------
            LateTextureFontInfo lateFontInfo;
            if (!textureBitmapInfos.TryGetValue(font.Name, out lateFontInfo))
            {
                //not found
                return null;
            }
            //check if we have create TextureFont
            TextureFontFace textureFontface = lateFontInfo.Fontface;
            if (textureFontface == null)
            {
                //load glyh image here
                GlyphImage glyphImage = null;
                using (var nativeImg = new PixelFarm.Drawing.Imaging.NativeImage(lateFontInfo.TextureBitmapFile))
                {
                    glyphImage = new GlyphImage(nativeImg.Width, nativeImg.Height);
                    var buffer = new int[nativeImg.Width * nativeImg.Height];
                    System.Runtime.InteropServices.Marshal.Copy(nativeImg.GetNativeImageHandle(), buffer, 0, buffer.Length);
                    glyphImage.SetImageBuffer(buffer, true);
                }
                textureFontface = new TextureFontFace(font.Name, lateFontInfo.FontMapFile, glyphImage);
                lateFontInfo.Fontface = textureFontface;
                return textureFontface.GetFontAtPointsSize(font.SizeInPoints);
            }
            if (textureFontface != null)
            {
                return font.ActualFont = t = (TextureFont)(textureFontface.GetFontAtPointsSize(font.SizeInPoints));
            }
            else
            {
                //
                //need to create font face
                //create fontface first

            }
            return null;
        }


        static Dictionary<string, LateTextureFontInfo> textureBitmapInfos = new Dictionary<string, LateTextureFontInfo>();
        public static void AddTextureFontInfo(string fontname, string fontMapFile, string textureBitmapFile)
        {
            //add info for texture font
            textureBitmapInfos[fontname] = new LateTextureFontInfo(fontname, fontMapFile, textureBitmapFile);
        }


        static GLES2PlatformFontMx s_fontMx = new GLES2PlatformFontMx();

        public static GLES2PlatformFontMx Default { get { return s_fontMx; } }
        class LateTextureFontInfo
        {
            public LateTextureFontInfo(string fontName, string fontMapFile, string textureBitmapFile)
            {
                this.FontName = fontName;
                this.FontMapFile = fontMapFile;
                this.TextureBitmapFile = textureBitmapFile;
            }
            public string FontName { get; set; }
            public string FontMapFile { get; set; }
            public string TextureBitmapFile { get; set; }
            public TextureFontFace Fontface { get; set; }
        }


        static InstalledFontCollection installFonts;
        internal static string defaultLang = "en";
        internal static HBDirection defaultHbDirection = HBDirection.HB_DIRECTION_LTR;
        internal static int defaultScriptCode = 0;
        static bool s_didLoadFonts;
        public static void LoadInstalledFont(IInstalledFontProvider provider)
        {
            if (s_didLoadFonts)
            {
                return;
            }
            s_didLoadFonts = true;
            installFonts = new InstalledFontCollection();
            installFonts.LoadInstalledFont(provider.GetInstalledFontIter());
        }
        public static bool DidLoadFonts
        {
            get
            {
                return s_didLoadFonts;
            }
        }
        public static InstalledFont GetInstalledFont(string fontName, InstalledFontStyle style)
        {
            return installFonts.GetFont(fontName, style);
        }

    }

    /// <summary>
    /// store native font here
    /// </summary>
    class NativeFontStore
    {
        Dictionary<InstalledFont, FontFace> fonts = new Dictionary<InstalledFont, FontFace>();
        Dictionary<FontKey, ActualFont> registerFonts = new Dictionary<FontKey, ActualFont>();
        //--------------------------------------------------

        public NativeFontStore()
        {

        }
        public ActualFont LoadFont(string fontName, float fontSizeInPoints)
        {
            //find install font from fontname
            InstalledFont found = GLES2PlatformFontMx.GetInstalledFont(fontName, InstalledFontStyle.Regular);
            if (found == null)
            {
                return null;
            }

            FontFace fontFace;
            if (!fonts.TryGetValue(found, out fontFace))
            {
                fontFace = FreeTypeFontLoader.LoadFont(found,
                    GLES2PlatformFontMx.defaultLang,
                    GLES2PlatformFontMx.defaultHbDirection,
                    GLES2PlatformFontMx.defaultScriptCode);
                if (fontFace == null)
                {
                    throw new NotSupportedException();
                }
                fonts.Add(found, fontFace);//register
            }
            //-----------
            //create font at specific size from this fontface
            FontKey fontKey = new FontKey(fontName, fontSizeInPoints, FontStyle.Regular);
            ActualFont createdFont;
            if (!registerFonts.TryGetValue(fontKey, out createdFont))
            {
                createdFont = fontFace.GetFontAtPointsSize(fontSizeInPoints);
            }
            //-----------
            return createdFont;
        }

        public ActualFont GetResolvedNativeFont(RequestFont reqFont)
        {
            ActualFont found;
            registerFonts.TryGetValue(reqFont.FontKey, out found);
            return found;
        }
    }
}