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
    public class FontGlyph
    {
        public ExportGlyph exportGlyph;
        /// <summary>
        /// original 8bpp image buffer
        /// </summary>
        public byte[] glyImgBuffer8;
        /// <summary>
        /// 32 bpp image for render
        /// </summary>
        public ActualImage glyphImage32;
        //----------------------------
        /// <summary>
        /// original glyph outline
        /// </summary>
        public VertexStore originalVxs;
        /// <summary>
        /// flaten version of original glyph outline
        /// </summary>
        public VertexStore flattenVxs;
        //----------------------------
        //metrics
        public int horiz_adv_x;
        public string glyphName;
        public int unicode;
    }
}