namespace TestGraphicPackage2
{
    partial class Form1
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
            this.cmdShowBasicFormCanvas = new System.Windows.Forms.Button();
            this.cmdSampleTextBox = new System.Windows.Forms.Button();
            this.cmdMultilineTextBox = new System.Windows.Forms.Button();
            this.chkShowLayoutInspector = new System.Windows.Forms.CheckBox();
            this.cmdMultiLineTextWithFormat = new System.Windows.Forms.Button();
            this.cmdTestTextDom = new System.Windows.Forms.Button();
            this.cmdShowMultipleBox = new System.Windows.Forms.Button();
            this.lstHtmlTestFiles = new System.Windows.Forms.ListBox();
            this.cmdSampleGridBox = new System.Windows.Forms.Button();
            this.cmdDrag = new System.Windows.Forms.Button();
            this.cmdDragSample = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdShowBasicFormCanvas
            // 
            this.cmdShowBasicFormCanvas.Location = new System.Drawing.Point(12, 43);
            this.cmdShowBasicFormCanvas.Name = "cmdShowBasicFormCanvas";
            this.cmdShowBasicFormCanvas.Size = new System.Drawing.Size(249, 45);
            this.cmdShowBasicFormCanvas.TabIndex = 1;
            this.cmdShowBasicFormCanvas.Text = "1.1. Show Single Box";
            this.cmdShowBasicFormCanvas.UseVisualStyleBackColor = true;
            this.cmdShowBasicFormCanvas.Click += new System.EventHandler(this.cmdShowBasicFormCanvas_Click);
            // 
            // cmdSampleTextBox
            // 
            this.cmdSampleTextBox.Location = new System.Drawing.Point(12, 181);
            this.cmdSampleTextBox.Name = "cmdSampleTextBox";
            this.cmdSampleTextBox.Size = new System.Drawing.Size(249, 45);
            this.cmdSampleTextBox.TabIndex = 3;
            this.cmdSampleTextBox.Text = "2. 1 Sample SingleLine TextBox";
            this.cmdSampleTextBox.UseVisualStyleBackColor = true;
            this.cmdSampleTextBox.Click += new System.EventHandler(this.cmdSampleTextBox_Click);
            // 
            // cmdMultilineTextBox
            // 
            this.cmdMultilineTextBox.Location = new System.Drawing.Point(12, 244);
            this.cmdMultilineTextBox.Name = "cmdMultilineTextBox";
            this.cmdMultilineTextBox.Size = new System.Drawing.Size(249, 45);
            this.cmdMultilineTextBox.TabIndex = 4;
            this.cmdMultilineTextBox.Text = "2. 2 Sample Multiline TextBox";
            this.cmdMultilineTextBox.UseVisualStyleBackColor = true;
            this.cmdMultilineTextBox.Click += new System.EventHandler(this.cmdMultilineTextBox_Click);
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
            // cmdMultiLineTextWithFormat
            // 
            this.cmdMultiLineTextWithFormat.Location = new System.Drawing.Point(12, 306);
            this.cmdMultiLineTextWithFormat.Name = "cmdMultiLineTextWithFormat";
            this.cmdMultiLineTextWithFormat.Size = new System.Drawing.Size(249, 45);
            this.cmdMultiLineTextWithFormat.TabIndex = 6;
            this.cmdMultiLineTextWithFormat.Text = "2.3 Sample Multiline TextBox with Format";
            this.cmdMultiLineTextWithFormat.UseVisualStyleBackColor = true;
            this.cmdMultiLineTextWithFormat.Click += new System.EventHandler(this.cmdMultiLineTextWithFormat_Click);
            // 
            // cmdTestTextDom
            // 
            this.cmdTestTextDom.Location = new System.Drawing.Point(749, 43);
            this.cmdTestTextDom.Name = "cmdTestTextDom";
            this.cmdTestTextDom.Size = new System.Drawing.Size(249, 45);
            this.cmdTestTextDom.TabIndex = 7;
            this.cmdTestTextDom.Text = "4. Test TextDom";
            this.cmdTestTextDom.UseVisualStyleBackColor = true;
            this.cmdTestTextDom.Click += new System.EventHandler(this.cmdTestTextDom_Click);
            // 
            // cmdShowMultipleBox
            // 
            this.cmdShowMultipleBox.Location = new System.Drawing.Point(12, 94);
            this.cmdShowMultipleBox.Name = "cmdShowMultipleBox";
            this.cmdShowMultipleBox.Size = new System.Drawing.Size(249, 45);
            this.cmdShowMultipleBox.TabIndex = 8;
            this.cmdShowMultipleBox.Text = "1.2. Multiple Box";
            this.cmdShowMultipleBox.UseVisualStyleBackColor = true;
            this.cmdShowMultipleBox.Click += new System.EventHandler(this.cmdShowMultipleBox_Click);
            // 
            // lstHtmlTestFiles
            // 
            this.lstHtmlTestFiles.Location = new System.Drawing.Point(294, 257);
            this.lstHtmlTestFiles.Name = "lstHtmlTestFiles";
            this.lstHtmlTestFiles.Size = new System.Drawing.Size(459, 251);
            this.lstHtmlTestFiles.TabIndex = 11;
            // 
            // cmdSampleGridBox
            // 
            this.cmdSampleGridBox.Location = new System.Drawing.Point(294, 43);
            this.cmdSampleGridBox.Name = "cmdSampleGridBox";
            this.cmdSampleGridBox.Size = new System.Drawing.Size(249, 45);
            this.cmdSampleGridBox.TabIndex = 12;
            this.cmdSampleGridBox.Text = "1.3 SampleGridBox";
            this.cmdSampleGridBox.UseVisualStyleBackColor = true;
            this.cmdSampleGridBox.Click += new System.EventHandler(this.cmdSampleGridBox_Click);
            // 
            // cmdDrag
            // 
            this.cmdDrag.Location = new System.Drawing.Point(294, 94);
            this.cmdDrag.Name = "cmdDrag";
            this.cmdDrag.Size = new System.Drawing.Size(249, 45);
            this.cmdDrag.TabIndex = 13;
            this.cmdDrag.Text = "1.4 SampleScrollBar";
            this.cmdDrag.UseVisualStyleBackColor = true;
            this.cmdDrag.Click += new System.EventHandler(this.cmdSampleScrollbar_Click);
            // 
            // cmdDragSample
            // 
            this.cmdDragSample.Location = new System.Drawing.Point(294, 145);
            this.cmdDragSample.Name = "cmdDragSample";
            this.cmdDragSample.Size = new System.Drawing.Size(249, 45);
            this.cmdDragSample.TabIndex = 14;
            this.cmdDragSample.Text = "1.5 Drag";
            this.cmdDragSample.UseVisualStyleBackColor = true;
            this.cmdDragSample.Click += new System.EventHandler(this.cmdDragSample_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1087, 529);
            this.Controls.Add(this.cmdDragSample);
            this.Controls.Add(this.cmdDrag);
            this.Controls.Add(this.cmdSampleGridBox);
            this.Controls.Add(this.lstHtmlTestFiles);
            this.Controls.Add(this.cmdShowMultipleBox);
            this.Controls.Add(this.cmdTestTextDom);
            this.Controls.Add(this.cmdMultiLineTextWithFormat);
            this.Controls.Add(this.chkShowLayoutInspector);
            this.Controls.Add(this.cmdMultilineTextBox);
            this.Controls.Add(this.cmdSampleTextBox);
            this.Controls.Add(this.cmdShowBasicFormCanvas);
            this.Name = "Form1";
            this.Text = "TestGraphicPackage2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

                                        private System.Windows.Forms.Button cmdShowBasicFormCanvas;
        private System.Windows.Forms.Button cmdSampleTextBox;
        private System.Windows.Forms.Button cmdMultilineTextBox;
        private System.Windows.Forms.CheckBox chkShowLayoutInspector;
        private System.Windows.Forms.Button cmdMultiLineTextWithFormat;
        private System.Windows.Forms.Button cmdTestTextDom;
        private System.Windows.Forms.Button cmdShowMultipleBox;
        private System.Windows.Forms.ListBox lstHtmlTestFiles;
        private System.Windows.Forms.Button cmdSampleGridBox;
        private System.Windows.Forms.Button cmdDrag;
        private System.Windows.Forms.Button cmdDragSample;
    }
}

