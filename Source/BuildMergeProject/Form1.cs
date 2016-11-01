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
        private void cmdBuildHtmlRenderOne_Click(object sender, EventArgs e)
        {
            MergeProject mergePro = CreateMergePixelFarmProject();
            mergePro.MergeAndSave(@"D:\projects\HTML-Renderer\Source\HtmlRenderer.One.csproj",
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
            mergePro.LoadSubProject(@"D:\projects\HTML-Renderer\Source\Deps\PixelFarm.One.csproj");//this is auto-gen project too
            //base modules

            mergePro.LoadSubProject(@"D:\projects\HTML-Renderer\Source\LayoutFarm.ClientPlatforms\LayoutFarm.ClientPlatforms.csproj");
            mergePro.LoadSubProject(@"D:\projects\HTML-Renderer\Source\LayoutFarm.CssBase\LayoutFarm.CssBase.csproj");
            mergePro.LoadSubProject(@"D:\projects\HTML-Renderer\Source\LayoutFarm.WebLexer\LayoutFarm.WebLexer.csproj");
            //layout and renderer module
            mergePro.LoadSubProject(@"D:\projects\HTML-Renderer\Source\LayoutFarm.BaseRenderer\LayoutFarm.BaseRenderer.csproj");
            mergePro.LoadSubProject(@"D:\projects\HTML-Renderer\Source\LayoutFarm.HtmlRenderer\LayoutFarm.HtmlRenderer.csproj");
            //integrated module
            mergePro.LoadSubProject(@"D:\projects\HTML-Renderer\Source\LayoutFarm.Composers\LayoutFarm.Composers.csproj");
            mergePro.LoadSubProject(@"D:\projects\HTML-Renderer\Source\LayoutFarm.TextEdit\LayoutFarm.TextEdit.csproj");
            mergePro.LoadSubProject(@"D:\projects\HTML-Renderer\Source\LayoutFarm.WebDom\LayoutFarm.WebDom.csproj");
            mergePro.LoadSubProject(@"D:\projects\HTML-Renderer\Source\LayoutFarm.YourCustomWidgets\LayoutFarm.YourCustomWidgets.csproj");
            //platforms.Windows
            mergePro.LoadSubProject(@"D:\projects\HTML-Renderer\Source\LayoutFarm.Platforms.WinForms\LayoutFarm.Platforms.WinForms.csproj");
            //js engine
            mergePro.LoadSubProject(@"D:\projects\HTML-Renderer\Source\Deps\Espresso\Net20\Espresso.Interfaces\Espresso.Interfaces.csproj");
            mergePro.LoadSubProject(@"D:\projects\HTML-Renderer\Source\Deps\Espresso\Net20\EspressoCore\EspressoCore.csproj");
            return mergePro;
        }
    }
}
