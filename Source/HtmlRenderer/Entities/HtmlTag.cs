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

using System.Collections.Generic;
using HtmlRenderer.Entities;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{

    public enum WellknownHtmlTagName
    {
        NotAssign,
        Unknown,
        HTML,
        A,
        HR,
        BR,
        STYLE,
        SCRIPT,
        IMG,
        DIV,
        SPAN,
        LINK,
        P,
        TABLE,
        TD,
        TR,
        IFREAME,

        X//test for extension 
    }

    public sealed class HtmlTag
    {
        #region Fields and Consts

        /// <summary>
        /// the name of the html tag
        /// </summary>
        private readonly string _name;
        readonly WellknownHtmlTagName wellknownTagName;

        /// <summary>
        /// collection of attributes and thier value the html tag has
        /// </summary>
        private readonly Dictionary<string, string> _attributes;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="name">the name of the html tag</param>
        /// <param name="attributes">collection of attributes and thier value the html tag has</param>
        /// <param name="text">the text sub-string of the html element</param>
        public HtmlTag(string name, Dictionary<string, string> attributes = null)
        {
            //must have name
            ArgChecker.AssertArgNotNullOrEmpty(name, "name");

            _name = name;
            _attributes = attributes;

            EvaluateTagName(out this.wellknownTagName, name.ToLower());
        }
        static void EvaluateTagName(out WellknownHtmlTagName wellKnownTagName, string name)
        {
            switch (name)
            {
                default:
                    wellKnownTagName = WellknownHtmlTagName.Unknown;
                    break;
                case "hr":
                    wellKnownTagName = WellknownHtmlTagName.HR;
                    break;
                case "a":
                    wellKnownTagName = WellknownHtmlTagName.A;
                    break;
                case "script":
                    wellKnownTagName = WellknownHtmlTagName.SCRIPT;
                    break;
                case "style":
                    wellKnownTagName = WellknownHtmlTagName.STYLE;
                    break;
                case "div":
                    wellKnownTagName = WellknownHtmlTagName.DIV;
                    break;
                case "span":
                    wellKnownTagName = WellknownHtmlTagName.SPAN;
                    break;
                case "img":
                    wellKnownTagName = WellknownHtmlTagName.IMG;
                    break;
                case "link":
                    wellKnownTagName = WellknownHtmlTagName.LINK;
                    break;
                case "p":
                    wellKnownTagName = WellknownHtmlTagName.P;
                    break;
                case "table":
                    wellKnownTagName = WellknownHtmlTagName.TABLE;
                    break;
                case "td":
                    wellKnownTagName = WellknownHtmlTagName.TD;
                    break;
                case "tr":
                    wellKnownTagName = WellknownHtmlTagName.TR;
                    break;
                case "br":
                    wellKnownTagName = WellknownHtmlTagName.BR;
                    break;
                case "html":
                    wellKnownTagName = WellknownHtmlTagName.HTML;
                    break;
                case "iframe":
                    wellKnownTagName = WellknownHtmlTagName.IFREAME;
                    break;
                case "x":
                    wellKnownTagName = WellknownHtmlTagName.X;
                    break;
            }
        }
        public WellknownHtmlTagName WellknownTagName
        {
            get
            {
                return this.wellknownTagName;
            }
        }
        /// <summary>
        /// Gets the name of this tag
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets collection of attributes and thier value the html tag has
        /// </summary>
        public Dictionary<string, string> Attributes
        {
            get { return _attributes; }
        }

        /// <summary>
        /// Gets if the tag is single placed; in other words it doesn't need a closing tag; 
        /// e.g. &lt;br&gt;
        /// </summary>
        public bool IsSingle
        {
            get { return HtmlUtils.IsSingleTag(Name); }
        }

        /// <summary>
        /// is the html tag has attributes.
        /// </summary>
        /// <returns>true - has attributes, false - otherwise</returns>
        public bool HasAttributes()
        {
            return _attributes != null && _attributes.Count > 0;
        }

        /// <summary>
        /// Gets a boolean indicating if the attribute list has the specified attribute
        /// </summary>
        /// <param name="attribute">attribute name to check if exists</param>
        /// <returns>true - attribute exists, false - otherwise</returns>
        public bool HasAttribute(string attribute)
        {
            return _attributes != null && _attributes.ContainsKey(attribute);
        }

        /// <summary>
        /// Get attribute value for given attribute name or null if not exists.
        /// </summary>
        /// <param name="attribute">attribute name to get by</param>
        /// <param name="defaultValue">optional: value to return if attribute is not specified</param>
        /// <returns>attribute value or null if not found</returns>
        public string TryGetAttribute(string attribute, string defaultValue = null)
        {
            return _attributes != null && _attributes.ContainsKey(attribute) ? _attributes[attribute] : defaultValue;
        }

        public override string ToString()
        {
            return string.Format("<{0}>", _name);
        }
    }
}