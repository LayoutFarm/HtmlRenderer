//MIT,2016, WinterDev
//----------------------------------- 
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;

namespace PixelFarm.DrawingGL
{
    /// <summary>
    /// cache texture font
    /// </summary>
    class TextureFontStore
    {
        Dictionary<FontKey, TextureFont> registerFonts = new Dictionary<FontKey, TextureFont>();
        public void RegisterFont(RequestFont f, TextureFont textureFont)
        {
            registerFonts.Add(f.FontKey, textureFont);
        }
        public TextureFont GetResolvedFont(RequestFont f)
        {
            TextureFont found;
            registerFonts.TryGetValue(f.FontKey, out found);
            return found;
        }
    }
}