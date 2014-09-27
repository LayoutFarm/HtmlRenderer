
//BSD 2014, WinterDev 
//ArthurHub

using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;

namespace HtmlRenderer.ContentManagers
{
    public class StylesheetLoadEventArgs2 : EventArgs
    {
        public string Src { get; set; }
        public string SetStyleSheet { get; set; }
    }
    public class TextContentManager
    {
        /// <summary>
        /// Raised when a stylesheet is about to be loaded by file path or URI by link element.<br/>
        /// This event allows to provide the stylesheet manually or provide new source (file or Uri) to load from.<br/>
        /// If no alternative data is provided the original source will be used.<br/>
        /// </summary>
        public event EventHandler<StylesheetLoadEventArgs2> StylesheetLoadingRequest;
        public TextContentManager()
        {

        }
        public void AddStyleSheetRequest(StylesheetLoadEventArgs2 arg)
        {
            this.StylesheetLoadingRequest(this, arg);
        }
    }

}