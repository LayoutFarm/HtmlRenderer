//Apache2, 2014-2016, WinterDev

namespace LayoutFarm.UI
{
    public class UIPlatformWinForm : UIPlatform
    {
        public UIPlatformWinForm()
        {
            LayoutFarm.UI.Clipboard.SetUIPlatform(this);
        }
        public override UITimer CreateUITimer()
        {
            return new MyUITimer();
        }
        public override void ClearClipboardData()
        {
            throw new System.NotSupportedException();
        }
        public override string GetClipboardData()
        {
            throw new System.NotSupportedException();
        }
        public override void SetClipboardData(string textData)
        {
            throw new System.NotSupportedException();
        }
    }
}