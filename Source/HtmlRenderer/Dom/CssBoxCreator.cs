//BSD 2014, WinterCore

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{

    public abstract class CustomCssBoxGenerator
    {
        public abstract CssBox CreateCssBox(HtmlTag tag, CssBox parentBox);

    }

    partial class CssBox
    {
        static List<CustomCssBoxGenerator> generators = new List<CustomCssBoxGenerator>();
        
        public static void RegisterCustomCssBoxGenerator(CustomCssBoxGenerator generator)
        {
            generators.Add(generator);
        }
        static CssBox CreateCustomCssBox(HtmlTag tag, CssBox parentBox)
        {
            int j = generators.Count;
            if (j > 0)
            {
                for (int i = j - 1; i >= 0; --i)
                {
                    var box = generators[i].CreateCssBox(tag, parentBox);
                    if (box != null)
                    {
                        return box;
                    }
                }
            }
            return null;
        }
    }
}