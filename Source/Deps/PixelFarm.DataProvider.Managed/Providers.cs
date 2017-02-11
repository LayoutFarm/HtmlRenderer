//MIT, 2016-2017, WinterDev
using System;
using System.Collections.Generic;

namespace PixelFarm.Drawing
{
    public interface IImageProvider
    {
        byte[] LoadImageBufferFromFile(string filename);
    }
}
namespace PixelFarm.Drawing.Fonts
{
    public interface IInstalledFontProvider
    {
        IEnumerable<string> GetInstalledFontIter();
    }
}