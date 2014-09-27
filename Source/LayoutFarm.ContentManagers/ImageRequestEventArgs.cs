//BSD 2014,WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;
namespace HtmlRenderer.ContentManagers
{
    public class ImageRequestEventArgs : EventArgs
    {
        public ImageRequestEventArgs(ImageBinder binder)
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