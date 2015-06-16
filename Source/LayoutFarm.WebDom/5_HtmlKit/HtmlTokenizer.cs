//
// HtmlTokenizer.cs
//
// Author: Jeffrey Stedfast <jeff@xamarin.com>
//
// Copyright (c) 2015 Xamarin Inc. (www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System.IO;
using System.Text;

namespace HtmlKit {
	/// <summary>
	/// An HTML tokenizer.
	/// </summary>
	/// <remarks>
	/// Tokenizes HTML text, emitting an <see cref="HtmlToken"/> for each token it encounters.
	/// </remarks>
	public partial class HtmlTokenizer
	{
		const string DocType = "doctype";
		const string CData = "[CDATA[";

		readonly HtmlEntityDecoder entity = new HtmlEntityDecoder ();
		readonly StringBuilder data = new StringBuilder ();
		readonly StringBuilder name = new StringBuilder ();
		readonly char[] cdata = new char[3];
		HtmlDocTypeToken doctype;
		HtmlAttribute attribute;
		string activeTagName;
		HtmlTagToken tag;
		int cdataIndex;
		bool isEndTag;
		char quote;

		TextReader text;

		/// <summary>
		/// Initializes a new instance of the <see cref="HtmlTokenizer"/> class.
		/// </summary>
		/// <remarks>
		/// Creates a new <see cref="HtmlTokenizer"/>.
		/// </remarks>
		/// <param name="reader">The <see cref="TextReader"/>.</param>
		public HtmlTokenizer (TextReader reader)
		{
			DecodeCharacterReferences = true;
			LinePosition = 1;
			LineNumber = 1;
			text = reader;
		}

		/// <summary>
		/// Get or set whether or not the tokenizer should decode character references.
		/// </summary>
		/// <remarks>
		/// <para>Gets or sets whether or not the tokenizer should decode character references.</para>
		/// <para>Note: Character references in attribute values will still be decoded even if this
		/// value is set to <c>false</c>.</para>
		/// </remarks>
		/// <value><c>true</c> if character references should be decoded; otherwise, <c>false</c>.</value>
		public bool DecodeCharacterReferences {
			get; set;
		}

		/// <summary>
		/// Get the current HTML namespace detected by the tokenizer.
		/// </summary>
		/// <remarks>
		/// Gets the current HTML namespace detected by the tokenizer.
		/// </remarks>
		/// <value>The html namespace.</value>
		public HtmlNamespace HtmlNamespace {
			get; private set;
		}

		/// <summary>
		/// Gets the current line number.
		/// </summary>
		/// <remarks>
		/// <para>This property is most commonly used for error reporting, but can be called
		/// at any time. The starting value for this property is <c>1</c>.</para>
		/// <para>Combined with <see cref="LinePosition"/>, a value of <c>1,1</c> indicates
		/// the start of the document.</para>
		/// </remarks>
		/// <value>The current line number.</value>
		public int LineNumber {
			get; private set;
		}

		/// <summary>
		/// Gets the current line position.
		/// </summary>
		/// <remarks>
		/// <para>This property is most commonly used for error reporting, but can be called
		/// at any time. The starting value for this property is <c>1</c>.</para>
		/// <para>Combined with <see cref="LineNumber"/>, a value of <c>1,1</c> indicates
		/// the start of the document.</para>
		/// </remarks>
		/// <value>The current line number.</value>
		public int LinePosition {
			get; private set;
		}

		/// <summary>
		/// Get the current state of the tokenizer.
		/// </summary>
		/// <remarks>
		/// Gets the current state of the tokenizer.
		/// </remarks>
		/// <value>The current state of the tokenizer.</value>
		public HtmlTokenizerState TokenizerState {
			get; private set;
		}

		/// <summary>
		/// Create a DOCTYPE token.
		/// </summary>
		/// <remarks>
		/// Creates a DOCTYPE token.
		/// </remarks>
		/// <returns>The DOCTYPE token.</returns>
		protected virtual HtmlDocTypeToken CreateDocType ()
		{
			return new HtmlDocTypeToken ();
		}

		HtmlDocTypeToken CreateDocTypeToken (string rawTagName)
		{
			var token = CreateDocType ();
			token.RawTagName = rawTagName;
			return token;
		}

		/// <summary>
		/// Create an HTML comment token.
		/// </summary>
		/// <remarks>
		/// Creates an HTML comment token.
		/// </remarks>
		/// <returns>The HTML comment token.</returns>
		/// <param name="comment">The comment.</param>
		protected virtual HtmlCommentToken CreateCommentToken (string comment)
		{
			return new HtmlCommentToken (comment);
		}

		/// <summary>
		/// Create an HTML character data token.
		/// </summary>
		/// <remarks>
		/// Creates an HTML character data token.
		/// </remarks>
		/// <returns>The HTML character data token.</returns>
		/// <param name="data">The character data.</param>
		protected virtual HtmlDataToken CreateDataToken (string data)
		{
			return new HtmlDataToken (data);
		}

		/// <summary>
		/// Create an HTML tag token.
		/// </summary>
		/// <remarks>
		/// Creates an HTML tag token.
		/// </remarks>
		/// <returns>The HTML tag token.</returns>
		/// <param name="name">The tag name.</param>
		/// <param name="isEndTag"><c>true</c> if the tag is an end tag; otherwise, <c>false</c>.</param>
		protected virtual HtmlTagToken CreateTagToken (string name, bool isEndTag = false)
		{
			return new HtmlTagToken (name, isEndTag);
		}

		/// <summary>
		/// Create an attribute.
		/// </summary>
		/// <remarks>
		/// Creates an attribute.
		/// </remarks>
		/// <returns>The attribute.</returns>
		/// <param name="name">THe attribute name.</param>
		protected virtual HtmlAttribute CreateAttribute (string name)
		{
			return new HtmlAttribute (name);
		}

		static bool IsAlphaNumeric (char c)
		{
			return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9');
		}

		static bool IsAsciiLetter (char c)
		{
			return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
		}

		static char ToLower (char c)
		{
			return (c >= 'A' && c <= 'Z') ? (char) (c + 0x20) : c;
		}

		int Peek ()
		{
			return text.Peek ();
		}

		int Read ()
		{
			int c;

			if ((c = text.Read ()) == -1)
				return -1;

			if (c == '\n') {
				LinePosition = 1;
				LineNumber++;
			} else {
				LinePosition++;
			}

			return c;
		}

		// Note: value must be lowercase
		bool NameIs (string value)
		{
			if (name.Length != value.Length)
				return false;

			for (int i = 0; i < name.Length; i++) {
				if (ToLower (name[i]) != value[i])
					return false;
			}

			return true;
		}

		void EmitTagAttribute ()
		{
			attribute = CreateAttribute (name.ToString ());
			tag.Attributes.Add (attribute);
			name.Length = 0;
		}

		bool EmitCommentToken (string comment)
		{
			token = CreateCommentToken (comment);
			data.Length = 0;
			name.Length = 0;
			return true;
		}

        void EmitCommentToken (StringBuilder comment)
		{
			EmitCommentToken (comment.ToString ());
		} 
        void EmitDataToken (bool encodeEntities)
		{
            
            if (data.Length > 0) {
				var dataToken = CreateDataToken (data.ToString ());
				dataToken.EncodeEntities = encodeEntities;
				token = dataToken;
				data.Length = 0;			 
			}
			token = null;
		}

		void EmitTagToken ()
		{
			if (!tag.IsEndTag && !tag.IsEmptyElement) {
				switch (tag.Id) {
				case HtmlTagId.Style: case HtmlTagId.Xmp: case HtmlTagId.IFrame: case HtmlTagId.NoEmbed: case HtmlTagId.NoFrames:
					TokenizerState = HtmlTokenizerState.RawText;
					activeTagName = tag.Name;
					break;
				case HtmlTagId.Title: case HtmlTagId.TextArea:
					TokenizerState = HtmlTokenizerState.RcData;
					activeTagName = tag.Name;
					break;
				case HtmlTagId.PlainText:
					TokenizerState = HtmlTokenizerState.PlainText;
					break;
				case HtmlTagId.Script:
					TokenizerState = HtmlTokenizerState.ScriptData;
					break;
				case HtmlTagId.NoScript:
					// TODO: only switch into the RawText state if scripting is enabled
					TokenizerState = HtmlTokenizerState.RawText;
					activeTagName = tag.Name;
					break;
				case HtmlTagId.Html:
					TokenizerState = HtmlTokenizerState.Data;

					for (int i = tag.Attributes.Count; i > 0; i--) {
						var attr = tag.Attributes[i - 1];

						if (attr.Id == HtmlAttributeId.XmlNS && attr.Value != null) {
							HtmlNamespace = tag.Attributes[i].Value.ToHtmlNamespace ();
							break;
						}
					}
					break;
				default:
					TokenizerState = HtmlTokenizerState.Data;
					break;
				}
			} else {
				TokenizerState = HtmlTokenizerState.Data;
			}

			data.Length = 0;
			token = tag;
			tag = null;			 
		}

		void ReadCharacterReference (HtmlTokenizerState next)
		{
			int nc = Peek ();
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.EndOfFile;
				data.Append ('&');

			    EmitDataToken ( true);
                return;
			}

			c = (char) nc;
			token = null;

			switch (c) {
			case '\t': case '\r': case '\n': case '\f': case ' ': case '<': case '&':
				// no character is consumed, emit '&'
				TokenizerState = next;
				data.Append ('&');
                return;
			}

			entity.Push ('&');

			while (entity.Push (c)) {
				Read ();

				if ((nc = Peek ()) == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					data.Append (entity.GetPushedInput ());
					entity.Reset ();

					EmitDataToken (true);
                    return;
				}

				c = (char) nc;
			}

			TokenizerState = next;

			data.Append (entity.GetValue ());
			entity.Reset ();

			if (c == ';') {
				// consume the ';'
				Read ();
			}

            
		}

		void ReadGenericRawTextLessThan ( HtmlTokenizerState rawText, HtmlTokenizerState rawTextEndTagOpen)
		{
			int nc = Peek ();

			data.Append ('<');

			switch ((char) nc) {
			case '/':
				TokenizerState = rawTextEndTagOpen;
				data.Append ('/');
				name.Length = 0;
				Read ();
				break;
			default:
				TokenizerState = rawText;
				break;
			}

			token = null;             
		}

		void ReadGenericRawTextEndTagOpen (bool decoded, HtmlTokenizerState rawText, HtmlTokenizerState rawTextEndTagName)
		{
			int nc = Peek ();
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.EndOfFile;
			    EmitDataToken ( decoded);
                return;
			}

			c = (char) nc;

			if (IsAsciiLetter (c)) {
				TokenizerState = rawTextEndTagName;
				name.Append (c);
				data.Append (c);
				Read ();
			} else {
				TokenizerState = rawText;
			}

			token = null;             
		}

		void ReadGenericRawTextEndTagName ( bool decoded, HtmlTokenizerState rawText)
		{
			var current = TokenizerState;

			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					name.Length = 0;

					EmitDataToken ( decoded);
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\t': case '\r': case '\n': case '\f': case ' ':
					if (NameIs (activeTagName)) {
						TokenizerState = HtmlTokenizerState.BeforeAttributeName;
						break;
					}

					goto default;
				case '/':
					if (NameIs (activeTagName)) {
						TokenizerState = HtmlTokenizerState.SelfClosingStartTag;
						break;
					}
					goto default;
				case '>':
					if (NameIs (activeTagName)) {
						token = CreateTagToken (name.ToString (), true);
						TokenizerState = HtmlTokenizerState.Data;
						data.Length = 0;
						name.Length = 0;
                        return;
					}
					goto default;
				default:
					if (!IsAsciiLetter (c)) {
						TokenizerState = rawText;
						token = null;
                        return;
					}

					name.Append (c == '\0' ? '\uFFFD' : c);
					break;
				}
			} while (TokenizerState == current);

			tag = CreateTagToken (name.ToString (), true);
			name.Length = 0;
			token = null;			 
		}

		void ReadDataToken()
		{
			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					break;
				}

				c = (char) nc;

				switch (c) {
				case '&':
					if (DecodeCharacterReferences) {
						TokenizerState = HtmlTokenizerState.CharacterReferenceInData;
                        token = null;
                        return;
					}

					goto default;
				case '<':
					TokenizerState = HtmlTokenizerState.TagOpen;
					break;
				//case 0: // parse error, but emit it anyway
				default:
					data.Append (c);

					// Note: we emit at 1024 characters simply to avoid
					// consuming too much memory.
					if (data.Length >= 1024) { 
						EmitDataToken ( DecodeCharacterReferences);
                        return;
                    }

					break;
				}
			} while (TokenizerState == HtmlTokenizerState.Data);

			EmitDataToken (DecodeCharacterReferences);
		}

		void ReadCharacterReferenceInData ()
		{
			ReadCharacterReference ( HtmlTokenizerState.Data);
		}

		void ReadRcData ()
		{
			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					break;
				}

				c = (char) nc;

				switch (c) {
				case '&':
					if (DecodeCharacterReferences) {
						TokenizerState = HtmlTokenizerState.CharacterReferenceInRcData;
						token = null;
                        return;
					}

					goto default;
				case '<':
					TokenizerState = HtmlTokenizerState.RcDataLessThan;
					EmitDataToken ( DecodeCharacterReferences);
                    return;
				default:
					data.Append (c == '\0' ? '\uFFFD' : c);

					// Note: we emit at 1024 characters simply to avoid
					// consuming too much memory.
					if (data.Length >= 1024) { 
						  EmitDataToken (DecodeCharacterReferences);
                    }

					break;
				}
			} while (TokenizerState == HtmlTokenizerState.RcData);

			if (data.Length > 0) { 
				  EmitDataToken (DecodeCharacterReferences);
                  return;
            }

			token = null; 
		}

		void ReadCharacterReferenceInRcData ()
		{
			 ReadCharacterReference ( HtmlTokenizerState.RcData);
            
		}

		void ReadRawText ()
		{
			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					break;
				}

				c = (char) nc;

				switch (c) {
				case '<':
					TokenizerState = HtmlTokenizerState.RawTextLessThan;
					EmitDataToken (false);
                    return;
				default:
					data.Append (c == '\0' ? '\uFFFD' : c);

					// Note: we emit at 1024 characters simply to avoid
					// consuming too much memory.
					if (data.Length >= 1024) { 
					    EmitDataToken (false);
                        return;
                    }

					break;
				}
			} while (TokenizerState == HtmlTokenizerState.RawText);

			if (data.Length > 0) { 
			   EmitDataToken (false);
               return;
            }

			token = null; 
		}

		void ReadScriptData ()
		{
			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					break;
				}

				c = (char) nc;

				switch (c) {
				case '<':
					TokenizerState = HtmlTokenizerState.ScriptDataLessThan;
					EmitDataToken (false);                     
                    return;
				default:
					data.Append (c == '\0' ? '\uFFFD' : c);

					// Note: we emit at 1024 characters simply to avoid
					// consuming too much memory.
					if (data.Length >= 1024) { 
					    EmitDataToken (false);
                        return;
                    }
					break;
				}
			} while (TokenizerState == HtmlTokenizerState.ScriptData);

            if (data.Length > 0)
            {
                EmitDataToken(false);
                return;
            }
			token = null;			  
		}

		void ReadPlainText ()
		{
			int nc = Read ();

			while (nc != -1) {
				char c = (char) nc;

				data.Append (c == '\0' ? '\uFFFD' : c);

                // Note: we emit at 1024 characters simply to avoid
                // consuming too much memory.
                if (data.Length >= 1024){
                   EmitDataToken(false);
                   return;
                }
				nc = Read ();
			}

			TokenizerState = HtmlTokenizerState.EndOfFile;

			EmitDataToken ( false);
		}
		bool ReadTagOpen ()
		{
			int nc = Read ();
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.EndOfFile;
				token = CreateDataToken ("<");
				return true;
			}

			token = null;

			c = (char) nc;

			// Note: we save the data in case we hit a parse error and have to emit a data token
			data.Append ('<');
			data.Append (c);

			switch ((c = (char) nc)) {
			case '!': TokenizerState = HtmlTokenizerState.MarkupDeclarationOpen; break;
			case '?': TokenizerState = HtmlTokenizerState.BogusComment; break;
			case '/': TokenizerState = HtmlTokenizerState.EndTagOpen; break;
			default:
				if (IsAsciiLetter (c)) {
					TokenizerState = HtmlTokenizerState.TagName;
					isEndTag = false;
					name.Append (c);
				} else {
					TokenizerState = HtmlTokenizerState.Data;
					return false;
				}
				break;
			}

			return false;
		}

		void ReadEndTagOpen ()
		{
			int nc = Read ();
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.EndOfFile;
				EmitDataToken (false);
                return;
			}

			c = (char) nc;
			token = null;

			// Note: we save the data in case we hit a parse error and have to emit a data token
			data.Append (c);

			switch (c) {
			case '>': // parse error
				TokenizerState = HtmlTokenizerState.Data;
				data.Length = 0;
				break;
			default:
				if (IsAsciiLetter (c)) {
					TokenizerState = HtmlTokenizerState.TagName;
					isEndTag = true;
					name.Append (c);
				} else {
					TokenizerState = HtmlTokenizerState.BogusComment;
                    return;
				}
				break;
			}
			 
		}

		void ReadTagName ()
		{
			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					name.Length = 0;

					EmitDataToken (false);
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\t': case '\r': case '\n': case '\f': case ' ':
					TokenizerState = HtmlTokenizerState.BeforeAttributeName;
					break;
				case '/':
					TokenizerState = HtmlTokenizerState.SelfClosingStartTag;
					break;
				case '>':
					token = CreateTagToken (name.ToString (), isEndTag);
					TokenizerState = HtmlTokenizerState.Data;
					data.Length = 0;
					name.Length = 0;
                    return;
				default:
					name.Append (c == '\0' ? '\uFFFD' : c);
					break;
				}
			} while (TokenizerState == HtmlTokenizerState.TagName);

			tag = CreateTagToken (name.ToString (), isEndTag);
			name.Length = 0;
			token = null;

			
		}

		void ReadRcDataLessThan ()
		{
			  ReadGenericRawTextLessThan ( HtmlTokenizerState.RcData, HtmlTokenizerState.RcDataEndTagOpen);
		}

        void ReadRcDataEndTagOpen ()
		{
			  ReadGenericRawTextEndTagOpen (DecodeCharacterReferences, HtmlTokenizerState.RcData, HtmlTokenizerState.RcDataEndTagName);
		}

        void ReadRcDataEndTagName ()
		{
			  ReadGenericRawTextEndTagName( DecodeCharacterReferences, HtmlTokenizerState.RcData);
		}

	   void ReadRawTextLessThan ()
		{
			 ReadGenericRawTextLessThan (HtmlTokenizerState.RawText, HtmlTokenizerState.RawTextEndTagOpen);
		}

	    void ReadRawTextEndTagOpen ()
		{
		     ReadGenericRawTextEndTagOpen ( false, HtmlTokenizerState.RawText, HtmlTokenizerState.RawTextEndTagName);
		}

		void ReadRawTextEndTagName ()
		{
			ReadGenericRawTextEndTagName (false, HtmlTokenizerState.RawText);
		}

		void ReadScriptDataLessThan ()
		{
			int nc = Peek ();

			data.Append ('<');

			switch ((char) nc) {
			case '/':
				TokenizerState = HtmlTokenizerState.ScriptDataEndTagOpen;
				data.Append ('/');
				name.Length = 0;
				Read ();
				break;
			case '!':
				TokenizerState = HtmlTokenizerState.ScriptDataEscapeStart;
				data.Append ('!');
				Read ();
				break;
			default:
				TokenizerState = HtmlTokenizerState.ScriptData;
				break;
			}

			token = null;
		}

		void ReadScriptDataEndTagOpen ()
		{
			int nc = Peek ();
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.EndOfFile;
			    EmitDataToken ( false);
                return;
			}

			c = (char) nc;

			if (c == 'S' || c == 's') {
				TokenizerState = HtmlTokenizerState.ScriptDataEndTagName;
				name.Append ('s');
				data.Append (c);
				Read ();
			} else {
				TokenizerState = HtmlTokenizerState.ScriptData;
			}

			token = null; 
		}

		void ReadScriptDataEndTagName ()
		{
			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					name.Length = 0;

					EmitDataToken (false);
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\t': case '\r': case '\n': case '\f': case ' ':
					if (NameIs ("script")) {
						TokenizerState = HtmlTokenizerState.BeforeAttributeName;
						break;
					}

					goto default;
				case '/':
					if (NameIs ("script")) {
						TokenizerState = HtmlTokenizerState.SelfClosingStartTag;
						break;
					}
					goto default;
				case '>':
					if (NameIs ("script")) {
						token = CreateTagToken (name.ToString (), true);
						TokenizerState = HtmlTokenizerState.Data;
						data.Length = 0;
						name.Length = 0;
                        return;
					}
					goto default;
				default:
					if (!IsAsciiLetter (c)) {
						TokenizerState = HtmlTokenizerState.ScriptData;
						token = null;
                        return;
					}

					name.Append (c == '\0' ? '\uFFFD' : c);
					break;
				}
			} while (TokenizerState == HtmlTokenizerState.ScriptDataEndTagName);

			tag = CreateTagToken (name.ToString (), true);
			name.Length = 0;
			token = null;			 
		}

		void ReadScriptDataEscapeStart ()
		{
			int nc = Peek ();

			if (nc == '-') {
				TokenizerState = HtmlTokenizerState.ScriptDataEscapeStartDash;
				Read ();
			} else {
				TokenizerState = HtmlTokenizerState.ScriptData;
			}

			token = null;			
		}

		void ReadScriptDataEscapeStartDash ()
		{
			int nc = Peek ();

			if (nc == '-') {
				TokenizerState = HtmlTokenizerState.ScriptDataEscapedDashDash;
				Read ();
			} else {
				TokenizerState = HtmlTokenizerState.ScriptData;
			}

			token = null;			 
		}

        void ReadScriptDataEscaped ()
		{
			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					EmitDataToken (false);
                    return;
				}

				c = (char) nc;

				switch (c) {
				case '-':
					TokenizerState = HtmlTokenizerState.ScriptDataEscapedDash;
					data.Append ('-');
					break;
				case '<':
					TokenizerState = HtmlTokenizerState.ScriptDataEscapedLessThan;
					break;
				default:
					data.Append (c == '\0' ? '\uFFFD' : c);
					break;
				}
			} while (TokenizerState == HtmlTokenizerState.ScriptDataEscaped);

			token = null;			 
		}

		void ReadScriptDataEscapedDash ()
		{
			int nc = Peek ();
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.EndOfFile;
				EmitDataToken (false);
                return;
			}

			switch ((c = (char) nc)) {
			case '-':
				TokenizerState = HtmlTokenizerState.ScriptDataEscapedDashDash;
				data.Append ('-');
				break;
			case '<':
				TokenizerState = HtmlTokenizerState.ScriptDataEscapedLessThan;
				break;
			default:
				TokenizerState = HtmlTokenizerState.ScriptDataEscaped;
				data.Append (c == '\0' ? '\uFFFD' : c);
				break;
			}

			token = null; 
		}

		void ReadScriptDataEscapedDashDash ()
		{
			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					EmitDataToken (false);
                    return;
				}

				c = (char) nc;

				switch (c) {
				case '-':
					TokenizerState = HtmlTokenizerState.ScriptDataEscapedDash;
					data.Append ('-');
					break;
				case '<':
					TokenizerState = HtmlTokenizerState.ScriptDataEscapedLessThan;
					break;
				case '>':
					TokenizerState = HtmlTokenizerState.ScriptData;
					data.Append ('>');
					break;
				default:
					TokenizerState = HtmlTokenizerState.ScriptDataEscaped;
					data.Append (c);
					break;
				}
			} while (TokenizerState == HtmlTokenizerState.ScriptDataEscaped);

			token = null;

			 
		}

		void ReadScriptDataEscapedLessThan ()
		{
			int nc = Peek ();
			char c = (char) nc;

			if (c == '/') {
				TokenizerState = HtmlTokenizerState.ScriptDataEndTagOpen;
				name.Length = 0;
				Read ();
			} else if (IsAsciiLetter (c)) {
				TokenizerState = HtmlTokenizerState.ScriptDataDoubleEscaped;
				data.Append ('<');
				data.Append (c);
				name.Append (c);
				Read ();
			} else {
				TokenizerState = HtmlTokenizerState.ScriptDataEscaped;
				data.Append ('<');
			}

			token = null;

			
		}

		void ReadScriptDataEscapedEndTagOpen ()
		{
			int nc = Peek ();
			char c;

			data.Append ("</");

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.EndOfFile;
				EmitDataToken (false);
                return;
			}

			c = (char) nc;

			if (IsAsciiLetter (c)) {
				TokenizerState = HtmlTokenizerState.ScriptDataEscapedEndTagName;
				name.Append (c);
				Read ();
			} else {
				TokenizerState = HtmlTokenizerState.ScriptDataEscaped;
			}

			token = null;

			 
		}

		void ReadScriptDataEscapedEndTagName ()
		{
			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					name.Length = 0;

			        EmitDataToken (false);
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\t': case '\r': case '\n': case '\f': case ' ':
					if (NameIs ("script")) {
						TokenizerState = HtmlTokenizerState.BeforeAttributeName;
						break;
					}

					goto default;
				case '/':
					if (NameIs ("script")) {
						TokenizerState = HtmlTokenizerState.SelfClosingStartTag;
						break;
					}
					goto default;
				case '>':
					if (NameIs ("script")) {
						token = CreateTagToken (name.ToString (), true);
						TokenizerState = HtmlTokenizerState.Data;
						data.Length = 0;
						name.Length = 0;
                        return;
					}
					goto default;
				default:
					if (!IsAsciiLetter (c)) {
						TokenizerState = HtmlTokenizerState.ScriptData;
						data.Append (c);
						token = null;
						return;
					}

					name.Append (c);
					break;
				}
			} while (TokenizerState == HtmlTokenizerState.ScriptDataEscapedEndTagName);

			tag = CreateTagToken (name.ToString (), true);
			name.Length = 0;
			token = null;

			 
		}

		void ReadScriptDataDoubleEscapeStart ()
		{
			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					name.Length = 0;

					EmitDataToken (false);
                    return;
				}

				c = (char) nc;

				data.Append (c);

				switch (c) {
				case '\t': case '\r': case '\n': case '\f': case ' ': case '/': case '>':
					if (NameIs ("script"))
						TokenizerState = HtmlTokenizerState.ScriptDataDoubleEscaped;
					else
						TokenizerState = HtmlTokenizerState.ScriptDataEscaped;
					name.Length = 0;
					break;
				default:
					if (!IsAsciiLetter (c))
						TokenizerState = HtmlTokenizerState.ScriptDataEscaped;
					else
						name.Append (c);
					break;
				}
			} while (TokenizerState == HtmlTokenizerState.ScriptDataDoubleEscapeStart);

			token = null;

			
		}

		void ReadScriptDataDoubleEscaped ()
		{
			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					EmitDataToken (false);
                    return;
				}

				c = (char) nc;

				switch (c) {
				case '-':
					TokenizerState = HtmlTokenizerState.ScriptDataDoubleEscapedDash;
					data.Append ('-');
					break;
				case '<':
					TokenizerState = HtmlTokenizerState.ScriptDataDoubleEscapedLessThan;
					break;
				default:
					data.Append (c == '\0' ? '\uFFFD' : c);
					break;
				}
			} while (TokenizerState == HtmlTokenizerState.ScriptDataEscaped);

			token = null;

			
		}

		void ReadScriptDataDoubleEscapedDash ()
		{
			int nc = Peek ();
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.EndOfFile;
			    EmitDataToken ( false);
                return;
			}

			switch ((c = (char) nc)) {
			case '-':
				TokenizerState = HtmlTokenizerState.ScriptDataDoubleEscapedDashDash;
				data.Append ('-');
				break;
			case '<':
				TokenizerState = HtmlTokenizerState.ScriptDataDoubleEscapedLessThan;
				break;
			default:
				TokenizerState = HtmlTokenizerState.ScriptDataDoubleEscaped;
				data.Append (c == '\0' ? '\uFFFD' : c);
				break;
			}

			token = null;

			
		}

		void ReadScriptDataDoubleEscapedDashDash ()
		{
			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					EmitDataToken (false);
                    return;
				}

				c = (char) nc;

				switch (c) {
				case '-':
					data.Append ('-');
					break;
				case '<':
					TokenizerState = HtmlTokenizerState.ScriptDataDoubleEscapedLessThan;
					data.Append ('<');
					break;
				case '>':
					TokenizerState = HtmlTokenizerState.ScriptData;
					data.Append ('>');
					break;
				default:
					TokenizerState = HtmlTokenizerState.ScriptDataDoubleEscaped;
					data.Append (c);
					break;
				}
			} while (TokenizerState == HtmlTokenizerState.ScriptDataEscaped);

			token = null;

			
		}

		void ReadScriptDataDoubleEscapedLessThan ()
		{
			int nc = Peek ();

			if (nc == '/') {
				TokenizerState = HtmlTokenizerState.ScriptDataDoubleEscapeEnd;
				data.Append ('/');
				Read ();
			} else {
				TokenizerState = HtmlTokenizerState.ScriptDataDoubleEscaped;
			}

			token = null;

			 
		}

		void ReadScriptDataDoubleEscapeEnd ()
		{
			do {
				int nc = Peek ();
				char c = (char) nc;

				switch (c) {
				case '\t': case '\r': case '\n': case '\f': case ' ': case '/': case '>':
					if (NameIs ("script"))
						TokenizerState = HtmlTokenizerState.ScriptDataEscaped;
					else
						TokenizerState = HtmlTokenizerState.ScriptDataDoubleEscaped;
					data.Append (c);
					Read ();
					break;
				default:
					if (!IsAsciiLetter (c)) {
						TokenizerState = HtmlTokenizerState.ScriptDataDoubleEscaped;
					} else {
						name.Append (c);
						data.Append (c);
						Read ();
					}
					break;
				}
			} while (TokenizerState == HtmlTokenizerState.ScriptDataDoubleEscapeEnd);

			token = null;

			 
		}

		void ReadBeforeAttributeName ()
		{
			token = null;

			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					tag = null;

					EmitDataToken ( false);
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\t': case '\r': case '\n': case '\f': case ' ':
					break;
				case '/':
					TokenizerState = HtmlTokenizerState.SelfClosingStartTag;
                    return;
				case '>':
				    EmitTagToken ();
                    return;
				case '"': case '\'': case '<': case '=':
					// parse error
					goto default;
				default:
					TokenizerState = HtmlTokenizerState.AttributeName;
					name.Append (c == '\0' ? '\uFFFD' : c);
                    return;
				}
			} while (true);
		}

		void ReadAttributeName ()
		{
			token = null;

			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					name.Length = 0;
					tag = null;
					EmitDataToken (false);
                    return;
                }

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\t': case '\r': case '\n': case '\f': case ' ':
					TokenizerState = HtmlTokenizerState.AfterAttributeName;
					break;
				case '/':
					TokenizerState = HtmlTokenizerState.SelfClosingStartTag;
					break;
				case '=':
					TokenizerState = HtmlTokenizerState.BeforeAttributeValue;
					break;
				case '>':
					EmitTagAttribute ();

					EmitTagToken ();
                    return;
				default:
					name.Append (c == '\0' ? '\uFFFD' : c);
					break;
				}
			} while (TokenizerState == HtmlTokenizerState.AttributeName);

			EmitTagAttribute ();		 
		}

	    void ReadAfterAttributeName ()
		{
			token = null;

			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					tag = null;

					EmitDataToken (false);
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\t': case '\r': case '\n': case '\f': case ' ':
					break;
				case '/':
					TokenizerState = HtmlTokenizerState.SelfClosingStartTag;
                    return;
				case '=':
					TokenizerState = HtmlTokenizerState.BeforeAttributeValue;
					return;
				case '>':
					EmitTagToken ();
                    return;
                case '"': case '\'': case '<':
					// parse error
					goto default;
				default:
					TokenizerState = HtmlTokenizerState.AttributeName;
					name.Append (c == '\0' ? '\uFFFD' : c);
                    return;
                }
			} while (true);
		}

		void ReadBeforeAttributeValue ()
		{
			token = null;

			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					tag = null;

				    EmitDataToken (false);
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\t': case '\r': case '\n': case '\f': case ' ':
					break;
				case '"': case '\'': TokenizerState = HtmlTokenizerState.AttributeValueQuoted; quote = c;
                    return;
				case '&': TokenizerState = HtmlTokenizerState.AttributeValueUnquoted;return;
				case '/':
					TokenizerState = HtmlTokenizerState.SelfClosingStartTag;
                    return;
                case '>':
					 EmitTagToken ();
                     return;
				case '<': case '=': case '`':
					// parse error
					goto default;
				default:
					TokenizerState = HtmlTokenizerState.AttributeName;
					name.Append (c == '\0' ? '\uFFFD' : c);
                    return;
                }
			} while (true);
		}

		void ReadAttributeValueQuoted ()
		{
			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					name.Length = 0;

					EmitDataToken ( false);
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '&':
					TokenizerState = HtmlTokenizerState.CharacterReferenceInAttributeValue;
					token = null;
                    return;
				default:
					if (c == quote) {
						TokenizerState = HtmlTokenizerState.AfterAttributeValueQuoted;
						break;
					}

					name.Append (c == '\0' ? '\uFFFD' : c);
					break;
				}
			} while (TokenizerState == HtmlTokenizerState.AttributeValueQuoted);

			attribute.Value = name.ToString ();
			name.Length = 0;
			token = null;

            return;
		}

		void ReadAttributeValueUnquoted ()
		{
			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					name.Length = 0;

					EmitDataToken (false);
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\t': case '\r': case '\n': case '\f': case ' ':
					TokenizerState = HtmlTokenizerState.BeforeAttributeName;
					break;
				case '&':
					TokenizerState = HtmlTokenizerState.CharacterReferenceInAttributeValue;
					token = null;
                    return;
				case '>':
					EmitTagToken ();
                    return;
				case '\'': case '<': case '=': case '`':
					// parse error
					goto default;
				default:
					if (c == quote) {
						TokenizerState = HtmlTokenizerState.AfterAttributeValueQuoted;
						break;
					}

					name.Append (c == '\0' ? '\uFFFD' : c);
					break;
				}
			} while (TokenizerState == HtmlTokenizerState.AttributeValueUnquoted);

			attribute.Value = name.ToString ();
			name.Length = 0;
			token = null;

			
		}

		void ReadCharacterReferenceInAttributeValue ()
		{
			char additionalAllowedCharacter = quote == '\0' ? '>' : quote;
			int nc = Peek ();
			bool consume;
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.EndOfFile;
				data.Append ('&');
				name.Length = 0;

				EmitDataToken (false);
                return;
			}

			c = (char) nc;
			token = null;

			switch (c) {
			case '\t': case '\r': case '\n': case '\f': case ' ': case '<': case '&':
				// no character is consumed, emit '&'
				data.Append ('&');
				name.Append ('&');
				consume = false;
				break;
			default:
				if (c == additionalAllowedCharacter) {
					// this is not a character reference, nothing is consumed
					data.Append ('&');
					name.Append ('&');
					consume = false;
					break;
				}

				entity.Push ('&');

				while (entity.Push (c)) {
					Read ();

					if ((nc = Peek ()) == -1) {
						TokenizerState = HtmlTokenizerState.EndOfFile;
						data.Append (entity.GetPushedInput ());
						entity.Reset ();

						EmitDataToken ( false);
                        return;
					}

					c = (char) nc;
				}

				var pushed = entity.GetPushedInput ();
				string value;

				if (c == '=' || IsAlphaNumeric (c))
					value = pushed;
				else
					value = entity.GetValue ();

				data.Append (pushed);
				name.Append (value);
				consume = c == ';';
				entity.Reset ();
				break;
			}

			if (quote == '\0')
				TokenizerState = HtmlTokenizerState.AttributeValueUnquoted;
			else
				TokenizerState = HtmlTokenizerState.AttributeValueQuoted;

			if (consume)
				Read ();

			
		}

		void ReadAfterAttributeValueQuoted ()
		{
			int nc = Peek ();
			bool consume;
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.EndOfFile;
				EmitDataToken ( false);
                return;
			}

			c = (char) nc;
			token = null;

			switch (c) {
			case '\t': case '\r': case '\n': case '\f': case ' ':
				TokenizerState = HtmlTokenizerState.BeforeAttributeName;
				consume = true;
				break;
			case '/':
				TokenizerState = HtmlTokenizerState.SelfClosingStartTag;
				consume = true;
				break;
			case '>':
				EmitTagToken ();
				consume = true;
				break;
			default:
				TokenizerState = HtmlTokenizerState.BeforeAttributeName;
				consume = false;
				break;
			}

			if (consume)
				Read ();			
		}

		void ReadSelfClosingStartTag ()
		{
			int nc = Read ();
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.EndOfFile;
			    EmitDataToken (false);
                return;
			}

			c = (char) nc;

			if (c == '>') {
				tag.IsEmptyElement = true;

				EmitTagToken ();
                return;
			}

			// parse error
			TokenizerState = HtmlTokenizerState.BeforeAttributeName;

			// Note: we save the data in case we hit a parse error and have to emit a data token
			data.Append (c);

			token = null;

			
		}

		void ReadBogusComment ()
		{
			int nc;
			char c;

			if (data.Length > 0) {
				c = data[data.Length - 1];
				data.Length = 1;
				data[0] = c;
			}

			do {
				if ((nc = Read ()) == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					break;
				}

				if ((c = (char) nc) == '>')
					break;

				data.Append (c == '\0' ? '\uFFFD' : c);
			} while (true);

		     EmitCommentToken (data);
		}

		void ReadMarkupDeclarationOpen ()
		{
			int count = 0, nc;
			char c = '\0';

			while (count < 2) {
				if ((nc = Peek ()) == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					EmitDataToken ( false);
                    return;
				}

				if ((c = (char) nc) != '-')
					break;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);
				Read ();
				count++;
			}

			token = null;

			if (count == 2) {
				TokenizerState = HtmlTokenizerState.CommentStart;
				name.Length = 0;
                return;
			}

			if (count == 1) {
				// parse error
				TokenizerState = HtmlTokenizerState.BogusComment;
                return;
			}

			if (c == 'D' || c == 'd') {
				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);
				name.Append (c);
				Read ();
				count = 1;

				while (count < 7) {
					if ((nc = Read ()) == -1) {
						TokenizerState = HtmlTokenizerState.EndOfFile;
						EmitDataToken ( false);
                        return;
					}

					if (ToLower ((c = (char) nc)) != DocType[count])
						break;

					// Note: we save the data in case we hit a parse error and have to emit a data token
					data.Append (c);
					name.Append (c);
					count++;
				}

				if (count == 7) {
					doctype = CreateDocTypeToken (name.ToString ());
					TokenizerState = HtmlTokenizerState.DocType;
					name.Length = 0;
                    return;
				}

				name.Length = 0;
			} else if (c == '[') {
				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);
				Read ();
				count = 1;

				while (count < 7) {
					if ((nc = Read ()) == -1) {
						TokenizerState = HtmlTokenizerState.EndOfFile;
						EmitDataToken ( false);
                        return;
					}

					if ((c = (char) nc) != CData[count])
						break;

					// Note: we save the data in case we hit a parse error and have to emit a data token
					data.Append (c);
					count++;
				}

				if (count == 7) {
					TokenizerState = HtmlTokenizerState.CDataSection;
                    return;
				}
			}

			// parse error
			TokenizerState = HtmlTokenizerState.BogusComment;

			
		}

		void ReadCommentStart ()
		{
			int nc = Read ();
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.Data;

				EmitCommentToken (string.Empty);
                return;
			}

			c = (char) nc;

			data.Append (c);

			switch (c) {
			case '-':
				TokenizerState = HtmlTokenizerState.CommentStartDash;
				break;
			case '>': // parse error
				TokenizerState = HtmlTokenizerState.Data;
				 EmitCommentToken (string.Empty);
                 return;
			default: // parse error
				TokenizerState = HtmlTokenizerState.Comment;
				name.Append (c == '\0' ? '\uFFFD' : c);
				break;
			}

			token = null; 
		}

		void ReadCommentStartDash ()
		{
			int nc = Read ();
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.Data;
			    EmitCommentToken (name);
                return;
			}

			c = (char) nc;

			data.Append (c);

			switch (c) {
			case '-':
				TokenizerState = HtmlTokenizerState.CommentEnd;
				break;
			case '>': // parse error
				TokenizerState = HtmlTokenizerState.Data;
				EmitCommentToken (name);
                return;
			default: // parse error
				TokenizerState = HtmlTokenizerState.Comment;
				name.Append ('-');
				name.Append (c == '\0' ? '\uFFFD' : c);
				break;
			}

			token = null;			 
		}

		void ReadComment ()
		{
			token = null;

			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
			        EmitCommentToken (name);
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '-':
					TokenizerState = HtmlTokenizerState.CommentEndDash;
                    return;
				default:
					name.Append (c == '\0' ? '\uFFFD' : c);
					break;
				}
			} while (true);
		}

		// FIXME: this is exactly the same as ReadCommentStartDash
		void ReadCommentEndDash ()
		{
			int nc = Read ();
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.Data;
				EmitCommentToken (name);
                return;
			}

			c = (char) nc;

			data.Append (c);

			switch (c) {
			case '-':
				TokenizerState = HtmlTokenizerState.CommentEnd;
				break;
			case '>': // parse error
				TokenizerState = HtmlTokenizerState.Data;
				EmitCommentToken (name);
                return;
			default: // parse error
				TokenizerState = HtmlTokenizerState.Comment;
				name.Append ('-');
				name.Append (c == '\0' ? '\uFFFD' : c);
				break;
			}

			token = null;

			 
		}

		void ReadCommentEnd ()
		{
			token = null;

			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					EmitCommentToken (name);
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '>':
					TokenizerState = HtmlTokenizerState.Data;
					EmitCommentToken (name);
                    return;
				case '!': // parse error
					TokenizerState = HtmlTokenizerState.CommentEndBang;
                    return;
				case '-':
					name.Append ('-');
					break;
				default:
					TokenizerState = HtmlTokenizerState.Comment;
					name.Append (c == '\0' ? '\uFFFD' : c);
                    return;
				}
			} while (true);
		}

		void ReadCommentEndBang ()
		{
			int nc = Read ();
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.EndOfFile;
				EmitCommentToken (name);
                return;
			}

			c = (char) nc;

			data.Append (c);

			switch (c) {
			case '-':
				TokenizerState = HtmlTokenizerState.CommentEndDash;
				name.Append ("--!");
				break;
			case '>':
				TokenizerState = HtmlTokenizerState.Data;
				EmitCommentToken (name);
                return;
			default: // parse error
				TokenizerState = HtmlTokenizerState.Comment;
				name.Append ("--!");
				name.Append (c == '\0' ? '\uFFFD' : c);
				break;
			}

			token = null;

			
		}

		void ReadDocType ()
		{
			int nc = Peek ();
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.EndOfFile;
				doctype.ForceQuirksMode = true;
				token = doctype;
				doctype = null;
				data.Length = 0;
				name.Length = 0;
                return;
			}

			TokenizerState = HtmlTokenizerState.BeforeDocTypeName;
			c = (char) nc;
			token = null;

			switch (c) {
			case '\t': case '\r': case '\n': case '\f': case ' ':
				data.Append (c);
				Read ();
				break;
			}

            return;
		}

		void ReadBeforeDocTypeName ()
		{
			token = null;

			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					doctype.ForceQuirksMode = true;
					token = doctype;
					doctype = null;
					data.Length = 0;
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\t': case '\r': case '\n': case '\f': case ' ':
					break;
				case '>':
					TokenizerState = HtmlTokenizerState.Data;
					doctype.ForceQuirksMode = true;
					token = doctype;
					doctype = null;
					data.Length = 0;
                    return;
				case '\0':
					TokenizerState = HtmlTokenizerState.DocTypeName;
					name.Append ('\uFFFD');
                    return;
                default:
					TokenizerState = HtmlTokenizerState.DocTypeName;
					name.Append (c);
                    return;
                }
			} while (true);
		}

		bool ReadDocTypeName ()
		{
			token = null;

			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					doctype.Name = name.ToString ();
					doctype.ForceQuirksMode = true;
					token = doctype;
					data.Length = 0;
					name.Length = 0;
					return true;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\t': case '\r': case '\n': case '\f': case ' ':
					TokenizerState = HtmlTokenizerState.AfterDocTypeName;
					break;
				case '>':
					TokenizerState = HtmlTokenizerState.Data;
					doctype.Name = name.ToString ();
					token = doctype;
					doctype = null;
					data.Length = 0;
					name.Length = 0;
					return true;
				case '\0':
					name.Append ('\uFFFD');
					break;
				default:
					name.Append (c);
					break;
				}
			} while (TokenizerState == HtmlTokenizerState.DocTypeName);

			doctype.Name = name.ToString ();
			name.Length = 0;

			return false;
		}

		bool ReadAfterDocTypeName ()
		{
			token = null;

			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					doctype.ForceQuirksMode = true;
					token = doctype;
					doctype = null;
					data.Length = 0;
					return true;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\t': case '\r': case '\n': case '\f': case ' ':
					break;
				case '>':
					TokenizerState = HtmlTokenizerState.Data;
					token = doctype;
					doctype = null;
					data.Length = 0;
					return true;
				default:
					name.Append (c);
					if (name.Length < 6)
						break;

					if (NameIs ("public")) {
						TokenizerState = HtmlTokenizerState.AfterDocTypePublicKeyword;
						doctype.PublicKeyword = name.ToString ();
					} else if (NameIs ("system")) {
						TokenizerState = HtmlTokenizerState.AfterDocTypeSystemKeyword;
						doctype.SystemKeyword = name.ToString ();
					} else {
						TokenizerState = HtmlTokenizerState.BogusDocType;
					}

					name.Length = 0;
					return false;
				}
			} while (true);
		}

		public bool ReadAfterDocTypePublicKeyword ()
		{
			int nc = Read ();
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.EndOfFile;
				doctype.ForceQuirksMode = true;
				token = doctype;
				doctype = null;
				data.Length = 0;
				return true;
			}

			c = (char) nc;

			// Note: we save the data in case we hit a parse error and have to emit a data token
			data.Append (c);

			switch (c) {
			case '\t': case '\r': case '\n': case '\f': case ' ':
				TokenizerState = HtmlTokenizerState.BeforeDocTypePublicIdentifier;
				break;
			case '"': case '\'': // parse error
				TokenizerState = HtmlTokenizerState.DocTypePublicIdentifierQuoted;
				doctype.PublicIdentifier = string.Empty;
				quote = c;
				break;
			case '>': // parse error
				TokenizerState = HtmlTokenizerState.Data;
				doctype.ForceQuirksMode = true;
				token = doctype;
				doctype = null;
				data.Length = 0;
				return true;
			default: // parse error
				TokenizerState = HtmlTokenizerState.BogusDocType;
				doctype.ForceQuirksMode = true;
				break;
			}

			token = null;

			return false;
		}

		public bool ReadBeforeDocTypePublicIdentifier ()
		{
			token = null;

			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					doctype.ForceQuirksMode = true;
					token = doctype;
					doctype = null;
					data.Length = 0;
					return true;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\t': case '\r': case '\n': case '\f': case ' ':
					break;
				case '"': case '\'':
					TokenizerState = HtmlTokenizerState.DocTypePublicIdentifierQuoted;
					doctype.PublicIdentifier = string.Empty;
					quote = c;
					return false;
				case '>': // parse error
					TokenizerState = HtmlTokenizerState.Data;
					doctype.ForceQuirksMode = true;
					token = doctype;
					doctype = null;
					data.Length = 0;
					return true;
				default: // parse error
					TokenizerState = HtmlTokenizerState.BogusDocType;
					doctype.ForceQuirksMode = true;
					return false;
				}
			} while (true);
		}

		void ReadDocTypePublicIdentifierQuoted ()
		{
			token = null;

			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					doctype.PublicIdentifier = name.ToString ();
					doctype.ForceQuirksMode = true;
					token = doctype;
					doctype = null;
					data.Length = 0;
					name.Length = 0;
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\0': // parse error
					name.Append ('\uFFFD');
					break;
				case '>': // parse error
					TokenizerState = HtmlTokenizerState.Data;
					doctype.PublicIdentifier = name.ToString ();
					doctype.ForceQuirksMode = true;
					token = doctype;
					doctype = null;
					data.Length = 0;
					name.Length = 0;
                    return;
				default:
					if (c == quote) {
						TokenizerState = HtmlTokenizerState.AfterDocTypePublicIdentifier;
						break;
					}

					name.Append (c);
					break;
				}
			} while (TokenizerState == HtmlTokenizerState.DocTypePublicIdentifierQuoted);

			doctype.PublicIdentifier = name.ToString ();
			name.Length = 0;

			
		}

		public void ReadAfterDocTypePublicIdentifier ()
		{
			int nc = Read ();
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.EndOfFile;
				doctype.ForceQuirksMode = true;
				token = doctype;
				doctype = null;
				data.Length = 0;
                return;
			}

			c = (char) nc;

			// Note: we save the data in case we hit a parse error and have to emit a data token
			data.Append (c);

			switch (c) {
			case '\t': case '\r': case '\n': case '\f': case ' ':
				TokenizerState = HtmlTokenizerState.BetweenDocTypePublicAndSystemIdentifiers;
				break;
			case '>':
				TokenizerState = HtmlTokenizerState.Data;
				token = doctype;
				doctype = null;
				data.Length = 0;
                return;
			case '"': case '\'': // parse error
				TokenizerState = HtmlTokenizerState.DocTypeSystemIdentifierQuoted;
				doctype.SystemIdentifier = string.Empty;
				quote = c;
				break;
			default: // parse error
				TokenizerState = HtmlTokenizerState.BogusDocType;
				doctype.ForceQuirksMode = true;
				break;
			}

			token = null;

			 
		}

		void ReadBetweenDocTypePublicAndSystemIdentifiers ()
		{
			token = null;

			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					doctype.ForceQuirksMode = true;
					token = doctype;
					doctype = null;
					data.Length = 0;
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\t': case '\r': case '\n': case '\f': case ' ':
					break;
				case '>':
					TokenizerState = HtmlTokenizerState.Data;
					token = doctype;
					doctype = null;
					data.Length = 0;
                    return;
				case '"': case '\'':
					TokenizerState = HtmlTokenizerState.DocTypeSystemIdentifierQuoted;
					doctype.SystemIdentifier = string.Empty;
					quote = c;
                    return;
				default: // parse error
					TokenizerState = HtmlTokenizerState.BogusDocType;
					doctype.ForceQuirksMode = true;
                    return;
				}
			} while (true);
		}

		void ReadAfterDocTypeSystemKeyword ()
		{
			int nc = Read ();
			char c;

			if (nc == -1) {
				TokenizerState = HtmlTokenizerState.EndOfFile;
				doctype.ForceQuirksMode = true;
				token = doctype;
				doctype = null;
				data.Length = 0;
                return;
			}

			c = (char) nc;

			// Note: we save the data in case we hit a parse error and have to emit a data token
			data.Append (c);

			switch (c) {
			case '\t': case '\r': case '\n': case '\f': case ' ':
				TokenizerState = HtmlTokenizerState.BeforeDocTypeSystemIdentifier;
				break;
			case '"': case '\'': // parse error
				TokenizerState = HtmlTokenizerState.DocTypeSystemIdentifierQuoted;
				doctype.SystemIdentifier = string.Empty;
				quote = c;
				break;
			case '>': // parse error
				TokenizerState = HtmlTokenizerState.Data;
				doctype.ForceQuirksMode = true;
				token = doctype;
				doctype = null;
				data.Length = 0;
                return;
			default: // parse error
				TokenizerState = HtmlTokenizerState.BogusDocType;
				doctype.ForceQuirksMode = true;
				break;
			}

			token = null;			
		}

		void ReadBeforeDocTypeSystemIdentifier ()
		{
			token = null;

			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					doctype.ForceQuirksMode = true;
					token = doctype;
					doctype = null;
					data.Length = 0;
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\t': case '\r': case '\n': case '\f': case ' ':
					break;
				case '"': case '\'':
					TokenizerState = HtmlTokenizerState.DocTypeSystemIdentifierQuoted;
					doctype.SystemIdentifier = string.Empty;
					quote = c;
                    return;
				case '>': // parse error
					TokenizerState = HtmlTokenizerState.Data;
					doctype.ForceQuirksMode = true;
					token = doctype;
					doctype = null;
					data.Length = 0;
					return;
				default: // parse error
					TokenizerState = HtmlTokenizerState.BogusDocType;
					doctype.ForceQuirksMode = true;
                    return;
                }
			} while (true);
		}

		void ReadDocTypeSystemIdentifierQuoted ()
		{
			token = null;

			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					doctype.SystemIdentifier = name.ToString ();
					doctype.ForceQuirksMode = true;
					token = doctype;
					doctype = null;
					data.Length = 0;
					name.Length = 0;
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\0': // parse error
					name.Append ('\uFFFD');
					break;
				case '>': // parse error
					TokenizerState = HtmlTokenizerState.Data;
					doctype.SystemIdentifier = name.ToString ();
					doctype.ForceQuirksMode = true;
					token = doctype;
					doctype = null;
					data.Length = 0;
					name.Length = 0;
					return;
				default:
					if (c == quote) {
						TokenizerState = HtmlTokenizerState.AfterDocTypeSystemIdentifier;
						break;
					}

					name.Append (c);
					break;
				}
			} while (TokenizerState == HtmlTokenizerState.DocTypeSystemIdentifierQuoted);

			doctype.SystemIdentifier = name.ToString ();
			name.Length = 0;

            
		}

		public void ReadAfterDocTypeSystemIdentifier ()
		{
			token = null;

			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					doctype.ForceQuirksMode = true;
					token = doctype;
					doctype = null;
					data.Length = 0;
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				switch (c) {
				case '\t': case'\r': case '\n': case '\f': case ' ':
					break;
				case '>':
					TokenizerState = HtmlTokenizerState.Data;
					token = doctype;
					doctype = null;
					data.Length = 0;
					return;
				default: // parse error
					TokenizerState = HtmlTokenizerState.BogusDocType;
					return;
				}
			} while (true);
		}

		void ReadBogusDocType ()
		{
			token = null;

			do {
				int nc = Read ();
				char c;

				if (nc == -1) {
					TokenizerState = HtmlTokenizerState.EndOfFile;
					doctype.ForceQuirksMode = true;
					token = doctype;
					doctype = null;
					data.Length = 0;
                    return;
				}

				c = (char) nc;

				// Note: we save the data in case we hit a parse error and have to emit a data token
				data.Append (c);

				if (c == '>') {
					TokenizerState = HtmlTokenizerState.Data;
					token = doctype;
					doctype = null;
					data.Length = 0;
                    return;
				}
			} while (true);
		}

		void ReadCDataSection ()
		{
			// FIXME: maybe we should have a CDATA token?
			int nc = Read ();

			while (nc != -1) {
				char c = (char) nc;

                if (cdataIndex >= 3) {
                    data.Append(cdata[0]);
                    cdata[0] = cdata[1];
                    cdata[1] = cdata[2];
                    cdata[2] = c;

                    if (cdata[0] == ']' && cdata[1] == ']' && cdata[2] == '>') {
                        TokenizerState = HtmlTokenizerState.Data;
                        cdataIndex = 0;

                        EmitDataToken(true);
                        return;
                    }

                    if (data.Length > 1024)
                    {
                        EmitDataToken(true);
                        return;
                    }
				} else {
					cdata[cdataIndex++] = c;
				}

				nc = Read ();
			}

			TokenizerState = HtmlTokenizerState.EndOfFile;

			for (int i = 0; i < cdataIndex; i++)
				data.Append (cdata[i]);

			cdataIndex = 0;

			EmitDataToken ( true);
		}

		
	}
}
