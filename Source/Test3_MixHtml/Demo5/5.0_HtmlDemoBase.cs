//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.Composers;
using LayoutFarm.CustomWidgets;
namespace LayoutFarm.WebWidgets
{
    abstract class HtmlDemoBase : DemoBase
    {
        LayoutFarm.ContentManagers.ImageContentManager imageContentMan;
        protected LayoutFarm.HtmlBoxes.HtmlHost myHtmlHost;
        protected HtmlBox groundHtmlBox;
        protected SampleViewport sampleViewport;
        HtmlDocument groundHtmlDoc;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            this.sampleViewport = viewport;
            imageContentMan = new ContentManagers.ImageContentManager();
            imageContentMan.ImageLoadingRequest += (s, e) =>
            {
                e.SetResultImage(LoadBitmap(e.ImagSource));
            };
            //init host 
            myHtmlHost = HtmlHostCreatorHelper.CreateHtmlHost(viewport,
              (s, e) => this.imageContentMan.AddRequestImage(e.ImageBinder),
              (s, e) => { });
            //-----------------------------------------------------

            this.groundHtmlBox = new HtmlBox(myHtmlHost, 800, 600);
            string html = @"<div></div>";
            //if you want to use full html-> use HtmlBox instead  

            this.sampleViewport.AddChild(groundHtmlBox);
            //----------------------------------------------------- 
            groundHtmlBox.LoadHtmlFragmentString(html);
            this.groundHtmlDoc = groundHtmlBox.HtmlContainer.WebDocument as HtmlDocument;
            OnHtmlHostCreated();
        }
        protected virtual void OnHtmlHostCreated()
        {
        }
  

        protected void AddToViewport(HtmlWidgets.HtmlWidgetBase htmlWidget)
        {
            //
            var presentationDomNode = htmlWidget.GetPresentationDomNode(this.groundHtmlDoc);
            this.groundHtmlDoc.BodyElement.AddChild(presentationDomNode);
            //this.groundHtmlDoc.RootNode.AddChild(presentationDomNode);
            //sampleViewport.AddContent(htmlWidget.GetPrimaryUIElement(myHtmlHost));
        }
    }
}