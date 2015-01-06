using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;

namespace LayoutFarm.Boxes
{
    public interface IUpdateChangeListener
    {
        void AddUpdatedImageBinder(ImageBinder binder);
    }
}