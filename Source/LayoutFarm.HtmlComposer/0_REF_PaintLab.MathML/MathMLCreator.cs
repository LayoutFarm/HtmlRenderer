﻿//MIT, 2020-present, WinterDev

using System;
using System.Collections.Generic;

using LayoutFarm.Composers;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.WebDom;

using MathLayout;

namespace PaintLab.MathML
{

    class MathMLBoxTreeCreator
    {
        //see also PaintLab.Svg.SvgCreator
#if DEBUG
        public MathMLBoxTreeCreator() { }
#endif
        MathMLDocument _currentDoc;
        MathMLDocBuilder _docBuilder = new MathMLDocBuilder();

        public CssBoxMathMLRoot CreateMathMLBox(CssBox parentBox,
          HtmlElement elementNode,
          LayoutFarm.Css.BoxSpec spec)
        {

            CssBoxMathMLRoot mathMLRoot = new CssBoxMathMLRoot(elementNode.Spec);

            MathMLDocument doc = new MathMLDocument();
            doc.CssActiveSheet = new CssActiveSheet();
            _currentDoc = doc;
            _docBuilder.ResultDoc = doc;
            _docBuilder.OnBegin();

            math mathNode = new math();
            _docBuilder.CurrentMathNode = mathNode;

            CreateBoxContent(elementNode);

            _docBuilder.OnEnd();

            MathMLRootEventPortal mathMLController = new MathMLRootEventPortal(elementNode);
            mathMLRoot.SetController(mathMLController);
            parentBox.AppendChild(mathMLRoot);
            return mathMLRoot;

            ////TODO: review here
            ////

            ////create blank svg document
            //VgDocument svgdoc = new VgDocument();
            //svgdoc.CssActiveSheet = new LayoutFarm.WebDom.CssActiveSheet();
            //_currentDoc = svgdoc;
            //_svgDocBuilder.ResultDocument = svgdoc;
            ////
            //_svgDocBuilder.OnBegin();
            //CreateBoxContent(elementNode);
            //_svgDocBuilder.OnEnd();

            ////-----------------------------------------
            //SvgRootEventPortal svgRootController = new SvgRootEventPortal(elementNode);
            //CssBoxSvgRoot svgRoot = new CssBoxSvgRoot(
            //    elementNode.Spec,
            //    svgdoc);

            //svgRoot.SetController(svgRootController);
            //svgRootController.SvgRoot = svgRoot;
            //parentBox.AppendChild(svgRoot);
            //return svgRoot;
        }
        void CreateBoxContent(HtmlElement elem)
        {
            //recursive ***

            _docBuilder.OnVisitNewElement(elem.Name);
            //
            _docBuilder.CurrentMathNode.SetController(elem); //**

            //some nodes have special content
            //linear gradient  
            foreach (LayoutFarm.WebDom.DomAttribute attr in elem.GetAttributeIterForward())
            {
                _docBuilder.OnAttribute(attr.Name, attr.Value);
            }

            _docBuilder.OnEnteringElementBody();
            int j = elem.ChildrenCount;
            for (int i = 0; i < j; ++i)
            {
                LayoutFarm.WebDom.DomNode childNode = elem.GetChildNode(i);
                switch (childNode.NodeKind)
                {
                    case LayoutFarm.WebDom.HtmlNodeKind.OpenElement:
                        {
                            if (childNode is HtmlElement htmlElem)
                            {
                                //recursive ***
                                CreateBoxContent(htmlElem);
                            }
                        }
                        break;
                    case LayoutFarm.WebDom.HtmlNodeKind.TextNode:
                        {
                            if (childNode is HtmlTextNode textnode)
                            {
                                if (elem.WellknownElementName == LayoutFarm.WebDom.WellKnownDomNodeName.style)
                                {
                                    //content of style node 
                                    MathStyleSpec styleSpec = (MathStyleSpec)_docBuilder.CurrentMathNode.ElemSpec;
                                    //content of the style elem
                                    //parse
                                    styleSpec.RawTextContent = new string(textnode.GetOriginalBuffer());
                                    //parse css content of the style element

                                    //TODO: review this again
                                    //LayoutFarm.WebDom.Parser.CssParserHelper.ParseStyleSheet(styleSpec.CssSheet = new LayoutFarm.WebDom.CssActiveSheet(), styleSpec.RawTextContent);
                                    //_currentDoc.CssActiveSheet.Combine(styleSpec.CssSheet);
                                    //TODO: review Combine again 
                                }
                            }
                        }
                        break;
                }
            }
            _docBuilder.OnExitingElementBody();
        }
    }


    class MathMLDocument
    {
        public MathMLDocument()
        {

        }
        public math Root { get; set; }
        public CssActiveSheet CssActiveSheet { get; set; }
    }

    class MathMLDocBuilder
    {
        DomNodeDefinitionStore _nodeDefs = new DomNodeDefinitionStore();
        Stack<MathNode> _nodes = new Stack<MathNode>();
        public MathMLDocBuilder()
        {
        }
        public MathNode CurrentMathNode { get; set; }
        public MathMLDocument ResultDoc { get; set; }
        public void OnBegin()
        {
            _nodes.Clear();
        }
        public void OnEnd()
        {

        }
        public void OnVisitNewElement(string elemName)
        {
            if (elemName == "math")
            {

                return;
            }

            MathNode currNode = CurrentMathNode;

            CurrentMathNode = _nodeDefs.CreateMathNode(elemName);
            currNode.AppendChild(CurrentMathNode);

        }
        public void OnEnteringElementBody()
        {
            _nodes.Push(CurrentMathNode);
        }
        public void OnExitingElementBody()
        {
            CurrentMathNode = _nodes.Pop();
        }
        public void OnAttribute(string attrName, string value)
        {

        }
    }

}