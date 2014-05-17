//BSD  2014 ,WinterCore

using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlRenderer.WebDom.Parser
{
    enum HtmlLexerEvent
    {
        VisitOpenAngle,        //  <a
        VisitOpenSlashAngle,   //  </a
        VisitCloseAngle,       //  a>
        VisitCloseSlashAngle,  //  />        
        VisitAttrAssign,      //=

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
        AttributeValue,
        AttributeValueAsLiteralString,

        SwitchToContentPart,
        FromContentPart
    }

    delegate void HtmlLexerEventHandler(HtmlLexerEvent lexEvent, int startIndex, int len);

    sealed class HtmlLexer
    {

        int _readIndex = 0;
        int _lastFlushAt = 0;
        int _appendCount = 0;
        int _firstAppendAt = -1;
        TextSnapshot textSnapshot;

#if DEBUG
        dbugLexerReport dbug_LexerReport;
        int dbug_currentLineCharIndex = -1;
        int dbug_currentLineNumber = 0;
#endif

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

#if  DEBUG
        void dbug_OnStartAnalyze()
        {
        }
        void dbug_OnFinishAnalyze()
        {
        }
        public void dbugStartRecord(string filename)
        {
            dbug_LexerReport = new dbugLexerReport();
            dbug_LexerReport.Start(filename);
        }

        public void dbugEndRecord()
        {
            dbug_LexerReport.End();
            dbug_LexerReport = null;
        }
#endif


#if DEBUG
        void dbugReportChar(char c, int currentState)
        {
            if (dbug_LexerReport != null)
            {

                dbug_LexerReport.WriteLine("[" + dbug_currentLineNumber + " ," +
                    dbug_currentLineCharIndex + "] state=" + currentState + " char=" + c);
            }
        }
#endif

        void FlushExisingBuffer(int lastFlushAtIndex, HtmlLexerEvent lexerEvent)
        {

            //raise lexer event
            if (_appendCount > 0)
            {
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
                                        //comment mode
                                        currentState = 2;
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
                                        //currentBuffer.Append(c);
                                        AppendBuffer(c, i);
                                    } break;
                            }
                        } break;
                    case 2:
                        {
                            if (c == '-')
                            {
                                currentState = 3;
                            }
                            else
                            {
                                //error
                                //
                            }
                        } break;
                    case 3:
                        {
                            //second - after <!-
                            if (c == '-')
                            {
                                currentState = 3;
                            }
                            else
                            {
                                currentState = 10;//inside comment node
                            }

                        } break;
                    case 5:
                        {
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
                                        //flush nodename
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
                                        AppendBuffer(c, i);
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
                            //inside commmen node
                            if (c == '-')
                            {
                                currentState = 11;
                            }
                            else
                            {
                                AppendBuffer(c, i);
                            }
                        } break;
                    case 11:
                        {
                            if (c == '-')
                            {

                            }
                            else
                            {

                            }
                        } break;

                }
            }

#if DEBUG
            dbug_OnFinishAnalyze();
#endif
        }
    }
}
