//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System; 

namespace ImageTools
{
    /// <summary>
    /// Stores meta information about a image, like the name of the author,
    /// the copyright information, the date, where the image was created
    /// or some other information.
    /// </summary>
    public sealed class ImageProperty : IEquatable<ImageProperty>
    {
        #region Invariant

#if !WINDOWS_PHONE

        private void ImagePropertyInvariantMethod()
        {
            //Contract.Invariant(!string.IsNullOrEmpty(_name));
        }
#endif

        #endregion

        #region Properties

        private string _name;
        /// <summary>
        /// Gets or sets the name of the value, indicating
        /// which kind of information this property stores.
        /// </summary>
        /// <value>The name of the property.</value>
        /// <example>Typical properties are the author, copyright
        /// information or other meta information.</example>
        public string Name
        {
            get
            {
                //  Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
                return _name;
            }
        }

        /// <summary>
        /// Gets or sets the value of the property, e.g. the name
        /// of the author.
        /// </summary>
        /// <value>The value of this property.</value>
        public string Value { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageProperty"/> class
        /// by setting the name and the value of the property.
        /// </summary>
        /// <param name="name">The name of the property. Cannot be null or empty</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public ImageProperty(string name, string value)
        {
            //Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name), "Name cannot be null or empty.");

            _name = name;

            Value = value;
        }

        #endregion

        #region IEquatable<ImageProperty> Members

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is 
        /// equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with 
        /// the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the 
        /// current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return Equals(obj as ImageProperty);
        }

        /// <summary>
        /// Indicates whether the current object is equal to 
        /// another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> 
        /// parameter; otherwise, false.
        /// </returns>
        public bool Equals(ImageProperty other)
        {
            if (other == null)
            {
                return false;
            }

            return Object.Equals(Name, other.Name) &&
                   Object.Equals(Value, other.Value);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            int hashCode = 1;

            if (Name != null)
            {
                hashCode ^= Name.GetHashCode();
            }

            if (Value != null)
            {
                hashCode ^= Value.GetHashCode();
            }

            return hashCode;
        }

        #endregion
    }
}
