//MIT, 2016-2017, WinterDev
using System;
using System.Collections.Generic;

namespace PixelFarm.Drawing.Fonts
{
    public interface IInstalledFontProvider
    {
        IEnumerable<string> GetInstalledFontIter();
    }
    public interface IFontLoader
    {
        InstalledFont GetFont(string fontName, InstalledFontStyle style);
    }

    public class InstalledFont
    {

        public InstalledFont(string fontName, string fontSubFamily, string fontPath)
        {
            FontName = fontName;
            FontSubFamily = fontSubFamily;
            FontPath = fontPath;
        }
        public string FontName { get; set; }
        public string FontSubFamily { get; set; }
        public string FontPath { get; set; }

#if DEBUG
        public override string ToString()
        {
            return FontName;
        }
#endif
    }

    [Flags]
    public enum InstalledFontStyle
    {
        Regular,
        Bold = 1 << 1,
        Italic = 1 << 2,
    }

    public delegate InstalledFont FontNotFoundHandler(InstalledFontCollection fontCollection, string fontName, InstalledFontStyle style);

    public class InstalledFontCollection
    {

        Dictionary<string, InstalledFont> regular_Fonts = new Dictionary<string, InstalledFont>();
        Dictionary<string, InstalledFont> bold_Fonts = new Dictionary<string, InstalledFont>();
        Dictionary<string, InstalledFont> italic_Fonts = new Dictionary<string, InstalledFont>();
        Dictionary<string, InstalledFont> boldItalic_Fonts = new Dictionary<string, InstalledFont>();
        Dictionary<string, InstalledFont> gras_Fonts = new Dictionary<string, InstalledFont>();
        Dictionary<string, InstalledFont> grasItalic_Fonts = new Dictionary<string, InstalledFont>();

        List<InstalledFont> installedFonts;
        FontNotFoundHandler fontNotFoundHandler;

        public InstalledFontCollection()
        {

        }
        public void SetFontNotFoundHandler(FontNotFoundHandler handler)
        {
            fontNotFoundHandler = handler;
        }
        public void LoadInstalledFont(IEnumerable<string> getFontFileIter)
        {
            installedFonts = ReadPreviewFontData(getFontFileIter);
            //classify
            //do 
            int j = installedFonts.Count;
            for (int i = 0; i < j; ++i)
            {
                InstalledFont f = installedFonts[i];
                if (f == null || f.FontName == "" || f.FontName.StartsWith("\0"))
                {
                    //no font name?
                    continue;
                }
                switch (f.FontSubFamily)
                {
                    case "Normal":
                    case "Regular":
                        {
                            regular_Fonts.Add(f.FontName.ToUpper(), f);
                        }
                        break;
                    case "Italic":
                    case "Italique":
                        {
                            italic_Fonts.Add(f.FontName.ToUpper(), f);
                        }
                        break;
                    case "Bold":
                        {
                            bold_Fonts.Add(f.FontName.ToUpper(), f);
                        }
                        break;
                    case "Bold Italic":
                        {
                            boldItalic_Fonts.Add(f.FontName.ToUpper(), f);
                        }
                        break;
                    case "Gras":
                        {
                            gras_Fonts.Add(f.FontName.ToUpper(), f);
                        }
                        break;
                    case "Gras Italique":
                        {
                            grasItalic_Fonts.Add(f.FontName.ToUpper(), f);
                        }
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

        }

        public InstalledFont GetFont(string fontName, InstalledFontStyle style)
        {
            //request font from installed font
            InstalledFont found;
            switch (style)
            {
                case (InstalledFontStyle.Bold | InstalledFontStyle.Italic):
                    {
                        //check if we have bold & italic 
                        //version of this font ?  
                        if (!boldItalic_Fonts.TryGetValue(fontName.ToUpper(), out found))
                        {
                            //if not found then goto italic 
                            goto case InstalledFontStyle.Italic;
                        }
                        return found;
                    }
                case InstalledFontStyle.Bold:
                    {

                        if (!bold_Fonts.TryGetValue(fontName.ToUpper(), out found))
                        {
                            //goto regular
                            goto default;
                        }
                        return found;
                    }
                case InstalledFontStyle.Italic:
                    {
                        //if not found then choose regular
                        if (!italic_Fonts.TryGetValue(fontName.ToUpper(), out found))
                        {
                            goto default;
                        }
                        return found;
                    }
                default:
                    {
                        //we skip gras style ?
                        if (!regular_Fonts.TryGetValue(fontName.ToUpper(), out found))
                        {

                            if (fontNotFoundHandler != null)
                            {
                                return fontNotFoundHandler(
                                    this,
                                    fontName,
                                    style);
                            }
                            return null;
                        }
                        return found;
                    }
            }
        }
        public static List<InstalledFont> ReadPreviewFontData(IEnumerable<string> getFontFileIter)
        {
            //-------------------------------------------------
            //TODO: review here, this is not platform depend
            //-------------------------------------------------
            //check if MAC or linux font folder too
            //-------------------------------------------------

            List<InstalledFont> installedFonts = new List<InstalledFont>();

            foreach (string fontFilename in getFontFileIter)
            {
                InstalledFont installedFont = FontPreview.GetFontDetails(fontFilename);
                installedFonts.Add(installedFont);
            }
            return installedFonts;
        }
    }


    public static class FontStyleExtensions
    {
        public static InstalledFontStyle ConvToInstalledFontStyle(this FontStyle style)
        {
            InstalledFontStyle installedStyle = InstalledFontStyle.Regular;//regular
            switch (style)
            {
                default: break;
                case FontStyle.Bold:
                    installedStyle = InstalledFontStyle.Bold;
                    break;
                case FontStyle.Italic:
                    installedStyle = InstalledFontStyle.Italic;
                    break;
                case FontStyle.Bold | FontStyle.Italic:
                    installedStyle = InstalledFontStyle.Italic;
                    break;
            }
            return installedStyle;
        }
    }

}