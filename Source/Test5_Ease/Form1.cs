using System;
using System.Drawing;
using System.Windows.Forms;
using LayoutFarm.Ease;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
using Espresso;
namespace Test5_Ease
{
    public partial class Form1 : Form
    {
        EaseViewport easeViewport;
        MyWebConsole myWbConsole;
        JsEngine myengine;
        JsContext myCtx;
        public Form1()
        {
            InitializeComponent();
            //1. create viewport
            easeViewport = EaseHost.CreateViewportControl(this, 800, 600);
            //2. add
            this.panel1.Controls.Add(easeViewport.ViewportControl);
            //this.Controls.Add(easeViewport.ViewportControl);
            this.Load += new EventHandler(Form1_Load);
            this.myWbConsole = new MyWebConsole(this.textBox1);
        }
        void Form1_Load(object sender, EventArgs e)
        {
            //load sample html text
            easeViewport.Ready();
            string filename = @"..\..\..\HtmlRenderer.Demo\Samples\ClassicSamples\00.Intro.htm";
            //read text file
            //var fileContent = System.IO.File.ReadAllText(filename);
            //start with about blank
            //easeViewport.LoadHtml(filename, fileContent); 
            easeViewport.LoadHtml(filename, "<html><body></body></html");
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
            var fileContent = "<html><body><div id=\"a\">A</div><div id=\"b\" style=\"background-color:blue\">B<span id=\"s1\">span content</span></div></body></html>";
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
            var htmlElementB = domNodeB as LayoutFarm.WebDom.IHtmlElement;
            string innerHtmlContent = htmlElementB.innerHTML;
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
                this.easeViewport.Print((PixelFarm.Drawing.WinGdi.MyGdiPlusCanvas)easeCanvas);
            };
            printdoc.Print();
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
            //JsBridge.LoadV8("..\\..\\dll\\VRoomJsNative.dll");

#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
            //===============================================================

            //2. access dom  

            var webdoc = easeViewport.GetHtmlDom() as IHtmlDocument;
            //create js engine and context
            var jstypeBuilder = new LayoutFarm.Scripting.MyJsTypeDefinitionBuilder();
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext(jstypeBuilder))
            {
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();
                ctx.SetVariableAutoWrap("document", webdoc);
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





        private void button6_Click(object sender, EventArgs e)
        {
            string filename = @"..\..\..\HtmlRenderer.Demo\Samples\ClassicSamples\00.Intro.htm";
            //1. blank html
            var fileContent = "<html><body><div id=\"a\">A</div><div id=\"b\" style=\"background-color:blue\">B</div><div id=\"c\">c_node</div></body></html>";
            easeViewport.LoadHtml(filename, fileContent);
            //----------------------------------------------------------------
            //after load html page 

            //load v8 if ready
            //JsBridge.LoadV8("..\\..\\dll\\VRoomJsNative.dll");
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
            //===============================================================

            //2. access dom  

            var webdoc = easeViewport.GetHtmlDom() as LayoutFarm.WebDom.IHtmlDocument;
            //create js engine and context
            if (myengine == null)
            {
                var jstypeBuilder = new LayoutFarm.Scripting.MyJsTypeDefinitionBuilder();
                myengine = new JsEngine();
                myCtx = myengine.CreateContext(jstypeBuilder);
            }

            System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
            stwatch.Reset();
            stwatch.Start();
            myCtx.SetVariableAutoWrap("document", webdoc);
            myCtx.SetVariableAutoWrap("console", myWbConsole);
            string simplejs = @"
                    (function(){
                        console.log('hello world!');
                        var domNodeA = document.getElementById('a');
                        var domNodeB = document.getElementById('b');
                        var domNodeC = document.getElementById('c');

                        var newText1 = document.createTextNode('... says hello world!');
                        domNodeA.appendChild(newText1);
                        for(var i=0;i<10;++i){
                            var newText2= document.createTextNode(i);
                            domNodeA.appendChild(newText2);       
                        } 

                        var newDivNode= document.createElement('div');
                        newDivNode.appendChild(document.createTextNode('new div'));
                        newDivNode.attachEventListener('mousedown',function(){console.log('new div');});
                        domNodeB.appendChild(newDivNode);    
                        
                        domNodeC.innerHTML='<div> from inner html <span> from span</span> </div>';
                        console.log(domNodeC.innerHTML);
                    })();
                ";
            object testResult = myCtx.Execute(simplejs);
            stwatch.Stop();
            Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());
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

        private void button7_Click(object sender, EventArgs e)
        {
            string filename = @"..\..\..\HtmlRenderer.Demo\Samples\ClassicSamples\00.Intro.htm";
            //1. blank html
            var fileContent = "<html><script>function doc_ready(){console.log('doc_ready');}</script><body onload=\"doc_ready()\"><div id=\"a\">A</div><div id=\"b\" style=\"background-color:blue\">B</div><div id=\"c\">c_node</div></body></html>";
            easeViewport.LoadHtml(filename, fileContent);
            //----------------------------------------------------------------
            //after load html page 

            //load v8 if ready
            // JsBridge.LoadV8("..\\..\\dll\\VRoomJsNative.dll");
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
            //===============================================================

            //2. access dom  

            var webdoc = easeViewport.GetHtmlDom() as LayoutFarm.WebDom.IHtmlDocument;
            //create js engine and context
            if (myengine == null)
            {
                var jstypeBuilder = new LayoutFarm.Scripting.MyJsTypeDefinitionBuilder();
                myengine = new JsEngine();
                myCtx = myengine.CreateContext(jstypeBuilder);
            }

            System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
            stwatch.Reset();
            stwatch.Start();
            myCtx.SetVariableAutoWrap("document", webdoc);
            myCtx.SetVariableAutoWrap("console", myWbConsole);
            string simplejs = @"
                    (function(){
                        console.log('hello world!');
                        var domNodeA = document.getElementById('a');
                        var domNodeB = document.getElementById('b');
                        var domNodeC = document.getElementById('c');

                        var newText1 = document.createTextNode('... says hello world!');
                        domNodeA.appendChild(newText1);

                        for(var i=0;i<10;++i){
                            var newText2= document.createTextNode(i.toString());
                            domNodeA.appendChild(newText2);       
                        } 

                        var newDivNode= document.createElement('div');
                        newDivNode.appendChild(document.createTextNode('new div'));
                        newDivNode.attachEventListener('mousedown',function(){console.log('new div');});
                        domNodeB.appendChild(newDivNode);    
                        
                        domNodeC.innerHTML='<div> from inner html <span> from span</span> </div>';
                        console.log(domNodeC.innerHTML);
                    })();
                ";
            object testResult = myCtx.Execute(simplejs);
            stwatch.Stop();
            Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());
        }
    }
}
