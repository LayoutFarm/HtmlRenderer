using System;
using System.Collections.Generic;
using System.Reflection;
namespace Espresso.Extension
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
    public partial class ExtensionAttribute : Attribute { }
}
#endif