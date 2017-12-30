//MIT, 2017, WinterDev

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;



namespace BuildMergeProject
{
    public partial class MergeProjectsToolBox : UserControl
    {
        SolutionMx slnMx;
        SolutionListViewController _slnListViewController;
        string slnFilename;
        public MergeProjectsToolBox()
        {
            InitializeComponent();
        }
        public void LoadSolution(string slnFilename)
        {
            this.slnFilename = slnFilename;
            ////read sln file 
            slnMx = new SolutionMx();
            slnMx.ReadSolution(slnFilename);

            _slnListViewController = new SolutionListViewController();
            _slnListViewController.SetSolutionListView(this.listView1);
            _slnListViewController.SetMergePlanListView(this.listView2);
            _slnListViewController.SetProjectReferenceListView(this.lstAsmReferenceList);
            _slnListViewController.LoadSolutionMx(slnMx);
        }

        private void cmdBuildSelectedMergePro_Click(object sender, EventArgs e)
        {
            _slnListViewController.BuildMergeProjectFromSelectedItem();
        }
    }
}
