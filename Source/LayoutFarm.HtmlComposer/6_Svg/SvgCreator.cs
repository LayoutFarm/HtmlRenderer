//Apache2, 2014-present, WinterDev
//MS-PL, Apache2 some parts derived from github.com/vvvv/svg 

using System;
using LayoutFarm.Composers;
using LayoutFarm.HtmlBoxes;
using PaintLab.Svg;

namespace LayoutFarm.Svg
{



    class SvgCreator
    {

        SvgDocBuilder _svgDocBuilder = new SvgDocBuilder();
        SvgElementSpecEvaluator _svgSpecEval = new SvgElementSpecEvaluator();

        public CssBoxSvgRoot CreateSvgBox(CssBox parentBox,
            HtmlElement elementNode,
            Css.BoxSpec spec)
        {
            SvgDocument svgdoc = new SvgDocument();
            _svgDocBuilder.ResultDocument = svgdoc;
            _svgDocBuilder.OnBegin();
            CreateSvgBoxContent(elementNode);
            _svgDocBuilder.OnEnd();

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

        void CreateSvgBoxContent(HtmlElement elem)
        {
            //recursive ***

            _svgDocBuilder.OnVisitNewElement(elem.Name);
            //
            _svgDocBuilder.CurrentSvgElem.SetController(elem); //**

            foreach (WebDom.DomAttribute attr in elem.GetAttributeIterForward())
            {
                _svgDocBuilder.OnAttribute(attr.Name, attr.Value);
            }
            _svgDocBuilder.OnEnteringElementBody();

            int j = elem.ChildrenCount;
            for (int i = 0; i < j; ++i)
            {
                WebDom.DomNode childNode = elem.GetChildNode(i);
                HtmlElement htmlElem = childNode as HtmlElement;

                if (htmlElem != null)
                {
                    //recursive ***
                    CreateSvgBoxContent(htmlElem);
                }
                else
                {

                }
            }
            _svgDocBuilder.OnExtingElementBody();
        }
    }
}