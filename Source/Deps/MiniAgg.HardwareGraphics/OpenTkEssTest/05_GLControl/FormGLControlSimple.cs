// This code is in the Public Domain. It is provided "as is"
// without express or implied warranty of any kind.

using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
namespace OpenTkEssTest
{
    // [Example("GLControl Simple", ExampleCategory.OpenTK, "GLControl", 1, Documentation="GLControlSimple")]
    public partial class FormGLControlSimple : Form
    {
        public FormGLControlSimple()
        {
            InitializeComponent();
        }

        #region Events
        protected void GLClearColor(System.Drawing.Color c)
        {
            GLHelper.ClearColor(PixelFarm.Drawing.Conv.ToColor(c));
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            glControl1_Resize(this, EventArgs.Empty);   // Ensure the Viewport is set up correctly             
            GLHelper.ClearColor(PixelFarm.Drawing.Color.LightGray);
        }

        private void redButton_Click(object sender, EventArgs e)
        {
            GLHelper.ClearColor(PixelFarm.Drawing.Color.Red);
            glControl1.Invalidate();
        }

        private void greenButton_Click(object sender, EventArgs e)
        {
            GLHelper.ClearColor(PixelFarm.Drawing.Color.Green);
            glControl1.Invalidate();
        }

        private void blueButton_Click(object sender, EventArgs e)
        {
            GLClearColor(Color.RoyalBlue);
            glControl1.Invalidate();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            glControl1.MakeCurrent();
            GL.Clear(ClearBufferMask.ColorBufferBit);
            glControl1.SwapBuffers();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (glControl1.ClientSize.Height == 0)
                glControl1.ClientSize = new System.Drawing.Size(glControl1.ClientSize.Width, 1);
            GL.Viewport(0, 0, glControl1.ClientSize.Width, glControl1.ClientSize.Height);
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        #endregion

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        //[STAThread]
        //public static void Main()
        //{
        //    using (SimpleForm example = new SimpleForm())
        //    {
        //        Utilities.SetWindowTitle(example);
        //        example.ShowDialog();
        //    }
        //}

        #endregion
    }
}
