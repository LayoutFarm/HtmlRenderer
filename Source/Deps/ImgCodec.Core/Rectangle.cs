//Apache2, 2010, Sebastian Stehle
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================


namespace ImageTools
{
    using System; 

    /// <summary>
    /// Stores a set of four integers that represent the location and 
    /// size of a rectangle. 
    /// </summary>
    public struct Rectangle : IEquatable<Rectangle>
    {
        #region Data Members

        /// <summary>
        /// Zero rectangle with no width and no height.
        /// </summary>
        public static readonly Rectangle Zero = new Rectangle(0, 0, 0, 0);

        /// <summary>
        /// The height of this rectangle.
        /// </summary>
        private int _height;
        /// <summary>
        /// The width of this rectangle.
        /// </summary>
        private int _width;
        /// <summary>
        /// The x-coordinate of the upper-left corner.
        /// </summary>
        private int _x;
        /// <summary>
        /// The y-coordinate of the upper-left corner.
        /// </summary>
        private int _y;
        /// <summary>
        /// Gets the y-coordinate of the bottom edge of this 
        /// <see cref="Rectangle"/> structure.
        /// </summary>
        /// <value>The y-coordinate of the bottom edge of this 
        /// <see cref="Rectangle"/> structure.</value>
        public int Bottom
        {
            get { return _y + _height; }
        }

        /// <summary>
        /// Gets or sets the height of this 
        /// <see cref="Rectangle"/> structure.
        /// </summary>
        /// <value>The width of this <see cref="Rectangle"/> structure.</value>
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// Gets the x-coordinate of the left edge of this 
        /// <see cref="Rectangle"/> structure.
        /// </summary>
        /// <value>The x-coordinate of the left edge of this 
        /// <see cref="Rectangle"/> structure.</value>
        public int Left
        {
            get { return _x; }
        }

        /// <summary>
        /// Gets the x-coordinate of the right edge of this 
        /// <see cref="Rectangle"/> structure.
        /// </summary>
        /// <value>The x-coordinate of the right edge of this 
        /// <see cref="Rectangle"/> structure.</value>
        public int Right
        {
            get { return _x + _width; }
        }

        /// <summary>
        /// Gets the y-coordinate of the top edge of this 
        /// <see cref="Rectangle"/> structure.
        /// </summary>
        /// <value>The y-coordinate of the top edge of this 
        /// <see cref="Rectangle"/> structure.</value>
        public int Top
        {
            get { return _y; }
        }

        /// <summary>
        /// Gets or sets the width of this 
        /// <see cref="Rectangle"/> structure.
        /// </summary>
        /// <value>The width of this <see cref="Rectangle"/> structure.</value>
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>
        /// Gets or sets the x-coordinate of the upper-left corner of this 
        /// <see cref="Rectangle"/> structure.
        /// </summary>
        /// <value>The x-coordinate of the upper-left corner 
        /// of this <see cref="Rectangle"/> structure.</value>
        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        /// Gets or sets the y-coordinate of the upper-left corner of this 
        /// <see cref="Rectangle"/> structure.
        /// </summary>
        /// <value>The y-coordinate of the upper-left corner 
        /// of this <see cref="Rectangle"/> structure.</value>
        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct
        /// with the specified location and size.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner 
        /// of the rectangle.</param>
        /// <param name="y">The y-coordinate of the upper-left corner 
        /// of the rectangle. </param>
        /// <param name="width">The width of the rectangle. </param>
        /// <param name="height">The height of the rectangle. </param>
        public Rectangle(int x, int y, int width, int height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct 
        /// from a <see cref="Rectangle"/>
        /// </summary>
        /// <param name="other">The other rectangle.</param>
        public Rectangle(Rectangle other)
            : this(other.X, other.Y, other.Width, other.Height)
        {
        }

        #endregion

        #region IEquatable<Rectangle> Members

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same 
        /// type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            bool result = false;
            if (obj is Rectangle)
            {
                result = Equals((Rectangle)obj);
            }

            return result;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the 
        /// <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Rectangle other)
        {
            return _x == other._x && _y == other._y && _width == other._width && _height == other._height;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return X ^ Y ^ Width ^ Height;
        }

        /// <summary>
        /// Tests whether two <see cref="Rectangle"/> structures have 
        /// equal location and size.
        /// </summary>
        /// <param name="left">The <see cref="Rectangle"/> structure that is to the 
        /// left of the equality operator.</param>
        /// <param name="right">The <see cref="Rectangle"/> structure that is to the 
        /// right of the equality operator.</param>
        /// <returns>This operator returns true if the two <see cref="Rectangle"/> structures 
        /// have equal X, Y, Width, and Height properties.</returns>
        public static bool operator ==(Rectangle left, Rectangle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests whether two <see cref="Rectangle"/> structures differ
        /// in location or size.
        /// </summary>
        /// <param name="left">The <see cref="Rectangle"/> structure that is to the 
        /// left of the inequality  operator.</param>
        /// <param name="right">The <see cref="Rectangle"/> structure that is to the 
        /// right of the inequality  operator.</param>
        /// <returns>This operator returns true if any of the X, Y, Width or Height 
        /// properties of the two <see cref="Rectangle"/> structures are unequal; otherwise false.</returns>
        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}
