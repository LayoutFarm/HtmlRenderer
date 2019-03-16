//Apache2, 2014-present, WinterDev

using System;
using LayoutFarm.Composers;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
namespace LayoutFarm.HtmlWidgets
{
    /// <summary>
    /// for option box ,or check box
    /// </summary>
    public class ChoiceBox : HtmlWidgetBase
    {
        string _buttonText = "";
        /// <summary>
        /// presentation node
        /// </summary>
        HtmlElement _pnode;
        DomElement _imgNode;
        bool _checked;
        LayoutFarm.Composers.HtmlInputElement _htmlInput;

        public event EventHandler<EventArgs> CheckValueAssigned;

        public ChoiceBox(int w, int h)
            : base(w, h)
        {
        }
        public void SetHtmlInputBox(LayoutFarm.Composers.HtmlInputElement htmlInput)
        {
            //link to html input
            _htmlInput = htmlInput;
        }
        //---------------------------------------------------------------------------
        public bool Checked
        {
            get => _checked;
            set
            {
                _checked = value;
                //
                CheckValueAssigned?.Invoke(this, EventArgs.Empty);
                //
                if (_imgNode != null)
                {
                    if (value)
                    {
                        _imgNode.SetAttribute("src", OnlyOne ? WidgetResList.opt_checked : WidgetResList.chk_checked);
                    }
                    else
                    {
                        _imgNode.SetAttribute("src", OnlyOne ? WidgetResList.opt_unchecked : WidgetResList.chk_unchecked);
                    }
                }
            }
        }
        public string Text
        {
            get => _buttonText;
            set => _buttonText = value;
        }

        public bool OnlyOne
        {
            get;
            set;
        }


        public override HtmlElement GetPresentationDomNode(HtmlDocument htmldoc)
        {
            if (_pnode != null) return _pnode;
            //----------------------------------
            _pnode = (HtmlElement)htmldoc.CreateElement("div");
            _pnode.SetAttribute("style", "display:inline-block;width:" + Width + "px;height:" + this.Height + "px;cursor:pointer");
            _pnode.AddChild("div", div2 =>
            {
                //init
                //div2.SetAttribute("style", "background-color:#dddddd;color:black;");
                div2.SetAttribute("style", "color:black;");
                _imgNode = div2.AddChild("img");

                _imgNode.SetAttribute("src", OnlyOne ? WidgetResList.opt_unchecked : WidgetResList.chk_unchecked);

                _imgNode.AttachMouseDownEvent(e =>
                {

                    Checked = !Checked; //toggle 
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
                    //imgNode.SetAttribute("src", "opt_checked.png");
                    //imgNode.SetAttribute("src", "chk_unchecked.png");
                    e.StopPropagation();
                });
                div2.AttachMouseUpEvent(e =>
                {
#if DEBUG
                    //                    div2.dbugMark = 2;
#endif
                    //imgNode.SetAttribute("src", "chk_unchecked.png");
                    //imgNode.SetAttribute("src", "opt_unchecked.png");
                    //div2.SetAttribute("style", "padding:5px;background-color:#dddddd;");
                    //                    //EaseScriptElement ee = new EaseScriptElement(div2);
                    //                    //ee.ChangeBackgroundColor(Color.FromArgb(0xdd, 0xdd, 0xdd));
                    e.StopPropagation();
                });
            });
            return _pnode;
        }
    }


}