//MIT, 2017, WinterDev

using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace BuildMergeProject
{

    class SolutionListViewController
    {
        SolutionMx _solutionMx;
        ListView _listview;
        ListBox _asmRefListView;
        List<SolutionProject> mergePlansProjects = new List<SolutionProject>();
        ListView _mergePlanListView;
        public SolutionListViewController()
        {

        }

        public void SetProjectReferenceListView(ListBox asmRefListView)
        {
            _asmRefListView = asmRefListView;
            asmRefListView.Items.Clear();
        }
        public void SetMergePlanListView(ListView mergePlanListView)
        {
            _mergePlanListView = mergePlanListView;
            var colHeader = new ColumnHeader();
            colHeader.Text = "Name";
            colHeader.Width = 300;
            _mergePlanListView.Columns.Add(colHeader);
            _mergePlanListView.SelectedIndexChanged += _mergePlanListView_SelectedIndexChanged;
        }

        SolutionProject _currentSelectedMergePro = null;



        private void _mergePlanListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentSelectedMergePro = null;//reset
            //show detail for this project
            //load solution detail
            if (_mergePlanListView.SelectedItems.Count == 0) { return; }
            //only 1
            ListViewItem selectedItem = _mergePlanListView.SelectedItems[0];
            var project = selectedItem.Tag as SolutionProject;
            if (project == null) { return; }
            _currentSelectedMergePro = project;
            //load project detail
            List<ProjectAsmReference> asmRefs = _solutionMx.GetReferenceAsmList(project.RelativePath);
            _asmRefListView.Items.Clear();
            int j = asmRefs.Count;
            for (int i = 0; i < j; ++i)
            {
                _asmRefListView.Items.Add(asmRefs[i]);
            }
        }

        public void SetSolutionListView(ListView listview)
        {
            _listview = listview;
            listview.Columns.Clear();

            var colHeader = new ColumnHeader();
            colHeader.Text = "Name";
            colHeader.Width = 300;
            listview.Columns.Add(colHeader);
            _listview.SelectedIndexChanged += _listview_SelectedIndexChanged;
        }
        public void LoadSolutionMx(SolutionMx solutionMx)
        {

            _solutionMx = solutionMx;
            _listview.SuspendLayout();
            _listview.Controls.Clear();
            mergePlansProjects.Clear();
            List<SolutionProject> tmpProjects = new List<SolutionProject>();
            foreach (SolutionProject project in solutionMx._currentSolution.Projects)
            {
                if (project.RelativePath.Contains("x_merge_projects"))
                {
                    //BY CONVENTION:
                    //this is special 'merge plan' project
                    mergePlansProjects.Add(project);
                }
                else
                {
                    tmpProjects.Add(project);
                }

            }

            //-------
            //sort
            tmpProjects.Sort((p0, p1) => p0.ProjectName.CompareTo(p1.ProjectName));
            mergePlansProjects.Sort((p0, p1) => p0.ProjectName.CompareTo(p1.ProjectName));
            //
            //-------
            foreach (SolutionProject project in tmpProjects)
            {
                ListViewItem lstItem = new ListViewItem();
                switch (project.ProjectType)
                {
                    case "KnownToBeMSBuildFormat":
                        //project
                        lstItem.Text = project.ProjectName;
                        lstItem.Tag = project;
                        _listview.Items.Add(lstItem);
                        break;
                    case "SolutionFolder":
                        //skip solution folder
                        break;
                    default:
                        break;
                }
            }
            _listview.ResumeLayout();
            //-------

            _mergePlanListView.Items.Clear();
            _mergePlanListView.SuspendLayout();
            foreach (SolutionProject project in mergePlansProjects)
            {
                ListViewItem lstItem = new ListViewItem();
                switch (project.ProjectType)
                {
                    case "KnownToBeMSBuildFormat":
                        //project
                        lstItem.Text = project.ProjectName;
                        lstItem.Tag = project;
                        _mergePlanListView.Items.Add(lstItem);
                        break;
                    case "SolutionFolder":
                        //skip solution folder
                        break;
                    default:
                        break;
                }
            }
            _mergePlanListView.ResumeLayout();
        }

        private void _listview_SelectedIndexChanged(object sender, EventArgs e)
        {
            //load solution detail
            if (_listview.SelectedItems.Count == 0) { return; }
            //only 1
            ListViewItem selectedItem = _listview.SelectedItems[0];
            var project = selectedItem.Tag as SolutionProject;
            if (project == null) { return; }
            //load project detail
            List<ProjectAsmReference> asmRefs = _solutionMx.GetReferenceAsmList(project.RelativePath);
            _asmRefListView.Items.Clear();
            int j = asmRefs.Count;
            for (int i = 0; i < j; ++i)
            {
                _asmRefListView.Items.Add(asmRefs[i]);
            }
        }

        MergeProject CreateMergeProjectPlan(SolutionProject project)
        {

            MergeProject mergePro = new MergeProject();
            List<ProjectAsmReference> asmRefs = _solutionMx.GetReferenceAsmList(project.RelativePath);
            string fullProjectDir = System.IO.Path.GetDirectoryName(_solutionMx.SolutionDir + "\\" + project.RelativePath);
            string projFilename = System.IO.Path.GetFileName(project.RelativePath);

            int j = asmRefs.Count;
            for (int i = 0; i < j; ++i)
            {
                ProjectAsmReference asmRef = asmRefs[i];
                switch (asmRef.Kind)
                {
                    case ProjectAsmReferenceKind.ProjectReference:
                        string result = System.IO.Path.Combine(fullProjectDir, asmRef.Name);
                        if (!System.IO.File.Exists(result))
                        {

                        }
                        mergePro.LoadSubProject(result);

                        break;
                    case ProjectAsmReferenceKind.Reference:

                        mergePro._asmReferences.Add(asmRef.Name);

                        break;
                }
            }
            //----------
            //find 
            var pro = GlobalLoadedProject.LoadProject(fullProjectDir + "\\" + projFilename);
            foreach (var item in pro.AllEvaluatedProperties)
            {
                //select some our interest features
                switch (item.Name)
                {
                    case "DefineConstants":
                        mergePro.DefineConstants = item.EvaluatedValue;
                        break;
                    case "TargetFrameworkVersion":
                        mergePro.TargetFrameworkVersion = item.EvaluatedValue;
                        break;
                }
            }
            return mergePro;
        }
        public void BuildMergeProjectFromSelectedItem()
        {
            if (_currentSelectedMergePro == null) return;
            //--- 


            MergeProject mergePro = CreateMergeProjectPlan(_currentSelectedMergePro);
            //BY_CONVENTION:
            //set target to build output, set to x_autogen_projects dir
            //-----------------------------------
            string rootSlnFolder = _solutionMx.SolutionDir;
            string rightPart;
            string beginAt = _solutionMx.BuildPathRelativeToSolution(Application.ExecutablePath, out rightPart);

            string targetProjectName = _currentSelectedMergePro.ProjectName;//  "PixelFarm.MiniAgg.One";
            string targetProjectFile = targetProjectName + ".csproj";
            string saveProjectName = beginAt + "x_autogen\\" + targetProjectName + "\\" + targetProjectFile;
            string[] asmReferences = mergePro._asmReferences.ToArray();

            //mergePro.MergeAndSave(saveProjectName,
            //   targetProjectName,
            //   "v2.0",
            //   " PIXEL_FARM,PIXEL_FARM_NET20",//additional define constant
            //   asmReferences);

            //-----------
            mergePro.MergeAndSave(saveProjectName,
               targetProjectName,
               mergePro.TargetFrameworkVersion,
               mergePro.DefineConstants,//additional define constant
               asmReferences);

            //-----------
            LinkProjectConverter.ConvertToLinkProject2(
                _solutionMx,
                saveProjectName,
                "x_autogen2\\" + targetProjectName,
                false);//after link project is created, we remove the targetProjectFile
        }
    }

}