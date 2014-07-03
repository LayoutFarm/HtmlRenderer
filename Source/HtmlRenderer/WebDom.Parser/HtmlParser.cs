//BSD  2014 ,WinterFarm

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HtmlRenderer.WebDom.Parser
{
    public class HtmlParser
    {
        HtmlDocument _resultHtmlDoc;
        HtmlStack htmlNodeStack = new HtmlStack();

        HtmlElement curHtmlNode = null;
        HtmlAttribute curAttr = null;
        HtmlTextNode curTextNode = null;
        int parseState = 0;
        TextSnapshot textSnapshot;
        HtmlLexer lexer;

        public HtmlParser()
        {
            lexer = new HtmlLexer();
            lexer.LexStateChanged += LexStateChanaged;
        }

        public HtmlDocument ResultHtmlDoc
        {
            get
            {
                return this._resultHtmlDoc;
            }
        }


        void LexStateChanaged(HtmlLexerEvent lexEvent, int startIndex, int len)
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
                            curTextNode = _resultHtmlDoc.CreateTextNode(HtmlUtils.DecodeHtml(this.textSnapshot, startIndex, len));

                            if (curHtmlNode != null)
                            {
                                curHtmlNode.AddChild(curTextNode);
                            }
                        }
                        else
                        {
                            curTextNode.AppendTextContent(HtmlUtils.DecodeHtml(this.textSnapshot, startIndex, len));

                        }
                    } break;
                case HtmlLexerEvent.AttributeValueAsLiteralString:
                    {

                        curAttr.Value = textSnapshot.Substring(startIndex, len);
                    } break;
                case HtmlLexerEvent.AttributeValue:
                    {
                    } break;
                case HtmlLexerEvent.Attribute:
                    {
                        string nodename = textSnapshot.Substring(startIndex, len);
                        curAttr = this._resultHtmlDoc.CreateAttribute(null, nodename);
                        curHtmlNode.AddAttribute(curAttr);
                    } break;
                case HtmlLexerEvent.NodeNameOrAttribute:
                    {
                        string nodename = textSnapshot.Substring(startIndex, len);
                        switch (parseState)
                        {
                            case 0:
                                {

                                    HtmlElement elem = this._resultHtmlDoc.CreateElement(null, nodename);
                                    if (curHtmlNode != null)
                                    {
                                        curHtmlNode.AddChild(elem);
                                        htmlNodeStack.Push(curHtmlNode);
                                    }
                                    curHtmlNode = elem;
                                    parseState = 1;//attribute
                                    curTextNode = null;
                                    curAttr = null;
                                } break;

                            case 2:
                                {
                                    //node name after open slash
                                    if (curHtmlNode.LocalName == nodename)
                                    {
                                        if (htmlNodeStack.Count > 0)
                                        {
                                            curTextNode = null;
                                            curAttr = null;
                                            curHtmlNode = htmlNodeStack.Pop();
                                        }
                                        parseState = 3;
                                    }
                                    else
                                    {
                                        //if not equal then check if current node need close tag or not
                                        if (HtmlUtils.IsSingleTag(curHtmlNode.nodeLocalNameIndex))
                                        {
                                            if (htmlNodeStack.Count > 0)
                                            {

                                                curHtmlNode = htmlNodeStack.Pop();
                                                curAttr = null;
                                                curTextNode = null;
                                            }
                                            if (curHtmlNode.LocalName == nodename)
                                            {
                                                if (htmlNodeStack.Count > 0)
                                                {
                                                    curTextNode = null;
                                                    curAttr = null;
                                                    curHtmlNode = htmlNodeStack.Pop();
                                                }
                                                parseState = 3;
                                            }
                                            else
                                            {
                                                throw new NotSupportedException();
                                            }

                                        }
                                        else
                                        {
                                            throw new NotSupportedException();
                                        }


                                    }

                                } break;
                            case 4:
                                {
                                    //attribute value as id
                                    if (curAttr != null)
                                    {
                                        curAttr.Value = nodename;
                                        curAttr = null;
                                        parseState = 0;
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

                        if (htmlNodeStack.Count > 0)
                        {
                            curTextNode = null;
                            curAttr = null;
                            curHtmlNode = htmlNodeStack.Pop();
                        }
                        parseState = 0;

                    } break;
                default:
                    {
                        //1. visit open angle
                    } break;

            }
        }

        void ResetParser()
        {

            this._resultHtmlDoc = null;
            this.htmlNodeStack.Clear();
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
        public void Parse(TextSnapshot textSnapshot)
        {
            ResetParser();

            this.textSnapshot = textSnapshot;
            //1. lex 
            lexer.BeginLex();
            //2. mini parser   
            HtmlDocument htmldoc = new HtmlDocument();
            this.curHtmlNode = htmldoc.RootNode;
            this._resultHtmlDoc = htmldoc;
            lexer.Analyze(textSnapshot);
            lexer.EndLex();

        }


    }
    class HtmlStack
    {
        List<HtmlElement> nodes = new List<HtmlElement>();
        int count;
        public HtmlStack()
        {
        }
        public void Push(HtmlElement node)
        {
            count++;
            this.nodes.Add(node);
        }
        public HtmlElement Pop()
        {
            HtmlElement node = this.nodes[count - 1];
            this.nodes.RemoveAt(count - 1);
            count--;
            return node;
        }
        public HtmlElement Peek()
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