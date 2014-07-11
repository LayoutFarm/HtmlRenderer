//2014 ,BSD, WinterDev

using System;

namespace HtmlRenderer 
{
     class MapAttribute : Attribute
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