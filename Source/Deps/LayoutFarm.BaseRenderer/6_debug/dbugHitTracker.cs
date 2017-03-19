
//Apache2, 2014-2017, WinterDev

using System.IO;
namespace LayoutFarm
{
#if DEBUG
    public class dbugHitTestTracker
    {
        StreamWriter strmWriter;
        bool play = false;
        public int seriesCounter = 0;
        bool enable = false;
        public dbugHitTestTracker(StreamWriter strmWriter)
        {
            if (enable)
            {
                this.strmWriter = strmWriter;
                //int fileCount = Directory.GetFiles(dbugCoreConst.dbugRootFolder + "\\ui_hittest").Length;
                //FileStream fs = new FileStream(dbugCoreConst.dbugRootFolder + "\\ui_hittest\\_hitTracker" + (fileCount + 1) + ".txt", FileMode.Create);
                //strmWriter = new StreamWriter(fs);
            }
        }

        public void WriteTrackNode(int level, string info)
        {
            if (enable && play)
            {
                strmWriter.WriteLine(new string(' ', level * 4) + info);
                strmWriter.Flush();
            }
        }
        public void Write(string info)
        {
            if (enable && play)
            {
                strmWriter.WriteLine(info);
                strmWriter.Flush();
            }
        }
        public bool Play
        {
            get
            {
                return play;
            }
            set
            {
                play = value;
            }
        }
        public void Close()
        {
        }
    }
#endif
}
