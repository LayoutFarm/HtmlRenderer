//MIT, 2016, WinterDev
using System;
using System.IO;
using System.Text;

using LayoutFarm.SimpleNuGet;

namespace espr_build_nuget
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //basic manual build nuget package for Espresso            
            //-----------------------------
            //sample target output
            //-----------------------------
            /*<?xml version="1.0"?>
            <package >
              <metadata>
                <id>Espresso</id>
                <version>1.0.46</version>
                <authors>win</authors>
                <owners>win</owners> 
                <requireLicenseAcceptance>false</requireLicenseAcceptance>
                <description>C# &lt;3 v8</description>
                <releaseNotes>pre-release</releaseNotes>
                <copyright>MIT</copyright>
                <tags>Tag1 Tag2</tags>
                <dependencies>
                  <dependency id="NETStandard.Library" version="1.6" />
                </dependencies> 
              </metadata> 
              <files>
	            <file src="Package\lib\netstandard1.6\EspressoCore.dll" target="lib\netstandard1.6\EspressoCore.dll"></file>
	            <file src="win7-x64\libespr.dll" target="runtimes\win7-x64\native\libespr.dll"></file> 
	            <file src="win7-x86\libespr.dll" target="runtimes\win7-x86\native\libespr.dll"></file> 	
              </files>    
            </package>*/
            //-----------------------------

            //-----------------------------
            //1.
            //note: ? if Name = managed asm name (eg. EspressoCore.dll)
            //the native dll will not found, so I choose only 'LayoutFarm.Espresso'

            Package package = new Package()
            {
                Name = "LayoutFarm.Espresso", 
                Id = "LayoutFarm.Espresso",
                Version = "1.0.57",
                Authors = "win",
                Owners = "win",
                RequiredLicenseAcceptance = false,
                Description = "C# &lt;3 v8",
                ReleaseNotes = "pre-release",
                CopyRight = "MIT",
                Tags = ".NETCORE V8 C# LayoutFarm Espresso",
                Dependencies = new[] { new PackageDependency() { PackageId = "NETStandard.Library", Version = "1.6" } },
                //-------------------------------------------------------------------------------------
                Files = new[]
                {
                       //this is managed file,
                    new FileMap {RelativeSourceFile=@"..\EspressoCore\bin\Release\netstandard1.6\EspressoCore.dll",
                                Target= @"lib\netstandard1.6\EspressoCore.dll" }, //** for .net standard we must put to pre-defined target
                    //-----------------------------------------------
                    //this is native dll***
                    new FileMap { RelativeSourceFile=@"..\native\v6.3.0_win64_release\libespr.dll",
                                Target= @"runtimes\win7-x64\native\libespr.dll"}, //*** must stay in this layout
                    //-----------------------------------------------
                   //this is native dll***
                    new FileMap { RelativeSourceFile=@"..\native\v6.3.0_win32_release\libespr.dll",
                                Target= @"runtimes\win7-x86\native\libespr.dll"} //*** must stay in this layout
                    //----------------------------------------------- 
                }
            };

            //2. check if the output is match with the template?
            StringBuilder st = package.CreateDocument();
            System.IO.File.WriteAllText(package.Name + ".nuspec", st.ToString());

            //3. build nuget package
            var startup = new System.Diagnostics.ProcessStartInfo("nuget", "pack " + package.Name + ".nuspec"); 
            System.Diagnostics.Process.Start(startup);


        }
    }



}
