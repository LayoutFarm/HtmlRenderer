//BSD  2015,2014 ,WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.WebLexer;

namespace LayoutFarm.WebDom.Parser
{
#if DEBUG
    partial class HtmlLexer
    {

        dbugLexerReport dbug_LexerReport;
        int dbug_currentLineCharIndex = -1;
        int dbug_currentLineNumber = 0;
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

        void dbugReportChar(char c, int currentState)
        {
            if (dbug_LexerReport != null)
            {

                dbug_LexerReport.WriteLine("[" + dbug_currentLineNumber + " ," +
                    dbug_currentLineCharIndex + "] state=" + currentState + " char=" + c);
            }
        }
    }
#endif
}