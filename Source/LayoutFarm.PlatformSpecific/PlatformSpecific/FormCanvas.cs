//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;




namespace LayoutFarm.Presentation
{
    public partial class FormCanvas : Form
    {

        ArtSurfaceViewportControl artUISurfaceViewport1;

        public FormCanvas()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.SizeChanged += new EventHandler(FormCanvas_SizeChanged);
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
        void FormCanvas_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                Screen currentScreen = GetScreenFromX(this.Left);
                if (artUISurfaceViewport1 != null)
                {
                    artUISurfaceViewport1.Size = currentScreen.WorkingArea.Size;
                    artUISurfaceViewport1.UpdateRootdocViewportSize();
                }
            }
        }
        protected sealed override void OnClosing(CancelEventArgs e)
        {
            artUISurfaceViewport1.Close();
        }
        public void InitViewport()
        {
            Rectangle screenClientAreaRect = Screen.PrimaryScreen.WorkingArea;
            artUISurfaceViewport1 = new ArtSurfaceViewportControl();
            artUISurfaceViewport1.Bounds = new Rectangle(0, 0, screenClientAreaRect.Width, screenClientAreaRect.Height);
            this.Controls.Add(artUISurfaceViewport1);
        }


        public ArtSurfaceViewportControl SurfaceViewport
        {
            get
            {
                return artUISurfaceViewport1;
            }
        }



    }
}
