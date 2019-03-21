//Apache2, 2014-present, WinterDev


using LayoutFarm.Composers;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
namespace LayoutFarm.HtmlWidgets
{
    public class Button : HtmlWidgetBase
    {
        string _buttonText = "";
        HtmlElement _pnode;
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
        public override HtmlElement GetPresentationDomNode(Composers.HtmlElement orgDomElem)
        {
            if (_pnode != null) return _pnode;
            //----------------------------------
            _pnode = orgDomElem.OwnerHtmlDoc.CreateHtmlDiv();
            _pnode.SetStyleAttribute("display:inline-block;width:" + Width + "px;height:" + this.Height + "px;cursor:pointer");
            _pnode.AddHtmlDivElement(div2 =>
            {
                //init                 
                div2.SetStyleAttribute("padding:5px;background-color:#dddddd;color:black;");
                //
                HtmlImageElement imgNode = div2.AddHtmlImageElement();
                imgNode.SetImageSource(WidgetResList.chk_unchecked);

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
                    div2.SetStyleAttribute("padding:5px;background-color:yellow;");
                    e.StopPropagation();
                });
                div2.AttachMouseUpEvent(e =>
                {
#if DEBUG
                    //                    div2.dbugMark = 2;
#endif

                    div2.SetStyleAttribute("padding:5px;background-color:#dddddd;");
                    //                    //EaseScriptElement ee = new EaseScriptElement(div2);
                    //                    //ee.ChangeBackgroundColor(Color.FromArgb(0xdd, 0xdd, 0xdd));
                    e.StopPropagation();
                });
            });

            return _pnode;
        }
    }
}