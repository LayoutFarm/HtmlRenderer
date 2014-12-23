//using System;
//using System.Collections.Generic;
//
//using System.Text;

//using NUnit.Framework;
//using MatterHackers.Agg;
//using MatterHackers.Agg.Image;

//namespace MatterHackers.Agg.Font
//{
//    [TestFixture]
//    public class FontTests
//    {
//        [Test]
//        public void CanPrintTests()
//        {
//            ImageBuffer testImage = new ImageBuffer(300, 300, 32, new BlenderBGRA());
//            testImage.NewGraphics2D().DrawString("\r", 30, 30);
//            Assert.IsTrue(true, "We can print only a \\r");
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
//                ranTests = true;
//                FontTests test = new FontTests();
//                test.CanPrintTests();
//            }
//        }
//    }
//}
