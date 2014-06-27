//BSD 2014,
//ArthurHub

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
        //empty value must be 0, and auto must be 1 ****
        //(range usage)
        //------------------------------
        EmptyValue,//extension flags 
        AutoLength,//extension flags
        //------------------------------

        NormalLength,//extension flags

        BorderThick,//extension flags
        BorderThin,//extension flags
        BorderMedium,//extension flags

        Percent,//extension flags
        //-----------------------
        //W3C
        Ems,
        Pixels,
        Ex,
        Inches,
        Centimeters,
        Milimeters,
        Points,
        Picas,
        //----------

        Unknown,//extension flags


    }
}