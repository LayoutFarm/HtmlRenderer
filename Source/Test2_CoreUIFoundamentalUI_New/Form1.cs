using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using LayoutFarm;


namespace TestGraphicPackage2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
        }

        void Form1_Load(object sender, EventArgs e)
        {
            LoadDemoList();
            this.lstDemoList.DoubleClick += new EventHandler(lstDemoList_DoubleClick);
        }

        void lstDemoList_DoubleClick(object sender, EventArgs e)
        {
            //load demo sample
            var selectedDemoInfo = this.lstDemoList.SelectedItem as DemoInfo;
            if (selectedDemoInfo == null) return;
            //------------------------------------------------------------\


            DemoBase selectedDemo = (DemoBase)Activator.CreateInstance(selectedDemoInfo.DemoType);

            UISurfaceViewportControl viewport;

            Form formCanvas;
            CreateReadyForm(
                out viewport,
                out formCanvas);

            selectedDemo.StartDemo(viewport);
            viewport.WinTop.TopDownReCalculateContentSize();
            //==================================================  
            viewport.PaintMe();
            //ShowFormLayoutInspector(viewport); 
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

        static void CreateReadyForm(
          out UISurfaceViewportControl viewport,
          out Form formCanvas)
        {
            LayoutFarm.Text.EditableTextFlowLayer.DefaultFontInfo = 
                LayoutFarm.Drawing.CurrentGraphicPlatform.CreateTexFontInfo(
                    new System.Drawing.Font("tahoma", 10));
            //new TextFontInfo(
            //    LayoutFarm.Drawing.CurrentGraphicPlatform.CreateFont(
            //     new System.Drawing.Font("tahoma", 10)),
            //     new BasicGdi32FontHelper());

            formCanvas = FormCanvasHelper.CreateNewFormCanvas(out viewport);
            formCanvas.Text = "FormCanvas 1";

            viewport.PaintMe();

            formCanvas.WindowState = FormWindowState.Maximized;
            formCanvas.Show();

        }




        void LoadHtmlTestView(string filename)
        {

            UISurfaceViewportControl viewport;

            Form formCanvas;
            CreateReadyForm(
                out viewport,
                out formCanvas);

            ShowFormlayoutInspectIfNeed(viewport);

        }


        void ShowFormlayoutInspectIfNeed(UISurfaceViewportControl viewport)
        {
            if (this.chkShowLayoutInspector.Checked)
            {
                ShowFormLayoutInspector(viewport);
            }
        }






        void LoadDemoList()
        {
            this.lstDemoList.Items.Clear();

            var demoBaseType = typeof(DemoBase);
            var thisAssem = System.Reflection.Assembly.GetAssembly(this.GetType());
            List<DemoInfo> demoInfoList = new List<DemoInfo>();
            foreach (var t in thisAssem.GetTypes())
            {
                if (demoBaseType.IsAssignableFrom(t) && t != demoBaseType)
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
    }
}
