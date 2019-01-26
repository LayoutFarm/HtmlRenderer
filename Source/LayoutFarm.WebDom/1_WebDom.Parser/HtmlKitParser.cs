//MIT, 2015-present, WinterDev

using System;
using HtmlKit;
namespace LayoutFarm.WebDom.Parser
{
    class HtmlKitParser : HtmlParser
    {
        WebDocument _resultHtmlDoc;
        HtmlStack _openEltStack = new HtmlStack();
        public override void Parse(TextSource textSnapshot, WebDocument htmldoc, DomElement currentNode)
        {
            _resultHtmlDoc = htmldoc;
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
                                    DomElement elem = _resultHtmlDoc.CreateElement(null, tag.Name);
                                    
                                    currentNode.AddChild(elem);
                                    foreach (var attribute in tag.Attributes)
                                    {
                                        var attr = _resultHtmlDoc.CreateAttribute(null, attribute.Name);
                                        if (attribute.Value != null)
                                        {
                                            attr.Value = attribute.Value;
                                        }
                                        elem.AddAttribute(attr);
                                    }
                                    if (!tag.IsEmptyElement)
                                    {
                                        _openEltStack.Push(currentNode);
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
                                        currentNode = _openEltStack.Pop();
                                    }
                                    else
                                    {
                                        //if not equal then check if current node need close tag or not
                                        int count = 20;//?
                                        bool ok = false;
                                        while (count > 0)
                                        {
                                            if (HtmlTagMatching.IsSingleTag(currentNode.LocalNameIndex))
                                            {
                                                if (_openEltStack.Count > 0)
                                                {
                                                    currentNode = _openEltStack.Pop();
                                                }
                                                if (currentNode.LocalName == tag.Name)
                                                {
                                                    if (_openEltStack.Count > 0)
                                                    {
                                                        currentNode = _openEltStack.Pop();
                                                        ok = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            else if (HtmlTagMatching.CanAutoClose(currentNode.LocalNameIndex))
                                            {
                                                if (_openEltStack.Count > 0)
                                                {
                                                    currentNode = _openEltStack.Pop();
                                                }
                                                if (currentNode.LocalName == tag.Name)
                                                {
                                                    if (_openEltStack.Count > 0)
                                                    {
                                                        currentNode = _openEltStack.Pop();
                                                        ok = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //implement err handling here!
                                                throw new NotSupportedException();
                                            }
                                            count--;
                                        }
                                        if (!ok)
                                        {
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
            _resultHtmlDoc = null;
            _openEltStack.Clear();
        }
    }
}