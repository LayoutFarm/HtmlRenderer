//Apache2, 2014-2017, WinterDev

namespace LayoutFarm.UI
{
    public class UIPlatformWinForm : UIPlatform
    {
        private UIPlatformWinForm()
        {
            //set up winform platform 
            LayoutFarm.UI.Clipboard.SetUIPlatform(this);
            //gdi+
            PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetFontLoader(YourImplementation.BootStrapWinGdi.myFontLoader);
            PixelFarm.Drawing.WinGdi.WinGdiFontFace.SetFontLoader(YourImplementation.BootStrapWinGdi.myFontLoader);

            //gles2
            OpenTK.Toolkit.Init();
            PixelFarm.Drawing.GLES2.GLES2Platform.SetFontLoader(YourImplementation.BootStrapOpenGLES2.myFontLoader);
            //skia 
            if (!YourImplementation.BootStrapSkia.IsNativeLibAvailable())
            {
                //handle  when native dll is not ready
            }
            else
            {
                //when ready
                PixelFarm.Drawing.Skia.SkiaGraphicsPlatform.SetFontLoader(YourImplementation.BootStrapSkia.myFontLoader);
            }

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


        PixelFarm.Drawing.WinGdi.Gdi32IFonts _gdiPlusIFonts = new PixelFarm.Drawing.WinGdi.Gdi32IFonts();
        public PixelFarm.Drawing.IFonts GetIFonts()
        {
            return this._gdiPlusIFonts;
        }
        public readonly static UIPlatformWinForm platform = new UIPlatformWinForm();

    }
}