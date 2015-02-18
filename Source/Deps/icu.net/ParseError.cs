// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Runtime.InteropServices;

namespace Icu
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct ParseError
	{
		/// <summary>
		/// The line on which the error occured.  If the parser uses this
		 /// field, it sets it to the line number of the source text line on
		 /// which the error appears, which will be be a value &gt;= 1.  If the
		 /// parse does not support line numbers, the value will be &lt;= 0.
		/// </summary>
		public Int32 Line;
		/// <summary>
		/// The character offset to the error.  If the line field is &gt;= 1,
		 /// then this is the offset from the start of the line.  Otherwise,
		 /// this is the offset from the start of the text.  If the parser
		 /// does not support this field, it will have a value &lt; 0.
		/// </summary>
		public Int32 Offset;
		/// <summary>
		/// Textual context before the error.
		/// The empty string if not supported by parser.
		/// </summary>
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		public string PreContext;
		/// <summary>
		/// The error itself and/or textual context after the error.
		/// The empty string if not supported by parser.
		/// </summary>
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		public string PostContext;

		public override string ToString()
		{
			return ToString(string.Empty);
		}

		public string ToString(string rules)
		{
			if (rules == null)
			{
				throw new ArgumentNullException();
			}

			string result = "\nAt " ;
			if (Line > 0)
			{
				result += "Line " + Line + ' ';
			}
			if (Offset >= 0)
			{
				result += "Offset " + Offset + '\n';
			}

			string ruleError = string.Empty;
			try
			{
				if (rules.Length > 0 && Offset > 0)
				{
					ruleError = GetLine(rules, Line) + '\n';
					ruleError += new string('-', Offset - 1) + "^\n";
				}
			}
			catch
			{
				ruleError = string.Empty;
			}
			result += ruleError;

			result += "PreContext: " + PreContext + '\n' + "PostContext: " + PostContext;
			return result;
		}

		static private string GetLine(string text, int line)
		{
			return text.Split('\n')[line];
		}
	}
}
