//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Forms;
using YourImplementation;
using LayoutFarm;

namespace TestGraphicPackage2
{
    static class Program
    {

        [STAThread]
        static void Main()
        {


            YourImplementation.LocalFileStorageProvider file_storageProvider = new YourImplementation.LocalFileStorageProvider("");
            PixelFarm.Platforms.StorageService.RegisterProvider(file_storageProvider);
            YourImplementation.FrameworkInitGLES.SetupDefaultValues();

            //2.2 Icu Text Break info
            //test Typography's custom text break,
            //check if we have that data?            
            //-------------------------------------------  
            string icu_datadir = YourImplementation.RelativePathBuilder.SearchBackAndBuildFolderPath(System.IO.Directory.GetCurrentDirectory(), "HtmlRenderer", @"..\Typography\Typography.TextBreak\icu62\brkitr");
            if (!System.IO.Directory.Exists(icu_datadir))
            {
                throw new System.NotSupportedException("dic");
            }
            var dicProvider = new Typography.TextBreak.IcuSimpleTextFileDictionaryProvider() { DataDir = icu_datadir };
            Typography.TextBreak.CustomBreakerBuilder.Setup(dicProvider);



            PixelFarm.CpuBlit.MemBitmapExtensions.DefaultMemBitmapIO = new ImgCodecMemBitmapIO();
            //PixelFarm.CpuBlit.MemBitmapExtensions.DefaultMemBitmapIO = new PixelFarm.Drawing.WinGdi.GdiBitmapIO();

            if (!PixelFarm.GLFWPlatforms.Init())
            {
                System.Diagnostics.Debug.WriteLine("can't init glfw");
                return;
            }

            LayoutFarm.Demo_MixHtml mix1 = new LayoutFarm.Demo_MixHtml();
            var myApp = new MyApp(mix1);
            myApp.CreateMainForm();
            GlfwApp.RunMainLoop();

        }
    }


}
