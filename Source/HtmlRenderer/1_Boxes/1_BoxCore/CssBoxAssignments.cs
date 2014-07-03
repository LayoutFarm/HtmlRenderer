//BSD, 2014, WinterDev

using System;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{

    static class CssBoxAssignments
    {
        public const int LINE_HEIGHT = 1 << (21 - 1);
        public const int WORD_SPACING = 1 << (22 - 1);
        public const int TEXT_INDENT = 1 << (23 - 1);
        //---------------------------------------------
        public const int BORDER_SPACING_H = 1 << (24 - 1);
        public const int BORDER_SPACING_V = 1 << (25 - 1);
        //--------------------------------------------- 
    }
     
}