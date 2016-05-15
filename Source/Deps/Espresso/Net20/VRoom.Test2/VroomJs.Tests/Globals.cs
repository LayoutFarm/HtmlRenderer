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
    public class Globals
    {
        JsEngine jsEngine;
        JsContext js;
        [SetUp]
        public void Setup()
        {
            jsEngine = new JsEngine();
            js = jsEngine.CreateContext();

        }

        [TearDown]
        public void Teardown()
        {
            jsEngine.Dispose();
        }

        [TestCase]
        public void SimpleExpressionNull()
        {

            Assert.That(js.Execute("null"), Is.Null);
        }

        [TestCase]
        public void SimpleExpressionBoolean()
        {
            Assert.That(js.Execute("0 == 0"), Is.EqualTo(true));
        }

        [TestCase]
        public void SimpleExpressionInteger()
        {
            Assert.That(js.Execute("1+1"), Is.EqualTo(2));
        }

        [TestCase]
        public void SimpleExpressionNumber()
        {
            Assert.That(js.Execute("3.14159+2.71828"), Is.EqualTo(5.85987));
        }

        [TestCase]
        public void SimpleExpressionString()
        {
            Assert.That(js.Execute("'paco'+'cico'"), Is.EqualTo("pacocico"));
        }

        [TestCase]
        public void SimpleExpressionDate()
        {

            //DateTime d1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //var d2 = d1.ToLocalTime();
            //var ticks_ms = d1.Ticks / 10000;

            //DateTime d3 = new DateTime(1970, 1, 1, 0, 0, 0);
            //var d4 = d3.ToLocalTime();


            Assert.That(js.Execute(
                  "new Date('2014-01-01')"),//utc
                  Is.EqualTo(new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc)));

            Assert.That(js.Execute(
                 "new Date(2014,1,1)"),//utc
                 Is.EqualTo(new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc)));


            //Assert.That(js.Execute("new Date('1971-10-19')"),
            //               Is.EqualTo(new DateTime(1971, 10, 19, 0, 0, 0, DateTimeKind.Utc)));
            //Assert.That(js.Execute("new Date(1971, 10, 19, 0, 42, 59)"),
            //    Is.EqualTo(new DateTime(1971, 10, 19, 0, 42, 59, DateTimeKind.Utc)));
        }
        static DateTime FromJsDateTime(long jsMs)
        {
            return new DateTime(jsMs * 10000, DateTimeKind.Utc);
        }

        [TestCase]
        public void SimpleExpressionArray()
        {
            var res = (object[])js.Execute("['foobar', 3.14159+2.71828, 42]");
            Assert.That(res.Length, Is.EqualTo(3));
            Assert.That(res[0], Is.EqualTo("foobar"));
            Assert.That(res[1], Is.EqualTo(5.85987));
            Assert.That(res[2], Is.EqualTo(42));
        }

        [TestCase]
        public void SimpleExpressionObject()
        {
            // Note that a simple "{answer:42}" at top level just returns "42", so we
            // have to use a function and a "return" statement.

            try
            {
                dynamic res = js.Execute("(function () { return {answer:42}})()");
            }
            catch (Exception ex)
            {

            }
#if NET40
            Assert.That(res.answer, Is.EqualTo(42));
#endif
        }

        [TestCase]
        public void ExpressionAndCall()
        {

            JsObject x = js.Execute(@"
                (function () { 
                    return {'answer':42, 'tellme':function (x) { return x+' The answer is: '+this.answer; }}
                })()") as JsObject;
            if (x != null)
            {
                var tellmeFunc = x["tellme"] as JsFunction;
                //tellmeFunc.Invoke(new object[] { "What is the answer to ...?" });
                object s;
                x.TryInvokeMember("tellme", new[] { "What is the answer to ...?" }, out s); 
                Assert.That(s, Is.EqualTo("What is the answer to ...? The answer is: 42"));
            }

#if NET40
            object s = x.tellme("What is the answer to ...?");
            Assert.That(s, Is.EqualTo("What is the answer to ...? The answer is: 42"));
#endif
        }

        [TestCase]
        public void UnicodeScript()
        {
            Assert.That(js.Execute("var àbç = 12, $ùì = 30; àbç+$ùì;"), Is.EqualTo(42));
        }

        [TestCase]
        public void SetGetVariableNull()
        {
            js.SetVariableNull("foo");
            Assert.That(js.GetVariable("foo"), Is.Null);
        }

        [TestCase]
        public void SetGetVariableBoolean()
        {
            js.SetVariableFromAny("foo", true);
            Assert.That(js.GetVariable("foo"), Is.EqualTo(true));
        }

        [TestCase]
        public void SetGetVariableInteger()
        {
            js.SetVariable("foo", 13);
            Assert.That(js.GetVariable("foo"), Is.EqualTo(13));
        }

        [TestCase]
        public void SetGetVariableNumber()
        {
            js.SetVariable("foo", 3.14159);
            Assert.That(js.GetVariable("foo"), Is.EqualTo(3.14159));
        }

        [TestCase]
        public void SetGetVariableString()
        {
            js.SetVariable("foo", "bar");
            Assert.That(js.GetVariable("foo"), Is.EqualTo("bar"));
        }

        [TestCase]
        public void SetGetVariableDate()
        {
            var dt = new DateTime(1971, 10, 19, 0, 42, 59);
            js.SetVariable("foo", dt);
            Assert.That(js.GetVariable("foo"), Is.EqualTo(dt));
        }

        [TestCase]
        public void SetGetVariableArray()
        {
            var v = new object[] { "foobar", 3.14159, 42 };
            js.SetVariableFromAny("foo", v);
            js.Execute("foo[1] += 2.71828");
            object r = js.GetVariable("foo");
            Assert.That(r, Is.AssignableTo<object[]>());
            var a = (object[])r;
            Assert.That(a.Length, Is.EqualTo(3));
            Assert.That(a[0], Is.EqualTo("foobar"));
            Assert.That(a[1], Is.EqualTo(5.85987));
            Assert.That(a[2], Is.EqualTo(42));
        }

        [TestCase]
        public void ObjectCount()
        {
            // Apparently there is no safe way to force a full GC on the JS side.
            // We will check that the number of live objects _at least_ got lower
            // than what we created.
            var dt = new TestClass();
            for (int i = 0; i < 100000; i++)
                js.SetVariableFromAny("foo", dt);
            js.SetVariableNull("foo");
            js.Flush();
            Assert.That(js.GetStats().KeepAliveUsedSlots, Is.LessThan(80000));
        }

    }
}
