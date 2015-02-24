//BSD, 2013-2015, Florian Rappl and collab
namespace AngleSharp.Dom
{
    using AngleSharp.Attributes;

    /// <summary>
    /// An HTMLAllCollection is always rooted at document and matching all
    /// elements. It represents the tree of elements in a one-dimensional
    /// fashion.
    /// </summary>
    [DomName("HTMLAllCollection")]
    public interface IHtmlAllCollection : IHtmlCollection
    {
    }
}
