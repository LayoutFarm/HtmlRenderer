////2016 MIT, WinterDev
//using System.Collections.Generic;
//using PixelFarm.Agg;
//using PixelFarm.Drawing;
//using PixelFarm.Drawing.Fonts;
//using PixelFarm.Drawing.Text;

//namespace PixelFarm.DrawingGL
//{
//    class TextureFontStore
//    {
//        Dictionary<FontKey, TextureFont> registerFonts = new Dictionary<FontKey, TextureFont>();
//        public void RegisterFont(RequestFont f, TextureFont textureFont)
//        {
//            registerFonts.Add(f.FontKey, textureFont);
//        }
//        public TextureFont GetResolvedFont(RequestFont f)
//        {
//            TextureFont found;
//            registerFonts.TryGetValue(f.FontKey, out found);
//            return found;
//        }
//    }

//}