//----------------------------------- 

using System;
namespace PixelFarm.Agg.Fonts
{
    public abstract class Font : IDisposable
    {
        protected abstract void OnDispose();
        public abstract FontGlyph GetGlyphByIndex(uint glyphIndex);
        public abstract FontGlyph GetGlyph(char c);
        public abstract FontFace FontFace { get; }
        public abstract void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs);
        public abstract int EmSizeInPixels { get; }

        public abstract int GetAdvanceForCharacter(char c);
        public abstract int GetAdvanceForCharacter(char c, char next_c);
        public abstract double AscentInPixels { get; }
        public abstract double DescentInPixels { get; }
        public abstract double XHeightInPixels { get; }
        public abstract double CapHeightInPixels { get; }


        public void Dispose()
        {
            OnDispose();
        }

        ~Font()
        {
            Dispose();
        }

        public abstract bool IsAtlasFont { get; }
    }
}