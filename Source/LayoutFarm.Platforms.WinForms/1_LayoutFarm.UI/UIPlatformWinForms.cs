// 2015,2014 ,Apache2, WinterDev

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
            System.Windows.Forms.Clipboard.Clear();
        }
        public override string GetClipboardData()
        {
            return System.Windows.Forms.Clipboard.GetText();
        }
        public override void SetClipboardData(string textData)
        {
            System.Windows.Forms.Clipboard.SetText(textData);
        }


    }

}