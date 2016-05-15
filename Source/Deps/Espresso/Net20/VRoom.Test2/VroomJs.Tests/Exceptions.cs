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
    public class Exceptions
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

        [TestCase]
        [ExpectedException(typeof(JsException))]
        public void SimpleExpressionException()
        {
            using (JsContext js = jsEngine.CreateContext())
            {
                js.Execute("throw 'xxx'");
            }
        }

        [TestCase]
        [ExpectedException(typeof(JsException))]
        public void JsObjectException()
        {
            using (JsContext js = jsEngine.CreateContext())
            {
                js.Execute("throw {msg:'Error!'}");
            }
        }

        [TestCase]
        [ExpectedException(typeof(JsException))]
        public void CompilationException()
        {
            using (JsContext js = jsEngine.CreateContext())
            {
                js.Execute("a+§");
            }
        }
    }
}

