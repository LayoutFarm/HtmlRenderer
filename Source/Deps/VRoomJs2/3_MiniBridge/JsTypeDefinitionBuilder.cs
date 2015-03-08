//2015, MIT ,WinterDev

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
 
namespace VroomJs
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
    }
}