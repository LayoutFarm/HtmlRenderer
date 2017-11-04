namespace BuildMergeProject
{
    partial class Form1
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
            this.cmdBuildHtmlRenderOne = new System.Windows.Forms.Button();
            //this.cmdCopyNativeResources = new System.Windows.Forms.Button();
            this.cmdBuildSelectedMergePro = new System.Windows.Forms.Button();
            this.lstAsmReferenceList = new System.Windows.Forms.ListBox();
            this.listView2 = new System.Windows.Forms.ListView();
            this.lstPreset = new System.Windows.Forms.ListBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.cmdReadSln = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmdBuildHtmlRenderOne
            // 
            this.cmdBuildHtmlRenderOne.Location = new System.Drawing.Point(25, 22);
            this.cmdBuildHtmlRenderOne.Name = "cmdBuildHtmlRenderOne";
            this.cmdBuildHtmlRenderOne.Size = new System.Drawing.Size(203, 57);
            this.cmdBuildHtmlRenderOne.TabIndex = 0;
            this.cmdBuildHtmlRenderOne.Text = "BuildMerge HtmlRenderer.One";
            this.cmdBuildHtmlRenderOne.UseVisualStyleBackColor = true;
            this.cmdBuildHtmlRenderOne.Click += new System.EventHandler(this.cmdBuildHtmlRenderOne_Click);
            // 
            // cmdCopyNativeResources
            // 
            //this.cmdCopyNativeResources.Location = new System.Drawing.Point(25, 109);
            //this.cmdCopyNativeResources.Name = "cmdCopyNativeResources";
            //this.cmdCopyNativeResources.Size = new System.Drawing.Size(203, 63);
            //this.cmdCopyNativeResources.TabIndex = 2;
            //this.cmdCopyNativeResources.Text = "CopyNativeResources";
            //this.cmdCopyNativeResources.UseVisualStyleBackColor = true;
            //this.cmdCopyNativeResources.Click += new System.EventHandler(this.cmdCopyNativeResources_Click);
            // 
            // cmdBuildSelectedMergePro
            // 
            this.cmdBuildSelectedMergePro.Location = new System.Drawing.Point(228, 322);
            this.cmdBuildSelectedMergePro.Name = "cmdBuildSelectedMergePro";
            this.cmdBuildSelectedMergePro.Size = new System.Drawing.Size(102, 37);
            this.cmdBuildSelectedMergePro.TabIndex = 22;
            this.cmdBuildSelectedMergePro.Text = "Build Merge";
            this.cmdBuildSelectedMergePro.UseVisualStyleBackColor = true;
            this.cmdBuildSelectedMergePro.Click += new System.EventHandler(this.cmdBuildSelectedMergePro_Click);
            // 
            // lstAsmReferenceList
            // 
            this.lstAsmReferenceList.FormattingEnabled = true;
            this.lstAsmReferenceList.Location = new System.Drawing.Point(336, 322);
            this.lstAsmReferenceList.Name = "lstAsmReferenceList";
            this.lstAsmReferenceList.Size = new System.Drawing.Size(260, 251);
            this.lstAsmReferenceList.TabIndex = 21;
            // 
            // listView2
            // 
            this.listView2.FullRowSelect = true;
            this.listView2.Location = new System.Drawing.Point(336, 97);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(260, 205);
            this.listView2.TabIndex = 20;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // lstPreset
            // 
            this.lstPreset.FormattingEnabled = true;
            this.lstPreset.Location = new System.Drawing.Point(163, 491);
            this.lstPreset.Name = "lstPreset";
            this.lstPreset.Size = new System.Drawing.Size(167, 82);
            this.lstPreset.TabIndex = 19;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(602, 97);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(367, 476);
            this.listView1.TabIndex = 18;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // cmdReadSln
            // 
            this.cmdReadSln.Location = new System.Drawing.Point(336, 32);
            this.cmdReadSln.Name = "cmdReadSln";
            this.cmdReadSln.Size = new System.Drawing.Size(102, 37);
            this.cmdReadSln.TabIndex = 17;
            this.cmdReadSln.Text = "Read Sln";
            this.cmdReadSln.UseVisualStyleBackColor = true;
            this.cmdReadSln.Click += new System.EventHandler(this.cmdReadSln_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(602, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "All projects";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(333, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Merge Plans";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 588);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdBuildSelectedMergePro);
            this.Controls.Add(this.lstAsmReferenceList);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.lstPreset);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.cmdReadSln);
            //this.Controls.Add(this.cmdCopyNativeResources);
            this.Controls.Add(this.cmdBuildHtmlRenderOne);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdBuildHtmlRenderOne;
        //private System.Windows.Forms.Button cmdCopyNativeResources;
        private System.Windows.Forms.Button cmdBuildSelectedMergePro;
        private System.Windows.Forms.ListBox lstAsmReferenceList;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ListBox lstPreset;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button cmdReadSln;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

