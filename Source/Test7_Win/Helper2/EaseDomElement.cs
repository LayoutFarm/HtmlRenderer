//MIT, 2014-2017, WinterDev

using LayoutFarm.Composers;
namespace LayoutFarm.Ease
{
    public struct EaseDomElement
    {
        EaseScriptElement easeScriptElement;
        public EaseDomElement(WebDom.DomElement domElement)
        {
            this.easeScriptElement = new EaseScriptElement(domElement);
        }
        //public void SetBackgroundColor(System.Drawing.Color c)
        //{
        //    this.easeScriptElement.ChangeBackgroundColor(new Color(c.A, c.R, c.G, c.B));
        //}
    }
}