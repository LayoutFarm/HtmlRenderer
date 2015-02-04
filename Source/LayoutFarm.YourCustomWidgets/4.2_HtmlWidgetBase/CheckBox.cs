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


    public class CheckBox : Panel
    {

        bool isChecked;

        RenderElement myRenderE;
        DomElement imageBox;
        HtmlIslandHost htmlIslandHost;
        LightHtmlBox lightHtmlBox;
        public CheckBox(HtmlIslandHost htmlIslandHost, int w, int h)
            : base(w, h)
        {
            this.htmlIslandHost = htmlIslandHost;

        }
        protected override RenderElement CurrentPrimaryRenderElement
        {
            get
            {
                return this.myRenderE;
            }
        }
        protected override bool HasReadyRenderElement
        {
            get
            {
                return this.myRenderE != null;
            }
        }

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {

            if (!this.HasReadyRenderElement)
            {
                lightHtmlBox = new LightHtmlBox(htmlIslandHost, this.Width, this.Height);
                lightHtmlBox.LoadHtmlDom(CreateHtml());
                lightHtmlBox.SetLocation(this.Left, this.Top);
                myRenderE = lightHtmlBox.GetPrimaryRenderElement(rootgfx);
                return myRenderE;

            }
            else
            {
                return myRenderE;
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
        public event EventHandler WhenChecked;

        FragmentHtmlDocument CreateHtml()
        {
            FragmentHtmlDocument htmldoc = this.htmlIslandHost.CreateNewFragmentHtml();

            //TODO: use template engine, 
            //ideas:  AngularJS style ?

            //1. create body node             
            // and content   
            //style 2, lambda and adhoc attach event


            var domElement =
                htmldoc.RootNode.AddChild("div", div =>
                {
                    div.SetAttribute("style", "font:10pt tahoma;");
                    div.AddChild("img", img =>
                    {
                        this.imageBox = img;
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
                    div.AddChild("img", img =>
                    {
                        this.imageBox = img;

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

                });

            return htmldoc;
        }
    }


}