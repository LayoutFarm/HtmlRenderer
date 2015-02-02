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


        HtmlIslandHost islandHost;
        LightHtmlBoxHost lightBoxHost;
        //check icon 
        bool isChecked;

        RenderElement myRenderE;
        ImageBox imageBox;
         
        public CheckBox(int w, int h)
            : base(w, h)
        {

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
                this.islandHost = new HtmlIslandHost();
                this.islandHost.BaseStylesheet = LayoutFarm.Composers.CssParserHelper.ParseStyleSheet(null, true);

                lightBoxHost = new LightHtmlBoxHost(islandHost, rootgfx.P);
                lightBoxHost.RootGfx = rootgfx;

                LightHtmlBox lightHtmlBox2 = lightBoxHost.CreateLightBox(this.Width, this.Height);
                lightHtmlBox2.LoadHtmlFragmentDom(CreateSampleHtmlDoc());
                lightHtmlBox2.SetLocation(this.Left, this.Top);
                myRenderE = lightHtmlBox2.GetPrimaryRenderElement(rootgfx);
                return myRenderE;
                //floatPart.AddChildBox(lightHtmlBox2);
                ////light box can't load full html
                ////all light boxs of the same lightbox host share resource with the host
                ////string html2 = @"<div>OK1</div><div>OK2</div><div>OK3</div><div>OK4</div>";
                ////if you want to use ful l html-> use HtmlBox instead  
                //lightHtmlBox2.LoadHtmlFragmentDom(CreateSampleHtmlDoc(floatPart));

                //var dom = CreateSampleHtmlDoc();
                //return null;
                //generate custom html checkbox 
                //RenderElement baseRenderElement = base.GetPrimaryRenderElement(rootgfx);
                //imageBox = new ImageBox(16, 16);
                //if (this.isChecked)
                //{
                //    imageBox.ImageBinder = ResImageList.GetImageBinder(ImageName.CheckBoxChecked);
                //}
                //else
                //{
                //    imageBox.ImageBinder = ResImageList.GetImageBinder(ImageName.CheckBoxUnChecked);
                //}

                //imageBox.MouseDown += (s, e) =>
                //{
                //    //toggle checked/unchecked
                //    this.Checked = !this.Checked;

                //};
                //this.AddChildBox(imageBox);
                //return baseRenderElement;
            }
            else
            {
                return myRenderE;
                //return base.GetPrimaryRenderElement(rootgfx);
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

                    if (this.isChecked)
                    {
                        imageBox.ImageBinder = ResImageList.GetImageBinder(ImageName.CheckBoxChecked);
                    }
                    else
                    {
                        imageBox.ImageBinder = ResImageList.GetImageBinder(ImageName.CheckBoxUnChecked);
                    }



                    if (value && this.WhenChecked != null)
                    {
                        this.WhenChecked(this, EventArgs.Empty);
                    }
                }
            }
        }
        public event EventHandler WhenChecked;
        HtmlDocument CreateSampleHtmlDoc()
        {

            HtmlDocument myHtmlDoc = new HtmlDocument();
            var myHtmlRootElement = myHtmlDoc.RootNode;
            var bodyNode = myHtmlRootElement.AddChild("body");

            //1. create body node             
            // and content   
            //style 2, lambda and adhoc attach event
            var dombox =
                bodyNode.AddChild("div", div =>
                {
                    var styleAttr = myHtmlDoc.CreateAttribute(WellknownName.Style);
                    styleAttr.Value = "font:10pt tahoma";
                    div.AddAttribute(styleAttr);

                    MenuBox menuBox = new MenuBox(200, 100);
                    div.AddChild("span", span =>
                    {
                        //test menubox 
                        span.AddTextContent("ABCD");
                        //3. attach event to specific span
                        span.AttachMouseDownEvent(e =>
                        {
#if DEBUG
                            // System.Diagnostics.Debugger.Break();                           
                            //test change span property 
                            //clear prev content and add new  text content                             
                            span.ClearAllElements();
                            span.AddTextContent("XYZ0001");
#endif

                            menuBox.SetLocation(50, 50);
                            //add to top window
                            menuBox.ShowMenu(this.CurrentPrimaryRenderElement.Root);

                            e.StopPropagation();

                        });
                    });

                    div.AddChild("span", span =>
                    {
                        span.AddTextContent("EFGHIJK");
                        span.AttachMouseDownEvent(e =>
                        {
                            span.ClearAllElements();
                            span.AddTextContent("LMNOP0003");

                            //test hide menu                             
                            menuBox.HideMenu();

                        });
                    });
                    //----------------------
                    div.AttachEvent(UIEventName.MouseDown, e =>
                    {
#if DEBUG
                        //this will not print 
                        //if e has been stop by its child
                        // System.Diagnostics.Debugger.Break();
                        Console.WriteLine("div");
#endif

                    });
                });

            return myHtmlDoc;
        }
    }


}