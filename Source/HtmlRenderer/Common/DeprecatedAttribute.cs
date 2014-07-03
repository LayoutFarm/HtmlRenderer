//2014 ,BSD, WinterFarm

using System;
using System.Text;
using System.Collections.Generic;

namespace HtmlRenderer
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