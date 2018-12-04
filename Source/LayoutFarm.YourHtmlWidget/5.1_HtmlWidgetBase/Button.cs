//Apache2, 2014-present, WinterDev


using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
namespace LayoutFarm.HtmlWidgets
{
    public class Button : HtmlWidgetBase
    {
        string _buttonText = "";
        DomElement _pnode;
        public Button(int w, int h)
            : base(w, h)
        {
        }
        //---------------------------------------------------------------------------
        public string Text
        {
            get => _buttonText;
            set => _buttonText = value;
        }
        public override DomElement GetPresentationDomNode(WebDom.Impl.HtmlDocument htmldoc)
        {
            if (_pnode != null) return _pnode;
            //----------------------------------
            _pnode = htmldoc.CreateElement("div");
            _pnode.SetAttribute("style", "display:inline-block;width:" + Width + "px;height:" + this.Height + "px;cursor:pointer");
            _pnode.AddChild("div", div2 =>
            {
                //init
                div2.SetAttribute("style", "padding:5px;background-color:#dddddd;color:black;");
                DomElement imgNode = div2.AddChild("img");
                imgNode.SetAttribute("src", "chk_unchecked.png");

                //div2.AddChild("span", span =>
                //{
                //    span.AddTextContent(this.buttonText);
                //});
                //------------------------------
#if DEBUG
                div2.dbugMark = 10;
#endif
                div2.AttachMouseDownEvent(e =>
                {
#if DEBUG
                    //                    div2.dbugMark = 1;
#endif
                    // div2.SetAttribute("style", "padding:5px;background-color:#aaaaaa;");
                    //EaseScriptElement ee = new EaseScriptElement(div2);
                    //ee.ChangeBackgroundColor(Color.FromArgb(0xaa, 0xaa, 0xaa));
                    div2.SetAttribute("style", "padding:5px;background-color:yellow;");
                    e.StopPropagation();
                });
                div2.AttachMouseUpEvent(e =>
                {
#if DEBUG
                    //                    div2.dbugMark = 2;
#endif
                    div2.SetAttribute("style", "padding:5px;background-color:#dddddd;");
                    //                    //EaseScriptElement ee = new EaseScriptElement(div2);
                    //                    //ee.ChangeBackgroundColor(Color.FromArgb(0xdd, 0xdd, 0xdd));
                    e.StopPropagation();
                });
            });
            return _pnode;
        }
    }
}