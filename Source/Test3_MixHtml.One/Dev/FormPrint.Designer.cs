namespace LayoutFarm.Dev
{
    partial class FormPrint
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
            this.cmdPrint = new System.Windows.Forms.Button();
            this.cmdPrintToPrinter = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdPrint
            // 
            this.cmdPrint.Location = new System.Drawing.Point(2, 2);
            this.cmdPrint.Name = "cmdPrint";
            this.cmdPrint.Size = new System.Drawing.Size(83, 32);
            this.cmdPrint.TabIndex = 0;
            this.cmdPrint.Text = "Print To Bmp";
            this.cmdPrint.UseVisualStyleBackColor = true;
            this.cmdPrint.Click += new System.EventHandler(this.cmdPrint_Click);
            // 
            // cmdPrintToPrinter
            // 
            this.cmdPrintToPrinter.Location = new System.Drawing.Point(91, 2);
            this.cmdPrintToPrinter.Name = "cmdPrintToPrinter";
            this.cmdPrintToPrinter.Size = new System.Drawing.Size(101, 32);
            this.cmdPrintToPrinter.TabIndex = 1;
            this.cmdPrintToPrinter.Text = "Print To Printer";
            this.cmdPrintToPrinter.UseVisualStyleBackColor = true;
            this.cmdPrintToPrinter.Click += new System.EventHandler(this.cmdPrintToPrinter_Click);
            // 
            // FormPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 46);
            this.Controls.Add(this.cmdPrintToPrinter);
            this.Controls.Add(this.cmdPrint);
            this.Name = "FormPrint";
            this.Text = "FormPrint";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdPrint;
        private System.Windows.Forms.Button cmdPrintToPrinter;
    }
}