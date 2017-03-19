//Apache2, 2014-2017, WinterDev

using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class UIFloatWindow : EaseBox, ITopWindowBox
    {
        IPlatformWindowBox platformWindowBox;
        public UIFloatWindow(int w, int h)
            : base(w, h)
        {
        }
        IPlatformWindowBox ITopWindowBox.PlatformWinBox
        {
            get { return this.platformWindowBox; }
            set
            {
                bool isFirstTime = this.platformWindowBox == null;
                this.platformWindowBox = value;
                if (isFirstTime)
                {
                    platformWindowBox.Visible = this.Visible;
                    platformWindowBox.SetLocation(this.Left, this.Top);
                    platformWindowBox.SetSize(this.Width, this.Height);
                }
            }
        }

        public override void Walk(UIVisitor visitor)
        {
            //TODO: implement this 
        }
        public override void SetLocation(int left, int top)
        {
            if (platformWindowBox != null)
            {
                //use location relative to top parent/control
                platformWindowBox.SetLocation(left, top);
            }
            else
            {
                base.SetLocation(left, top);
            }
        }
        public override void SetSize(int width, int height)
        {
            if (platformWindowBox != null)
            {
                platformWindowBox.SetSize(width, height);
            }
        }
        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                if (platformWindowBox != null)
                {
                    platformWindowBox.Visible = value;
                }
                base.Visible = value;
            }
        }
    }
}