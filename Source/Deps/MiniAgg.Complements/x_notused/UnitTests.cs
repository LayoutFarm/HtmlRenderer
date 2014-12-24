//using System;
//using System.Collections.Generic;
//
//using System.Text;

//using NUnit.Framework;
//using MatterHackers.Agg;
//using MatterHackers.VectorMath;

//namespace MatterHackers.Agg.Image
//{
//    [TestFixture]
//    public class ImageTests
//    {
//        bool ClearAndCheckImage(ImageBuffer image, RGBA_Bytes color)
//        {
//            image.NewGraphics2D().Clear(color);

//            for (int y = 0; y < image.Height; y++)
//            {
//                for (int x = 0; x < image.Width; x++)
//                {
//                    if (image.GetPixel(x, y) != color)
//                    {
//                        return false;
//                    }
//                }
//            }

//            return true;
//        }

//        bool ClearAndCheckImageFloat(ImageBufferFloat image, RGBA_Floats color)
//        {
//            image.NewGraphics2D().Clear(color);

//            switch (image.BitDepth)
//            {
//                case 128:
//                    for (int y = 0; y < image.Height; y++)
//                    {
//                        for (int x = 0; x < image.Width; x++)
//                        {
//                            RGBA_Floats pixelColor = image.GetPixel(x, y);
//                            if (pixelColor != color)
//                            {
//                                return false;
//                            }
//                        }
//                    }
//                    break;

//                default:
//                    throw new NotImplementedException();
//            }

//            return true;
//        }

//        [Test]
//        public void ClearTests()
//        {
//            {
//                ImageBuffer clearSurface24 = new ImageBuffer(50, 50, 24, new BlenderBGR());
//                Assert.IsTrue(ClearAndCheckImage(clearSurface24, RGBA_Bytes.White), "Clear 24 to white");
//                Assert.IsTrue(ClearAndCheckImage(clearSurface24, RGBA_Bytes.Black), "Clear 24 to black");

//                ImageBuffer clearSurface32 = new ImageBuffer(50, 50, 32, new BlenderBGRA());
//                Assert.IsTrue(ClearAndCheckImage(clearSurface32, RGBA_Bytes.White), "Clear 32 to white");
//                Assert.IsTrue(ClearAndCheckImage(clearSurface32, RGBA_Bytes.Black), "Clear 32 to black");
//                Assert.IsTrue(ClearAndCheckImage(clearSurface32, new RGBA_Bytes(0, 0, 0, 0)), "Clear 32 to nothing");

//                ImageBufferFloat clearSurface3ComponentFloat = new ImageBufferFloat(50, 50, 128, new BlenderBGRAFloat());
//                Assert.IsTrue(ClearAndCheckImageFloat(clearSurface3ComponentFloat, RGBA_Floats.White), "Clear float to white");
//                Assert.IsTrue(ClearAndCheckImageFloat(clearSurface3ComponentFloat, RGBA_Floats.Black), "Clear float to black");
//                Assert.IsTrue(ClearAndCheckImageFloat(clearSurface3ComponentFloat, new RGBA_Floats(0, 0, 0, 0)), "Clear float to nothing");
//            }
//        }

//        public void ContainsTests()
//        {
//            // look for 24 bit
//            {
//                ImageBuffer imageToSearch = new ImageBuffer(150, 150, 24, new BlenderBGR());
//                imageToSearch.NewGraphics2D().Circle(new Vector2(100, 100), 3, RGBA_Bytes.Red);
//                ImageBuffer circleToFind = new ImageBuffer(10, 10, 24, new BlenderBGR());
//                circleToFind.NewGraphics2D().Circle(new Vector2(5, 5), 3, RGBA_Bytes.Red);
//                Assert.IsTrue(imageToSearch.Contains(circleToFind), "We should be able to find the circle.");

//                ImageBuffer squareToFind = new ImageBuffer(10, 10, 24, new BlenderBGR());
//                squareToFind.NewGraphics2D().FillRectangle(4, 4, 8, 8, RGBA_Bytes.Red);
//                Assert.IsTrue(!imageToSearch.Contains(squareToFind), "We should be not find a square.");
//            }

//            // look for 32 bit
//            {
//                ImageBuffer imageToSearch = new ImageBuffer(150, 150, 32, new BlenderBGRA());
//                imageToSearch.NewGraphics2D().Circle(new Vector2(100, 100), 3, RGBA_Bytes.Red);
//                ImageBuffer circleToFind = new ImageBuffer(10, 10, 32, new BlenderBGRA());
//                circleToFind.NewGraphics2D().Circle(new Vector2(5, 5), 3, RGBA_Bytes.Red);
//                Assert.IsTrue(imageToSearch.Contains(circleToFind), "We should be able to find the circle.");


//                ImageBuffer squareToFind = new ImageBuffer(10, 10, 32, new BlenderBGRA());
//                squareToFind.NewGraphics2D().FillRectangle(4, 4, 8, 8, RGBA_Bytes.Red);
//                Assert.IsTrue(!imageToSearch.Contains(squareToFind), "We should be not find a square.");
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
//                ranTests = true;
//                ImageTests test = new ImageTests();
//                test.ClearTests();
//                test.ContainsTests();
//            }
//        }
//    }
//}
