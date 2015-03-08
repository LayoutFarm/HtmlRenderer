//2015, MIT ,WinterDev

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

using VroomJs;

namespace LayoutFarm.Scripting
{
    class MyJsTypeDefinitionBuilder : JsTypeDefinitionBuilder
    {
        //we can customize how to build jstype on specific type


        Type typeOfJsTypeAttr = typeof(JsTypeAttribute);
        Type typeOfJsMethodAttr = typeof(JsMethodAttribute);
        Type typeOfJsPropertyAttr = typeof(JsPropertyAttribute);

        public MyJsTypeDefinitionBuilder()
        {
            //use built in attr
        }
        protected override JsTypeDefinition OnBuildRequest(Type t)
        {

            //find member that has JsPropertyAttribute or JsMethodAttribute
            JsTypeDefinition typedefinition = new JsTypeDefinition(t.Name);

            //only instance /public method /prop***
            var methods = t.GetMethods(System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            foreach (var met in methods)
            {
                if (met.IsSpecialName)
                {
                    continue;
                }
                var customAttrs = met.GetCustomAttributes(typeOfJsMethodAttr, false);
                if (customAttrs != null && customAttrs.Length > 0)
                {
                    var attr = customAttrs[0] as JsMethodAttribute;

                    typedefinition.AddMember(new JsMethodDefinition(attr.Name ?? GetProperMemberName(met), met));
                }
            }
            var properties = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var property in properties)
            {
                var customAttrs = property.GetCustomAttributes(typeOfJsPropertyAttr, false);
                if (customAttrs != null && customAttrs.Length > 0)
                {
                    var attr = customAttrs[0] as JsPropertyAttribute;
                    typedefinition.AddMember(new JsPropertyDefinition(attr.Name ?? GetProperMemberName(property), property));
                }

            }
            return typedefinition;
        }
        static string GetProperMemberName(System.Reflection.MemberInfo mbInfo)
        {
            int dotpos = mbInfo.Name.LastIndexOf('.');
            if (dotpos > 0)
            {
                
                return mbInfo.Name.Substring(dotpos + 1);
            }
            else
            {
                return mbInfo.Name;
            }

        }
    }

}