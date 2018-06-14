//BSD, 2014-present, WinterDev 
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