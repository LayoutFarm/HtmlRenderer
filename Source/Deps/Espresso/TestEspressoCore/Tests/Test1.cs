using System;
using Espresso;

namespace TestEspressoCore
{
    public class Test1
    {
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
        [Test("1", "Test1")]
        static void TestCase1()
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
                //test value of
                object re = ctx.Execute("(function(){function myNumberType(n) {    this.number = n;}" +
                        "myNumberType.prototype.valueOf = function() {    return this.number;};" +
                        "myObj = new myNumberType(4);" +
                        "return myObj + 3;" +
                        "})()");
                stwatch.Stop();

                Console.WriteLine("met1 template:" + stwatch.ElapsedMilliseconds.ToString());
                //Assert.That(result, Is.EqualTo(100));
            }
        }

        [Test("2", "Test2")]
        static void TestCase2()
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
    }
}
