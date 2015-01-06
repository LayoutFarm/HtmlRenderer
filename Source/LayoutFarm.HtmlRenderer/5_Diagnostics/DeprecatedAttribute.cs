//2014,2015 ,BSD, WinterDev

using System;
namespace LayoutFarm.Diagnostics
{
    public class FeatureDeprecatedAttribute : Attribute
    {
        public FeatureDeprecatedAttribute()
        {
        }
        public FeatureDeprecatedAttribute(string note)
        {
            this.Note = note;
        }
        public string Note { get; set; }
    }
}