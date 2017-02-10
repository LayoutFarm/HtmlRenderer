//Apache2, 2014-2017, WinterDev

namespace LayoutFarm
{
    public interface IBoxElement
    {
        void ChangeElementSize(int w, int h);
        int MinHeight { get; }
    }
}