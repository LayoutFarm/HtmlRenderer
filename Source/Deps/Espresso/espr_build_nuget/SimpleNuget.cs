//MIT, 2016, WinterDev
using System;
using System.Text;

namespace LayoutFarm.SimpleNuGet
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

    static class StBuilderExt
    {
        public static void Elm(this StringBuilder stbuilder, string elementName, string textContent)
        {
            if (textContent == null)
            {
                throw new Exception("content must not be null");
            }
            stbuilder.Append("<" + elementName + ">");
            stbuilder.Append(textContent);
            stbuilder.Append("</" + elementName + ">");
        }
        public static void Elm(this StringBuilder stbuilder, string elementName, bool value)
        {
            Elm(stbuilder, elementName, value.ToString().ToLower());
        }
        public static void B(this StringBuilder stbuilder, string elementName)
        {
            stbuilder.Append("<");
            stbuilder.Append(elementName);
            stbuilder.Append(">");
        }
        public static void E(this StringBuilder stbuilder, string elementName)
        {
            stbuilder.Append("</");
            stbuilder.Append(elementName);
            stbuilder.Append(">");
        }
    }

    class PackageDependency
    {
        public string PackageId { get; set; }
        public string Version { get; set; }

        public void WriteTo(StringBuilder stbuilder)
        {
            stbuilder.Append("<dependency id=\"" + PackageId + "\" version=\"" + Version + "\"></dependency>");
        }
    }
    class Package
    {
        public string Name { get; set; }
        public FileMap[] Files { get; set; }
        public PackageDependency[] Dependencies { get; set; }
        public string Id { get; set; }
        public string Version { get; set; }
        public string Authors { get; set; }
        public string Owners { get; set; }
        public string Description { get; set; }
        public string LicenseUrl { get; set; } //**opt
        public string ProjectUrl { get; set; }//**opt
        public string IconUrl { get; set; }//**opt

        public bool RequiredLicenseAcceptance { get; set; }
        public string ReleaseNotes { get; set; }

        public string CopyRight { get; set; }
        public string Tags { get; set; }//**opt 
        //------------------------------------------------------------
        public StringBuilder CreateDocument()
        {
            StringBuilder st = new StringBuilder();
            //----
            //just a string !
            st.Append("<?xml version=\"1.0\"?>");
            st.B("package");
            {
                st.B("metadata");
                {

                    st.Elm("id", Id);
                    st.Elm("version", Version);
                    st.Elm("authors", Authors);
                    st.Elm("owners", Owners);
                    if (ProjectUrl != null) { st.Elm("projectUrl", ProjectUrl); }
                    if (LicenseUrl != null) { st.Elm("licenseUrl", LicenseUrl); }
                    if (IconUrl != null) { st.Elm("iconUrl", IconUrl); }

                    st.Elm("requireLicenseAcceptance", RequiredLicenseAcceptance);
                    st.Elm("description", Description);
                    st.Elm("releaseNotes", ReleaseNotes);
                    st.Elm("copyright", CopyRight);
                    if (Tags != null) { st.Elm("tags", Tags); }
                    //-----------------------------------
                    st.B("dependencies");
                    {
                        foreach (PackageDependency dep in Dependencies)
                        {
                            dep.WriteTo(st);
                        }
                    }
                    st.E("dependencies");
                    //-----------------------------------
                }
                st.E("metadata");
                //-----------------------------------
                st.B("files");
                foreach (FileMap filemap in Files)
                {
                    filemap.WriteTo(st);
                }
                st.E("files");
                //-----------------------------------
            }
            st.E("package");

            return st;
        }
        static void AppendElement(StringBuilder stbuilder, string elementName, string textContent)
        {
            if (textContent == null)
            {
                throw new Exception("content must not be null");
            }
            stbuilder.Append("<" + elementName + ">");
            stbuilder.Append(textContent);
            stbuilder.Append("</" + elementName + ">");
        }
        static void AppendElement(StringBuilder stbuilder, string elementName, bool value)
        {
            AppendElement(stbuilder, elementName, value.ToString().ToLower());
        }
    }
    class FileMap
    {
        public FileMap()
        {

        }
        /// <summary>
        /// relative path to src file
        /// </summary>
        public string RelativeSourceFile { get; set; }
        /// <summary>
        /// target in the asm
        /// </summary>
        public string Target { get; set; }
        public void WriteTo(StringBuilder stbuilder)
        {
            stbuilder.Append("<file src=\"" + RelativeSourceFile + "\" target=\"" + Target + "\"></file>");
        }
    }


}