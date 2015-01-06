using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;

namespace HtmlRenderer.Boxes
{
    public interface IUpdateChangeListener
    {
        void AddUpdatedImageBinder(ImageBinder binder);
    }
}