//Apache2, 2014-2018, WinterDev

using System.Text;
using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    [DemoNote("4.4 CssLeanBox")]
    class Demo_CssLeanBox : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            ////==================================================
            //html box
            var htmlHost = HtmlHostCreatorHelper.CreateHtmlHost(viewport, null, null);
            var htmlBox = new HtmlBox(htmlHost, 800, 400);
            StringBuilder stbuilder = new StringBuilder();
            stbuilder.Append("<html><head></head><body>");
            stbuilder.Append("<div>custom box1</div>");
            stbuilder.Append("<x id=\"my_custombox1\"></x>");
            stbuilder.Append("<div>custom box2</div>");
            stbuilder.Append("<x type=\"textbox\" id=\"my_custombox1\"></x>");
            stbuilder.Append("</body></html>");
            htmlBox.LoadHtmlString(stbuilder.ToString());
            viewport.AddChild(htmlBox);
            //==================================================  

            //textbox
            var textbox = new LayoutFarm.CustomWidgets.TextBox(400, 100, true);
            textbox.SetLocation(0, 200);
            viewport.AddChild(textbox);
            textbox.Focus();
        }
    }
}