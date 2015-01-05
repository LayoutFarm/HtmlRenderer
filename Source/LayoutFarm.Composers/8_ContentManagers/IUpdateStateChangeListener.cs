using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;

namespace HtmlRenderer.ContentManagers
{
    public interface IUpdateChangeListener
    {
        void AddUpdatedImageBinder(ImageBinder binder);
    }
}