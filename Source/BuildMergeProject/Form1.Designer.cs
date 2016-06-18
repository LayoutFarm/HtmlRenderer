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
            this.cmdBuild_HtmlRendererOnePortable = new System.Windows.Forms.Button();
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
            // cmdBuild_HtmlRendererOnePortable
            // 
            this.cmdBuild_HtmlRendererOnePortable.Location = new System.Drawing.Point(291, 22);
            this.cmdBuild_HtmlRendererOnePortable.Name = "cmdBuild_HtmlRendererOnePortable";
            this.cmdBuild_HtmlRendererOnePortable.Size = new System.Drawing.Size(226, 57);
            this.cmdBuild_HtmlRendererOnePortable.TabIndex = 1;
            this.cmdBuild_HtmlRendererOnePortable.Text = "BuildMerge HtmlRenderer.One Portable";
            this.cmdBuild_HtmlRendererOnePortable.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(727, 161);
            this.Controls.Add(this.cmdBuild_HtmlRendererOnePortable);
            this.Controls.Add(this.cmdBuildHtmlRenderOne);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdBuildHtmlRenderOne;
        private System.Windows.Forms.Button cmdBuild_HtmlRendererOnePortable;
    }
}

