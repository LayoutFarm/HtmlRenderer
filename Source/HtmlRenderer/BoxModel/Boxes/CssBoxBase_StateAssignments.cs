//BSD, 2014, WinterCore

using System;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{


    partial class CssBoxBase
    {
        
        int _prop_pass_eval;
        int _prop_wait_eval;


        static class CssBoxBaseAssignments
        {

            //---------------------------------------------
            public const int WIDTH = 1 << (6 - 1);
            public const int HEIGHT = 1 << (7 - 1);
            //---------------------------------------------
            public const int PADDING_LEFT = 1 << (8 - 1);
            public const int PADDING_TOP = 1 << (10 - 1);
            public const int PADDING_RIGHT = 1 << (11 - 1);
            public const int PADDING_BOTTOM = 1 << (12 - 1);
            //---------------------------------------------
            public const int MARGIN_LEFT_EVALULATED = 1 << (13 - 1);
            public const int MARGIN_TOP_EVALULATED = 1 << (14 - 1);
            public const int MARGIN_RIGHT_EVALULATED = 1 << (15 - 1);
            public const int MARGIN_BOTTOM_EVALULATED = 1 << (16 - 1);
            //---------------------------------------------
            public const int BORDER_WIDTH_LEFT = 1 << (17 - 1);
            public const int BORDER_WIDTH_TOP = 1 << (18 - 1);
            public const int BORDER_WIDTH_RIGHT = 1 << (19 - 1);
            public const int BORDER_WIDTH_BOTTOM = 1 << (20 - 1);
            //---------------------------------------------
            public const int LINE_HEIGHT = 1 << (21 - 1);
            public const int WORD_SPACING = 1 << (22 - 1);
            public const int TEXT_INDENT = 1 << (23 - 1);
            //---------------------------------------------
            public const int BORDER_SPACING_H = 1 << (24 - 1);
            public const int BORDER_SPACING_V = 1 << (25 - 1);
            //---------------------------------------------
            public const int CORNER_NE = 1 << (26 - 1);
            public const int CORNER_NW = 1 << (27 - 1);
            public const int CORNER_SE = 1 << (28 - 1);
            public const int CORNER_SW = 1 << (29 - 1);

        }

    }
}
