//BSD, 2014-2017, WinterDev 

using System;
using System.Collections.Generic;


namespace PixelFarm.Drawing.Fonts
{
    //cross-platform font mx***
    class GLES2PlatformFontMx
    {

        //static InstalledFontCollection installFonts;
        internal static ScriptLang defaultScriptLang = ScriptLangs.Latin;
        internal static WriteDirection defaultHbDirection = WriteDirection.LTR;
         
        static IFontLoader s_fontLoader;
        public static void SetFontLoader(IFontLoader fontLoader)
        {
            s_fontLoader = fontLoader;
        }
        public static InstalledFont GetInstalledFont(string fontName, InstalledFontStyle style)
        {
            return s_fontLoader.GetFont(fontName, style);
        }
        /////////////////////////////////////////
        //

        //public ActualFont ResolveForTextureFont(RequestFont font)
        //{

        //    return null;

        //    ////check if we have texture font fot this font 
        //    //TextureFont t = TextureFont.GetCacheFontAsTextureFont(font);
        //    //if (t != null)
        //    //{
        //    //    return t;
        //    //}
        //    ////--------------------------------------------------------------------
        //    //LateTextureFontInfo lateFontInfo;
        //    //if (!textureBitmapInfos.TryGetValue(font.Name, out lateFontInfo))
        //    //{
        //    //    //not found
        //    //    return null;
        //    //}
        //    ////check if we have create TextureFont
        //    //TextureFontFace textureFontface = lateFontInfo.Fontface;
        //    //if (textureFontface == null)
        //    //{
        //    //    throw new System.NotSupportedException();
        //    //    ////load glyh image here
        //    //    //GlyphImage glyphImage = null;
        //    //    //using (var nativeImg = new PixelFarm.Drawing.Imaging.NativeImage(lateFontInfo.TextureBitmapFile))
        //    //    //{
        //    //    //    glyphImage = new GlyphImage(nativeImg.Width, nativeImg.Height);
        //    //    //    var buffer = new int[nativeImg.Width * nativeImg.Height];
        //    //    //    System.Runtime.InteropServices.Marshal.Copy(nativeImg.GetNativeImageHandle(), buffer, 0, buffer.Length);
        //    //    //    glyphImage.SetImageBuffer(buffer, true);
        //    //    //}

        //    //    //InstalledFont installedFont = GLES2PlatformFontMx.GetInstalledFont(font.Name, InstalledFontStyle.Regular);
        //    //    //FontFace nOpenTypeFontFace = OpenFontLoader.LoadFont(installedFont.FontPath, GLES2PlatformFontMx.defaultScriptLang);


        //    //    //textureFontface = new TextureFontFace(nOpenTypeFontFace, lateFontInfo.FontMapFile, glyphImage);
        //    //    //lateFontInfo.Fontface = textureFontface;
        //    //    //return textureFontface.GetFontAtPointsSize(font.SizeInPoints);
        //    //}
        //    //if (textureFontface != null)
        //    //{
        //    //    t = (TextureFont)(textureFontface.GetFontAtPointsSize(font.SizeInPoints));
        //    //    t.AssignToRequestFont(font);
        //    //    return t;
        //    //}
        //    //else
        //    //{
        //    //    //
        //    //    //need to create font face
        //    //    //create fontface first

        //    //}
        //    return null;
        //}


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
            //  public TextureFontFace Fontface { get; set; }
        }



    }



}