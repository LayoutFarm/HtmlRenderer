// Copyright (c) 2007-2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("icu.net")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("SIL International")]
[assembly: AssemblyProduct("icu.net")]
[assembly: AssemblyCopyright("Copyright © SIL International 2007-2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3439151b-347b-4321-9f2f-d5fa28b46477")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers
// by using the '*' as shown below:
// (The AssemblyVersion needs to match the version that Palaso builds against.)
// (The AssemblyFileVersion matches the ICU version number we're building against.)
[assembly: AssemblyVersion("4.2.1.0")]
#if ICU_VERSION_40
[assembly: AssemblyFileVersion("4.2.1.*")]
#elif ICU_VERSION_48
[assembly: AssemblyFileVersion("4.8.1.1")]
#else
[assembly: AssemblyFileVersion("5.0.0.2")]
#endif
