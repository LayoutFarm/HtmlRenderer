//BSD  2014 ,WinterFarm

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HtmlRenderer.WebDom.Parser
{


#if DEBUG
    public class dbugLexerReport
    {
        FileStream fs;
        StreamWriter strmWriter;
        int lineCount = 0;
        public void Start(string fileName)
        {
            fs = new FileStream(fileName, FileMode.Create);
            strmWriter = new StreamWriter(fs);
            strmWriter.AutoFlush = true;
        }
        public void WriteLine(string info)
        {
            
            strmWriter.WriteLine(lineCount + " " + info);
            strmWriter.Flush();
            lineCount++;
        }
        public void End()
        {
            strmWriter.Close();
            fs.Close();
            fs.Dispose();
        }
    }
#endif

}