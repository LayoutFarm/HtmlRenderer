using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{
    public interface IUpdateStateChangedListener
    {
        void AddRequestImageBinderUpdate(ImageBinder binder);
    }
}