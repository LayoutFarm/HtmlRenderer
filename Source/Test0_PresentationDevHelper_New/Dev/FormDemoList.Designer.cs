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
            this.chkUseGLCanvas = new System.Windows.Forms.CheckBox();
            this._samplesTreeView = new System.Windows.Forms.TreeView();
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
            // chkUseGLCanvas
            // 
            this.chkUseGLCanvas.AutoSize = true;
            this.chkUseGLCanvas.Location = new System.Drawing.Point(300, 20);
            this.chkUseGLCanvas.Name = "chkUseGLCanvas";
            this.chkUseGLCanvas.Size = new System.Drawing.Size(101, 17);
            this.chkUseGLCanvas.TabIndex = 16;
            this.chkUseGLCanvas.Text = "Use GL Canvas";
            this.chkUseGLCanvas.UseVisualStyleBackColor = true;
            // 
            // _samplesTreeView
            // 
            this._samplesTreeView.Location = new System.Drawing.Point(432, 43);
            this._samplesTreeView.Name = "_samplesTreeView";
            this._samplesTreeView.Size = new System.Drawing.Size(387, 399);
            this._samplesTreeView.TabIndex = 17;
            // 
            // FormDemoList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 529);
            this.Controls.Add(this._samplesTreeView);
            this.Controls.Add(this.chkUseGLCanvas);
            this.Controls.Add(this.lstDemoList);
            this.Controls.Add(this.lstHtmlTestFiles);
            this.Controls.Add(this.chkShowLayoutInspector);
            this.Name = "FormDemoList";
            this.Text = "TestGraphicPackage2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkShowLayoutInspector;
        private System.Windows.Forms.ListBox lstHtmlTestFiles;
        private System.Windows.Forms.ListBox lstDemoList;
        private System.Windows.Forms.CheckBox chkUseGLCanvas;
        private System.Windows.Forms.TreeView _samplesTreeView;
    }
}

