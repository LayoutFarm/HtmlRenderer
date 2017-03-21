//BSD, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.ContentManagers
{
    class ImageCacheSystem
    {
        Dictionary<string, Image> cacheImages = new Dictionary<string, Image>();
        public ImageCacheSystem()
        {
        }
        public bool TryGetCacheImage(string url, out Image img)
        {
            return cacheImages.TryGetValue(url, out img);
        }
        public void AddCacheImage(string url, Image img)
        {
            this.cacheImages[url] = img;
        }
    }
}