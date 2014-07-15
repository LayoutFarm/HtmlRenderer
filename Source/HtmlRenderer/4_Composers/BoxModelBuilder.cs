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
using System.Drawing;
using System.Collections.Generic;
using HtmlRenderer.Css;
using HtmlRenderer.WebDom;
using HtmlRenderer.WebDom.Parser;
using HtmlRenderer.Boxes;
using HtmlRenderer.Drawing;

namespace HtmlRenderer.Composers
{



    /// <summary>
    /// Handle css DOM tree generation from raw html and stylesheet.
    /// </summary>
    public class BoxModelBuilder
    {
        //======================================
        static ContentTextSplitter contentTextSplitter = new ContentTextSplitter();

        /// <summary>
        /// Parses the source html to css boxes tree structure.
        /// </summary>
        /// <param name="source">the html source to parse</param>
        static BridgeHtmlDocument ParseDocument(TextSnapshot snapSource)
        {
            var parser = new HtmlRenderer.WebDom.Parser.HtmlParser();
            //------------------------
            var blankHtmlDoc = new BridgeHtmlDocument();
            parser.Parse(snapSource, blankHtmlDoc);
            return blankHtmlDoc;
        }

        //-----------------------------------------------------------------
        static void PrepareBridgeTree(HtmlContainer container,
             WebDom.HtmlDocument htmldoc,
             ActiveCssTemplate activeCssTemplate)
        {
            BrigeRootElement bridgeRoot = (BrigeRootElement)htmldoc.RootNode;
            PrepareChildNodes(container, bridgeRoot, activeCssTemplate);
        }
        static void PrepareChildNodes(
            HtmlContainer container,
            BridgeHtmlElement parentElement,
            ActiveCssTemplate activeCssTemplate)
        {
            //recursive 
            foreach (WebDom.HtmlNode node in parentElement.GetChildNodeIterForward())
            {
                switch (node.NodeType)
                {
                    case WebDom.HtmlNodeType.OpenElement:
                    case WebDom.HtmlNodeType.ShortElement:
                        {
                            BridgeHtmlElement bridgeElement = (BridgeHtmlElement)node;
                            bridgeElement.WellknownElementName = UserMapUtil.EvaluateTagName(bridgeElement.LocalName);

                            switch (bridgeElement.WellknownElementName)
                            {
                                case WellknownElementName.style:
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
                                                        BridgeHtmlTextNode textNode = (BridgeHtmlTextNode)bridgeElement.GetChildNode(0);
                                                        activeCssTemplate.LoadRawStyleElementContent(new string(textNode.GetOriginalBuffer()));
                                                        //break
                                                        i = j;
                                                    } break;
                                            }
                                        }
                                        continue;
                                    }
                                case WellknownElementName.link:
                                    {
                                        //<link rel="stylesheet"
                                        HtmlAttribute relAttr;
                                        if (bridgeElement.TryGetAttribute(WellknownHtmlName.Rel, out relAttr)
                                            && relAttr.Value.ToLower() == "stylesheet")
                                        {
                                            //if found
                                            string stylesheet;
                                            CssActiveSheet stylesheetData;

                                            HtmlAttribute hrefAttr;
                                            if (bridgeElement.TryGetAttribute(WellknownHtmlName.Href, out hrefAttr))
                                            {
                                                HtmlContainer.RaiseRequestStyleSheet(
                                                container,
                                                hrefAttr.Value,
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
                                        }

                                        continue;
                                    }
                            }
                            //-----------------------------                            
                            //apply style for this node  
                            ApplyStyleSheetForSingleBridgeElement(bridgeElement, parentElement.Spec, activeCssTemplate);
                            //-----------------------------

                            //recursive 
                            PrepareChildNodes(container, bridgeElement, activeCssTemplate);
                            //-----------------------------
                        } break;
                    case WebDom.HtmlNodeType.TextNode:
                        {

                            BridgeHtmlTextNode textnode = (BridgeHtmlTextNode)node;
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

        static bool CorrectParentChildRelationship(
             CssBox newChildBox,
             CssBox parentBox,
             bool isLineFormattingContext)
        {

            int parentChildCount = parentBox.ChildCount;

            if (parentBox.IsBlock)
            {
                //br correction
                if (newChildBox.IsBrElement)
                {
                    CssBox.ChangeDisplayType(newChildBox, CssDisplay.Block);
                    newChildBox.DirectSetHeight(FontDefaultConfig.DEFAULT_FONT_SIZE * 0.95f);
                }
            }
            //----------
            else if (parentBox.IsInline)
            {
                if (newChildBox.IsBlock)
                {
                    //correct 
                    CssBox.ChangeDisplayType(newChildBox, CssDisplay.BlockInsideInlineAfterCorrection);
                }
            }
            else if (parentBox.CssDisplay == CssDisplay.TableCell)//(parentBox.WellknownTagName == WellknownHtmlTagName.td)
            {
                if (isLineFormattingContext)
                {
                    if (newChildBox.IsBlock)
                    {
                        CssBox.ChangeDisplayType(newChildBox, CssDisplay.BlockInsideInlineAfterCorrection); ;
                    }
                }
                else
                {
                    if (newChildBox.IsBrElement)
                    {
                        CssBox.ChangeDisplayType(newChildBox, CssDisplay.Block);
                        newChildBox.DirectSetHeight(FontDefaultConfig.DEFAULT_FONT_SIZE * 0.95f);
                    }
                }
            }
            else
            {
                //throw new NotSupportedException();
            }
            //---------- 
            if (isLineFormattingContext)
            {
                if (newChildBox.IsBlock)
                {
                    //change to block level formatting context 
                    //1. create anon block 
                    if (parentChildCount > 1)
                    {
                        var a = parentBox.GetFirstChild();
                        var upperAnon = CssBox.CreateAnonBlock(parentBox, a);
                        int ncount = parentBox.ChildCount - 1;
                        while (ncount >= 2)
                        {
                            var tmp = a.GetNextNode();
                            a.SetNewParentBox(upperAnon);
                            a = tmp;
                            ncount--;
                        }
                    }

                    //true //****
                    return true;
                    //    //------------------------
                    //    //change parent box to new context 
                    //    var lowerAnon = CssBox.CreateAnonBlock(parentBox);
                    //    parentBox = lowerAnon;
                    //    isLineFormattingContext = true;
                    //    return;
                    //    //------------------------
                    //}
                    //else
                    //{
                    //    //just lower anon
                    //    var lowerAnon = CssBox.CreateAnonBlock(parentBox);
                    //    parentBox = lowerAnon;
                    //    isLineFormattingContext = true;
                    //    return;
                    //}
                }
            }
            else
            {
                //block formatting context

                if (newChildBox.IsInline)
                {
                    newChildBox.SetNewParentBox(null);
                    var newAnonBlock = CssBox.CreateAnonBlock(parentBox);
                    newChildBox.SetNewParentBox(newAnonBlock);
                }
            }
            return false;

        }
        static void GenerateCssBoxes(BridgeHtmlElement parentElement, CssBox parentBox)
        {
            switch (parentElement.ChildrenCount)
            {
                case 0: { } break;
                case 1:
                    {
                        HtmlNode bridgeChild = parentElement.GetChildNode(0);
                        int newBox = 0;
                        switch (bridgeChild.NodeType)
                        {
                            case HtmlNodeType.TextNode:
                                {
                                    //parent has single child 
                                    BridgeHtmlTextNode singleTextNode = (BridgeHtmlTextNode)bridgeChild;
                                    //create textrun under policy  
                                    RunListHelper.AddRunList(parentBox, parentElement.Spec, singleTextNode);
                                } break;
                            case HtmlNodeType.ShortElement:
                            case HtmlNodeType.OpenElement:
                                {

                                    BridgeHtmlElement elem = (BridgeHtmlElement)bridgeChild;
                                    var spec = elem.Spec;
                                    if (spec.CssDisplay == CssDisplay.None)
                                    {
                                        return;
                                    }
                                    newBox++;

                                    CssBox box = BoxCreator.CreateBox(parentBox, elem);
                                    elem.SetPrinicalBox(box);
                                    //----------
                                    bool isInlineFormattingContext = true;
                                    CorrectParentChildRelationship(box, parentBox, isInlineFormattingContext);
                                    GenerateCssBoxes(elem, box);

                                } break;
                        }
                    } break;
                default:
                    {
                        switch (parentElement.Spec.WhiteSpace)
                        {
                            case CssWhiteSpace.Pre:
                            case CssWhiteSpace.PreWrap:
                                {
                                    CreateChildBoxesPreserveWhitespace(parentElement, parentBox);

                                } break;
                            case CssWhiteSpace.PreLine:
                                {
                                    CreateChildBoxesRespectNewLine(parentElement, parentBox);

                                } break;
                            default:
                                {
                                    CreateChildBoxesDefault(parentElement, parentBox);
                                } break;
                        }

                    } break;
            }
        } 
        
        enum CurrentLineFormattingContext
        {
            Init, //not sure if inline or block
            Inline,
            Block
        }

        static void CreateChildBoxesPreserveWhitespace(BridgeHtmlElement parentElement, CssBox parentBox)
        {

            int newBox = 0;
            int childCount = parentElement.ChildrenCount;
            //default
            bool isLineFormattingContext = true;
            bool startNewAnonBlockBeforeUse = false;

            for (int i = 0; i < childCount; ++i)
            {
                var childNode = parentElement.GetChildNode(i);
                switch (childNode.NodeType)
                {
                    case HtmlNodeType.TextNode:
                        {
                            BridgeHtmlTextNode textNode = (BridgeHtmlTextNode)childNode;
                            //-------------------------------------------------------------------------------
                            if (startNewAnonBlockBeforeUse)
                            {
                                parentBox = CssBox.CreateAnonBlock(parentBox);
                                startNewAnonBlockBeforeUse = false;
                                isLineFormattingContext = true;
                            }

                            RunListHelper.AddRunList(
                                isLineFormattingContext ? CssBox.CreateAnonInline(parentBox) : CssBox.CreateAnonBlock(parentBox),
                                parentElement.Spec, textNode);

                            newBox++;
                        } break;
                    case HtmlNodeType.ShortElement:
                    case HtmlNodeType.OpenElement:
                        {
                            BridgeHtmlElement childElement = (BridgeHtmlElement)childNode;
                            var spec = childElement.Spec;
                            if (spec.CssDisplay == CssDisplay.None)
                            {
                                continue;
                            }

                            newBox++;
                            if (startNewAnonBlockBeforeUse)
                            {
                                parentBox = CssBox.CreateAnonBlock(parentBox);
                                startNewAnonBlockBeforeUse = false;
                                isLineFormattingContext = true;
                            }

                            CssBox box = BoxCreator.CreateBox(parentBox, childElement);
                            childElement.SetPrinicalBox(box);

                            startNewAnonBlockBeforeUse = CorrectParentChildRelationship(box, parentBox, isLineFormattingContext);
                            GenerateCssBoxes(childElement, box);
                        } break;
                    default:
                        {
                        } break;
                }
            }
        }
        static void CreateChildBoxesRespectNewLine(BridgeHtmlElement parentElement, CssBox parentBox)
        {
            int newBox = 0;
            int childCount = parentElement.ChildrenCount;
            bool isLineFormattingContext = false;
            bool startNewAnonBlockBeforeUse = false;

            for (int i = 0; i < childCount; ++i)
            {
                var childNode = parentElement.GetChildNode(i);

                switch (childNode.NodeType)
                {
                    case HtmlNodeType.TextNode:
                        {
                            BridgeHtmlTextNode textNode = (BridgeHtmlTextNode)childNode;
                            if (i == 0 && textNode.IsWhiteSpace)
                            {
                                continue;//skip
                            }
                            if (startNewAnonBlockBeforeUse)
                            {
                                parentBox = CssBox.CreateAnonBlock(parentBox);
                                startNewAnonBlockBeforeUse = false;
                                isLineFormattingContext = true;
                            }
                            RunListHelper.AddRunList(
                                isLineFormattingContext ? CssBox.CreateAnonInline(parentBox) : CssBox.CreateAnonBlock(parentBox),
                                parentElement.Spec, textNode);

                            newBox++;
                        } break;
                    case HtmlNodeType.OpenElement:
                    case HtmlNodeType.ShortElement:
                        {
                            //other node type
                            BridgeHtmlElement childElement = (BridgeHtmlElement)childNode;
                            var spec = childElement.Spec;
                            if (spec.CssDisplay == CssDisplay.None)
                            {
                                continue;
                            }
                            newBox++;

                            if (startNewAnonBlockBeforeUse)
                            {
                                parentBox = CssBox.CreateAnonBlock(parentBox);
                                startNewAnonBlockBeforeUse = false;
                                isLineFormattingContext = true;
                            }

                            CssBox box = BoxCreator.CreateBox(parentBox, childElement);
                            childElement.SetPrinicalBox(box);

                            startNewAnonBlockBeforeUse = CorrectParentChildRelationship(box, parentBox, isLineFormattingContext);

                            //    //switch to new anon block
                            //    parentBox = CssBox.CreateAnonBlock(parentBox);
                            //    isLineFormattingContext = true;
                            //}

                            GenerateCssBoxes(childElement, box);

                        } break;
                    default:
                        {

                        } break;
                }
            }
        }

        static void CreateChildBoxesDefault(BridgeHtmlElement parentElement, CssBox parentBox)
        {

            int childCount = parentElement.ChildrenCount;

            //default
            bool isLineFormattingContext = true;
            bool startNewAnonBlockBeforeUse = false;

            for (int i = 0; i < childCount; ++i)
            {
                var childNode = parentElement.GetChildNode(i);
                switch (childNode.NodeType)
                {
                    case HtmlNodeType.TextNode:
                        {

                            BridgeHtmlTextNode textNode = (BridgeHtmlTextNode)childNode;
                            if (textNode.IsWhiteSpace)
                            {
                                continue;//skip
                            }
                            //-------------------------------------------------------------------------------
                            if (startNewAnonBlockBeforeUse)
                            {
                                parentBox = CssBox.CreateAnonBlock(parentBox);
                                startNewAnonBlockBeforeUse = false;
                                isLineFormattingContext = true;
                            }
                            RunListHelper.AddRunList(
                                isLineFormattingContext ? CssBox.CreateAnonInline(parentBox) : CssBox.CreateAnonBlock(parentBox),
                                parentElement.Spec, textNode);


                        } break;
                    case HtmlNodeType.ShortElement:
                    case HtmlNodeType.OpenElement:
                        {
                            BridgeHtmlElement childElement = (BridgeHtmlElement)childNode;
                            var spec = childElement.Spec;
                            if (spec.CssDisplay == CssDisplay.None)
                            {
                                continue;
                            }

                            if (startNewAnonBlockBeforeUse)
                            {
                                parentBox = CssBox.CreateAnonBlock(parentBox);
                                startNewAnonBlockBeforeUse = false;
                                isLineFormattingContext = true;
                            }


                            CssBox box = BoxCreator.CreateBox(parentBox, childElement);
                            childElement.SetPrinicalBox(box);
                            //current parent may change after validation process                            

                            startNewAnonBlockBeforeUse = CorrectParentChildRelationship(box, parentBox, isLineFormattingContext);
                            //if (CorrectParentChildRelationship(box, parentBox, isLineFormattingContext))
                            //{
                            //    //switch to new anon block
                            //    parentBox = CssBox.CreateAnonBlock(parentBox);
                            //    isLineFormattingContext = true;
                            //}
                            GenerateCssBoxes(childElement, box);

                        } break;
                    default:
                        {
                        } break;
                }
            }
        }




        /// <summary>
        /// Generate css tree by parsing the given html and applying the given css style data on it.
        /// </summary>
        /// <param name="html">the html to parse</param>
        /// <param name="htmlContainer">the html container to use for reference resolve</param>
        /// <param name="cssData">the css data to use</param>
        /// <returns>the root of the generated tree</returns>
        public CssBox ParseAndBuildBoxTree(
            string html,
            IFonts iFonts,
            HtmlContainer htmlContainer,
            CssActiveSheet cssData)
        {

#if DEBUG

#endif
            CssBox rootBox = null;
            WebDom.HtmlDocument htmldoc = null; ;
            ActiveCssTemplate activeCssTemplate = null;

            //1. parse
            //var t0 = dbugCounter.Snap(() =>
            // {
            htmldoc = ParseDocument(new TextSnapshot(html.ToCharArray()));

            // });

            // long t1 = dbugCounter.Snap(() =>
            // {
            activeCssTemplate = new ActiveCssTemplate(cssData);
            //});

            //2. active css template 
            // var t2 = dbugCounter.Snap(() =>
            // {
            //3. prepare tree
            PrepareBridgeTree(htmlContainer, htmldoc, activeCssTemplate);
            //----------------------------------------------------------------  
            //4. assign styles 
            //ApplyStyleSheetTopDownForBridgeElement(bridgeRoot, null, activeCssTemplate);
            //----------------------------------------------------------------
            //5. box generation                 
            rootBox = BoxCreator.CreateRootBlock(iFonts);
            //});

            // var t3 = dbugCounter.Snap(() =>
            // {
            GenerateCssBoxes((BrigeRootElement)htmldoc.RootNode, rootBox);
#if DEBUG
            dbugTestParsePerformance(html);
#endif


            SetTextSelectionStyle(htmlContainer, cssData);
            OnePassBoxCorrection(rootBox);
            // });


            //Console.Write("2245=> ");
            //Console.WriteLine(string.Format("t0:{0}, t1:{1}, t2:{2}, total={3}", t0, t1, t2, (t0 + t1 + t2)));
            //Console.WriteLine(t0 + t1 + t2 + t3);
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
        static void ApplyStyleSheetForSingleBridgeElement(BridgeHtmlElement element, BoxSpec parentSpec, ActiveCssTemplate activeCssTemplate)
        {
            BoxSpec curSpec = element.Spec;
            //0.
            curSpec.InheritStylesFrom(parentSpec);
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
            //if (element.TryGetAttribute(WellknownHtmlName.Id, out idValue))
            //{

            //    throw new NotSupportedException();
            //}
            //if (element.HasAttribute("id"))
            //{
            //    throw new NotSupportedException();
            //    //string id = element.GetAttributeValue("id", null);
            //    //if (id != null)
            //    //{   
            //    //    //AssignStylesForElementId(box, activeCssTemplate, "#" + id);
            //    //}
            //}

            //3. some html translate attributes
            AssignStylesFromTranslatedAttributesHTML5(element, activeCssTemplate);
            //AssignStylesFromTranslatedAttributes_Old(box, activeCssTemplate);
            //------------------------------------------------------------------- 
            //4. a style attribute value
            string attrStyleValue;

            if (element.TryGetAttribute(WellknownHtmlName.Style, out attrStyleValue))
            {
                var ruleset = activeCssTemplate.ParseCssBlock(element.LocalName, attrStyleValue);
                foreach (WebDom.CssPropertyDeclaration propDecl in ruleset.GetAssignmentIter())
                {
                    SpecSetter.AssignPropertyValue(
                        curSpec,
                        parentSpec,
                        propDecl);
                }
            }
            //===================== 
            curSpec.Freeze(); //***
            //===================== 
        }
        static void ApplyStyleSheetTopDownForBridgeElement(BridgeHtmlElement element, BoxSpec parentSpec, ActiveCssTemplate activeCssTemplate)
        {

            ApplyStyleSheetForSingleBridgeElement(element, parentSpec, activeCssTemplate);
            BoxSpec curSpec = element.Spec;

            int n = element.ChildrenCount;
            for (int i = 0; i < n; ++i)
            {
                BridgeHtmlElement childElement = element.GetChildNode(i) as BridgeHtmlElement;
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

            if (tag.AttributeCount > 0)
            {
                foreach (var attr in tag.GetAttributeIterForward())
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
                                //assign 
                                var spec = tag.Spec;
                                spec.CssDirection = UserMapUtil.GetCssDirection(propValue);
                            }
                            break;
                        case WebDom.WellknownHtmlName.Face:
                            //deprecate
                            //box.FontFamily = CssParser.ParseFontFamily(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.Height:
                            {
                                var spec = tag.Spec;
                                spec.Height = TranslateLength(attr);

                            } break;
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
                                tag.Spec.VerticalAlign = UserMapUtil.GetVerticalAlign(propValue);


                            } break;
                        case WebDom.WellknownHtmlName.VSpace:
                            //deprecated
                            //box.MarginTop = box.MarginBottom = TranslateLength(attr);
                            break;
                        case WebDom.WellknownHtmlName.Width:
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
        public static CssLength TranslateLength(HtmlAttribute attr)
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


        static void OnePassBoxCorrection(CssBox root)
        {

        }
        #endregion
    }





}