//MIT 2014,WinterDev
//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//----------------------------------- 
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using PixelFarm.Agg;

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