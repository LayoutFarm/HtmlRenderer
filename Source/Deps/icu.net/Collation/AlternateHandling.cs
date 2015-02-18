// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)

namespace Icu.Collation
{
	/// <summary>
	/// Attribute for handling variable elements.
	/// </summary>
	public enum AlternateHandling
	{
		Default = -1,
		/// <summary>
		/// causes codepoints with primary weights that are equal or below the variable top value
		///to be ignored on primary level and moved to the quaternary level.
		/// </summary>
		Shifted = 20,
		/// <summary>
		/// treats all the codepoints with non-ignorable primary weights in the same way
		/// </summary>
		NonIgnorable = 21,
	}
}
