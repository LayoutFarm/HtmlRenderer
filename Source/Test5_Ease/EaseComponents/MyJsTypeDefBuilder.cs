//MIT, 2015-2016, WinterDev

using System;
using System.Collections.Generic;
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

        static JsTypeAttribute FindJsTypeAttribute(Type type)
        {

            var customAttrs = type.GetCustomAttributes(typeOfJsTypeAttr, false);
            if (customAttrs != null && customAttrs.Length > 0)
            {
                for (int i = customAttrs.Length - 1; i >= 0; --i)
                {
                    var attr = customAttrs[i] as JsTypeAttribute;
                    if (attr != null)
                    {
                        return attr;
                    }
                }
            }
            return null;
        }

        protected override JsTypeDefinition OnBuildRequest(Type actualType)
        {

#if DEBUG
            if (actualType.IsInterface)
            {
                //should not occurs since we send this as actual type ***
                throw new NotSupportedException();
            }
#endif
            //********************
            //this is sample only
            //you can rewrite this yourself
            //********************
            //TODO: review, typename collision ,
            //use fullname instead ? 
            JsTypeDefinition typedefinition = new JsTypeDefinition(actualType.Name);
            AddTypeDefinitionDetail(actualType, typedefinition);
            return typedefinition;
        }
        /// <summary>
        /// add detail member to js type definition
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetTypeDef"></param>
        /// <returns></returns>
        void AddTypeDefinitionDetail(Type source, JsTypeDefinition targetTypeDef)
        {
            //------------------------------
            //this is sample only ***           
            //------------------------------

            JsTypeAttribute foundJsTypeAttr = FindJsTypeAttribute(source);
            Type[] interfaces = source.GetInterfaces();
            Dictionary<string, JsPropertyDefinition> declarProps = new Dictionary<string, JsPropertyDefinition>();

            for (int i = interfaces.Length - 1; i >= 0; --i)
            {
                Type i_interface = interfaces[i];
                //check if an interface has Js...attribute or not
                JsTypeAttribute foundJsTypeAttr2 = FindJsTypeAttribute(i_interface);
                if (foundJsTypeAttr2 != null)
                {
                    InterfaceMapping interfaceMapping = source.GetInterfaceMap(i_interface);
                    //all members is
                    int m = interfaceMapping.InterfaceMethods.Length;
                    for (int n = 0; n < m; ++n)
                    {
                        MethodInfo interface_met = interfaceMapping.InterfaceMethods[n];
                        MethodInfo target_met = interfaceMapping.TargetMethods[n];
                        if (!target_met.IsPublic)
                        {
                            //if this is public we will add it later***
                            if (target_met.IsSpecialName)
                            {
                                //eg. property ?                            
                                string metName = GetProperMemberName(target_met);
                                if (metName.StartsWith("get_"))
                                {
                                    string onlyPropName = metName.Substring(4);
                                    //property get
                                    JsPropertyDefinition existingProp;
                                    if (!declarProps.TryGetValue(onlyPropName, out existingProp))
                                    {
                                        existingProp = new JsPropertyDefinition(onlyPropName);
                                        declarProps.Add(onlyPropName, existingProp);
                                    }
                                    existingProp.GetterMethod = new JsPropertyGetDefinition(metName, target_met);
                                }
                                else if (metName.StartsWith("set_"))
                                {
                                    //property set
                                    string onlyPropName = metName.Substring(4);
                                    //property get
                                    JsPropertyDefinition existingProp;
                                    if (!declarProps.TryGetValue(onlyPropName, out existingProp))
                                    {
                                        existingProp = new JsPropertyDefinition(onlyPropName);
                                        declarProps.Add(onlyPropName, existingProp);
                                    }
                                    existingProp.SetterMethod = new JsPropertySetDefinition(metName, target_met);
                                }
                            }
                            else
                            {
                                JsMethodAttribute foundJsMetAttr = FindJsMethodAttribute(target_met);
                                string metName;
                                if (foundJsMetAttr != null)
                                {
                                    metName = foundJsMetAttr.Name ?? GetProperMemberName(target_met);
                                }
                                else
                                {
                                    metName = GetProperMemberName(target_met);
                                }
                                targetTypeDef.AddMember(new JsMethodDefinition(metName, target_met));
                            }
                        }
                    }
                }
            }
            //------------------------------
            int propCount = declarProps.Count;
            if (declarProps.Count > 0)
            {
                foreach (JsPropertyDefinition prop in declarProps.Values)
                {
                    targetTypeDef.AddMember(prop);
                }
            }
            //------------------------------
            MethodInfo[] methods = source.GetMethods(
                System.Reflection.BindingFlags.Instance |
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
                    targetTypeDef.AddMember(new JsMethodDefinition(foundJsMetAttr.Name ?? GetProperMemberName(met), met));
                }
                //in this version, 
                //if not found js attr, -> not expose it

            }
            var properties = source.GetProperties(System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.Public);
            foreach (var property in properties)
            {
                JsPropertyAttribute foundJsPropAttr = FindJsPropertyAttribute(property);
                if (foundJsPropAttr != null)
                {
                    targetTypeDef.AddMember(new JsPropertyDefinition(foundJsPropAttr.Name ?? GetProperMemberName(property), property));
                }
                //in this version, 
                //if not found js attr, -> not expose it
            } 
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