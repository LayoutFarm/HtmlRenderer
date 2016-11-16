//MIT, 2014-2016, WinterDev 
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NOpenType;
using System.IO;
using PixelFarm.Agg;
namespace PixelFarm.Drawing.Fonts
{
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

}