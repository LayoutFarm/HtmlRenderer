//2014 ,BSD, WinterDev

using System; 

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