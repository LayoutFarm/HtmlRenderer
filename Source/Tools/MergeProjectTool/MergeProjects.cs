//MIT, 2016-2017, WinterDev
//credit: http://stackoverflow.com/questions/24557807/programmatically-create-a-csproj-on-the-fly-without-visual-studio-installed

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Reflection;

using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;

namespace BuildMergeProject
{
    public static class StartupConfig
    {
        public static string defaultSln;
    }

    public enum ProjectAsmReferenceKind
    {
        ProjectReference,
        Reference
    }
    public class ProjectAsmReference
    {

        public ProjectAsmReference(ProjectItem proItem, ProjectAsmReferenceKind kind)
        {
            this.Name = proItem.EvaluatedInclude;
            this.Kind = kind;
            this.Item = proItem;
        }
        public ProjectItem Item { get; set; }
        public ProjectAsmReferenceKind Kind { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Kind + " : " + Name;
        }

    }


    public static class GlobalLoadedProject
    {
        static Dictionary<string, Project> s_loadedProjects = new Dictionary<string, Project>();
        public static Project LoadProject(string projectFilename)
        {
            Project found;
            if (!s_loadedProjects.TryGetValue(projectFilename, out found))
            {
                found = new Project(projectFilename);
                return s_loadedProjects[projectFilename] = found;
            }
            return found;
        }
    }

    public class SolutionMx
    {
        public Solution _currentSolution;
        public string SolutionDir { get; set; }
        public string FullPath { get; set; }

        public void ReadSolution(string path)
        {
            this.FullPath = path;
            this.SolutionDir = Path.GetDirectoryName(path);
            //--------------
            //since this should be run on .net4,
            //so we read 
            //we just read what project are in the solution
            //easy!
            //--------------
            _currentSolution = new Solution(path);
            //foreach (SolutionProject pro in solution.Projects)
            //{
            //    string proName = pro.ProjectName;
            //    string relPath = pro.RelativePath; 
            //}
        }
        public static string CombineRelativePath(string exePath)
        {
            string[] sub_paths = exePath.Split('\\');
            List<string> totals = new List<string>();
            int j = sub_paths.Length;

            for (int i = 0; i < j; ++i)
            {
                string p = sub_paths[i];
                if (p == "..")
                {
                    //remove latest data in str
                    if (totals.Count > 0)
                    {
                        //remove the last one
                        totals.RemoveAt(totals.Count - 1);
                    }
                    else
                    {

                    }
                }
                else
                {

                    totals.Add(p);
                }
            }


            System.Text.StringBuilder stbuilder = new System.Text.StringBuilder();
            j = totals.Count;
            for (int i = 0; i < j; ++i)
            {
                stbuilder.Append(totals[i]);
                if (i != j - 1)
                {
                    stbuilder.Append("\\");
                }
            }

            return stbuilder.ToString();
        }
        public string BuildPathRelativeToSolution(string subpath, out string rightPart)
        {
            //-----------------------------------
            string rootSlnFolder = this.SolutionDir;
            //exe dir 
            if (!subpath.StartsWith(rootSlnFolder))
            {
                throw new NotSupportedException();
            }
            string sub = subpath.Substring(rootSlnFolder.Length);
            string[] sub_steps = sub.Split('\\');
            //step up to reach solution folder
            int nsteps = sub_steps.Length - 2;
            string beginAt = "";
            for (int i = 0; i < nsteps; ++i)
            {
                beginAt += "..\\";
            }
            rightPart = sub;
            return beginAt;
        }
        static int FindFirstDiff(string[] s0_splits, string[] s1_splits)
        {
            int j = 0;
            if ((j = s0_splits.Length) <= s1_splits.Length)
            {
                for (int i = 0; i < j; ++i)
                {
                    //compare part by part
                    string s0_p = s0_splits[i];
                    string s1_p = s1_splits[i];
                    if (s0_p != s1_p)
                    {
                        //stop at this part
                        return i;
                    }
                    else
                    {
                        //next
                    }
                }
                return j - 1;
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        public string BuildPathRelativeToOther(string mainPath, string subpath, out string rightPart)
        {
            //s1.Length must >= s0.Length

            string[] s0_splits = mainPath.Split('\\');
            string[] s1_splits = subpath.Split('\\');

            int diffPos = FindFirstDiff(s0_splits, s1_splits);
            //same at diffPos-1

            int nsteps = diffPos - 1;
            string beginAt = "";
            for (int i = 0; i < nsteps; ++i)
            {
                beginAt += "..\\";
            }
            rightPart = "";
            int j = s1_splits.Length;
            int m = 0;
            for (int n = diffPos; n < j; ++n)
            {
                if (m > 0 && m < j - 1)
                {
                    rightPart += '\\';
                }
                rightPart += s1_splits[n];
                m++;

            }
            return beginAt;
        }
        public string GetFullProjectPath(string projectRelativePath)
        {
            string result = Path.Combine(this.SolutionDir, projectRelativePath);
            return result;
        }

        public List<ProjectAsmReference> GetReferenceAsmList(string projectFile)
        {
            string fullProjectName = GetFullProjectPath(projectFile);
            List<ProjectAsmReference> asmReferenceList = new List<ProjectAsmReference>();
            Project pro = GlobalLoadedProject.LoadProject(fullProjectName);

            foreach (ProjectItem item in pro.AllEvaluatedItems)
            {
                switch (item.ItemType)
                {
                    default:
                        {

                        }
                        break;
                    case "BootstrapperPackage":
                    case "None":
                        break;
                    case "Compile":
                        //skip
                        break;
                    //    {
                    //        string onlyFileName = Path.GetFileName(item.EvaluatedInclude);
                    //        if (onlyFileName != "AssemblyInfo.cs") //special case ***no include this file
                    //        {
                    //            allItems.Add(item);
                    //        }
                    //    }
                    //    break;
                    case "ProjectReference":
                        asmReferenceList.Add(new ProjectAsmReference(item, ProjectAsmReferenceKind.ProjectReference));
                        break;
                    case "Reference":
                        asmReferenceList.Add(new ProjectAsmReference(item, ProjectAsmReferenceKind.Reference));
                        break;
                }
            }
            return asmReferenceList;
        }
    }

    public static class LinkProjectConverter
    {
        public static void ConvertToLinkProject2(SolutionMx slnMx, string srcProject, string autoGenFolder, bool removeOriginalSrcProject)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(srcProject);
            var compileNodes = SelectCompileNodes(xmldoc.DocumentElement);
            string onlyFileName = Path.GetFileName(srcProject);
            string saveFileName = slnMx.SolutionDir + "\\" + autoGenFolder + "\\" + onlyFileName;
            string targetSaveFolder = slnMx.SolutionDir + "\\" + autoGenFolder;
            string rightPart;

            string beginAt = slnMx.BuildPathRelativeToSolution(targetSaveFolder, out rightPart);
            foreach (XmlElement elem in compileNodes)
            {
                XmlAttribute includeAttr = elem.GetAttributeNode("Include");
                string includeValue = includeAttr.Value;
                string combinedPath = SolutionMx.CombineRelativePath(includeValue);


                string b2 = slnMx.BuildPathRelativeToOther(targetSaveFolder, combinedPath, out rightPart);
                //this version:
                //auto gen project is lower than original 1 level
                //so change the original src location
                //and create linked child node
                includeAttr.Value = "..\\" + beginAt + rightPart;


                XmlElement linkNode = xmldoc.CreateElement("Link", elem.NamespaceURI);
                linkNode.InnerText = rightPart;
                elem.AppendChild(linkNode);
            }

            string targetSaveDir = System.IO.Path.GetDirectoryName(saveFileName);
            if (!Directory.Exists(targetSaveDir))
            {
                Directory.CreateDirectory(targetSaveDir);
            }

            xmldoc.Save(saveFileName);
            if (removeOriginalSrcProject)
            {
                File.Delete(srcProject);
            }
        }
        public static void ConvertToLinkProject(string srcProject, string autoGenFolder, bool removeOriginalSrcProject)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(srcProject);
            var compileNodes = SelectCompileNodes(xmldoc.DocumentElement);
            foreach (XmlElement elem in compileNodes)
            {
                XmlAttribute includeAttr = elem.GetAttributeNode("Include");
                string includeValue = includeAttr.Value;
                //this version:
                //auto gen project is lower than original 1 level
                //so change the original src location
                //and create linked child node
                includeAttr.Value = "..\\" + includeAttr.Value;
                XmlElement linkNode = xmldoc.CreateElement("Link", elem.NamespaceURI);
                linkNode.InnerText = includeValue;
                elem.AppendChild(linkNode);
            }

            string onlyFileName = Path.GetFileName(srcProject);
            if (!Directory.Exists(autoGenFolder))
            {
                Directory.CreateDirectory(autoGenFolder);
            }
            xmldoc.Save(autoGenFolder + "\\" + onlyFileName);
            if (removeOriginalSrcProject)
            {
                File.Delete(srcProject);
            }
        }
        static List<XmlElement> SelectCompileNodes(XmlElement projectNode)
        {
            //TODO: use xpath ...

            List<XmlElement> compileNodes = new List<XmlElement>();
            foreach (XmlElement item in projectNode)
            {
                if (item.Name == "ItemGroup")
                {
                    foreach (XmlElement item2 in item)
                    {
                        if (item2.Name == "Compile")
                        {
                            compileNodes.Add(item2);
                        }
                    }
                }
            }
            return compileNodes;
        }
    }
    public class MergeProject
    {
        List<ToMergeProject> subProjects = new List<ToMergeProject>();
        public List<string> _asmReferences = new List<string>();

        bool portable;
        public MergeProject(bool portable = false)
        {
            this.portable = portable;
        }
        public void LoadSubProject(string projectFile)
        {
            ToMergeProject pro = new ToMergeProject();
            pro.Load(projectFile);
            subProjects.Add(pro);
        }
        public string DefineConstants { get; set; }
        public string TargetFrameworkVersion { get; set; }

        static ProjectPropertyGroupElement CreatePropertyGroup(ProjectRootElement root,
           string targetFrameworkVersion,
           string condition, string platform_condition,
           string configuration, string platform, bool unsafeMode,
           string assemblyName)
        {
            ProjectPropertyGroupElement group = root.AddPropertyGroup();
            var config = group.AddProperty("Configuration", configuration);
            config.Condition = condition;
            var platform_ = group.AddProperty("Platform", platform);
            platform_.Condition = platform_condition;
            group.AddProperty("SchemaVersion", "2.0");
            group.AddProperty("OutputType", "Library");
            group.AddProperty("TargetFrameworkVersion", targetFrameworkVersion);
            group.AddProperty("FileAlignment", "512");
            group.AddProperty("AssemblyName", assemblyName);
            return group;
        }
        static ProjectPropertyGroupElement CreatePropertyGroupChoice(ProjectRootElement root,
            string condition,
             bool unsafeMode, string outputPath, bool optimize,
            bool debugSymbol, string debugType, string constants
            )
        {
            ProjectPropertyGroupElement group = root.AddPropertyGroup();
            group.Condition = condition;
            if (unsafeMode)
            {
                group.AddProperty("AllowUnsafeBlocks", "true");
            }
            group.AddProperty("ErrorReport", "prompt");
            group.AddProperty("WarningLevel", "4");
            group.AddProperty("OutputPath", outputPath); //,eg  @"bin\Debug\"
            group.AddProperty("Optimize", optimize ? "true" : "false");
            group.AddProperty("DebugType", debugType);
            if (debugSymbol)
            {
                group.AddProperty("DebugSymbols", "true");
            }
            group.AddProperty("DefineConstants", constants); //eg DEBUG; TRACE             
            return group;
        }

        public void MergeAndSave(string csprojFilename, string assemblyName, string targetFrameworkVersion, string additonalDefineConst, string[] references)
        {
            ProjectRootElement root = ProjectRootElement.Create();
            if (portable)
            {
                root.ToolsVersion = "14.0";
                root.DefaultTargets = "Build";
                var import = root.AddImport(@"$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props");
                import.Condition = @"Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')";
            }
            else
            {
                root.AddImport(@"$(MSBuildToolsPath)\Microsoft.CSharp.targets");
            }
            ProjectPropertyGroupElement one1 = CreatePropertyGroup(root,
                targetFrameworkVersion,
                " '$(Configuration)' == '' ",
                " '$(Platform)' == '' ",
                "Debug", "AnyCPU", true, assemblyName);
            if (portable)
            {
                one1.AddProperty("MinimumVisualStudioVersion", "10.0");
                one1.AddProperty("ProjectTypeGuids", "{786C830F-07A1-408B-BD7F-6EE04809D6DB};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}");
                one1.AddProperty("TargetFrameworkProfile", "Profile111");
            }

            ProjectPropertyGroupElement debugGroup = CreatePropertyGroupChoice(root,
                " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ",
                  true,
                @"bin\Debug\", false, true, "full", "DEBUG; TRACE" + additonalDefineConst);
            ProjectPropertyGroupElement releaseGroup = CreatePropertyGroupChoice(root,
                " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ",
                  true,
                @"bin\Release\", true, false, "pdbonly", " TRACE" + additonalDefineConst);
            if (references.Length > 0)
            {
                AddItems(root, "Reference", references);
            }
            List<string> allList = new List<string>();
            Dictionary<string, bool> uniqueFileList = new Dictionary<string, bool>();
            string onlyProjPath = Path.GetDirectoryName(csprojFilename) + "\\";
            int onlyProjPathLength = onlyProjPath.Length;
            //TODO: review here
            //special for support .net20
            bool foundFirstExtensionAttributeFile = false;
            foreach (ToMergeProject toMergePro in subProjects)
            {
                List<string> allAbsFiles = toMergePro.GetAllAbsoluteFilenames();
                foreach (string filename in allAbsFiles)
                {
                    string onlyFileName = Path.GetFileName(filename);

                    if (onlyFileName == "PORTING_NOTMERGE.cs")
                    {
                        //our convention
                        continue;//skip
                    } 
                    else if (onlyFileName == "ExtensionAttribute.cs")
                    {    //this is our convention
                         //... if we have ExtensionAttribute.cs
                         //the 
                        if (foundFirstExtensionAttributeFile)
                        {
                            continue;
                        }
                        else
                        {
                            foundFirstExtensionAttributeFile = true;
                        }
                    }

                    string combindedFilename = SolutionMx.CombineRelativePath(filename).ToUpper();
                    if (uniqueFileList.ContainsKey(combindedFilename))
                    {
                        continue;
                    }
                    uniqueFileList[combindedFilename] = true;//
                    if (filename.StartsWith(onlyProjPath))
                    {
                        allList.Add(filename.Substring(onlyProjPathLength));
                    }
                    else
                    {
                        allList.Add(filename);
                    }
                }
            }
            // items to compile
            AddItems(root, "Compile", allList.ToArray());
            if (portable)
            {
                root.AddImport(@"$(MSBuildExtensionsPath32)\Microsoft\Portable\$(TargetFrameworkVersion)\Microsoft.Portable.CSharp.targets");
            }

            root.Save(csprojFilename);
        }
        static void AddItems(ProjectRootElement elem, string groupName, params string[] items)
        {
            ProjectItemGroupElement group = elem.AddItemGroup();
            foreach (var item in items)
            {
                ProjectItemElement projItem = group.AddItem(groupName, item);
                //if (groupName == "Compile")
                //{
                //    projItem.AddMetadata("Link", item);
                //}
            }
        }

    }
    public class ToMergeProject
    {
        List<ProjectItem> allItems = new List<ProjectItem>();
        public string ProjectFileName { get; set; }
        public string DefineConstants { get; set; }
        public string TargetFrameworkVersion { get; set; }

        public void Load(string projectFile)
        {
            this.ProjectFileName = projectFile;

            Project pro = GlobalLoadedProject.LoadProject(projectFile);

            foreach (var item in pro.AllEvaluatedProperties)
            {
                //select some our interest features
                switch (item.Name)
                {
                    case "DefineConstants":
                        DefineConstants = item.EvaluatedValue;
                        break;
                    case "TargetFrameworkVersion":
                        TargetFrameworkVersion = item.EvaluatedValue;
                        break;
                }
            }

            foreach (ProjectItem item in pro.AllEvaluatedItems)
            {
                switch (item.ItemType)
                {
                    case "Compile":
                        {
                            string onlyFileName = Path.GetFileName(item.EvaluatedInclude);
                            if (onlyFileName != "AssemblyInfo.cs") //special case ***no include this file
                            {
                                allItems.Add(item);
                            }
                        }
                        break;
                    case "Reference":
                        break;
                }
            }
        }
        public List<string> GetAllAbsoluteFilenames()
        {
            List<string> absFilenames = new List<string>();
            string projectDirName = Path.GetDirectoryName(ProjectFileName);
            foreach (var item in allItems)
            {
                string filename = item.EvaluatedInclude;
                if (!Path.IsPathRooted(filename))
                {
                    absFilenames.Add(projectDirName + "\\" + filename);
                }
                else
                {
                    absFilenames.Add(filename);
                }

            }

            return absFilenames;
        }
    }


    public class Solution
    {
        //from http://stackoverflow.com/questions/707107/parsing-visual-studio-solution-files
        //internal class SolutionParser
        //Name: Microsoft.Build.Construction.SolutionParser
        //Assembly: Microsoft.Build, Version=4.0.0.0

        static readonly Type s_SolutionParser;
        static readonly PropertyInfo s_SolutionParser_solutionReader;
        static readonly MethodInfo s_SolutionParser_parseSolution;
        static readonly PropertyInfo s_SolutionParser_projects;

        static Solution()
        {
            s_SolutionParser = Type.GetType("Microsoft.Build.Construction.SolutionParser, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            if (s_SolutionParser != null)
            {
                s_SolutionParser_solutionReader = s_SolutionParser.GetProperty("SolutionReader", BindingFlags.NonPublic | BindingFlags.Instance);
                s_SolutionParser_projects = s_SolutionParser.GetProperty("Projects", BindingFlags.NonPublic | BindingFlags.Instance);
                s_SolutionParser_parseSolution = s_SolutionParser.GetMethod("ParseSolution", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        public List<SolutionProject> Projects { get; private set; }

        public Solution(string solutionFileName)
        {
            if (s_SolutionParser == null)
            {
                throw new InvalidOperationException("Can not find type 'Microsoft.Build.Construction.SolutionParser' are you missing a assembly reference to 'Microsoft.Build.dll'?");
            }
            var solutionParser = s_SolutionParser.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0].Invoke(null);
            using (var streamReader = new StreamReader(solutionFileName))
            {
                s_SolutionParser_solutionReader.SetValue(solutionParser, streamReader, null);
                s_SolutionParser_parseSolution.Invoke(solutionParser, null);
            }
            var projects = new List<SolutionProject>();
            var array = (Array)s_SolutionParser_projects.GetValue(solutionParser, null);
            for (int i = 0; i < array.Length; i++)
            {
                projects.Add(new SolutionProject(array.GetValue(i)));
            }
            this.Projects = projects;
        }
    }


    [DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectGuid}")]
    public class SolutionProject
    {
        static readonly Type s_ProjectInSolution;
        static readonly PropertyInfo s_ProjectInSolution_ProjectName;
        static readonly PropertyInfo s_ProjectInSolution_RelativePath;
        static readonly PropertyInfo s_ProjectInSolution_ProjectGuid;
        static readonly PropertyInfo s_ProjectInSolution_ProjectType;

        static SolutionProject()
        {
            s_ProjectInSolution = Type.GetType("Microsoft.Build.Construction.ProjectInSolution, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            if (s_ProjectInSolution != null)
            {
                s_ProjectInSolution_ProjectName = s_ProjectInSolution.GetProperty("ProjectName", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ProjectInSolution_RelativePath = s_ProjectInSolution.GetProperty("RelativePath", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ProjectInSolution_ProjectGuid = s_ProjectInSolution.GetProperty("ProjectGuid", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ProjectInSolution_ProjectType = s_ProjectInSolution.GetProperty("ProjectType", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        public string ProjectName { get; private set; }
        public string RelativePath { get; private set; }
        public string ProjectGuid { get; private set; }
        public string ProjectType { get; private set; }

        public SolutionProject(object solutionProject)
        {
            this.ProjectName = s_ProjectInSolution_ProjectName.GetValue(solutionProject, null) as string;
            this.RelativePath = s_ProjectInSolution_RelativePath.GetValue(solutionProject, null) as string;
            this.ProjectGuid = s_ProjectInSolution_ProjectGuid.GetValue(solutionProject, null) as string;
            object o = s_ProjectInSolution_ProjectType.GetValue(solutionProject, null);
            this.ProjectType = s_ProjectInSolution_ProjectType.GetValue(solutionProject, null).ToString();
        }
    }
}