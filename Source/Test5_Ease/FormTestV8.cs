using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;

using System.Text;
using System.Windows.Forms;
using VroomJs;

using LayoutFarm.Scripting;

namespace Test5_Ease
{
    public partial class FormTestV8 : Form
    {
        public FormTestV8()
        {
            InitializeComponent();
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


        [JsType]
        class AboutMe
        {
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
                var proxy = ctx.CreateWrapper(t1, jstypedef);
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

            MyJsTypeDefinitionBuilder customBuilder = new MyJsTypeDefinitionBuilder();

            using (JsEngine engine = new JsEngine())
            using (JsContext ctx = engine.CreateContext(customBuilder))
            {

                ctx.RegisterTypeDefinition(jstypedef);

                GC.Collect();
                System.Diagnostics.Stopwatch stwatch = new System.Diagnostics.Stopwatch();
                stwatch.Reset();
                stwatch.Start();

                //AboutMe ab = new AboutMe();
                //ctx.SetVariableFromAny("x", ab);
                //ctx.SetVariable(

                TestMe1 t1 = new TestMe1();
                var proxy = ctx.CreateWrapper(t1, jstypedef);
                ctx.SetVariable("x", proxy);

                //string testsrc = "(function(){if(x.C()){return  x.B();}else{return 0;}})()";
                //string testsrc = "(function(){if(x.D != null){ x.E=300; return  x.B();}else{return 0;}})()";
                //string testsrc = "x.B(x.D.IsOk,x.D.Test1());";
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





        private void FormTestV8_Load(object sender, EventArgs e)
        {

        }


    }
}
