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
                        AssignPropertyValueToCssBox(box, propDecl);
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
                        AssignPropertyValueToCssBox(box, propDecl);
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
                    AssignStyleToCssBox2(box, ruleGroup);
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


        static void AssignPropertyValueToCssBox(CssBox box, WebDom.CssPropertyDeclaration decl)
        {

            if (decl.IsExpand)
            {

                return;
            }
            if (decl.MarkedAsInherit && box.ParentBox != null)
            {
                //use parent property 
                CssUtils.SetPropertyValueFromParent(box, decl.PropertyName);
            }
            else
            {
                if (IsStyleOnElementAllowed2(box, decl))
                {
                    string value = null;
                    int valueCount = decl.ValueCount;
                    switch (valueCount)
                    {
                        case 0:
                            {
                                throw new NotSupportedException();
                            } break;
                        case 1:
                            {
                                //convert to string ?
                                value = decl.GetPropertyValue(0).ToString();
                            } break;
                        default:
                            {

                                throw new NotSupportedException();
                            } break;
                    }

                    CssUtils.SetPropertyValue2(box, decl);
                }
            }

        }
        static void AssignStyleToCssBox2(CssBox box, CssRuleSetGroup block)
        {
            foreach (WebDom.CssPropertyDeclaration decl in block.GetPropertyDeclIter())
            {
                AssignPropertyValueToCssBox(box, decl);
            }

        }
        static bool IsStyleOnElementAllowed2(CssBox box, WebDom.CssPropertyDeclaration cssProperty)
        {
            if (box.HtmlTag != null && cssProperty.PropertyName == HtmlConstants.Display)
            {
                string value = cssProperty.GetPropertyValue(0).ToString();

                switch (box.HtmlTag.Name)
                {
                    case HtmlConstants.Table:
                        return value == CssConstants.Table;
                    case HtmlConstants.Tr:
                        return value == CssConstants.TableRow;
                    case HtmlConstants.Tbody:
                        return value == CssConstants.TableRowGroup;
                    case HtmlConstants.Thead:
                        return value == CssConstants.TableHeaderGroup;
                    case HtmlConstants.Tfoot:
                        return value == CssConstants.TableFooterGroup;
                    case HtmlConstants.Col:
                        return value == CssConstants.TableColumn;
                    case HtmlConstants.Colgroup:
                        return value == CssConstants.TableColumnGroup;
                    case HtmlConstants.Td:
                    case HtmlConstants.Th:
                        return value == CssConstants.TableCell;
                    case HtmlConstants.Caption:
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


        private static void TranslateAttributes(IHtmlTag tag, CssBox box)
        {

            if (tag.HasAttributes())
            {
                foreach (IHtmlAttribute attr in tag.GetAttributeIter())
                {
                    string value = attr.Value;
                    switch (attr.Name)
                    {
                        case HtmlConstants.Align:

                            if (value == HtmlConstants.Left
                                || value == HtmlConstants.Center
                                || value == HtmlConstants.Right
                                || value == HtmlConstants.Justify)
                            {
                                box.SetTextAlign(value.ToLower());
                            }
                            else
                            {
                                box.SetVerticalAlign(value.ToLower());
                            }
                            break;
                        case HtmlConstants.Background:
                            box.BackgroundImage = value.ToLower();
                            break;
                        case HtmlConstants.Bgcolor:
                            box.BackgroundColor = CssValueParser.GetActualColor(value.ToLower());
                            break;
                        case HtmlConstants.Border:

                            if (!string.IsNullOrEmpty(value) && value != "0")
                            {
                                box.BorderLeftStyle = box.BorderTopStyle = box.BorderRightStyle = box.BorderBottomStyle = CssBorderStyle.Solid;// CssConstants.Solid;
                            }

                            box.BorderLeftWidth =
                                box.BorderTopWidth =
                                box.BorderRightWidth =
                                box.BorderBottomWidth = TranslateLength(CssLength.MakeBorderLength(value));

                            if (tag.WellknownTagName == WellknownHtmlTagName.TABLE)
                            {
                                if (value != "0")
                                {
                                    ApplyTableBorder(box, "1px");
                                }
                            }
                            else
                            {
                                box.BorderTopStyle = box.BorderLeftStyle = box.BorderRightStyle = box.BorderBottomStyle = CssBorderStyle.Solid; //CssConstants.Solid;
                            }
                            break;
                        case HtmlConstants.Bordercolor:
                            box.BorderLeftColor =
                                box.BorderTopColor =
                                box.BorderRightColor =
                                box.BorderBottomColor = CssValueParser.GetActualColor(value.ToLower());
                            break;
                        case HtmlConstants.Cellspacing:
                            box.BorderSpacingHorizontal = box.BorderSpacingVertical = TranslateLength(value);
                            break;
                        case HtmlConstants.Cellpadding:
                            ApplyTablePadding(box, value);
                            break;
                        case HtmlConstants.Color:
                            box.Color = CssValueParser.GetActualColor(value.ToLower());

                            break;
                        case HtmlConstants.Dir:
                            box.SetCssDirection(value.ToLower());
                            break;
                        case HtmlConstants.Face:
                            box.FontFamily = CssParser.ParseFontFamily(value);
                            break;
                        case HtmlConstants.Height:
                            box.Height = TranslateLength(value);
                            break;
                        case HtmlConstants.Hspace:
                            box.MarginRight = box.MarginLeft = TranslateLength(value);
                            break;
                        case HtmlConstants.Nowrap:
                            box.WhiteSpace = CssWhiteSpace.NoWrap;
                            break;
                        case HtmlConstants.Size:

                            if (tag.WellknownTagName == WellknownHtmlTagName.HR)
                            {
                                box.Height = TranslateLength(value);

                            }
                            else if (tag.WellknownTagName == WellknownHtmlTagName.FONT)
                            {
                                box.SetFontSize(value);
                            }

                            break;
                        case HtmlConstants.Valign:
                            box.SetVerticalAlign(value.ToLower());

                            break;
                        case HtmlConstants.Vspace:
                            box.MarginTop = box.MarginBottom = TranslateLength(value);
                            break;
                        case HtmlConstants.Width:
                            box.Width = TranslateLength(value);
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
                return new CssLength(string.Format(NumberFormatInfo.InvariantInfo, "{0}px", htmlLength));
            }
            return len;
        }
        private static CssLength TranslateLength(CssLength len)
        {

            if (len.HasError)
            {
                //if unknown unit number
                return new CssLength(len.Number, false, CssUnit.Pixels);
            }
            return len;
            //return htmlLength;
        }
        /// <summary>
        /// Cascades to the TD's the border spacified in the TABLE tag.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="border"></param>
        private static void ApplyTableBorder(CssBox table, string border)
        {
            SetForAllCells(table, cell =>
            {
                cell.BorderLeftStyle = cell.BorderTopStyle = cell.BorderRightStyle = cell.BorderBottomStyle = CssBorderStyle.Solid; // CssConstants.Solid;
                cell.BorderLeftWidth = cell.BorderTopWidth = cell.BorderRightWidth = cell.BorderBottomWidth = CssLength.MakeBorderLength(border);
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





    }
}