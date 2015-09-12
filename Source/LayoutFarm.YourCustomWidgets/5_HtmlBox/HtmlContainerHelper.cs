// 2015,2014 ,Apache2, WinterDev 
using LayoutFarm.Composers;

namespace LayoutFarm.HtmlBoxes
{

    static class HtmlContainerHelper
    {
        public static MyHtmlContainer CreateHtmlContainerFromFullHtml(
            HtmlHost htmlHost,
            string fullHtmlString,
            HtmlRenderBox htmlFrgmentRenderBox)
        {


            var htmldoc = WebDocumentParser.ParseDocument(
                 new LayoutFarm.WebDom.Parser.TextSource(fullHtmlString.ToCharArray()));
            //1. builder 
            var renderTreeBuilder = htmlHost.GetRenderTreeBuilder();
            //------------------------------------------------------------------- 
            //2. generate render tree
            ////build rootbox from htmldoc

            var rootElement = renderTreeBuilder.BuildCssRenderTree(htmldoc,
                htmlHost.BaseStylesheet,
                htmlFrgmentRenderBox);
            //3. create small htmlContainer

            var htmlContainer = new MyHtmlContainer(htmlHost);

            htmlContainer.WebDocument = htmldoc;
            htmlContainer.RootCssBox = rootElement;
            htmlContainer.SetMaxSize(htmlFrgmentRenderBox.Width, 0);

            var lay = htmlHost.GetSharedHtmlLayoutVisitor(htmlContainer);
            htmlContainer.PerformLayout(lay);
            htmlHost.ReleaseHtmlLayoutVisitor(lay);


            htmlFrgmentRenderBox.SetHtmlContainer(htmlContainer, rootElement);
            return htmlContainer;

        }
        public static MyHtmlContainer CreateHtmlContainerFromFragmentHtml(
            HtmlHost htmlHost,
            string htmlFragment,
            HtmlRenderBox htmlFrgmentRenderBox)
        {

            var htmldoc = htmlHost.CreateNewSharedHtmlDoc();
            var myHtmlBodyElement = htmldoc.CreateElement("body");
            htmldoc.RootNode.AddChild(myHtmlBodyElement);

            //data is wraped up within div?
            //TODO: review this, use shadow dom instead
            var newDivHost = htmldoc.CreateElement("div");
            myHtmlBodyElement.AddChild(newDivHost);
            WebDocumentParser.ParseHtmlDom(
                     new LayoutFarm.WebDom.Parser.TextSource(htmlFragment.ToCharArray()),
                     htmldoc,
                     newDivHost);

            //1. builder 
            var renderTreeBuilder = htmlHost.GetRenderTreeBuilder();
            //-------------------------------------------------------------------
            //2. generate render tree  
            var rootElement = renderTreeBuilder.BuildCssRenderTree(
                 htmldoc,
                 htmldoc.CssActiveSheet,
                 htmlFrgmentRenderBox);

            //3. create small htmlContainer

            var htmlContainer = new MyHtmlContainer(htmlHost);

            htmlContainer.WebDocument = newDivHost.OwnerDocument;
            htmlContainer.RootCssBox = rootElement;
            htmlContainer.SetMaxSize(htmlFrgmentRenderBox.Width, 0);

            var lay = htmlHost.GetSharedHtmlLayoutVisitor(htmlContainer);
            htmlContainer.PerformLayout(lay);
            htmlHost.ReleaseHtmlLayoutVisitor(lay);


            htmlFrgmentRenderBox.SetHtmlContainer(htmlContainer, rootElement);
            return htmlContainer;

        }

        public static MyHtmlContainer CreateHtmlContainer(
            HtmlHost htmlHost,
            WebDom.WebDocument htmldoc,
            HtmlRenderBox htmlFrgmentRenderBox)
        {

            //1. builder 
            var renderTreeBuilder = htmlHost.GetRenderTreeBuilder();
            //-------------------------------------------------------------------


            //2. generate render tree
            ////build rootbox from htmldoc

            var rootElement = renderTreeBuilder.BuildCssRenderTree(htmldoc,
                htmlHost.BaseStylesheet,
                htmlFrgmentRenderBox);
            //3. create small htmlContainer

            var htmlContainer = new MyHtmlContainer(htmlHost);

            htmlContainer.WebDocument = htmldoc;
            htmlContainer.RootCssBox = rootElement;
            htmlContainer.SetMaxSize(htmlFrgmentRenderBox.Width, 0);

            var lay = htmlHost.GetSharedHtmlLayoutVisitor(htmlContainer);
            htmlContainer.PerformLayout(lay);
            htmlHost.ReleaseHtmlLayoutVisitor(lay);


            htmlFrgmentRenderBox.SetHtmlContainer(htmlContainer, rootElement);
            return htmlContainer;
        }




    }
}