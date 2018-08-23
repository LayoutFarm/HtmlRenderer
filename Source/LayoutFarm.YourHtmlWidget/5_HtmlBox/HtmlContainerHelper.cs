//Apache2, 2014-present, WinterDev

using LayoutFarm.Composers;
namespace LayoutFarm.HtmlBoxes
{
    static class HtmlContainerHelper
    {
        public static MyHtmlVisualRoot CreateHtmlContainerFromFullHtml(
            HtmlHost htmlHost,
            string fullHtmlString,
            HtmlRenderBox htmlFrgmentRenderBox)
        {
            HtmlDocument htmldoc = WebDocumentParser.ParseDocument(
                 htmlHost,
                 new LayoutFarm.WebDom.Parser.TextSource(fullHtmlString.ToCharArray()));
            //1. builder 
            RenderTreeBuilder renderTreeBuilder = htmlHost.GetRenderTreeBuilder();
            //------------------------------------------------------------------- 
            //2. generate render tree
            ////build rootbox from htmldoc

            CssBox rootElement = renderTreeBuilder.BuildCssRenderTree(htmldoc,
                htmlHost.BaseStylesheet,
                htmlFrgmentRenderBox);
            //3. create small htmlContainer

            MyHtmlVisualRoot htmlContainer = new MyHtmlVisualRoot(htmlHost);
            htmlContainer.WebDocument = htmldoc;
            htmlContainer.SetRootCssBox(rootElement);
            htmlContainer.SetMaxSize(htmlFrgmentRenderBox.Width, 0);
            //
            LayoutVisitor lay = htmlHost.GetSharedHtmlLayoutVisitor(htmlContainer);
            htmlContainer.PerformLayout(lay);
            htmlHost.ReleaseHtmlLayoutVisitor(lay);
            htmlFrgmentRenderBox.SetHtmlContainer(htmlContainer, rootElement);
            return htmlContainer;
        }
        public static MyHtmlVisualRoot CreateHtmlContainerFromFragmentHtml(
            HtmlHost htmlHost,
            string htmlFragment,
            HtmlRenderBox htmlFrgmentRenderBox)
        {
            HtmlDocument htmldoc = htmlHost.CreateNewSharedHtmlDoc();
            WebDom.DomElement myHtmlBodyElement = htmldoc.CreateElement("body");
            htmldoc.RootNode.AddChild(myHtmlBodyElement);
            //data is wraped up within div?
            //TODO: review this, use shadow dom instead
            WebDom.DomElement newDivHost = htmldoc.CreateElement("div");
            myHtmlBodyElement.AddChild(newDivHost);
            WebDocumentParser.ParseHtmlDom(
                     new LayoutFarm.WebDom.Parser.TextSource(htmlFragment.ToCharArray()),
                     htmldoc,
                     newDivHost);
            //1. builder 
            RenderTreeBuilder renderTreeBuilder = htmlHost.GetRenderTreeBuilder();
            //-------------------------------------------------------------------
            //2. generate render tree  
            CssBox rootElement = renderTreeBuilder.BuildCssRenderTree(
                 htmldoc,
                 htmldoc.CssActiveSheet,
                 htmlFrgmentRenderBox);
            //3. create small htmlContainer

            MyHtmlVisualRoot htmlContainer = new MyHtmlVisualRoot(htmlHost);
            htmlContainer.WebDocument = newDivHost.OwnerDocument;
            htmlContainer.SetRootCssBox(rootElement);
            htmlContainer.SetMaxSize(htmlFrgmentRenderBox.Width, 0);
            //htmlContainer.SetRootRenderElement(htmlFrgmentRenderBox);

            //
            LayoutVisitor lay = htmlHost.GetSharedHtmlLayoutVisitor(htmlContainer);
            htmlContainer.PerformLayout(lay);
            htmlHost.ReleaseHtmlLayoutVisitor(lay);
            htmlFrgmentRenderBox.SetHtmlContainer(htmlContainer, rootElement);
            return htmlContainer;
        }

        public static MyHtmlVisualRoot CreateHtmlContainer(
            HtmlHost htmlHost,
            WebDom.WebDocument htmldoc,
            HtmlRenderBox htmlFrgmentRenderBox)
        {
            //1. builder 
            RenderTreeBuilder renderTreeBuilder = htmlHost.GetRenderTreeBuilder();
            //-------------------------------------------------------------------


            //2. generate render tree
            ////build rootbox from htmldoc

            CssBox rootElement = renderTreeBuilder.BuildCssRenderTree(htmldoc,
                htmlHost.BaseStylesheet,
                htmlFrgmentRenderBox);
            //3. create small htmlContainer

            MyHtmlVisualRoot htmlContainer = new MyHtmlVisualRoot(htmlHost);
            htmlContainer.WebDocument = htmldoc;
            htmlContainer.SetRootCssBox(rootElement);
            htmlContainer.SetMaxSize(htmlFrgmentRenderBox.Width, 0);
            //htmlContainer.SetRootRenderElement(htmlFrgmentRenderBox);

            LayoutVisitor lay = htmlHost.GetSharedHtmlLayoutVisitor(htmlContainer);
            htmlContainer.PerformLayout(lay);
            htmlHost.ReleaseHtmlLayoutVisitor(lay);
            htmlFrgmentRenderBox.SetHtmlContainer(htmlContainer, rootElement);
            return htmlContainer;
        }
    }
}