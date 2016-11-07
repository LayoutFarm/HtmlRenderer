//Apache2, 2014-2016, WinterDev

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

            ExampleFolderConfig.InitIcuData();
            //-----------------------
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //temp
            //TODO: fix this , 
            var startPars = new LayoutFarm.UI.GdiPlus.MyWinGdiPortalSetupParameters();
            var platform = LayoutFarm.UI.GdiPlus.MyWinGdiPortal.Start(startPars);
            formDemoList = new LayoutFarm.Dev.FormDemoList();
            formDemoList.LoadDemoList(typeof(Program));
            LoadHtmlSamples(formDemoList.SamplesTreeView);
            Application.Run(formDemoList);
            LayoutFarm.UI.GdiPlus.MyWinGdiPortal.End();
        }
        static void LoadHtmlSamples(TreeView _samplesTreeView)
        {
            //find sample folder 
            string execFromFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string checkFolder = ExampleFolderConfig.GetCheckFolder();

            //only from debug ?
            if (!execFromFolder.EndsWith(checkFolder))
            {
                return;
            }

            int index = execFromFolder.LastIndexOf(checkFolder);
            string rootSampleFolder = execFromFolder.Substring(0, index) + "\\Source\\HtmlRenderer.Demo\\Samples";
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
                var fileContent = System.IO.File.ReadAllText(filename);
                LayoutFarm.Demo_UIHtmlBox demoHtmlBox = new LayoutFarm.Demo_UIHtmlBox();
                demoHtmlBox.LoadHtml(filename, fileContent);
                formDemoList.RunDemo(demoHtmlBox);
            }
        }
    }

}
