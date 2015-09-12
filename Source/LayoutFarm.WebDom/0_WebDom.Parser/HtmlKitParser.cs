//MIT 2015, WinterDev

using System;
using HtmlKit;

namespace LayoutFarm.WebDom.Parser
{
    class HtmlKitParser : HtmlParser
    {
        WebDocument _resultHtmlDoc;
        HtmlStack openEltStack = new HtmlStack();

        public override void Parse(TextSource textSnapshot, WebDocument htmldoc, DomElement currentNode)
        {
            this._resultHtmlDoc = htmldoc;
            char[] copyBuffer = textSnapshot.ActualSnapshot.Copy(0, textSnapshot.ActualSnapshot.Length);
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
                                currentNode.AddChild(_resultHtmlDoc.CreateTextNode(text.Data.ToCharArray()));
                            }
                            break;
                        case HtmlTokenKind.Tag:
                            {
                                var tag = (HtmlTagToken)token;
                                if (!tag.IsEndTag)
                                {
                                    //open tag 
                                    DomElement elem = this._resultHtmlDoc.CreateElement(null, tag.Name);
                                    currentNode.AddChild(elem);

                                    foreach (var attribute in tag.Attributes)
                                    {
                                        var attr = this._resultHtmlDoc.CreateAttribute(null, attribute.Name);
                                        if (attribute.Value != null)
                                        {
                                            attr.Value = attribute.Value;
                                        }
                                        elem.AddAttribute(attr);
                                    }
                                    if (!tag.IsEmptyElement)
                                    {
                                        openEltStack.Push(currentNode);
                                        currentNode = elem;
                                    }
                                }
                                else
                                {
                                    //this is end tag
                                    //check end tag match or not
                                    int tagNameIndex = _resultHtmlDoc.AddStringIfNotExists(tag.Name);

                                    if (currentNode.Name == tag.Name)
                                    {
                                        currentNode = openEltStack.Pop();
                                    }
                                    else
                                    {
                                        //if not equal then check if current node need close tag or not
                                        if (HtmlDecodeHelper.IsSingleTag(currentNode.LocalNameIndex))
                                        {
                                            if (openEltStack.Count > 0)
                                            {
                                                currentNode = openEltStack.Pop();
                                            }
                                            if (currentNode.LocalName == tag.Name)
                                            {
                                                if (openEltStack.Count > 0)
                                                {
                                                    currentNode = openEltStack.Pop();
                                                }
                                            }
                                            else
                                            {
                                                //implement err handling here!
                                                throw new NotSupportedException();
                                            }
                                        }
                                        else
                                        {
                                            //implement err handling here!
                                            throw new NotSupportedException();
                                        }
                                    }

                                }

                            }
                            break;
                        case HtmlTokenKind.Comment:

                            break;
                        case HtmlTokenKind.DocType:

                            break;
                        default:
                            {
                            }
                            break;
                    }
                }
            }
        }
        public override void ResetParser()
        {
            this._resultHtmlDoc = null;
            this.openEltStack.Clear();
        }
    }

}