//BSD, 2014-2017, WinterDev 
//ArthurHub  , Jose Manuel Menendez Poo

using System;
namespace LayoutFarm.ContentManagers
{
    public delegate void RequestStyleSheetEventHandler(TextRequestEventArgs args);
    public class TextRequestEventArgs : EventArgs
    {
        public TextRequestEventArgs(string src)
        {
            this.Src = src;
        }
        public string Src { get; private set; }
        public string TextContent { get; set; }
    }

    public class TextContentManager
    {
        /// <summary>
        /// Raised when a stylesheet is about to be loaded by file path or URI by link element.<br/>
        /// This event allows to provide the stylesheet manually or provide new source (file or Uri) to load from.<br/>
        /// If no alternative data is provided the original source will be used.<br/>
        /// </summary>
        public event EventHandler<TextRequestEventArgs> StylesheetLoadingRequest;
        public TextContentManager()
        {
        }
        public void AddStyleSheetRequest(TextRequestEventArgs arg)
        {
            this.StylesheetLoadingRequest(this, arg);
        }
    }
}