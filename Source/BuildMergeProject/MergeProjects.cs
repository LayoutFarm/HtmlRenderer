//MIT 2016, WinterDev
//credit: http://stackoverflow.com/questions/24557807/programmatically-create-a-csproj-on-the-fly-without-visual-studio-installed

using System;
using System.Collections.Generic;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using System.IO;
namespace BuildMergeProject
{
    class MergeProject
    {
        List<ToMergeProject> subProjects = new List<ToMergeProject>();
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
            string onlyProjPath = Path.GetDirectoryName(csprojFilename) + "\\";
            int onlyProjPathLength = onlyProjPath.Length;
            //TODO: review here
            //special for support .net20
            bool foundFirstExtensionAttributeFile = false;
            foreach (var toMergePro in subProjects)
            {
                var allAbsFiles = toMergePro.GetAllAbsoluteFilenames();
                foreach (var filename in allAbsFiles)
                {
                    string onlyFileName = Path.GetFileName(filename);
                    if (onlyFileName == "ExtensionAttribute.cs")
                    {
                        if (foundFirstExtensionAttributeFile)
                        {
                            continue;
                        }
                        else
                        {
                            foundFirstExtensionAttributeFile = true;
                        }
                    }
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
            var group = elem.AddItemGroup();
            foreach (var item in items)
            {
                group.AddItem(groupName, item);
            }
        }
    }
    class ToMergeProject
    {
        List<ProjectItem> allItems = new List<ProjectItem>();
        public string ProjectFileName { get; set; }
        public void Load(string projectFile)
        {
            this.ProjectFileName = projectFile;
            var pro = new Project(projectFile);
            foreach (var item in pro.AllEvaluatedItems)
            {
                switch (item.ItemType)
                {
                    case "Compile":
                        {
                            string onlyFileName = Path.GetFileName(item.EvaluatedInclude);
                            if (onlyFileName != "AssemblyInfo.cs")
                            {
                                //TODO: review here
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
                absFilenames.Add(projectDirName + "\\" + filename);
            }

            return absFilenames;
        }
    }
}
