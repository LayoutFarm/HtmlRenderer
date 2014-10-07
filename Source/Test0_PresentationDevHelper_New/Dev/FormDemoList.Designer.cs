namespace LayoutFarm.Dev
{
    partial class FormDemoList
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.chkShowLayoutInspector = new System.Windows.Forms.CheckBox();
            this.lstHtmlTestFiles = new System.Windows.Forms.ListBox();
            this.lstDemoList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // chkShowLayoutInspector
            // 
            this.chkShowLayoutInspector.AutoSize = true;
            this.chkShowLayoutInspector.Location = new System.Drawing.Point(12, 5);
            this.chkShowLayoutInspector.Name = "chkShowLayoutInspector";
            this.chkShowLayoutInspector.Size = new System.Drawing.Size(153, 17);
            this.chkShowLayoutInspector.TabIndex = 5;
            this.chkShowLayoutInspector.Text = "Also show LayoutInspector";
            this.chkShowLayoutInspector.UseVisualStyleBackColor = true;
            // 
            // lstHtmlTestFiles
            // 
            this.lstHtmlTestFiles.Location = new System.Drawing.Point(432, 448);
            this.lstHtmlTestFiles.Name = "lstHtmlTestFiles";
            this.lstHtmlTestFiles.Size = new System.Drawing.Size(387, 69);
            this.lstHtmlTestFiles.TabIndex = 11;
            // 
            // lstDemoList
            // 
            this.lstDemoList.FormattingEnabled = true;
            this.lstDemoList.Location = new System.Drawing.Point(13, 43);
            this.lstDemoList.Name = "lstDemoList";
            this.lstDemoList.Size = new System.Drawing.Size(388, 472);
            this.lstDemoList.TabIndex = 15;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 529);
            this.Controls.Add(this.lstDemoList);
            this.Controls.Add(this.lstHtmlTestFiles);
            this.Controls.Add(this.chkShowLayoutInspector);
            this.Name = "Form1";
            this.Text = "TestGraphicPackage2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkShowLayoutInspector;
        private System.Windows.Forms.ListBox lstHtmlTestFiles;
        private System.Windows.Forms.ListBox lstDemoList;
    }
}

