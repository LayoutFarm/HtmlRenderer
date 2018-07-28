//Apache2, 2014-present, WinterDev

using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    [DemoNote("4.2 MixHtml and Text")]
    class Demo_MixHtml : App
    {
        protected override void OnStart(AppHost host)
        {
            var htmlhost = HtmlHostCreatorHelper.CreateHtmlHost(host, null, null);
            ////==================================================
            //html box
            var htmlBox = new HtmlBox(htmlhost, 800, 400);
            htmlBox.SetLocation(30, 30);
            host.AddChild(htmlBox);
            string html = @"<html><head></head><body><div>OK1</div><div>OK2</div></body></html>";
            htmlBox.LoadHtmlString(html);
            //================================================== 

            //textbox
            var textbox = new LayoutFarm.CustomWidgets.TextBox(400, 100, true);
            textbox.SetLocation(0, 200);
            host.AddChild(textbox);
            textbox.Focus();
        }
    }
}