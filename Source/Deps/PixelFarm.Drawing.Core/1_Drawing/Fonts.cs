//MIT, 2014-2016, WinterDev

using System;
using PixelFarm.Drawing.Fonts;
namespace PixelFarm.Drawing
{

    /// <summary>
    ///font specification     
    /// </summary>
    public sealed class RequestFont
    {

        //each platform/canvas has its own representation of this Font
        //actual font will be resolved by the platform.

        
        /// <summary>
        /// font size in points unit
        /// </summary>
        float sizeInPoints;
        FontKey fontKey;
        public RequestFont(string facename, float fontSizeInPts, FontStyle style = FontStyle.Regular)
        {
            HBDirection = Fonts.HBDirection.HB_DIRECTION_LTR;//default
            ScriptCode = HBScriptCode.HB_SCRIPT_LATIN;//default 
            Lang = "en";//default
            Name = facename;
            SizeInPoints = fontSizeInPts;
            Style = style;
            fontKey = new FontKey(facename, fontSizeInPts, style);
            //temp fix 
            //we need font height*** 
            //this.Height = SizeInPixels;
        }
        public FontKey FontKey
        {
            get { return this.fontKey; }
        }

        /// <summary>
        /// font's face name
        /// </summary>
        public string Name { get; private set; }
        public FontStyle Style { get; private set; }

        /// <summary>
        /// emheight in point unit
        /// </summary>
        public float SizeInPoints
        {
            get { return sizeInPoints; }
            private set
            {
                sizeInPoints = value;
                
            }
        }
       

        static int s_POINTS_PER_INCH = 72; //default value
        static int s_PIXELS_PER_INCH = 96; //default value

        public ActualFont ActualFont
        {
            get;
            set;
        }
        //--------------------------
        //font shaping info (for native font/shaping engine)
        public HBDirection HBDirection { get; set; }
        public int ScriptCode { get; set; }
        public string Lang { get; set; }
        public static float ConvEmSizeInPointsToPixels(float emsizeInPoint)
        {
            return (int)(((float)emsizeInPoint / (float)s_POINTS_PER_INCH) * (float)s_PIXELS_PER_INCH);
        }

    }

    public interface IFonts
    {

        float MeasureWhitespace(RequestFont f);
        Size MeasureString(char[] str, int startAt, int len, RequestFont font);
        Size MeasureString(char[] str, int startAt, int len, RequestFont font, float maxWidth, out int charFit, out int charFitWidth);
        ActualFont ResolveActualFont(RequestFont f);
        void Dispose();
    }



}