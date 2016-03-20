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

        JsTypeDefinition BuildTypeDefinition(Type t, JsTypeDefinition typedefinition)
        {


            var methods = t.GetMethods(System.Reflection.BindingFlags.Instance |
               System.Reflection.BindingFlags.Public);

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
                else
                {
                    typedefinition.AddMember(new JsMethodDefinition(met.Name ?? GetProperMemberName(met), met));
                }

            }
            var properties = t.GetProperties(System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.Public);

            foreach (var property in properties)
            {
                var customAttrs = property.GetCustomAttributes(typeOfJsPropertyAttr, false);
                if (customAttrs != null && customAttrs.Length > 0)
                {
                    var attr = customAttrs[0] as JsPropertyAttribute;
                    typedefinition.AddMember(new JsPropertyDefinition(attr.Name ?? GetProperMemberName(property), property));
                }
                else
                {
                    typedefinition.AddMember(new JsPropertyDefinition(property.Name ?? GetProperMemberName(property), property));
                }
            }
            return typedefinition;
        }
        protected override JsTypeDefinition OnBuildRequest(Type t)
        {
            JsTypeDefinition typedefinition = new JsTypeDefinition(t.Name);
            if (t.IsInterface)
            {
                return BuildTypeDefinition(t, typedefinition);
            }
            //-----------
            //find js web interface
            Type[] jsWebInterfaces = t.GetInterfaces();
            if (jsWebInterfaces.Length > 0)
            {
                //TODO: review here again
                //we select only last interface?
                BuildTypeDefinition(jsWebInterfaces[jsWebInterfaces.Length - 1], typedefinition);
                return typedefinition;
            }
            //-----------
            //find member that has JsPropertyAttribute or JsMethodAttribute

            //only instance /public method /prop***
            var methods = t.GetMethods(System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public);

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
            var properties = t.GetProperties(System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.Public);

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