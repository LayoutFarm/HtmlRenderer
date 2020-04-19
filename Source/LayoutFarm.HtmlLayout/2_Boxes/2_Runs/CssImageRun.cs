//ArthurHub, Jose Manuel Menendez Poo
// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

//MIT, 2018-present, WinterDev

using System.Text;
using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    /// <summary>
    /// Represents a word inside an inline box
    /// </summary>
    public sealed class CssImageRun : CssRun
    {
        ImageBinder _imgBinder;
        /// <summary>
        /// the image rectange restriction as returned from image load event
        /// </summary>
        Rectangle _imageRectangle;
        /// <summary>
        /// Creates a new BoxWord which represents an image
        /// </summary>
        /// <param name="owner">the CSS box owner of the word</param>
        public CssImageRun()
            : base(CssRunKind.SolidContent)
        {
        }
        /// <summary>
        /// Gets the image this words represents (if one exists)
        /// </summary>
        Image Image
        {
            get
            {
                if (_imgBinder != null)
                {
                    return _imgBinder.LocalImage;
                }
                return null;
            }
        }
        public int OriginalImageWidth
        {
            get
            {
                Image img = this.Image;
                if (img != null)
                {
                    return img.Width;
                }
                return 1; //default image width
            }
        }
        public int OriginalImageHeight
        {
            get
            {
                Image img = this.Image;
                if (img != null)
                {
                    return img.Height;
                }
                return 1; //default image width
            }
        }
        public bool HasUserImageContent => this.Image != null;
        public ImageBinder ImageBinder
        {
            get => _imgBinder;
            set => _imgBinder = value;

        }
        /// <summary>
        /// the image rectange restriction as returned from image load event
        /// </summary>
        public Rectangle ImageRectangle
        {
            get => _imageRectangle;
            set => _imageRectangle = value;
        }

        public override void WriteContent(System.Text.StringBuilder stbuilder, int start, int length)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("copy_image:...");
#endif
        }
        public override void WriteContent(StringBuilder stbuilder, int start = 0)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("copy_image:...");
#endif
        }

#if DEBUG
        /// <summary>
        /// Represents this word for debugging purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Image";
        }
#endif
    }
}
