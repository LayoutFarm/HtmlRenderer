namespace OpenTkEssTest
{
    partial class FormTestWinGLControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.derivedGLControl1 = new OpenTK.MyGLControl();
            this.SuspendLayout();
            // 
            // derivedGLControl1
            // 
            this.derivedGLControl1.BackColor = System.Drawing.Color.Black;
            this.derivedGLControl1.ClearColor = new OpenTK.Graphics.Color4(1f, 1f, 1f, 1f);
            this.derivedGLControl1.Location = new System.Drawing.Point(31, 12);
            this.derivedGLControl1.Name = "derivedGLControl1";
            this.derivedGLControl1.Size = new System.Drawing.Size(576, 416);
            this.derivedGLControl1.TabIndex = 0;
            this.derivedGLControl1.VSync = false;
            // 
            // FormTestWinGLControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 463);
            this.Controls.Add(this.derivedGLControl1);
            this.Name = "FormTestWinGLControl";
            this.Text = "FormTestWinGLControl";
            this.ResumeLayout(false);

        }

        #endregion

        private OpenTK.MyGLControl derivedGLControl1;

    }
}