//Apache2, 2014-present, WinterDev

namespace LayoutFarm.DzBoardSample
{
    [DemoNote("7.1 Demo DesignBoard")]
    class Demo_DzBoard : App
    {
        AppModule appModule = new AppModule();
        protected override void OnStart(AppHost host)
        {
            appModule.StartModule(host);
        }
    }
}