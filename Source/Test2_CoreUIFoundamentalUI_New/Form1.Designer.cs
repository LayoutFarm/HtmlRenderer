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
            this.cmdHtmlView = new System.Windows.Forms.Button();
            this.cmdSampleTextBox = new System.Windows.Forms.Button();
            this.cmdMultilineTextBox = new System.Windows.Forms.Button();
            this.chkShowLayoutInspector = new System.Windows.Forms.CheckBox();
            this.cmdMultiLineTextWithFormat = new System.Windows.Forms.Button();
            this.cmdTestTextDom = new System.Windows.Forms.Button();
            this.cmdShowMultipleBox = new System.Windows.Forms.Button();
            this.lstHtmlTestFiles = new System.Windows.Forms.ListBox();
            this.cmdHtmlViewNew = new System.Windows.Forms.Button();
            this.SuspendLayout();
                                                this.cmdShowBasicFormCanvas.Location = new System.Drawing.Point(12, 43);
            this.cmdShowBasicFormCanvas.Name = "cmdShowBasicFormCanvas";
            this.cmdShowBasicFormCanvas.Size = new System.Drawing.Size(249, 45);
            this.cmdShowBasicFormCanvas.TabIndex = 1;
            this.cmdShowBasicFormCanvas.Text = "1.1. Show Single Box";
            this.cmdShowBasicFormCanvas.UseVisualStyleBackColor = true;
            this.cmdShowBasicFormCanvas.Click += new System.EventHandler(this.cmdShowBasicFormCanvas_Click);
                                                this.cmdHtmlView.Location = new System.Drawing.Point(12, 366);
            this.cmdHtmlView.Name = "cmdHtmlView";
            this.cmdHtmlView.Size = new System.Drawing.Size(249, 45);
            this.cmdHtmlView.TabIndex = 2;
            this.cmdHtmlView.Text = "3.1 Show HtmlView Old";
            this.cmdHtmlView.UseVisualStyleBackColor = true;
            this.cmdHtmlView.Click += new System.EventHandler(this.cmdHtmlView_Click);
                                                this.cmdSampleTextBox.Location = new System.Drawing.Point(12, 181);
            this.cmdSampleTextBox.Name = "cmdSampleTextBox";
            this.cmdSampleTextBox.Size = new System.Drawing.Size(249, 45);
            this.cmdSampleTextBox.TabIndex = 3;
            this.cmdSampleTextBox.Text = "2. 1 Sample SingleLine TextBox";
            this.cmdSampleTextBox.UseVisualStyleBackColor = true;
            this.cmdSampleTextBox.Click += new System.EventHandler(this.cmdSampleTextBox_Click);
                                                this.cmdMultilineTextBox.Location = new System.Drawing.Point(12, 244);
            this.cmdMultilineTextBox.Name = "cmdMultilineTextBox";
            this.cmdMultilineTextBox.Size = new System.Drawing.Size(249, 45);
            this.cmdMultilineTextBox.TabIndex = 4;
            this.cmdMultilineTextBox.Text = "2. 2 Sample Multiline TextBox";
            this.cmdMultilineTextBox.UseVisualStyleBackColor = true;
            this.cmdMultilineTextBox.Click += new System.EventHandler(this.cmdMultilineTextBox_Click);
                                                this.chkShowLayoutInspector.AutoSize = true;
            this.chkShowLayoutInspector.Location = new System.Drawing.Point(12, 5);
            this.chkShowLayoutInspector.Name = "chkShowLayoutInspector";
            this.chkShowLayoutInspector.Size = new System.Drawing.Size(153, 17);
            this.chkShowLayoutInspector.TabIndex = 5;
            this.chkShowLayoutInspector.Text = "Also show LayoutInspector";
            this.chkShowLayoutInspector.UseVisualStyleBackColor = true;
                                                this.cmdMultiLineTextWithFormat.Location = new System.Drawing.Point(12, 306);
            this.cmdMultiLineTextWithFormat.Name = "cmdMultiLineTextWithFormat";
            this.cmdMultiLineTextWithFormat.Size = new System.Drawing.Size(249, 45);
            this.cmdMultiLineTextWithFormat.TabIndex = 6;
            this.cmdMultiLineTextWithFormat.Text = "2.3 Sample Multiline TextBox with Format";
            this.cmdMultiLineTextWithFormat.UseVisualStyleBackColor = true;
            this.cmdMultiLineTextWithFormat.Click += new System.EventHandler(this.cmdMultiLineTextWithFormat_Click);
                                                this.cmdTestTextDom.Location = new System.Drawing.Point(749, 43);
            this.cmdTestTextDom.Name = "cmdTestTextDom";
            this.cmdTestTextDom.Size = new System.Drawing.Size(249, 45);
            this.cmdTestTextDom.TabIndex = 7;
            this.cmdTestTextDom.Text = "4. Test TextDom";
            this.cmdTestTextDom.UseVisualStyleBackColor = true;
            this.cmdTestTextDom.Click += new System.EventHandler(this.cmdTestTextDom_Click);
                                                this.cmdShowMultipleBox.Location = new System.Drawing.Point(12, 94);
            this.cmdShowMultipleBox.Name = "cmdShowMultipleBox";
            this.cmdShowMultipleBox.Size = new System.Drawing.Size(249, 45);
            this.cmdShowMultipleBox.TabIndex = 8;
            this.cmdShowMultipleBox.Text = "1.2. Multiple Box";
            this.cmdShowMultipleBox.UseVisualStyleBackColor = true;
            this.cmdShowMultipleBox.Click += new System.EventHandler(this.cmdShowMultipleBox_Click);
                                                this.lstHtmlTestFiles.Location = new System.Drawing.Point(315, 181);
            this.lstHtmlTestFiles.Name = "lstHtmlTestFiles";
            this.lstHtmlTestFiles.Size = new System.Drawing.Size(459, 251);
            this.lstHtmlTestFiles.TabIndex = 11;
                                                this.cmdHtmlViewNew.Location = new System.Drawing.Point(342, 43);
            this.cmdHtmlViewNew.Name = "cmdHtmlViewNew";
            this.cmdHtmlViewNew.Size = new System.Drawing.Size(249, 45);
            this.cmdHtmlViewNew.TabIndex = 10;
            this.cmdHtmlViewNew.Text = "3.2 Show HtmlView";
            this.cmdHtmlViewNew.UseVisualStyleBackColor = true;
            this.cmdHtmlViewNew.Click += new System.EventHandler(this.cmdHtmlViewNew_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1087, 529);
            this.Controls.Add(this.cmdHtmlViewNew);
            this.Controls.Add(this.lstHtmlTestFiles);
            this.Controls.Add(this.cmdShowMultipleBox);
            this.Controls.Add(this.cmdTestTextDom);
            this.Controls.Add(this.cmdMultiLineTextWithFormat);
            this.Controls.Add(this.chkShowLayoutInspector);
            this.Controls.Add(this.cmdMultilineTextBox);
            this.Controls.Add(this.cmdSampleTextBox);
            this.Controls.Add(this.cmdHtmlView);
            this.Controls.Add(this.cmdShowBasicFormCanvas);
            this.Name = "Form1";
            this.Text = "TestGraphicPackage2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdShowBasicFormCanvas;
        private System.Windows.Forms.Button cmdHtmlView;
        private System.Windows.Forms.Button cmdSampleTextBox;
        private System.Windows.Forms.Button cmdMultilineTextBox;
        private System.Windows.Forms.CheckBox chkShowLayoutInspector;
        private System.Windows.Forms.Button cmdMultiLineTextWithFormat;
        private System.Windows.Forms.Button cmdTestTextDom;
        private System.Windows.Forms.Button cmdShowMultipleBox;
        private System.Windows.Forms.ListBox lstHtmlTestFiles;
        private System.Windows.Forms.Button cmdHtmlViewNew;
    }
}

