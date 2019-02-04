//BSD, 2014-present, WinterDev 

using System;
namespace LayoutFarm.HtmlDiagnostics
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ArgNotNullAttribute : Attribute
    {
        public ArgNotNullAttribute()
        {
        }
        public ArgNotNullAttribute(string desc)
        {
            this.Description = desc;
        }
        public string Description { get; private set; }
    }
}
