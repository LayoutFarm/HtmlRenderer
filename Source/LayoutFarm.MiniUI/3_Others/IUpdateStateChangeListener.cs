using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;

namespace LayoutFarm
{
    public interface IUpdateStateChangedListener
    {
        void AddRequestImageBinderUpdate(ImageBinder binder);
    }
}