//BSD, 2013-2015, Florian Rappl and collab
namespace AngleSharp.Dom.Html
{
    using AngleSharp.Attributes;
    using System;

    /// <summary>
    /// Represents the title HTML element.
    /// </summary>
    [DomName("HTMLTitleElement")]
    public interface IHtmlTitleElement : IHtmlElement
    {
        /// <summary>
        /// Gets or sets the text of the title.
        /// </summary>
        [DomName("text")]
        String Text { get; set; }
    }
}
