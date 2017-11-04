//MIT, 2016-2017, WinterDev
using System;
using System.Windows.Forms;

namespace BuildMergeProject
{
    public partial class FormBuildMergeProject : Form
    {
        public FormBuildMergeProject()
        {
            InitializeComponent();
        }
        private void FormBuildMergeProject_Load(object sender, EventArgs e)
        {
            if (StartupConfig.defaultSln != null)
            {
                mergeProjectsToolBox1.LoadSolution(StartupConfig.defaultSln);
            }
        }
    }
}
