using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Mini;
namespace OpenTkEssTest
{
    public partial class FormTestWinGLControl : Form
    {
        public FormTestWinGLControl()
        {
            InitializeComponent();
            this.Load += new EventHandler(FormTestWinGLControl_Load);
        }

        void FormTestWinGLControl_Load(object sender, EventArgs e)
        {
            this.derivedGLControl1.ClearColor = new OpenTK.Graphics.Color4(1f, 1f, 1f, 1f);
        }
    }
}
