// 2015,2014 ,Apache2, WinterDev
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
        public GraphicsPlatform P
        {
            get { return this.vw.P; }
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
    public abstract class DemoBase
    {
        public void StartDemo(SampleViewport viewport)
        {
            OnStartDemo(viewport);
        }
        protected virtual void OnStartDemo(SampleViewport viewport)
        {
        }
        public virtual string Desciption
        {
            get { return ""; }
        }
    }
    public class DemoNoteAttribute : Attribute
    {
        public DemoNoteAttribute(string msg)
        {
            this.Message = msg;
        }
        public string Message { get; set; }
    }
    class DemoInfo
    {
        public readonly Type DemoType;
        public readonly string DemoNote;
        public DemoInfo(Type demoType, string demoNote)
        {
            this.DemoType = demoType;
            this.DemoNote = demoNote;
        }
        public override string ToString()
        {
            if (string.IsNullOrEmpty(DemoNote))
            {
                return this.DemoType.Name;
            }
            else
            {
                return this.DemoNote + " : " + this.DemoType.Name;
            }
        }
    }

}