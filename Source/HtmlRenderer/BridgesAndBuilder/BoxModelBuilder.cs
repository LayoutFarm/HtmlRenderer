//BSD 2014, WinterDev

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
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;

using HtmlRenderer.Entities;
using HtmlRenderer.Handlers;
using HtmlRenderer.Utils;
using HtmlRenderer.Parse;

using HtmlRenderer.WebDom;


namespace HtmlRenderer.Dom
{
    /// <summary>
    /// Handle css DOM tree generation from raw html and stylesheet.
    /// </summary>
    static partial class BoxModelBuilder
    {
        //======================================

        /// <summary>
        /// Parses the source html to css boxes tree structure.
        /// </summary>
        /// <param name="source">the html source to parse</param>
        static WebDom.HtmlDocument ParseDocument(TextSnapshot snapSource)
        {
            var parser = new HtmlRenderer.WebDom.Parser.HtmlParser();
            //------------------------
            parser.Parse(snapSource);
            return parser.ResultHtmlDoc;
        }

        static BrigeRootElement CreateBridgeTree(HtmlContainer container,
            WebDom.HtmlDocument htmldoc,
            ActiveCssTemplate activeCssTemplate)
        {
            BrigeRootElement bridgeRoot = new BrigeRootElement(htmldoc.RootNode);
            BuildBridgeContent(container, htmldoc.RootNode, bridgeRoot, activeCssTemplate);




            return bridgeRoot;
        }
        static void BuildBridgeContent(
            HtmlContainer container,
            WebDom.HtmlElement parentHtmlNode,
            BridgeHtmlElement parentBridge,
            ActiveCssTemplate activeCssTemplate)
        {
            //recursive 
            foreach (WebDom.HtmlNode node in parentHtmlNode.GetChildNodeIterForward())
            {
                switch (node.NodeType)
                {
                    case WebDom.HtmlNodeType.OpenElement:
                    case WebDom.HtmlNodeType.ShortElement:
                        {
                            WebDom.HtmlElement elemNode = node as WebDom.HtmlElement;

                            BridgeHtmlElement bridgeElement = new BridgeHtmlElement(elemNode,
                                UserMapUtil.EvaluateTagName(elemNode.LocalName));
                            parentBridge.AddChildElement(bridgeElement);

                            //-----------------------------
                            //recursive 
                            BuildBridgeContent(container, elemNode, bridgeElement, activeCssTemplate);
                            //-----------------------------

                            switch (bridgeElement.WellknownTagName)
                            {
                                case WellknownHtmlTagName.style:
                                    {
                                        //style element should have textnode child
                                        switch (bridgeElement.ChildCount)
                                        {
                                            default:
                                                {
                                                    throw new NotSupportedException();
                                                }
                                            case 1:
                                                {
                                                    BridgeHtmlTextNode textNode = (BridgeHtmlTextNode)bridgeElement.GetNode(0);
                                                    activeCssTemplate.LoadRawStyleElementContent(new string(textNode.CopyTextBuffer()));
                                                } break;
                                        }

                                    } break;
                                case WellknownHtmlTagName.link:
                                    {

                                        if (bridgeElement.GetAttributeValue("rel", string.Empty).Equals("stylesheet", StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            //load 

                                            string stylesheet;
                                            CssActiveSheet stylesheetData;
                                            StylesheetLoadHandler.LoadStylesheet(container,
                                                bridgeElement.GetAttributeValue("href", string.Empty),  //load style sheet from external ?
                                                out stylesheet, out stylesheetData);

                                            if (stylesheet != null)
                                            {
                                                activeCssTemplate.LoadRawStyleElementContent(stylesheet);
                                            }
                                            else if (stylesheetData != null)
                                            {
                                                activeCssTemplate.LoadAnotherStylesheet(stylesheetData);
                                            }
                                        }
                                    } break;
                            }

                        } break;
                    case WebDom.HtmlNodeType.TextNode:
                        {
                            BridgeHtmlTextNode bridgeElement = new BridgeHtmlTextNode((WebDom.HtmlTextNode)node);
                            parentBridge.AddChildElement(bridgeElement);
                        } break;
                }
            }
        }
        static void GenerateCssBoxes(BridgeHtmlElement parentElement, CssBox parentBox)
        {

            int childCount = parentElement.ChildCount;

            switch (childCount)
            {
                case 0: { } break;
                case 1:
                    {
                        BridgeHtmlNode bridgeChild = parentElement.GetNode(0);
                        int newBox = 0;
                        switch (bridgeChild.NodeType)
                        {
                            case BridgeNodeType.Text:
                                {
                                    //parse and evaluate whitespace here !  
                                    parentBox.SetTextContent(((BridgeHtmlTextNode)bridgeChild).CopyTextBuffer());
                                } break;
                            case BridgeNodeType.Element:
                                {
<<<<<<< HEAD
=======

>>>>>>> 1.7.2105.1
                                    BridgeHtmlElement elem = (BridgeHtmlElement)bridgeChild;
                                    var spec = elem.Spec;
                                    if (spec.CssDisplay == CssDisplay.None)
                                    {
                                        return;
                                    }
                                    //if (spec.CssDisplay != CssDisplay.None)
                                    //{
                                    newBox++;
<<<<<<< HEAD

                                    CssBox box = BoxCreator.CreateBoxNotInherit(elem, parentBox);

                                    //box.InheritStyles(elem.Spec, true);
=======
                                    CssBox box = BoxCreator.CreateBoxNotInherit(parentBox, elem);
                                    box.Spec.CloneAllStylesFrom(elem.Spec);
>>>>>>> 1.7.2105.1

                                    GenerateCssBoxes(elem, box);

                                } break;
                        }

                    } break;
                default:
                    {
                        int newBox = 0;
                        for (int i = 0; i < childCount; ++i)
                        {
                            //create and correct box in one pass here !?
                            BridgeHtmlNode childNode = parentElement.GetNode(i);
                            switch (childNode.NodeType)
                            {
                                case BridgeNodeType.Text:
                                    {
                                        //create anonymous box  but not inherit ***
<<<<<<< HEAD

                                        CssBox anonText = new CssBox(parentBox, null, null);
=======
                                        var parentSpec = parentBox.Spec;
                                        CssBox anonText = new CssBox(parentBox, null, parentSpec.GetAnonVersion());
>>>>>>> 1.7.2105.1
                                        //parse and evaluate whitespace here ! 
                                        BridgeHtmlTextNode textNode = (BridgeHtmlTextNode)childNode;
                                        anonText.dbugAnonCreator = parentElement;
                                        anonText.SetTextContent(textNode.CopyTextBuffer());

                                    } break;
                                default:
                                    {
                                        BridgeHtmlElement childElement = (BridgeHtmlElement)childNode;
                                        var spec = childElement.Spec;
                                        if (spec.CssDisplay == CssDisplay.None)
                                        {
                                            continue;
                                        }

                                        newBox++;
<<<<<<< HEAD
                                        CssBox box = BoxCreator.CreateBoxNotInherit(childElement, parentBox);

                                        //box.InheritStyles(spec, true);
=======
                                        CssBox box = BoxCreator.CreateBoxNotInherit(parentBox, childElement);
                                        box.Spec.CloneAllStylesFrom(childElement.Spec);
                                        //childElement.Spec.CloneAllStylesFrom(parentBox.Spec);
>>>>>>> 1.7.2105.1

                                        GenerateCssBoxes(childElement, box);

                                    } break;
                            }

                        }
                    } break;
            }
        }

        /// <summary>
        /// Generate css tree by parsing the given html and applying the given css style data on it.
        /// </summary>
        /// <param name="html">the html to parse</param>
        /// <param name="htmlContainer">the html container to use for reference resolve</param>
        /// <param name="cssData">the css data to use</param>
        /// <returns>the root of the generated tree</returns>
        public static CssBox ParseAndBuildBoxTree(
            string html,
            HtmlContainer htmlContainer,
            CssActiveSheet cssData)
        {

            //1. parse
            HtmlDocument htmldoc = ParseDocument(new TextSnapshot(html.ToCharArray()));
            //2. active css template
            //----------------------------------------------------------------
            ActiveCssTemplate activeCssTemplate = new ActiveCssTemplate(cssData);
            //3. create bridge root
            BrigeRootElement bridgeRoot = CreateBridgeTree(htmlContainer, htmldoc, activeCssTemplate);
            //---------------------------------------------------------------- 
<<<<<<< HEAD
            //4. first spec        
            bridgeRoot.Spec = new BoxSpec();
            TopDownApplyStyleSheet(bridgeRoot, null, activeCssTemplate);
            //----------------------------------------------------------------
            //box generation
            //3. create cssbox from root
            CssBox root = BoxCreator.CreateRootBlock(bridgeRoot.Spec);
            GenerateCssBoxes(bridgeRoot, root);
=======


            //attach style to elements
            ApplyStyleSheetForBridgeElement(bridgeRoot, null, activeCssTemplate);

            //----------------------------------------------------------------
            //box generation
            //3. create cssbox from root
            CssBox rootBox = BoxCreator.CreateRootBlock(); 
            GenerateCssBoxes(bridgeRoot, rootBox);
>>>>>>> 1.7.2105.1

#if DEBUG
            dbugTestParsePerformance(html);
#endif

            //2. decorate cssbox with styles
            if (rootBox != null)
            {
<<<<<<< HEAD
                CssBox.SetHtmlContainer(root, htmlContainer);
                //ApplyStyleSheet(root, activeCssTemplate);
=======

                CssBox.SetHtmlContainer(rootBox, htmlContainer);
                //------------------------------------------------------------------- 

                var rootspec = new BoxSpec(WellknownHtmlTagName.Unknown);

                //ApplyStyleSheetForBox(rootBox, null, activeCssTemplate);
>>>>>>> 1.7.2105.1
                //-------------------------------------------------------------------
                SetTextSelectionStyle(htmlContainer, cssData);
                OnePassBoxCorrection(rootBox);
                CorrectTextBoxes(rootBox);
                //CorrectImgBoxes(root);

                bool followingBlock = true;

                CorrectLineBreaksBlocks(rootBox, ref followingBlock); 
                //1. must test first
                CorrectInlineBoxesParent(rootBox);
                //2. then ...
                CorrectBlockInsideInline(rootBox);

            }
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


<<<<<<< HEAD

        /// <summary>
        /// Applies style to all boxes in the tree.<br/>
        /// If the html tag has style defined for each apply that style to the css box of the tag.<br/>
        /// If the html tag has "class" attribute and the class name has style defined apply that style on the tag css box.<br/>
        /// If the html tag has "style" attribute parse it and apply the parsed style on the tag css box.<br/>
        /// If the html tag is "style" tag parse it content and add to the css data for all future tags parsing.<br/>
        /// If the html tag is "link" that point to style data parse it content and add to the css data for all future tags parsing.<br/>
        /// </summary>
        /// <param name="box"></param>
        /// <param name="htmlContainer">the html container to use for reference resolve</param>
        /// <param name="cssData"> </param>
        /// <param name="cssDataChanged">check if the css data has been modified by the handled html not to change the base css data</param>
        //static void ApplyStyleSheet(CssBox box, ActiveCssTemplate activeCssTemplate)
        //{
        //    //recursive  
        //    //-------------------------------------------------------------------            
        //    box.InheritStyles(box.ParentBox);

        //    if (box.HtmlElement != null)
        //    {
        //        //------------------------------------------------------------------- 
        //        //1. element tag
        //        //2. css class 
        //        // try assign style using the html element tag    
        //        activeCssTemplate.ApplyActiveTemplateForElement(box.ParentBox, box);
        //        //3.
        //        // try assign style using the "id" attribute of the html element
        //        if (box.HtmlElement.HasAttribute("id"))
        //        {
        //            var id = box.HtmlElement.TryGetAttribute("id");
        //            AssignStylesForElementId(box, activeCssTemplate, "#" + id);
        //        }
        //        //-------------------------------------------------------------------
        //        //4. 
        //        //element attribute
        //        AssignStylesFromTranslatedAttributesHTML5(box, activeCssTemplate);
        //        //AssignStylesFromTranslatedAttributes_Old(box, activeCssTemplate);
        //        //------------------------------------------------------------------- 
        //        //5.
        //        //style attribute value of element
        //        if (box.HtmlElement.HasAttribute("style"))
        //        {
        //            var ruleset = activeCssTemplate.ParseCssBlock(box.HtmlElement.Name, box.HtmlElement.TryGetAttribute("style"));
        //            foreach (WebDom.CssPropertyDeclaration propDecl in ruleset.GetAssignmentIter())
        //            {
        //                CssPropSetter.AssignPropertyValue(box, box.ParentBox, propDecl);
        //            }
        //        }
        //    }

        //    //===================================================================
        //    //parent style assignment is complete before step down into child ***
        //    foreach (var childBox in box.GetChildBoxIter())
        //    {
        //        //recursive
        //        ApplyStyleSheet(childBox, activeCssTemplate);
        //    }
        //}
        static void TopDownApplyStyleSheet(BridgeHtmlElement element, BridgeHtmlElement parentElement, ActiveCssTemplate activeCssTemplate)
        {
            //-------------------------------------------------------------------                        
            BoxSpec parentSpec = null;
            if (parentElement != null)
            {
                parentSpec = parentElement.Spec;
            }
            BoxSpec currentElementSpec = element.Spec;
            if (currentElementSpec == null)
            {
                element.Spec = currentElementSpec = new BoxSpec();
                currentElementSpec.InheritStylesFrom(parentSpec);
            }
=======
        static void ApplyStyleSheetForBridgeElement(BridgeHtmlElement element, BoxSpec parentSpec, ActiveCssTemplate activeCssTemplate)
        {

            BoxSpec curSpec = element.Spec;
            //-------------------------------
            //0.
            curSpec.InheritStylesFrom(parentSpec);

            //1. apply style  
            activeCssTemplate.ApplyActiveTemplate(element.Name,
               element.TryGetAttribute("class", null),
               curSpec,
               parentSpec);

>>>>>>> 1.7.2105.1

            //-------------------------------------------------------------------                        
            //2. specific id
            if (element.HasAttribute("id"))
            {
                throw new NotSupportedException();
                //string id = element.GetAttributeValue("id", null);
                //if (id != null)
                //{   
                //    //AssignStylesForElementId(box, activeCssTemplate, "#" + id);
                //}
            }

            //3. some html translate attributes
            AssignStylesFromTranslatedAttributesHTML5(element, activeCssTemplate);
            //AssignStylesFromTranslatedAttributes_Old(box, activeCssTemplate);
            //------------------------------------------------------------------- 
            //4. a style attribute value
            string attrStyleValue;
            if (element.TryGetAttribute2("style", out attrStyleValue))
            {
                var ruleset = activeCssTemplate.ParseCssBlock(element.Name, attrStyleValue);
                foreach (WebDom.CssPropertyDeclaration propDecl in ruleset.GetAssignmentIter())
                {
                    CssPropSetter.AssignPropertyValue(
                        curSpec,
                        parentSpec,
                        propDecl);
                }
            }
<<<<<<< HEAD
=======

            //===================================================================
            curSpec.Freeze();

>>>>>>> 1.7.2105.1
            //5. children
            //parent style assignment is complete before step down into child ***            
            int n = element.ChildCount;
            for (int i = 0; i < n; ++i)
            {
                BridgeHtmlElement childElement = element.GetNode(i) as BridgeHtmlElement;
                if (childElement != null)
                {
<<<<<<< HEAD
                    TopDownApplyStyleSheet(childElement, element, activeCssTemplate);
                }
            }

=======
                    ApplyStyleSheetForBridgeElement(childElement, curSpec, activeCssTemplate);
                }
            }
>>>>>>> 1.7.2105.1
        }


        /// <summary>
        /// Set the selected text style (selection text color and background color).
        /// </summary>
        /// <param name="htmlContainer"> </param>
        /// <param name="cssData">the style data</param>
        static void SetTextSelectionStyle(HtmlContainer htmlContainer, CssActiveSheet cssData)
        {
            //comment out for another technique
            htmlContainer.SelectionForeColor = Color.Empty;
            htmlContainer.SelectionBackColor = Color.Empty;

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



<<<<<<< HEAD
        //================================================================
        //        static void AssignStylesFromTranslatedAttributes_Old(BridgeHtmlElement elem, BoxSpec box, ActiveCssTemplate activeTemplate)
        //        {
        //            //some html attr contains css value 

        //            IHtmlElement tag = elem;
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
        //                                            BoxSpec cell_spec = cell.BoxSpec;
        //                                            cell_spec.BorderLeftStyle = cell_spec.BorderTopStyle = cell_spec.BorderRightStyle = cell_spec.BorderBottomStyle = CssBorderStyle.Solid; // CssConstants.Solid;
        //                                            cell_spec.BorderLeftWidth = cell_spec.BorderTopWidth = cell_spec.BorderRightWidth = cell_spec.BorderBottomWidth = borderWidth;
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
        //                                        var cellSpec = cell.BoxSpec;
        //                                        cellSpec.PaddingLeft = cellSpec.PaddingTop = cellSpec.PaddingRight = cellSpec.PaddingBottom = len02;
        //                                    });

        //                                }
        //                                else
        //                                {
        //                                    ForEachCellInTable(box, cell =>{
        //                                        var cellSpec = cell.BoxSpec;
        //                                        cellSpec.PaddingLeft = cellSpec.PaddingTop = cellSpec.PaddingRight = cellSpec.PaddingBottom = len01;
        //                                    });
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
        //                                                CssPropSetter.AssignPropertyValue(box, elem.Parent.Spec, propDecl);
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
        static void AssignStylesFromTranslatedAttributesHTML5(CssBox box, ActiveCssTemplate activeTemplate)
        {
            //some html attr contains css value 
            IHtmlElement tag = box.HtmlElement;

=======

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
        static void AssignStylesFromTranslatedAttributesHTML5(BridgeHtmlElement tag, ActiveCssTemplate activeTemplate)
        {
            //some html attr contains css value  
            
>>>>>>> 1.7.2105.1
            if (tag.HasAttributes())
            {
                foreach (IHtmlAttribute attr in tag.GetAttributeIter())
                {
                    //attr switch by wellknown property name 
                    switch ((WebDom.WellknownHtmlName)attr.LocalNameIndex)
                    {
                        case WebDom.WellknownHtmlName.Align:
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
                        case WebDom.WellknownHtmlName.Background:
                            //deprecated in HTML4.1
                            //box.BackgroundImageBinder = new ImageBinder(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.BackgroundColor:
                            //deprecated in HTML5
                            //box.BackgroundColor = CssValueParser.GetActualColor(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.Border:
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
                        case WebDom.WellknownHtmlName.BorderColor:

                            //box.BorderLeftColor =
                            //    box.BorderTopColor =
                            //    box.BorderRightColor =
                            //    box.BorderBottomColor = CssValueParser.GetActualColor(attr.Value.ToLower());

                            break;
                        case WebDom.WellknownHtmlName.CellSpacing:

                            //html5 not support in HTML5, use CSS instead
                            //box.BorderSpacingHorizontal = box.BorderSpacingVertical = TranslateLength(attr);

                            break;
                        case WebDom.WellknownHtmlName.CellPadding:
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
                        case WebDom.WellknownHtmlName.Color:

                            //deprecate  
                            // box.Color = CssValueParser.GetActualColor(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.Dir:
                            {
                                WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
                                        attr.Value.ToLower(), WebDom.CssValueHint.Iden);
<<<<<<< HEAD
                                throw new NotSupportedException();
                                // box.BoxSpec.CssDirection = UserMapUtil.GetCssDirection(propValue);
=======
                                //assign 
                                var spec = tag.Spec;
                                spec.CssDirection = UserMapUtil.GetCssDirection(propValue);
>>>>>>> 1.7.2105.1
                            }
                            break;
                        case WebDom.WellknownHtmlName.Face:
                            //deprecate
                            //box.FontFamily = CssParser.ParseFontFamily(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.Height:
<<<<<<< HEAD
                            box.BoxSpec.Height = TranslateLength(attr);
                            break;
=======
                            {
                                var spec = tag.Spec;
                                spec.Height = TranslateLength(attr);

                            } break;
>>>>>>> 1.7.2105.1
                        case WebDom.WellknownHtmlName.HSpace:
                            //deprecated
                            //box.MarginRight = box.MarginLeft = TranslateLength(attr);
                            break;
                        case WebDom.WellknownHtmlName.Nowrap:
                            //deprecate
                            //box.WhiteSpace = CssWhiteSpace.NoWrap;
                            break;
                        case WebDom.WellknownHtmlName.Size:
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
                        case WebDom.WellknownHtmlName.VAlign:
                            {
                                //w3.org 
                                //valign for table display elements:
                                //col,colgroup,tbody,td,tfoot,th,thead,tr

                                WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
                                          attr.Value.ToLower(), WebDom.CssValueHint.Iden);
<<<<<<< HEAD
                                box.BoxSpec.VerticalAlign = UserMapUtil.GetVerticalAlign(propValue);
=======
                                var spec = tag.Spec;
                                spec.VerticalAlign = UserMapUtil.GetVerticalAlign(propValue);
>>>>>>> 1.7.2105.1


                            } break;
                        case WebDom.WellknownHtmlName.VSpace:
                            //deprecated
                            //box.MarginTop = box.MarginBottom = TranslateLength(attr);
                            break;
                        case WebDom.WellknownHtmlName.Width:
                            {
                                var spec = tag.Spec;
                                spec.Width = TranslateLength(attr);

<<<<<<< HEAD
                            box.BoxSpec.Width = TranslateLength(attr);
                            break;
=======
                            } break;
>>>>>>> 1.7.2105.1
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
        public static CssLength TranslateLength(IHtmlAttribute attr)
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
        static void ForEachCellInTable(BoxSpec table, Action<CssBox> cellAction)
        {
            //foreach (var c1 in table.GetChildBoxIter())
            //{
            //    foreach (var c2 in c1.GetChildBoxIter())
            //    {
            //        if (c2.WellknownTagName == WellknownHtmlTagName.td)
            //        {
            //            cellAction(c2);
            //        }
            //        else
            //        {
            //            foreach (var c3 in c2.GetChildBoxIter())
            //            {
            //                cellAction(c3);
            //            }
            //        }
            //    }
            //}
        }





#if DEBUG
        static int dbugCorrectCount = 0;
#endif

        /// <summary>
        /// Check if the given box contains only inline child boxes in all subtree.
        /// </summary>
        /// <param name="box">the box to check</param>
        /// <returns>true - only inline child boxes, false - otherwise</returns>
        static bool ContainsInlinesOnlyDeep(CssBox box)
        {
            //recursive
            foreach (var childBox in box.GetChildBoxIter())
            {
                if (!childBox.IsInline || !ContainsInlinesOnlyDeep(childBox))
                {

                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Check if the given box contains inline and block child boxes.
        /// </summary>
        /// <param name="box">the box to check</param>
        /// <returns>true - has variant child boxes, false - otherwise</returns>
        static bool ContainsMixedInlineAndBlockBoxes(CssBox box, out int mixFlags)
        {
            if (box.ChildCount == 0 && box.HasRuns)
            {
                mixFlags = HAS_IN_LINE;
                return false;
            }
            mixFlags = 0;
            var children = CssBox.UnsafeGetChildren(box);
            for (int i = children.Count - 1; i >= 0; --i)
            {
                if (children[i].IsInline)
                {
                    mixFlags |= HAS_IN_LINE;
                }
                else
                {
                    mixFlags |= HAS_BLOCK;
                }

                if (mixFlags == (HAS_BLOCK | HAS_IN_LINE))
                {
                    return true;
                }

            }
            return false;
        }


        const int HAS_BLOCK = 1 << (1 - 1);
        const int HAS_IN_LINE = 1 << (2 - 1);


        #endregion



    }
}