//Apache2, 2014-present, WinterDev

namespace LayoutFarm.HtmlWidgets
{
    public abstract class WidgetBase
    {
        int _width;
        int _height;
        int _left;
        int _top;
        int _viewportX;
        int _viewportY;
        public WidgetBase(int w, int h)
        {
            _width = w;
            _height = h;
        }
        //
        public virtual int Width => _width;
        //
        public virtual int Height => _height;
        //
        public int Left => _left;
        //
        public int Top => _top;
        //
        public int ViewportX => _viewportX;
        public int ViewportY => _viewportY;
        //
        public int ViewportWidth => this.Width;
        public int ViewportHeight => this.Height;
        //
        public virtual void SetLocation(int left, int top)
        {
            _left = left;
            _top = top;
        }
        public virtual void SetSize(int width, int height)
        {
            _width = width;
            _height = height;
        }
        public virtual void SetViewport(int x, int y)
        {
            _viewportX = x;
            _viewportY = y;
        }
    }
    public abstract class HtmlWidgetBase : WidgetBase
    {
        public HtmlWidgetBase(int w, int h)
            : base(w, h)
        {
        }
        public abstract Composers.HtmlElement GetPresentationDomNode(Composers.HtmlElement orgDomElem);
    }
}