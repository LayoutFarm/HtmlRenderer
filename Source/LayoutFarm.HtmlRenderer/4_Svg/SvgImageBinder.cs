//MS-PL, 
//Apache2, 2014-2017, WinterDev

namespace LayoutFarm.Svg
{
    public class SvgImageBinder : LayoutFarm.ImageBinder
    {
        string imgsrc;
        public SvgImageBinder(string imgsrc)
        {
            this.imgsrc = imgsrc;
        }
    }
}