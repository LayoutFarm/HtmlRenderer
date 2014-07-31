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
            this.chkShowLayoutInspector = new System.Windows.Forms.CheckBox();
            this.cmdMixHtml = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
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
            // cmdMixHtml
            // 
            this.cmdMixHtml.Location = new System.Drawing.Point(12, 57);
            this.cmdMixHtml.Name = "cmdMixHtml";
            this.cmdMixHtml.Size = new System.Drawing.Size(173, 46);
            this.cmdMixHtml.TabIndex = 6;
            this.cmdMixHtml.Text = "MixHtml";
            this.cmdMixHtml.UseVisualStyleBackColor = true;
            this.cmdMixHtml.Click += new System.EventHandler(this.cmdMixHtml_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 128);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(173, 46);
            this.button1.TabIndex = 7;
            this.button1.Text = "MixHtml+Text";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1087, 529);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmdMixHtml);
            this.Controls.Add(this.chkShowLayoutInspector);
            this.Name = "Form1";
            this.Text = "TestGraphicPackage2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

                                        private System.Windows.Forms.CheckBox chkShowLayoutInspector;
                                        private System.Windows.Forms.Button cmdMixHtml;
                                        private System.Windows.Forms.Button button1;
    }
}

