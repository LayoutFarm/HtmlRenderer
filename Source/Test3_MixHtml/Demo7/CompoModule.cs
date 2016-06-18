// 2015,2014 ,Apache2, WinterDev

namespace LayoutFarm.DzBoardSample
{
    public abstract class CompoModule
    {
        protected HtmlBoxes.HtmlHost htmlHost;
        protected SampleViewport viewport;
        public void StartModule(HtmlBoxes.HtmlHost htmlHost, SampleViewport viewport)
        {
            this.htmlHost = htmlHost;
            this.viewport = viewport;
            OnStartModule();
        }
        protected virtual void OnStartModule()
        {
        }
    }
}