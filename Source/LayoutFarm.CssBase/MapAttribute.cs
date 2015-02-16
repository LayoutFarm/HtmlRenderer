//2014,2015 ,BSD, WinterDev 
using System;
namespace LayoutFarm
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