namespace HtmlRenderer.Entities
{
    /// <summary>
    /// Represents the possible units of the CSS lengths
    /// </summary>
    /// <remarks>
    /// http://www.w3.org/TR/CSS21/syndata.html#length-units
    /// </remarks>
    public enum CssUnit : byte
    {
        None,
        Unknown,
        Ems,
        Pixels,
        Ex,
        Inches,
        Centimeters,
        Milimeters,
        Points,
        Picas
    }
}