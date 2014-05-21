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
using HtmlRenderer.Utils;

namespace HtmlRenderer.Entities
{

    public class CssProperty
    {

      
        public CssProperty(string propertyName, string value)
        {
            this.Name = propertyName;
            this.Value = value;
        }
        public string Name
        {
            get;
            private set;
        }
        public string Value
        {
            get;
            private set;
        }

        public bool MarkedAsInheritValue
        {
            get
            {
                return this.Value == CssConstants.Inherit;
            }
        }
        public bool IsValidValue
        {
            get;
            set;
        }
            
    }

    /// <summary>
    /// Represents a block of CSS property values.<br/>
    /// Contains collection of key-value pairs that are CSS properties for specific css class.<br/>
    /// Css class can be either custom or html tag name.
    /// </summary>
    /// <remarks>
    /// To learn more about CSS blocks visit CSS spec: http://www.w3.org/TR/CSS21/syndata.html#block
    /// </remarks>
    public sealed class CssCodeBlock
    {

        /// <summary>
        /// the name of the css class of the block
        /// </summary>
        private readonly string _className;
        /// <summary>
        /// additional selectors to used in hierarchy (p className1 > className2)
        /// </summary>
        private readonly List<CssCodeBlockSelector> _selectors;


        /// <summary>
        /// the CSS block properties and values
        /// </summary>
        private readonly Dictionary<string, CssProperty> _properties;
        /// <summary>
        /// is the css block has :hover pseudo-class
        /// </summary>
        private readonly bool _hover;
        /// <summary>
        /// Creates a new block from the block's source
        /// </summary>
        /// <param name="cssClassName">the name of the css class of the block</param>
        /// <param name="properties">the CSS block properties and values</param>
        /// <param name="selectors">optional: additional selectors to used in hierarchy</param>
        /// <param name="hover">optional: is the css block has :hover pseudo-class</param>
        public CssCodeBlock(string cssClassName, Dictionary<string, CssProperty> properties, List<CssCodeBlockSelector> selectors = null, bool hover = false)
        {
            ArgChecker.AssertArgNotNullOrEmpty(cssClassName, "cssClassName");
            ArgChecker.AssertArgNotNull(properties, "properties");

            _className = cssClassName;
            _selectors = selectors;
            _properties = properties;
            _hover = hover;
        }

        /// <summary>
        /// the name of the css class of the block
        /// </summary>
        public string CssClassName
        {
            get { return _className; }
        }

        /// <summary>
        /// additional selectors to used in hierarchy (p className1 > className2)
        /// </summary>
        public List<CssCodeBlockSelector> Selectors
        {
            get { return _selectors; }
        }

        /// <summary>
        /// Gets the CSS block properties and its values
        /// </summary>
        public IDictionary<string, CssProperty> Properties
        {
            get { return _properties; }
        }
        public CssProperty GetProperty(string propName)
        {
            CssProperty property;
            _properties.TryGetValue(propName, out property);
            return property;
        }
        public string GetPropertyValueAsString(string propName)
        {
            CssProperty property;
            if (_properties.TryGetValue(propName, out property))
            {
                return property.Value;
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// is the css block has :hover pseudo-class
        /// </summary>
        public bool Hover
        {
            get { return _hover; }
        }

        /// <summary>
        /// Merge the other block properties into this css block.<br/>
        /// Other block properties can overwrite this block properties.
        /// </summary>
        /// <param name="other">the css block to merge with</param>
        public void Merge(CssCodeBlock other)
        {
            ArgChecker.AssertArgNotNull(other, "other");

            foreach (var prop in other._properties.Keys)
            {
                _properties[prop] = other._properties[prop];
            }
        }

        /// <summary>
        /// Create deep copy of the CssBlock.
        /// </summary>
        /// <returns>new CssBlock with same data</returns>
        public CssCodeBlock Clone()
        {
            return new CssCodeBlock(_className, new Dictionary<string, CssProperty>(_properties), _selectors != null ? new List<CssCodeBlockSelector>(_selectors) : null);
        }

        /// <summary>
        /// Check if the two css blocks are the same (same class, selectors and properties).
        /// </summary>
        /// <param name="other">the other block to compare to</param>
        /// <returns>true - the two blocks are the same, false - otherwise</returns>
        public bool Equals(CssCodeBlock other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (!Equals(other._className, _className))
                return false;

            if (!Equals(other._properties.Count, _properties.Count))
                return false;

            foreach (var property in _properties)
            {
                if (!other._properties.ContainsKey(property.Key))
                    return false;
                if (!Equals(other._properties[property.Key], property.Value))
                    return false;
            }

            if (!EqualsSelector(other))
                return false;

            return true;
        }

        /// <summary>
        /// Check if the selectors of the css blocks is the same.
        /// </summary>
        /// <param name="other">the other block to compare to</param>
        /// <returns>true - the selectors on blocks are the same, false - otherwise</returns>
        public bool EqualsSelector(CssCodeBlock other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (other.Hover != Hover)
                return false;
            if (other._selectors == null && _selectors != null)
                return false;
            if (other._selectors != null && _selectors == null)
                return false;

            if (other._selectors != null && _selectors != null)
            {
                if (!Equals(other._selectors.Count, _selectors.Count))
                    return false;

                for (int i = 0; i < _selectors.Count; i++)
                {
                    if (!Equals(other._selectors[i].Class, _selectors[i].Class))
                        return false;
                    if (!Equals(other._selectors[i].DirectParent, _selectors[i].DirectParent))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if the two css blocks are the same (same class, selectors and properties).
        /// </summary>
        /// <param name="obj">the other block to compare to</param>
        /// <returns>true - the two blocks are the same, false - otherwise</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(CssCodeBlock))
                return false;
            return Equals((CssCodeBlock)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>A hash code for the current <see cref="T:System.Object"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_className != null ? _className.GetHashCode() : 0) * 397) ^ (_properties != null ? _properties.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        public override string ToString()
        {
            var str = _className + " { ";
            foreach (var property in _properties)
            {
                str += string.Format("{0}={1}; ", property.Key, property.Value);
            }
            return str + " }";
        }
    }
}