//Apache2, 2014-present, WinterDev

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
        public RootGraphic RootGfx { get { return viewport.RootGfx; } }
        protected virtual void OnStartModule()
        {
        }
    }
}