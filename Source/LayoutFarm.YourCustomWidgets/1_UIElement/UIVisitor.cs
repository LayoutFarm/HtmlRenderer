//Apache2, 2014-2017, WinterDev

namespace LayoutFarm.UI
{
    public abstract class UIVisitor
    {
        public abstract void BeginElement(UIElement ui, string uiname);
        public abstract void Attribute(string name, string value);
        public abstract void Attribute(string name, int value);
        public abstract void Attribute(string name, double value);
        public abstract void TextNode(string content);
        public abstract void Comment(string content);
        public abstract void EndElement();
    }
}
