//MIT, 2014-2017, WinterDev 

using System.IO;
using Typography.OpenFont;

namespace PixelFarm.Drawing.Fonts
{
    public static class OpenFontLoader
    {
        public static FontFace LoadFont(string fontfile,
            ScriptLang scriptLang,
            WriteDirection writeDirection = WriteDirection.LTR)
        {
            //read font file
            OpenFontReader openTypeReader = new OpenFontReader();
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
            var openFont = new ManagedFontFace(typeface, typeface.Name, fontfile);
            return openFont;
        }
    }


}