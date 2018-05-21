//MS-PL, 
//Apache2, 2014-2018, WinterDev

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