// 2015,2014 ,BSD, WinterDev

//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007-2011
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
//
// Class StringPrinter.cs
// 
// Class to output the vertex source of a string as a run of glyphs.
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using PixelFarm.VectorMath;
namespace PixelFarm.Agg.Fonts
{
    public class MyTypeFacePrinter
    {
        Vector2 totalSizeCach;
        string textToPrint;
        Font currentFont;
        public MyTypeFacePrinter()
        {
            this.Baseline = Baseline.Text;
            this.Justification = Justification.Left;
        }
        public Font CurrentFont
        {
            get { return this.currentFont; }
            set
            {
                this.currentFont = value;
            }
        }
        public Justification Justification { get; set; }
        public Baseline Baseline { get; set; }
        public bool DrawFromHintedCache { get; set; }


        public VertexStore MakeVxs()
        {
            return VertexStoreBuilder.CreateVxs(this.GetVertexIter(textToPrint));
        }
        public VertexStoreSnap MakeVertexSnap()
        {
            return new VertexStoreSnap(this.MakeVxs());
        }

        public void LoadText(string textToPrint)
        {
            this.textToPrint = textToPrint;
        }


        void RenderFromCache(Graphics2D graphics2D, double x, double y, string text)
        {
            //if (text != null && text.Length > 0)
            //{
            //    Vector2 currentOffset = Vector2.Zero;

            //    currentOffset = GetBaseline(currentOffset);
            //    currentOffset.y += Origin.y;

            //    string[] lines = text.Split('\n');
            //    foreach (string line in lines)
            //    {
            //        currentOffset = GetXPositionForLineBasedOnJustification(currentOffset, line);
            //        currentOffset.x += Origin.x;

            //        for (int currentChar = 0; currentChar < line.Length; currentChar++)
            //        {
            //            ImageReaderWriterBase currentGlyphImage = typeFaceStyle.GetImageForCharacter(line[currentChar], 0, 0);

            //            if (currentGlyphImage != null)
            //            {
            //                graphics2D.Render(currentGlyphImage, currentOffset.x, currentOffset.y);
            //            }

            //            // get the advance for the next character
            //            if (currentChar < line.Length - 1)
            //            {
            //                // pass the next char so the typeFaceStyle can do kerning if it needs to.
            //                currentOffset.x += typeFaceStyle.GetAdvanceForCharacter(line[currentChar], line[currentChar + 1]);
            //            }
            //            else
            //            {
            //                currentOffset.x += typeFaceStyle.GetAdvanceForCharacter(line[currentChar]);
            //            }
            //        }

            //        // before we go onto the next line we need to move down a line
            //        currentOffset.x = 0;
            //        currentOffset.y -= typeFaceStyle.EmSizeInPixels;
            //    }
            //}
        }

        IEnumerable<VertexData> GetVertexIter(string text)
        {
            if (text != null && text.Length > 0)
            {
                Vector2 currentOffset = new Vector2(0, 0);
                currentOffset = GetBaseline(currentOffset);
                string[] lines = text.Split('\n');
                foreach (string line in lines)
                {
                    currentOffset = GetXPositionForLineBasedOnJustification(currentOffset, line);
                    for (int currentChar = 0; currentChar < line.Length; currentChar++)
                    {
                        var currentGlyph = currentFont.GetGlyph(line[currentChar]);
                        if (currentGlyph != null)
                        {
                            //use flatten ?
                            var glyphVxs = currentGlyph.flattenVxs;
                            int j = glyphVxs.Count;
                            for (int i = 0; i < j; ++i)
                            {
                                double x, y;
                                var cmd = glyphVxs.GetVertex(i, out x, out y);
                                if (cmd != VertexCmd.Stop)
                                {
                                    yield return new VertexData(cmd,
                                        (x + currentOffset.x),
                                        (y + currentOffset.y));
                                }
                            }
                        }

                        // get the advance for the next character
                        if (currentChar < line.Length - 1)
                        {
                            // pass the next char so the typeFaceStyle can do kerning if it needs to.
                            currentOffset.x += currentFont.GetAdvanceForCharacter(line[currentChar], line[currentChar + 1]);
                        }
                        else
                        {
                            currentOffset.x += currentFont.GetAdvanceForCharacter(line[currentChar]);
                        }
                    }

                    // before we go onto the next line we need to move down a line
                    currentOffset.x = 0;
                    currentOffset.y -= currentFont.EmSizeInPixels;
                }
            }
            yield return new VertexData(VertexCmd.Stop);
        }

        private Vector2 GetXPositionForLineBasedOnJustification(Vector2 currentOffset, string line)
        {
            Vector2 size = GetSize(line);
            switch (Justification)
            {
                case Justification.Left:
                    currentOffset.x = 0;
                    break;
                case Justification.Center:
                    currentOffset.x = -size.x / 2;
                    break;
                case Justification.Right:
                    currentOffset.x = -size.x;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return currentOffset;
        }

        Vector2 GetBaseline(Vector2 currentOffset)
        {
            switch (Baseline)
            {
                case Baseline.Text:
                    currentOffset.y = 0;
                    break;
                case Baseline.BoundsTop:
                    currentOffset.y = -currentFont.AscentInPixels;
                    break;
                case Baseline.BoundsCenter:
                    currentOffset.y = -currentFont.AscentInPixels / 2;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return currentOffset;
        }

#if true

        public bool IsDynamicVertexGen
        {
            get { return true; }
        }




#else
        public void rewind(int pathId)
        {
            currentChar = 0;
            currentOffset = new Vector2(0, 0);
            if (text != null && text.Length > 0)
            {
                currentGlyph = typeFaceStyle.GetGlyphForCharacter(text[currentChar]);
                if (currentGlyph != null)
                {
                    currentGlyph.rewind(0);
                }
            }
        }

        public ShapePath.FlagsAndCommand vertex(out double x, out double y)
        {
            x = 0;
            y = 0;
            if (text != null && text.Length > 0)
            {
                ShapePath.FlagsAndCommand curCommand = ShapePath.FlagsAndCommand.CommandStop;
                if (currentGlyph != null)
                {
                    curCommand = currentGlyph.vertex(out x, out y);
                }

                double xAlignOffset = 0;
                Vector2 size = GetSize();
                switch (Justification)
                {
                    case Justification.Left:
                        xAlignOffset = 0;
                        break;

                    case Justification.Center:
                        xAlignOffset = -size.x / 2;
                        break;

                    case Justification.Right:
                        xAlignOffset = -size.x;
                        break;

                    default:
                        throw new NotImplementedException();
                }

                double yAlignOffset = 0;
                switch (Baseline)
                {
                    case Baseline.Text:
                        //yAlignOffset = -typeFaceStyle.DescentInPixels;
                        yAlignOffset = 0;
                        break;

                    case Baseline.BoundsTop:
                        yAlignOffset = -typeFaceStyle.AscentInPixels;
                        break;

                    case Baseline.BoundsCenter:
                        yAlignOffset = -typeFaceStyle.AscentInPixels / 2;
                        break;

                    default:
                        throw new NotImplementedException();
                }


                while (curCommand == ShapePath.FlagsAndCommand.CommandStop
                    && currentChar < text.Length - 1)
                {
                    if (currentChar == 0 && text[currentChar] == '\n')
                    {
                        currentOffset.x = 0;
                        currentOffset.y -= typeFaceStyle.EmSizeInPixels;
                    }
                    else
                    {
                        if (currentChar < text.Length)
                        {
                            // pass the next char so the typeFaceStyle can do kerning if it needs to.
                            currentOffset.x += typeFaceStyle.GetAdvanceForCharacter(text[currentChar], text[currentChar + 1]);
                        }
                        else
                        {
                            currentOffset.x += typeFaceStyle.GetAdvanceForCharacter(text[currentChar]);
                        }
                    }

                    currentChar++;
                    currentGlyph = typeFaceStyle.GetGlyphForCharacter(text[currentChar]);
                    if (currentGlyph != null)
                    {
                        currentGlyph.rewind(0);
                        curCommand = currentGlyph.vertex(out x, out y);
                    }
                    else if (text[currentChar] == '\n')
                    {
                        if (currentChar + 1 < text.Length - 1 && (text[currentChar + 1] == '\n') && text[currentChar] != text[currentChar + 1])
                        {
                            currentChar++;
                        }
                        currentOffset.x = 0;
                        currentOffset.y -= typeFaceStyle.EmSizeInPixels;
                    }
                }

                if (ShapePath.is_vertex(curCommand))
                {

                    x += currentOffset.x + xAlignOffset + Origin.x;
                    y += currentOffset.y + yAlignOffset + Origin.y;

                }

                return curCommand;
            }

            return ShapePath.FlagsAndCommand.CommandStop;
        }
#endif

        public Vector2 GetSize(string text)
        {
            if (text == null)
            {
                text = this.textToPrint;
            }

            if (text != this.textToPrint)
            {
                Vector2 calculatedSize;
                GetSize(0, Math.Max(0, text.Length - 1), out calculatedSize, text);
                return calculatedSize;
            }

            if (totalSizeCach.x == 0)
            {
                Vector2 calculatedSize;
                GetSize(0, Math.Max(0, text.Length - 1), out calculatedSize, text);
                totalSizeCach = calculatedSize;
            }

            return totalSizeCach;
        }

        public void GetSize(int characterToMeasureStartIndexInclusive,
            int characterToMeasureEndIndexInclusive,
            out Vector2 offset,
            string text)
        {
            if (text == null)
            {
                text = this.textToPrint;
            }

            offset.x = 0;
            offset.y = currentFont.EmSizeInPixels;
            double currentLineX = 0;
            for (int i = characterToMeasureStartIndexInclusive; i < characterToMeasureEndIndexInclusive; i++)
            {
                if (text[i] == '\n')
                {
                    if (i + 1 < characterToMeasureEndIndexInclusive && (text[i + 1] == '\n') && text[i] != text[i + 1])
                    {
                        i++;
                    }
                    currentLineX = 0;
                    offset.y += currentFont.EmSizeInPixels;
                }
                else
                {
                    if (i + 1 < text.Length)
                    {
                        currentLineX += currentFont.GetAdvanceForCharacter(text[i], text[i + 1]);
                    }
                    else
                    {
                        currentLineX += currentFont.GetAdvanceForCharacter(text[i]);
                    }
                    if (currentLineX > offset.x)
                    {
                        offset.x = currentLineX;
                    }
                }
            }

            if (text.Length > characterToMeasureEndIndexInclusive)
            {
                if (text[characterToMeasureEndIndexInclusive] == '\n')
                {
                    currentLineX = 0;
                    offset.y += currentFont.EmSizeInPixels;
                }
                else
                {
                    offset.x += currentFont.GetAdvanceForCharacter(text[characterToMeasureEndIndexInclusive]);
                }
            }
        }
    }
}
