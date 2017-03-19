//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{
    //library specific 
    //for 
    public static class ResImageList
    {
        //temp ***

        static Dictionary<ImageName, Image> images;
        public static bool HasImages
        {
            get { return images != null; }
        }
        public static void SetImageList(Dictionary<ImageName, Image> images)
        {
            ResImageList.images = images;
        }
        public static Image GetImage(ImageName imageName)
        {
            Image found;
            images.TryGetValue(imageName, out found);
            return found;
        }
        public static ImageBinder GetImageBinder(ImageName imageName)
        {
            Image found;
            images.TryGetValue(imageName, out found);
            ImageBinder binder = new MyClientImageBinder(null);
            binder.SetImage(found);
            binder.State = ImageBinderState.Loaded;
            return binder;
        }
    }

    public enum ImageName
    {
        CheckBoxChecked,
        CheckBoxUnChecked,
        RadioBoxChecked,
        RadioBoxUnChecked
    }
}