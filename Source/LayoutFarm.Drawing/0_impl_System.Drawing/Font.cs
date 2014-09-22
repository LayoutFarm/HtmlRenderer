using System.Drawing;
namespace LayoutFarm.Drawing
{
    public class Font:System.IDisposable
    {

        System.Drawing.Font  myFont;
        public Font(object f)
        {
            this.myFont = (System.Drawing.Font)f;
        }
        public string Name
        {
            get { return this.myFont.Name; }
            
        }
        public float Size
        {
            get { return this.myFont.Size; }             
        }
        public FontStyle Style
        {
            get
            {
                throw new System.NotSupportedException();
            }
        }

        public void Dispose()
        {
            myFont.Dispose();
            myFont = null;
        }
        public object InnerFont
        {
            get { return this.myFont; }
        }
    }
    public class FontFamily
    {
        System.Drawing.FontFamily ff;
        internal FontFamily(System.Drawing.FontFamily ff)
        {
            this.ff = ff;
            
        }
        public string Name
        {
            get { return this.ff.Name; }
            
        }
        public static FontFamily GenericSerif = new FontFamily(System.Drawing.FontFamily.GenericSansSerif);
         
    } 
}