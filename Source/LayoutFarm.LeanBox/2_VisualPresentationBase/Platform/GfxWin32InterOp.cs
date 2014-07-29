using System;
using System.Drawing; 
namespace LayoutFarm.Presentation
{

    public static class GraphicWin32InterOp
    {
        public static int ColorToWin32(Color c)
        {
            return ((c.R | (c.G << 8)) | (c.B << 0x10));

        }
    }
}