using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;

using System.Text;
using System.Windows.Forms;
using VroomJs;
using NUnit.Framework;

namespace VRoomJsTest2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //-----------------------------------------------------------------------            
            var asm = typeof(VroomJs.Tests.TestClass).Assembly;
            var testFixtureAttr = typeof(NUnit.Framework.TestFixtureAttribute);
            var testAttr = typeof(NUnit.Framework.TestAttribute);
            var setupAttr = typeof(NUnit.Framework.SetUpAttribute);
            var tearDownAttr = typeof(NUnit.Framework.TearDownAttribute);
            var testCaseAttr = typeof(NUnit.Framework.TestCaseAttribute);

            //-----------------------------------------------------------------------            
            List<TestCaseInstance> testCaseList = new List<TestCaseInstance>();
            foreach (var t in asm.GetTypes())
            {
                var founds = t.GetCustomAttributes(testFixtureAttr, false);
                if (founds != null && founds.Length > 0)
                {
                    //test 
                    var testCaseInstance = new TestCaseInstance(t);
                    //find setup/teardown method
                    foreach (var met in t.GetMethods())
                    {
                        var someSetUpAttrs = met.GetCustomAttributes(setupAttr, false);
                        if (someSetUpAttrs != null && someSetUpAttrs.Length > 0)
                        {
                            testCaseInstance.SetSetupMethod(met);
                            continue;
                        }
                        var someTeardownAttrs = met.GetCustomAttributes(tearDownAttr, false);
                        if (someTeardownAttrs != null && someTeardownAttrs.Length > 0)
                        {
                            testCaseInstance.SetTeardownMethod(met);
                            continue;
                        }
                        var someTestAttrs = met.GetCustomAttributes(testAttr, false);
                        if (someTestAttrs != null && someTestAttrs.Length > 0)
                        {

                            var testMethod = testCaseInstance.AddTestMethod(met);
                            this.listBox1.Items.Add(testMethod);
                            continue;
                        }
                        someTestAttrs = met.GetCustomAttributes(testCaseAttr, false);
                        if (someTestAttrs != null && someTestAttrs.Length > 0)
                        {
                            var testMethod = testCaseInstance.AddTestMethod(met);
                            this.listBox1.Items.Add(testMethod);
                        }
                    }

                    testCaseList.Add(testCaseInstance);
                }
            }
            //---------------------------------------------------------------------
            this.listBox1.DoubleClick += new EventHandler(listBox1_DoubleClick);
        }

        void listBox1_DoubleClick(object sender, EventArgs e)
        {
            var testCaseMethod = listBox1.SelectedItem as TestCaseMethod;
            if (testCaseMethod != null)
            {
                //run test
                testCaseMethod.RunTest();
            }
        }

        class TestMe1
        {
            public int B()
            {
                return 100;
            }
            public bool C()
            {
                return true;
            }
        }

        delegate object AboutMeEventHandler(object[] args);

        [JsType]
        class AboutMe
        {
            AboutMeEventHandler mousedownEventHandler;

            [JsMethod]
            public int Test1()
            {
                return 123;
            }

            [JsMethod]
            public string Test2(string text)
            {
                return "hello " + text;
            }
            [JsProperty]
            public bool IsOK
            {
                get
                {
                    return true;
                }
            }
            [JsMethod]
            public AboutMe NewAboutMe()
            {
                return new AboutMe();
            }
            [JsMethod]
            public void AttachEvent(string eventName, AboutMeEventHandler evHandler)
            {
                this.mousedownEventHandler = evHandler;
            }
            [JsMethod]
            public void FireEventMouseDown(object eventArg)
            {
                if (mousedownEventHandler != null)
                {
                    //JsFunction func = mousedownEventHandler.Target as JsFunction;
                    //func.Invoke(eventArg);
                    //SimpleDelegate simpleDel = mousedownEventHandler as SimpleDelegate;
                    //simpleDel(new object[] { eventArg }); 
                    //mousedownEventHandler(new object[] { eventArg });
                    mousedownEventHandler(new object[] { null, eventArg });
                    //mousedownEventHandler(this, eventArg);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif

            JsTypeDefinition jstypedef = new JsTypeDefinition("AA");
            jstypedef.AddMember(new JsMethodDefinition("B", args =>
            {
                args.SetResult(100);
            }));
            jstypedef.AddMember(new JsMethodDefinition("C", args =>
            {
                args.SetResult(true);
            }));
            //===============================================================
            //create js engine and context
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext())
            {

                if (!jstypedef.IsRegisterd)
                {
                    ctx.RegisterTypeDefinition(jstypedef);
                }
                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Start();

                TestMe1 t1 = new TestMe1();
                var proxy = ctx.CreateWrapper(t1, jstypedef);

                for (int i = 2000; i >= 0; --i)
                {
                    ctx.SetVariableFromAny("x", proxy);
                    object result = ctx.Execute("(function(){if(x.C()){return  x.B();}else{return 0;}})()");
                }
                stwatch.Stop();

                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());
                //Assert.That(result, Is.EqualTo(100));
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
            //create js engine and context

            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext())
            {
                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Start();

                TestMe1 t1 = new TestMe1();

                for (int i = 2000; i >= 0; --i)
                {
                    ctx.SetVariableFromAny("x", t1);
                    object result = ctx.Execute("(function(){if(x.C()){return  x.B();}else{return 0;}})()");
                }
                stwatch.Stop();
                Console.WriteLine("met2 managed reflection:" + stwatch.ElapsedMilliseconds.ToString());
                //Assert.That(result, Is.EqualTo(100)); 
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
            JsTypeDefinition jstypedef = new JsTypeDefinition("AA");
            jstypedef.AddMember(new JsMethodDefinition("B", args =>
            {
                args.SetResult(100);
            }));
            jstypedef.AddMember(new JsMethodDefinition("C", args =>
            {
                args.SetResult(true);
            }));

            jstypedef.AddMember(new JsPropertyDefinition("D",
                args =>
                {   //getter
                    args.SetResult(true);
                },
                args =>
                {
                    //setter 
                }));
            jstypedef.AddMember(new JsPropertyDefinition("E",
                args =>
                {   //getter
                    args.SetResult(250);
                },
                args =>
                {
                    //setter 
                }));

            //===============================================================
            //create js engine and context
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext())
            {

                ctx.RegisterTypeDefinition(jstypedef);

                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();

                TestMe1 t1 = new TestMe1();
                var proxy = ctx.CreateWrapper(t1, jstypedef);

                //for (int i = 2000; i >= 0; --i)
                //{
                ctx.SetVariableFromAny("x", proxy);
                //object result = ctx.Execute("(function(){if(x.C()){return  x.B();}else{return 0;}})()");
                object result = ctx.Execute("(function(){if(x.D){ x.E=300; return  x.B();}else{return 0;}})()");

                //}
                stwatch.Stop();

                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());
                //Assert.That(result, Is.EqualTo(100));
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
            JsTypeDefinition jstypedef = new JsTypeDefinition("AA");
            jstypedef.AddMember(new JsMethodDefinition("B", args =>
            {
                var argCount = args.ArgCount;
                var thisArg = args.GetThisArg();
                var arg0 = args.GetArgAsObject(0);

                args.SetResult(100);
            }));
            jstypedef.AddMember(new JsMethodDefinition("C", args =>
            {
                args.SetResult(true);
            }));

            //----------------------------------------------------- 
            jstypedef.AddMember(new JsPropertyDefinition("D",
                args =>
                {
                    //getter
                    TestMe1 t2 = new TestMe1();
                    args.SetResultObj(t2, jstypedef);

                },
                args =>
                {
                    //setter 
                }));
            jstypedef.AddMember(new JsPropertyDefinition("E",
                args =>
                {   //getter
                    args.SetResult(250);
                },
                args =>
                {
                    //setter 
                }));

            //===============================================================
            //create js engine and context
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext())
            {

                ctx.RegisterTypeDefinition(jstypedef);

                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();

                TestMe1 t1 = new TestMe1();
                INativeScriptable proxy = ctx.CreateWrapper(t1, jstypedef);

                ctx.SetVariable("x", proxy);

                //string testsrc = "(function(){if(x.C()){return  x.B();}else{return 0;}})()";
                //string testsrc = "(function(){if(x.D != null){ x.E=300; return  x.B();}else{return 0;}})()";
                string testsrc = "x.B(x.D,15);";

                object result = ctx.Execute(testsrc);
                stwatch.Stop();

                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
            JsTypeDefinition jstypedef = new JsTypeDefinition("AA");
            jstypedef.AddMember(new JsMethodDefinition("B", args =>
            {
                var argCount = args.ArgCount;
                var thisArg = args.GetThisArg();
                var arg0 = args.GetArgAsObject(0);
                args.SetResult((bool)arg0);

            }));
            jstypedef.AddMember(new JsMethodDefinition("C", args =>
            {
                args.SetResult(true);
            }));

            //----------------------------------------------------- 
            jstypedef.AddMember(new JsPropertyDefinition("D",
                args =>
                {
                    var ab = new AboutMe();
                    args.SetResultAutoWrap(ab);
                },
                args =>
                {
                    //setter 
                }));
            jstypedef.AddMember(new JsPropertyDefinition("E",
                args =>
                {   //getter
                    args.SetResult(250);
                },
                args =>
                {
                    //setter 
                }));
            //===============================================================
            //create js engine and context 
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext())
            {

                ctx.RegisterTypeDefinition(jstypedef);

                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();


                TestMe1 t1 = new TestMe1();

                INativeScriptable proxy = ctx.CreateWrapper(t1, jstypedef);
                ctx.SetVariable("x", proxy);


                string testsrc = "x.B(x.D.IsOK);";
                object result = ctx.Execute(testsrc);
                stwatch.Stop();

                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext(new MyJsTypeDefinitionBuilder()))
            {

                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();

                var ab = new AboutMe();
                ctx.SetVariableAutoWrap("x", ab);

                //string testsrc = "x.IsOK;";
                string testsrc = "x.Test2('AAA');";
                object result = ctx.Execute(testsrc);
                stwatch.Stop();

                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
#if DEBUG
            JsBridge.dbugTestCallbacks();
#endif
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext(new MyJsTypeDefinitionBuilder()))
            {

                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();

                var ab = new AboutMe();
                ctx.SetVariableAutoWrap("x", ab);

                //string testsrc = "x.IsOK;";
                string testsrc = "x.NewAboutMe();";
                object result = ctx.Execute(testsrc);
                stwatch.Stop();

                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());

            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int version = JsBridge.LibVersion;
            JsBridge.dbugTestCallbacks();
            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext(new MyJsTypeDefinitionBuilder()))
            {

                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();

                var ab = new AboutMe();
                ctx.SetVariableAutoWrap("x", ab);

                string testsrc = @"(function(){
                    x.AttachEvent('mousedown',function(evArgs){});
                    x.FireEventMouseDown({});
                })()";
                object result = ctx.Execute(testsrc);
                stwatch.Stop();

                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());

            }

        }
    }
}
