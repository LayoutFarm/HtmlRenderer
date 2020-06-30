//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

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

using PixelFarm.Drawing;
using LayoutFarm.Css;
namespace LayoutFarm.HtmlBoxes
{
    /// <summary>
    /// Base class for css box to handle the css properties.<br/>
    /// Has field and property for every css property that can be set, the properties add additional parsing like
    /// setting the correct border depending what border value was set (single, two , all four).<br/>
    /// Has additional fields to control the location and size of the box and 'actual' css values for some properties
    /// that require additional calculations and parsing.<br/>
    /// </summary>


    partial class CssBox
    {
        public CssLength Height => _myspec.Height;


        public CssDirection CssDirection => _myspec.CssDirection;


        //----------------------------------------------- 
        public CssBorderStyle BorderLeftStyle => _myspec.BorderLeftStyle;

        public CssBorderStyle BorderTopStyle => _myspec.BorderTopStyle;

        public CssBorderStyle BorderRightStyle => _myspec.BorderRightStyle;

        public CssBorderStyle BorderBottomStyle => _myspec.BorderBottomStyle;

        //--------------------------------------------
        public Color BorderLeftColor => _myspec.BorderLeftColor;

        public Color BorderTopColor => _myspec.BorderTopColor;

        public Color BorderRightColor => _myspec.BorderRightColor;

        public Color BorderBottomColor => _myspec.BorderBottomColor;

        //--------------------------------------------
        public CssLength BorderSpacingVertical => _myspec.BorderSpacingVertical;

        public CssLength BorderSpacingHorizontal => _myspec.BorderSpacingHorizontal;

        public CssBorderCollapse BorderCollapse => _myspec.BorderCollapse;


        public bool IsBorderCollapse => this.BorderCollapse == CssBorderCollapse.Collapse;


        //------------------------------------------------------ 
        public CssLength Left => _myspec.Left;

        public CssLength Top => _myspec.Top;


        public CssLength Width => _myspec.Width;

        public CssFloat Float => _myspec.Float;

        public CssLength MaxWidth => _myspec.MaxWidth;


        //------------------------------------------------------ 
        Color BackgroundColor => _myspec.BackgroundColor;

        public ImageBinder BackgroundImageBinder => _myspec.BackgroundImageBinder;

        public CssLength BackgroundPositionX => _myspec.BackgroundPositionX;

        public CssLength BackgroundPositionY => _myspec.BackgroundPositionY;

        public CssBackgroundRepeat BackgroundRepeat => _myspec.BackgroundRepeat;

        public Color BackgroundGradient => _myspec.BackgroundGradient;


        public float BackgroundGradientAngle => _myspec.ActualBackgroundGradientAngle;


        CssEmptyCell EmptyCells => _myspec.EmptyCells;


        public CssPosition Position => _myspec.Position;

        //----------------------------------------------------

        public CssVerticalAlign VerticalAlign => _myspec.VerticalAlign;

        CssLength TextIndent => _myspec.TextIndent;

        CssLength LineHeight => _myspec.LineHeight;


        public CssTextAlign CssTextAlign => _myspec.CssTextAlign;

        public CssTextDecoration TextDecoration => _myspec.TextDecoration;

        //-----------------------------------
        public CssWhiteSpace WhiteSpace => _myspec.WhiteSpace;

        //----------------------------------- 
        CssVisibility Visibility => _myspec.Visibility;

        public CssLength WordSpacing => _myspec.WordSpacing;

        CssWordBreak WordBreak => _myspec.WordBreak;

        //-----------------------------------  
        public CssOverflow Overflow => _myspec.Overflow; 
        /// <summary>
        /// Gets the second color that creates a gradient for the background
        /// </summary>
        public Color ActualBackgroundGradient => _myspec.BackgroundGradient; 
        /// <summary>
        /// Gets the actual angle specified for the background gradient
        /// </summary>
        public float ActualBackgroundGradientAngle => _myspec.BackgroundGradientAngle; 
        /// <summary>
        /// 
        /// Gets the actual color for the text.
        /// </summary>
        public Color ActualColor => _myspec.ActualColor; 
        /// <summary>
        /// Gets the actual background color of the box
        /// </summary>
        public Color ActualBackgroundColor => _myspec.ActualBackgroundColor; 

        public CssCursorName CursorName => _myspec.CursorName; 
        /// <summary>
        /// Gets the font that should be actually used to paint the text of the box
        /// </summary>
        internal RequestFont ResolvedFont => _resolvedFont;

        //ResolvedFont _resolvedFont1;
        //internal ResolvedFont ResolvedFont1 => _resolvedFont1;
        /// <summary>
        /// Gets the height of the font in the specified units
        /// </summary>
        /// <returns></returns>
        public float GetEmHeight() => 16;

        //after has actual font 
        //temp fixed here
        //TODO: review here

        //return _actualEmHeight;//  

    }
}