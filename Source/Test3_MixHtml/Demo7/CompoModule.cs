//Apache2, 2014-present, WinterDev

namespace LayoutFarm.DzBoardSample
{
    public abstract class CompoModule
    {
        protected HtmlBoxes.HtmlHost htmlHost;
        protected AppHost _host;
        public void StartModule(HtmlBoxes.HtmlHost htmlHost, AppHost host)
        {
            this.htmlHost = htmlHost;
            this._host = host;
            OnStartModule();
        }
        public RootGraphic RootGfx { get { return _host.RootGfx; } }
        protected virtual void OnStartModule()
        {
        }
    }
}