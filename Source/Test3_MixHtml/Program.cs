//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace TestGraphicPackage2
{
    static class Program
    {
        static LayoutFarm.Dev.FormDemoList formDemoList;


        [STAThread]
        static void Main()
        {

            YourImplementation.FrameworkInitWinGDI.SetupDefaultValues(); 

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


            PixelFarm.CpuBlit.MemBitmapExtensions.DefaultMemBitmapIO = new PixelFarm.Drawing.WinGdi.GdiBitmapIO();

#if DEBUG
            PixelFarm.CpuBlit.Imaging.PngImageWriter.InstallImageSaveToFileService((IntPtr imgBuffer, int stride, int width, int height, string filename) =>
            {

                using (System.Drawing.Bitmap newBmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    PixelFarm.CpuBlit.BitmapHelper.CopyToGdiPlusBitmapSameSize(imgBuffer, newBmp);
                    //save
                    newBmp.Save(filename);
                }
            });
#endif



            //-------------------------------
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //temp
            //TODO: fix this , 
            //LayoutFarm.Composers.Default.TextBreaker = new LayoutFarm.Composers.MyManagedTextBreaker();
            //LayoutFarm.Composers.Default.TextBreaker = new LayoutFarm.Composers.MyNativeTextBreaker();

            ////------------------------------- 
            formDemoList = new LayoutFarm.Dev.FormDemoList();
            formDemoList.LoadDemoList(typeof(Program));
            LoadHtmlSamples(formDemoList.SamplesTreeView);
            Application.Run(formDemoList);
        }
        static void LoadHtmlSamples(TreeView _samplesTreeView)
        {
            //find sample folder 
            string execFromFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string rootSampleFolder = "..\\Test8_HtmlRenderer.Demo\\Samples";
            var root = new TreeNode("HTML Renderer");
            _samplesTreeView.Nodes.Add(root);
            string[] sampleDirs = System.IO.Directory.GetDirectories(rootSampleFolder);
            //only 1 file level (not recursive)
            foreach (string dirName in sampleDirs)
            {
                var dirNode = new TreeNode(System.IO.Path.GetFileName(dirName));
                root.Nodes.Add(dirNode);
                string[] fileNames = System.IO.Directory.GetFiles(dirName, "*.htm");
                foreach (string fname in fileNames)
                {
                    var onlyFileName = System.IO.Path.GetFileName(fname);
                    if (!onlyFileName.StartsWith("x"))
                    {
                        //for our convention: 
                        //file start with x will not show here  
                        //(it used for comment out/backup file)
                        var fileNameNode = new TreeNode(System.IO.Path.GetFileName(fname));
                        dirNode.Nodes.Add(fileNameNode);
                        fileNameNode.Tag = fname;
                    }
                }
            }
            root.ExpandAll();
            _samplesTreeView.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(_samplesTreeView_NodeMouseDoubleClick);
        }
        static void _samplesTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var filename = e.Node.Tag as string;
            if (filename != null)
            {
                //load html from file 
                string fileContent = System.IO.File.ReadAllText(filename);
                var demoHtmlBox = new LayoutFarm.Demo_UIHtmlBox();
                demoHtmlBox.LoadHtml(filename, fileContent);
                formDemoList.RunDemo(demoHtmlBox);
            }
        }
    }


}
