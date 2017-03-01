////Apache2, 2014-2017, WinterDev
//#if GL_ENABLE
//using System;
//using PixelFarm.Drawing;
//using PixelFarm.Drawing.Fonts;
//namespace LayoutFarm.UI.OpenGL
//{
//    //class OpenGLGfxPlatform : GraphicsPlatform
//    //{
//    //    public override Canvas CreateCanvas(int left, int top, int width,
//    //        int height,
//    //        CanvasInitParameters canvasInitPars = default(CanvasInitParameters))
//    //    {
//    //        throw new NotImplementedException();
//    //    }
//    //    public static void SetFontEncoding(System.Text.Encoding encoding)
//    //    {
//    //        //WinGdiTextService.SetDefaultEncoding(encoding);
//    //    }
//    //    //public static void SetFontNotFoundHandler(FontNotFoundHandler fontNotFoundHandler)
//    //    //{
//    //    //    s_installFontCollection.SetFontNotFoundHandler(fontNotFoundHandler);
//    //    //}
//    //    //static InstalledFontCollection s_installFontCollection = new InstalledFontCollection();
//    //}
//    //public static class MyOpenGLPortal
//    //{
//    //    static OpenGLGfxPlatform _winGdiPlatform;
//    //    static bool isInit;
//    //    static GraphicsPlatform Start(MyWinGdiPortalSetupParameters initParams)
//    //    {
//    //        if (isInit)
//    //        {
//    //            return _winGdiPlatform;
//    //        }
//    //        isInit = true;

//    //        //text services:            
//    //        //TextServices.IFonts = initParams.TextServiceInstance ?? new GdiPlusIFonts();
//    //        //ActualFontResolver.Resolver = initParams.ActualFontResolver ?? new GdiFontResolver();
//    //        //set if we use pixelfarm's native myft.dll
//    //        //or use managed text break
//    //        //-------------------------------------
//    //        //if we use ICU text breaker
//    //        //1. load icu data
//    //        //if (initParams.IcuDataFile != null)
//    //        //{
//    //        //    //check icu file is exist 
//    //        //    //TODO: review  file/resource load mechanism again ***
//    //        //     NativeTextBreaker.SetICUDataFile(initParams.IcuDataFile);
//    //        //}
//    //        ////2. text breaker
//    //        //RootGraphic.SetTextBreakerGenerator(
//    //        //    initParams.TextBreakGenerator ??
//    //        //    (locale => new NativeTextBreaker(TextBreakKind.Word, locale))
//    //        //    );

//    //        //-------------------------------------
//    //        //config encoding
//    //        OpenGLGfxPlatform.SetFontEncoding(System.Text.Encoding.GetEncoding(874));
//    //        //-------------------------------------
//    //        OpenGLGfxPlatform.SetFontNotFoundHandler(
//    //            (fontCollection, fontName, style) =>
//    //            {
//    //                //TODO: implement font not found mapping here
//    //                //_fontsMapping["monospace"] = "Courier New";
//    //                //_fontsMapping["Helvetica"] = "Arial";
//    //                fontName = fontName.ToUpper();
//    //                switch (fontName)
//    //                {
//    //                    case "MONOSPACE":
//    //                        return fontCollection.GetFont("Courier New", style);
//    //                    case "HELVETICA":
//    //                        return fontCollection.GetFont("Arial", style);
//    //                    case "TAHOMA":
//    //                        //default font must found
//    //                        //if not throw err 
//    //                        //this prevent infinit loop
//    //                        throw new System.NotSupportedException();
//    //                    default:
//    //                        return fontCollection.GetFont("tahoma", style);
//    //                }
//    //            });
//    //        _winGdiPlatform = new OpenGL.OpenGLGfxPlatform();
//    //        return _winGdiPlatform;
//    //    } 
//    //} 
//}
//#endif


//////Apache2, 2014-2017, WinterDev
////#if GL_ENABLE
////using PixelFarm.Drawing;
////using PixelFarm.Drawing.WinGdi;

////namespace LayoutFarm.UI.OpenGL
////{
////    public static class MyOpenGLPortal
////    {
////        static PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform _winGdiPlatform;
////        static bool isInit;
////        public static GraphicsPlatform Start(MyWinGdiPortalSetupParameters initParams)
////        {
////            if (isInit)
////            {
////                return _winGdiPlatform;
////            }
////            isInit = true;

////            //text services:            
////            TextServices.IFonts = initParams.TextServiceInstance ?? new GdiPlusIFonts();
////            ActualFontResolver.Resolver = initParams.ActualFontResolver ?? new GdiFontResolver();
////            //set if we use pixelfarm's native myft.dll
////            //or use managed text break
////            //-------------------------------------
////            //if we use ICU text breaker
////            //1. load icu data
////            //if (initParams.IcuDataFile != null)
////            //{
////            //    //check icu file is exist 
////            //    //TODO: review  file/resource load mechanism again ***
////            //     NativeTextBreaker.SetICUDataFile(initParams.IcuDataFile);
////            //}
////            ////2. text breaker
////            //RootGraphic.SetTextBreakerGenerator(
////            //    initParams.TextBreakGenerator ??
////            //    (locale => new NativeTextBreaker(TextBreakKind.Word, locale))
////            //    );

////            //-------------------------------------
////            //config encoding
////            WinGdiPlusPlatform.SetFontEncoding(System.Text.Encoding.GetEncoding(874));
////            //-------------------------------------
////            //WinGdiPlusPlatform.SetFontNotFoundHandler(
////            //    (fontCollection, fontName, style) =>
////            //    {
////            //        //TODO: implement font not found mapping here
////            //        //_fontsMapping["monospace"] = "Courier New";
////            //        //_fontsMapping["Helvetica"] = "Arial";
////            //        fontName = fontName.ToUpper();
////            //        switch (fontName)
////            //        {
////            //            case "MONOSPACE":
////            //                return fontCollection.GetFont("Courier New", style);
////            //            case "HELVETICA":
////            //                return fontCollection.GetFont("Arial", style);
////            //            case "TAHOMA":
////            //                //default font must found
////            //                //if not throw err 
////            //                //this prevent infinit loop
////            //                throw new System.NotSupportedException();
////            //            default:
////            //                return fontCollection.GetFont("tahoma", style);
////            //        }

////            //    });
////            _winGdiPlatform = new PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform();

////            return _winGdiPlatform;
////        }
////        public static void End()
////        {
////            //PixelFarm.Drawing.DrawingGL.CanvasGLPortal.End();
////        }
////        public static GraphicsPlatform P
////        {
////            get
////            {
////                return null;
////                  //PixelFarm.Drawing.DrawingGL.CanvasGLPortal.P;
////            }
////        }
////    }
////    public class MyWinGdiPortalSetupParameters
////    {
////        public string IcuDataFile { get; set; }
////        public IFonts TextServiceInstance { get; set; }
////        public IActualFontResolver ActualFontResolver { get; set; }
////    }
 

////}
////#endif