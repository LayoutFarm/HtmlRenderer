namespace BuildMergeProject
{
    partial class FormBuildMergeProject
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


        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mergeProjectsToolBox1 = new BuildMergeProject.MergeProjectsToolBox();
            this.SuspendLayout();
            // 
            // mergeProjectsToolBox1
            // 
            this.mergeProjectsToolBox1.Location = new System.Drawing.Point(13, 13);
            this.mergeProjectsToolBox1.Name = "mergeProjectsToolBox1";
            this.mergeProjectsToolBox1.Size = new System.Drawing.Size(853, 566);
            this.mergeProjectsToolBox1.TabIndex = 0;
            // 
            // FormBuildMergeProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 603);
            this.Controls.Add(this.mergeProjectsToolBox1);
            this.Name = "FormBuildMergeProject";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormBuildMergeProject_Load);
            this.ResumeLayout(false);

        }


        private BuildMergeProject.MergeProjectsToolBox mergeProjectsToolBox1;
    }
}