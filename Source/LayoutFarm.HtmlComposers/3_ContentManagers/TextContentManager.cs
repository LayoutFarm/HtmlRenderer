
//BSD 2014, WinterDev 
//ArthurHub

using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;
using LayoutFarm.Drawing;
using HtmlRenderer.Boxes;
using System.Threading;
using HtmlRenderer.WebDom;

namespace HtmlRenderer.ContentManagers
{

    public class TextContentManager
    {
        RootVisualBox parentHtmlContainer;

        /// <summary>
        /// Raised when a stylesheet is about to be loaded by file path or URI by link element.<br/>
        /// This event allows to provide the stylesheet manually or provide new source (file or Uri) to load from.<br/>
        /// If no alternative data is provided the original source will be used.<br/>
        /// </summary>
        public event EventHandler<StylesheetLoadEventArgs> StylesheetLoadingRequest;

        public TextContentManager(RootVisualBox parentHtmlContainer)
        {
            this.parentHtmlContainer = parentHtmlContainer;
        }
        public void AddStyleSheetRequest(StylesheetLoadEventArgs arg)
        {
            this.StylesheetLoadingRequest(this, arg);             
        }
    }

}