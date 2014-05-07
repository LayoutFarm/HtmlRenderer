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
using HtmlRenderer.Dom;
using HtmlRenderer.Entities;
using HtmlRenderer.Handlers;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Parse
{
    /// <summary>
    /// Handle css DOM tree generation from raw html and stylesheet.
    /// </summary>
    internal static class DomParser
    {
        /// <summary>
        /// Generate css tree by parsing the given html and applying the given css style data on it.
        /// </summary>
        /// <param name="html">the html to parse</param>
        /// <param name="htmlContainer">the html container to use for reference resolve</param>
        /// <param name="cssData">the css data to use</param>
        /// <returns>the root of the generated tree</returns>
        public static CssBox GenerateCssTree(
            string html,
            HtmlContainer htmlContainer,
            ref CssData cssData)
        {

            //1. generate css box  from html data
            CssBox root = HtmlParser.ParseDocument(html);
            //2. decorate cssbox with styles
            if (root != null)
            {

                root.HtmlContainer = htmlContainer;

                bool cssDataChanged = false;
                CascadeStyles(root, htmlContainer, ref cssData, ref cssDataChanged);
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
        private static void CascadeStyles(CssBox box, HtmlContainer htmlContainer, ref CssData cssData, ref bool cssDataChanged)
        {
            //recursive
            box.InheritStyle();

            if (box.HtmlTag != null)
            {
                // try assign style using the html element tag
                AssignCssBlocks(box, cssData, box.HtmlTag.Name);

                // try assign style using the "class" attribute of the html element
                if (box.HtmlTag.HasAttribute("class"))
                {
                    AssignClassCssBlocks(box, cssData);
                }

                // try assign style using the "id" attribute of the html element
                if (box.HtmlTag.HasAttribute("id"))
                {
                    var id = box.HtmlTag.TryGetAttribute("id");
                    AssignCssBlocks(box, cssData, "#" + id);
                }

                TranslateAttributes(box.HtmlTag, box);

                // Check for the style="" attribute
                if (box.HtmlTag.HasAttribute("style"))
                {
                    var block = CssParser.ParseCssBlock(box.HtmlTag.Name, box.HtmlTag.TryGetAttribute("style"));
                    AssignCssBlock(box, block);
                }

                // Check for the <style> tag
                //if (box.HtmlTag.Name.Equals("style", StringComparison.CurrentCultureIgnoreCase) && box.Boxes.Count == 1)
                if (box.WellknownTagName == WellknownHtmlTagName.STYLE && box.ChildCount == 1)
                {
                    CloneCssData(ref cssData, ref cssDataChanged);
                    CssParser.ParseStyleSheet(cssData, box.GetFirstChild().Text.CutSubstring());
                }

                // Check for the <link rel=stylesheet> tag
                //                if (box.HtmlTag.Name.Equals("link", StringComparison.CurrentCultureIgnoreCase) &&
                if (box.WellknownTagName == WellknownHtmlTagName.LINK &&
                    box.GetAttribute("rel", string.Empty).Equals("stylesheet", StringComparison.CurrentCultureIgnoreCase))
                {
                    CloneCssData(ref cssData, ref cssDataChanged);
                    string stylesheet;
                    CssData stylesheetData;
                    StylesheetLoadHandler.LoadStylesheet(htmlContainer,
                        box.GetAttribute("href", string.Empty),
                        box.HtmlTag.Attributes, out stylesheet, out stylesheetData);

                    if (stylesheet != null)
                        CssParser.ParseStyleSheet(cssData, stylesheet);
                    else if (stylesheetData != null)
                        cssData.Combine(stylesheetData);
                }
            }

            // cascade text decoration only to boxes that actually have text so it will be handled correctly.
            //if (box.TextDecoration != String.Empty && box.Text == null)
            //{
            //    foreach (var childBox in box.Boxes)
            //    {
            //        childBox.TextDecoration = box.TextDecoration;
            //    }
            //    box.TextDecoration = string.Empty;
            //}
            if (box.TextDecoration != CssTextDecoration.NotAssign && box.Text == null)
            {
                foreach (var childBox in box.GetChildBoxIter())
                {
                    childBox.TextDecoration = box.TextDecoration;
                }
                box.TextDecoration = CssTextDecoration.NotAssign;
            }
            foreach (var childBox in box.GetChildBoxIter())
            {
                CascadeStyles(childBox, htmlContainer, ref cssData, ref cssDataChanged);
            }
        }

        /// <summary>
        /// Set the selected text style (selection text color and background color).
        /// </summary>
        /// <param name="htmlContainer"> </param>
        /// <param name="cssData">the style data</param>
        private static void SetTextSelectionStyle(HtmlContainer htmlContainer, CssData cssData)
        {
            htmlContainer.SelectionForeColor = Color.Empty;
            htmlContainer.SelectionBackColor = Color.Empty;

            if (cssData.ContainsCssBlock("::selection"))
            {
                var blocks = cssData.GetCssBlock("::selection");
                foreach (var block in blocks)
                {
                    if (block.Properties.ContainsKey("color"))
                        htmlContainer.SelectionForeColor = CssValueParser.GetActualColor(block.Properties["color"]);
                    if (block.Properties.ContainsKey("background-color"))
                        htmlContainer.SelectionBackColor = CssValueParser.GetActualColor(block.Properties["background-color"]);
                }
            }
        }

        /// <summary>
        /// Assigns the given css classes to the given css box checking if matching.<br/>
        /// Support multiple classes in single attribute separated by whitespace.
        /// </summary>
        /// <param name="box">the css box to assign css to</param>
        /// <param name="cssData">the css data to use to get the matching css blocks</param>
        private static void AssignClassCssBlocks(CssBox box, CssData cssData)
        {
            var classes = box.HtmlTag.TryGetAttribute("class");

            var startIdx = 0;
            while (startIdx < classes.Length)
            {
                while (startIdx < classes.Length && classes[startIdx] == ' ')
                    startIdx++;

                if (startIdx < classes.Length)
                {
                    var endIdx = classes.IndexOf(' ', startIdx);

                    if (endIdx < 0)
                        endIdx = classes.Length;

                    var cls = "." + classes.Substring(startIdx, endIdx - startIdx);
                    AssignCssBlocks(box, cssData, cls);
                    AssignCssBlocks(box, cssData, box.HtmlTag.Name + cls);

                    startIdx = endIdx + 1;
                }
            }
        }

        /// <summary>
        /// Assigns the given css style blocks to the given css box checking if matching.
        /// </summary>
        /// <param name="box">the css box to assign css to</param>
        /// <param name="cssData">the css data to use to get the matching css blocks</param>
        /// <param name="className">the class selector to search for css blocks</param>
        private static void AssignCssBlocks(CssBox box, CssData cssData, string className)
        {
            var blocks = cssData.GetCssBlock(className);
            foreach (var block in blocks)
            {
                if (IsBlockAssignableToBox(box, block))
                {
                    AssignCssBlock(box, block);
                }
            }
        }

        /// <summary>
        /// Check if the given css block is assignable to the given css box.<br/>
        /// the block is assignable if it has no hierarchical selectors or if the hierarchy matches.<br/>
        /// Special handling for ":hover" pseudo-class.<br/>
        /// </summary>
        /// <param name="box">the box to check assign to</param>
        /// <param name="block">the block to check assign of</param>
        /// <returns>true - the block is assignable to the box, false - otherwise</returns>
        private static bool IsBlockAssignableToBox(CssBox box, CssBlock block)
        {
            bool assignable = true;
            if (block.Selectors != null)
            {
                assignable = IsBlockAssignableToBoxWithSelector(box, block);
            }
            //else if (box.HtmlTag.Name.Equals("a", StringComparison.OrdinalIgnoreCase) && block.Class.Equals("a", StringComparison.OrdinalIgnoreCase) && !box.HtmlTag.HasAttribute("href"))
            else if (box.WellknownTagName == WellknownHtmlTagName.A &&
                 block.Class.Equals("a", StringComparison.OrdinalIgnoreCase) && !box.HtmlTag.HasAttribute("href"))
            {
                assignable = false;
            }

            if (assignable && block.Hover)
            {
                box.HtmlContainer.AddHoverBox(box, block);
                assignable = false;
            }

            return assignable;
        }

        /// <summary>
        /// Check if the given css block is assignable to the given css box by validating the selector.<br/>
        /// </summary>
        /// <param name="box">the box to check assign to</param>
        /// <param name="block">the block to check assign of</param>
        /// <returns>true - the block is assignable to the box, false - otherwise</returns>
        private static bool IsBlockAssignableToBoxWithSelector(CssBox box, CssBlock block)
        {
            foreach (var selector in block.Selectors)
            {
                bool matched = false;
                while (!matched)
                {
                    box = box.ParentBox;
                    while (box != null && box.HtmlTag == null)
                    {
                        box = box.ParentBox;
                    }

                    if (box == null)
                    {
                        return false;
                    }

                    if (box.HtmlTag.Name.Equals(selector.Class, StringComparison.InvariantCultureIgnoreCase))
                    {
                        matched = true;
                    }

                    if (!matched && box.HtmlTag.HasAttribute("class"))
                    {
                        var className = box.HtmlTag.TryGetAttribute("class");
                        if (selector.Class.Equals("." + className, StringComparison.InvariantCultureIgnoreCase) || selector.Class.Equals(box.HtmlTag.Name + "." + className, StringComparison.InvariantCultureIgnoreCase))
                            matched = true;
                    }

                    if (!matched && box.HtmlTag.HasAttribute("id"))
                    {
                        var id = box.HtmlTag.TryGetAttribute("id");
                        if (selector.Class.Equals("#" + id, StringComparison.InvariantCultureIgnoreCase))
                            matched = true;
                    }

                    if (!matched && selector.DirectParent)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Assigns the given css style block properties to the given css box.
        /// </summary>
        /// <param name="box">the css box to assign css to</param>
        /// <param name="block">the css block to assign</param>
        private static void AssignCssBlock(CssBox box, CssBlock block)
        {
            foreach (var prop in block.Properties)
            {
                //สำหรับทุก property 
                var value = prop.Value;
                if (prop.Value == CssConstants.Inherit && box.ParentBox != null)
                {
                    value = CssUtils.GetPropertyValue(box.ParentBox, prop.Key);
                }
                if (IsStyleOnElementAllowed(box, prop.Key, value))
                {
                    CssUtils.SetPropertyValue(box, prop.Key, value);
                }
            }
        }

        /// <summary>
        /// Check if the given style is allowed to be set on the given css box.<br/>
        /// Used to prevent invalid CssBoxes creation like table with inline display style.
        /// </summary>
        /// <param name="box">the css box to assign css to</param>
        /// <param name="key">the style key to cehck</param>
        /// <param name="value">the style value to check</param>
        /// <returns>true - style allowed, false - not allowed</returns>
        private static bool IsStyleOnElementAllowed(CssBox box, string key, string value)
        {
            if (box.HtmlTag != null && key == HtmlConstants.Display)
            {
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
        private static void CloneCssData(ref CssData cssData, ref bool cssDataChanged)
        {
            if (!cssDataChanged)
            {
                cssDataChanged = true;
                cssData = cssData.Clone();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="box"></param>
        private static void TranslateAttributes(HtmlTag tag, CssBox box)
        {
            if (tag.HasAttributes())
            {
                foreach (string att in tag.Attributes.Keys)
                {
                    string value = tag.Attributes[att];

                    switch (att)
                    {
                        case HtmlConstants.Align:
                            if (value == HtmlConstants.Left || value == HtmlConstants.Center || value == HtmlConstants.Right || value == HtmlConstants.Justify)
                            {
                                //box.TextAlign = value.ToLower();
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
                            box.BackgroundColor = value.ToLower();
                            break;
                        case HtmlConstants.Border:
                            if (!string.IsNullOrEmpty(value) && value != "0")
                            {
                                box.BorderLeftStyle = box.BorderTopStyle = box.BorderRightStyle = box.BorderBottomStyle = CssBorderStyle.Solid;// CssConstants.Solid;
                            }
                            box.BorderLeftWidth = box.BorderTopWidth = box.BorderRightWidth = box.BorderBottomWidth = TranslateLength(CssLength.MakeBorderLength(value));

                            if (tag.Name == HtmlConstants.Table)
                            {
                                if (value != "0")
                                    ApplyTableBorder(box, "1px");
                            }
                            else
                            {
                                box.BorderTopStyle = box.BorderLeftStyle = box.BorderRightStyle = box.BorderBottomStyle = CssBorderStyle.Solid; //CssConstants.Solid;
                            }
                            break;
                        case HtmlConstants.Bordercolor:
                            box.BorderLeftColor = box.BorderTopColor = box.BorderRightColor = box.BorderBottomColor = CssValueParser.GetActualColor(value.ToLower());
                            break;
                        case HtmlConstants.Cellspacing:

                            //box.BorderSpacing = TranslateLength(value);
                            box.BorderSpacingHorizontal = box.BorderSpacingVertical = new CssLength(TranslateLength(value));
                            break;
                        case HtmlConstants.Cellpadding:
                            ApplyTablePadding(box, value);
                            break;
                        case HtmlConstants.Color:
                            box.Color = value.ToLower();
                            break;
                        case HtmlConstants.Dir:
                            //box.Direction = value.ToLower();
                            box.SetCssDirection(value.ToLower());
                            break;
                        case HtmlConstants.Face:
                            box.FontFamily = CssParser.ParseFontFamily(value);
                            break;
                        case HtmlConstants.Height:
                            box.Height = new CssLength(TranslateLength(value));
                            break;
                        case HtmlConstants.Hspace:
                            box.MarginRight = box.MarginLeft = new CssLength(TranslateLength(value));
                            break;
                        case HtmlConstants.Nowrap:
                            box.WhiteSpace = CssWhiteSpace.NoWrap;
                            break;
                        case HtmlConstants.Size:
                            if (tag.Name.Equals(HtmlConstants.Hr, StringComparison.OrdinalIgnoreCase))
                                box.Height = new CssLength(TranslateLength(value));
                            else if (tag.Name.Equals(HtmlConstants.Font, StringComparison.OrdinalIgnoreCase))
                                box.FontSize = value;
                            break;
                        case HtmlConstants.Valign:
                            box.SetVerticalAlign(value.ToLower());

                            break;
                        case HtmlConstants.Vspace:
                            box.MarginTop = box.MarginBottom = new CssLength(TranslateLength(value));
                            break;
                        case HtmlConstants.Width:
                            box.Width = new CssLength(TranslateLength(value));
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
        public static string TranslateLength(string htmlLength)
        {
            CssLength len = new CssLength(htmlLength);
            if (len.HasError)
            {
                return string.Format(NumberFormatInfo.InvariantInfo, "{0}px", htmlLength);
            }
            return htmlLength;
        }
        private static CssLength TranslateLength(CssLength len)
        {

            if (len.HasError)
            {
                //if unknown unit number
                return new CssLength(len.Number, false, CssUnit.Pixels);
                //return string.Format(NumberFormatInfo.InvariantInfo, "{0}px", htmlLength);
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
            var length = TranslateLength(padding);
            //SetForAllCells(table, cell => cell.PaddingLeft = cell.PaddingTop = cell.PaddingRight = cell.PaddingBottom = length);
            SetForAllCells(table, cell => cell.PaddingLeft = cell.PaddingTop = cell.PaddingRight = cell.PaddingBottom = new CssLength(padding));
        }

        /// <summary>
        /// Execute action on all the "td" cells of the table.<br/>
        /// Handle if there is "theader" or "tbody" exists.
        /// </summary>
        /// <param name="table">the table element</param>
        /// <param name="action">the action to execute</param>
        private static void SetForAllCells(CssBox table, ActionInt<CssBox> action)
        {
            foreach (var l1 in table.GetChildBoxIter())
            {
                foreach (var l2 in l1.GetChildBoxIter())
                {
                    //if (l2.HtmlTag != null && l2.HtmlTag.Name == "td")
                    if (l2.WellknownTagName == WellknownHtmlTagName.TD)
                    {
                        action(l2);
                    }
                    else
                    {
                        foreach (var l3 in l2.GetChildBoxIter())
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
            foreach (var childBox in box.GetChildBoxIter())
            {
                //if (childBox is CssBoxImage &&  childBox.Display == CssConstants.Block)
                if (childBox is CssBoxImage && childBox.CssDisplay == CssDisplay.Block)
                {
                    //create new anonymous box
                    var block = CssBox.CreateBlock(childBox.ParentBox, null, childBox);
                    //move this imgbox to new child
                    childBox.ParentBox = block;
                    //childBox.Display = CssConstants.Inline;
                    childBox.CssDisplay = CssDisplay.Inline;
                }
                else
                {
                    // recursive
                    CorrectImgBoxes(childBox);
                }
            }
            //for (int i = box.ChildCount - 1; i >= 0; i--)
            //{
            //    var childBox = box.Boxes[i];
            //    //if (childBox is CssBoxImage &&  childBox.Display == CssConstants.Block)
            //    if (childBox is CssBoxImage && childBox.DisplayType == CssBoxDisplayType.Block)
            //    {
            //        //create new anonymous box
            //        var block = CssBox.CreateBlock(childBox.ParentBox, null, childBox);
            //        childBox.ParentBox = block;
            //        //childBox.Display = CssConstants.Inline;
            //        childBox.DisplayType = CssBoxDisplayType.Inline;
            //    }
            //    else
            //    {
            //        // recursive
            //        CorrectImgBoxes(childBox);
            //    }
            //}
        }


        /// <summary>
        /// Correct DOM tree if there is block boxes that are inside inline blocks.<br/>
        /// Need to rearrange the tree so block box will be only the child of other block box.
        /// </summary>
        /// <param name="box">the current box to correct its sub-tree</param>
        private static void CorrectBlockInsideInline(CssBox box)
        {
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