using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace LayoutFarm.Demo
{
    public partial class DevForm : Form
    {
      
        public DevForm()
        {
           
            InitializeComponent();
            this.lstDemoList.DoubleClick += new EventHandler(lstDemoList_DoubleClick);
            LoadDemoList();
        }

        void lstDemoList_DoubleClick(object sender, EventArgs e)
        {
            var demoInfo = lstDemoList.SelectedItem as DemoInfo;
            if (demoInfo != null)
            {
                DemoBase demoInstance = (DemoBase)Activator.CreateInstance(demoInfo.DemoType);
                if (demoInstance != null)
                {
                    DemoForm demoForm = new DemoForm();
                    demoForm.Show();
                    demoForm.Activate();
                    demoForm.LoadDemo(demoInstance);
                }
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
               d1.DemoType.Name.CompareTo(d2.DemoType.Name));
            foreach (var demo in demoInfoList)
            {
                this.lstDemoList.Items.Add(demo);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            DemoForm demoForm = new DemoForm();
            demoForm.PrepareSamples();
            demoForm.Show();
            demoForm.Activate();
        }


        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; ++i)
            {
                DemoForm demoForm = new DemoForm();
                demoForm.StartAtSampleIndex = 2;
                demoForm.PrepareSamples();
                demoForm.Show();
                demoForm.Activate();
                System.Threading.Thread.Sleep(10);
                demoForm.Close();
            }
        }
    }
}
