//2015, MIT ,WinterDev

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace VRoomJsTest2
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

}