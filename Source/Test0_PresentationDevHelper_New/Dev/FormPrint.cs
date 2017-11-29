using System;
using System.Drawing;
using System.Windows.Forms;

namespace LayoutFarm.Dev
{
    public partial class FormPrint : Form
    {
        LayoutFarm.UI.UISurfaceViewportControl vwport;
        public FormPrint()
        {
            InitializeComponent();
        }
        public void Connect(LayoutFarm.UI.UISurfaceViewportControl vwport)
        {
#if DEBUG
            this.vwport = vwport;
#endif
            this.TopMost = true;
        }
        private void cmdPrint_Click(object sender, EventArgs e)
        {
            //using (var bmp = new Bitmap(800, 600, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            //using (var g = System.Drawing.Graphics.FromImage(bmp))
            //{

            //    //var canvasInit = new PixelFarm.Drawing.CanvasInitParameters();
            //    //canvasInit.externalCanvas = g;
            //    var canvas = new PixelFarm.Drawing.WinGdi.MyGdiPlusCanvas(0, 0, 800, 600); ;// LayoutFarm.UI.GdiPlus.MyWinGdiPortal.P.CreateCanvas(0, 0, 800, 600, canvasInit);
            //    vwport.PrintMe(canvas);
            //    bmp.Save("d:\\WImageTest\\testhtml.bmp");
            // }

        }

        private void cmdPrintToPrinter_Click(object sender, EventArgs e)
        {
            //System.Drawing.Printing.PrintDocument printdoc = new System.Drawing.Printing.PrintDocument();
            //printdoc.PrintPage += (e2, s2) =>
            //{
            //    var g = s2.Graphics;
            //    //var canvasInit = new PixelFarm.Drawing.CanvasInitParameters();
            //    //canvasInit.externalCanvas = g;
            //    var canvas = new PixelFarm.Drawing.WinGdi.MyGdiPlusCanvas(0, 0, 800, 600);
            //    vwport.PrintMe(canvas);
            //};
            //printdoc.Print();
        }
    }
}
