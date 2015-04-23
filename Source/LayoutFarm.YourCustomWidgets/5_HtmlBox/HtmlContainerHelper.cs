// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm;
using LayoutFarm.ContentManagers;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.Composers;

using PixelFarm.Drawing;
using LayoutFarm.UI;

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
                 new LayoutFarm.WebDom.Parser.TextSnapshot(fullHtmlString.ToCharArray()));
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
            //data is wrapp up with div?
            var newDivHost = htmldoc.CreateElement("div");

            myHtmlBodyElement.AddChild(newDivHost);

            WebDocumentParser.ParseHtmlDom(
                     new LayoutFarm.WebDom.Parser.TextSnapshot(htmlFragment.ToCharArray()),
                     htmldoc,
                     newDivHost); 

            //return CreateHtmlContainer(htmlHost, newDivHost, myHtmlBodyElement, htmlFrgmentRenderBox); 

            //1. builder 
            var renderTreeBuilder = htmlHost.GetRenderTreeBuilder();
            //-------------------------------------------------------------------
            //2. generate render tree
            ////build rootbox from htmldoc
            var hostDivElement = newDivHost.OwnerDocument.CreateElement("div");
            myHtmlBodyElement.AddChild(hostDivElement);


            var rootElement = renderTreeBuilder.BuildCssRenderTree(
                hostDivElement,
                newDivHost,
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
            WebDom.HtmlDocument htmldoc,
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