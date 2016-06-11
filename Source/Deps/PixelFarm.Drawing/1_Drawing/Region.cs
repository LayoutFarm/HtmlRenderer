// 2015,2014, MIT, WinterDev

namespace PixelFarm.Drawing
{
    public abstract class Region : System.IDisposable
    {
        public abstract void Dispose();
        public abstract object InnerRegion { get; }
    }
}