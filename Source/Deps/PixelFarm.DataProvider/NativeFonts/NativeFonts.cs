//MIT, 2014-2016, WinterDev

//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//-----------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
namespace PixelFarm.Drawing.Fonts
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ExportTypeFaceInfo
    {
        public bool hasKerning;
        public IntPtr hb_font;
    }


    static class NativeMyFontsLib
    {
        const string myfontLib = NativeDLL.MyFtLibName;
        static object syncObj = new object();
        static bool isInitLib = false;
        static NativeModuleHolder nativeModuleHolder;
        static NativeMyFontsLib()
        {

            //dynamic load dll from current directory??
            string appBaseDir = AppDomain.CurrentDomain.BaseDirectory;
            LoadLib(appBaseDir + "\\" + myfontLib);
            //---------------
            //init library
            int initResult = 0;
            lock (syncObj)
            {
                if (!isInitLib)
                {
                    initResult = NativeMyFontsLib.MyFtInitLib();
                    isInitLib = true;
                }
            }
            //---------------
            nativeModuleHolder = new NativeModuleHolder();
        }

        [DllImport(myfontLib)]
        public static extern int MyFtLibGetVersion();

        [DllImport(myfontLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern void MyFtLibGetFullVersion(out int major, out int minor, out int revision);
        
        [DllImport(myfontLib)]
        public static extern int MyFtInitLib();
        [DllImport(myfontLib)]
        public static extern void MyFtShutdownLib();
        [DllImport(myfontLib)]
        public static extern IntPtr MyFtNewMemoryFace(IntPtr membuffer, int memSizeInBytes);
        [DllImport(myfontLib)]
        public static extern void MyFtDoneFace(IntPtr faceHandle);
        [DllImport(myfontLib)]
        public static extern void MyFtGetFaceInfo(IntPtr faceHandle, ref ExportTypeFaceInfo exportTypeFaceInfo);
        [DllImport(myfontLib)]
        public static extern void MyFtSetPixelSizes(IntPtr myface, int pxsize);
        [DllImport(myfontLib)]
        public static extern void MyFtSetCharSize(IntPtr faceHandle, int char_width26_6,
            int char_height26_6,
            int h_device_resolution,
            int v_device_resolution);
        [DllImport(myfontLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MyFtLoadChar(IntPtr faceHandle, int charcode, out GlyphMatrix ftOutline);
        [DllImport(myfontLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MyFtLoadGlyph(IntPtr faceHandle, uint codepoint, out GlyphMatrix ftOutline);
        [DllImport(myfontLib, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void MyFtGetFaceData(IntPtr faceHandle, ExportFace* exportFace);

        //    MY_DLL_EXPORT int MyFtGetCharIndex(FT_Face myface, char charcode);
        //MY_DLL_EXPORT long MyFtGetFirstChar(FT_Face myface,  unsigned int* glyphIndex);
        //MY_DLL_EXPORT long MyFtGetNextChar(FT_Face myface,long charcode, unsigned int glyphIndex);

        [DllImport(myfontLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MyFtGetCharIndex(IntPtr faceHandle, char charcode);
        [DllImport(myfontLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MyFtGetFirstChar(IntPtr faceHandle, out uint glyIndex);
        [DllImport(myfontLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MyFtGetNextChar(IntPtr faceHandle, int charcode, out uint glyIndex);
        //============================================================================
        //HB shaping ....
        [DllImport(myfontLib, CharSet = CharSet.Ansi)]
        public static extern int MyFtSetupShapingEngine(IntPtr faceHandle, string langName,
            int langNameLen, HBDirection hbDirection, int hbScriptCode, ref ExportTypeFaceInfo exportTypeFaceInfo);
        [DllImport(myfontLib)]
        public static unsafe extern int MyFtShaping(IntPtr my_hb_ft_font,
            char* text,
            int charCount,
            ProperGlyph* properGlyphs);
        //============================================================================


        static bool isLoaded = false;
        static bool LoadLib(string dllFilename)
        {
            if (isLoaded)
            {
                return true;
            }
            if (!File.Exists(dllFilename))
            {
                //TODO review here
                //load from specific folder 
            }
            isLoaded = true;
            return true;
        }


        [DllImport(myfontLib)]
        public static extern void DeleteUnmanagedObj(IntPtr unmanagedObject);

        class NativeModuleHolder : IDisposable
        {
            ~NativeModuleHolder()
            {
                Dispose();
            }
            public void Dispose()
            {
                NativeMyFontsLib.MyFtShutdownLib();
            }
        }
    }





    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    unsafe struct ExportFace
    {
        public int ascender;
        public int descender;
        public int height;

        public int max_advance_width;
        public int max_advance_height;

        public int underline_position;
        public int underline_thickness;

        public int num_faces;
        public int face_index;

        public int face_flags;
        public int style_flags;

        public int num_glyphs;

        public char* family_name; //ascii
        public char* style_name; //ascii

        public FTBBox bbox;

        public ushort units_per_EM;

    };
    [StructLayout(LayoutKind.Sequential)]
    struct FTBBox
    {
        public int xMin, yMin, xMax, yMax;
    }

    public static class MyFtLib
    {
        const string MYFT = NativeDLL.MyFtLibName;
        [DllImport(MYFT, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MyFtMSDFGEN(int argc, string[] argv);
        [DllImport(MYFT, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DeleteUnmanagedObj(IntPtr unmanagedPtr);
        [DllImport(MYFT, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateShape();
        [DllImport(MYFT, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ShapeAddBlankContour(IntPtr shape);
        [DllImport(MYFT, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ShapeFindBounds(IntPtr shape,
         out double left, out double bottom,
         out double right, out double top);
        [DllImport(MYFT, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ContourAddLinearSegment(IntPtr cnt,
            double x0, double y0,
            double x1, double y1);
        [DllImport(MYFT, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ContourAddQuadraticSegment(IntPtr cnt,
            double x0, double y0,
            double ctrl0X, double ctrl0Y,
            double x1, double y1);
        [DllImport(MYFT, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ContourAddCubicSegment(IntPtr cnt,
            double x0, double y0,
            double ctrl0X, double ctrl0Y,
            double ctrl1X, double ctrl1Y,
            double x1, double y1);
        [DllImport(MYFT, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void MyFtGenerateMsdf(IntPtr shape, int width, int height, double range,
            double scale, double tx, double ty, double edgeThreshold, double angleThreshold, int* outputBitmap);
        [DllImport(MYFT)]
        public static extern bool ShapeValidate(IntPtr shape);
        [DllImport(MYFT)]
        public static extern void ShapeNormalize(IntPtr shape);
        [DllImport(MYFT)]
        public static extern void SetInverseYAxis(IntPtr shape, bool inverseYAxis);
        [DllImport(MYFT)]
        public static extern int MyFtLibGetVersion();
    }

    public class MsdfParameters
    {
        public string fontName;
        public bool useClassicSdf;
        public char character;
        public string outputFile;
        public int sizeW = 32;
        public int sizeH = 32;
        public int pixelRange = 4;
        public string testRenderFileName;
        public bool enableRenderTestFile = true;
        public MsdfParameters(string fontName, char character)
        {
            this.fontName = fontName;
            this.character = character;
        }
        public override string ToString()
        {
            string[] args = GetArgs();
            StringBuilder stbulder = new StringBuilder();
            stbulder.Append(args[0]);
            int j = args.Length;
            for (int i = 1; i < j; ++i)
            {
                stbulder.Append(' ');
                stbulder.Append(args[i]);
            }
            return stbulder.ToString();
        }
        public string[] GetArgs()
        {
            List<string> args = new List<string>();
            //0.
            args.Add("msdfgen");
            //1.
            string genMode = "msdf";
            if (useClassicSdf)
            {
                genMode = "sdf";
            }
            args.Add(genMode);
            //2.
            if (fontName == null) { throw new Exception(); }
            args.Add("-font"); args.Add(fontName);
            args.Add("0x" + ((int)character).ToString("X")); //accept unicode char
            //3.
            if (outputFile == null)
            {
                //use default
                outputFile = genMode + "_" + ((int)character).ToString() + ".png";
            }
            args.Add("-o"); args.Add(outputFile);
            //4.
            args.Add("-size"); args.Add(sizeW.ToString()); args.Add(sizeH.ToString());
            //5.
            args.Add("-pxrange"); args.Add(pixelRange.ToString());
            //6.
            args.Add("-autoframe");//default
            //7.
            if (enableRenderTestFile)
            {
                if (testRenderFileName == null)
                {
                    testRenderFileName = "test_" + genMode + "_" + character + ".png";
                }
                args.Add("-testrender"); args.Add(testRenderFileName);
                args.Add("1024");
                args.Add("1024");
            }
            return args.ToArray();
        }
    }
}