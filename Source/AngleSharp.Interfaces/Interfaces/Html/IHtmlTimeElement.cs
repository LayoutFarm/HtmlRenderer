//BSD, 2013-2015, Florian Rappl and collab
namespace AngleSharp.Dom.Html
{
    using AngleSharp.Attributes;
    using System;

    /// <summary>
    /// Represents the time HTML element.
    /// </summary>
    [DomName("HTMLTimeElement")]
    public interface IHtmlTimeElement : IHtmlElement
    {
        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        [DomName("datetime")]
        String DateTime { get; set; }
    }
}
