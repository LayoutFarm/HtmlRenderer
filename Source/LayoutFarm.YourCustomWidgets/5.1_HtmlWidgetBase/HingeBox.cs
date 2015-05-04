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
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.HtmlWidgets
{

    public class HingeBox : HtmlWidgetBase
    {
        DomElement floatPartDomElement;


        DomElement presentationNode;
        Color backColor = Color.LightGray;
        bool isOpen;
        ////1. land part
        //HtmlBox landPart;
        ////2. float part   
        //HtmlBox floatPart;

        //RenderElement landPartRenderElement;
        //RenderElement floatPartRenderElement;

        HingeFloatPartStyle floatPartStyle;

        public HingeBox(int w, int h)
            : base(w, h)
        {
        }
        DomElement CreateFloatPartDom(WebDom.Impl.HtmlDocument htmldoc)
        {
            //create land part 
            var div = htmldoc.CreateElement("div");
            div.SetAttribute("style", "position:absolute;left:0px;top:0px;width:300px;height:500px;");

            for (int i = 0; i < 10; ++i)
            {
                div.AddChild("div", div2 =>
                {
                    div2.AddTextContent("HELLO!" + i);
                });
            }
            return div;
        }
        public override DomElement GetPresentationDomNode(WebDom.Impl.HtmlDocument htmldoc)
        {
            if (presentationNode != null)
            {
                return presentationNode;
            }
            //-------------------
            presentationNode = htmldoc.CreateElement("div");
            presentationNode.AddChild("div", div =>
            {
                div.SetAttribute("style", "font:10pt tahoma;");
                div.AddChild("img", img =>
                {
                    //init 
                    img.SetAttribute("src", "../../Demo/arrow_close.png");
                    img.AttachMouseDownEvent(e =>
                    {
                        //img.SetAttribute("src", this.IsOpen ?
                        //    "../../Demo/arrow_open.png" :
                        //    "../../Demo/arrow_close.png");
                        ////------------------------------

                        if (this.IsOpen)
                        {
                            img.SetAttribute("src", "../../Demo/arrow_close.png");
                            this.CloseHinge();
                        }
                        else
                        {
                            img.SetAttribute("src", "../../Demo/arrow_open.png");
                            this.OpenHinge();
                        }

                        this.InvalidateGraphics();
                        //----------------------------- 
                        e.StopPropagation();
                    });

                });
            });
            //-------------------

            this.floatPartDomElement = this.CreateFloatPartDom(htmldoc);

            return presentationNode;
        }

        //public HtmlBox LandPart
        //{
        //    get { return this.landPart; }
        //}
        //public HtmlBox FloatPart
        //{
        //    get { return this.floatPart; }
        //}
        //protected override void OnPrimaryUIElementCreated(HtmlDocument htmldoc, HtmlHost htmlhost)
        //{

        //    if (this.floatPartDomElement == null)
        //    {
        //        this.floatPartDomElement = CreateFloatPartDom(htmldoc);
        //    }

        //    //if (this.landPartDomElement == null)
        //    //{
        //    //    this.landPartDomElement = this.GetPresentationDomNode(
        //    //}
        //    //if (this.landPart == null)
        //    //{
        //    //    this.landPart = (HtmlBox)base.GetPrimaryUIElement(htmlhost);
        //    //}
        //    //if (floatPart == null)
        //    //{
        //    //    this.floatPart = new HtmlBox(htmlhost, this.Width, 300);
        //    //    this.floatPart.Visible = false;
        //    //    this.floatPart.LoadHtmlDom(CreateFloatPartDom());
        //    //}
        //}

        //---------------------------------------------------- 
        public bool IsOpen
        {
            get { return this.isOpen; }
        }
        //---------------------------------------------------- 


        public void OpenHinge()
        {
            if (isOpen) return;
            this.isOpen = true;

            //if (this.landPartRenderElement == null)
            //{
            //    landPartRenderElement = this.landPart.CurrentPrimaryRenderElement;
            //}

            //if (floatPart == null) return;


            switch (floatPartStyle)
            {
                default:
                case HingeFloatPartStyle.Popup:
                    {
                        var htmldoc = this.presentationNode.OwnerDocument as HtmlDocument;
                        htmldoc.BodyElement.AddChild(this.floatPartDomElement);

                        //add float part to top window layer***
                        //var topRenderBox = landPartRenderElement.GetTopWindowRenderBox();
                        //if (topRenderBox != null)
                        //{
                        //    Point globalLocation = landPartRenderElement.GetGlobalLocation();
                        //    floatPart.SetLocation(globalLocation.X, globalLocation.Y + landPartRenderElement.Height);

                        //    this.floatPartRenderElement = this.floatPart.GetPrimaryRenderElement(landPartRenderElement.Root);
                        //    topRenderBox.AddChild(floatPartRenderElement);
                        //}

                    } break;
                case HingeFloatPartStyle.Embeded:
                    {

                    } break;
            }
        }
        public void CloseHinge()
        {
            if (!isOpen) return;
            this.isOpen = false;


            if (floatPartDomElement == null)
            {
                return;
            }
            //if (this.landPartRenderElement == null)
            //{
            //    landPartRenderElement = this.landPart.CurrentPrimaryRenderElement;
            //}
            //if (this.landPartRenderElement == null) return;
            //if (floatPart == null) return;



            switch (floatPartStyle)
            {
                default:
                    {
                    } break;
                case HingeFloatPartStyle.Popup:
                    {
                        //remove float part from 
                        //if (floatPartRenderElement != null)
                        //{
                        //    //temp
                        //    var topRenderBox = floatPartRenderElement.GetTopWindowRenderBox();
                        //    topRenderBox.RemoveChild(floatPartRenderElement);

                        //    //var parentContainer = floatPartRenderElement.ParentRenderElement as TopWindowRenderBox;
                        //    //parentContainer.RemoveChild(floatPartRenderElement);
                        //    //if (parentContainer.Layers != null)
                        //    //{
                        //    //    PlainLayer plainLayer = (PlainLayer)parentContainer.Layers.GetLayer(0);
                        //    //    plainLayer.RemoveChild(floatPartRenderElement);

                        //    //}
                        //}

                    } break;
                case HingeFloatPartStyle.Embeded:
                    {
                    } break;

            }
        }

        public HingeFloatPartStyle FloatPartStyle
        {
            get { return this.floatPartStyle; }
            set
            {
                this.floatPartStyle = value;
            }
        }
    }



}
