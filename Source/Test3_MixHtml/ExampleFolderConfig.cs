//Apache2, 2014-2017, WinterDev

using Typography.TextBreak;
namespace TestGraphicPackage2
{
    static class ExampleFolderConfig
    {
        static string execFromFolder;
        static ExampleFolderConfig()
        {
            //string execLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //execFromFolder = System.IO.Path.GetDirectoryName(execLocation); 
            //Typography.TextBreak.CustomBreakerBuilder.DataDir = execFromFolder + "\\dictionaries"; 
            ////@"Deps_I18N\LayoutFarm.TextBreak\icu58\brkitr_src\dictionaries"; 
        }
        public static void InitData()
        { 
            //string icu_dataFile = @"icudt57l.dat";
            //Typography.TextBreak.ICU.NativeTextBreaker.SetICUDataFile(execFromFolder + "\\" + icu_dataFile);
        }

    }
}