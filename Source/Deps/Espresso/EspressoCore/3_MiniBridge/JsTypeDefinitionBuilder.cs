//MIT, 2015-2016, WinterDev, EngineKit, brezza92

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
 
namespace Espresso
{
     
    public abstract class JsTypeDefinitionBuilder
    {
        internal JsTypeDefinition BuildTypeDefinition(Type t)
        {
            return this.OnBuildRequest(t);
        }
        protected abstract JsTypeDefinition OnBuildRequest(Type t);
    } 
    
    class DefaultJsTypeDefinitionBuilder : JsTypeDefinitionBuilder
    {

#if NET20
        protected override JsTypeDefinition OnBuildRequest(Type t)
        {
            JsTypeDefinition typedefinition = new JsTypeDefinition(t.Name);

            //only instance /public method /prop***
            var methods = t.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var met in methods)
            {
                typedefinition.AddMember(new JsMethodDefinition(met.Name, met));
            }

            var properties = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var property in properties)
            {
                typedefinition.AddMember(new JsPropertyDefinition(property.Name, property));
            }

            return typedefinition;
        }
#else
        protected override JsTypeDefinition OnBuildRequest(Type t)
        {
            JsTypeDefinition typedefinition = new JsTypeDefinition(t.Name);

            //only instance /public method /prop***
            //MethodInfo[] methods = t.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            var methods = t.GetRuntimeMethods();
            foreach (var met in methods)
            {
                if(!met.IsStatic && met.IsPublic)
                {
                    typedefinition.AddMember(new JsMethodDefinition(met.Name, met));
                }
            }

            //var properties = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            var properties = t.GetRuntimeProperties();
            //TODO finding GetProperties with BindingFlags
            foreach (var property in properties)
            {
                typedefinition.AddMember(new JsPropertyDefinition(property.Name, property)); 
            }

            return typedefinition;
        }
#endif
    }
}