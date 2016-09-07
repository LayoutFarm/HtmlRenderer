//Apache2, 2014-2016, WinterDev

namespace LayoutFarm.DzBoardSample
{
    [DemoNote("7.1 Demo DesignBoard")]
    class Demo_DzBoard : DemoBase
    {
        AppModule appModule = new AppModule();
        protected override void OnStartDemo(SampleViewport viewport)
        {
            appModule.StartModule(viewport);
        }
    }
}