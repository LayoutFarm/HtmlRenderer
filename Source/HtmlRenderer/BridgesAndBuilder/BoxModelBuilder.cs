//BSD 2014, WinterCore

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
        static BrigeRootElement CreateBridgeRoot(WebDom.HtmlDocument htmldoc)
        {
            BrigeRootElement bridgeRoot = new BrigeRootElement(htmldoc.RootNode);
            RecursiveBuildBridgeContent(htmldoc.RootNode, bridgeRoot);
            return bridgeRoot;
        }
        static void RecursiveBuildBridgeContent(WebDom.HtmlElement parentHtmlNode, BridgeHtmlNode parentBridge)
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

                            BridgeHtmlNode bridgeElement = new BridgeHtmlNode(elemNode,
                            UserMapUtil.EvaluateTagName(elemNode.LocalName));
                            parentBridge.AddChildElement(bridgeElement);
                            //recursive 
                            RecursiveBuildBridgeContent(elemNode, bridgeElement);

                        } break;
                    case WebDom.HtmlNodeType.TextNode:
                        {
                            BridgeHtmlNode bridgeElement = new BridgeHtmlNode((WebDom.HtmlTextNode)node);
                            parentBridge.AddChildElement(bridgeElement);
                        } break;
                }
            }
        }
        static void RecursiveGenerateCssBoxContent(BridgeHtmlNode parentBrigeNode, CssBox parentHtmlNode)
        {
            int childCount = parentBrigeNode.ChildCount;
            switch (childCount)
            {
                case 0:
                    { } break;
                case 1:
                    {
                        BridgeHtmlNode bridgeChild = parentBrigeNode.GetNode(0);
                        if (bridgeChild.IsTextNode)
                        {
                            parentHtmlNode.SetTextContent(bridgeChild.CopyTextBuffer());
                            //parse and evaluate whitespace here ! 
                            
                        }
                        else
                        {
                            CssBox box = BoxCreator.CreateBoxNotInherit(bridgeChild, parentHtmlNode); 
                            RecursiveGenerateCssBoxContent(bridgeChild, box); 
                        }
                    } break;
                default:
                    {
                        for (int i = 0; i < childCount; ++i)
                        {
                            //create and correct box in one pass here !?
                            BridgeHtmlNode bridgeChild = parentBrigeNode.GetNode(i);
                            if (bridgeChild.IsTextNode)
                            {

                                //create anonymous box  but not inherit
                                CssBox anonText = new CssBox(parentHtmlNode, null);
                                //parse and evaluate whitespace here ! 
                                anonText.SetTextContent(bridgeChild.CopyTextBuffer());
                            }
                            else
                            {
                                CssBox box = BoxCreator.CreateBoxNotInherit(bridgeChild, parentHtmlNode);
                                RecursiveGenerateCssBoxContent(bridgeChild, box);
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
            //2. create bridge root
            BrigeRootElement bridgeRoot = CreateBridgeRoot(htmldoc);
            //-----------------------





            //-----------------------
            //box generation
            //3. create cssbox from root
            CssBox root = BoxCreator.CreateRootBlock();
            RecursiveGenerateCssBoxContent(bridgeRoot, root); 

#if DEBUG
            dbugTestParsePerformance(html);
#endif


            //2. decorate cssbox with styles
            if (root != null)
            {

                CssBox.SetHtmlContainer(root, htmlContainer);
                //-------------------------------------------------------------------
                ActiveCssTemplate activeCssTemplate = new ActiveCssTemplate(htmlContainer, cssData);
                ApplyStyleSheet(root, activeCssTemplate);
                //-------------------------------------------------------------------
                SetTextSelectionStyle(htmlContainer, cssData);
                OnePassBoxCorrection(root);
                CorrectTextBoxes(root);
                //CorrectImgBoxes(root);

                bool followingBlock = true;

                CorrectLineBreaksBlocks(root, ref followingBlock);

                //1. must test first
                CorrectInlineBoxesParent(root);
                //2. then ...
                CorrectBlockInsideInline(root);

            }
            return root;
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
        static void ApplyStyleSheet(CssBox box, ActiveCssTemplate activeCssTemplate)
        {
            //recursive 
            //if (box.dbugId == 36)
            //{
            //}
            //-------------------------------------------------------------------            
            box.InheritStyles(box.ParentBox);

            if (box.HtmlElement != null)
            {
                //------------------------------------------------------------------- 
                //1. element tag
                //2. css class 
                // try assign style using the html element tag    
                activeCssTemplate.ApplyActiveTemplateForElement(box.ParentBox, box);
                //3.
                // try assign style using the "id" attribute of the html element
                if (box.HtmlElement.HasAttribute("id"))
                {
                    var id = box.HtmlElement.TryGetAttribute("id");
                    AssignStylesForElementId(box, activeCssTemplate, "#" + id);
                }
                //-------------------------------------------------------------------
                //4. 
                //element attribute
                AssignStylesFromTranslatedAttributesHTML5(box, activeCssTemplate);
                //AssignStylesFromTranslatedAttributes_Old(box, activeCssTemplate);
                //------------------------------------------------------------------- 
                //5.
                //style attribute value of element
                if (box.HtmlElement.HasAttribute("style"))
                {
                    var ruleset = activeCssTemplate.ParseCssBlock(box.HtmlElement.Name, box.HtmlElement.TryGetAttribute("style"));
                    foreach (WebDom.CssPropertyDeclaration propDecl in ruleset.GetAssignmentIter())
                    {
                        AssignPropertyValue(box, box.ParentBox, propDecl);
                    }
                }
                //------------------------------------------------------------------- 
                //some special tags...
                // Check for the <style> tag   
                // Check for the <link rel=stylesheet> tag 
                switch (box.WellknownTagName)
                {
                    case WellknownHtmlTagName.style:
                        {
                            switch (box.ChildCount)
                            {
                                case 0:
                                    {
                                        activeCssTemplate.LoadRawStyleElementContent(box.CopyTextContent());
                                    } break;
                                case 1:
                                    {
                                        activeCssTemplate.LoadRawStyleElementContent(box.GetFirstChild().CopyTextContent());
                                    } break;
                            }

                        } break;
                    case WellknownHtmlTagName.link:
                        {
                            if (box.GetAttribute("rel", string.Empty).Equals("stylesheet", StringComparison.CurrentCultureIgnoreCase))
                            {
                                activeCssTemplate.LoadLinkStyleSheet(box.GetAttribute("href", string.Empty));
                            }
                        } break;
                }
            }
            //--------------------------------------------------------------------
            //2014-06-27
            //if (box.TextDecoration != CssTextDecoration.NotAssign && !box.MayHasSomeTextContent)
            //{
            //    //text decoration : not inherit
            //    //foreach (var childBox in box.GetChildBoxIter())
            //    //{
            //    //    //send to child ?
            //    //    childBox.TextDecoration = box.TextDecoration;
            //    //}
            //    //box.TextDecoration = CssTextDecoration.NotAssign;
            //}
            //===================================================================
            //parent style assignment is complete before step down into child ***
            foreach (var childBox in box.GetChildBoxIter())
            {
                //recursive
                ApplyStyleSheet(childBox, activeCssTemplate);
            }
            //if (box.dbugId == 36)
            //{
            //}
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
        internal static void AssignPropertyValue(CssBoxBase box, CssBoxBase boxParent, WebDom.CssPropertyDeclaration decl)
        {
            if (decl.IsExpand)
            {
                return;
            }

            if (decl.MarkedAsInherit && boxParent != null)
            {
                //use parent property 
                SetPropertyValueFromParent(box, boxParent, decl.WellknownPropertyName);
            }
            else
            {
                if (IsStyleOnElementAllowed(box, decl))
                {
                    SetPropertyValue(box, boxParent, decl);
                }
            }
        }




        static bool IsStyleOnElementAllowed(CssBoxBase box, WebDom.CssPropertyDeclaration cssProperty)
        {
            //if (box.HtmlTag != null &&
            //    cssProperty.WellknownPropertyName == WebDom.WellknownCssPropertyName.Display)
            //{

            if (box.WellknownTagName != WellknownHtmlTagName.NotAssign &&
                cssProperty.WellknownPropertyName == WebDom.WellknownCssPropertyName.Display)
            {
                CssDisplay display = UserMapUtil.GetDisplayType(cssProperty.GetPropertyValue(0));
                switch (box.WellknownTagName)
                {
                    case WellknownHtmlTagName.table:
                        return display == CssDisplay.Table;
                    case WellknownHtmlTagName.tr:
                        return display == CssDisplay.TableRow;
                    case WellknownHtmlTagName.tbody:
                        return display == CssDisplay.TableRowGroup;
                    case WellknownHtmlTagName.thead:
                        return display == CssDisplay.TableHeaderGroup;
                    case WellknownHtmlTagName.tfoot:
                        return display == CssDisplay.TableFooterGroup;
                    case WellknownHtmlTagName.col:
                        return display == CssDisplay.TableColumn;
                    case WellknownHtmlTagName.colgroup:
                        return display == CssDisplay.TableColumnGroup;
                    case WellknownHtmlTagName.td:
                    case WellknownHtmlTagName.th:
                        return display == CssDisplay.TableCell;
                    case WellknownHtmlTagName.caption:
                        return display == CssDisplay.TableCaption;
                }
            }
            return true;
        }


        static void AssignStylesFromTranslatedAttributes_Old(CssBox box, ActiveCssTemplate activeTemplate)
        {
            //some html attr contains css value 
            IHtmlElement tag = box.HtmlElement;

            if (tag.HasAttributes())
            {
                foreach (IHtmlAttribute attr in tag.GetAttributeIter())
                {
                    //attr switch by wellknown property name 
                    switch ((WebDom.WellknownHtmlName)attr.LocalNameIndex)
                    {
                        case WebDom.WellknownHtmlName.Align:
                            {
                                //align attribute -- deprecated in HTML5

                                string value = attr.Value.ToLower();
                                if (value == "left"
                                    || value == "center"
                                    || value == "right"
                                    || value == "justify")
                                {
                                    WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
                                        value, WebDom.CssValueHint.Iden);

                                    box.CssTextAlign = UserMapUtil.GetTextAlign(propValue);
                                }
                                else
                                {
                                    WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
                                     value, WebDom.CssValueHint.Iden);
                                    box.VerticalAlign = UserMapUtil.GetVerticalAlign(propValue);
                                }
                                break;
                            }
                        case WebDom.WellknownHtmlName.Background:
                            box.BackgroundImageBinder = new ImageBinder(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.BackgroundColor:
                            box.BackgroundColor = CssValueParser.GetActualColor(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.Border:
                            {
                                //not support in HTML5 
                                CssLength borderLen = TranslateLength(UserMapUtil.MakeBorderLength(attr.Value.ToLower()));
                                if (!borderLen.HasError)
                                {

                                    if (borderLen.Number > 0)
                                    {
                                        box.BorderLeftStyle =
                                            box.BorderTopStyle =
                                            box.BorderRightStyle =
                                            box.BorderBottomStyle = CssBorderStyle.Solid;
                                    }

                                    box.BorderLeftWidth =
                                    box.BorderTopWidth =
                                    box.BorderRightWidth =
                                    box.BorderBottomWidth = borderLen;

                                    if (tag.WellknownTagName == WellknownHtmlTagName.table && borderLen.Number > 0)
                                    {
                                        //Cascades to the TD's the border spacified in the TABLE tag.
                                        var borderWidth = CssLength.MakePixelLength(1);
                                        ForEachCellInTable(box, cell =>
                                        {
                                            //for all cells
                                            cell.BorderLeftStyle = cell.BorderTopStyle = cell.BorderRightStyle = cell.BorderBottomStyle = CssBorderStyle.Solid; // CssConstants.Solid;
                                            cell.BorderLeftWidth = cell.BorderTopWidth = cell.BorderRightWidth = cell.BorderBottomWidth = borderWidth;
                                        });

                                    }

                                }
                            } break;
                        case WebDom.WellknownHtmlName.BorderColor:

                            box.BorderLeftColor =
                                box.BorderTopColor =
                                box.BorderRightColor =
                                box.BorderBottomColor = CssValueParser.GetActualColor(attr.Value.ToLower());

                            break;
                        case WebDom.WellknownHtmlName.CellSpacing:

                            //html5 not support in HTML5, use CSS instead
                            box.BorderSpacingHorizontal = box.BorderSpacingVertical = TranslateLength(attr);

                            break;
                        case WebDom.WellknownHtmlName.CellPadding:
                            {
                                //html5 not support in HTML5, use CSS instead ***

                                CssLength len01 = UserMapUtil.ParseGenericLength(attr.Value.ToLower());
                                if (len01.HasError && (len01.Number > 0))
                                {
                                    CssLength len02 = CssLength.MakePixelLength(len01.Number);
                                    ForEachCellInTable(box, cell =>
                                    {
#if DEBUG
                                        // cell.dbugBB = dbugTT++;
#endif
                                        cell.PaddingLeft = cell.PaddingTop = cell.PaddingRight = cell.PaddingBottom = len02;
                                    });

                                }
                                else
                                {
                                    ForEachCellInTable(box, cell =>
                                         cell.PaddingLeft = cell.PaddingTop = cell.PaddingRight = cell.PaddingBottom = len01);
                                }

                            } break;
                        case WebDom.WellknownHtmlName.Color:

                            box.Color = CssValueParser.GetActualColor(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.Dir:
                            {
                                WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
                                        attr.Value.ToLower(), WebDom.CssValueHint.Iden);
                                box.CssDirection = UserMapUtil.GetCssDirection(propValue);
                            }
                            break;
                        case WebDom.WellknownHtmlName.Face:
                            box.FontFamily = CssParser.ParseFontFamily(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.Height:
                            box.Height = TranslateLength(attr);
                            break;
                        case WebDom.WellknownHtmlName.HSpace:
                            box.MarginRight = box.MarginLeft = TranslateLength(attr);
                            break;
                        case WebDom.WellknownHtmlName.Nowrap:
                            box.WhiteSpace = CssWhiteSpace.NoWrap;
                            break;
                        case WebDom.WellknownHtmlName.Size:
                            {
                                switch (tag.WellknownTagName)
                                {
                                    case WellknownHtmlTagName.hr:
                                        {
                                            box.Height = TranslateLength(attr);
                                        } break;
                                    case WellknownHtmlTagName.font:
                                        {
                                            //font tag is not support in Html5
                                            var ruleset = activeTemplate.ParseCssBlock("", attr.Value.ToLower());
                                            foreach (WebDom.CssPropertyDeclaration propDecl in ruleset.GetAssignmentIter())
                                            {
                                                //assign each property
                                                AssignPropertyValue(box, box.ParentBox, propDecl);
                                            }

                                        } break;
                                }
                            } break;
                        case WebDom.WellknownHtmlName.VAlign:
                            {
                                WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
                                          attr.Value.ToLower(), WebDom.CssValueHint.Iden);
                                box.VerticalAlign = UserMapUtil.GetVerticalAlign(propValue);
                            } break;
                        case WebDom.WellknownHtmlName.VSpace:
                            box.MarginTop = box.MarginBottom = TranslateLength(attr);
                            break;
                        case WebDom.WellknownHtmlName.Width:
                            box.Width = TranslateLength(attr);
                            break;
                    }
                }
            }
        }
        static void AssignStylesFromTranslatedAttributesHTML5(CssBox box, ActiveCssTemplate activeTemplate)
        {
            //some html attr contains css value 
            IHtmlElement tag = box.HtmlElement;

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
                                box.CssDirection = UserMapUtil.GetCssDirection(propValue);
                            }
                            break;
                        case WebDom.WellknownHtmlName.Face:
                            //deprecate
                            //box.FontFamily = CssParser.ParseFontFamily(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.Height:
                            box.Height = TranslateLength(attr);
                            break;
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
                                box.VerticalAlign = UserMapUtil.GetVerticalAlign(propValue);


                            } break;
                        case WebDom.WellknownHtmlName.VSpace:
                            //deprecated
                            //box.MarginTop = box.MarginBottom = TranslateLength(attr);
                            break;
                        case WebDom.WellknownHtmlName.Width:

                            box.Width = TranslateLength(attr);
                            break;
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
        static void ForEachCellInTable(CssBox table, Action<CssBox> cellAction)
        {
            foreach (var c1 in table.GetChildBoxIter())
            {
                foreach (var c2 in c1.GetChildBoxIter())
                {
                    if (c2.WellknownTagName == WellknownHtmlTagName.td)
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

        internal static void SetPropertyValue(CssBoxBase cssBox, CssBoxBase parentBox, WebDom.CssPropertyDeclaration decl)
        {
            //assign property  
            WebDom.CssCodeValueExpression cssValue = decl.GetPropertyValue(0);

            switch (decl.WellknownPropertyName)
            {
                case WebDom.WellknownCssPropertyName.BorderBottomWidth:
                    cssBox.BorderBottomWidth = cssValue.AsBorderLength();
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftWidth:
                    cssBox.BorderLeftWidth = cssValue.AsBorderLength();
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightWidth:
                    cssBox.BorderRightWidth = cssValue.AsBorderLength();
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopWidth:
                    cssBox.BorderTopWidth = cssValue.AsBorderLength();
                    break;

                case WebDom.WellknownCssPropertyName.BorderBottomStyle:
                    cssBox.BorderBottomStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftStyle:
                    cssBox.BorderLeftStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightStyle:
                    cssBox.BorderRightStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopStyle:
                    cssBox.BorderTopStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderBottomColor:
                    cssBox.BorderBottomColor = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftColor:
                    cssBox.BorderLeftColor = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightColor:
                    cssBox.BorderRightColor = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopColor:
                    cssBox.BorderTopColor = cssValue.AsColor();
                    break;

                case WebDom.WellknownCssPropertyName.BorderSpacing:

                    cssBox.SetBorderSpacing(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderCollapse:
                    cssBox.BorderCollapse = UserMapUtil.GetBorderCollapse(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.CornerRadius:
                    cssBox.SetCornerRadius(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.CornerNWRadius:
                    cssBox.CornerNWRadius = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.CornerNERadius:
                    cssBox.CornerNERadius = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.CornerSERadius:
                    cssBox.CornerSERadius = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.CornerSWRadius:
                    cssBox.CornerSWRadius = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.MarginBottom:
                    cssBox.MarginBottom = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.MarginLeft:
                    cssBox.MarginLeft = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.MarginRight:
                    cssBox.MarginRight = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.MarginTop:
                    cssBox.MarginTop = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.PaddingBottom:
                    cssBox.PaddingBottom = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.PaddingLeft:
                    cssBox.PaddingLeft = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.PaddingRight:
                    cssBox.PaddingRight = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.PaddingTop:
                    cssBox.PaddingTop = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.Left:
                    cssBox.Left = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.Top:
                    cssBox.Top = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.Width:
                    cssBox.Width = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.MaxWidth:
                    cssBox.MaxWidth = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.Height:
                    cssBox.Height = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundColor:
                    cssBox.BackgroundColor = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundImage:
                    cssBox.BackgroundImageBinder = new ImageBinder(cssValue.GetTranslatedStringValue());
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundPosition:

                    cssBox.SetBackgroundPosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundRepeat:
                    cssBox.BackgroundRepeat = UserMapUtil.GetBackgroundRepeat(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradient:
                    cssBox.BackgroundGradient = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradientAngle:
                    {
                        cssBox.BackgroundGradientAngle = cssValue.AsNumber();

                        //float angle;
                        //if (float.TryParse(cssValue.GetTranslatedStringValue(), out angle))
                        //{
                        //    cssBox.BackgroundGradientAngle = angle;
                        //}
                    } break;
                case WebDom.WellknownCssPropertyName.Color:
                    cssBox.Color = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.Display:
                    cssBox.CssDisplay = UserMapUtil.GetDisplayType(cssValue);

                    break;
                case WebDom.WellknownCssPropertyName.Direction:

                    cssBox.CssDirection = UserMapUtil.GetCssDirection(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.EmptyCells:
                    cssBox.EmptyCells = UserMapUtil.GetEmptyCell(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Float:
                    cssBox.Float = UserMapUtil.GetFloat(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Position:
                    cssBox.Position = UserMapUtil.GetCssPosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.LineHeight:

                    cssBox.SetLineHeight(cssValue.AsLength());
                    break;
                case WebDom.WellknownCssPropertyName.VerticalAlign:
                    cssBox.VerticalAlign = UserMapUtil.GetVerticalAlign(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.TextIndent:
                    cssBox.TextIndent = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.TextAlign:

                    cssBox.CssTextAlign = UserMapUtil.GetTextAlign(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.TextDecoration:
                    cssBox.TextDecoration = UserMapUtil.GetTextDecoration(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Whitespace:
                    cssBox.WhiteSpace = UserMapUtil.GetWhitespace(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.WordBreak:
                    cssBox.WordBreak = UserMapUtil.GetWordBreak(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Visibility:
                    cssBox.CssVisibility = UserMapUtil.GetVisibility(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.WordSpacing:
                    cssBox.WordSpacing = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.FontFamily:
                    cssBox.FontFamily = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.FontSize:
                    cssBox.SetFontSize(parentBox, cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.FontStyle:
                    cssBox.FontStyle = UserMapUtil.GetFontStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.FontVariant:
                    cssBox.FontVariant = UserMapUtil.GetFontVariant(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.FontWeight:
                    cssBox.FontWeight = UserMapUtil.GetFontWeight(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.ListStyle:
                    cssBox.ListStyle = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.ListStylePosition:
                    cssBox.ListStylePosition = UserMapUtil.GetListStylePosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleImage:
                    cssBox.ListStyleImage = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleType:
                    cssBox.ListStyleType = UserMapUtil.GetListStyleType(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Overflow:
                    cssBox.Overflow = UserMapUtil.GetOverflow(cssValue);
                    break;
            }
        }


        /// <summary>
        /// assign property value from parent
        /// </summary>
        /// <param name="cssBox"></param>
        /// <param name="propName"></param>
        static void SetPropertyValueFromParent(CssBoxBase cssBox, CssBoxBase parentCssBox, HtmlRenderer.WebDom.WellknownCssPropertyName propName)
        {

            switch (propName)
            {
                case WebDom.WellknownCssPropertyName.BorderBottomWidth:
                    cssBox.BorderBottomWidth = parentCssBox.BorderBottomWidth;
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftWidth:
                    cssBox.BorderLeftWidth = parentCssBox.BorderLeftWidth;
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightWidth:
                    cssBox.BorderRightWidth = parentCssBox.BorderRightWidth;
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopWidth:
                    cssBox.BorderTopWidth = parentCssBox.BorderTopWidth;
                    break;
                case WebDom.WellknownCssPropertyName.BorderBottomStyle:
                    cssBox.BorderBottomStyle = parentCssBox.BorderBottomStyle;
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftStyle:
                    cssBox.BorderLeftStyle = parentCssBox.BorderBottomStyle;
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightStyle:
                    cssBox.BorderRightStyle = parentCssBox.BorderRightStyle;
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopStyle:
                    cssBox.BorderTopStyle = parentCssBox.BorderTopStyle;
                    break;
                case WebDom.WellknownCssPropertyName.BorderBottomColor:
                    cssBox.BorderBottomColor = parentCssBox.BorderBottomColor;
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftColor:
                    cssBox.BorderLeftColor = parentCssBox.BorderLeftColor;
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightColor:
                    cssBox.BorderRightColor = parentCssBox.BorderRightColor;
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopColor:
                    cssBox.BorderTopColor = parentCssBox.BorderTopColor;
                    break;
                case WebDom.WellknownCssPropertyName.BorderSpacing:

                    cssBox.BorderSpacingHorizontal = parentCssBox.BorderSpacingHorizontal;
                    cssBox.BorderSpacingVertical = parentCssBox.BorderSpacingVertical;
                    break;
                case WebDom.WellknownCssPropertyName.BorderCollapse:
                    cssBox.BorderCollapse = parentCssBox.BorderCollapse;
                    break;
                case WebDom.WellknownCssPropertyName.CornerRadius:
                    cssBox.CornerNERadius = parentCssBox.CornerNERadius;
                    cssBox.CornerNWRadius = parentCssBox.CornerNWRadius;
                    cssBox.CornerSERadius = parentCssBox.CornerSERadius;
                    cssBox.CornerSWRadius = parentCssBox.CornerSWRadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerNWRadius:

                    cssBox.CornerNWRadius = parentCssBox.CornerNWRadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerNERadius:
                    cssBox.CornerNERadius = parentCssBox.CornerNERadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerSERadius:
                    cssBox.CornerSERadius = parentCssBox.CornerSERadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerSWRadius:
                    cssBox.CornerSWRadius = parentCssBox.CornerSWRadius;
                    break;
                case WebDom.WellknownCssPropertyName.MarginBottom:
                    cssBox.MarginBottom = parentCssBox.MarginBottom;
                    break;
                case WebDom.WellknownCssPropertyName.MarginLeft:
                    cssBox.MarginLeft = parentCssBox.MarginLeft;
                    break;
                case WebDom.WellknownCssPropertyName.MarginRight:
                    cssBox.MarginRight = parentCssBox.MarginRight;
                    break;
                case WebDom.WellknownCssPropertyName.MarginTop:
                    cssBox.MarginTop = parentCssBox.MarginTop;
                    break;
                case WebDom.WellknownCssPropertyName.PaddingBottom:
                    cssBox.PaddingBottom = parentCssBox.MarginBottom;
                    break;
                case WebDom.WellknownCssPropertyName.PaddingLeft:
                    cssBox.PaddingLeft = parentCssBox.PaddingLeft;
                    break;
                case WebDom.WellknownCssPropertyName.PaddingRight:
                    cssBox.PaddingRight = parentCssBox.PaddingRight;
                    break;
                case WebDom.WellknownCssPropertyName.PaddingTop:
                    cssBox.PaddingTop = parentCssBox.PaddingTop;
                    break;
                case WebDom.WellknownCssPropertyName.Left:
                    cssBox.Left = parentCssBox.Left;
                    break;
                case WebDom.WellknownCssPropertyName.Top:
                    cssBox.Top = parentCssBox.Top;
                    break;
                case WebDom.WellknownCssPropertyName.Width:
                    cssBox.Width = parentCssBox.Width;
                    break;
                case WebDom.WellknownCssPropertyName.MaxWidth:
                    cssBox.MaxWidth = parentCssBox.MaxWidth;
                    break;
                case WebDom.WellknownCssPropertyName.Height:
                    cssBox.Height = parentCssBox.Height;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundColor:
                    cssBox.BackgroundColor = parentCssBox.BackgroundColor;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundImage:
                    cssBox.BackgroundImageBinder = parentCssBox.BackgroundImageBinder;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundPosition:
                    cssBox.BackgroundPositionX = parentCssBox.BackgroundPositionX;
                    cssBox.BackgroundPositionY = parentCssBox.BackgroundPositionY;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundRepeat:
                    cssBox.BackgroundRepeat = parentCssBox.BackgroundRepeat;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradient:
                    cssBox.BackgroundGradient = parentCssBox.BackgroundGradient;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradientAngle:
                    {
                        cssBox.BackgroundGradientAngle = parentCssBox.BackgroundGradientAngle;
                        //float angle;
                        //if (float.TryParse(value, out angle))
                        //{
                        //    cssBox.BackgroundGradientAngle = angle;
                        //}
                    } break;
                case WebDom.WellknownCssPropertyName.Color:
                    cssBox.Color = parentCssBox.Color;
                    break;
                case WebDom.WellknownCssPropertyName.Display:
                    cssBox.CssDisplay = parentCssBox.CssDisplay;
                    break;
                case WebDom.WellknownCssPropertyName.Direction:
                    cssBox.CssDirection = parentCssBox.CssDirection;
                    break;
                case WebDom.WellknownCssPropertyName.EmptyCells:
                    cssBox.EmptyCells = parentCssBox.EmptyCells;
                    break;
                case WebDom.WellknownCssPropertyName.Float:
                    cssBox.Float = parentCssBox.Float;
                    break;
                case WebDom.WellknownCssPropertyName.Position:
                    cssBox.Position = parentCssBox.Position;
                    break;
                case WebDom.WellknownCssPropertyName.LineHeight:
                    cssBox.LineHeight = parentCssBox.LineHeight;
                    break;
                case WebDom.WellknownCssPropertyName.VerticalAlign:
                    cssBox.VerticalAlign = parentCssBox.VerticalAlign;
                    break;
                case WebDom.WellknownCssPropertyName.TextIndent:
                    cssBox.TextIndent = parentCssBox.TextIndent;
                    break;
                case WebDom.WellknownCssPropertyName.TextAlign:
                    cssBox.CssTextAlign = parentCssBox.CssTextAlign;
                    break;
                case WebDom.WellknownCssPropertyName.TextDecoration:
                    cssBox.TextDecoration = parentCssBox.TextDecoration;
                    break;
                case WebDom.WellknownCssPropertyName.Whitespace:
                    cssBox.WhiteSpace = parentCssBox.WhiteSpace;
                    break;
                case WebDom.WellknownCssPropertyName.WordBreak:
                    cssBox.WordBreak = parentCssBox.WordBreak;
                    break;
                case WebDom.WellknownCssPropertyName.Visibility:
                    cssBox.CssVisibility = parentCssBox.CssVisibility;
                    break;
                case WebDom.WellknownCssPropertyName.WordSpacing:
                    cssBox.WordSpacing = parentCssBox.WordSpacing;
                    break;
                case WebDom.WellknownCssPropertyName.FontFamily:
                    cssBox.FontFamily = parentCssBox.FontFamily;
                    //cssBox.FontFamily = value;
                    break;
                case WebDom.WellknownCssPropertyName.FontSize:
                    cssBox.FontSize = parentCssBox.FontSize;
                    break;
                case WebDom.WellknownCssPropertyName.FontStyle:
                    cssBox.FontStyle = parentCssBox.FontStyle;
                    break;
                case WebDom.WellknownCssPropertyName.FontVariant:
                    cssBox.FontVariant = parentCssBox.FontVariant;

                    break;
                case WebDom.WellknownCssPropertyName.FontWeight:
                    cssBox.FontWeight = parentCssBox.FontWeight;

                    break;
                case WebDom.WellknownCssPropertyName.ListStyle:

                    cssBox.ListStyle = parentCssBox.ListStyle;
                    break;
                case WebDom.WellknownCssPropertyName.ListStylePosition:
                    cssBox.ListStylePosition = parentCssBox.ListStylePosition;
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleImage:

                    cssBox.ListStyleImage = parentCssBox.ListStyleImage;
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleType:

                    cssBox.ListStyleType = parentCssBox.ListStyleType;
                    break;
                case WebDom.WellknownCssPropertyName.Overflow:

                    cssBox.Overflow = parentCssBox.Overflow;
                    break;
            }
        }


    }
}