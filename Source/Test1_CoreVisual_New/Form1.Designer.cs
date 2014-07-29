namespace TestGraphicPackage
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
            this.cmdShowEmbededViewport = new System.Windows.Forms.Button();
            this.SuspendLayout();
                                                this.cmdShowBasicFormCanvas.Location = new System.Drawing.Point(12, 12);
            this.cmdShowBasicFormCanvas.Name = "cmdShowBasicFormCanvas";
            this.cmdShowBasicFormCanvas.Size = new System.Drawing.Size(249, 45);
            this.cmdShowBasicFormCanvas.TabIndex = 0;
            this.cmdShowBasicFormCanvas.Text = "1. Show BasicFormCanvas";
            this.cmdShowBasicFormCanvas.UseVisualStyleBackColor = true;
            this.cmdShowBasicFormCanvas.Click += new System.EventHandler(this.cmdShowBasicFormCanvas_Click);
                                                this.cmdShowEmbededViewport.Location = new System.Drawing.Point(12, 72);
            this.cmdShowEmbededViewport.Name = "cmdShowEmbededViewport";
            this.cmdShowEmbededViewport.Size = new System.Drawing.Size(249, 45);
            this.cmdShowEmbededViewport.TabIndex = 1;
            this.cmdShowEmbededViewport.Text = "2. Show Embeded Viewport";
            this.cmdShowEmbededViewport.UseVisualStyleBackColor = true;
            this.cmdShowEmbededViewport.Click += new System.EventHandler(this.cmdShowEmbededViewport_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 317);
            this.Controls.Add(this.cmdShowEmbededViewport);
            this.Controls.Add(this.cmdShowBasicFormCanvas);
            this.Name = "Form1";
            this.Text = "TestGraphicPackage1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdShowBasicFormCanvas;
        private System.Windows.Forms.Button cmdShowEmbededViewport;
    }
}

