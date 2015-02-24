//BSD, 2013-2015, Florian Rappl and collab
namespace AngleSharp.Dom.Html
{
    using AngleSharp.Attributes;

    /// <summary>
    /// Represents the datalist HTML element.
    /// </summary>
    [DomName("HTMLDataListElement")]
    public interface IHtmlDataListElement : IHtmlElement
    {
        /// <summary>
        /// Gets the associated options.
        /// </summary>
        [DomName("options")]
        IHtmlCollection Options { get; }
    }
}
