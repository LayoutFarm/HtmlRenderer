//2014,2015 BSD,WinterDev   


namespace PixelFarm.Agg
{
    public abstract class Scanline
    {
        protected byte[] m_covers;
        protected ScanlineSpan[] m_spans;
        protected int last_span_index;
        protected int m_cover_index;
        protected int lineY;
        protected int last_x;
        public Scanline()
        {
            last_x = (0x7FFFFFF0);
            m_covers = new byte[1000];
            m_spans = new ScanlineSpan[1000];
        }
        public ScanlineSpan GetSpan(int index)
        {
            return m_spans[index];
        }
        public int SpanCount
        {
            get { return last_span_index; }
        }

        public void CloseLine(int y)
        {
            lineY = y;
        }
        public int Y { get { return lineY; } }
        public byte[] GetCovers()
        {
            return m_covers;
        }

        //---------------------------------------------------
        public abstract void AddCell(int x, int cover);
        public abstract void AddSpan(int x, int len, int cover);
        public abstract void ResetSpans(int min_x, int max_x);
        public abstract void ResetSpans();
    }
}
