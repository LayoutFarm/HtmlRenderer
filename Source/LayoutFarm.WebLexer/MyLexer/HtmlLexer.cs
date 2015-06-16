//BSD  2015,2014 ,WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.WebLexer;

namespace LayoutFarm.WebDom.Parser
{
    public enum HtmlLexerEvent
    {
        VisitOpenAngle,        //  <a
        VisitOpenSlashAngle,   //  </a
        VisitCloseAngle,       //  a>
        VisitCloseSlashAngle,  //  />        
        VisitAttrAssign,      //=

        VisitOpenAngleExclimation, //<! eg. document node <!doctype

        OpenComment,           //  <!--
        CloseComment,          //  -->

        OpenProcessInstruction,  //  <?
        CloseProcessInstruction, //  ?>

        NodeNameOrAttribute,
        NodeNamePrefix,
        NodeNameLocal,
        Attribute,
        AttributeNameLocal,
        AttributeNamePrefix, 
        AttributeValueAsLiteralString,

        SwitchToContentPart,
        FromContentPart,
        CommentContent
    }

    enum HtmlLexState
    {
        Init,
        AfterOpenAngle


    }

    public delegate void HtmlLexerEventHandler(HtmlLexerEvent lexEvent, int startIndex, int len);

    public sealed partial class HtmlLexer
    {

        int _readIndex = 0;
        int _lastFlushAt = 0;
        int _appendCount = 0;
        int _firstAppendAt = -1;
        LayoutFarm.WebLexer.TextSnapshot textSnapshot;
        public event HtmlLexerEventHandler LexStateChanged;
        public HtmlLexer()
        {
        }

        //reset
        public void BeginLex()
        {
#if DEBUG
            dbug_currentLineNumber = 0;
            dbug_currentLineCharIndex = -1;
#endif
            _readIndex = 0;
            _lastFlushAt = 0;
            _appendCount = 0;
            _firstAppendAt = -1;

        }
        public void EndLex()
        {

        }


        public void Analyze(TextSnapshot textSnapshot)
        {

#if DEBUG
            dbug_OnStartAnalyze();
#endif

            this.textSnapshot = textSnapshot;
            char[] sourceBuffer = TextSnapshot.UnsafeGetInternalBuffer(textSnapshot);

            int lim = sourceBuffer.Length;

            char strEscapeChar = '"';
            int currentState = 0;
            //-----------------------------

            for (int i = 0; i < lim; i++)
            {


                char c = sourceBuffer[i];
#if DEBUG
                dbug_currentLineCharIndex++;
                dbugReportChar(c, currentState);
#endif
                switch (currentState)
                {
                    default:
                        {
                            //???

                        } break;
                    case 0:  //from content mode 
                        {
                            if (c == '<')
                            {
                                //flush existing content 
                                //switch to content  tag mode 
                                FlushExisingBuffer(i, HtmlLexerEvent.FromContentPart);
                                currentState = 1;
                                //not need whitespace in this mode 
                            }
                            else
                            {
                                //in content mode
                                AppendBuffer(c, i);
                            }
                        } break;
                    case 1:
                        {
                            //after open angle
                            switch (c)
                            {
                                case '!':
                                    {
                                        currentState = 11;
                                    } break;
                                case '?':
                                    {
                                        //process instruction
                                        currentState = 8;
                                    } break;
                                case ':':
                                    {
                                        //shold not occurs
                                        currentState = 4;
                                    } break;
                                case '/':
                                    {
                                        //close tag 
                                        LexStateChanged(HtmlLexerEvent.VisitOpenSlashAngle, i, 1);
                                        currentState = 5;//collect node name 
                                    } break;
                                default:
                                    {
                                        currentState = 5;
                                        //clear prev buffer 
                                        //then start collect node name

                                        AppendBuffer(c, i);
                                    } break;
                            }
                        } break;
                    case 2:
                        {
                            //inside comment node
                            if (c == '-')
                            {
                                if (i < lim - 2)
                                {
                                    if (sourceBuffer[i + 1] == '-' && sourceBuffer[i + 2] == '>')
                                    {
                                        //end comment node  
                                        FlushExisingBuffer(i, HtmlLexerEvent.CommentContent);
                                        i += 2;
                                        currentState = 0;
                                        continue;
                                    }
                                }
                            }
                            //skip all comment  content ? 
                            AppendBuffer(c, i);

                        } break;
                    case 5:
                        {
                            //inside open angle
                            //name collecting
                            //terminate with... 
                            switch (c)
                            {
                                case '/':
                                    {
                                        currentState = 7;
                                    } break;
                                case '>':
                                    {

                                        FlushExisingBuffer(i, HtmlLexerEvent.NodeNameOrAttribute);
                                        LexStateChanged(HtmlLexerEvent.VisitCloseAngle, i, 1);
                                        //flush 
                                        currentState = 0;
                                        //goto content mode
                                    } break;
                                case ':':
                                    {
                                        //flush node name
                                        FlushExisingBuffer(i, HtmlLexerEvent.NodeNameOrAttribute);
                                        //start new node name

                                    } break;
                                case ' ':
                                    {
                                        //flush node name
                                        FlushExisingBuffer(i, HtmlLexerEvent.NodeNameOrAttribute);
                                    } break;
                                case '=':
                                    {
                                        //flush name
                                        FlushExisingBuffer(i, HtmlLexerEvent.Attribute);
                                        LexStateChanged(HtmlLexerEvent.VisitAttrAssign, i, 1);
                                        //start collect value of attr 
                                    } break;
                                case '"':
                                    {
                                        //start string escap with " 
                                        currentState = 6;
                                        strEscapeChar = '"';
                                    } break;
                                case '\'':
                                    {
                                        //start string escap with ' 
                                        currentState = 6;
                                        strEscapeChar = '\'';

                                    } break;
                                default:
                                    {
                                        //else collect 
                                        //flush nodename

                                        if (char.IsWhiteSpace(c))
                                        {
                                            FlushExisingBuffer(i, HtmlLexerEvent.NodeNameOrAttribute);

                                        }
                                        else
                                        {
                                            AppendBuffer(c, i);
                                        }
                                    } break;
                            }
                        } break;
                    case 6:
                        {
                            //collect string 
                            if (c == strEscapeChar)
                            {
                                //stop string escape
                                //flush 
                                FlushExisingBuffer(i, HtmlLexerEvent.AttributeValueAsLiteralString);

                                currentState = 5;
                            }
                            else
                            {
                                AppendBuffer(c, i);

                            }
                        } break;
                    case 7:
                        {
                            //after /   //must be >
                            if (c == '>')
                            {
                                FlushExisingBuffer(i, HtmlLexerEvent.NodeNameOrAttribute);
                                LexStateChanged(HtmlLexerEvent.VisitCloseSlashAngle, i, 1);
                                currentState = 0;
                            }
                            else
                            {
                                //error ?
                            }
                        } break;
                    case 10:
                        {
                            //unknown tag
                            //exit from this tag when found >
                            if (c == '>')
                            {
                                currentState = 0;
                            }
                        } break;
                    case 11:
                        {
                            //open_angle, exlcimation
                            switch (c)
                            {
                                case '-':
                                    {
                                        //looking for next char
                                        if (i < lim)
                                        {
                                            if (sourceBuffer[i + 1] == '-')
                                            {
                                                currentState = 2;
                                                continue;
                                            }
                                            else
                                            {
                                                //unknown tag?
                                                currentState = 10;
                                            }
                                        }

                                    } break;
                                case '[':
                                    {
                                        // <![
                                        //
                                        currentState = 10;//not implement,just skip
                                    } break;
                                default:
                                    {
                                        //doc type?
                                        if (char.IsLetter(sourceBuffer[i + 1]))
                                        {

                                            LexStateChanged(HtmlLexerEvent.VisitOpenAngleExclimation, i, 2);
                                            AppendBuffer(c, i);
                                            currentState = 5;
                                        }
                                        else
                                        {
                                            currentState = 10;//not implement, just skip
                                        }
                                    } break;
                            }
                        } break;

                }
            }

#if DEBUG
            dbug_OnFinishAnalyze();
#endif
        }



        void FlushExisingBuffer(int lastFlushAtIndex, HtmlLexerEvent lexerEvent)
        {

            //raise lexer event
            if (_appendCount > 0)
            {
#if DEBUG
                //Console.WriteLine(lexerEvent.ToString() + " : " +
                //    new string(this.textSnapshot.Copy(this._firstAppendAt, (this._readIndex - this._firstAppendAt) + 1)));
#endif

                LexStateChanged(lexerEvent, this._firstAppendAt, (this._readIndex - this._firstAppendAt) + 1);

            }

            this._lastFlushAt = lastFlushAtIndex;
            this._appendCount = 0;
        }

        void AppendBuffer(char c, int index)
        {
            if (_appendCount == 0)
            {
                this._firstAppendAt = index;
                this._appendCount++;
            }

            this._readIndex = index;
        }




    }
}
