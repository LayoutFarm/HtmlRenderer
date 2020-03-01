//
// HtmlTokenizerTests.cs
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

using System;
using System.IO;
using System.Text;
using HtmlKit;
using static System.Diagnostics.Debug;
namespace UnitTests
{

    public class HtmlTokenizerTests
    {

        static string Quote(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            var quoted = new StringBuilder(text.Length + 2, (text.Length * 2) + 2);
            quoted.Append("\"");
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\\' || text[i] == '"')
                    quoted.Append('\\');
                quoted.Append(text[i]);
            }
            quoted.Append("\"");
            return quoted.ToString();
        }

        static void VerifyHtmlTokenizerOutput(string path)
        {
            string tokens = Path.ChangeExtension(path, ".tokens");
            string expected = File.Exists(tokens) ? File.ReadAllText(tokens) : string.Empty;
            var actual = new StringBuilder();
            using (var textReader = File.OpenText(path))
            {
                var tokenizer = new HtmlTokenizer(textReader);
                HtmlToken token;
                Assert(HtmlTokenizerState.Data == tokenizer.TokenizerState);

                while (tokenizer.ReadNextToken(out token))
                {
                    actual.AppendFormat("{0}: ", token.Kind);
                    switch (token.Kind)
                    {
                        case HtmlTokenKind.Data:
                            var text = (HtmlDataToken)token;
                            for (int i = 0; i < text.Data.Length; i++)
                            {
                                switch (text.Data[i])
                                {
                                    case '\f': actual.Append("\\f"); break;
                                    case '\t': actual.Append("\\t"); break;
                                    case '\r': actual.Append("\\r"); break;
                                    case '\n': actual.Append("\\n"); break;
                                    default: actual.Append(text.Data[i]); break;
                                }
                            }
                            actual.AppendLine();
                            break;
                        case HtmlTokenKind.Tag:
                            var tag = (HtmlTagToken)token;
                            actual.AppendFormat("<{0}{1}", tag.IsEndTag ? "/" : "", tag.Name);
                            foreach (var attribute in tag.Attributes)
                            {
                                if (attribute.Value != null)
                                    actual.AppendFormat(" {0}={1}", attribute.Name, Quote(attribute.Value));
                                else
                                    actual.AppendFormat(" {0}", attribute.Name);
                            }

                            actual.Append(tag.IsEmptyElement ? "/>" : ">");
                            actual.AppendLine();
                            break;
                        case HtmlTokenKind.Comment:
                            var comment = (HtmlCommentToken)token;
                            actual.AppendLine(comment.Comment);
                            break;
                        case HtmlTokenKind.DocType:
                            var doctype = (HtmlDocTypeToken)token;
                            if (doctype.ForceQuirksMode)
                                actual.Append("<!-- force quirks mode -->");
                            actual.Append("<!DOCTYPE");
                            if (doctype.Name != null)
                                actual.AppendFormat(" {0}", doctype.Name.ToUpperInvariant());
                            if (doctype.PublicIdentifier != null)
                            {
                                actual.AppendFormat(" PUBLIC {0}", Quote(doctype.PublicIdentifier));
                                if (doctype.SystemIdentifier != null)
                                    actual.AppendFormat(" {0}", Quote(doctype.SystemIdentifier));
                            }
                            else if (doctype.SystemIdentifier != null)
                            {
                                actual.AppendFormat(" SYSTEM {0}", Quote(doctype.SystemIdentifier));
                            }

                            actual.Append(">");
                            actual.AppendLine();
                            break;
                        default:
                            Fail($"Unhandled token type: {token.Kind}");
                            break;
                    }
                }
                Assert(HtmlTokenizerState.EndOfFile == tokenizer.TokenizerState);
            }

            if (!File.Exists(tokens))
                File.WriteAllText(tokens, actual.ToString());

            Assert(expected == actual.ToString(), "The token stream does not match the expected tokens.");
        }

        //[Test]
        public void TestGoogleSignInAttemptBlocked()
        {
            VerifyHtmlTokenizerOutput(CombinePath("..", "..", "TestData", "html", "blocked.html"));
        }

        //[Test]
        public void TestXamarin3SampleHtml()
        {
            VerifyHtmlTokenizerOutput(CombinePath("..", "..", "TestData", "html", "xamarin3.html"));
        }

        //[Test]
        public void TestTokenizer()
        {
            VerifyHtmlTokenizerOutput(CombinePath("..", "..", "TestData", "html", "test.html"));
        }

        static string CombinePath(params string[] subpaths)
        {
            //for net20
            int j = subpaths.Length;
            switch (j)
            {
                case 0:
                    return "";
                case 1:
                    return subpaths[0];
                default:
                    string finalPath = subpaths[0];
                    for (int i = 1; i < j; ++i)
                    {
                        finalPath = Path.Combine(finalPath, subpaths[i]);
                    }
                    return finalPath;
            }
        }
    }
}
