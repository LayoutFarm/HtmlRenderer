namespace OpenTkEssTest
{
    partial class FormTestWinGLControl2
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
            this.myGLControl1 = new OpenTK.MyGLControl();
            this.SuspendLayout();
            // 
            // derivedGLControl1
            // 
            this.myGLControl1.BackColor = System.Drawing.Color.Black;
            this.myGLControl1.Location = new System.Drawing.Point(43, 12);
            this.myGLControl1.Name = "derivedGLControl1";
            this.myGLControl1.Size = new System.Drawing.Size(745, 469);
            this.myGLControl1.TabIndex = 0;
            this.myGLControl1.VSync = false;
            // 
            // FormTestWinGLControl2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1017, 531);
            this.Controls.Add(this.myGLControl1);
            this.Name = "FormTestWinGLControl2";
            this.Text = "FormTestWinGLControl";
            this.ResumeLayout(false);

        }

        #endregion

        private  OpenTK.MyGLControl myGLControl1;

    }
}