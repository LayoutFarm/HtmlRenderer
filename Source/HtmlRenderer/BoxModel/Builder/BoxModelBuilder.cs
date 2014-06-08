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

using HtmlRenderer.Entities;
using HtmlRenderer.Handlers;
using HtmlRenderer.Utils;
using HtmlRenderer.Parse;

namespace HtmlRenderer.Dom
{
    /// <summary>
    /// Handle css DOM tree generation from raw html and stylesheet.
    /// </summary>
    internal static class BoxModelBuilder
    {
        /// <summary>
        /// Generate css tree by parsing the given html and applying the given css style data on it.
        /// </summary>
        /// <param name="html">the html to parse</param>
        /// <param name="htmlContainer">the html container to use for reference resolve</param>
        /// <param name="cssData">the css data to use</param>
        /// <returns>the root of the generated tree</returns>
        public static CssBox BuildBoxesTree(
            string html,
            HtmlContainer htmlContainer,
            CssActiveSheet cssData)
        {

            //1. generate css box  from html data
            CssBox root = HtmlParser2.ParseDocument(new TextSnapshot(html.ToCharArray()));

#if DEBUG
            dbugTestParsePerformance(html);
#endif


            //2. decorate cssbox with styles
            if (root != null)
            {

                root.HtmlContainer = htmlContainer;
                ApplyStyleSheet(root, htmlContainer, ref cssData);
                //-------------------------------------------------------------------
                SetTextSelectionStyle(htmlContainer, cssData);

                CssBox.CorrectTextBoxes(root);

                CorrectImgBoxes(root);

                bool followingBlock = true;
                CssBox.CorrectLineBreaksBlocks(root, ref followingBlock);

                //1. must test first
                CssBox.CorrectInlineBoxesParent(root);
                //2. then ...
                CorrectBlockInsideInline(root);
                //3. another?
                CssBox.CorrectInlineBoxesParent(root);
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
                CssBox root2 = HtmlParser2.ParseDocument(snapSource);
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
        private static void ApplyStyleSheet(CssBox box, HtmlContainer htmlContainer, ref CssActiveSheet cssData)
        {
            //recursive 
            //------------------------------------------------------------------- 
            CssActiveSheet savedCss = cssData;
            //------------------------------------------------------------------- 

            box.InheritStyle();

            if (box.HtmlTag != null)
            {
                //------------------------------------------------------------------- 
                //1.
                // try assign style using the html element tag     
                AssignCssToSpecificBoxWithTagName(box, cssData);
                //2.
                // try assign style using the "class" attribute of the html element 
                if (box.HtmlTag.HasAttribute("class"))
                {
                    AssignCssToSpecificClass(box, cssData);
                }

                //3.
                // try assign style using the "id" attribute of the html element
                if (box.HtmlTag.HasAttribute("id"))
                {
                    var id = box.HtmlTag.TryGetAttribute("id");
                    AssignCssToSpecificBoxWithId(box, cssData, "#" + id);
                }
                //-------------------------------------------------------------------
                //4. 
                TranslateAttributes(box.HtmlTag, box);
                //-------------------------------------------------------------------

                // Check for the style attribute
                if (box.HtmlTag.HasAttribute("style"))
                {
                    WebDom.CssRuleSet ruleset = CssParser.ParseCssBlock2(box.HtmlTag.Name, box.HtmlTag.TryGetAttribute("style"));
                    foreach (WebDom.CssPropertyDeclaration propDecl in ruleset.GetAssignmentIter())
                    {
                        AssignPropertyValueToCssBox(box, AssignPropertySource.StyleAttribute, propDecl);
                    }
                }
                //-------------------------------------------------------------------

                // Check for the <style> tag                 
                if (box.WellknownTagName == WellknownHtmlTagName.STYLE && box.ChildCount == 1)
                {
                    cssData = CloneCssData(cssData);
                    CssParser.ParseStyleSheet(cssData, box.GetFirstChild().CopyTextContent());
                }
                //-------------------------------------------------------------------

                // Check for the <link rel=stylesheet> tag
                //                if (box.HtmlTag.Name.Equals("link", StringComparison.CurrentCultureIgnoreCase) &&
                if (box.WellknownTagName == WellknownHtmlTagName.LINK &&
                    box.GetAttribute("rel", string.Empty).Equals("stylesheet", StringComparison.CurrentCultureIgnoreCase))
                {
                    //----------------------
                    cssData = CloneCssData(cssData);
                    string stylesheet;
                    CssActiveSheet stylesheetData;

                    //load style sheet from external 
                    StylesheetLoadHandler.LoadStylesheet(htmlContainer,
                        box.GetAttribute("href", string.Empty),
                        out stylesheet, out stylesheetData);

                    if (stylesheet != null)
                    {
                        CssParser.ParseStyleSheet(cssData, stylesheet);
                    }
                    else if (stylesheetData != null)
                    {
                        cssData.Combine(stylesheetData);
                    }
                }
            }

            if (box.TextDecoration != CssTextDecoration.NotAssign && !box.MayHasSomeTextContent)
            {
                foreach (var childBox in box.GetChildBoxIter())
                {
                    childBox.TextDecoration = box.TextDecoration;
                }
                box.TextDecoration = CssTextDecoration.NotAssign;
            }

            foreach (var childBox in box.GetChildBoxIter())
            {
                //recursive
                ApplyStyleSheet(childBox, htmlContainer, ref cssData);
            }


        }



        /// <summary>
        /// Set the selected text style (selection text color and background color).
        /// </summary>
        /// <param name="htmlContainer"> </param>
        /// <param name="cssData">the style data</param>
        private static void SetTextSelectionStyle(HtmlContainer htmlContainer, CssActiveSheet cssData)
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

        static readonly char[] _whiteSplitter = new[] { ' ' };
        /// <summary>
        /// Assigns the given css classes to the given css box checking if matching.<br/>
        /// Support multiple classes in single attribute separated by whitespace.
        /// </summary>
        /// <param name="box">the css box to assign css to</param>
        /// <param name="cssData">the css data to use to get the matching css blocks</param>
        private static void AssignCssToSpecificClass(CssBox box, CssActiveSheet cssData)
        {
            var classes = box.HtmlTag.TryGetAttribute("class");
            //class attribute may has more than one value (multiple classes in single attribute);
            string[] classNames = classes.Split(_whiteSplitter, StringSplitOptions.RemoveEmptyEntries);

            int j = classNames.Length;
            for (int i = 0; i < j; ++i)
            {

                CssRuleSetGroup ruleSetGroup = cssData.GetRuleSetForClassName(classNames[i]);
                if (ruleSetGroup != null)
                {
                    foreach (var propDecl in ruleSetGroup.GetPropertyDeclIter())
                    {
                        AssignPropertyValueToCssBox(box, AssignPropertySource.ClassName, propDecl);
                    }
                    //---------------------------------------------------------
                    //find subgroup for more specific conditions
                    int subgroupCount = ruleSetGroup.SubGroupCount;
                    for (int m = 0; m < subgroupCount; ++m)
                    {
                        //find if selector condition match with this box
                        CssRuleSetGroup ruleSetSubGroup = ruleSetGroup.GetSubGroup(m);
                        var selector = ruleSetSubGroup.OriginalSelector;

                    }
                }
            }
        }


        private static void AssignCssToSpecificBoxWithTagName(CssBox box, CssActiveSheet cssData)
        {
            CssRuleSetGroup ruleGroup = cssData.GetRuleSetForTagName(box.HtmlTag.Name);
            if (ruleGroup != null)
            {
                //found  math tag name
                //simple selector with tag name 
                if (box.WellknownTagName == WellknownHtmlTagName.A &&
                   ruleGroup.Name == "a" &&   //block.CssClassName.Equals("a", StringComparison.OrdinalIgnoreCase) &&                 
                   !box.HtmlTag.HasAttribute("href"))
                {

                }
                else
                { 
                    foreach (WebDom.CssPropertyDeclaration decl in ruleGroup.GetPropertyDeclIter())
                    {
                        AssignPropertyValueToCssBox(box, AssignPropertySource.TagName, decl);
                    } 
                }
            }
        }
        private static void AssignCssToSpecificBoxWithId(CssBox box, CssActiveSheet cssData, string elementId)
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

        enum AssignPropertySource
        {
            Inherit,
            TagName,
            ClassName,

            StyleAttribute,
            HtmlAttribute,
            Id,
        }

        static void AssignPropertyValueToCssBox(CssBox box, AssignPropertySource propSource, WebDom.CssPropertyDeclaration decl)
        {
            if (decl.IsExpand)
            {
                return;
            }
            if (decl.MarkedAsInherit && box.ParentBox != null)
            {
                //use parent property 
                SetPropertyValueFromParent(box, decl.WellknownPropertyName);
            }
            else
            {
                if (IsStyleOnElementAllowed(box, decl))
                {
                    SetPropertyValue(box, decl);
                }
            }

        }

        static bool IsStyleOnElementAllowed(CssBox box, WebDom.CssPropertyDeclaration cssProperty)
        {
            if (box.HtmlTag != null &&
                cssProperty.WellknownPropertyName == WebDom.WellknownCssPropertyName.Display)
            {

                string value = cssProperty.GetPropertyValue(0).ToString();

                switch (box.WellknownTagName)
                {
                    case WellknownHtmlTagName.TABLE:
                        return value == CssConstants.Table;
                    case WellknownHtmlTagName.TR:
                        return value == CssConstants.TableRow;
                    case WellknownHtmlTagName.TBody:
                        return value == CssConstants.TableRowGroup;
                    case WellknownHtmlTagName.THead:
                        return value == CssConstants.TableHeaderGroup;
                    case WellknownHtmlTagName.TFoot:
                        return value == CssConstants.TableFooterGroup;
                    case WellknownHtmlTagName.COL:
                        return value == CssConstants.TableColumn;
                    case WellknownHtmlTagName.COLGROUP:
                        return value == CssConstants.TableColumnGroup;
                    case WellknownHtmlTagName.TD:
                    case WellknownHtmlTagName.TH:
                        return value == CssConstants.TableCell;
                    case WellknownHtmlTagName.CAPTION:
                        return value == CssConstants.TableCaption;
                }
            }
            return true;
        }
        /// <summary>
        /// Clone css data if it has not already been cloned.<br/>
        /// Used to preserve the base css data used when changed by style inside html.
        /// </summary>
        static CssActiveSheet CloneCssData(CssActiveSheet cssData)
        {
            //if (!cssDataChanged)
            //{                 
            //    cssData = cssData.Clone();
            //    return cssData;
            //}
            // cssData = cssData.Clone(newowner);

            return cssData.Clone(new object());
        }


        private static void TranslateAttributes(IHtmlElement tag, CssBox box)
        {
            //some html attr contains css value 
            //
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
                                    box.SetTextAlign(propValue);
                                }
                                else
                                {
                                    WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
                                     value, WebDom.CssValueHint.Iden);
                                    box.SetVerticalAlign(propValue);
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
                                string value = attr.Value.ToLower();
                                if (!string.IsNullOrEmpty(value) && value != "0")
                                {
                                    box.BorderLeftStyle = box.BorderTopStyle
                                        = box.BorderRightStyle = box.BorderBottomStyle = CssBorderStyle.Solid;// CssConstants.Solid;
                                }

                                box.BorderLeftWidth =
                                    box.BorderTopWidth =
                                    box.BorderRightWidth =
                                    box.BorderBottomWidth = TranslateLength(CssLength.MakeBorderLength(value));

                                if (tag.WellknownTagName == WellknownHtmlTagName.TABLE)
                                {
                                    if (value != "0")
                                    {
                                        ApplyTableBorder(box, CssLength.MakePixelLength(1));
                                    }
                                }
                                else
                                {
                                    box.BorderTopStyle = box.BorderLeftStyle = box.BorderRightStyle = box.BorderBottomStyle = CssBorderStyle.Solid; //CssConstants.Solid;
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
                            ApplyTablePadding(box, attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.Color:
                            box.Color = CssValueParser.GetActualColor(attr.Value.ToLower());
                            break;
                        case WebDom.WellknownHtmlName.Dir:
                            {
                                WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
                                        attr.Value.ToLower(), WebDom.CssValueHint.Iden);
                                box.SetCssDirection(propValue);
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

                            if (tag.WellknownTagName == WellknownHtmlTagName.HR)
                            {
                                box.Height = TranslateLength(attr.Value.ToLower());
                            }
                            else if (tag.WellknownTagName == WellknownHtmlTagName.FONT)
                            {
                                WebDom.CssRuleSet ruleset = CssParser.ParseCssBlock2("", attr.Value.ToLower());
                                foreach (WebDom.CssPropertyDeclaration propDecl in ruleset.GetAssignmentIter())
                                {
                                    AssignPropertyValueToCssBox(box, AssignPropertySource.HtmlAttribute, propDecl);
                                }

                                //WebDom.CssCodePrimitiveExpression prim = new WebDom.CssCodePrimitiveExpression(value, 
                                //box.SetFontSize(value);
                            }

                            break;
                        case WebDom.WellknownHtmlName.VAlign:
                            {
                                WebDom.CssCodePrimitiveExpression propValue = new WebDom.CssCodePrimitiveExpression(
                                          attr.Value.ToLower(), WebDom.CssValueHint.Iden);
                                box.SetVerticalAlign(propValue);

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
            //return htmlLength;
        }
        /// <summary>
        /// Cascades to the TD's the border spacified in the TABLE tag.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="border"></param>
        private static void ApplyTableBorder(CssBox table, CssLength borderWidth)
        {
            SetForAllCells(table, cell =>
            {
                //for all cells
                cell.BorderLeftStyle = cell.BorderTopStyle = cell.BorderRightStyle = cell.BorderBottomStyle = CssBorderStyle.Solid; // CssConstants.Solid;
                cell.BorderLeftWidth = cell.BorderTopWidth = cell.BorderRightWidth = cell.BorderBottomWidth = borderWidth;
            });
        }

        /// <summary>
        /// Cascades to the TD's the border spacified in the TABLE tag.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="padding"></param>
        private static void ApplyTablePadding(CssBox table, string padding)
        {
            CssLength length = TranslateLength(padding);
            SetForAllCells(table, cell => cell.PaddingLeft = cell.PaddingTop = cell.PaddingRight = cell.PaddingBottom = length);
        }

        /// <summary>
        /// Execute action on all the "td" cells of the table.<br/>
        /// Handle if there is "theader" or "tbody" exists.
        /// </summary>
        /// <param name="table">the table element</param>
        /// <param name="action">the action to execute</param>
        private static void SetForAllCells(CssBox table, ActionInt<CssBox> action)
        {
            foreach (var tr in table.GetChildBoxIter())
            {
                foreach (var td in tr.GetChildBoxIter())
                {
                    if (td.WellknownTagName == WellknownHtmlTagName.TD)
                    {
                        action(td);
                    }
                    else
                    {
                        foreach (var l3 in td.GetChildBoxIter())
                        {
                            action(l3);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Go over all image boxes and if its display style is set to block, put it inside another block but set the image to inline.
        /// </summary>
        /// <param name="box">the current box to correct its sub-tree</param>
        private static void CorrectImgBoxes(CssBox box)
        {
            int childIndex = 0;
            foreach (var childBox in box.GetChildBoxIter())
            {

                if (childBox is CssBoxImage && childBox.CssDisplay == CssDisplay.Block)
                {
                    //create new anonymous box
                    var block = CssBox.CreateAnonBlock(childBox.ParentBox, childIndex);
                    //move this imgbox to new child 
                    childBox.SetNewParentBox(block);
                    //childBox.Display = CssConstants.Inline;
                    childBox.CssDisplay = CssDisplay.Inline;
                }
                else
                {
                    // recursive
                    CorrectImgBoxes(childBox);
                }
                childIndex++;
            }

        }

#if DEBUG
        static int dbugCorrectCount = 0;
#endif

        /// <summary>
        /// Correct DOM tree if there is block boxes that are inside inline blocks.<br/>
        /// Need to rearrange the tree so block box will be only the child of other block box.
        /// </summary>
        /// <param name="box">the current box to correct its sub-tree</param>
        private static void CorrectBlockInsideInline(CssBox box)
        {
#if DEBUG
            dbugCorrectCount++;
#endif
            //recursive
            try
            {
                if (DomUtils.ContainsInlinesOnly(box) && !CssBox.ContainsInlinesOnlyDeep(box))
                {
                    CssBox.CorrectBlockInsideInlineImp(box);
                }
                //----------------------------------------------------------------------
                if (!DomUtils.ContainsInlinesOnly(box))
                {
                    foreach (var childBox in box.GetChildBoxIter())
                    {
                        //recursive
                        CorrectBlockInsideInline(childBox);
                    }
                }
            }
            catch (Exception ex)
            {
                box.HtmlContainer.ReportError(HtmlRenderErrorType.HtmlParsing, "Failed in block inside inline box correction", ex);
            }
        }

        #endregion



        static void SetPropertyValue(CssBox cssBox, WebDom.CssPropertyDeclaration decl)
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
                    cssBox.SetBorderCollapse(cssValue);
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
                    cssBox.SetDisplayType(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Direction:
                    cssBox.SetCssDirection(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.EmptyCells:
                    cssBox.EmptyCells = CssBoxUserUtilExtension.GetEmptyCell(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Float:
                    cssBox.Float = CssBoxUserUtilExtension.GetFloat(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Position:
                    cssBox.SetCssPosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.LineHeight:
                    cssBox.SetLineHeight(cssValue.GetTranslatedStringValue());
                    break;
                case WebDom.WellknownCssPropertyName.VerticalAlign:
                    cssBox.SetVerticalAlign(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.TextIndent:
                    cssBox.TextIndent = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.TextAlign:
                    cssBox.SetTextAlign(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.TextDecoration:
                    cssBox.SetTextDecoration(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Whitespace:
                    cssBox.SetWhitespace(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.WordBreak:
                    cssBox.SetWordBreak(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Visibility:
                    cssBox.SetVisibility(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.WordSpacing:
                    cssBox.WordSpacing = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.FontFamily:
                    cssBox.FontFamily = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.FontSize:
                    cssBox.SetFontSize(cssValue);

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
                    cssBox.SetOverflow(cssValue);
                    break;
            }
        }


        /// <summary>
        /// assign property value from parent
        /// </summary>
        /// <param name="cssBox"></param>
        /// <param name="propName"></param>
        static void SetPropertyValueFromParent(CssBox cssBox, HtmlRenderer.WebDom.WellknownCssPropertyName propName)
        {
            CssBox parentCssBox = cssBox.ParentBox;

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
                    //cssBox.SetTextDecoration(value);
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