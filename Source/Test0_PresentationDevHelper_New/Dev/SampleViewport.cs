//Apache2, 2014-2017, WinterDev
using System;
using PixelFarm.Drawing;

namespace LayoutFarm
{
    public class SampleViewport
    {
        LayoutFarm.UI.UISurfaceViewportControl vw;
        int primaryScreenWorkingAreaW;
        int primaryScreenWorkingAreaH;
        public SampleViewport(LayoutFarm.UI.UISurfaceViewportControl vw)
        {
            this.vw = vw;
            var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            this.primaryScreenWorkingAreaW = workingArea.Width;
            this.primaryScreenWorkingAreaH = workingArea.Height;
        }
        public LayoutFarm.UIPlatform Platform
        {
            get
            {
                return vw.Platform;
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

        public LayoutFarm.UI.UISurfaceViewportControl ViewportControl
        {
            get { return this.vw; }
        }
        public RootGraphic Root
        {
            get { return this.vw.RootGfx; }
        }
    }
}