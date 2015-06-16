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

namespace HtmlKit
{
    public class TokenizerEventArgs : System.EventArgs
    {
        public HtmlTokenKind TokenKind { get; internal set; }
        public int LineNumber { get; internal set; }
        public int ColumnNumber { get; internal set; }
        public string Data { get; set; }
        /// <summary>
        /// stop at this point, or not
        /// </summary>
        public bool Stop { get; set; }
    }

    public delegate void TokenizerEmit(TokenizerEventArgs e);

    /// <summary>
    /// An HTML tokenizer.
    /// </summary>
    /// <remarks>
    /// Tokenizes HTML text, emitting an <see cref="HtmlToken"/> for each token it encounters.
    /// </remarks>
    partial class HtmlTokenizer
    {

        public event TokenizerEmit TokenEmit;
        public bool UseEventEmitterModel { get; set; }
        bool stopTokenizer;
        

        HtmlToken token
        {
            get;
            set;
        }
                

        public bool ReadNextToken(out HtmlToken output)
        {
            token = null;

            while (TokenizerState != HtmlTokenizerState.EndOfFile)
            {

                switch (TokenizerState)
                {
                    case HtmlTokenizerState.Data:
                        ReadDataToken();
                        break;
                    case HtmlTokenizerState.CharacterReferenceInData:
                        ReadCharacterReferenceInData();
                        break;
                    case HtmlTokenizerState.RcData:
                        ReadRcData();
                        break;
                    case HtmlTokenizerState.CharacterReferenceInRcData:
                        ReadCharacterReferenceInRcData();
                        break;
                    case HtmlTokenizerState.RawText:
                        ReadRawText();
                        break;
                    case HtmlTokenizerState.ScriptData:
                        ReadScriptData();
                        break;
                    case HtmlTokenizerState.PlainText:
                        ReadPlainText();
                        break;
                    case HtmlTokenizerState.TagOpen:
                        ReadTagOpen();
                        break;
                    case HtmlTokenizerState.EndTagOpen:
                        ReadEndTagOpen();
                        break;
                    case HtmlTokenizerState.TagName:
                        ReadTagName();
                        break;
                    case HtmlTokenizerState.RcDataLessThan:
                        ReadRcDataLessThan();
                        break;
                    case HtmlTokenizerState.RcDataEndTagOpen:
                        ReadRcDataEndTagOpen();
                        break;
                    case HtmlTokenizerState.RcDataEndTagName:
                        ReadRcDataEndTagName();
                        break;
                    case HtmlTokenizerState.RawTextLessThan:
                        ReadRawTextLessThan();
                        break;
                    case HtmlTokenizerState.RawTextEndTagOpen:
                        ReadRawTextEndTagOpen();
                        break;
                    case HtmlTokenizerState.RawTextEndTagName:
                        ReadRawTextEndTagName();
                        break;
                    case HtmlTokenizerState.ScriptDataLessThan:
                        ReadScriptDataLessThan();
                        break;
                    case HtmlTokenizerState.ScriptDataEndTagOpen:
                        ReadScriptDataEndTagOpen();
                        break;
                    case HtmlTokenizerState.ScriptDataEndTagName:
                        ReadScriptDataEndTagName();
                        break;
                    case HtmlTokenizerState.ScriptDataEscapeStart:
                        ReadScriptDataEscapeStart();
                        break;
                    case HtmlTokenizerState.ScriptDataEscapeStartDash:
                        ReadScriptDataEscapeStartDash();
                        break;
                    case HtmlTokenizerState.ScriptDataEscaped:
                        ReadScriptDataEscaped();
                        break;
                    case HtmlTokenizerState.ScriptDataEscapedDash:
                        ReadScriptDataEscapedDash();
                        break;
                    case HtmlTokenizerState.ScriptDataEscapedDashDash:
                        ReadScriptDataEscapedDashDash();
                        break;
                    case HtmlTokenizerState.ScriptDataEscapedLessThan:
                        ReadScriptDataEscapedLessThan();
                        break;
                    case HtmlTokenizerState.ScriptDataEscapedEndTagOpen:
                        ReadScriptDataEscapedEndTagOpen();
                        break;
                    case HtmlTokenizerState.ScriptDataEscapedEndTagName:
                        ReadScriptDataEscapedEndTagName();
                        break;
                    case HtmlTokenizerState.ScriptDataDoubleEscapeStart:
                        ReadScriptDataDoubleEscapeStart();
                        break;
                    case HtmlTokenizerState.ScriptDataDoubleEscaped:
                        ReadScriptDataDoubleEscaped();
                        break;
                    case HtmlTokenizerState.ScriptDataDoubleEscapedDash:
                        ReadScriptDataDoubleEscapedDash();
                        break;
                    case HtmlTokenizerState.ScriptDataDoubleEscapedDashDash:
                        ReadScriptDataDoubleEscapedDashDash();
                        break;
                    case HtmlTokenizerState.ScriptDataDoubleEscapedLessThan:
                        ReadScriptDataDoubleEscapedLessThan();
                        break;
                    case HtmlTokenizerState.ScriptDataDoubleEscapeEnd:
                        ReadScriptDataDoubleEscapeEnd();
                        break;
                    case HtmlTokenizerState.BeforeAttributeName:
                        ReadBeforeAttributeName();
                        break;
                    case HtmlTokenizerState.AttributeName:
                        ReadAttributeName();
                        break;
                    case HtmlTokenizerState.AfterAttributeName:
                        ReadAfterAttributeName();
                        break;
                    case HtmlTokenizerState.BeforeAttributeValue:
                        ReadBeforeAttributeValue();
                        break;
                    case HtmlTokenizerState.AttributeValueQuoted:
                        ReadAttributeValueQuoted();
                        break;
                    case HtmlTokenizerState.AttributeValueUnquoted:
                        ReadAttributeValueUnquoted();
                        break;
                    case HtmlTokenizerState.CharacterReferenceInAttributeValue:
                        ReadCharacterReferenceInAttributeValue();
                        break;
                    case HtmlTokenizerState.AfterAttributeValueQuoted:
                        ReadAfterAttributeValueQuoted();
                        break;
                    case HtmlTokenizerState.SelfClosingStartTag:
                        ReadSelfClosingStartTag();
                        break;
                    case HtmlTokenizerState.BogusComment:
                        ReadBogusComment();
                        break;
                    case HtmlTokenizerState.MarkupDeclarationOpen:
                        ReadMarkupDeclarationOpen();
                        break;
                    case HtmlTokenizerState.CommentStart:
                        ReadCommentStart();
                        break;
                    case HtmlTokenizerState.CommentStartDash:
                        ReadCommentStartDash();
                        break;
                    case HtmlTokenizerState.Comment:
                        ReadComment();
                        break;
                    case HtmlTokenizerState.CommentEndDash:
                        ReadCommentEndDash();
                        break;
                    case HtmlTokenizerState.CommentEnd:
                        ReadCommentEnd();
                        break;
                    case HtmlTokenizerState.CommentEndBang:
                        ReadCommentEndBang();
                        break;
                    case HtmlTokenizerState.DocType:
                        ReadDocType();
                        break;
                    case HtmlTokenizerState.BeforeDocTypeName:
                        ReadBeforeDocTypeName();
                        break;
                    case HtmlTokenizerState.DocTypeName:
                        ReadDocTypeName();
                        break;
                    case HtmlTokenizerState.AfterDocTypeName:
                        ReadAfterDocTypeName();
                        break;
                    case HtmlTokenizerState.AfterDocTypePublicKeyword:
                        ReadAfterDocTypePublicKeyword();
                        break;
                    case HtmlTokenizerState.BeforeDocTypePublicIdentifier:
                        ReadBeforeDocTypePublicIdentifier();
                        break;
                    case HtmlTokenizerState.DocTypePublicIdentifierQuoted:
                        ReadDocTypePublicIdentifierQuoted();
                        break;
                    case HtmlTokenizerState.AfterDocTypePublicIdentifier:
                        ReadAfterDocTypePublicIdentifier();
                        break;
                    case HtmlTokenizerState.BetweenDocTypePublicAndSystemIdentifiers:
                        ReadBetweenDocTypePublicAndSystemIdentifiers();
                        break;
                    case HtmlTokenizerState.AfterDocTypeSystemKeyword:
                        ReadAfterDocTypeSystemKeyword();
                        break;
                    case HtmlTokenizerState.BeforeDocTypeSystemIdentifier:
                        ReadBeforeDocTypeSystemIdentifier();
                        break;
                    case HtmlTokenizerState.DocTypeSystemIdentifierQuoted:
                        ReadDocTypeSystemIdentifierQuoted();
                        break;
                    case HtmlTokenizerState.AfterDocTypeSystemIdentifier:
                        ReadAfterDocTypeSystemIdentifier();
                        break;
                    case HtmlTokenizerState.BogusDocType:
                        ReadBogusDocType();
                        break;
                    case HtmlTokenizerState.CDataSection:
                        ReadCDataSection();
                        break;
                    case HtmlTokenizerState.EndOfFile:
                        output = token = null;
                        return false;
                }

                if ((output = token) != null)
                {
                    return true;//found next token 
                }
            }
            //3.
            output = token = null;
            return false;
        }
    }
}