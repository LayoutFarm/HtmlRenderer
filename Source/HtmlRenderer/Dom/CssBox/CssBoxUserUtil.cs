using System;
using System.Drawing;

using System.Collections.Generic;

using HtmlRenderer.Entities;
using HtmlRenderer.Handlers;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{

    public static class CssBoxUserUtilExtension
    {

        static Dictionary<string, CssBoxDisplayType> cssBoxDisplayTypes = new Dictionary<string, CssBoxDisplayType>();

        static CssBoxUserUtilExtension()
        {
            //setup  dictionary 

        }
        public static string GetDisplayString(this CssBox box)
        {
            switch (box.CssDisplay)
            {
                case CssBoxDisplayType.Block:
                    {
                        return CssConstants.Block;
                    }
                case CssBoxDisplayType.Inline:
                    {
                        return CssConstants.Inline;
                    }
                case CssBoxDisplayType.None:
                    {
                        return CssConstants.None;
                    }
                case CssBoxDisplayType.Table:
                    {
                        return CssConstants.Table;
                    }
                case CssBoxDisplayType.TableCaption:
                    {
                        return CssConstants.TableCaption;
                    }
                case CssBoxDisplayType.TableCell:
                    {
                        return CssConstants.TableCell;
                    }
                case CssBoxDisplayType.TableColumn:
                    {
                        return CssConstants.TableColumn;
                    }
                case CssBoxDisplayType.TableColumnGroup:
                    {
                        return CssConstants.TableColumnGroup;
                    }
                case CssBoxDisplayType.TableFooterGroup:
                    {
                        return CssConstants.TableFooterGroup;
                    }
                case CssBoxDisplayType.TableHeaderGroup:
                    {
                        return CssConstants.TableHeaderGroup;
                    }
                case CssBoxDisplayType.TableRow:
                    {
                        return CssConstants.TableRow;
                    }
                case CssBoxDisplayType.TableRowGroup:
                    {
                        return CssConstants.TableRow;
                    }
                case CssBoxDisplayType.ListItem:
                    {
                        return CssConstants.ListItem;
                    }
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }
        public static void SetDisplayType(this CssBox box, string cssdisplayValue)
        {
            switch (cssdisplayValue)
            {
                case CssConstants.Block:
                    {
                        box.CssDisplay = CssBoxDisplayType.Block;
                    } break;
                case CssConstants.Inline:
                    {
                        box.CssDisplay = CssBoxDisplayType.Inline;
                    } break;
                case CssConstants.None:
                    {
                        box.CssDisplay = CssBoxDisplayType.None;
                    } break;
                case CssConstants.Table:
                    {
                        box.CssDisplay = CssBoxDisplayType.Table;
                    } break;
                case CssConstants.TableCaption:
                    {
                        box.CssDisplay = CssBoxDisplayType.TableCaption;
                    } break;
                case CssConstants.TableCell:
                    {
                        box.CssDisplay = CssBoxDisplayType.TableCell;
                    } break;
                case CssConstants.TableColumn:
                    {
                        box.CssDisplay = CssBoxDisplayType.TableColumn;
                    } break;
                case CssConstants.TableColumnGroup:
                    {
                        box.CssDisplay = CssBoxDisplayType.TableColumnGroup;
                    } break;
                case CssConstants.TableFooterGroup:
                    {
                        box.CssDisplay = CssBoxDisplayType.TableFooterGroup;
                    } break;
                case CssConstants.TableHeaderGroup:
                    {
                        box.CssDisplay = CssBoxDisplayType.TableHeaderGroup;
                    } break;
                case CssConstants.TableRow:
                    {
                        box.CssDisplay = CssBoxDisplayType.TableRow;
                    } break;
                case CssConstants.TableRowGroup:
                    {
                        box.CssDisplay = CssBoxDisplayType.TableRowGroup;
                    } break;
                case CssConstants.ListItem:
                    {
                        box.CssDisplay = CssBoxDisplayType.ListItem;
                    } break;
                case CssConstants.InlineBlock:
                    {
                        box.CssDisplay = CssBoxDisplayType.InlineBlock;
                    } break;
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }
        //---------------------------------

        public static string GetCssDirection(this CssBox box)
        {
            switch (box.CssDirection)
            {
                default:
                    {
                        throw new NotSupportedException();
                    }
                case CssDirection.Rtl:
                    return CssConstants.Rtl;
                case CssDirection.Ltl:
                    return CssConstants.Ltr;
            }

        }
        public static void SetCssDirection(this CssBox box, string value)
        {

            switch (value)
            {
                case CssConstants.Rtl:
                    box.CssDirection = CssDirection.Rtl;
                    break;
                case CssConstants.Ltr:
                    box.CssDirection = CssDirection.Ltl;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        public static void SetCssPosition(this CssBox box, string value)
        {
            switch (value)
            {
                case CssConstants.Absolute:
                    box.Position = CssPositionType.Absolute;
                    break;
                case CssConstants.Relative:
                    box.Position = CssPositionType.Relative;
                    break;
                case CssConstants.Fixed:
                    box.Position = CssPositionType.Fixed;
                    break;
                case CssConstants.Static:
                    box.Position = CssPositionType.Static;
                    break;
            }
        }

        public static void SetWordBreak(this CssBox box, string value)
        {

            switch (value)
            {
                case CssConstants.Normal:
                    box.WordBreak = CssWordBreak.Normal;
                    break;
                case CssConstants.BreakAll:
                    box.WordBreak = CssWordBreak.BreakAll;
                    break;
                case CssConstants.KeepAll:
                    box.WordBreak = CssWordBreak.KeepAll;
                    break;
                case CssConstants.Inherit:
                    box.WordBreak = CssWordBreak.Inherit;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        public static void SetTextDecoration(this CssBox box, string value)
        {
            if (value == null)
            {
                box.TextDecoration = CssTextDecoration.NotAssign;
            }
            switch (value)
            {
                case "":
                    box.TextDecoration = CssTextDecoration.NotAssign;
                    break;
                case CssConstants.None:
                    box.TextDecoration = CssTextDecoration.None;
                    break;
                case CssConstants.LineThrough:
                    box.TextDecoration = CssTextDecoration.LineThrough;
                    break;
                case CssConstants.Underline:
                    box.TextDecoration = CssTextDecoration.Underline;
                    break;
                case CssConstants.Overline:
                    box.TextDecoration = CssTextDecoration.Overline;
                    break;
            }
        }
        public static void SetOverflow(this CssBox box, string value)
        {
            switch (value)
            {
                case CssConstants.Hidden:
                    box.Overflow = CssOverflow.Hidden;
                    break;
                case CssConstants.Auto:
                    box.Overflow = CssOverflow.Auto;
                    break;
                case CssConstants.Scroll:
                    box.Overflow = CssOverflow.Scroll;
                    break;
                case CssConstants.Visible:
                    box.Overflow = CssOverflow.Visible;
                    break;
                default:
                    {
                        throw new NotSupportedException();
                    }
            }

        }
        public static void SetTextAlign(this CssBox box, string value)
        {
            switch (value)
            {
                default:
                case "":
                    {
                        box.CssTextAlign = CssTextAlign.NotAssign;
                    } break;
                case CssConstants.Left:
                    box.CssTextAlign = CssTextAlign.Left;
                    break;
                case CssConstants.Right:
                    box.CssTextAlign = CssTextAlign.Right;
                    break;
                case CssConstants.Center:
                    box.CssTextAlign = CssTextAlign.Center;
                    break;
                case CssConstants.Justify:
                    box.CssTextAlign = CssTextAlign.Justify;
                    break;
                case CssConstants.Inherit:
                    box.CssTextAlign = CssTextAlign.Inherit;
                    break;
            }
        }
        public static void SetVerticalAlign(this CssBox box, string value)
        {
            switch (value)
            {
                case CssConstants.Sub:
                    box.VerticalAlign = CssVerticalAlign.Sub;
                    break;
                case CssConstants.Super:
                    box.VerticalAlign = CssVerticalAlign.Super;
                    break;
                case CssConstants.TextTop:
                    box.VerticalAlign = CssVerticalAlign.TextTop;
                    break;
                case CssConstants.TextBottom:
                    box.VerticalAlign = CssVerticalAlign.TextBottom;
                    break;
                case CssConstants.Top:
                    box.VerticalAlign = CssVerticalAlign.Top;
                    break;
                case CssConstants.Bottom:
                    box.VerticalAlign = CssVerticalAlign.Bottom;
                    break;
                case CssConstants.Middle:
                    box.VerticalAlign = CssVerticalAlign.Middle;
                    break;
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }
        public static void SetVisibility(this CssBox box, string value)
        {
            switch (value)
            {
                case CssConstants.Visible:
                    box.CssVisibility = CssVisibility.Visible;
                    break;
                case CssConstants.Hidden:
                    box.CssVisibility = CssVisibility.Hidden;
                    break;
                case CssConstants.Collapse:
                    box.CssVisibility = CssVisibility.Collapse;
                    break;
                case CssConstants.Inherit:
                    box.CssVisibility = CssVisibility.Inherit;
                    break;
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }
        
        public static void SetWhitespace(this CssBox box, string value)
        {
            switch (value)
            {
                case CssConstants.Pre:
                    {
                        box.WhiteSpace = CssWhiteSpace.Pre;
                    } break;
                case CssConstants.PreLine:
                    {
                        box.WhiteSpace = CssWhiteSpace.PreLine;
                    } break;
                case CssConstants.PreWrap:
                    {
                        box.WhiteSpace = CssWhiteSpace.PreWrap;
                    } break;
                case CssConstants.NoWrap:
                    {
                        box.WhiteSpace = CssWhiteSpace.NoWrap;
                    } break;
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }
    }
}