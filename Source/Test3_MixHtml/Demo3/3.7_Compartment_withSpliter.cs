//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    [DemoNote("3.7 Demo_CompartmentWithSpliter")]
    class Demo_CompartmentWithSpliter : DemoBase
    {
        NinespaceBox ninespaceBox;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            //--------------------------------
            {
                //background element
                var bgbox = new LayoutFarm.CustomWidgets.SimpleBox(800, 600);
                bgbox.BackColor = Color.White;
                bgbox.SetLocation(0, 0);
                SetupBackgroundProperties(bgbox);
                viewport.AddContent(bgbox);
            }
            //--------------------------------
            //ninespace compartment
            ninespaceBox = new NinespaceBox(800, 600);
            ninespaceBox.ShowGrippers = true;
            viewport.AddContent(ninespaceBox);
            ninespaceBox.SetSize(800, 600);
        }
        void SetupBackgroundProperties(LayoutFarm.CustomWidgets.EaseBox backgroundBox)
        {
        }
    }
}