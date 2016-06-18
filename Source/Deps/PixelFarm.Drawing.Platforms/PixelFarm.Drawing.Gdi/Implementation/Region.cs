// 2015,2014 ,BSD, WinterDev 


namespace PixelFarm.Drawing.WinGdi
{
    class MyRegion : Region
    {
        System.Drawing.Region rgn = new System.Drawing.Region();
        public override object InnerRegion
        {
            get { return this.rgn; }
        }
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