//2016, MIT WinterDev

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
            MergeProject mergePro = CreateMergePixelFarmProject();
            mergePro.MergeAndSave(DEV_DIR + "HtmlRenderer.One.csproj",
               "HtmlRenderer.One",
               "v2.0",
                //"ICU_VER_54;NET20",
               ";NET20",
               new string[] {
                  "System",
                  "System.Drawing",
                  "System.Windows.Forms",
                  "System.XML",
               });
        }
        static MergeProject CreateMergePixelFarmProject()
        {
            MergeProject mergePro = new MergeProject();
            mergePro.LoadSubProject(DEV_DIR + @"Deps\PixelFarm.One.HtmlRenderer.csproj");//this is auto-gen project too
           
            //------------------------------------------------------------------------------------------------
            //base modules
            mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.ClientPlatforms\LayoutFarm.ClientPlatforms.csproj");
            mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.CssBase\LayoutFarm.CssBase.csproj");
            mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.WebLexer\LayoutFarm.WebLexer.csproj");
            //layout and renderer module
            mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.BaseRenderer\LayoutFarm.BaseRenderer.csproj");
            mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.HtmlRenderer\LayoutFarm.HtmlRenderer.csproj");
            //integrated module
            mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.Composers\LayoutFarm.Composers.csproj");
            mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.TextEdit\LayoutFarm.TextEdit.csproj");
            mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.WebDom\LayoutFarm.WebDom.csproj");
            mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.YourCustomWidgets\LayoutFarm.YourCustomWidgets.csproj");
            //platforms.Windows
            mergePro.LoadSubProject(DEV_DIR + @"LayoutFarm.Platforms.WinForms\LayoutFarm.Platforms.WinForms.csproj");
            //js engine
            mergePro.LoadSubProject(DEV_DIR + @"Deps\Espresso\Net20\Espresso.Interfaces\Espresso.Interfaces.csproj");
            mergePro.LoadSubProject(DEV_DIR + @"Deps\Espresso\Net20\EspressoCore\EspressoCore.csproj");
            return mergePro;
        }
        private void cmdCopyNativeResources_Click(object sender, EventArgs e)
        {
            //
        }
    }
}
