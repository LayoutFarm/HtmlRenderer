namespace LayoutFarm.Dev
{
    partial class FormLayoutInspector
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tlstrpDumpSelectedVisualProps = new System.Windows.Forms.ToolStripButton();
            this.tlstrpSaveSelectedVisualProps = new System.Windows.Forms.ToolStripButton();
            this.tlstrpReArrange = new System.Windows.Forms.ToolStripButton();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.listBox3 = new System.Windows.Forms.ListBox();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBox1);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(888, 447);
            this.splitContainer1.SplitterDistance = 95;
            this.splitContainer1.TabIndex = 47;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 25);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(888, 70);
            this.listBox1.TabIndex = 46;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.toolStripButton1,
            this.toolStripTextBox1,
            this.toolStripSeparator1,
            this.tlstrpDumpSelectedVisualProps,
            this.tlstrpSaveSelectedVisualProps,
            this.tlstrpReArrange});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(888, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(50, 22);
            this.toolStripButton1.Text = "Refresh";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click_1);
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 25);
            this.toolStripTextBox1.Text = "800";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tlstrpDumpSelectedVisualProps
            // 
            this.tlstrpDumpSelectedVisualProps.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tlstrpDumpSelectedVisualProps.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tlstrpDumpSelectedVisualProps.Name = "tlstrpDumpSelectedVisualProps";
            this.tlstrpDumpSelectedVisualProps.Size = new System.Drawing.Size(105, 22);
            this.tlstrpDumpSelectedVisualProps.Text = "DumpVisualProps";
            this.tlstrpDumpSelectedVisualProps.Click += new System.EventHandler(this.tlstrpDumpSelectedVisualProps_Click);
            // 
            // tlstrpSaveSelectedVisualProps
            // 
            this.tlstrpSaveSelectedVisualProps.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tlstrpSaveSelectedVisualProps.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tlstrpSaveSelectedVisualProps.Name = "tlstrpSaveSelectedVisualProps";
            this.tlstrpSaveSelectedVisualProps.Size = new System.Drawing.Size(35, 22);
            this.tlstrpSaveSelectedVisualProps.Text = "Save";
            this.tlstrpSaveSelectedVisualProps.Click += new System.EventHandler(this.tlstrpSaveSelectedVisualProps_Click);
            // 
            // tlstrpReArrange
            // 
            this.tlstrpReArrange.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tlstrpReArrange.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tlstrpReArrange.Name = "tlstrpReArrange";
            this.tlstrpReArrange.Size = new System.Drawing.Size(66, 22);
            this.tlstrpReArrange.Text = "ReArrange";
            this.tlstrpReArrange.Click += new System.EventHandler(this.tlstrpReArrange_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listBox2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.listBox3);
            this.splitContainer2.Size = new System.Drawing.Size(888, 348);
            this.splitContainer2.SplitterDistance = 233;
            this.splitContainer2.TabIndex = 0;
            // 
            // listBox2
            // 
            this.listBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(0, 0);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(233, 348);
            this.listBox2.TabIndex = 0;
            // 
            // listBox3
            // 
            this.listBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox3.FormattingEnabled = true;
            this.listBox3.Location = new System.Drawing.Point(0, 0);
            this.listBox3.Name = "listBox3";
            this.listBox3.Size = new System.Drawing.Size(651, 348);
            this.listBox3.TabIndex = 1;
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(98, 22);
            this.toolStripButton2.Text = "CopyDrawResult";
            // 
            // FormLayoutInspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(888, 447);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormLayoutInspector";
            this.Text = "FormLayoutInspector";
            this.TopMost = true;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.ListBox listBox3;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton tlstrpDumpSelectedVisualProps;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tlstrpSaveSelectedVisualProps;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripButton tlstrpReArrange;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
    }
}