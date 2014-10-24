//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using LayoutFarm;
using LayoutFarm.UI;


namespace TestGraphicPackage
{
    public partial class Form1 : Form
    {
        UIPlatform uiPlatformWinForm;
        public Form1()
        {
            InitializeComponent();
            uiPlatformWinForm = new LayoutFarm.UI.WinForm.UIPlatformWinForm();
        }

        static void ShowFormLayoutInspector(UISurfaceViewportControl viewport)
        {

            var formLayoutInspector = new LayoutFarm.Dev.FormLayoutInspector();
            formLayoutInspector.Show();

            formLayoutInspector.FormClosed += (s, e2) =>
            {
                formLayoutInspector = null;
            };
            formLayoutInspector.Connect(viewport);
            formLayoutInspector.Show();

        }
        private void cmdShowBasicFormCanvas_Click(object sender, EventArgs e)
        {

            UISurfaceViewportControl viewport;
            UITimer wintimer = uiPlatformWinForm.CreateUITimer();

            int w = 800;
            int h = 600;
            MyRootGraphic rootgfx = new MyRootGraphic(
                LayoutFarm.Drawing.WinGdiPortal.P,
                wintimer, w, h);

            var topWin = new TopWindowRenderBox(rootgfx, w, h);
            Form formCanvas = FormCanvasHelper.CreateNewFormCanvas(topWin, new WinEventBridge(topWin), out viewport);

            viewport.PaintMe();
            formCanvas.Show();
            ShowFormLayoutInspector(viewport);
        }

        private void cmdShowEmbededViewport_Click(object sender, EventArgs e)
        {
            Form simpleForm = new Form();
            simpleForm.Text = "SimpleForm2";
            simpleForm.WindowState = FormWindowState.Maximized;
            Rectangle screenClientAreaRect = Screen.PrimaryScreen.WorkingArea;
            UISurfaceViewportControl viewport = new UISurfaceViewportControl();
            viewport.Bounds = new Rectangle(0, 0, screenClientAreaRect.Width, screenClientAreaRect.Height);
            simpleForm.Controls.Add(viewport);

            int w = 800;
            int h = 600;
            UITimer wintimer = uiPlatformWinForm.CreateUITimer();
            MyRootGraphic rootgfx = new MyRootGraphic(
                LayoutFarm.Drawing.WinGdiPortal.P,
                wintimer, w, h);

            var topWin = new TopWindowRenderBox(rootgfx, w, h);
            viewport.InitRootGraphics(topWin, new WinEventBridge(topWin), rootgfx);
            viewport.PaintMe();

            simpleForm.Show();

            ShowFormLayoutInspector(viewport);
        }
    }
}
