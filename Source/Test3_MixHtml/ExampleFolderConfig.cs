//Apache2, 2014-2017, WinterDev


namespace TestGraphicPackage2
{
    static class ExampleFolderConfig
    {
        static string execFromFolder;
        static ExampleFolderConfig()
        {
            string execLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            execFromFolder = System.IO.Path.GetDirectoryName(execLocation);

            LayoutFarm.TextBreak.CustomBreakerBuilder.DataDir = execFromFolder + "\\dictionaries"; 
            //@"Deps_I18N\LayoutFarm.TextBreak\icu58\brkitr_src\dictionaries";

        }
        public static void InitIcuData()
        {

            string icu_dataFile = @"icudt57l.dat";
            LayoutFarm.TextBreak.ICU.NativeTextBreaker.SetICUDataFile(execFromFolder + "\\" + icu_dataFile);
        }

    }
}