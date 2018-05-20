//BSD, 2014-2017, WinterDev

using System.IO;
namespace LayoutFarm.WebLexer
{
#if DEBUG
    public class dbugLexerReport
    {
        StreamWriter strmWriter;
        int lineCount = 0;
        public void Start(StreamWriter strmWriter)
        {
            this.strmWriter = strmWriter;
            strmWriter.AutoFlush = true;
        }
        public void WriteLine(string info)
        {
            strmWriter.WriteLine(lineCount + " " + info);
            strmWriter.Flush();
            lineCount++;
        }
        public void Flush()
        {
            strmWriter.Flush();
        }
    }
#endif
}