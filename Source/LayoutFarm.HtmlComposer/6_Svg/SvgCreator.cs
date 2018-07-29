//Apache2, 2014-present, WinterDev
//MS-PL, Apache2 some parts derived from github.com/vvvv/svg 

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.Composers;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.WebDom;
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

        void CreateSvgBoxContent(HtmlElement elementNode)
        {
            //recursive ***

            _svgDocBuilder.OnVisitNewElement(elementNode.Name);
            //
            _svgDocBuilder.CurrentSvgElem.SetController(elementNode); //**

            foreach (WebDom.DomAttribute attr in elementNode.GetAttributeIterForward())
            {
                _svgDocBuilder.OnAttribute(attr.Name, attr.Value);
            }
            _svgDocBuilder.OnEnteringElementBody();

            int j = elementNode.ChildrenCount;
            for (int i = 0; i < j; ++i)
            {
                HtmlElement node = elementNode.GetChildNode(i) as HtmlElement;
                if (node != null)
                {
                    //recursive ***
                    CreateSvgBoxContent(node);
                }
            }
            _svgDocBuilder.OnExtingElementBody();
        }
    }
}