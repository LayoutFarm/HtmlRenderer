//MS-PL, 
//Apache2, 2014-present, WinterDev

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