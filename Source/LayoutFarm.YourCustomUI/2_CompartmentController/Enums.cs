//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;

namespace LayoutFarm.UI
{
    public enum VerticalBoxExpansion : byte
    {
        Default,//middel
        Top,
        Bottom
    }
    public enum SpaceName : byte
    {
        Left,
        Top,
        Right,
        Bottom,
        Center,
        LeftTop,
        LeftBottom,
        RightTop,
        RightBottom
    }
    public enum SpaceConcept : byte
    {
        TwoSpaceVertical,
        TwoSpaceHorizontal,
        ThreeSpaceVertical,
        ThreeSpaceHorizontal,
        FourSpace,
        FiveSpace,
        NineSpace,
        NineSpaceFree
    }
    public enum NamedSpaceContainerOverlapMode : byte
    {
        Outer,
        Inner,
        Middle
    }
}