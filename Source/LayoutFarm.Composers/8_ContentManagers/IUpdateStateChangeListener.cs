using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;

namespace HtmlRenderer.ContentManagers
{
    public interface IUpdateStateChangedListener
    {
        void AddRequestImageBinderUpdate(ImageBinder binder);
    }
}