//BSD 2014, WinterDev 
using System;
using HtmlRenderer.Boxes;
using HtmlRenderer.Composers;
namespace HtmlRenderer
{
#if DEBUG
    public delegate void dbugCounterAction();
    public static class dbugCounter
    {
        public static bool dbugStartRecord = false;
        static int _dbugDrawStringCount;
        static int _dbugBoxPaint;
        static int _dbugLinePaint;
        static int _dbugRunPaint;
        public static int dbugBoxPaintCount
        {
            get { return _dbugBoxPaint; }
            set { _dbugBoxPaint = value; }
        }
        public static int dbugLinePaintCount
        {
            get { return _dbugLinePaint; }
            set { _dbugLinePaint = value; }
        }
        public static int dbugRunPaintCount
        {
            get { return _dbugRunPaint; }
            set { _dbugRunPaint = value; }
        }

        public static void ResetPaintCount()
        {
            _dbugBoxPaint = 0;
            _dbugLinePaint = 0;
            _dbugRunPaint = 0;
        }
        public static int dbugDrawStringCount
        {
            get { return _dbugDrawStringCount; }
            set
            {
                _dbugDrawStringCount = value;
            }
        }
        public static long Snap(System.Diagnostics.Stopwatch sw, dbugCounterAction codeRgn)
        {

            sw.Stop();
            sw.Reset();
            sw.Start();
            codeRgn();
            sw.Stop();
            return sw.ElapsedMilliseconds;
            //return sw.ElapsedTicks;
        }
        public static long GCAndSnapTicks(dbugCounterAction codeRgn)
        {
            GC.Collect();
            var newWatch = System.Diagnostics.Stopwatch.StartNew();
            codeRgn();
            newWatch.Stop();
            return newWatch.ElapsedTicks;
        }

        /// <summary>
        /// Get CSS box property value by the CSS name.<br/>
        /// Used as a mapping between CSS property and the class property.
        /// </summary>
        /// <param name="cssBox">the CSS box to get it's property value</param>
        /// <param name="propName">the name of the CSS property</param>
        /// <returns>the value of the property, null if no such property exists</returns>
        public static string GetPropertyValue(CssBox cssBox, string propName)
        {
            switch (propName)
            {
                case "border-bottom-width":
                    return cssBox.BorderBottomWidth.ToString();
                case "border-left-width":
                    return cssBox.BorderLeftWidth.ToString();
                case "border-right-width":
                    return cssBox.BorderRightWidth.ToString();
                case "border-top-width":
                    return cssBox.BorderTopWidth.ToString();

                case "border-bottom-style":
                    return cssBox.BorderBottomStyle.ToCssStringValue();
                case "border-left-style":
                    return cssBox.BorderLeftStyle.ToCssStringValue();
                case "border-right-style":
                    return cssBox.BorderRightStyle.ToCssStringValue();
                case "border-top-style":
                    return cssBox.BorderTopStyle.ToCssStringValue();

                case "border-bottom-color":
                    return cssBox.BorderBottomColor.ToHexColor();
                case "border-left-color":
                    return cssBox.BorderLeftColor.ToHexColor();
                case "border-right-color":
                    return cssBox.BorderRightColor.ToHexColor();
                case "border-top-color":
                    return cssBox.BorderTopColor.ToHexColor();

                case "border-spacing":
                    return cssBox.BorderSpacingHorizontal.ToString() + " " +
                           cssBox.BorderSpacingVertical;

                case "border-collapse":
                    return cssBox.BorderCollapse.ToCssStringValue();
                case "corner-radius":
                    return cssBox.GetCornerRadius();

                case "corner-nw-radius":
                    return cssBox.CornerNWRadius.ToString();
                case "corner-ne-radius":
                    return cssBox.CornerNERadius.ToString();
                case "corner-se-radius":
                    return cssBox.CornerSERadius.ToString();
                case "corner-sw-radius":
                    return cssBox.CornerSWRadius.ToString();

                case "margin-bottom":
                    return cssBox.MarginBottom.ToString();
                case "margin-left":
                    return cssBox.MarginLeft.ToString();
                case "margin-right":
                    return cssBox.MarginRight.ToString();
                case "margin-top":
                    return cssBox.MarginTop.ToString();
                case "padding-bottom":

                    return cssBox.PaddingBottom.ToString();
                case "padding-left":
                    return cssBox.PaddingLeft.ToString();
                case "padding-right":
                    return cssBox.PaddingRight.ToString();
                case "padding-top":
                    return cssBox.PaddingTop.ToString();

                case "left":
                    return cssBox.Left.ToString();
                case "top":
                    return cssBox.Top.ToString();
                case "width":
                    return cssBox.Width.ToString();
                case "max-width":
                    return cssBox.MaxWidth.ToString();
                case "height":
                    return cssBox.Height.ToString();

                case "background-color":
                    return cssBox.BackgroundColor.ToHexColor();
                case "background-image":
                    return cssBox.BackgroundImageBinder.ImageSource;

                case "background-position":
                    return cssBox.BackgroundPositionX.ToString() + " " + cssBox.BackgroundPositionY.ToString();
                case "background-repeat":
                    return cssBox.BackgroundRepeat.ToCssStringValue();

                case "background-gradient":
                    return cssBox.BackgroundGradient.ToHexColor();
                case "background-gradient-angle":
                    return cssBox.BackgroundGradientAngle.ToString();
                case "color":
                    return cssBox.Color.ToHexColor();
                case "display":
                    return cssBox.CssDisplay.ToCssStringValue();
                case "direction":
                    return cssBox.CssDirection.ToCssStringValue();
                case "empty-cells":
                    return cssBox.EmptyCells.ToCssStringValue();
                case "float":
                    return cssBox.Float.ToCssStringValue();
                case "position":
                    return cssBox.Position.ToCssStringValue();
                case "line-height":
                    return cssBox.LineHeight.ToString();
                case "vertical-align":
                    return cssBox.VerticalAlign.ToCssStringValue();
                case "text-indent":
                    return cssBox.TextIndent.ToString();
                case "text-align":
                    return cssBox.CssTextAlign.ToCssStringValue();
                case "text-decoration":
                    return cssBox.TextDecoration.ToCssStringValue();
                case "white-space":
                    return cssBox.WhiteSpace.ToCssStringValue();
                case "word-break":
                    return cssBox.WordBreak.ToCssStringValue();
                case "visibility":
                    return cssBox.Visibility.ToCssStringValue();
                case "word-spacing":
                    return cssBox.WordSpacing.ToString();
                case "font-family":
                    return cssBox.FontFamily;
                case "font-size":
                    return cssBox.FontSize.ToFontSizeString();
                case "font-style":
                    return cssBox.FontStyle.ToCssStringValue();
                case "font-variant":
                    return cssBox.FontVariant.ToCssStringValue();
                case "font-weight":
                    return cssBox.FontWeight.ToCssStringValue();
                case "list-style":
                    return cssBox.ListStyle;
                case "list-style-position":
                    return cssBox.ListStylePosition.ToCssStringValue();
                case "list-style-image":
                    return cssBox.ListStyleImage;
                case "list-style-type":
                    return cssBox.ListStyleType.ToCssStringValue();
                case "overflow":
                    return cssBox.Overflow.ToString();
            }
            return null;
        }


    }

#endif

}