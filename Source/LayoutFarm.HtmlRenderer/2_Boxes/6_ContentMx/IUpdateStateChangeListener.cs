using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;

namespace LayoutFarm.HtmlBoxes
{
    public interface IUpdateChangeListener
    {
        void AddUpdatedImageBinder(ImageBinder binder);
    }
}