using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using LayoutFarm.Ease;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;


using VroomJs;

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
            JsBridge.LoadV8("..\\..\\dll\\VRoomJsNative.dll");
            FormTestV8 formTestV8 = new FormTestV8();
            formTestV8.Show();
        }
        private void button5_Click(object sender, EventArgs e)
        {

            string filename = @"..\..\..\HtmlRenderer.Demo\Samples\ClassicSamples\00.Intro.htm";

            //1. blank html
            var fileContent = "<html><body><div id=\"a\">A</div><div id=\"b\" style=\"background-color:blue\">B</div></body></html>";
            easeViewport.LoadHtml(filename, fileContent);
            //----------------------------------------------------------------
            //after load html page 

            //test javascript ...
            JsBridge.LoadV8("..\\..\\dll\\VRoomJsNative.dll");
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
            //===============================================================

            //2. access dom  
            WebDocument webdoc = easeViewport.GetHtmlDom();
            var htmldoc = new LayoutFarm.WebDom.Wrap.HtmlDocument(webdoc);

            //create js engine and context
            var jstypeBuilder = new LayoutFarm.WebDom.Wrap.MyJsTypeDefinitionBuilder();
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext(jstypeBuilder))
            {
                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();

                ctx.SetVariableAutoWrap("document", htmldoc);

                string testsrc1 = "document.getElementById('a');";
                object domNodeA = ctx.Execute(testsrc1);

                string testsrc2 = "document.getElementById('b');";
                object domNodeB = ctx.Execute(testsrc2);

                stwatch.Stop();

                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());

            }

            ////3. get element by id 
            //var domNodeA = webdoc.GetElementById("a");
            //var domNodeB = webdoc.GetElementById("b");

            //domNodeA.AddTextContent("Hello from A");
            //domNodeB.AddChild("div", div =>
            //{
            //    div.SetAttribute("style", "background-color:yellow");
            //    div.AddTextContent("Hello from B");
            //});

            //domNodeB.AttachMouseDownEvent(ev =>
            //{
            //    var domB = new EaseDomElement(domNodeB);
            //    domB.SetBackgroundColor(Color.Red);
            //    ev.StopPropagation();
            //    //domNodeB.SetAttribute("style", "background-color:red");
            //});
            //domNodeB.AttachMouseUpEvent(ev =>
            //{
            //    var domB = new EaseDomElement(domNodeB);
            //    domB.SetBackgroundColor(Color.Yellow);
            //    ev.StopPropagation();
            //    //domNodeB.SetAttribute("style", "background-color:red");
            //}); 
        }

    }
}
