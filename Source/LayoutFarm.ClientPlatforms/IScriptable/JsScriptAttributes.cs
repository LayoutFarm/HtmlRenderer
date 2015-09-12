// 2015,2014 ,Apache2, WinterDev 
using System;

namespace LayoutFarm.Scripting
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JsTypeAttribute : Attribute
    {
        public JsTypeAttribute()
        {
        }
        public JsTypeAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; private set; }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class JsMethodAttribute : Attribute
    {
        public JsMethodAttribute()
        {
        }
        public JsMethodAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class JsPropertyAttribute : Attribute
    {
        public JsPropertyAttribute()
        {
        }
        public JsPropertyAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; private set; }
    }
}
