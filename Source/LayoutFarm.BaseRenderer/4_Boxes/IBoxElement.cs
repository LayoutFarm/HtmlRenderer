// 2015,2014 ,Apache2, WinterDev
 
namespace LayoutFarm
{
    public interface IBoxElement
    {
        void ChangeElementSize(int w, int h);
        int MinHeight { get; }
    }

}