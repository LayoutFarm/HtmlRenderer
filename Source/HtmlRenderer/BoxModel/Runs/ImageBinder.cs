//BSD 2014,WinterCore

using System;
using System.Drawing;

namespace HtmlRenderer.Dom
{
    public delegate void ReadyStateChangedHandler();


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
        public void LoadImageIfFirstTime(CssBox requestBox, ReadyStateChangedHandler handler)
        {
            //1. support sync and async model 
            switch (this.State)
            {
                case ImageBinderState.NoImage:
                    {

                    } break;
                case ImageBinderState.Unload:
                    {
                        //
                    } break;
                case ImageBinderState.Loading:
                    {
                        //...

                    } break;
                case ImageBinderState.Loaded:
                    {
                        //load finish 
                    } break;
            }
        }
        public ImageBinderState State
        {
            get;
            private set;
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