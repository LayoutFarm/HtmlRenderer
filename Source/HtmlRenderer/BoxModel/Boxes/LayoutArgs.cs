//BSD 2014, WinterCore

using System;
namespace HtmlRenderer.Dom
{

    public class LayoutArgs
    {
        public LayoutArgs(IGraphics gfx)
        {
            this.Gfx = gfx;
        }
        public IGraphics Gfx
        {
            get;
            private set;
        }
    }
}