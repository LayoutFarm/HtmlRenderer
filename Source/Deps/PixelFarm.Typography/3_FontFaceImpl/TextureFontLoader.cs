//MIT, 2014-2017, WinterDev 

using System.IO;
using Typography.OpenFont;
using Typography.Rendering;

namespace PixelFarm.Drawing.Fonts
{
    public static class TextureFontLoader
    {
        public static FontFace LoadFont(string fontfile,
            ScriptLang scriptLang,
            WriteDirection writeDirection,
             out SimpleFontAtlas fontAtlas)
        {

            //1. read font info
            ManagedFontFace openFont = (ManagedFontFace)OpenFontLoader.LoadFont(fontfile, scriptLang, writeDirection);

            //2. build texture font on the fly! OR load from prebuilt file
            //
            //2.1 test build texture on the fly
            SimpleFontAtlasBuilder atlas1 = CreateSampleMsdfTextureFont(fontfile, 14, 0, 255);
            GlyphImage glyphImg2 = atlas1.BuildSingleImage();
            fontAtlas = atlas1.CreateSimpleFontAtlas();
            fontAtlas.TotalGlyph = glyphImg2;

            //string xmlFontFileInfo = "";
            //GlyphImage glyphImg = null; 
            //MySimpleFontAtlasBuilder atlasBuilder = new MySimpleFontAtlasBuilder();
            //SimpleFontAtlas fontAtlas = atlasBuilder.LoadFontInfo(xmlFontFileInfo);
            //glyphImg = atlasBuilder.BuildSingleImage(); //we can create a new glyph or load from prebuilt file
            //fontAtlas.TotalGlyph = glyphImg; 


            var textureFontFace = new TextureFontFace(openFont, fontAtlas);
            return textureFontFace;
        }

        static SimpleFontAtlasBuilder CreateSampleMsdfTextureFont(string fontfile,
            float sizeInPoint,
            ushort startGlyphIndex, ushort endGlyphIndex)
        {

            //read type face from file
            Typeface typeface;
            using (var fs = new FileStream(fontfile, FileMode.Open, FileAccess.Read))
            {
                var reader = new OpenFontReader();
                //1. read typeface from font file
                typeface = reader.Read(fs);
            }
            //sample: create sample msdf texture 
            //-------------------------------------------------------------
            var builder = new GlyphPathBuilder(typeface);
            //builder.UseTrueTypeInterpreter = this.chkTrueTypeHint.Checked;
            //builder.UseVerticalHinting = this.chkVerticalHinting.Checked;
            //-------------------------------------------------------------
            var atlasBuilder = new SimpleFontAtlasBuilder();
            var msdfBuilder = new MsdfGlyphGen();

            for (ushort n = startGlyphIndex; n <= endGlyphIndex; ++n)
            {
                //build glyph
                builder.BuildFromGlyphIndex(n, sizeInPoint);

                var msdfGlyphGen = new MsdfGlyphGen();
                var actualImg = msdfGlyphGen.CreateMsdfImage(
                    builder.GetOutputPoints(),
                    builder.GetOutputContours(),
                    builder.GetPixelScale());
                atlasBuilder.AddGlyph((int)n, actualImg);

                //using (Bitmap bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                //{
                //    var bmpdata = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                //    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                //    bmp.UnlockBits(bmpdata);
                //    bmp.Save("d:\\WImageTest\\a001_xn2_" + n + ".png");
                //}
            }

            return atlasBuilder;
            //var glyphImg2 = atlasBuilder.BuildSingleImage();
            //using (Bitmap bmp = new Bitmap(glyphImg2.Width, glyphImg2.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            //{
            //    var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, glyphImg2.Width, glyphImg2.Height),
            //        System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            //    int[] intBuffer = glyphImg2.GetImageBuffer();

            //    System.Runtime.InteropServices.Marshal.Copy(intBuffer, 0, bmpdata.Scan0, intBuffer.Length);
            //    bmp.UnlockBits(bmpdata);
            //    bmp.Save("d:\\WImageTest\\a_total.png");
            //}
            //atlasBuilder.SaveFontInfo("d:\\WImageTest\\a_info.xml");

        }
    }


}