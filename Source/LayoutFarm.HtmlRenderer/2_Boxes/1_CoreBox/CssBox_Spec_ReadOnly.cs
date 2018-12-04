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
        public CssLength Height
        {
            get { return _myspec.Height; }
        }

        public CssDirection CssDirection
        {
            get { return _myspec.CssDirection; }
        }

        //----------------------------------------------- 
        public CssBorderStyle BorderLeftStyle
        {
            get { return _myspec.BorderLeftStyle; }
        }
        public CssBorderStyle BorderTopStyle
        {
            get { return _myspec.BorderTopStyle; }
        }
        public CssBorderStyle BorderRightStyle
        {
            get { return _myspec.BorderRightStyle; }
        }
        public CssBorderStyle BorderBottomStyle
        {
            get { return _myspec.BorderBottomStyle; }
        }
        //--------------------------------------------
        public Color BorderLeftColor
        {
            get { return _myspec.BorderLeftColor; }
        }
        public Color BorderTopColor
        {
            get { return _myspec.BorderTopColor; }
        }
        public Color BorderRightColor
        {
            get { return _myspec.BorderRightColor; }
        }
        public Color BorderBottomColor
        {
            get { return _myspec.BorderBottomColor; }
        }
        //--------------------------------------------
        public CssLength BorderSpacingVertical
        {
            get { return _myspec.BorderSpacingVertical; }
        }
        public CssLength BorderSpacingHorizontal
        {
            get { return _myspec.BorderSpacingHorizontal; }
        }
        public CssBorderCollapse BorderCollapse
        {
            get { return _myspec.BorderCollapse; }
        }

        public bool IsBorderCollapse
        {
            get { return this.BorderCollapse == CssBorderCollapse.Collapse; }
        }

        //------------------------------------------------------ 
        public CssLength Left
        {
            get { return _myspec.Left; }
        }
        public CssLength Top
        {
            get { return _myspec.Top; }
        }

        public CssLength Width
        {
            get { return _myspec.Width; }
        }
        public CssFloat Float
        {
            get { return _myspec.Float; }
        }
        public CssLength MaxWidth
        {
            get { return _myspec.MaxWidth; }
        }

        //------------------------------------------------------ 
        Color BackgroundColor
        {
            get { return _myspec.BackgroundColor; }
        }
        public ImageBinder BackgroundImageBinder
        {
            get { return _myspec.BackgroundImageBinder; }
        }
        public CssLength BackgroundPositionX
        {
            get { return _myspec.BackgroundPositionX; }
        }
        public CssLength BackgroundPositionY
        {
            get { return _myspec.BackgroundPositionY; }
        }
        public CssBackgroundRepeat BackgroundRepeat
        {
            get { return _myspec.BackgroundRepeat; }
        }
        public Color BackgroundGradient
        {
            get { return _myspec.BackgroundGradient; }
        }

        public float BackgroundGradientAngle
        {
            get { return _myspec.ActualBackgroundGradientAngle; }
        }

        CssEmptyCell EmptyCells
        {
            get { return _myspec.EmptyCells; }
        }

        public CssPosition Position
        {
            get { return _myspec.Position; }
        }
        //----------------------------------------------------

        public CssVerticalAlign VerticalAlign
        {
            get { return _myspec.VerticalAlign; }
        }
        CssLength TextIndent
        {
            get { return _myspec.TextIndent; }
        }
        CssLength LineHeight
        {
            get { return _myspec.LineHeight; }
        }

        public CssTextAlign CssTextAlign
        {
            get { return _myspec.CssTextAlign; }
        }
        public CssTextDecoration TextDecoration
        {
            get { return _myspec.TextDecoration; }
        }
        //-----------------------------------
        public CssWhiteSpace WhiteSpace
        {
            get { return _myspec.WhiteSpace; }
        }
        //----------------------------------- 
        CssVisibility Visibility
        {
            get { return _myspec.Visibility; }
        }
        public CssLength WordSpacing
        {
            get { return _myspec.WordSpacing; }
        }
        CssWordBreak WordBreak
        {
            get { return _myspec.WordBreak; }
        }
        //-----------------------------------  
        public CssOverflow Overflow
        {
            get { return _myspec.Overflow; }
        }
        //----------------------------------- 


        /// <summary>
        /// Gets the second color that creates a gradient for the background
        /// </summary>
        public Color ActualBackgroundGradient
        {
            get
            {
                return _myspec.BackgroundGradient;
            }
        }

        /// <summary>
        /// Gets the actual angle specified for the background gradient
        /// </summary>
        public float ActualBackgroundGradientAngle
        {
            get
            {
                return _myspec.BackgroundGradientAngle;
            }
        }

        /// <summary>
        /// 
        /// Gets the actual color for the text.
        /// </summary>
        public Color ActualColor
        {
            get
            {
                return _myspec.ActualColor;
            }
        }

        /// <summary>
        /// Gets the actual background color of the box
        /// </summary>
        public Color ActualBackgroundColor
        {
            get
            {
                return _myspec.ActualBackgroundColor;
            }
        }
        public CssCursorName CursorName
        {
            get
            {
                return _myspec.CursorName;
            }
        }
        /// <summary>
        /// Gets the font that should be actually used to paint the text of the box
        /// </summary>
        internal RequestFont ResolvedFont
        {
            get
            {
                return _resolvedFont;
            }
        }
        /// <summary>
        /// Gets the height of the font in the specified units
        /// </summary>
        /// <returns></returns>
        public float GetEmHeight()
        {
            //after has actual font 
            //temp fixed here
            //TODO: review here
            return 16;
            //return _actualEmHeight;//  
        }
    }
}