﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using LayoutFarm;
using LayoutFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.Dev
{
    public partial class FormDemoList : Form
    {

        UIPlatform uiPlatformWinForm;
        public FormDemoList()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
            uiPlatformWinForm = new LayoutFarm.UI.WinForms.UIPlatformWinForm(LayoutFarm.Drawing.CurrentGraphicPlatform.P);
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
            //------------------------------------------------------------\


            DemoBase selectedDemo = (DemoBase)Activator.CreateInstance(selectedDemoInfo.DemoType);

            LayoutFarm.UI.WinForms.UISurfaceViewportControl viewport;

            Form formCanvas;
            CreateReadyForm(
                out viewport,
                out formCanvas);

            selectedDemo.StartDemo(new SampleViewport(viewport));
            viewport.TopDownRecalculateContent();
            //==================================================  
            viewport.PaintMe();
            //ShowFormLayoutInspector(viewport); 
        }

        static void ShowFormLayoutInspector(LayoutFarm.UI.WinForms.UISurfaceViewportControl viewport)
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

        void CreateReadyForm(
        out LayoutFarm.UI.WinForms.UISurfaceViewportControl viewport,
        out Form formCanvas)
        {

            int w = 800;
            int h = 600;

            MyRootGraphic rootgfx = new MyRootGraphic(uiPlatformWinForm, w, h);
            TopWindowRenderBox topRenderBox = rootgfx.CreateTopWindowRenderBox(w, h);
            formCanvas = FormCanvasHelper.CreateNewFormCanvas(topRenderBox,
                rootgfx.CreateUserEventPortal(topRenderBox), out viewport);

            formCanvas.Text = "FormCanvas 1";

            viewport.PaintMe();

            formCanvas.WindowState = FormWindowState.Maximized;
            formCanvas.Show();
        }
        void ShowFormlayoutInspectIfNeed(LayoutFarm.UI.WinForms.UISurfaceViewportControl viewport)
        {
            if (this.chkShowLayoutInspector.Checked)
            {
                ShowFormLayoutInspector(viewport);
            }
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