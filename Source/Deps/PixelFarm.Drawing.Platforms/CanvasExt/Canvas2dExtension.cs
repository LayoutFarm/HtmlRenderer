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
using PixelFarm.Agg.Transform;
using PixelFarm.Agg.VertexSource;
using PixelFarm.VectorMath;
using PixelFarm.Agg.Fonts;
namespace PixelFarm.Agg
{
    public static class Canvas2dExtension
    {
        public static void DrawString(this Graphics2D gx,
            string text,
            double x,
            double y,
            double pointSize = 12,
            Justification justification = Justification.Left,
            Baseline baseline = Baseline.Text,
            ColorRGBA color = new ColorRGBA(),
            bool drawFromHintedCache = false,
            ColorRGBA backgroundColor = new ColorRGBA())
        {
            ////use svg font 
            var svgFont = SvgFontStore.LoadFont(SvgFontStore.DEFAULT_SVG_FONTNAME, (int)pointSize);
            var stringPrinter = new MyTypeFacePrinter();
            stringPrinter.CurrentFont = svgFont;
            stringPrinter.DrawFromHintedCache = false;
            stringPrinter.LoadText(text);
            var vxs = stringPrinter.MakeVxs();
            vxs = Affine.NewTranslation(x, y).TransformToVxs(vxs);
            gx.Render(vxs, ColorRGBA.Black);
        }


        public static void Rectangle(this Graphics2D gx, double left, double bottom, double right, double top, ColorRGBA color, double strokeWidth = 1)
        {
            RoundedRect rect = new RoundedRect(left + .5, bottom + .5, right - .5, top - .5, 0);
            gx.Render(new Stroke(strokeWidth).MakeVxs(rect.MakeVxs()), color);
        }

        public static void Rectangle(this Graphics2D gx, RectD rect, ColorRGBA color, double strokeWidth = 1)
        {
            gx.Rectangle(rect.Left, rect.Bottom, rect.Right, rect.Top, color, strokeWidth);
        }

        public static void Rectangle(this Graphics2D gx, RectInt rect, ColorRGBA color)
        {
            gx.Rectangle(rect.Left, rect.Bottom, rect.Right, rect.Top, color);
        }

        public static void FillRectangle(this Graphics2D gx, RectD rect, ColorRGBA fillColor)
        {
            gx.FillRectangle(rect.Left, rect.Bottom, rect.Right, rect.Top, fillColor);
        }

        public static void FillRectangle(this Graphics2D gx, RectInt rect, ColorRGBA fillColor)
        {
            gx.FillRectangle(rect.Left, rect.Bottom, rect.Right, rect.Top, fillColor);
        }

        public static void FillRectangle(this Graphics2D gx, Vector2 leftBottom,
            Vector2 rightTop, ColorRGBA fillColor)
        {
            gx.FillRectangle(leftBottom.x, leftBottom.y, rightTop.x, rightTop.y, fillColor);
        }

        public static void FillRectangle(this Graphics2D gx, double left,
            double bottom, double right, double top, ColorRGBA fillColor)
        {
            if (right < left || top < bottom)
            {
                throw new ArgumentException();
            }
            RoundedRect rect = new RoundedRect(left, bottom, right, top, 0);
            gx.Render(rect.MakeVertexSnap(), fillColor);
        }
        public static void Circle(this Graphics2D g, double x, double y, double radius, ColorRGBA color)
        {
            Ellipse elipse = new Ellipse(x, y, radius, radius);
            g.Render(elipse.MakeVxs(), color);
        }
        public static void Circle(this Graphics2D g, Vector2 origin, double radius, ColorRGBA color)
        {
            Circle(g, origin.x, origin.y, radius, color);
        }
    }
}

