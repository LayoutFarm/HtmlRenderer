using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using LayoutFarm.Ease;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;

using NativeV8;

namespace Test5_Ease
{
    public partial class Form1 : Form
    {
        EaseViewport easeViewport;
        public Form1()
        {
            InitializeComponent();

            //1. create viewport
            easeViewport = EaseHost.CreateViewportControl(this, 800, 600);
            //2. add
            this.panel1.Controls.Add(easeViewport.ViewportControl);


            //this.Controls.Add(easeViewport.ViewportControl);
            this.Load += new EventHandler(Form1_Load);
        }
        void Form1_Load(object sender, EventArgs e)
        {
            //load sample html text
            easeViewport.Ready();
            string filename = @"..\..\..\HtmlRenderer.Demo\Samples\ClassicSamples\00.Intro.htm";
            //read text file
            var fileContent = System.IO.File.ReadAllText(filename);
            easeViewport.LoadHtml(filename, fileContent);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //test load html text 
            string filename = @"..\..\..\HtmlRenderer.Demo\Samples\ClassicSamples\00.Intro.htm";
            //read html text file
            var fileContent = "<html><body><div>Hello !</div></body></html>";
            easeViewport.LoadHtml(filename, fileContent);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string filename = @"..\..\..\HtmlRenderer.Demo\Samples\ClassicSamples\00.Intro.htm";

            //1. blank html
            var fileContent = "<html><body><div id=\"a\">AAA</div></body></html>";
            easeViewport.LoadHtml(filename, fileContent);
            //2. access dom 

            WebDocument webdoc = easeViewport.GetHtmlDom();
            var div_a = webdoc.GetElementById("a");
            div_a.AddChild("div", div =>
                {
                    div.AddChild("div", div_1 =>
                    {
                        div_1.SetAttribute("style", "font:10pt tahoma");
                        div_1.AddChild("span", span =>
                        {
                            span.AddTextContent("test1");
                        });
                        div_1.AddChild("span", span =>
                        {
                            span.AddTextContent("test2");
                        });
                    });
                });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string filename = @"..\..\..\HtmlRenderer.Demo\Samples\ClassicSamples\00.Intro.htm";

            //1. blank html
            var fileContent = "<html><body><div id=\"a\">A</div><div id=\"b\" style=\"background-color:blue\">B</div></body></html>";
            easeViewport.LoadHtml(filename, fileContent);


            //2. access dom  
            WebDocument webdoc = easeViewport.GetHtmlDom();
            //3. get element by id 
            var domNodeA = webdoc.GetElementById("a");
            var domNodeB = webdoc.GetElementById("b");


            domNodeA.AddTextContent("Hello from A");
            domNodeB.AddChild("div", div =>
            {
                div.SetAttribute("style", "background-color:yellow");
                div.AddTextContent("Hello from B");
            });

            domNodeB.AttachMouseDownEvent(ev =>
            {
                var domB = new EaseDomElement(domNodeB);
                domB.SetBackgroundColor(Color.Red);
                ev.StopPropagation();
                //domNodeB.SetAttribute("style", "background-color:red");
            });
            domNodeB.AttachMouseUpEvent(ev =>
            {
                var domB = new EaseDomElement(domNodeB);
                domB.SetBackgroundColor(Color.Yellow);
                ev.StopPropagation();
                //domNodeB.SetAttribute("style", "background-color:red");
            });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Drawing.Printing.PrintDocument printdoc = new System.Drawing.Printing.PrintDocument();

            printdoc.PrintPage += (e2, s2) =>
            {
                var g = s2.Graphics;
                var easeCanvas = EaseHost.CreatePrintCanvas(g, 800, 600);
                this.easeViewport.Print(easeCanvas);
            };
            printdoc.Print();
        }

        private void cmdTestV8Js1_Click(object sender, EventArgs e)
        {
            FormTestV8 formTestV8 = new FormTestV8();
            formTestV8.Show();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            string filename = @"..\..\..\HtmlRenderer.Demo\Samples\ClassicSamples\00.Intro.htm";

            //1. blank html
            var fileContent = "<html><body><div id=\"a\">AAA</div></body></html>";
            easeViewport.LoadHtml(filename, fileContent);

            WebDocument webdoc = easeViewport.GetHtmlDom();


            //NativeV8JsInterOp.LoadV8("..\\..\\dll\\MiniJsBridge.dll");
            NativeV8JsInterOp.LoadV8(@"D:\projects\V8Engine\MiniJsBridge\VS2010\Debug\VS2010\MiniJsBridge.dll");
             
            //===============================================================  
            JsContext context = NativeV8JsInterOp.CreateNewJsContext();
            HtmlDocumentWrapper htmldocWrapper = new HtmlDocumentWrapper(webdoc);//pure managed 
            context.EnterContext();

            //prepare wrapper 
            //style1: ...
            JsTypeDefinition jstypedef = new JsTypeDefinition("HtmlDocument");
            JsTypeDefinition htmlElementTypeDef = new JsTypeDefinition("HtmlElement");

            jstypedef.AddMember(new JsMethodDefinition("getElementById", args =>
            {
                //implement with lambda 
                //plan: move inside wrapper object
                var found = webdoc.GetElementById(args.GetArgAsString(0));
                if (found != null)
                {
                    //found  
                    //DomElementWrapper foundWrapper = new DomElementWrapper(found);
                    //NativeJsInstanceProxy elem_prox = context.CreateWrapper(foundWrapper, jstypedef);
                    //args.SetResultAsNativeObject(elem_prox);
                    args.SetResult(0);

                }
                else
                {

                }
            }));

            //--------------------------------------------------------------------------------------
            context.RegisterTypeDefinition(jstypedef);
            context.RegisterTypeDefinition(htmlElementTypeDef);
            //-------------------------------------------------------------
            //context.SetParameter("a", 10);
            //context.SetParameter("b", 20);
            //------------------------------------------------------------- 
            NativeJsInstanceProxy prox_doc = context.CreateWrapper(htmldocWrapper, jstypedef);
            context.SetParameter("document", prox_doc);
            //  run code 
            //context.Run("a+b");
            //context.Run("(function(){return a+b;})()");
            context.Run("if(document.getElementById(\"a\")!= null)return 1; else return 0;");
            context.ExitContext();
            context.Close();
            context = null;
        }

        struct HtmlDocumentWrapper
        {
            WebDocument webdoc;
            public HtmlDocumentWrapper(WebDocument webdoc)
            {
                this.webdoc = webdoc;
            }
            public void GetElementById(string id)
            {

            }
        }
        struct DomElementWrapper
        {
            public DomElement domElement;
            public DomElementWrapper(DomElement domElement)
            {
                this.domElement = domElement;
            }
        }

    }
}
