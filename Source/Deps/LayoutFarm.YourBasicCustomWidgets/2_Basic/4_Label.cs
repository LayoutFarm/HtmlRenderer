//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class Label : UIBox
    {
        string text;
        Color textColor;
        CustomTextRun myTextRun;
        public Label(int w, int h)
            : base(w, h)
        {
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (this.myTextRun == null)
            {
                var trun = new CustomTextRun(rootgfx, this.Width, this.Height);
                trun.SetLocation(this.Left, this.Top);
                trun.TextColor = this.textColor;
                trun.Text = this.Text;
                this.myTextRun = trun;
            }
            //-----------
            return myTextRun;
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.myTextRun; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this.myTextRun != null; }
        }
        public string Text
        {
            get { return this.text; }
            set
            {
                this.text = value;
                if (this.myTextRun != null)
                {
                    this.myTextRun.Text = value;
                }
            }
        }
        public Color Color
        {
            get { return this.textColor; }
            set
            {
                this.textColor = value;
                if (myTextRun != null)
                {
                    myTextRun.TextColor = value;
                }
            }
        }
        public override int DesiredHeight
        {
            get
            {
                return this.Height;
            }
        }
        public override int DesiredWidth
        {
            get
            {
                return this.Width;
            }
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "label");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}