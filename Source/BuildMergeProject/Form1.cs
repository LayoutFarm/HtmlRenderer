//MIT, 2016-2017, WinterDev

using System;
using System.Windows.Forms;
namespace BuildMergeProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        const string DEV_DIR = @"D:\projects\HTML-Renderer\Source\";

        private void cmdBuildHtmlRenderOne_Click(object sender, EventArgs e)
        {
            //MergeProject mergePro = CreateMergePixelFarmProject();
            //mergePro.MergeAndSave(DEV_DIR + "HtmlRenderer.One.csproj",
            //   "HtmlRenderer.One",
            //   "v2.0",
            //   //"ICU_VER_54;NET20",
            //   ";NET20, __DESKTOP__"+ ",NET20,PIXEL_FARM,PIXEL_FARM_NET20,__SKIA__, GL_ENABLE",//additional define constant,
            //   new string[] {
            //      "System",
            //      "System.Drawing",
            //      "System.Windows.Forms",
            //      "System.XML",
            //   });
        }
        //static MergeProject CreateMergePixelFarmProject()
        //{
        //    MergeProject mergePro = new MergeProject();
        //    mergePro.LoadSubProject(DEV_DIR + @"Deps\PixelFarm.One.HtmlRenderer.csproj");//this is auto-gen project too             
        //    //mergePro.LoadSubProject(DEV_DIR + @"Deps\PixelFarm.SkiaSharp\PixelFarm.SkiaSharp.csproj");
        //    //mergePro.LoadSubProject(DEV_DIR + @"Deps\PixelFarm.Drawing.Skia\PixelFarm.Drawing.Skia.csproj");
        //    //mergePro.LoadSubProject(DEV_DIR + @"Deps\Tesselate\Tesselate.csproj");
        //    //------------------------------------------------------------------------------------------------
        //    //base modules
        //    mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.ClientPlatforms\LayoutFarm.ClientPlatforms.csproj");
        //    mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.CssBase\LayoutFarm.CssBase.csproj");
        //    mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.WebLexer\LayoutFarm.WebLexer.csproj");
        //    //layout and renderer module
        //    mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.BaseRenderer\LayoutFarm.BaseRenderer.csproj");
        //    mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.HtmlRenderer\LayoutFarm.HtmlRenderer.csproj");
        //    //integrated module
        //    mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.HtmlComposer\LayoutFarm.HtmlComposer.csproj");
        //    mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.TextEdit\LayoutFarm.TextEdit.csproj");
        //    mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.WebDom\LayoutFarm.WebDom.csproj");
        //    mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.YourCustomWidgets\LayoutFarm.YourCustomWidgets.csproj");
        //    //platforms.Windows
        //    mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.PlatformBase\LayoutFarm.PlatformBase.csproj");
        //    mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.Platforms.WinForms\LayoutFarm.Platforms.WinForms.csproj");

        //    //js engine
        //    mergePro.LoadSubProject(DEV_DIR + @"Deps\Espresso\Net20\Espresso.Interfaces\Espresso.Interfaces.csproj");
        //    mergePro.LoadSubProject(DEV_DIR + @"Deps\Espresso\Net20\EspressoCore\EspressoCore.csproj");
        //    return mergePro;
        //}
       
        SolutionMx slnMx;
        SolutionListViewController _slnListViewController;
        private void cmdReadSln_Click(object sender, EventArgs e)
        {
            //read sln file 
            slnMx = new SolutionMx();
            slnMx.ReadSolution(@"D:\projects\HTML-Renderer\Source\MainDev.sln");
            _slnListViewController = new SolutionListViewController();
            _slnListViewController.SetSolutionListView(this.listView1);
            _slnListViewController.SetMergePlanListView(this.listView2);
            _slnListViewController.SetProjectReferenceListView(this.lstAsmReferenceList);
            _slnListViewController.LoadSolutionMx(slnMx);
        }

        private void cmdBuildSelectedMergePro_Click(object sender, EventArgs e)
        {
            _slnListViewController.BuildMergeProjectFromSelectedItem();
        }

       
    }
}
