//Apache2, 2014-present, WinterDev

using System;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
namespace LayoutFarm.HtmlWidgets
{
    public class ChoiceBox : HtmlWidgetBase
    {
        string buttonText = "";
        DomElement pnode;
        bool _checked;
        //
        public event EventHandler<EventArgs> CheckValueAssigned;

        public ChoiceBox(int w, int h)
            : base(w, h)
        {
        }
        //---------------------------------------------------------------------------
        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                if (CheckValueAssigned != null)
                {
                    CheckValueAssigned(this, EventArgs.Empty);
                }
            }
        }
        public string Text
        {
            get { return this.buttonText; }
            set
            {
                this.buttonText = value;
            }
        }

        public bool OnlyOne
        {
            get;
            set;
        }
        public override DomElement GetPresentationDomNode(WebDom.Impl.HtmlDocument htmldoc)
        {
            if (pnode != null) return pnode;
            //----------------------------------
            pnode = htmldoc.CreateElement("div");
            pnode.SetAttribute("style", "display:inline-block;width:" + Width + "px;height:" + this.Height + "px;cursor:pointer");
            pnode.AddChild("div", div2 =>
            {
                //init
                div2.SetAttribute("style", "background-color:#dddddd;color:black;");
                DomElement imgNode = div2.AddChild("img");
                //imgNode.SetAttribute("src", "chk_unchecked.png");
                imgNode.SetAttribute("src", "chk_unchecked.png");

                imgNode.AttachMouseDownEvent(e =>
                {
                    imgNode.SetAttribute("src", "chk_checked.png");
                    //imgNode.SetAttribute("style", "background-color:yellow");
                    e.StopPropagation();
                });

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
                    //div2.SetAttribute("style", "padding:5px;background-color:yellow;");
                    imgNode.SetAttribute("src", "chk_checked.png");
                    imgNode.SetAttribute("src", "chk_unchecked.png");
                    e.StopPropagation();
                });
                div2.AttachMouseUpEvent(e =>
                {
#if DEBUG
                    //                    div2.dbugMark = 2;
#endif
                    imgNode.SetAttribute("src", "chk_unchecked.png");
                    //div2.SetAttribute("style", "padding:5px;background-color:#dddddd;");
                    //                    //EaseScriptElement ee = new EaseScriptElement(div2);
                    //                    //ee.ChangeBackgroundColor(Color.FromArgb(0xdd, 0xdd, 0xdd));
                    e.StopPropagation();
                });
            });
            return pnode;
        }
    }


}