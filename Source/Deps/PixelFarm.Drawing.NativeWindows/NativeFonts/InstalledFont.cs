//from http://stackoverflow.com/questions/3633000/net-enumerate-winforms-font-styles
// https://www.microsoft.com/Typography/OTSpec/name.htm
//MIT 2016, WinterDev

using System;
using System.Collections.Generic;
using System.IO;
using System.Text; 
namespace PixelFarm.Drawing.Fonts
{
    public class InstalledFont
    {
        string _fontName = string.Empty;
        string _fontSubFamily = string.Empty;
        string _fontPath = string.Empty;

        public InstalledFont(string fontName, string fontSubFamily, string fontPath)
        {
            _fontName = fontName;
            _fontSubFamily = fontSubFamily;
            _fontPath = fontPath;
        }
        public string FontName { get { return _fontName; } set { _fontName = value; } }
        public string FontSubFamily { get { return _fontSubFamily; } set { _fontSubFamily = value; } }
        public string FontPath { get { return _fontPath; } set { _fontPath = value; } }

#if DEBUG
        public override string ToString()
        {
            return _fontName;
        }
#endif
    }

}