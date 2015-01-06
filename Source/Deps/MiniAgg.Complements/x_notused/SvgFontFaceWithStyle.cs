////2014,2015 BSD,WinterDev   
////----------------------------------------------------------------------------
//// Anti-Grain Geometry - Version 2.4
////
//// C# Port port by: Lars Brubaker
////                  larsbrubaker@gmail.com
//// Copyright (C) 2007-2011
////
//// Permission to copy, use, modify, sell and distribute this software 
//// is granted provided this copyright notice appears in all copies. 
//// This software is provided "as is" without express or implied
//// warranty, and with no claim as to its suitability for any purpose.
////
////----------------------------------------------------------------------------
////
//// Class StyledTypeFace.cs
////
////----------------------------------------------------------------------------
//using System;
//using System.Collections.Generic;
//using System.Text;

//using PixelFarm.Agg;
//using PixelFarm.Agg.Transform;
//using PixelFarm.Agg.VertexSource;
//using PixelFarm.Agg.Image;

//namespace PixelFarm.Agg.Fonts
//{


//    class StyledTypeFaceImageCache
//    {
//        static StyledTypeFaceImageCache instance;

//        Dictionary<SvgFontFace, Dictionary<double, Dictionary<char, ImageReaderWriterBase>>> typeFaceImageCache = new Dictionary<SvgFontFace, Dictionary<double, Dictionary<char, ImageReaderWriterBase>>>();

//        // private so you can't use it by accident (it is a singlton)
//        StyledTypeFaceImageCache()
//        {
//        }

//        public static Dictionary<char, ImageReaderWriterBase> GetCorrectCache(SvgFontFace typeFace, double emSizeInPoints)
//        {
//            // check if the cache is getting too big and if so prune it (or just delete it and start over).

//            Dictionary<double, Dictionary<char, ImageReaderWriterBase>> foundTypeFaceSizes;
//            Instance.typeFaceImageCache.TryGetValue(typeFace, out foundTypeFaceSizes);
//            if (foundTypeFaceSizes == null)
//            {
//                // add in the type face
//                foundTypeFaceSizes = new Dictionary<double, Dictionary<char, ImageReaderWriterBase>>();
//                Instance.typeFaceImageCache.Add(typeFace, foundTypeFaceSizes);
//            }

//            Dictionary<char, ImageReaderWriterBase> foundTypeFaceSize;
//            foundTypeFaceSizes.TryGetValue(emSizeInPoints, out foundTypeFaceSize);
//            if (foundTypeFaceSize == null)
//            {
//                // add in the point size
//                foundTypeFaceSize = new Dictionary<char, ImageReaderWriterBase>();
//                foundTypeFaceSizes.Add(emSizeInPoints, foundTypeFaceSize);
//            }

//            return foundTypeFaceSize;
//        }

//        static StyledTypeFaceImageCache Instance
//        {
//            get
//            {
//                if (instance == null)
//                {
//                    instance = new StyledTypeFaceImageCache();
//                }

//                return instance;
//            }
//        }
//    }


//    class SvgFontFaceWithStyle : FontFace
//    {
//        SvgFontFace typeFace;

//        const int POINTS_PER_INCH = 72;
//        const int PIXEL_PER_INCH = 96;

//        double emSizeInPixels;
//        double currentEmScalling;
//        bool flatenCurves = true;

//        public SvgFontFaceWithStyle(SvgFontFace typeFace, double emSizeInPoints, bool underline = false, bool flatenCurves = true)
//        {
//            this.typeFace = typeFace;
//            emSizeInPixels = emSizeInPoints / POINTS_PER_INCH * PIXEL_PER_INCH;
//            currentEmScalling = emSizeInPixels / typeFace.UnitsPerEm;
//            DoUnderline = underline;
//            FlatenCurves = flatenCurves;
//        }

//        public bool DoUnderline { get; set; }

//        protected override void OnDispose()
//        {

//        }
//        /// <summary>
//        /// <para>If true the font will have it's curves flattened to the current point size when retrieved.</para>
//        /// <para>You may want to disable this so you can flaten the curve after other transforms have been applied,</para>
//        /// <para>such as skewing or scalling.  Rotation and Translation will not alter how a curve is flattened.</para>
//        /// </summary>
//        public bool FlatenCurves
//        {
//            get
//            {
//                return flatenCurves;
//            }

//            set
//            {
//                flatenCurves = value;
//            }
//        }

//        /// <summary>
//        /// Sets the Em size for the font in pixels.
//        /// </summary>
//        public double EmSizeInPixels
//        {
//            get
//            {
//                return emSizeInPixels;
//            }
//        }

//        /// <summary>
//        /// Sets the Em size for the font assuming there are 72 points per inch and there are 96 pixels per inch.
//        /// </summary>
//        public double EmSizeInPoints
//        {
//            get
//            {
//                return emSizeInPixels / PIXEL_PER_INCH * POINTS_PER_INCH;
//            }
//        }

//        public double AscentInPixels
//        {
//            get
//            {
//                return typeFace.Ascent * currentEmScalling;
//            }
//        }

//        public double DescentInPixels
//        {
//            get
//            {
//                return typeFace.Descent * currentEmScalling;
//            }
//        }

//        public double XHeightInPixels
//        {
//            get
//            {
//                return typeFace.X_height * currentEmScalling;
//            }
//        }

//        public double CapHeightInPixels
//        {
//            get
//            {
//                return typeFace.Cap_height * currentEmScalling;
//            }
//        }

//        public RectD BoundingBoxInPixels
//        {
//            get
//            {
//                RectD pixelBounds = new RectD(typeFace.BoundingBox);
//                pixelBounds *= currentEmScalling;
//                return pixelBounds;
//            }
//        }

//        public double UnderlineThicknessInPixels
//        {
//            get
//            {
//                return typeFace.Underline_thickness * currentEmScalling;
//            }
//        }

//        public double UnderlinePositionInPixels
//        {
//            get
//            {
//                return typeFace.Underline_position * currentEmScalling;
//            }
//        }

//        public ImageReaderWriterBase GetImageForCharacter(char character, double xFraction, double yFraction)
//        {

//            if (xFraction > 1 || xFraction < 0 || yFraction > 1 || yFraction < 0)
//            {
//                throw new ArgumentException("The x and y fractions must both be between 0 and 1.");
//            }

//            ImageReaderWriterBase imageForCharacter;
//            Dictionary<char, ImageReaderWriterBase> characterImageCache = StyledTypeFaceImageCache.GetCorrectCache(this.typeFace, this.emSizeInPixels);
//            characterImageCache.TryGetValue(character, out imageForCharacter);
//            if (imageForCharacter != null)
//            {
//                return imageForCharacter;
//            }

//            var fontGlyph = GetGlyphForCharacter(character);
//            if (fontGlyph == null)
//            {
//                return null;
//            }

//            VertexStore glyphVxs = fontGlyph.flattenVxs;
//            double x, y;


//            int j = glyphVxs.Count;
//            glyphVxs.GetVertex(0, out x, out y);
//            RectD bounds = new RectD(x, y, x, y);
//            for (int i = 0; i < j; ++i)
//            {
//                var cmd = glyphVxs.GetVertex(i, out x, out y);
//                if (cmd == VertexCmd.Stop)
//                {
//                    break;
//                }
//                else
//                {
//                    bounds.ExpandToInclude(x, y);
//                }
//            }

//            ActualImage myCharImage = new ActualImage(
//                Math.Max((int)(bounds.Width + .5), 1),
//                Math.Max((int)(bounds.Height + .5), 1),
//                PixelFormat.Rgba32);

//            MyImageReaderWriter charImage = new MyImageReaderWriter(myCharImage);

//            var gfx = Graphics2D.CreateFromImage(myCharImage);
//            gfx.Render(new VertexStoreSnap(glyphVxs), xFraction, yFraction, ColorRGBA.Black);
//            characterImageCache[character] = charImage;

//            return charImage;
//        }

//        public FontGlyph GetGlyphForCharacter(char character)
//        {
//            // scale it to the correct size.

//            FontGlyph sourceGlyph = typeFace.GetGlyphForCharacter(character);
//            if (sourceGlyph != null)
//            {
//                //if (DoUnderline)
//                //{
//                //    //sourceGlyph = new GlyphWithUnderline(sourceGlyph,
//                //    //    typeFace.GetAdvanceForCharacter(character),
//                //    //    typeFace.Underline_position,
//                //    //    typeFace.Underline_thickness).MakeVxs();
//                //} 
//                Affine glyphTransform = Affine.NewMatix(AffinePlan.Scale(currentEmScalling));
//                VertexStore characterGlyph = glyphTransform.TransformToVxs(sourceGlyph.originalVxs);
//                if (FlatenCurves)
//                {
//                    characterGlyph = new CurveFlattener().MakeVxs(characterGlyph);
//                    sourceGlyph.flattenVxs = characterGlyph;
//                }
//                else
//                {
//                    sourceGlyph.flattenVxs = characterGlyph;
//                }
//                return sourceGlyph;
//            }

//            return null;
//        }

//        public double GetAdvanceForCharacter(char character, char nextCharacterToKernWith)
//        {
//            return typeFace.GetAdvanceForCharacter(character, nextCharacterToKernWith) * currentEmScalling;
//        }

//        public double GetAdvanceForCharacter(char character)
//        {
//            return typeFace.GetAdvanceForCharacter(character) * currentEmScalling;
//        }
//    }
//}
