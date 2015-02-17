using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
             
            vwport.PrintMe();
        }
    }
}
