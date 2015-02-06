
// 2015,2014 ,BSD, WinterDev 
//ArthurHub

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;

namespace LayoutFarm.ContentManagers
{
    public class TextLoadRequestEventArgs : EventArgs
    {
        public TextLoadRequestEventArgs(string src)
        {
            this.Src = src;
        }
        public string Src { get; private set; }
        public string SetStyleSheet { get; set; }
    }

    public class TextContentManager
    {
        /// <summary>
        /// Raised when a stylesheet is about to be loaded by file path or URI by link element.<br/>
        /// This event allows to provide the stylesheet manually or provide new source (file or Uri) to load from.<br/>
        /// If no alternative data is provided the original source will be used.<br/>
        /// </summary>
        public event EventHandler<TextLoadRequestEventArgs> StylesheetLoadingRequest;
        public TextContentManager()
        { 

        }
        public void AddStyleSheetRequest(TextLoadRequestEventArgs arg)
        {
            this.StylesheetLoadingRequest(this, arg);
        }
    }

}