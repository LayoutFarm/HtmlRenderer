using System.Drawing;
namespace LayoutFarm.Drawing
{
    public abstract class Font : System.IDisposable
    {
        public abstract string Name { get; }
        public abstract float Size { get; }
        public abstract FontStyle Style { get; }
        public abstract object InnerFont { get; }
        public abstract void Dispose();
    }

    public abstract class FontFamily
    {
       
        public abstract string Name { get; }
    }
}