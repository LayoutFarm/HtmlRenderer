//BSD, 2013-2015, Florian Rappl and collab
namespace AngleSharp.Dom
{
    /// <summary>
    /// An enumeration with all contenteditable modes.
    /// </summary>
    enum ContentEditableMode : ushort
    {
        /// <summary>
        /// Not contenteditable.
        /// </summary>
        False,
        /// <summary>
        /// The element is contenteditable.
        /// </summary>
        True,
        /// <summary>
        /// Inherited from the parent element.
        /// </summary>
        Inherited
    }
}
