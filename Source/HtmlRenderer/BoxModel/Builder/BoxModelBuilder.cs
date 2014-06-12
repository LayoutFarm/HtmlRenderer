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

namespace HtmlRenderer.Dom
{
    /// <summary>
    /// Handle css DOM tree generation from raw html and stylesheet.
    /// </summary>
    static partial class BoxModelBuilder
    {
        //======================================
        #region Parse
        /// <summary>
        /// Parses the source html to css boxes tree structure.
        /// </summary>
        /// <param name="source">the html source to parse</param>
        static CssBox ParseDocument(TextSnapshot snapSource)
        {
            var parser = new HtmlRenderer.WebDom.Parser.HtmlParser();
            //------------------------
            parser.Parse(snapSource);

            WebDom.HtmlDocument resultHtmlDoc = parser.ResultHtmlDoc;
            var rootCssBox = CssBox.CreateRootBlock();
            var curBox = rootCssBox;
            //walk on tree and create cssbox
            foreach (WebDom.HtmlNode node in resultHtmlDoc.RootNode.GetChildNodeIterForward())
            {
                WebDom.HtmlElement elemNode = node as WebDom.HtmlElement;
                if (elemNode != null)
                {
                    CreateCssBox(elemNode, curBox);
                }
            }
            return rootCssBox;
        }
        static void CreateCssBox(WebDom.HtmlElement htmlElement, CssBox parentNode)
        {
            //recursive  
            CssBox box = CssBox.CreateBox(new HtmlTagBridge(htmlElement), parentNode);
            foreach (WebDom.HtmlNode node in htmlElement.GetChildNodeIterForward())
            {
                switch (node.NodeType)
                {
                    case WebDom.HtmlNodeType.OpenElement:
                    case WebDom.HtmlNodeType.ShortElement:
                        {
                            //recursive
                            CreateCssBox((WebDom.HtmlElement)node, box);

                        } break;
                    case WebDom.HtmlNodeType.TextNode:
                        {

                            /// Add html text anon box to the current box, this box will have the rendered text<br/>
                            /// Adding box also for text that contains only whitespaces because we don't know yet if
                            /// the box is preformatted. At later stage they will be removed if not relevant.                             
                            WebDom.HtmlTextNode textNode = (WebDom.HtmlTextNode)node;
                            //create anonymos box
                            CssBox.CreateBox(box).SetTextContent(textNode.CopyTextBuffer());

                        } break;
                    default:
                        {
                        } break;
                }
            }

        }
        #endregion Parse
        //======================================






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

            //1. generate css box  from html data
            CssBox root = ParseDocument(new TextSnapshot(html.ToCharArray()));

#if DEBUG
            dbugTestParsePerformance(html);
#endif


            //2. decorate cssbox with styles
            if (root != null)
            {

                root.HtmlContainer = htmlContainer;
                //-------------------------------------------------------------------
                ActiveCssTemplate activeCssTemplate = new ActiveCssTemplate(htmlContainer, cssData);
                ApplyStyleSheet(root, activeCssTemplate);
                //-------------------------------------------------------------------
                SetTextSelectionStyle(htmlContainer, cssData);

                CorrectTextBoxes(root);

                CorrectImgBoxes(root);

                bool followingBlock = true;

                CorrectLineBreaksBlocks(root, ref followingBlock);

                //1. must test first
                CorrectInlineBoxesParent(root);
                //2. then ...
                CorrectBlockInsideInline(root);
                //3. another?
                CorrectInlineBoxesParent(root);
            }
            return root;
        }

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
            for (int i = nround; i >= 0; --i)
            {
                CssBox root2 = ParseDocument(snapSource);
            }
            sw1.Stop();
            long ee2 = sw1.ElapsedTicks;
            long ee2_ms = sw1.ElapsedMilliseconds;

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
            //-------------------------------------------------------------------            
            box.InheritStyles(box.ParentBox);

            if (box.HtmlTag != null)
            {
                //------------------------------------------------------------------- 
                //1. element tag
                //2. css class 
                // try assign style using the html element tag    
                activeCssTemplate.ApplyActiveTemplateForElement(box.ParentBox, box);
                //3.
                // try assign style using the "id" attribute of the html element
                if (box.HtmlTag.HasAttribute("id"))
                {
                    var id = box.HtmlTag.TryGetAttribute("id");
                    AssignStylesForElementId(box, activeCssTemplate, "#" + id);
                }
                //-------------------------------------------------------------------
                //4. 
                //element attribute
                AssignStylesFromTranslatedAttributes(box, activeCssTemplate);
                //------------------------------------------------------------------- 
                //5.
                //style attribute value of element
                if (box.HtmlTag.HasAttribute("style"))
                {
                    var ruleset = activeCssTemplate.ParseCssBlock(box.HtmlTag.Name, box.HtmlTag.TryGetAttribute("style"));
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
                    case WellknownHtmlTagName.STYLE:
                        {
                            if (box.ChildCount == 1)
                            {
                                activeCssTemplate.LoadRawStyleElementContent(box.GetFirstChild().CopyTextContent());
                            }
                        } break;
                    case WellknownHtmlTagName.LINK:
                        {
                            if (box.GetAttribute("rel", string.Empty).Equals("stylesheet", StringComparison.CurrentCultureIgnoreCase))
                            {
                                activeCssTemplate.LoadLinkStyleSheet(box.GetAttribute("href", string.Empty));
                            }
                        } break;
                }
            }
            //--------------------------------------------------------------------
            if (box.TextDecoration != CssTextDecoration.NotAssign && !box.MayHasSomeTextContent)
            {
                foreach (var childBox in box.GetChildBoxIter())
                {
                    childBox.TextDecoration = box.TextDecoration;
                }
                box.TextDecoration = CssTextDecoration.NotAssign;
            }

            //===================================================================
            //parent style assignment is complete before step down into child ***

            foreach (var childBox in box.GetChildBoxIter())
            {
                //recursive
                ApplyStyleSheet(childBox, activeCssTemplate);
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
                CssDisplay display = CssBoxUserUtilExtension.GetDisplayType(cssProperty.GetPropertyValue(0));
                switch (box.WellknownTagName)
                {
                    case WellknownHtmlTagName.TABLE:
                        return display == CssDisplay.Table;
                    case WellknownHtmlTagName.TR:
                        return display == CssDisplay.TableRow;
                    case WellknownHtmlTagName.TBody:
                        return display == CssDisplay.TableRowGroup;
                    case WellknownHtmlTagName.THead:
                        return display == CssDisplay.TableHeaderGroup;
                    case WellknownHtmlTagName.TFoot:
                        return display == CssDisplay.TableFooterGroup;
                    case WellknownHtmlTagName.COL:
                        return display == CssDisplay.TableColumn;
                    case WellknownHtmlTagName.COLGROUP:
                        return display == CssDisplay.TableColumnGroup;
                    case WellknownHtmlTagName.TD:
                    case WellknownHtmlTagName.TH:
                        return display == CssDisplay.TableCell;
                    case WellknownHtmlTagName.CAPTION:
                        return display == CssDisplay.TableCaption;
                }
            }
            return true;
        }


        static void AssignStylesFromTranslatedAttributes(CssBox box, ActiveCssTemplate activeTemplate)
        {
            //some html attr contains css value 
            IHtmlElement tag = box.HtmlTag;

            if (tag.HasAttributes())
            {
                foreach (IHtmlAttribute attr in tag.GetAttributeIter())
                {
                    //attr switch by wellknown property name 
                    switch ((WebDom.WellknownHtmlName)attr.LocalNameIndex)
                    {
                        case WebDom.WellknownHtmlName.Align:
                            {

                                string value = attr.Value.ToLower();
                                if (value == "left"
                                    || value == "center"
                                    || value == "right"
                                    || value == "justify")
                                {
                                    WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
                                        value, WebDom.CssValueHint.Iden);

                                    box.CssTextAlign = CssBoxUserUtilExtension.GetTextAlign(propValue);
                                }
                                else
                                {
                                    WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
                                     value, WebDom.CssValueHint.Iden);
                                    box.VerticalAlign = CssBoxUserUtilExtension.GetVerticalAlign(propValue);
                                }
                                break;
                            }
                        case WebDom.WellknownHtmlName.Background:
                            box.BackgroundImage = attr.Value.ToLower();
                            break;
                        case WebDom.WellknownHtmlName.BackgroundColor:
                            box.BackgroundColor = CssValueParser.GetActualColor(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.Border:
                            {

                                CssLength borderLen = TranslateLength(CssLength.MakeBorderLength(attr.Value.ToLower()));
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

                                    if (tag.WellknownTagName == WellknownHtmlTagName.TABLE && borderLen.Number > 0)
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
                            box.BorderSpacingHorizontal = box.BorderSpacingVertical = TranslateLength(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.CellPadding:
                            {
                                // Cascades to the TD's the border spacified in the TABLE tag.
                                CssLength length = TranslateLength(attr.Value.ToLower());
                                ForEachCellInTable(box, cell =>
                                     cell.PaddingLeft = cell.PaddingTop = cell.PaddingRight = cell.PaddingBottom = length);

                            } break;
                        case WebDom.WellknownHtmlName.Color:

                            box.Color = CssValueParser.GetActualColor(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.Dir:
                            {
                                WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
                                        attr.Value.ToLower(), WebDom.CssValueHint.Iden);
                                box.CssDirection = CssBoxUserUtilExtension.GetCssDirection(propValue);
                            }
                            break;
                        case WebDom.WellknownHtmlName.Face:
                            box.FontFamily = CssParser.ParseFontFamily(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.Height:
                            box.Height = TranslateLength(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.HSpace:
                            box.MarginRight = box.MarginLeft = TranslateLength(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.Nowrap:
                            box.WhiteSpace = CssWhiteSpace.NoWrap;
                            break;
                        case WebDom.WellknownHtmlName.Size:
                            {
                                switch (tag.WellknownTagName)
                                {
                                    case WellknownHtmlTagName.HR:
                                        {
                                            box.Height = TranslateLength(attr.Value.ToLower());
                                        } break;
                                    case WellknownHtmlTagName.FONT:
                                        {
                                            var ruleset = activeTemplate.ParseCssBlock("", attr.Value.ToLower());
                                            foreach (WebDom.CssPropertyDeclaration propDecl in ruleset.GetAssignmentIter())
                                            {
                                                //assign each property
                                                AssignPropertyValue(box, box.ParentBox, propDecl);
                                            }
                                            //WebDom.CssCodePrimitiveExpression prim = new WebDom.CssCodePrimitiveExpression(value, 
                                            //box.SetFontSize(value);
                                        } break;
                                }
                            } break;
                        case WebDom.WellknownHtmlName.VAlign:
                            {
                                WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
                                          attr.Value.ToLower(), WebDom.CssValueHint.Iden);
                                box.VerticalAlign = CssBoxUserUtilExtension.GetVerticalAlign(propValue);
                            } break;
                        case WebDom.WellknownHtmlName.VSpace:
                            box.MarginTop = box.MarginBottom = TranslateLength(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.Width:
                            box.Width = TranslateLength(attr.Value.ToLower());
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Converts an HTML length into a Css length
        /// </summary>
        /// <param name="htmlLength"></param>
        /// <returns></returns>
        public static CssLength TranslateLength(string htmlLength)
        {
            CssLength len = new CssLength(htmlLength);
            if (len.HasError)
            {
                return CssLength.MakePixelLength(0);
            }
            return len;
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
        static void ForEachCellInTable(CssBox table, ActionInt<CssBox> cellAction)
        {
            foreach (var c1 in table.GetChildBoxIter())
            {
                foreach (var c2 in c1.GetChildBoxIter())
                {
                    if (c2.WellknownTagName == WellknownHtmlTagName.TD)
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

            mixFlags = 0;

            var children = CssBox.UnsafeGetChildren(box);
            for (int i = children.Count - 1; i >= 0; --i)
            {
                if ((mixFlags |= children[i].IsInline ? HAS_IN_LINE : HAS_BLOCK) == (HAS_BLOCK | HAS_IN_LINE))
                {
                    return true;
                }
            }

            return false;
            //return checkFlags == (HAS_BLOCK | HAS_IN_LINE);
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
                    cssBox.BorderBottomStyle = CssBoxUserUtilExtension.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftStyle:
                    cssBox.BorderLeftStyle = CssBoxUserUtilExtension.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightStyle:
                    cssBox.BorderRightStyle = CssBoxUserUtilExtension.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopStyle:
                    cssBox.BorderTopStyle = CssBoxUserUtilExtension.GetBorderStyle(cssValue);
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
                    cssBox.BorderCollapse = CssBoxUserUtilExtension.GetBorderCollapse(cssValue);
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
                    cssBox.BackgroundImage = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundPosition:
                    cssBox.BackgroundPosition = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundRepeat:
                    cssBox.BackgroundRepeat = cssValue.GetTranslatedStringValue();
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
                    cssBox.CssDisplay = CssBoxUserUtilExtension.GetDisplayType(cssValue);

                    break;
                case WebDom.WellknownCssPropertyName.Direction:

                    cssBox.CssDirection = CssBoxUserUtilExtension.GetCssDirection(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.EmptyCells:
                    cssBox.EmptyCells = CssBoxUserUtilExtension.GetEmptyCell(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Float:
                    cssBox.Float = CssBoxUserUtilExtension.GetFloat(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Position:
                    cssBox.Position = CssBoxUserUtilExtension.GetCssPosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.LineHeight:

                    cssBox.SetLineHeight(cssValue.AsLength());
                    break;
                case WebDom.WellknownCssPropertyName.VerticalAlign:
                    cssBox.VerticalAlign = CssBoxUserUtilExtension.GetVerticalAlign(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.TextIndent:
                    cssBox.TextIndent = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.TextAlign:

                    cssBox.CssTextAlign = CssBoxUserUtilExtension.GetTextAlign(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.TextDecoration:
                    cssBox.TextDecoration = CssBoxUserUtilExtension.GetTextDecoration(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Whitespace:
                    cssBox.WhiteSpace = CssBoxUserUtilExtension.GetWhitespace(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.WordBreak:
                    cssBox.WordBreak = CssBoxUserUtilExtension.GetWordBreak(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Visibility:
                    cssBox.CssVisibility = CssBoxUserUtilExtension.GetVisibility(cssValue);
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
                    cssBox.FontStyle = CssBoxUserUtilExtension.GetFontStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.FontVariant:
                    cssBox.FontVariant = CssBoxUserUtilExtension.GetFontVariant(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.FontWeight:
                    cssBox.FontWeight = CssBoxUserUtilExtension.GetFontWeight(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.ListStyle:
                    cssBox.ListStyle = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.ListStylePosition:
                    cssBox.ListStylePosition = CssBoxUserUtilExtension.GetListStylePosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleImage:
                    cssBox.ListStyleImage = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleType:
                    cssBox.ListStyleType = CssBoxUserUtilExtension.GetListStyleType(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Overflow:
                    cssBox.Overflow = CssBoxUserUtilExtension.GetOverflow(cssValue);
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
                    cssBox.BackgroundImage = parentCssBox.BackgroundImage;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundPosition:
                    cssBox.BackgroundPosition = parentCssBox.BackgroundPosition;
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