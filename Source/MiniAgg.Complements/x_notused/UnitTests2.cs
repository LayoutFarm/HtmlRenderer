///*
//Copyright (c) 2014, Lars Brubaker
//All rights reserved.

//Redistribution and use in source and binary forms, with or without
//modification, are permitted provided that the following conditions are met: 

//1. Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer. 
//2. Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
//ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

//The views and conclusions contained in the software and documentation are those
//of the authors and should not be interpreted as representing official policies, 
//either expressed or implied, of the FreeBSD Project.
//*/

//using System;
//using System.Collections.Generic;
//
//using System.Text;

//using NUnit.Framework;
//using MatterHackers.Agg;
//using MatterHackers.Agg.Image;
//using MatterHackers.VectorMath;

//namespace MatterHackers.Agg.Tests
//{
//    [TestFixture]
//    public class SimpleTests
//    {
//        [Test]
//        public void TestGetHashCode()
//        {
//            {
//                RGBA_Bytes a = new RGBA_Bytes(10, 11, 12);
//                RGBA_Bytes b = new RGBA_Bytes(10, 11, 12);
//                Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
//            }
//            {
//                RGBA_Floats a = new RGBA_Floats(10, 11, 12);
//                RGBA_Floats b = new RGBA_Floats(10, 11, 12);
//                Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
//            }
//            {
//                BorderDouble a = new BorderDouble(10, 11, 12, 13);
//                BorderDouble b = new BorderDouble(10, 11, 12, 13);
//                Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
//            }
//            {
//                Point2D a = new Point2D(10, 11);
//                Point2D b = new Point2D(10, 11);
//                Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
//            }
//            {
//                RectangleDouble a = new RectangleDouble(10, 11, 12, 13);
//                RectangleDouble b = new RectangleDouble(10, 11, 12, 13);
//                Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
//            }
//            {
//                RectangleInt a = new RectangleInt(10, 11, 12, 13);
//                RectangleInt b = new RectangleInt(10, 11, 12, 13);
//                Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
//            }
//        }
//    }

//    public static class UnitTests
//    {
//        static bool ranTests = false;

//        public static bool RanTests { get { return ranTests; } }
//        public static void Run()
//        {
//            if (!ranTests)
//            {
//                SimpleTests simpleTest = new SimpleTests();
//                simpleTest.TestGetHashCode();

//                ranTests = true;
//            }
//        }
//    }
//}
