// 2015,2014 ,MIT, WinterDev
//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//-----------------------------------

using System.Collections.Generic;
namespace PixelFarm.Agg.Fonts
{
    public static class SvgFontStore
    {
        public const string DEFAULT_SVG_FONTNAME = "svg-LiberationSansFont";
        static Dictionary<string, SvgFontFace> fontFaces = new Dictionary<string, SvgFontFace>();
        internal static void SetShapingEngine(SvgFontFace fontFace, string lang, HBDirection hb_direction, int hb_scriptcode)
        {
            ////string lang = "en";
            ////PixelFarm.Font2.NativeMyFontsLib.MyFtSetupShapingEngine(ftFaceHandle,
            ////    lang,
            ////    lang.Length,
            ////    HBDirection.HB_DIRECTION_LTR,
            ////    HBScriptCode.HB_SCRIPT_LATIN); 
            //ExportTypeFaceInfo exportTypeInfo = new ExportTypeFaceInfo();
            //NativeMyFontsLib.MyFtSetupShapingEngine(fontFace.Handle,
            //    lang,
            //    lang.Length,
            //    hb_direction,
            //    hb_scriptcode,
            //    ref exportTypeInfo);
            //fontFace.HBFont = exportTypeInfo.hb_font;
        }

        public static Font LoadFont(string filename, int fontPointSize)
        {
            //load font from specific file 
            SvgFontFace fontFace;
            if (!fontFaces.TryGetValue(filename, out fontFace))
            {
                //temp ....
                //all svg font remap to DEFAULT_SVG_FONTNAME
                //TODO: add more svg font
                if (filename != DEFAULT_SVG_FONTNAME)
                {
                    filename = DEFAULT_SVG_FONTNAME;
                }
                //----------------------------------------
                if (filename == DEFAULT_SVG_FONTNAME)
                {
                    fontFaces.Add(filename, fontFace = SvgFontFace_LiberationSans.Instance);
                }
                else
                {
                    //use default?,  svg-liberation san fonts

                }
            }
            if (fontFace == null)
            {
                return null;
            }



            return fontFace.GetFontAtSpecificSize(fontPointSize);
        }


        //---------------------------------------------------
        //helper function
        public static int ConvertFromPointUnitToPixelUnit(float point)
        {
            //from FreeType Documenetation
            //pixel_size = (pointsize * (resolution/72);
            return (int)(point * 96 / 72);
        }
    }
}