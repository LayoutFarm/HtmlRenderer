//BSD 2014,WinterDev

using System;
using System.Drawing;

namespace HtmlRenderer 
{
 

    public class ImageBinder
    {
        Image _image;
        string _imageSource;
        internal static readonly ImageBinder NoImage = new ImageBinder();

        private ImageBinder()
        {
            this.State = ImageBinderState.NoImage;
        }
        public ImageBinder(string imgSource)
        {
            this._imageSource = imgSource;
        }
        public string ImageSource
        {
            get { return this._imageSource; }
        }

        
        
        public ImageBinderState State
        {
            get;
            private set;
        }
        public Image Image
        {
            get { return this._image; }
        }
        public void SetImage(Image image)
        {
            if (image != null)
            {
                this._image = image;
                this.State = ImageBinderState.Loaded;
            }
        }

    }
    public enum ImageBinderState
    {
        Unload,
        Loaded,
        Loading,
        Error,
        NoImage
    }

}