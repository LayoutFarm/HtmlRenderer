// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.Composers;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
using LayoutFarm.UI;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.CustomWidgets;

namespace LayoutFarm.HtmlWidgets
{

    public class CheckBox : HtmlWidgetBase
    {
        DomElement pnode;
        bool isChecked;
        string checkBoxText = "";
        public event EventHandler WhenChecked;
        public CheckBox(int w, int h)
            : base(w, h)
        {
        }
        //---------------------------------------------------------------------------
        public string Text
        {
            get { return this.checkBoxText; }
            set
            {
                this.checkBoxText = value;
            }
        }
        public bool Checked
        {
            get { return this.isChecked; }
            set
            {
                if (value != this.isChecked)
                {
                    this.isChecked = value;
                    //check check image too!
                    ImageBinder imgBinder = null;
                    if (this.isChecked)
                    {
                        imgBinder = ResImageList.GetImageBinder(ImageName.CheckBoxChecked);
                    }
                    else
                    {
                        imgBinder = ResImageList.GetImageBinder(ImageName.CheckBoxUnChecked);
                    }
                    //----------


                    //----------
                    if (value && this.WhenChecked != null)
                    {
                        this.WhenChecked(this, EventArgs.Empty);
                    }
                }
            }
        }

        public override DomElement GetPresentationDomNode(WebDom.Impl.HtmlDocument htmldoc)
        {
            //TODO: use template engine, 
            //ideas:  AngularJS style ?

            //1. create body node             
            // and content   
            //style 2, lambda and adhoc attach event  
            if (pnode != null) return pnode;
            //---------------------------------------------------
            pnode = htmldoc.CreateElement("div");
            pnode.AddChild("img", img =>
            {
                //init 
                bool is_close = true;
                img.SetAttribute("src", "../../Demo/arrow_close.png");
                img.AttachMouseDownEvent(e =>
                {
                    img.SetAttribute("src", is_close ?
                        "../../Demo/arrow_open.png" :
                        "../../Demo/arrow_close.png");
                    is_close = !is_close;
                    e.StopPropagation();

                    this.InvalidateGraphics();
                });
            });
            pnode.AddChild("img", img =>
            {

                //change style
                bool is_close = true;
                img.SetAttribute("src", "../../Demo/arrow_close.png");
                //3. attach event to specific span 
                img.AttachMouseDownEvent(e =>
                {
                    img.SetAttribute("src", is_close ?
                        "../../Demo/arrow_open.png" :
                        "../../Demo/arrow_close.png");

                    is_close = !is_close;
                    e.StopPropagation();

                    this.InvalidateGraphics();
                });

            });
            pnode.AddChild("span", span =>
            {
                span.AddTextContent(this.checkBoxText);
            });

            return pnode;
        }

    }
}