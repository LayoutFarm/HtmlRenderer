//2014 ,BSD, WinterFarm

using System;
using System.Text;
using System.Collections.Generic;

namespace HtmlRenderer
{
    public class MapAttribute : Attribute
    {
        public MapAttribute(string name)
        {
            this.Name = name;
        }

        public string Name
        {
            get;
            private set;
        }

    }
}