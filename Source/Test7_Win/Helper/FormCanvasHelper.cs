//Apache2, 2014-2017, WinterDev
using LayoutFarm.UI.WinNeutral;
using PixelFarm.Drawing;
using PixelFarm.Forms;
namespace LayoutFarm.UI
{
    public static class FormCanvasHelper
    {

        public static Form CreateNewFormCanvas(
            MyRootGraphic myRootGfx,
            InnerViewportKind internalViewportKind,
            out UISurfaceViewportControl canvasViewport)
        {


            Form form1 = new Form();
            var innerViewport = canvasViewport = new UISurfaceViewportControl();
            Rectangle screenClientAreaRect = new Rectangle(0, 0, 800, 600); //Conv.ToRect(Screen.PrimaryScreen.WorkingArea);

            canvasViewport.InitRootGraphics(myRootGfx, myRootGfx.TopWinEventPortal, internalViewportKind);
            canvasViewport.Bounds =
                new Rectangle(0, 0,
                    screenClientAreaRect.Width,
                    screenClientAreaRect.Height);
            //---------------------- 
            form1.Controls.Add(canvasViewport);
            //----------------------
            MakeFormCanvas(form1, canvasViewport);

            //TODO: review here
            //form1.SizeChanged += (s, e) =>
            //{
            //    if (form1.WindowState == FormWindowState.Maximized)
            //    {
            //        Screen currentScreen = GetScreenFromX(form1.Left);
            //        //make full screen ?
            //        if (innerViewport != null)
            //        {
            //            innerViewport.Size = currentScreen.WorkingArea.Size;
            //        }
            //    }
            //};
            //----------------------
            return form1;

        }
        public static void MakeFormCanvas(Form form1, UISurfaceViewportControl surfaceViewportControl)
        {
            //form1.FormClosing += (s, e) =>
            //{
            //    surfaceViewportControl.Close();
            //};

        }

        //static Screen GetScreenFromX(int xpos)
        //{
        //    Screen[] allScreens = Screen.AllScreens;
        //    int j = allScreens.Length;
        //    int accX = 0;
        //    for (int i = 0; i < j; ++i)
        //    {
        //        Screen sc1 = allScreens[i];
        //        if (accX + sc1.WorkingArea.Width > xpos)
        //        {
        //            return sc1;
        //        }
        //    }
        //    return Screen.PrimaryScreen;
        //}
    }
}