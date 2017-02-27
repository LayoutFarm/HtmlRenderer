//MIT, 2014-2017, WinterDev

using LayoutFarm.UI.WinNeutral;
using LayoutFarm.WebDom;
using PixelFarm.Forms;

namespace LayoutFarm.Ease
{
    public class EaseViewport
    {

        UserHtmlWorkspace userWorkspace = new UserHtmlWorkspace();
        UISurfaceViewportControl viewportControl;
        SampleViewport sampleViewport;
        internal EaseViewport(UISurfaceViewportControl viewportControl)
        {
            this.viewportControl = viewportControl;
            this.sampleViewport = new SampleViewport(viewportControl);
        }
        /// <summary>
        /// example logical viewport
        /// </summary>
        public SampleViewport SampleViewPort
        {
            get { return this.sampleViewport; }
        }

        /// <summary>
        /// viewport's window control ( LayoutFarm.UI.UISurfaceViewportControl) 
        /// </summary>
        public Control PhysicalViewportControl
        {
            get { return this.viewportControl; }
        }
        /// <summary>
        /// notify that the host is ready
        /// </summary>
        public void Ready()
        {
            userWorkspace.OnViewportReady(sampleViewport);
        }
        public string RootDir { get; set; }
        /// <summary>
        /// load html string
        /// </summary>
        /// <param name="rootdir">root dir for solve other resource</param>
        /// <param name="htmlText">raw html text</param>
        public void LoadHtmlString(string rootdir, string htmlText)
        {
            userWorkspace.LoadHtml(rootdir, htmlText);
            viewportControl.PaintMeFullMode();
        }
        public WebDocument GetHtmlDom()
        {
            return userWorkspace.GetHtmlDom();
        }
        //public void Print(PixelFarm.Drawing.WinGdi.MyGdiPlusCanvas canvas)
        //{
        //    viewportControl.PrintMe(canvas);
        //}
        public UISurfaceViewportControl UISurfaceViewport
        {
            get { return this.viewportControl; }
        }
    }
}
