//MIT, 2017-present, WinterDev

using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace BuildMergeProject
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            var walker = new SimpleDirWalker(System.IO.Directory.GetCurrentDirectory());
            string sourceDir = walker.StepBack(4)
                                     .EnsureCurrentDirName("Source")
                                     .GetFullCurrentDirName();

            StartupConfig.defaultSln = walker.LookupForFile("HtmlRenderer.sln");

            if (!System.IO.File.Exists(StartupConfig.defaultSln))
            {
                MessageBox.Show("Err,file not found:" + walker.GetFullFilename("HtmlRenderer.sln"));
                return;
            }
            else
            {
                //
            }

            Application.Run(new FormBuildMergeProject());
        }

        /// <summary>
        /// simple directory path walker util
        /// </summary>
        class SimpleDirWalker
        {
            
            readonly string _initAbsPath;
            string[] _splitPaths;
            int _current_index;

            static readonly string[] s_pathSeps = new string[] { "/", "\\" };

            public SimpleDirWalker(string initAbsPath)
            {
                _initAbsPath = initAbsPath;
                _splitPaths = initAbsPath.Split(s_pathSeps, StringSplitOptions.RemoveEmptyEntries);
                _current_index = _splitPaths.Length - 1;
            }
            public SimpleDirWalker StepBack(int ntimes)
            {
                _current_index -= ntimes;
                return this;//use fluent style
            }
            public SimpleDirWalker EnsureCurrentDirName(string dirname)
            {
                if (_splitPaths[_current_index] == dirname)
                {
                    return this;
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            /// <summary>
            /// check if there is a filename in current dir or not
            /// </summary>
            /// <param name="onlyFilename"></param>
            /// <returns></returns>
            public string LookupForFile(string onlyFilename)
            {
                string fullname = GetFullFilename(onlyFilename);
                return (System.IO.File.Exists(fullname)) ? fullname : null;
            }
            public string GetFullFilename(string onlyFilename)
            {
                return GetFullCurrentDirName() + System.IO.Path.DirectorySeparatorChar + onlyFilename;
            }
            public string GetFullCurrentDirName()
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i <= _current_index; ++i)
                {
                    if (i > 0)
                    {
                        sb.Append(System.IO.Path.DirectorySeparatorChar);
                    }
                    sb.Append(_splitPaths[i]);
                }
                return sb.ToString();
            }
        }

    }

}

