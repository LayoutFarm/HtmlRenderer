//MIT  2015,2014 ,WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.WebLexer;
using HtmlKit;

namespace LayoutFarm.WebDom.Parser
{

    class HtmlKitLexer : HtmlLexer
    {
        public HtmlKitLexer()
        {

        }
        public override void Analyze(TextSnapshot textSnapshot)
        {

            char[] copyBuffer = textSnapshot.Copy(0, textSnapshot.Length);
            using (var ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(copyBuffer)))
            using (var textReader = new System.IO.StreamReader(ms))
            {
                var tokenizer = new HtmlTokenizer(textReader);
                HtmlToken token;
                while (tokenizer.ReadNextToken(out token))
                {
                    switch (token.Kind)
                    {
                        case HtmlTokenKind.Data:
                            {
                                var text = (HtmlDataToken)token;
                                for (int i = 0; i < text.Data.Length; i++)
                                {

                                }
                            } break;
                        case HtmlTokenKind.Tag:
                            {
                                var tag = (HtmlTagToken)token;

                                foreach (var attribute in tag.Attributes)
                                {

                                }
                            } break;
                        case HtmlTokenKind.Comment:

                            break;
                        case HtmlTokenKind.DocType: 

                            break;
                        default: 
                            break;
                    }
                }
            }
        }
        public override void BeginLex()
        {

        }
        public override void EndLex()
        {

        }
    }

}