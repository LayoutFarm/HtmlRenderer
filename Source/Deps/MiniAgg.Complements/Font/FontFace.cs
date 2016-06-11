// 2015,2014 ,MIT, WinterDev
//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//----------------------------------- 

using System;
namespace PixelFarm.Agg.Fonts
{
    public abstract class FontFace : IDisposable
    {
        public bool HasKerning { get; set; }
        protected abstract void OnDispose();
        public void Dispose()
        {
            OnDispose();
        }
        ~FontFace()
        {
            OnDispose();
        }
    }
}