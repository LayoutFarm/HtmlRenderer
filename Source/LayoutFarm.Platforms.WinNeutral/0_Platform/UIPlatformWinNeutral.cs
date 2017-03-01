//Apache2, 2014-2017, WinterDev

namespace LayoutFarm.UI
{
    public class UIPlatformWinNeutral : UIPlatform
    {
        private UIPlatformWinNeutral()
        {
            LayoutFarm.UI.Clipboard.SetUIPlatform(this);
            PixelFarm.Drawing.GLES2.GLES2Platform.SetFontLoader(YourImplementation.BootStrapOpenGLES2.myFontLoader);

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

        public static readonly UIPlatformWinNeutral platform = new UIPlatformWinNeutral();
    }
}