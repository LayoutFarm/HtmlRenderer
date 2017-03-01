//Apache2, 2014-2017, WinterDev
using System.Windows.Forms;
using PixelFarm.Drawing;

namespace LayoutFarm.UI
{
    public static class FormCanvasHelper
    {

        public static Form CreateNewFormCanvas(
            int w, int h,
            InnerViewportKind internalViewportKind,
            out LayoutFarm.UI.UISurfaceViewportControl canvasViewport)
        {

            MyRootGraphic myRootGfx = new MyRootGraphic(
                LayoutFarm.UI.UIPlatformWinForm.platform,
                LayoutFarm.UI.UIPlatformWinForm.platform.GetIFonts(),
                w, h);

            Form form1 = new Form();
            var innerViewport = canvasViewport = new LayoutFarm.UI.UISurfaceViewportControl();
            Rectangle screenClientAreaRect = Conv.ToRect(Screen.PrimaryScreen.WorkingArea);

            canvasViewport.InitRootGraphics(myRootGfx, myRootGfx.TopWinEventPortal, internalViewportKind);
            canvasViewport.Bounds =
                new System.Drawing.Rectangle(0, 0,
                    screenClientAreaRect.Width,
                    screenClientAreaRect.Height);
            //---------------------- 
            form1.Controls.Add(canvasViewport);
            //----------------------
            MakeFormCanvas(form1, canvasViewport);

            form1.SizeChanged += (s, e) =>
            {
                if (form1.WindowState == FormWindowState.Maximized)
                {
                    Screen currentScreen = GetScreenFromX(form1.Left);
                    //make full screen ?
                    if (innerViewport != null)
                    {
                        innerViewport.Size = currentScreen.WorkingArea.Size;
                    }
                }
            };
            //----------------------
            return form1;

        }
        public static void MakeFormCanvas(Form form1, LayoutFarm.UI.UISurfaceViewportControl surfaceViewportControl)
        {
            form1.FormClosing += (s, e) =>
            {
                surfaceViewportControl.Close();
            };

        }

        static Screen GetScreenFromX(int xpos)
        {
            Screen[] allScreens = Screen.AllScreens;
            int j = allScreens.Length;
            int accX = 0;
            for (int i = 0; i < j; ++i)
            {
                Screen sc1 = allScreens[i];
                if (accX + sc1.WorkingArea.Width > xpos)
                {
                    return sc1;
                }
            }
            return Screen.PrimaryScreen;
        }
    }
}