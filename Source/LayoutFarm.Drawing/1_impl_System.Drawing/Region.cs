using System.Drawing;
namespace LayoutFarm.Drawing
{
    public abstract class Region : System.IDisposable
    {
        public abstract void Dispose();
        public abstract object InnerRegion { get; }
       
    }
}