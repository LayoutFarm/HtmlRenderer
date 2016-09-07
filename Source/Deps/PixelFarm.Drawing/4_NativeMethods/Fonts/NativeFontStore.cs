//MIT, 2014-2016, WinterDev
//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//-----------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
namespace PixelFarm.Drawing.Fonts
{
    public class GlyphImage
    {
        int[] pixelBuffer;
        public GlyphImage(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
        public RectangleF OriginalGlyphBounds
        {
            get;
            internal set;
        }


        public int Width
        {
            get;
            private set;
        }
        public int Height
        {
            get;
            private set;
        }
        public bool IsBigEndian
        {
            get;
            private set;
        }

        public int BorderXY
        {
            get;
            internal set;
        }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public int[] GetImageBuffer()
        {
            return pixelBuffer;
        }
        public void SetImageBuffer(int[] pixelBuffer, bool isBigEndian)
        {
            this.pixelBuffer = pixelBuffer;
            this.IsBigEndian = isBigEndian;
        }
    }
    public static class NativeFontStore
    {
        static Dictionary<string, NativeFontFace> fonts = new Dictionary<string, NativeFontFace>();
        internal static void SetShapingEngine(NativeFontFace fontFace, string lang, HBDirection hb_direction, int hb_scriptcode)
        {
            //string lang = "en";
            //PixelFarm.Font2.NativeMyFontsLib.MyFtSetupShapingEngine(ftFaceHandle,
            //    lang,
            //    lang.Length,
            //    HBDirection.HB_DIRECTION_LTR,
            //    HBScriptCode.HB_SCRIPT_LATIN); 
            ExportTypeFaceInfo exportTypeInfo = new ExportTypeFaceInfo();
            NativeMyFontsLib.MyFtSetupShapingEngine(fontFace.Handle,
               lang,
               lang.Length,
               hb_direction,
               hb_scriptcode,
               ref exportTypeInfo);
            fontFace.HBFont = exportTypeInfo.hb_font;
        }

        public static Font LoadFont(string filename, int fontPointSize)
        {
            //load font from specific file 
            NativeFontFace fontFace;
            if (!fonts.TryGetValue(filename, out fontFace))
            {
                //if not found
                //then load it
                byte[] fontFileContent = File.ReadAllBytes(filename);
                int filelen = fontFileContent.Length;
                IntPtr unmanagedMem = Marshal.AllocHGlobal(filelen);
                Marshal.Copy(fontFileContent, 0, unmanagedMem, filelen);
                //---------------------------------------------------
                //convert font point size to pixel size 
                //---------------------------------------------------
                IntPtr faceHandle = NativeMyFontsLib.MyFtNewMemoryFace(unmanagedMem, filelen);
                if (faceHandle != IntPtr.Zero)
                {
                    //ok pass 
                    //-------------------
                    //test change font size
                    //NativeMyFontsLib.MyFtSetCharSize(faceHandle,
                    //    0, //char_width in 1/64th of points, value 0 => same as height
                    //    16 * 64,//16 pt //char_height in 1*64 of ppoints
                    //    96,//horizontal device resolution (eg screen resolution 96 dpi)
                    //    96);// vertical device resolution 

                    //------------------- 
                    fontFace = new NativeFontFace(unmanagedMem, faceHandle);
                    ExportTypeFaceInfo exportTypeInfo = new ExportTypeFaceInfo();
                    NativeMyFontsLib.MyFtGetFaceInfo(faceHandle, ref exportTypeInfo);
                    fontFace.HasKerning = exportTypeInfo.hasKerning;
                    //for shaping engine***
                    SetShapingEngine(fontFace, "th", HBDirection.HB_DIRECTION_LTR, HBScriptCode.HB_SCRIPT_THAI);
                    fonts.Add(filename, fontFace);
                }
                else
                {
                    //load font error
                    Marshal.FreeHGlobal(unmanagedMem);
                }
            }
            //-------------------------------------------------
            //get font that specific size from found font face
            //-------------------------------------------------

            return fontFace.GetFontAtPointSize(fontPointSize);
        }


        //---------------------------------------------------
        //helper function
        public static int ConvertFromPointUnitToPixelUnit(float point)
        {
            //from FreeType Documenetation
            //pixel_size = (pointsize * (resolution/72);
            return (int)(point * 96f / 72f);
        }


        public static GlyphImage BuildMsdfFontImage(FontGlyph fontGlyph)
        {
            return NativeFontGlyphBuilder.BuildMsdfFontImage(fontGlyph);
        }
        public static void SwapColorComponentFromBigEndianToWinGdi(int[] bitbuffer)
        {
            unsafe
            {
                int j = bitbuffer.Length;
                fixed (int* p0 = &(bitbuffer[j - 1]))
                {
                    int* p = p0;
                    for (int i = j - 1; i >= 0; --i)
                    {
                        int color = *p;
                        int a = color >> 24;
                        int b = (color >> 16) & 0xff;
                        int g = (color >> 8) & 0xff;
                        int r = color & 0xff;
                        *p = (a << 24) | (r << 16) | (g << 8) | b;
                        p--;
                    }
                }
            }
        }
    }
}
