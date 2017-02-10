//Apache2, 2014-2017, WinterDev

namespace LayoutFarm.UI
{
    public enum VerticalBoxExpansion : byte
    {
        Default = 0,//middel
        Top = 0x1,
        Bottom = 0x2,
        TopBottom = 0x3
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