//BSD  2015,2014 ,WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.WebLexer;
namespace LayoutFarm.WebDom.Parser
{
    public class HtmlParser
    {
        WebDocument _resultHtmlDoc;
        HtmlStack openEltStack = new HtmlStack();

        DomElement curHtmlNode = null;
        DomAttribute curAttr = null;
        DomTextNode curTextNode = null;
        int parseState = 0;
        TextSnapshot textSnapshot;
        HtmlLexer lexer;
        string waitingAttrName;

        public HtmlParser()
        {
            lexer = new HtmlLexer();
            lexer.LexStateChanged += LexStateChanged;
        }


        void LexStateChanged(HtmlLexerEvent lexEvent, int startIndex, int len)
        {
            switch (lexEvent)
            {
                case HtmlLexerEvent.CommentContent:
                    {
                        //var commentContent = this.textSnapshot.Copy(startIndex, len); 

                    } break;
                case HtmlLexerEvent.FromContentPart:
                    {

                        if (curTextNode == null)
                        {
                            curTextNode = _resultHtmlDoc.CreateTextNode(
                                HtmlDecodeHelper.DecodeHtml(this.textSnapshot, startIndex, len));

                            if (curHtmlNode != null)
                            {
                                curHtmlNode.AddChild(curTextNode);
                            }
                        }
                        else
                        {
                            curTextNode.AppendTextContent(HtmlDecodeHelper.DecodeHtml(this.textSnapshot, startIndex, len));

                        }
                    } break;
                case HtmlLexerEvent.AttributeValueAsLiteralString:
                    {
                        //assign value and add to parent
                        curAttr.Value = textSnapshot.Substring(startIndex, len);
                        curHtmlNode.AddAttribute(curAttr);

                    } break;
                case HtmlLexerEvent.AttributeValue:
                    {
                    } break;
                case HtmlLexerEvent.Attribute:
                    {
                        string nodename = textSnapshot.Substring(startIndex, len);
                        curAttr = this._resultHtmlDoc.CreateAttribute(null, nodename);

                    } break;
                case HtmlLexerEvent.NodeNameOrAttribute:
                    {
                        string name = textSnapshot.Substring(startIndex, len);
                        switch (parseState)
                        {
                            case 0:
                                {

                                    DomElement elem = this._resultHtmlDoc.CreateElement(null, name);

                                    if (curHtmlNode != null)
                                    {
                                        curHtmlNode.AddChild(elem);
                                        openEltStack.Push(curHtmlNode);
                                    }
                                    curHtmlNode = elem;
                                    parseState = 1;//attribute
                                    curTextNode = null;
                                    curAttr = null;
                                    waitingAttrName = null;
                                } break;
                            case 1:
                                {
                                    //wait for attr value 
                                    if (waitingAttrName != null)
                                    {
                                        //push waiting attr
                                        curAttr = this._resultHtmlDoc.CreateAttribute(null, waitingAttrName);
                                        curAttr.Value = "";
                                        curHtmlNode.AddAttribute(curAttr);
                                        curAttr = null;
                                    }
                                    waitingAttrName = name;
                                } break;
                            case 2:
                                {
                                    //node name after open slash
                                    if (curHtmlNode.LocalName == name)
                                    {
                                        if (openEltStack.Count > 0)
                                        {
                                            waitingAttrName = null;
                                            curTextNode = null;
                                            curAttr = null;
                                            curHtmlNode = openEltStack.Pop();
                                        }
                                        parseState = 3;
                                    }
                                    else
                                    {
                                        //if not equal then check if current node need close tag or not
                                        if (HtmlDecodeHelper.IsSingleTag(curHtmlNode.LocalNameIndex))
                                        {
                                            if (openEltStack.Count > 0)
                                            {
                                                waitingAttrName = null;
                                                curHtmlNode = openEltStack.Pop();
                                                curAttr = null;
                                                curTextNode = null;
                                            }
                                            if (curHtmlNode.LocalName == name)
                                            {
                                                if (openEltStack.Count > 0)
                                                {
                                                    curTextNode = null;
                                                    curAttr = null;
                                                    curHtmlNode = openEltStack.Pop();
                                                    waitingAttrName = null;
                                                }
                                                parseState = 3;
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
                                } break;
                            case 4:
                                {
                                    //attribute value as id
                                    if (curAttr != null)
                                    {
                                        curAttr.Value = name;
                                        curAttr = null;
                                        parseState = 0;
                                        waitingAttrName = null;
                                    }
                                    else
                                    {

                                    }
                                } break;
                            default:
                                {
                                } break;
                        }

                    } break;
                case HtmlLexerEvent.VisitCloseAngle:
                    {
                        //close angle of current new node
                        //enter into its content

                        if (waitingAttrName != null)
                        {
                            curAttr = this._resultHtmlDoc.CreateAttribute(null, waitingAttrName);
                            curAttr.Value = "";
                            curHtmlNode.AddAttribute(curAttr);
                            curAttr = null;
                        }
                        waitingAttrName = null;
                        parseState = 0;
                        curTextNode = null;
                        curAttr = null;
                    } break;
                case HtmlLexerEvent.VisitAttrAssign:
                    {
                        parseState = 4;
                    } break;
                case HtmlLexerEvent.VisitOpenSlashAngle:
                    {
                        parseState = 2;
                    } break;
                case HtmlLexerEvent.VisitCloseSlashAngle:
                    {

                        if (openEltStack.Count > 0)
                        {
                            curTextNode = null;
                            curAttr = null;
                            waitingAttrName = null;
                            curHtmlNode = openEltStack.Pop();
                        }
                        parseState = 0;

                    } break;
                default:
                    {
                        //1. visit open angle
                    } break;

            }
        }

        public void ResetParser()
        {
            this._resultHtmlDoc = null;
            this.openEltStack.Clear();
            this.curHtmlNode = null;
            this.curAttr = null;
            this.curTextNode = null;
            this.parseState = 0;
            this.textSnapshot = null;
        }
        /// <summary>
        /// parse to htmldom
        /// </summary>
        /// <param name="stbuilder"></param>
        internal void Parse(TextSnapshot textSnapshot, WebDocument htmldoc, DomElement currentNode)
        {
            this.textSnapshot = textSnapshot;
            //1. lex 
            lexer.BeginLex();
            //2. mini parser    
            this.curHtmlNode = currentNode;
            this._resultHtmlDoc = htmldoc;
            lexer.Analyze(textSnapshot);
            lexer.EndLex();

        }
        public void Parse(TextSource textSnapshot, WebDocument htmldoc, DomElement currentNode)
        {
            this.Parse(textSnapshot.ActualSnapshot, htmldoc, currentNode);

        }
    }

    class HtmlStack
    {

        List<DomElement> nodes = new List<DomElement>();
        int count;

        public HtmlStack()
        {
        }
        public void Push(DomElement node)
        {
            count++;
            this.nodes.Add(node);
        }
        public DomElement Pop()
        {
            DomElement node = this.nodes[count - 1];
            this.nodes.RemoveAt(count - 1);
            count--;
            return node;
        }
        public DomElement Peek()
        {
            if (count > 1)
            {
                return nodes[count - 1];
            }
            else
            {
                return null;
            }
        }
        public int Count
        {
            get
            {
                return this.count;
            }
        }
        public void Clear()
        {
            this.nodes.Clear();
            this.count = 0;
        }
    }

}