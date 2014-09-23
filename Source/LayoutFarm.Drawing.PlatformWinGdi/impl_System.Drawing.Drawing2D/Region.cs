using System.Drawing;

namespace LayoutFarm.Drawing.WinGdiPlatform
{

    class MyRegion : Region
    {
        System.Drawing.Region rgn = new System.Drawing.Region();
        public override void Dispose()
        {
            if (rgn != null)
            {
                rgn.Dispose();
                rgn = null;
            }
        }
    }
}