//BSD 2014,WinterDev

using System;
using System.Collections.Generic;
using System.Drawing;
using HtmlRenderer.WebDom;
namespace HtmlRenderer.Dom
{
    public class HtmlImageRequestEventArgs : EventArgs
    {
        public HtmlImageRequestEventArgs(IHtmlElement element, ImageBinder binder)
        {
            this.Element = element;
            this.ImageBinder = binder;
        }
        public IHtmlElement Element { get; private set; }
        public ImageBinder ImageBinder { get; private set; }
        public string ImagSource
        {
            get { return this.ImageBinder.ImageSource; }
        }
        public void SetResultImage(Image img)
        {
            this.ImageBinder.SetImage(img);
        }
    }

}