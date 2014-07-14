//BSD 2014, WinterDev
//ArthurHub

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

namespace HtmlRenderer.WebDom
{

   
    public interface IHtmlAttribute
    {
        string Name { get; }
        string Value { get; }
        int LocalNameIndex { get; }
    }
    public interface IHtmlElement
    {
        
        /// <summary>
        /// Gets the name of this tag
        /// </summary>
        string Name { get; }
         
        /// <summary>
        /// is the html tag has attributes.
        /// </summary>
        /// <returns>true - has attributes, false - otherwise</returns>
        bool HasAttributes();
         
        /// <summary>
        /// Get attribute value for given attribute name or null if not exists.
        /// </summary>
        /// <param name="attribute">attribute name to get by</param>
        /// <param name="defaultValue">optional: value to return if attribute is not specified</param>
        /// <returns>attribute value or null if not found</returns>
     
        System.Collections.Generic.IEnumerable<IHtmlAttribute> GetAttributeIter();

        string Id { get; }
        string ClassName { get; }
        string Style { get; }

    }


}