//Apache2, 2014-2016, WinterDev

namespace LayoutFarm
{
    public interface IBoxElement
    {
        void ChangeElementSize(int w, int h);
        int MinHeight { get; }
    }
}