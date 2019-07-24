//Apache2, 2014-present, WinterDev

using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    [DemoNote("4.2 MixHtml and Text")]
    class Demo_MixHtml : App
    {
        protected override void OnStart(AppHost host)
        {
            {
                string html = @"<html><head></head><body><div>OK1</div><div>OK2</div></body></html>";


                string testFilename = @"D:\projects\HtmlRenderer\Source\Test8_HtmlRenderer.Demo\Samples\0_acid1_dev\00.htm";
                //string testFilename = "..\\Test8_HtmlRenderer.Demo\\Samples\\0_acid1_dev\\00.htm";
                if (System.IO.File.Exists(testFilename))
                {
                    html = System.IO.File.ReadAllText(testFilename);
                }


                //html box1
                HtmlBoxes.HtmlHost htmlhost = HtmlHostCreatorHelper.CreateHtmlHost(host, null, null);
                //html box             
                var htmlBox = new HtmlBox(htmlhost, 800, 600);
                htmlBox.SetLocation(0, 0);
                //htmlBox.PreferSoftwareRenderer = true;
                host.AddChild(htmlBox);

                htmlBox.LoadHtmlString(html);
                //================================================== 
            }
            {
                //html box2

                HtmlBoxes.HtmlHost htmlhost = HtmlHostCreatorHelper.CreateHtmlHost(host, null, null);
                //html box             
                var htmlBox = new HtmlBox(htmlhost, 800, 400);
#if DEBUG
                htmlBox.dbugPreferSoftwareRenderer = true;
#endif
                htmlBox.SetLocation(0, 610);//**
                host.AddChild(htmlBox);
                string html = @"<html><head></head><body><div>OK3</div><div>OK4</div></body></html>";
                htmlBox.LoadHtmlString(html);
                //================================================== 
            }

            //{
            //    //textbox
            //    var textbox = new LayoutFarm.CustomWidgets.TextBox(400, 25, true);
            //    textbox.SetLocation(0, 0);
            //    host.AddChild(textbox);
            //    textbox.Focus();
            //}
        }
    }
}