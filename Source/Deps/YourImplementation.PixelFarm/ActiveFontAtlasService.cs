//MIT, 2016-2017, WinterDev

using System;
using System.Collections.Generic;
//

using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;


namespace PixelFarm.DrawingGL
{
    static class ActiveFontAtlasService
    {
        struct FontTextureKey
        {
            public float sizeInPoint;
            public string fontName;
            public FontStyle fontStyle;
            public string scriptLang;
        }

        struct TextureAtlasCache
        {
            public FontFace fontFace;
            public SimpleFontAtlas atlas;
        }

        static Dictionary<FontTextureKey, TextureAtlasCache> s_cachedFontAtlas = new Dictionary<FontTextureKey, TextureAtlasCache>();

        public static ActualFont GetTextureFontAtlasOrCreateNew(
            IFontLoader fontLoader,
            RequestFont font,
            out SimpleFontAtlas fontAtlas)
        {
            //check if we have created this font
            var key = new FontTextureKey();
            key.fontName = font.Name;
            key.scriptLang = font.ScriptLang.shortname;
            key.sizeInPoint = font.SizeInPoints;
            key.fontStyle = font.Style;
            //------------------------
            TextureAtlasCache found;
            FontFace ff = null;
            if (!s_cachedFontAtlas.TryGetValue(key, out found))
            {
                //if not, then create the new one 
                string fontfile = fontLoader.GetFont(font.Name, font.Style.ConvToInstalledFontStyle()).FontPath;
                //ptimize here
                //TODO: review
                TextureFontCreationParams creationParams = new TextureFontCreationParams();
                creationParams.originalFontSizeInPoint = font.SizeInPoints;
                creationParams.scriptLang = font.ScriptLang;
                creationParams.writeDirection = WriteDirection.RTL;//default 
                //TODO: review here, langBits can be created with scriptLang ?
                creationParams.langBits = new Typography.OpenFont.Tables.UnicodeLangBits[]
                {
                    Typography.OpenFont.Tables.UnicodeLangBits.BasicLatin,     //0-127 
                    Typography.OpenFont.Tables.UnicodeLangBits.Thai //eg. Thai, for test with complex script, you can change to your own
                };
                //
                creationParams.textureKind = Typography.Rendering.TextureKind.AggSubPixel;
                if (font.SizeInPoints >= 4 && font.SizeInPoints <= 14)
                {
                    creationParams.hintTechnique = Typography.Rendering.HintTechnique.TrueTypeInstruction_VerticalOnly;
                }
                //
                ff = TextureFontLoader.LoadFont(fontfile, creationParams, out fontAtlas);


                //cache it 
                var textureAtlasCache = new TextureAtlasCache();
                textureAtlasCache.fontFace = ff;
                textureAtlasCache.atlas = fontAtlas;
                s_cachedFontAtlas.Add(key, textureAtlasCache);
                return ff.GetFontAtPointSize(font.SizeInPoints);
            }
            fontAtlas = found.atlas;
            ff = found.fontFace;
            return ff.GetFontAtPointSize(font.SizeInPoints);
        }

    }


}