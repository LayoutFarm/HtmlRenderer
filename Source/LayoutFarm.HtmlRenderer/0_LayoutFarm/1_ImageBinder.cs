//BSD, 2014-2017, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm
{
    public abstract class ImageBinder
    {
        Image _image;
        string _imageSource;
        LazyLoadImageFunc lazyLoadImgFunc;
#if DEBUG
        static int dbugTotalId;
        public int dbugId = dbugTotalId++;
#endif

        public ImageBinder()
        {
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
            set;
        }
        public Image Image
        {
            get { return this._image; }
        }

        public int ImageWidth
        {
            get
            {
                if (this._image != null)
                {
                    return this._image.Width;
                }
                else
                {
                    return 16;
                }
            }
        }
        public int ImageHeight
        {
            get
            {
                if (this._image != null)
                {
                    return this._image.Height;
                }
                else
                {
                    return 16;
                }
            }
        }

        public void SetImage(Image image)
        {
            if (image != null)
            {
                this._image = image;
                this.State = ImageBinderState.Loaded;
                this.OnImageChanged();
            }
        }
        protected virtual void OnImageChanged()
        {
        }
        public bool HasLazyFunc
        {
            get { return this.lazyLoadImgFunc != null; }
        }

        public void SetLazyFunc(LazyLoadImageFunc lazyLoadFunc)
        {
            this.lazyLoadImgFunc = lazyLoadFunc;
        }
        public void LazyLoadImage()
        {
            if (this.lazyLoadImgFunc != null)
            {
                this.lazyLoadImgFunc(this);
            }
        }
        public static readonly ImageBinder NoImage = new NoImageImageBinder();
        class NoImageImageBinder : ImageBinder
        {
            public NoImageImageBinder()
            {
                this.State = ImageBinderState.NoImage;
            }
        }
    }


    public delegate void LazyLoadImageFunc(ImageBinder binder);
    public enum ImageBinderState
    {
        Unload,
        Loaded,
        Loading,
        Error,
        NoImage
    }
}