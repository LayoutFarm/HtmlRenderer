//Apache2, 2014-2017, WinterDev
 
using LayoutFarm.UI.WinNeutral;

namespace LayoutFarm
{
    public class SampleViewport
    {
        UISurfaceViewportControl vw;
        int primaryScreenWorkingAreaW;
        int primaryScreenWorkingAreaH;

        public SampleViewport(UISurfaceViewportControl vw)
        {
            this.vw = vw;
            //  var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            this.primaryScreenWorkingAreaW = 800;// workingArea.Width;
            this.primaryScreenWorkingAreaH = 600;// orkingArea.Height; 
        }
        public LayoutFarm.UI.UIPlatform Platform
        {
            get
            {
                return this.vw.Platform;
            }
        }
        public int PrimaryScreenWidth
        {
            get { return this.primaryScreenWorkingAreaW; }
        }
        public int PrimaryScreenHeight
        {
            get { return this.primaryScreenWorkingAreaH; }
        }
        public void AddContent(RenderElement renderElement)
        {
            this.vw.AddContent(renderElement);
        }

        public UISurfaceViewportControl ViewportControl
        {
            get { return this.vw; }
        }
        public RootGraphic Root
        {
            get { return this.vw.RootGfx; }
        }
    }
}