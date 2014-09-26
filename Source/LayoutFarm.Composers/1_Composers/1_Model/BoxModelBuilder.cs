//BSD 2014, WinterDev
//ArthurHub

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using LayoutFarm.Drawing;
using System.Collections.Generic;
using HtmlRenderer.Css;
using HtmlRenderer.WebDom;
using HtmlRenderer.WebDom.Parser;
using HtmlRenderer.Boxes;
using LayoutFarm.Drawing;
using HtmlRenderer.Composers.BridgeHtml;

namespace HtmlRenderer.Composers
{



    /// <summary>
    /// Handle css DOM tree generation from raw html and stylesheet.
    /// </summary>
    public class BoxModelBuilder
    {

        WebDom.Parser.CssParser miniCssParser = new CssParser();
        ContentTextSplitter contentTextSplitter = new ContentTextSplitter();
        public event ContentManagers.RequestStyleSheetEventHandler RequestStyleSheet;

        public BoxModelBuilder()
        {

        }

        /// <summary>
        /// Parses the source html to css boxes tree structure.
        /// </summary>
        /// <param name="source">the html source to parse</param>
        public WebDocument ParseDocument(TextSnapshot snapSource)
        {
            var parser = new HtmlParser();
            //------------------------
            var blankHtmlDoc = new BridgeHtmlDocument();
            parser.Parse(snapSource, blankHtmlDoc);
            return blankHtmlDoc;
        }
        //-----------------------------------------------------------------


        void RaiseRequestStyleSheet(
            string hrefSource,
            out string stylesheet,
            out CssActiveSheet stylesheetData)
        {
            if (hrefSource == null || RequestStyleSheet == null)
            {
                stylesheet = null;
                stylesheetData = null;
                return;
            }

            ContentManagers.StylesheetLoadEventArgs e = new ContentManagers.StylesheetLoadEventArgs(hrefSource);
            RequestStyleSheet(e);
            stylesheet = e.SetStyleSheet;
            stylesheetData = e.SetStyleSheetData;

        }


        void PrepareStylesAndContentOfChildNodes(
          HtmlElement parentElement,
          ActiveCssTemplate activeCssTemplate)
        {
            //recursive 
            foreach (WebDom.DomNode node in parentElement.GetChildNodeIterForward())
            {
                switch (node.NodeType)
                {
                    case WebDom.HtmlNodeType.OpenElement:
                    case WebDom.HtmlNodeType.ShortElement:
                        {
                            HtmlElement bridgeElement = (HtmlElement)node;
                            bridgeElement.WellknownElementName = UserMapUtil.EvaluateTagName(bridgeElement.LocalName);
                            switch (bridgeElement.WellknownElementName)
                            {
                                case WellKnownDomNodeName.style:
                                    {
                                        //style element should have textnode child
                                        int j = bridgeElement.ChildrenCount;
                                        for (int i = 0; i < j; ++i)
                                        {
                                            var ch = bridgeElement.GetChildNode(i);
                                            switch (ch.NodeType)
                                            {
                                                case HtmlNodeType.TextNode:
                                                    {
                                                        HtmlTextNode textNode = (HtmlTextNode)bridgeElement.GetChildNode(0);
                                                        activeCssTemplate.LoadRawStyleElementContent(new string(textNode.GetOriginalBuffer()));
                                                        //break
                                                        i = j;
                                                    } break;
                                            }
                                        }
                                        continue;
                                    }
                                case WellKnownDomNodeName.link:
                                    {
                                        //<link rel="stylesheet"
                                        DomAttribute relAttr;
                                        if (bridgeElement.TryGetAttribute(WellknownName.Rel, out relAttr)
                                            && relAttr.Value.ToLower() == "stylesheet")
                                        {
                                            //if found
                                            string stylesheet;
                                            CssActiveSheet stylesheetData;

                                            DomAttribute hrefAttr;
                                            if (bridgeElement.TryGetAttribute(WellknownName.Href, out hrefAttr))
                                            {
                                                RaiseRequestStyleSheet(
                                                    hrefAttr.Value,
                                                    out stylesheet,
                                                    out stylesheetData);

                                                if (stylesheet != null)
                                                {
                                                    activeCssTemplate.LoadRawStyleElementContent(stylesheet);
                                                }
                                                else if (stylesheetData != null)
                                                {
                                                    activeCssTemplate.LoadAnotherStylesheet(stylesheetData);
                                                }

                                            }
                                        }

                                        continue;
                                    }
                            }
                            //-----------------------------                            
                            //apply style for this node  
                            ApplyStyleSheetForSingleBridgeElement(bridgeElement, parentElement.Spec, activeCssTemplate);
                            //-----------------------------

                            //recursive 
                            PrepareStylesAndContentOfChildNodes(bridgeElement, activeCssTemplate);
                            //-----------------------------
                        } break;
                    case WebDom.HtmlNodeType.TextNode:
                        {

                            HtmlTextNode textnode = (HtmlTextNode)node;
                            //inner content is parsed here 

                            var parentSpec = parentElement.Spec;
                            char[] originalBuffer = textnode.GetOriginalBuffer();

                            List<CssRun> runlist;
                            bool hasSomeCharacter;
                            contentTextSplitter.ParseWordContent(originalBuffer, parentSpec, out runlist, out hasSomeCharacter);
                            textnode.SetSplitParts(runlist, hasSomeCharacter);

                        } break;
                }
            }
        }


        public CssBox BuildCssTree(WebDocument htmldoc,
            IFonts iFonts,
            HtmlIsland htmlContainer,
            CssActiveSheet cssData)
        {

            CssBox rootBox = null;
            ActiveCssTemplate activeCssTemplate = null;
            activeCssTemplate = new ActiveCssTemplate(cssData);

            BridgeHtmlDocument bridgeHtmlDoc = (BridgeHtmlDocument)htmldoc;
            bridgeHtmlDoc.ActiveCssTemplate = activeCssTemplate;

            htmldoc.SetDocumentState(DocumentState.Building);
            //----------------------------------------------------------------  
            RootElement bridgeRoot = (RootElement)htmldoc.RootNode;
            PrepareStylesAndContentOfChildNodes(bridgeRoot, activeCssTemplate);

            //----------------------------------------------------------------  
            rootBox = BoxCreator.CreateRootBlock(iFonts);
            ((HtmlElement)htmldoc.RootNode).SetPrincipalBox(rootBox);

            BoxCreator.GenerateChildBoxes((RootElement)htmldoc.RootNode, true);

            htmldoc.SetDocumentState(DocumentState.Idle);
            //----------------------------------------------------------------  
            SetTextSelectionStyle(htmlContainer, cssData);
            return rootBox;
        }

        //----------------------------------------------------------------
        public CssBox RefreshCssTree(WebDocument htmldoc,
          IFonts iFonts,
          HtmlIsland htmlContainer)
        {

            CssBox rootBox = null;
            BridgeHtmlDocument bridgeHtmlDoc = (BridgeHtmlDocument)htmldoc;
            ActiveCssTemplate activeCssTemplate = bridgeHtmlDoc.ActiveCssTemplate;

            htmldoc.SetDocumentState(DocumentState.Building);
            //----------------------------------------------------------------  
            RootElement bridgeRoot = (RootElement)htmldoc.RootNode;
            PrepareStylesAndContentOfChildNodes(bridgeRoot, activeCssTemplate);

            //----------------------------------------------------------------  
            CssBox principalBox = RootElement.InternalGetPrincipalBox(bridgeRoot);
            principalBox.Clear();
            BoxCreator.GenerateChildBoxes((RootElement)htmldoc.RootNode, false);

            htmldoc.SetDocumentState(DocumentState.Idle);
            //----------------------------------------------------------------  

            return rootBox;
        }
        //------------------------------------------
        #region Private methods
#if DEBUG
        static void dbugTestParsePerformance(string htmlstr)
        {
            return;
            System.Diagnostics.Stopwatch sw1 = new System.Diagnostics.Stopwatch();


            sw1.Reset();
            GC.Collect();
            //sw1.Start();
            int nround = 100;
            var snapSource = new TextSnapshot(htmlstr.ToCharArray());
            //for (int i = nround; i >= 0; --i)
            //{
            //    CssBox root1 = HtmlParser.ParseDocument(snapSource);
            //}
            //sw1.Stop();
            //long ee1 = sw1.ElapsedTicks;
            //long ee1_ms = sw1.ElapsedMilliseconds;


            //sw1.Reset();
            //GC.Collect();
            sw1.Start();
            //for (int i = nround; i >= 0; --i)
            //{
            //    CssBox root2 = ParseDocument(snapSource);
            //}
            //sw1.Stop();
            //long ee2 = sw1.ElapsedTicks;
            //long ee2_ms = sw1.ElapsedMilliseconds;

        }
#endif
        void ApplyStyleSheetForSingleBridgeElement(
          HtmlElement element,
          BoxSpec parentSpec,
          ActiveCssTemplate activeCssTemplate)
        {
            BoxSpec curSpec = element.Spec;
            //0. 
            BoxSpec.InheritStyles(curSpec, parentSpec);
            //--------------------------------
            string classValue = null;
            if (element.HasAttributeClass)
            {
                classValue = element.AttrClassValue;
            }

            //--------------------------------
            //1. apply style  
            activeCssTemplate.ApplyActiveTemplate(element.LocalName,
               classValue,//class
               curSpec,
               parentSpec);

            //-------------------------------------------------------------------  
            //2. specific id 
            if (element.HasAttributeElementId)
            {
                // element.ElementId;
                activeCssTemplate.ApplyActiveTemplateForSpecificElementId(element);
            }
            //3. some html translate attributes

            if (element.WellknownElementName != WellKnownDomNodeName.svg)
            {
                //translate svg attributes
                AssignStylesFromTranslatedAttributesHTML5(element);
            }
            else
            {
                AssignSvgAttributes(element);

            }

            //AssignStylesFromTranslatedAttributes_Old(box, activeCssTemplate);
            //------------------------------------------------------------------- 
            //4. a style attribute value 
            //'style' object of this element
            if (!element.IsStyleEvaluated)
            {
                CssRuleSet parsedRuleSet = null;
                string attrStyleValue;
                if (element.TryGetAttribute(WellknownName.Style, out attrStyleValue))
                {
                    parsedRuleSet = miniCssParser.ParseCssPropertyDeclarationList(attrStyleValue.ToCharArray());
                    //step up version number
                    //after apply style  
                    BoxSpec.SetVersionNumber(curSpec, curSpec.VersionNumber + 1);
                    if (curSpec.IsFreezed)
                    {
                        curSpec.Defreeze();
                    }
                    foreach (WebDom.CssPropertyDeclaration propDecl in parsedRuleSet.GetAssignmentIter())
                    {
                        SpecSetter.AssignPropertyValue(
                            curSpec,
                            parentSpec,
                            propDecl);
                    }
                }
                //----------------------------------------------------------------
                element.IsStyleEvaluated = true;
                element.ElementRuleSet = parsedRuleSet;
            }
            else
            {
                var elemRuleSet = element.ElementRuleSet;
                if (elemRuleSet != null)
                {
                    BoxSpec.SetVersionNumber(curSpec, curSpec.VersionNumber + 1);
                    if (curSpec.IsFreezed)
                    {
                        curSpec.Defreeze();
                    }
                    foreach (WebDom.CssPropertyDeclaration propDecl in elemRuleSet.GetAssignmentIter())
                    {
                        SpecSetter.AssignPropertyValue(
                            curSpec,
                            parentSpec,
                            propDecl);
                    }

                }

            }



            //===================== 
            curSpec.Freeze(); //***
            //===================== 
        }
        void ApplyStyleSheetTopDownForBridgeElement(HtmlElement element, BoxSpec parentSpec, ActiveCssTemplate activeCssTemplate)
        {

            ApplyStyleSheetForSingleBridgeElement(element, parentSpec, activeCssTemplate);
            BoxSpec curSpec = element.Spec;

            int n = element.ChildrenCount;
            for (int i = 0; i < n; ++i)
            {
                HtmlElement childElement = element.GetChildNode(i) as HtmlElement;
                if (childElement != null)
                {
                    ApplyStyleSheetTopDownForBridgeElement(childElement, curSpec, activeCssTemplate);
                }
            }
        }


        /// <summary>
        /// Set the selected text style (selection text color and background color).
        /// </summary>
        /// <param name="htmlContainer"> </param>
        /// <param name="cssData">the style data</param>
        static void SetTextSelectionStyle(HtmlIsland htmlContainer, CssActiveSheet cssData)
        {
            //comment out for another technique
 

            //foreach (var block in cssData.GetCssRuleSetIter("::selection"))
            //{
            //    if (block.Properties.ContainsKey("color"))
            //        htmlContainer.SelectionForeColor = CssValueParser.GetActualColor(block.GetPropertyValueAsString("color"));
            //    if (block.Properties.ContainsKey("background-color"))
            //        htmlContainer.SelectionBackColor = CssValueParser.GetActualColor(block.GetPropertyValueAsString("background-color"));
            //}

            //if (cssData.ContainsCssBlock("::selection"))
            //{
            //    var blocks = cssData.GetCssBlock("::selection");
            //    foreach (var block in blocks)
            //    {

            //    }
            //}
        }
        private static void AssignStylesForElementId(CssBox box, ActiveCssTemplate activeCssTemplate, string elementId)
        {

            throw new NotSupportedException();
            //foreach (var ruleSet in cssData.GetCssRuleSetIter(elementId))
            //{
            //    if (IsBlockAssignableToBox(box, ruleSet))
            //    {
            //        AssignStyleToCssBox(box, ruleSet);
            //    }
            //}
        }




        //        static void AssignStylesFromTranslatedAttributes_Old(CssBox box, ActiveCssTemplate activeTemplate)
        //        {
        //            //some html attr contains css value 
        //            IHtmlElement tag = box.HtmlElement;
        //            if (tag.HasAttributes())
        //            {
        //                foreach (IHtmlAttribute attr in tag.GetAttributeIter())
        //                {
        //                    //attr switch by wellknown property name 
        //                    switch ((WebDom.WellknownHtmlName)attr.LocalNameIndex)
        //                    {
        //                        case WebDom.WellknownHtmlName.Align:
        //                            {
        //                                //align attribute -- deprecated in HTML5

        //                                string value = attr.Value.ToLower();
        //                                if (value == "left"
        //                                    || value == "center"
        //                                    || value == "right"
        //                                    || value == "justify")
        //                                {
        //                                    WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
        //                                        value, WebDom.CssValueHint.Iden);

        //                                    box.CssTextAlign = UserMapUtil.GetTextAlign(propValue);
        //                                }
        //                                else
        //                                {
        //                                    WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
        //                                     value, WebDom.CssValueHint.Iden);
        //                                    box.VerticalAlign = UserMapUtil.GetVerticalAlign(propValue);
        //                                }
        //                                break;
        //                            }
        //                        case WebDom.WellknownHtmlName.Background:
        //                            box.BackgroundImageBinder = new ImageBinder(attr.Value.ToLower());
        //                            break;
        //                        case WebDom.WellknownHtmlName.BackgroundColor:
        //                            box.BackgroundColor = CssValueParser.GetActualColor(attr.Value.ToLower());
        //                            break;
        //                        case WebDom.WellknownHtmlName.Border:
        //                            {
        //                                //not support in HTML5 
        //                                CssLength borderLen = TranslateLength(UserMapUtil.MakeBorderLength(attr.Value.ToLower()));
        //                                if (!borderLen.HasError)
        //                                {

        //                                    if (borderLen.Number > 0)
        //                                    {
        //                                        box.BorderLeftStyle =
        //                                            box.BorderTopStyle =
        //                                            box.BorderRightStyle =
        //                                            box.BorderBottomStyle = CssBorderStyle.Solid;
        //                                    }

        //                                    box.BorderLeftWidth =
        //                                    box.BorderTopWidth =
        //                                    box.BorderRightWidth =
        //                                    box.BorderBottomWidth = borderLen;

        //                                    if (tag.WellknownTagName == WellknownHtmlTagName.table && borderLen.Number > 0)
        //                                    {
        //                                        //Cascades to the TD's the border spacified in the TABLE tag.
        //                                        var borderWidth = CssLength.MakePixelLength(1);
        //                                        ForEachCellInTable(box, cell =>
        //                                        {
        //                                            //for all cells
        //                                            cell.BorderLeftStyle = cell.BorderTopStyle = cell.BorderRightStyle = cell.BorderBottomStyle = CssBorderStyle.Solid; // CssConstants.Solid;
        //                                            cell.BorderLeftWidth = cell.BorderTopWidth = cell.BorderRightWidth = cell.BorderBottomWidth = borderWidth;
        //                                        });

        //                                    }

        //                                }
        //                            } break;
        //                        case WebDom.WellknownHtmlName.BorderColor:

        //                            box.BorderLeftColor =
        //                                box.BorderTopColor =
        //                                box.BorderRightColor =
        //                                box.BorderBottomColor = CssValueParser.GetActualColor(attr.Value.ToLower());

        //                            break;
        //                        case WebDom.WellknownHtmlName.CellSpacing:

        //                            //html5 not support in HTML5, use CSS instead
        //                            box.BorderSpacingHorizontal = box.BorderSpacingVertical = TranslateLength(attr);

        //                            break;
        //                        case WebDom.WellknownHtmlName.CellPadding:
        //                            {
        //                                //html5 not support in HTML5, use CSS instead ***

        //                                CssLength len01 = UserMapUtil.ParseGenericLength(attr.Value.ToLower());
        //                                if (len01.HasError && (len01.Number > 0))
        //                                {
        //                                    CssLength len02 = CssLength.MakePixelLength(len01.Number);
        //                                    ForEachCellInTable(box, cell =>
        //                                    {
        //#if DEBUG
        //                                        // cell.dbugBB = dbugTT++;
        //#endif
        //                                        cell.PaddingLeft = cell.PaddingTop = cell.PaddingRight = cell.PaddingBottom = len02;
        //                                    });

        //                                }
        //                                else
        //                                {
        //                                    ForEachCellInTable(box, cell =>
        //                                         cell.PaddingLeft = cell.PaddingTop = cell.PaddingRight = cell.PaddingBottom = len01);
        //                                }

        //                            } break;
        //                        case WebDom.WellknownHtmlName.Color:

        //                            box.Color = CssValueParser.GetActualColor(attr.Value.ToLower());
        //                            break;
        //                        case WebDom.WellknownHtmlName.Dir:
        //                            {
        //                                WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
        //                                        attr.Value.ToLower(), WebDom.CssValueHint.Iden);
        //                                box.CssDirection = UserMapUtil.GetCssDirection(propValue);
        //                            }
        //                            break;
        //                        case WebDom.WellknownHtmlName.Face:
        //                            box.FontFamily = CssParser.ParseFontFamily(attr.Value.ToLower());
        //                            break;
        //                        case WebDom.WellknownHtmlName.Height:
        //                            box.Height = TranslateLength(attr);
        //                            break;
        //                        case WebDom.WellknownHtmlName.HSpace:
        //                            box.MarginRight = box.MarginLeft = TranslateLength(attr);
        //                            break;
        //                        case WebDom.WellknownHtmlName.Nowrap:
        //                            box.WhiteSpace = CssWhiteSpace.NoWrap;
        //                            break;
        //                        case WebDom.WellknownHtmlName.Size:
        //                            {
        //                                switch (tag.WellknownTagName)
        //                                {
        //                                    case WellknownHtmlTagName.hr:
        //                                        {
        //                                            box.Height = TranslateLength(attr);
        //                                        } break;
        //                                    case WellknownHtmlTagName.font:
        //                                        {
        //                                            //font tag is not support in Html5
        //                                            var ruleset = activeTemplate.ParseCssBlock("", attr.Value.ToLower());
        //                                            foreach (WebDom.CssPropertyDeclaration propDecl in ruleset.GetAssignmentIter())
        //                                            {
        //                                                //assign each property
        //                                                CssPropSetter.AssignPropertyValue(
        //                                                    box.Spec,
        //                                                    box.ParentBox.Spec,
        //                                                    propDecl);
        //                                            }

        //                                        } break;
        //                                }
        //                            } break;
        //                        case WebDom.WellknownHtmlName.VAlign:
        //                            {
        //                                WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
        //                                          attr.Value.ToLower(), WebDom.CssValueHint.Iden);
        //                                box.VerticalAlign = UserMapUtil.GetVerticalAlign(propValue);
        //                            } break;
        //                        case WebDom.WellknownHtmlName.VSpace:
        //                            box.MarginTop = box.MarginBottom = TranslateLength(attr);
        //                            break;
        //                        case WebDom.WellknownHtmlName.Width:
        //                            box.Width = TranslateLength(attr);
        //                            break;
        //                    }
        //                }
        //            }
        //        }
        //static void AssignStylesFromTranslatedAttributesHTML5(CssBox box, ActiveCssTemplate activeTemplate)
        //{
        //    return;
        //    //some html attr contains css value 
        //    IHtmlElement tag = box.HtmlElement;

        //    if (tag.HasAttributes())
        //    {
        //        foreach (IHtmlAttribute attr in tag.GetAttributeIter())
        //        {
        //            //attr switch by wellknown property name 
        //            switch ((WebDom.WellknownHtmlName)attr.LocalNameIndex)
        //            {
        //                case WebDom.WellknownHtmlName.Align:
        //                    {
        //                        //deprecated in HTML4.1
        //                        //string value = attr.Value.ToLower();
        //                        //if (value == "left"
        //                        //    || value == "center"
        //                        //    || value == "right"
        //                        //    || value == "justify")
        //                        //{
        //                        //    WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
        //                        //        value, WebDom.CssValueHint.Iden);

        //                        //    box.CssTextAlign = UserMapUtil.GetTextAlign(propValue);
        //                        //}
        //                        //else
        //                        //{
        //                        //    WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
        //                        //     value, WebDom.CssValueHint.Iden);
        //                        //    box.VerticalAlign = UserMapUtil.GetVerticalAlign(propValue);
        //                        //}
        //                        //break;
        //                    } break;
        //                case WebDom.WellknownHtmlName.Background:
        //                    //deprecated in HTML4.1
        //                    //box.BackgroundImageBinder = new ImageBinder(attr.Value.ToLower());
        //                    break;
        //                case WebDom.WellknownHtmlName.BackgroundColor:
        //                    //deprecated in HTML5
        //                    //box.BackgroundColor = CssValueParser.GetActualColor(attr.Value.ToLower());
        //                    break;
        //                case WebDom.WellknownHtmlName.Border:
        //                    {
        //                        //not support in HTML5 
        //                        //CssLength borderLen = TranslateLength(UserMapUtil.MakeBorderLength(attr.Value.ToLower()));
        //                        //if (!borderLen.HasError)
        //                        //{

        //                        //    if (borderLen.Number > 0)
        //                        //    {
        //                        //        box.BorderLeftStyle =
        //                        //            box.BorderTopStyle =
        //                        //            box.BorderRightStyle =
        //                        //            box.BorderBottomStyle = CssBorderStyle.Solid;
        //                        //    }

        //                        //    box.BorderLeftWidth =
        //                        //    box.BorderTopWidth =
        //                        //    box.BorderRightWidth =
        //                        //    box.BorderBottomWidth = borderLen;

        //                        //    if (tag.WellknownTagName == WellknownHtmlTagName.TABLE && borderLen.Number > 0)
        //                        //    {
        //                        //        //Cascades to the TD's the border spacified in the TABLE tag.
        //                        //        var borderWidth = CssLength.MakePixelLength(1);
        //                        //        ForEachCellInTable(box, cell =>
        //                        //        {
        //                        //            //for all cells
        //                        //            cell.BorderLeftStyle = cell.BorderTopStyle = cell.BorderRightStyle = cell.BorderBottomStyle = CssBorderStyle.Solid; // CssConstants.Solid;
        //                        //            cell.BorderLeftWidth = cell.BorderTopWidth = cell.BorderRightWidth = cell.BorderBottomWidth = borderWidth;
        //                        //        });

        //                        //    }

        //                        //}
        //                    } break;
        //                case WebDom.WellknownHtmlName.BorderColor:

        //                    //box.BorderLeftColor =
        //                    //    box.BorderTopColor =
        //                    //    box.BorderRightColor =
        //                    //    box.BorderBottomColor = CssValueParser.GetActualColor(attr.Value.ToLower());

        //                    break;
        //                case WebDom.WellknownHtmlName.CellSpacing:

        //                    //html5 not support in HTML5, use CSS instead
        //                    //box.BorderSpacingHorizontal = box.BorderSpacingVertical = TranslateLength(attr);

        //                    break;
        //                case WebDom.WellknownHtmlName.CellPadding:
        //                    {
        //                        //html5 not support in HTML5, use CSS instead ***

        //                        //                                CssLength len01 = UserMapUtil.ParseGenericLength(attr.Value.ToLower());
        //                        //                                if (len01.HasError && (len01.Number > 0))
        //                        //                                {
        //                        //                                    CssLength len02 = CssLength.MakePixelLength(len01.Number);
        //                        //                                    ForEachCellInTable(box, cell =>
        //                        //                                    {
        //                        //#if DEBUG
        //                        //                                        // cell.dbugBB = dbugTT++;
        //                        //#endif
        //                        //                                        cell.PaddingLeft = cell.PaddingTop = cell.PaddingRight = cell.PaddingBottom = len02;
        //                        //                                    });

        //                        //                                }
        //                        //                                else
        //                        //                                {
        //                        //                                    ForEachCellInTable(box, cell =>
        //                        //                                         cell.PaddingLeft = cell.PaddingTop = cell.PaddingRight = cell.PaddingBottom = len01);
        //                        //                                }

        //                    } break;
        //                case WebDom.WellknownHtmlName.Color:

        //                    //deprecate  
        //                    // box.Color = CssValueParser.GetActualColor(attr.Value.ToLower());
        //                    break;
        //                case WebDom.WellknownHtmlName.Dir:
        //                    {
        //                        WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
        //                                attr.Value.ToLower(), WebDom.CssValueHint.Iden);
        //                        box.CssDirection = UserMapUtil.GetCssDirection(propValue);
        //                    }
        //                    break;
        //                case WebDom.WellknownHtmlName.Face:
        //                    //deprecate
        //                    //box.FontFamily = CssParser.ParseFontFamily(attr.Value.ToLower());
        //                    break;
        //                case WebDom.WellknownHtmlName.Height:
        //                    box.Height = TranslateLength(attr);
        //                    break;
        //                case WebDom.WellknownHtmlName.HSpace:
        //                    //deprecated
        //                    //box.MarginRight = box.MarginLeft = TranslateLength(attr);
        //                    break;
        //                case WebDom.WellknownHtmlName.Nowrap:
        //                    //deprecate
        //                    //box.WhiteSpace = CssWhiteSpace.NoWrap;
        //                    break;
        //                case WebDom.WellknownHtmlName.Size:
        //                    {
        //                        //deprecate 
        //                        //switch (tag.WellknownTagName)
        //                        //{
        //                        //    case WellknownHtmlTagName.HR:
        //                        //        {
        //                        //            box.Height = TranslateLength(attr);
        //                        //        } break;
        //                        //    case WellknownHtmlTagName.FONT:
        //                        //        {
        //                        //            var ruleset = activeTemplate.ParseCssBlock("", attr.Value.ToLower());
        //                        //            foreach (WebDom.CssPropertyDeclaration propDecl in ruleset.GetAssignmentIter())
        //                        //            {
        //                        //                //assign each property
        //                        //                AssignPropertyValue(box, box.ParentBox, propDecl);
        //                        //            }
        //                        //            //WebDom.CssCodePrimitiveExpression prim = new WebDom.CssCodePrimitiveExpression(value, 
        //                        //            //box.SetFontSize(value);
        //                        //        } break;
        //                        //}
        //                    } break;
        //                case WebDom.WellknownHtmlName.VAlign:
        //                    {
        //                        //w3.org 
        //                        //valign for table display elements:
        //                        //col,colgroup,tbody,td,tfoot,th,thead,tr

        //                        WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
        //                                  attr.Value.ToLower(), WebDom.CssValueHint.Iden);
        //                        box.VerticalAlign = UserMapUtil.GetVerticalAlign(propValue);


        //                    } break;
        //                case WebDom.WellknownHtmlName.VSpace:
        //                    //deprecated
        //                    //box.MarginTop = box.MarginBottom = TranslateLength(attr);
        //                    break;
        //                case WebDom.WellknownHtmlName.Width:

        //                    box.Width = TranslateLength(attr);
        //                    break;
        //            }
        //        }
        //    }
        //}

        static void AssignSvgAttributes(HtmlElement tag)
        {
            SvgCreator.TranslateSvgAttributesMain(tag);
        }
        static void AssignStylesFromTranslatedAttributesHTML5(HtmlElement tag)
        {
            //some html attr contains css value  

            if (tag.AttributeCount > 0)
            {
                foreach (var attr in tag.GetAttributeIterForward())
                {
                    //attr switch by wellknown property name 
                    switch ((WebDom.WellknownName)attr.LocalNameIndex)
                    {
                        case WebDom.WellknownName.Align:
                            {
                                //deprecated in HTML4.1
                                //string value = attr.Value.ToLower();
                                //if (value == "left"
                                //    || value == "center"
                                //    || value == "right"
                                //    || value == "justify")
                                //{
                                //    WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
                                //        value, WebDom.CssValueHint.Iden);

                                //    box.CssTextAlign = UserMapUtil.GetTextAlign(propValue);
                                //}
                                //else
                                //{
                                //    WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
                                //     value, WebDom.CssValueHint.Iden);
                                //    box.VerticalAlign = UserMapUtil.GetVerticalAlign(propValue);
                                //}
                                //break;
                            } break;
                        case WebDom.WellknownName.Background:
                            //deprecated in HTML4.1
                            //box.BackgroundImageBinder = new ImageBinder(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownName.BackgroundColor:
                            //deprecated in HTML5
                            //box.BackgroundColor = CssValueParser.GetActualColor(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownName.Border:
                            {
                                //not support in HTML5 
                                //CssLength borderLen = TranslateLength(UserMapUtil.MakeBorderLength(attr.Value.ToLower()));
                                //if (!borderLen.HasError)
                                //{

                                //    if (borderLen.Number > 0)
                                //    {
                                //        box.BorderLeftStyle =
                                //            box.BorderTopStyle =
                                //            box.BorderRightStyle =
                                //            box.BorderBottomStyle = CssBorderStyle.Solid;
                                //    }

                                //    box.BorderLeftWidth =
                                //    box.BorderTopWidth =
                                //    box.BorderRightWidth =
                                //    box.BorderBottomWidth = borderLen;

                                //    if (tag.WellknownTagName == WellknownHtmlTagName.TABLE && borderLen.Number > 0)
                                //    {
                                //        //Cascades to the TD's the border spacified in the TABLE tag.
                                //        var borderWidth = CssLength.MakePixelLength(1);
                                //        ForEachCellInTable(box, cell =>
                                //        {
                                //            //for all cells
                                //            cell.BorderLeftStyle = cell.BorderTopStyle = cell.BorderRightStyle = cell.BorderBottomStyle = CssBorderStyle.Solid; // CssConstants.Solid;
                                //            cell.BorderLeftWidth = cell.BorderTopWidth = cell.BorderRightWidth = cell.BorderBottomWidth = borderWidth;
                                //        });

                                //    }

                                //}
                            } break;
                        case WebDom.WellknownName.BorderColor:

                            //box.BorderLeftColor =
                            //    box.BorderTopColor =
                            //    box.BorderRightColor =
                            //    box.BorderBottomColor = CssValueParser.GetActualColor(attr.Value.ToLower());

                            break;
                        case WebDom.WellknownName.CellSpacing:

                            //html5 not support in HTML5, use CSS instead
                            //box.BorderSpacingHorizontal = box.BorderSpacingVertical = TranslateLength(attr);

                            break;
                        case WebDom.WellknownName.CellPadding:
                            {
                                //html5 not support in HTML5, use CSS instead ***

                                //                                CssLength len01 = UserMapUtil.ParseGenericLength(attr.Value.ToLower());
                                //                                if (len01.HasError && (len01.Number > 0))
                                //                                {
                                //                                    CssLength len02 = CssLength.MakePixelLength(len01.Number);
                                //                                    ForEachCellInTable(box, cell =>
                                //                                    {
                                //#if DEBUG
                                //                                        // cell.dbugBB = dbugTT++;
                                //#endif
                                //                                        cell.PaddingLeft = cell.PaddingTop = cell.PaddingRight = cell.PaddingBottom = len02;
                                //                                    });

                                //                                }
                                //                                else
                                //                                {
                                //                                    ForEachCellInTable(box, cell =>
                                //                                         cell.PaddingLeft = cell.PaddingTop = cell.PaddingRight = cell.PaddingBottom = len01);
                                //                                }

                            } break;
                        case WebDom.WellknownName.Color:

                            //deprecate  
                            // box.Color = CssValueParser.GetActualColor(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownName.Dir:
                            {
                                WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
                                        attr.Value.ToLower(), WebDom.CssValueHint.Iden);
                                //assign 
                                var spec = tag.Spec;
                                spec.CssDirection = UserMapUtil.GetCssDirection(propValue);
                            }
                            break;
                        case WebDom.WellknownName.Face:
                            //deprecate
                            //box.FontFamily = CssParser.ParseFontFamily(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownName.Height:
                            {
                                var spec = tag.Spec;
                                spec.Height = TranslateLength(attr);

                            } break;
                        case WebDom.WellknownName.HSpace:
                            //deprecated
                            //box.MarginRight = box.MarginLeft = TranslateLength(attr);
                            break;
                        case WebDom.WellknownName.Nowrap:
                            //deprecate
                            //box.WhiteSpace = CssWhiteSpace.NoWrap;
                            break;
                        case WebDom.WellknownName.Size:
                            {
                                //deprecate 
                                //switch (tag.WellknownTagName)
                                //{
                                //    case WellknownHtmlTagName.HR:
                                //        {
                                //            box.Height = TranslateLength(attr);
                                //        } break;
                                //    case WellknownHtmlTagName.FONT:
                                //        {
                                //            var ruleset = activeTemplate.ParseCssBlock("", attr.Value.ToLower());
                                //            foreach (WebDom.CssPropertyDeclaration propDecl in ruleset.GetAssignmentIter())
                                //            {
                                //                //assign each property
                                //                AssignPropertyValue(box, box.ParentBox, propDecl);
                                //            }
                                //            //WebDom.CssCodePrimitiveExpression prim = new WebDom.CssCodePrimitiveExpression(value, 
                                //            //box.SetFontSize(value);
                                //        } break;
                                //}
                            } break;
                        case WebDom.WellknownName.VAlign:
                            {
                                //w3.org 
                                //valign for table display elements:
                                //col,colgroup,tbody,td,tfoot,th,thead,tr

                                WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
                                          attr.Value.ToLower(), WebDom.CssValueHint.Iden);
                                tag.Spec.VerticalAlign = UserMapUtil.GetVerticalAlign(propValue);


                            } break;
                        case WebDom.WellknownName.VSpace:
                            //deprecated
                            //box.MarginTop = box.MarginBottom = TranslateLength(attr);
                            break;
                        case WebDom.WellknownName.Width:
                            {
                                var spec = tag.Spec;
                                spec.Width = TranslateLength(attr);

                            } break;
                    }
                }
            }
        }
#if DEBUG
        static int dbugTT = 0;
#endif
        /// <summary>
        /// Converts an HTML length into a Css length
        /// </summary>
        /// <param name="htmlLength"></param>
        /// <returns></returns>
        public static CssLength TranslateLength(DomAttribute attr)
        {
            return UserMapUtil.TranslateLength(attr.Value.ToLower());

        }
        private static CssLength TranslateLength(CssLength len)
        {
            if (len.HasError)
            {
                //if unknown unit number
                return CssLength.MakePixelLength(len.Number);
            }
            return len;
        }
        static void ForEachCellInTable(CssBox table, Action<CssBox> cellAction)
        {
            foreach (var c1 in table.GetChildBoxIter())
            {
                foreach (var c2 in c1.GetChildBoxIter())
                {
                    if (c2.CssDisplay == CssDisplay.TableCell)
                    {
                        cellAction(c2);
                    }
                    else
                    {
                        foreach (var c3 in c2.GetChildBoxIter())
                        {
                            cellAction(c3);
                        }
                    }
                }
            }
        }

        #endregion
    }





}