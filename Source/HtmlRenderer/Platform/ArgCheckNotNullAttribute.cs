//BSD 2014,WinterFarm

using System;
using System.IO;

namespace HtmlRenderer.Utils
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
