//MIT
//Mike Kruger, ICSharpCode,

using System;
using System.Collections.Generic;
using System.ComponentModel;
 
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LayoutFarm.UI
{
    partial class AbstractCompletionWindow : Form
    {
        Form linkedParentForm;
        Control linkedParentControl;

        public AbstractCompletionWindow()
        {
            InitializeComponent();

            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;

        }
        public Form LinkedParentForm
        {
            get { return this.linkedParentForm; }
            set { this.linkedParentForm = value; }
        }
        public Control LinkedParentControl
        {
            get { return this.linkedParentControl; }
            set
            {

                this.linkedParentControl = value;
            }
        }
        protected override CreateParams CreateParams
        {
            get
            {
                var createParams = base.CreateParams;
                createParams.ClassStyle |= 0x00020000;//add window shadow
                return createParams;
            }
        }
        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        public void ShowForm()
        {
            this.Show();
            this.linkedParentControl.Focus();
        }
    }
}
