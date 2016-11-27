//BSD, 2014-2016, WinterDev
namespace LayoutFarm.Css
{
    public class BoxSpecNode
    {
        BoxSpecNode parentNode;
        public BoxSpecNode ParentNode
        {
            get { return this.parentNode; }
            set { this.parentNode = value; }
        }
    }
}