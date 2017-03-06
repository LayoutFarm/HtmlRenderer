//Apache2, 2010, Sebastian Stehle 
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System; 
namespace ImageTools.Helpers
{
    /// <summary>
    /// A static class with a lot of helper methods, which guards 
    /// a method agains invalid parameters.
    /// </summary>
    public static class Guard
    { 
         
        /// <summary>
        /// Verifies, that the method parameter with specified object value and message  
        /// is not null and throws an exception if the object is null.
        /// </summary>
        /// <param name="target">The target object, which cannot be null.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is
        /// null (Nothing in Visual Basic).</exception>
        public static void NotNull(object target, string parameterName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        

        /// <summary>
        /// Verifies, that the string method parameter with specified object value and message
        /// is not null, not empty and does not contain only blanls and throws an exception 
        /// if the object is null.
        /// </summary>
        /// <param name="target">The target string, which should be checked against being null or empty.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is
        /// null (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="target"/> is
        /// empty or contains only blanks.</exception>
        public static void NotNullOrEmpty(string target, string parameterName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (string.IsNullOrEmpty(target) || target.Trim().Equals(string.Empty))
            {
                throw new ArgumentException("String parameter cannot be null or empty and cannot contain only blanks.", parameterName);
            }
        }

       
    }
}
