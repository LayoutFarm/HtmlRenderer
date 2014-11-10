using System;
using System.Collections.Generic;
using System.Text;

using LayoutFarm.Drawing;

using Svg.Pathing;
using Svg.Transforms;

namespace LayoutFarm.SvgDom
{

    public class SvgPath : SvgVisualElement
    {
        SvgPathSpec spec;
        public SvgPath(SvgPathSpec spec, object controller)
            : base(controller)
        {
            this.spec = spec;
        }

    }
}