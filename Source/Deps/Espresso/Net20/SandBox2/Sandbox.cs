// This file is part of the VroomJs library.
//
// Author:
//     Federico Di Gregorio <fog@initd.org>
//
// Copyright (c) 2013 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using VroomJs;

namespace Sandbox
{
    public delegate R Func<T, R>(T a1);

    public class debugtest
    {
        public int TestProp { get; set; }

        public static bool BoolTest(int a, int b)
        {
            return a == b;
        }

        public void Write(object v)
        {
            Console.WriteLine(v);
        }

        public void RunFunc(Func<int, string> callback)
        {
            for (int i = 1; i <= 3; i++)
            {
                string data = callback(i);
                Console.WriteLine(data);
            }
        }

        public double ValueOf()
        {
            return 31.7777;
        }
    }


    class Sandbox
    {
        public static void Main(string[] args)
        {
            // string lodash = File.ReadAllText(@"c:\lodash.js");
            using (JsEngine engine = new JsEngine())
            {
                //Stopwatch watch = new Stopwatch();
                //	watch.Start();
                JsScript script = engine.CompileScript("3+3");
                using (JsContext ctx = engine.CreateContext())
                {
                    ctx.Execute(script);
                }
            }

            debugtest dbg = new debugtest();

            //	Delegate.CreateDelegate()
            //Dictionary<string, object> values = new Dictionary<string, object>();

            //values["test"] = 333;
            while (true)
            {
                using (JsEngine js = new JsEngine(4, 32))
                {   
                    using (JsContext context = js.CreateContext())
                    {
                        //context.SetVariable("dbg", dbg);
                        //object result = context.Execute("dbg.Write(dbg.valueOf());");
                        context.SetVariableFromAny("Debug", typeof(debugtest));

                        object result = context.Execute("Debug.BoolTest(3,4);");

                    }
                    GC.Collect();
                    js.DumpHeapStats();
                }
            }


            //context.SetVariable("values", values);
            //object result = context.Execute("dbg.runFunc(); values.test ? true : false;");



            //	object result =
            //	context.Execute("var obj = { test: 0 }; obj.ft = function (v) { dbg.Write(v); return 'from JS'; }; dbg.runFunc(obj.ft); dbg.write(obj.test);");

            //int a = 1;

            //	context.SetFunction("runfunc", new Func<int, bool>(Activate));
            //object result = context.Execute("runfunc(2);");
            //  }

            //		}
            //}
            /*using (JsEngine js = new JsEngine()) {
                using (JsContext context = js.CreateContext()) {
                    using (JsScript script = js.CompileScript("3 * 4")) {
                        object result = context.Execute(script, TimeSpan.FromHours(200));
                        Console.WriteLine(result);
                    }
                }
            }*/

            //return;
            /*
            using (JsEngine js = new JsEngine()) {
                using (JsContext context = js.CreateContext()) {
                    for (int i = 0; i < 10; i++) {
                        context.SetVariable("a", new Simple { N = i, S = (i * 10).ToString() });
                        Console.WriteLine(context.Execute("a.N+' '+a.S"));
                    }
                    Console.WriteLine(context.Execute("a.N+' '+a.X"));
                }
            }*/
        }

        private static bool Activate(int arg)
        {
            return arg > 5;
        }
    }

    class Simple
    {
        public int N { get; set; }
        public string S { get; set; }
        public string X
        {
            get { throw new InvalidOperationException("ahr ahr"); }
        }
    }
}
