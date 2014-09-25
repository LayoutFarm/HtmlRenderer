//2014 Apache2, WinterDev
using System;
using LayoutFarm.Drawing;

namespace LayoutFarm
{

    static class GraphicWin32InterOp
    {
        public static int ColorToWin32(Color c)
        {
            return ((c.R | (c.G << 8)) | (c.B << 0x10));

        }
    }
}