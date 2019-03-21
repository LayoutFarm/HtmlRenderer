//BSD, 2014-present, WinterDev 

using System;
using LayoutFarm.WebLexer;
using HtmlLexerEvent = LayoutFarm.WebDom.Parser.XmlLexerEvent;

namespace LayoutFarm.WebDom.Parser
{
    class MyHtmlParser : HtmlParser
    {
        WebDocument _resultHtmlDoc;
        HtmlStack _openEltStack = new HtmlStack();
        DomElement _curHtmlNode = null;
        DomAttribute _curAttr = null;
        DomTextNode _curTextNode = null;
        int _parseState = 0;
        TextSnapshot _textSnapshot;
        HtmlLexer _lexer;
        string _waitingAttrName;
        DomDocumentNode _domDocNode;
        public MyHtmlParser()
        {
            _lexer = HtmlLexer.CreateLexer();
            _lexer.LexStateChanged += LexStateChanged;
        }


        void LexStateChanged(HtmlLexerEvent lexEvent, int startIndex, int len)
        {
            switch (lexEvent)
            {
                case HtmlLexerEvent.CommentContent:
                    {
                        //var commentContent = this.textSnapshot.Copy(startIndex, len); 

                    }
                    break;
                case HtmlLexerEvent.FromContentPart:
                    {
                        if (_curTextNode == null)
                        {
                            _curTextNode = _resultHtmlDoc.CreateTextNode(
                                HtmlDecodeHelper.DecodeHtml(_textSnapshot, startIndex, len));
                            if (_curHtmlNode != null)
                            {
                                _curHtmlNode.AddChild(_curTextNode);
                            }
                        }
                        else
                        {
                            _curTextNode.AppendTextContent(HtmlDecodeHelper.DecodeHtml(_textSnapshot, startIndex, len));
                        }
                    }
                    break;
                case HtmlLexerEvent.AttributeValueAsLiteralString:
                    {
                        //assign value and add to parent
                        if (_parseState == 11)
                        {
                            //document node
                            //doc 
                            _domDocNode.AddParameter(_textSnapshot.Substring(startIndex, len));
                        }
                        else
                        {
                            _curAttr.Value = _textSnapshot.Substring(startIndex, len);
                            _curHtmlNode.SetAttribute(_curAttr);
                        }
                    }
                    break;
                case HtmlLexerEvent.Attribute:
                    {
                        //create attribute node and wait for its value
                        string nodename = _textSnapshot.Substring(startIndex, len);
                        _curAttr = _resultHtmlDoc.CreateAttribute(nodename);
                    }
                    break;
                case HtmlLexerEvent.NodeNameOrAttribute:
                    {
                        //the lexer dose not store state of element name or attribute name
                        //so we use parseState to decide here
                        string name = _textSnapshot.Substring(startIndex, len);
                        switch (_parseState)
                        {
                            case 0:
                                {
                                    //create element 
                                    DomElement elem = _resultHtmlDoc.CreateElement(null, name);
                                    if (_curHtmlNode != null)
                                    {
                                        _curHtmlNode.AddChild(elem);
                                        _openEltStack.Push(_curHtmlNode);
                                    }
                                    _curHtmlNode = elem;
                                    _parseState = 1;//attribute
                                    _curTextNode = null;
                                    _curAttr = null;
                                    _waitingAttrName = null;
                                }
                                break;
                            case 1:
                                {
                                    //wait for attr value 
                                    if (_waitingAttrName != null)
                                    {
                                        //push waiting attr
                                        _curAttr = _resultHtmlDoc.CreateAttribute(_waitingAttrName, "");
                                        _curHtmlNode.SetAttribute(_curAttr);
                                        _curAttr = null;
                                    }
                                    _waitingAttrName = name;
                                }
                                break;
                            case 2:
                                {
                                    //****
                                    //node name after open slash
                                    //TODO: review here,avoid direct string comparison
                                    if (_curHtmlNode.LocalName == name)
                                    {
                                        if (_openEltStack.Count > 0)
                                        {
                                            _waitingAttrName = null;
                                            _curTextNode = null;
                                            _curAttr = null;
                                            _curHtmlNode = _openEltStack.Pop();
                                        }
                                        _parseState = 3;
                                    }
                                    else
                                    {
                                        //if not equal then check if current node need close tag or not
                                        if (HtmlTagMatching.IsSingleTag(_curHtmlNode.LocalNameIndex))
                                        {
                                            if (_openEltStack.Count > 0)
                                            {
                                                _waitingAttrName = null;
                                                _curHtmlNode = _openEltStack.Pop();
                                                _curAttr = null;
                                                _curTextNode = null;
                                            }
                                            if (_curHtmlNode.LocalName == name)
                                            {
                                                if (_openEltStack.Count > 0)
                                                {
                                                    _curTextNode = null;
                                                    _curAttr = null;
                                                    _curHtmlNode = _openEltStack.Pop();
                                                    _waitingAttrName = null;
                                                }
                                                _parseState = 3;
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
                                break;
                            case 4:
                                {
                                    //attribute value as id
                                    if (_curAttr != null)
                                    {
                                        _curAttr.Value = name;
                                        _curAttr = null;
                                        _parseState = 0;
                                        _waitingAttrName = null;
                                    }
                                    else
                                    {
                                    }
                                }
                                break;
                            case 10:
                                {
                                    //document node 

                                    _parseState = 11;
                                    //after docnodename , this may be attr of the document node
                                    _domDocNode = (DomDocumentNode)_resultHtmlDoc.CreateDocumentNodeElement();
                                    _domDocNode.DocNodeName = name;
                                }
                                break;
                            case 11:
                                {
                                    //doc 
                                    _domDocNode.AddParameter(name);
                                }
                                break;
                            default:
                                {
                                }
                                break;
                        }
                    }
                    break;
                case HtmlLexerEvent.VisitCloseAngle:
                    {
                        //close angle of current new node
                        //enter into its content

                        if (_parseState == 11)
                        {
                            //add doctype to html
                            _resultHtmlDoc.RootNode.AddChild(_domDocNode);
                            _domDocNode = null;
                        }

                        if (_waitingAttrName != null)
                        {
                            _curAttr = _resultHtmlDoc.CreateAttribute(_waitingAttrName, "");
                            _curHtmlNode.SetAttribute(_curAttr);
                            _curAttr = null;
                        }


                        _waitingAttrName = null;
                        _parseState = 0;
                        _curTextNode = null;
                        _curAttr = null;
                    }
                    break;
                case HtmlLexerEvent.VisitAttrAssign:
                    {
                        _parseState = 4;
                    }
                    break;
                case HtmlLexerEvent.VisitOpenSlashAngle:
                    {
                        _parseState = 2;
                    }
                    break;
                case HtmlLexerEvent.VisitCloseSlashAngle:
                    {
                        if (_openEltStack.Count > 0)
                        {
                            _curTextNode = null;
                            _curAttr = null;
                            _waitingAttrName = null;
                            _curHtmlNode = _openEltStack.Pop();
                        }
                        _parseState = 0;
                    }
                    break;
                case HtmlLexerEvent.VisitOpenAngleExclimation:
                    {
                        //eg. doctype
                        _parseState = 10;
                    }
                    break;
                default:
                    {
                        //1. visit open angle
                    }
                    break;
            }
        }

        public override void ResetParser()
        {
            _resultHtmlDoc = null;
            _openEltStack.Clear();
            _curHtmlNode = null;
            _curAttr = null;
            _curTextNode = null;
            _parseState = 0;
            _textSnapshot = null;
        }
        public override void Parse(TextSource textSnapshot, WebDocument htmldoc, DomElement currentNode)
        {
            this.Parse(textSnapshot.ActualSnapshot, htmldoc, currentNode);
        }
        /// <summary>
        /// parse to htmldom
        /// </summary>
        /// <param name="stbuilder"></param>
        internal void Parse(TextSnapshot textSnapshot, WebDocument htmldoc, DomElement currentNode)
        {
            _textSnapshot = textSnapshot;
            //1. lex 
            _lexer.BeginLex();
            //2. mini parser    
            _curHtmlNode = currentNode;
            _resultHtmlDoc = htmldoc;
            _lexer.Analyze(textSnapshot);
            _lexer.EndLex();
        }
    }
}