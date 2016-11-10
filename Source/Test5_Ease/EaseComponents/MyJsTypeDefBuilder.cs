//MIT, 2015-2016, WinterDev

using System;
using System.Reflection;
using Espresso;
namespace LayoutFarm.Scripting
{
    class MyJsTypeDefinitionBuilder : JsTypeDefinitionBuilder
    {
        //we can customize how to build jstype on specific type 
        static Type typeOfJsTypeAttr = typeof(JsTypeAttribute);
        static Type typeOfJsMethodAttr = typeof(JsMethodAttribute);
        static Type typeOfJsPropertyAttr = typeof(JsPropertyAttribute);
        public MyJsTypeDefinitionBuilder()
        {
            //use built in attr
        }

        static JsMethodAttribute FindJsMethodAttribute(MethodInfo met)
        {
            var customAttrs = met.GetCustomAttributes(typeOfJsMethodAttr, false);
            if (customAttrs != null && customAttrs.Length > 0)
            {
                for (int i = customAttrs.Length - 1; i >= 0; --i)
                {
                    var attr = customAttrs[i] as JsMethodAttribute;
                    if (attr != null)
                    {
                        return attr;
                    }
                }
            }
            return null;
        }
        static JsPropertyAttribute FindJsPropertyAttribute(PropertyInfo propertyInfo)
        {
            var customAttrs = propertyInfo.GetCustomAttributes(typeOfJsMethodAttr, false);
            if (customAttrs != null && customAttrs.Length > 0)
            {
                for (int i = customAttrs.Length - 1; i >= 0; --i)
                {
                    var attr = customAttrs[i] as JsPropertyAttribute;
                    if (attr != null)
                    {
                        return attr;
                    }
                }
            }
            return null;
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
                JsMethodAttribute foundJsMetAttr = FindJsMethodAttribute(met);
                if (foundJsMetAttr != null)
                {
                    typedefinition.AddMember(new JsMethodDefinition(foundJsMetAttr.Name ?? GetProperMemberName(met), met));
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
                JsPropertyAttribute foundJsPropAttr = FindJsPropertyAttribute(property);
                if (foundJsPropAttr != null)
                {
                    typedefinition.AddMember(new JsPropertyDefinition(foundJsPropAttr.Name ?? GetProperMemberName(property), property));
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
            //********************
            //this is sample only
            //you can rewrite this yourself
            //********************
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
            return BuildTypeDefinition(t, typedefinition);

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