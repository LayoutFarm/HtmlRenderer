namespace HtmlRenderer.Demo
{
    partial class DevForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.cmdCreateHtmlDom = new System.Windows.Forms.Button();
            this.createHtmlDomDemo2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(188, 48);
            this.button1.TabIndex = 0;
            this.button1.Text = "1. Go to HtmlDemo Form";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 166);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(188, 48);
            this.button2.TabIndex = 1;
            this.button2.Text = "2. Test Html Parser";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 86);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(188, 48);
            this.button3.TabIndex = 2;
            this.button3.Text = "1. Go to HtmlDemo Form x10";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // cmdCreateHtmlDom
            // 
            this.cmdCreateHtmlDom.Location = new System.Drawing.Point(12, 300);
            this.cmdCreateHtmlDom.Name = "cmdCreateHtmlDom";
            this.cmdCreateHtmlDom.Size = new System.Drawing.Size(188, 48);
            this.cmdCreateHtmlDom.TabIndex = 3;
            this.cmdCreateHtmlDom.Text = "Create HtmlDom";
            this.cmdCreateHtmlDom.UseVisualStyleBackColor = true;
            this.cmdCreateHtmlDom.Click += new System.EventHandler(this.cmdCreateHtmlDom_Click);
            // 
            // createHtmlDomDemo2
            // 
            this.createHtmlDomDemo2.Location = new System.Drawing.Point(12, 354);
            this.createHtmlDomDemo2.Name = "createHtmlDomDemo2";
            this.createHtmlDomDemo2.Size = new System.Drawing.Size(188, 48);
            this.createHtmlDomDemo2.TabIndex = 4;
            this.createHtmlDomDemo2.Text = "Create HtmlDom2";
            this.createHtmlDomDemo2.UseVisualStyleBackColor = true;
            this.createHtmlDomDemo2.Click += new System.EventHandler(this.createHtmlDomDemo2_Click);
            // 
            // DevForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 474);
            this.Controls.Add(this.createHtmlDomDemo2);
            this.Controls.Add(this.cmdCreateHtmlDom);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "DevForm";
            this.Text = "DevForm : Developer Form";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button cmdCreateHtmlDom;
        private System.Windows.Forms.Button createHtmlDomDemo2;
    }
}