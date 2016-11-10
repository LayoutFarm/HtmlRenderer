//Apache2, 2014-2016, WinterDev 

using System;
namespace LayoutFarm.Scripting
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class JsTypeAttribute : Attribute
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
    public sealed class JsMethodAttribute : Attribute
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
    public sealed class JsPropertyAttribute : Attribute
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


    namespace Internal
    {
        //this for internal used only
        public class JsExtendedMapAttribute : Attribute
        {
            public JsExtendedMapAttribute(int scriptMemberId)
            {
                this.MemberId = scriptMemberId;
            }
            public int MemberId
            {
                get;
                private set;
            }
        }
    }
}
