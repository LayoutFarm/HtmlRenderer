using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace VroomJs.Extension
{
    public static class TypeExtention
    {
        public static MemberInfo[] GetMembers(this Type type)
        {

#if NET20
            var members = type.GetMembers();
            List<MemberInfo> memList = new List<MemberInfo>();
            foreach (var mem in members)
            {
                memList.Add(mem);
            }
            return memList.ToArray();
#else
            var members = type.GetTypeInfo().DeclaredMembers;
            List<MemberInfo> memList = new List<MemberInfo>();
            foreach (var mem in members)
            {
                memList.Add(mem);
            }
            return memList.ToArray();
#endif
        }
    }
}

#if NET20
namespace System.Runtime.CompilerServices
{
    public class ExtensionAttribute : Attribute { }
}
#endif