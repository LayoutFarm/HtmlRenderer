//MIT, 2014-2016, WinterDev

using System;
using PixelFarm.Drawing.Fonts;
namespace PixelFarm.Drawing
{

    public sealed class Font : IDisposable
    {

        float emSizeInPixels;
        /// <summary>
        /// emsize in point
        /// </summary>
        float emSize;
        //--------------------------
        /// <summary>
        /// font's face name
        /// </summary>
        public string Name { get; private set; }
        public int Height { get; set; } //TODO: review here
        public FontStyle Style { get; set; } //TODO: review here

        /// <summary>
        /// emheight in point unit
        /// </summary>
        public float EmSize
        {
            get { return emSize; }
            private set
            {
                emSize = value;
                emSizeInPixels = ConvEmSizeInPointsToPixels(value);
            }
        }
        public float EmSizeInPixels
        {
            get
            {
                return emSizeInPixels;
            }
        }

        static int s_POINTS_PER_INCH = 72; //default value
        static int s_PIXELS_PER_INCH = 96; //default value


        public static int PointsPerInch
        {
            get { return s_POINTS_PER_INCH; }
            set { s_POINTS_PER_INCH = value; }
        }
        public static int PixelsPerInch
        {
            get { return s_PIXELS_PER_INCH; }
            set { s_PIXELS_PER_INCH = value; }
        }


        //--------------------------
        //font shaping info (for native font/shaping engine)
        public HBDirection HBDirection { get; set; }
        public int ScriptCode { get; set; }
        public string Lang { get; set; }


        public void Dispose()
        {
        }


        public Font(string facename, float emSizeInPoints)
        {
            HBDirection = Fonts.HBDirection.HB_DIRECTION_LTR;//default
            ScriptCode = HBScriptCode.HB_SCRIPT_LATIN;//default 
            Lang = "en";//default
            Name = facename;
            EmSize = emSizeInPoints;
        }
        public static float ConvEmSizeInPointsToPixels(float emsizeInPoint)
        {
            return (int)(((float)emsizeInPoint / (float)s_POINTS_PER_INCH) * (float)s_PIXELS_PER_INCH);
        }

    }

    public interface IFonts
    {
        Font GetFont(string fontname, float fsize, FontStyle st);
        float MeasureWhitespace(Font f);
        Size MeasureString(char[] str, int startAt, int len, Font font);
        Size MeasureString(char[] str, int startAt, int len, Font font, float maxWidth, out int charFit, out int charFitWidth);
      
        void Dispose();
    }



}