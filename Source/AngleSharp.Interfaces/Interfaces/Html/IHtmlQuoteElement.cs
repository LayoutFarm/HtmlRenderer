//BSD, 2013-2015, Florian Rappl and collab
namespace AngleSharp.Dom.Html
{
    using AngleSharp.Attributes;
    using System;

    /// <summary>
    /// Represents the q HTML element.
    /// </summary>
    [DomName("HTMLQuoteElement")]
    public interface IHtmlQuoteElement : IHtmlElement
    {
        /// <summary>
        /// Gets or sets the citation of the element.
        /// </summary>
        [DomName("cite")]
        String Citation { get; set; }
    }
}
