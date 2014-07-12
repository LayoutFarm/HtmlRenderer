//BSD 2014,WinterDev

using System;
using System.Collections.Generic;
using System.Drawing;
using HtmlRenderer.WebDom;
namespace HtmlRenderer.Boxes
{
    public class HtmlImageRequestEventArgs : EventArgs
    {
        public HtmlImageRequestEventArgs(ImageBinder binder)
        { 
            this.ImageBinder = binder;
        }
         
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