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

        internal static CssBox CreateBoxNotInherit(IHtmlElement tag, CssBox parent)
        {

            switch (tag.WellknownTagName)
            {
                case WellknownHtmlTagName.img:
                    return new CssBoxImage(parent, tag);
                case WellknownHtmlTagName.iframe:
                    return new CssBoxHr(parent, tag);
                case WellknownHtmlTagName.hr:
                    return new CssBoxHr(parent, tag);
                //test extension box
                case WellknownHtmlTagName.X:
                    var customBox = CreateCustomBox(tag, parent);
                    if (customBox == null)
                    {
                        return new CssBox(parent, tag);
                    }
                    else
                    {
                        return customBox;
                    }
                default:
                    return new CssBox(parent, tag);
            }
        }

        static CssBox CreateCustomBox(IHtmlElement tag, CssBox parent)
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
            var box = new CssBox(null, null);
            box.CssDisplay = CssDisplay.Block;
            return box;
        }
        public static CssBox CreateBoxAndInherit(CssBox parent, IHtmlElement tag)
        {
            var newBox = new CssBox(parent, tag);
            newBox.InheritStyles(parent);
            return newBox;
        }
        //---------------------------------------------------

        /// <summary>
        /// Create new css block box for the given parent with the given optional html tag and insert it either
        /// at the end or before the given optional box.<br/>
        /// If no html tag is given the box will be anonymous.<br/>
        /// If no before box is given the new box will be added at the end of parent boxes collection.<br/>
        /// If before box doesn't exists in parent box exception is thrown.<br/>
        /// </summary>
        /// <remarks>
        /// To learn more about anonymous block boxes visit CSS spec:
        /// http://www.w3.org/TR/CSS21/visuren.html#anonymous-block-level
        /// </remarks>
        /// <param name="parent">the box to add the new block box to it as child</param>
        /// <param name="tag">optional: the html tag to define the box</param>
        /// <param name="before">optional: to insert as specific location in parent box</param>
        /// <returns>the new block box</returns>
        internal static CssBox CreateAnonBlock(CssBox parent, int insertAt = -1)
        {
            var newBox = CreateBoxAndInherit(parent, null, insertAt);
            newBox.CssDisplay = CssDisplay.Block;
            return newBox;
        }
        static CssBox CreateBoxAndInherit(CssBox parent, IHtmlElement tag, int insertAt)
        {
            var newBox = new CssBox(parent, tag);
            newBox.InheritStyles(parent);
            if (insertAt > -1)
            {
                newBox.ChangeSiblingOrder(insertAt);
            }
            return newBox;
        }
    }
}