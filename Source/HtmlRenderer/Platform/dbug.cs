//BSD 2014, WinterDev 
using System;
namespace HtmlRenderer
{
#if DEBUG
    public delegate void dbugCounterAction();
    public static class dbugCounter
    {
        public static bool dbugStartRecord = false;
        static int _dbugDrawStringCount;
        static int _dbugBoxPaint;
        static int _dbugLinePaint;
        static int _dbugRunPaint;
        public static int dbugBoxPaintCount
        {
            get { return _dbugBoxPaint; }
            set { _dbugBoxPaint = value; }
        }
        public static int dbugLinePaintCount
        {
            get { return _dbugLinePaint; }
            set { _dbugLinePaint = value; }
        }
        public static int dbugRunPaintCount
        {
            get { return _dbugRunPaint; }
            set { _dbugRunPaint = value; }
        }

        public static void ResetPaintCount()
        {
            _dbugBoxPaint = 0;
            _dbugLinePaint = 0;
            _dbugRunPaint = 0;
        }
        public static int dbugDrawStringCount
        {
            get { return _dbugDrawStringCount; }
            set
            {
                _dbugDrawStringCount = value;
            }
        }
<<<<<<< HEAD
        public static long Snap(System.Diagnostics.Stopwatch sw, dbugCounterAction codeRgn)
        {

            sw.Stop();
            sw.Reset();
            sw.Start();
            codeRgn();
            sw.Stop();
            return sw.ElapsedTicks;
=======
        public static long GCAndSnap(dbugCounterAction codeRgn)
        {
            //GC.Collect();
            var newWatch = System.Diagnostics.Stopwatch.StartNew();
            codeRgn();
            newWatch.Stop();
            return newWatch.ElapsedTicks;
>>>>>>> v1.7.2094.1
        }
        public static long GCAndSnapTicks(dbugCounterAction codeRgn)
        {
            GC.Collect();
            var newWatch = System.Diagnostics.Stopwatch.StartNew();
            codeRgn();
            newWatch.Stop();
            return newWatch.ElapsedTicks;
        }
    }

#endif

}