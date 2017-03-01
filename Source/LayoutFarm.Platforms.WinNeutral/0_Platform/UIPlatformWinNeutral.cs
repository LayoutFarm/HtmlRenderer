//Apache2, 2014-2017, WinterDev

namespace LayoutFarm.UI
{
    public class UIPlatformWinNeutral : UIPlatform
    {
        private UIPlatformWinNeutral()
        {
            LayoutFarm.UI.Clipboard.SetUIPlatform(this);

            //no gdi+

            //gles2 
            
            PixelFarm.Drawing.GLES2.GLES2Platform.SetFontLoader(YourImplementation.BootStrapOpenGLES2.myFontLoader);
            //skia
             
            if (!YourImplementation.BootStrapSkia.IsNativeLibAvailable())
            {
                //set font not found handler
                PixelFarm.Drawing.Skia.SkiaGraphicsPlatform.SetFontLoader(YourImplementation.BootStrapSkia.myFontLoader);

            }
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
        public PixelFarm.Drawing.IFonts GetIFonts()
        {
            throw new System.NotSupportedException();
        }
        public static readonly UIPlatformWinNeutral platform = new UIPlatformWinNeutral();
    }
}