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
using NUnit.Framework;

namespace VroomJs.Tests
{
    [TestFixture]
    public class Objects
    {
        JsEngine jsEngine;
        [SetUp]
        public void Setup()
        {
            jsEngine = new JsEngine();
        }

        [TearDown]
        public void Teardown()
        {
            jsEngine.Dispose();
        }

        [Test]
        public void GetManagedIntegerProperty()
        {
            var v = 42;
            var t = new TestClass { Int32Property = v };
            using (JsContext js = jsEngine.CreateContext())
            {
                js.SetVariableFromAny("o", t);
                Assert.That(js.Execute("o.Int32Property"), Is.EqualTo(v));
            }
        }
        [Test]
        public void GetManagedBoolProperty()
        {
            var v = 42;
            var t = new TestClass { BoolProperty = true, Int32Property = 30 };
            using (JsContext js = jsEngine.CreateContext())
            {
                js.SetVariableFromAny("o", t);
                Assert.That(js.Execute("(function(){if(o.BoolProperty){return 10; }else{ return 20;}})()"), Is.EqualTo(10));
            }
        }
        [Test]
        public void GetManagedStringProperty()
        {
            var v = "The lazy dog bla bla bla...";
            var t = new TestClass { StringProperty = v };
            using (JsContext js = jsEngine.CreateContext())
            {
                js.SetVariableFromAny("o", t);
                Assert.That(js.Execute("o.StringProperty"), Is.EqualTo(v));
            }
        }

        [Test]
        public void GetManagedNestedProperty()
        {
            var v = "The lazy dog bla bla bla...";
            var t = new TestClass { NestedObject = new TestClass { StringProperty = v } };
            using (JsContext js = jsEngine.CreateContext())
            {
                js.SetVariableFromAny("o", t);
                var result = js.Execute("o.NestedObject.StringProperty");
                Assert.That(result, Is.EqualTo(v));
            }
        }

        [Test]
        public void SetManagedIntegerProperty()
        {
            var t = new TestClass();
            using (JsContext js = jsEngine.CreateContext())
            {
                js.SetVariableFromAny("o", t);
                js.Execute("o.Int32Property = 42");
                Assert.That(t.Int32Property, Is.EqualTo(42));
            }
        }

        [Test]
        public void SetManagedStringProperty()
        {
            var t = new TestClass();
            using (JsContext js = jsEngine.CreateContext())
            {
                js.SetVariableFromAny("o", t);
                js.Execute("o.StringProperty = 'This was set from Javascript!'");
                Assert.That(t.StringProperty, Is.EqualTo("This was set from Javascript!"));
            }
        }

        [Test]
        public void SetManagedNestedProperty()
        {
            var t = new TestClass();
            var n = new TestClass();
            using (JsContext js = jsEngine.CreateContext())
            {
                js.SetVariableFromAny("o", t);
                js.SetVariableFromAny("n", n);
                js.Execute("o.NestedObject = n; o.NestedObject.Int32Property = 42");
                Assert.That(t.NestedObject, Is.EqualTo(n));
                Assert.That(n.Int32Property, Is.EqualTo(42));
            }
        }

        [Test]
        public void CallManagedMethod()
        {
            var t = new TestClass { Int32Property = 13, StringProperty = "Wow" };
            using (JsContext js = jsEngine.CreateContext())
            {
                js.SetVariableFromAny("o", t);
                js.Execute("var r = o.Method1(29, '!')");
                object r = js.GetVariable("r");
                Assert.That(r, Is.AssignableTo<TestClass>());
                TestClass c = (TestClass)r;
                Assert.That(c.Int32Property, Is.EqualTo(42));
                Assert.That(c.StringProperty, Is.EqualTo("Wow!"));
            }
        }

        [Test]
        public void GetJsIntegerProperty()
        {
            using (JsContext js = jsEngine.CreateContext())
            {
                js.Execute("var x = { the_answer: 42 }");
                var x = js.GetVariable("x") as JsObject;
                Assert.That(x["the_answer"], Is.EqualTo(42));

            }
        }

        [Test]
        public void GetJsStringProperty()
        {
            using (JsContext js = jsEngine.CreateContext())
            {
                js.Execute("var x = { a_string: 'This was set from Javascript!' }");
                var x = js.GetVariable("x") as JsObject;
                Assert.That(x["a_string"], Is.EqualTo("This was set from Javascript!"));

            }
        }

        [Test]
        public void GetMixedProperty1()
        {
            using (JsContext js = jsEngine.CreateContext())
            {
                var t = new TestClass();
                js.SetVariableFromAny("o", t);
                js.Execute("var x = { nested: o }; x.nested.Int32Property = 42");
                var x = js.GetVariable("x") as JsObject;

                Assert.That(x["nested"], Is.EqualTo(t));
                Assert.That(t.Int32Property, Is.EqualTo(42));

            }
        }

        [Test]
        public void SetJsIntegerProperty()
        {
            using (JsContext js = jsEngine.CreateContext())
            {
                var v = 42;
                js.Execute("var x = {}");
                var x = js.GetVariable("x") as JsObject;
                x["the_answer"] = v;
                Assert.That(js.Execute("x.the_answer"), Is.EqualTo(v));

            }
        }

        [Test]
        public void SetJsStringProperty()
        {
            using (JsContext js = jsEngine.CreateContext())
            {
                var v = "This was set from managed code!";
                js.Execute("var x = {}");
                var x = js.GetVariable("x") as JsObject;

                x["a_string"] = v;
                Assert.That(js.Execute("x.a_string"), Is.EqualTo(v));

            }
        } 
        [Test]
        public void CallJsProperty()
        {
            using (JsContext js = jsEngine.CreateContext())
            {

                js.Execute("var x = { f: function (a, b) { return [a*2, b+'!']; }}");

                var x = js.GetVariable("x") as JsObject;
                var f = x["f"] as JsFunction; 
                var r = f.Invoke(21, "Can't believe this worked");
                 
                Assert.That(r, Is.AssignableTo<object[]>());
                object[] a = (object[])r;
                Assert.That(a.Length, Is.EqualTo(2));
                Assert.That(a[0], Is.EqualTo(42));
                Assert.That(a[1], Is.EqualTo("Can't believe this worked!"));

            }
        }

    }
}

