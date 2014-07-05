//BSD 2014, WinterDev

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
        public abstract CssBox CreateCssBox(IHtmlElement tag, CssBox parentBox);

    }

    public static class BoxCreator
    {
        static List<CustomCssBoxGenerator> generators = new List<CustomCssBoxGenerator>();
        public static void RegisterCustomCssBoxGenerator(CustomCssBoxGenerator generator)
        {
            generators.Add(generator);
        }
        static CssBox CreateCustomCssBox(IHtmlElement tag, CssBox parentBox)
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


        internal static CssBox CreateBox(CssBox parent, BridgeHtmlElement tag)
        {
            CssBox newBox = null;
            //----------------------------------------- 
            //1. create new box
            //-----------------------------------------
            switch (tag.WellknownTagName)
            {
                case WellknownHtmlTagName.img:
                    newBox = new CssBoxImage(parent, tag);
                    break;
                //case WellknownHtmlTagName.iframe:
                //    newBox = new CssBoxHr(parent, tag);//?
                //    break;
                case WellknownHtmlTagName.hr:
                    newBox = new CssBoxHr(parent, tag);
                    break;
                //test extension box
                case WellknownHtmlTagName.X:
                    newBox = CreateCustomBox(parent, tag);
                    if (newBox == null)
                    {
                        newBox = new CssBox(parent, tag);
                    }
                    break;
                default:
                    newBox = new CssBox(parent, tag);
                    break;
            }
            //----------------------------------------- 
            //2. clone exact spec from prepared BoxSpec
            //----------------------------------------- 
            var newBoxSpec = CssBox.UnsafeGetBoxSpec(newBox);
            newBoxSpec.CloneAllStylesFrom(tag.Spec);
            newBox.CloseSpec();
            return newBox;
        }

        static CssBox CreateCustomBox(CssBox parent, IHtmlElement tag)
        {
            for (int i = generators.Count - 1; i >= 0; --i)
            {
                var newbox = generators[i].CreateCssBox(tag, parent);
                if (newbox != null)
                {
                    return newbox;
                }
            }
            return null;
        }

        /// <summary>
        /// Create new css block box.
        /// </summary>
        /// <returns>the new block box</returns>
        internal static CssBox CreateRootBlock()
        {
            var spec = new BoxSpec();
            spec.CssDisplay = CssDisplay.Block;
            var box = new CssBox(null, null, spec);
            //------------------------------------
            box.ReEvaluateFont(10);
            //------------------------------------
            return box;
        }



    }
}