//MIT, 2014-2016, WinterDev

using System;
namespace PixelFarm.Drawing.Fonts
{
    /// <summary>
    /// specific fontface + size + style
    /// </summary>
    public abstract class ActualFont : IDisposable
    {

        public abstract string Name { get; }
        public abstract int Height { get; }
        /// <summary>
        /// emheight
        /// </summary>
        public abstract float EmSize { get; }
        public abstract int EmSizeInPixels { get; }
        public abstract FontStyle Style { get; }


        public void Dispose()
        {
            OnDispose();
        }
#if DEBUG
        static int dbugTotalId = 0;
        public readonly int dbugId = dbugTotalId++;
        public ActualFont()
        {

        }
#endif
        protected abstract void OnDispose();
        public abstract FontGlyph GetGlyphByIndex(uint glyphIndex);
        public abstract FontGlyph GetGlyph(char c);
        public abstract FontFace FontFace { get; }
        public abstract void GetGlyphPos(char[] buffer,
            int start,
            int len,
            ProperGlyph[] properGlyphs);
        public abstract int GetAdvanceForCharacter(char c);
        public abstract int GetAdvanceForCharacter(char c, char next_c);
        public abstract double AscentInPixels { get; }
        public abstract double DescentInPixels { get; }
        public abstract double XHeightInPixels { get; }
        public abstract double CapHeightInPixels { get; }

        ~ActualFont()
        {
            Dispose();
        }
    }

    public abstract class PlatformFont : ActualFont { }
    public abstract class OutlineFont : ActualFont { }
}