//BSD  2015,2014 ,WinterDev

using System.IO;

namespace LayoutFarm.WebLexer
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