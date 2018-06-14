//Apache2, 2014-2018, WinterDev

using System;
using LayoutFarm.CustomWidgets;
namespace LayoutFarm.DzBoardSample
{
    class MenuItemClickEventArgs : EventArgs
    {
        public MenuItemClickEventArgs(string menuItemName)
        {
            this.MenuName = menuItemName;
        }
        public string MenuName { get; private set; }
    }

    class MenuBoardModule : CompoModule
    {
        public event EventHandler<MenuItemClickEventArgs> menuItemClick;
        protected override void OnStartModule()
        {
            var htmlBox = new HtmlBox(htmlHost, 500, 40);
            viewport.AddChild(htmlBox);
            //design buttons here 

            string htmltext = @"<html><head></head><body id='body_elem'>
                <span id='cmd_box' style='padding:2px'>Box</span>
                <span id='cmd_rect' style='padding:2px'>Rect</span>   
                <span id='cmd_img' style='padding:2px'>Image</span>                 
            </body></html>";
            htmlBox.LoadHtmlString(htmltext);
            var dom1 = htmlBox.HtmlContainer.WebDocument as LayoutFarm.WebDom.IHtmlDocument;
            if (dom1 != null)
            {
                var bodyElemet = dom1.getElementById("body_elem");
                bodyElemet.attachEventListener("mousedown", e =>
                {
                    var srcElement = e.ExactHitObject;
                    HtmlBoxes.CssRun run = srcElement as HtmlBoxes.CssRun;
                    if (run != null)
                    {
                        menuItemClick(this, new MenuItemClickEventArgs(run.Text));
                    }
                    e.StopPropagation();
                });
            }
        }
    }
}