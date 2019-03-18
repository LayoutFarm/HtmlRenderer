//Apache2, 2014-present, WinterDev

using LayoutFarm.Composers;
using LayoutFarm.CustomWidgets;
namespace LayoutFarm.WebWidgets
{
    abstract class HtmlDemoBase : App
    {
        LayoutFarm.ContentManagers.ImageLoadingQueueManager _imgLoadingQ;
        protected LayoutFarm.HtmlBoxes.HtmlHost _myHtmlHost;
        protected HtmlBox _groundHtmlBox;
        protected AppHost _host;
        protected HtmlDocument _groundHtmlDoc;
        protected override void OnStart(AppHost host)
        {
            _host = host;
            _imgLoadingQ = new ContentManagers.ImageLoadingQueueManager();
            _imgLoadingQ.AskForImage += (s, e) =>
            {
                e.SetResultImage(host.LoadImage(e.ImagSource));
            };
            //init host 
            _myHtmlHost = HtmlHostCreatorHelper.CreateHtmlHost(host,
              (s, e) => _imgLoadingQ.AddRequestImage(e.ImageBinder),
              (s, e) => { });
            //-----------------------------------------------------

            _groundHtmlBox = new HtmlBox(_myHtmlHost, 800, 600);
            string html = @"<div></div>";
            //if you want to use full html-> use HtmlBox instead  

            _host.AddChild(_groundHtmlBox);
            //----------------------------------------------------- 
            _groundHtmlBox.LoadHtmlFragmentString(html);
            _groundHtmlDoc = _groundHtmlBox.HtmlDoc as HtmlDocument;

            OnHtmlHostCreated();
        }
        protected virtual void OnHtmlHostCreated()
        {
        }
        protected void AddToViewport(HtmlWidgets.HtmlWidgetBase htmlWidget)
        {
            //
            WebDom.DomElement presentationDomNode = htmlWidget.GetPresentationDomNode((HtmlElement)_groundHtmlDoc.RootNode);
            _groundHtmlDoc.BodyElement.AddChild(presentationDomNode);
        }
    }
}