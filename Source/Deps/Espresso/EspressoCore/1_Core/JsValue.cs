// This file is part of the VroomJs library.
//
// Author:
//     Federico Di Gregorio <fog@initd.org>
//
// Copyright © 2013 Federico Di Gregorio <fog@initd.org>
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
using System.Runtime.InteropServices;

namespace Espresso
{
    [StructLayout(LayoutKind.Explicit)]
    struct JsValue
    {
        [FieldOffset(0)]
        public int I32;
        [FieldOffset(0)]
        public long I64;
        [FieldOffset(0)]
        public double Num;
        /// <summary>
        /// ptr from native side
        /// </summary>
        [FieldOffset(0)]
        public IntPtr Ptr;

        /// <summary>
        /// offset(8)See JsValueType, marshaled as integer. 
        /// </summary>
        [FieldOffset(8)]
        public JsValueType Type;

        /// <summary>
        /// offset(12) Length of array or string 
        /// </summary>
        [FieldOffset(12)]
        public int Length;
        /// <summary>
        /// offset(12) managed object keepalive index. 
        /// </summary>
        [FieldOffset(12)]
        public int Index;




        public static JsValue Null
        {
            get { return new JsValue() { Type = JsValueType.Null }; }
        }

        public static JsValue Empty
        {
            get { return new JsValue() { Type = JsValueType.Empty }; }
        }

        public static JsValue Error(int slot)
        {
            return new JsValue { Type = JsValueType.ManagedError, Index = slot };
        }

        public override string ToString()
        {
            return string.Format("[JsValue({0})]", Type);
        }
    }
}
