using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LayoutFarm.UI;
namespace LayoutFarm.Dev
{
    public partial class FormDemoList : Form
    {


        public FormDemoList()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
            //------
            this.cmbPlatformSelector.Items.Add(InnerViewportKind.GdiPlus);
            this.cmbPlatformSelector.Items.Add(InnerViewportKind.Skia);
            this.cmbPlatformSelector.Items.Add(InnerViewportKind.GL);
            this.cmbPlatformSelector.SelectedIndex = 0;//set default


        }
        public TreeView SamplesTreeView
        {
            get { return this._samplesTreeView; }
        }
        void Form1_Load(object sender, EventArgs e)
        {
            this.lstDemoList.DoubleClick += new EventHandler(lstDemoList_DoubleClick);
        }

        void lstDemoList_DoubleClick(object sender, EventArgs e)
        {
            //load demo sample
            var selectedDemoInfo = this.lstDemoList.SelectedItem as DemoInfo;
            if (selectedDemoInfo == null) return;
            //------------------------------------------------------------ 
            DemoBase selectedDemo = (DemoBase)Activator.CreateInstance(selectedDemoInfo.DemoType);
            RunDemo(selectedDemo);
            //------------------------------------------------------------ 
            //LayoutFarm.UI.UISurfaceViewportControl viewport; 
            //Form formCanvas;
            //CreateReadyForm(
            //    out viewport,
            //    out formCanvas);

            //selectedDemo.StartDemo(new SampleViewport(viewport));
            //viewport.TopDownRecalculateContent();
            ////==================================================  
            //viewport.PaintMe();
            //ShowFormLayoutInspector(viewport); 
        }
        LayoutFarm.UI.UISurfaceViewportControl viewport;
        public void RunDemo(DemoBase selectedDemo)
        {   
            Form formCanvas;
            CreateReadyForm(out viewport, out formCanvas);

            selectedDemo.StartDemo(new SampleViewport(viewport));
            viewport.TopDownRecalculateContent();
            //==================================================  
            viewport.PaintMe();

            if (this.chkShowLayoutInspector.Checked)
            {
                ShowFormLayoutInspector(viewport);
            }

            if (this.chkShowFormPrint.Checked)
            {
                ShowFormPrint(viewport);
            }

        }
        static void ShowFormLayoutInspector(LayoutFarm.UI.UISurfaceViewportControl viewport)
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
        static void ShowFormPrint(LayoutFarm.UI.UISurfaceViewportControl viewport)
        {

            var formPrint = new LayoutFarm.Dev.FormPrint();
            formPrint.Show();

            formPrint.FormClosed += (s, e2) =>
            {
                formPrint = null;
            };
            formPrint.Connect(viewport);
        }
        void CreateReadyForm(
            out LayoutFarm.UI.UISurfaceViewportControl viewport,
            out Form formCanvas)
        {

            //1. select view port kind
            InnerViewportKind innerViewportKind = (InnerViewportKind)cmbPlatformSelector.SelectedItem;

            var workingArea = Screen.PrimaryScreen.WorkingArea;

            formCanvas = FormCanvasHelper.CreateNewFormCanvas(
              workingArea.Width,
              workingArea.Height,
              innerViewportKind,
              out viewport);

            formCanvas.Text = "FormCanvas 1 :" + innerViewportKind;

            viewport.PaintMe();

            formCanvas.WindowState = FormWindowState.Maximized;
            formCanvas.Show();
        }

        public void ClearDemoList()
        {
            this.lstDemoList.Items.Clear();
        }

        public void LoadDemoList(Type sampleAssemblySpecificType)
        {


            var demoBaseType = typeof(DemoBase);
            var thisAssem = System.Reflection.Assembly.GetAssembly(sampleAssemblySpecificType);
            List<DemoInfo> demoInfoList = new List<DemoInfo>();
            foreach (var t in thisAssem.GetTypes())
            {
                if (demoBaseType.IsAssignableFrom(t) && t != demoBaseType && !t.IsAbstract)
                {
                    string demoTypeName = t.Name;
                    object[] notes = t.GetCustomAttributes(typeof(DemoNoteAttribute), false);
                    string noteMsg = null;
                    if (notes != null && notes.Length > 0)
                    {
                        //get one only
                        DemoNoteAttribute note = notes[0] as DemoNoteAttribute;
                        if (note != null)
                        {
                            noteMsg = note.Message;
                        }
                    }
                    demoInfoList.Add(new DemoInfo(t, noteMsg));
                }
            }
            demoInfoList.Sort((d1, d2) =>
            {
                if (d1.DemoNote != null && d2.DemoNote != null)
                {
                    return d1.DemoNote.CompareTo(d2.DemoNote);
                }
                else
                {
                    return d1.DemoType.Name.CompareTo(d2.DemoType.Name);
                }
            });
            foreach (var demo in demoInfoList)
            {
                this.lstDemoList.Items.Add(demo);
            }

        }

        private void chkShowLayoutInspector_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkShowFormPrint_CheckedChanged(object sender, EventArgs e)
        {

        } 

        private void cmdSaveToImage_Click(object sender, EventArgs e)
        {
            int w = viewport.Width;
            int h = viewport.Height;

            //create target gdi+ bmp 
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(w, h))
            {
                System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                //
                this.viewport.PaintToPixelBuffer(bmpData.Scan0);
                //
                bmp.UnlockBits(bmpData);
                bmp.Save("d:\\WImageTest\\001.png");
            }
            //using (var bmp = new System.Drawing.Bitmap(800, 600, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            //using (var g = System.Drawing.Graphics.FromImage(bmp))
            //{
            //    //var canvasInit = new PixelFarm.Drawing.CanvasInitParameters();
            //    //canvasInit.externalCanvas = g;
            //    var canvas = new PixelFarm.Drawing.WinGdi.MyGdiPlusCanvas(0, 0, 800, 600); ;// LayoutFarm.UI.GdiPlus.MyWinGdiPortal.P.CreateCanvas(0, 0, 800, 600, canvasInit);
            //    canvas.CurrentFont = new PixelFarm.Drawing.RequestFont("Tahoma", 10, PixelFarm.Drawing.FontStyle.Regular);
            //    canvas.StrokeColor = PixelFarm.Drawing.Color.Black;
            //    viewport.PrintMe(canvas);
            //    bmp.Save("testhtml.bmp");
            //}
        }
    }
}
