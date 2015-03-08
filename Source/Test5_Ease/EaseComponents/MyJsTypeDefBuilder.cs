//2015, MIT ,WinterDev

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

using VroomJs; 

namespace LayoutFarm.WebDom.Wrap
{

    [AttributeUsage(AttributeTargets.Class)]
    public class JsTypeAttribute : Attribute
    {
        public JsTypeAttribute() { }
        public JsTypeAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; private set; }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class JsMethodAttribute : Attribute
    {
        public JsMethodAttribute() { }
        public JsMethodAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class JsPropertyAttribute : Attribute
    {
        public JsPropertyAttribute() { }
        public JsPropertyAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; private set; }
    }
    class MyJsTypeDefinitionBuilder : JsTypeDefinitionBuilderBase
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
            var methods = t.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var met in methods)
            {
                var customAttrs = met.GetCustomAttributes(typeOfJsMethodAttr, false);
                if (customAttrs != null && customAttrs.Length > 0)
                {
                    var attr = customAttrs[0] as JsMethodAttribute;
                    typedefinition.AddMember(new JsMethodDefinition(attr.Name ?? met.Name, met));
                }
            }

            var properties = t.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var property in properties)
            {
                var customAttrs = property.GetCustomAttributes(typeOfJsPropertyAttr, false);
                if (customAttrs != null && customAttrs.Length > 0)
                {
                    var attr = customAttrs[0] as JsPropertyAttribute;
                    typedefinition.AddMember(new JsPropertyDefinition(attr.Name ?? property.Name, property));
                }
            }

            return typedefinition;
        }

    }

}