using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace HtmlRenderer.Dom
{
    static class ConstConfig
    {
        internal const int BOX_MAX_RIGHT = 90999;
        internal const int BOX_MAX_BOTTOM = 90999;
        internal const float TABLE_MAX_WIDTH = 9999f;

        /// <summary>
        /// serif
        /// </summary>
        internal static string DEFAULT_FONT_NAME = FontFamily.GenericSerif.Name;
        /// <summary>
        /// Default font size in points. Change this value to modify the default font size.
        /// </summary>
        public const float DEFAULT_FONT_SIZE = 11f;

    }
}
