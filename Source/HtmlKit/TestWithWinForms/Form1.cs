using System;
using System.IO;

using System.Text;
using System.Windows.Forms;


using LayoutFarm.WebDom.Parser;

using HtmlKit;
using NUnit.Framework;

namespace TestWithWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //test our html lexer





        }
        private void button1_Click(object sender, EventArgs e)
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
            var tokens = Path.ChangeExtension(path, ".tokens");
            var expected = File.Exists(tokens) ? File.ReadAllText(tokens) : string.Empty;
            var actual = new StringBuilder();
           
            using (var textReader = File.OpenText(path))
            {
                var tokenizer = new HtmlTokenizer(textReader);
                HtmlToken token;

                Assert.AreEqual(HtmlTokenizerState.Data, tokenizer.TokenizerState);

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
                            Assert.Fail("Unhandled token type: {0}", token.Kind);
                            break;
                    }
                }

                Assert.AreEqual(HtmlTokenizerState.EndOfFile, tokenizer.TokenizerState);
            }

            if (!File.Exists(tokens))
                File.WriteAllText(tokens, actual.ToString());

            Assert.AreEqual(expected, actual.ToString(), "The token stream does not match the expected tokens.");
        }


    }
}
