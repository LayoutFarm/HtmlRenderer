//Apache2, 2014-present, WinterDev
//MS-PL, Apache2 some parts derived from github.com/vvvv/svg 

using System;
using LayoutFarm.Composers;
using LayoutFarm.HtmlBoxes;

namespace PaintLab.Svg
{
    class SvgCreator
    { 
        VgDocBuilder _svgDocBuilder = new VgDocBuilder();
        VgDocument _currentDoc;
        public CssBoxSvgRoot CreateSvgBox(CssBox parentBox,
            HtmlElement elementNode,
            LayoutFarm.Css.BoxSpec spec)
        {


            //TODO: review here
            //

            //create blank svg document
            VgDocument svgdoc = new VgDocument();
            svgdoc.CssActiveSheet = new LayoutFarm.WebDom.CssActiveSheet();
            _currentDoc = svgdoc;
            _svgDocBuilder.ResultDocument = svgdoc;
            //
            _svgDocBuilder.OnBegin();
            CreateBoxContent(elementNode);
            _svgDocBuilder.OnEnd();

            //-----------------------------------------
            SvgRootEventPortal svgRootController = new SvgRootEventPortal(elementNode);
            CssBoxSvgRoot svgRoot = new CssBoxSvgRoot(
                elementNode.Spec,
                parentBox.RootGfx,
                svgdoc);

            svgRoot.SetController(svgRootController);
            svgRootController.SvgRoot = svgRoot;
            parentBox.AppendChild(svgRoot);
            return svgRoot;
        }


        void CreateBoxContent(HtmlElement elem)
        {
            //recursive ***

            _svgDocBuilder.OnVisitNewElement(elem.Name);
            //
            _svgDocBuilder.CurrentSvgElem.SetController(elem); //**

            //some nodes have special content
            //linear gradient  
            foreach (LayoutFarm.WebDom.DomAttribute attr in elem.GetAttributeIterForward())
            {
                _svgDocBuilder.OnAttribute(attr.Name, attr.Value);
            }
            _svgDocBuilder.OnEnteringElementBody();
            int j = elem.ChildrenCount;
            for (int i = 0; i < j; ++i)
            {
                LayoutFarm.WebDom.DomNode childNode = elem.GetChildNode(i);
                switch (childNode.NodeKind)
                {
                    case LayoutFarm.WebDom.HtmlNodeKind.OpenElement:
                        {

                            HtmlElement htmlElem = childNode as HtmlElement;
                            if (htmlElem != null)
                            {
                                //recursive ***
                                CreateBoxContent(htmlElem);
                            }
                        }
                        break;
                    case LayoutFarm.WebDom.HtmlNodeKind.TextNode:
                        {
                            HtmlTextNode textnode = childNode as HtmlTextNode;
                            if (textnode != null)
                            {
                                if (elem.WellknownElementName == LayoutFarm.WebDom.WellKnownDomNodeName.style)
                                {
                                    //content of style node 
                                    SvgStyleSpec styleSpec = (SvgStyleSpec)_svgDocBuilder.CurrentSvgElem.ElemSpec;
                                    //content of the style elem
                                    //parse
                                    styleSpec.RawTextContent = new string(textnode.GetOriginalBuffer());
                                    //parse css content of the style element

                                    LayoutFarm.WebDom.Parser.CssParserHelper.ParseStyleSheet(styleSpec.CssSheet = new LayoutFarm.WebDom.CssActiveSheet(), styleSpec.RawTextContent);
                                    _currentDoc.CssActiveSheet.Combine(styleSpec.CssSheet);
                                    //TODO: review Combine again 
                                }
                                else if (elem.Name == "text")
                                {
                                    //svg text node
                                    SvgTextSpec textspec = (SvgTextSpec)_svgDocBuilder.CurrentSvgElem.ElemSpec;
                                    textspec.TextContent = new string(textnode.GetOriginalBuffer());
                                    textspec.ExternalTextNode = elem;
                                }
                            }
                        }
                        break;
                }
            }
            _svgDocBuilder.OnExitingElementBody();
        }
    }
}