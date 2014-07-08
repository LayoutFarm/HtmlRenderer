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
        public static int dbugDrawStringCount
        {
            get { return _dbugDrawStringCount; }
            set
            {
                _dbugDrawStringCount = value;
            }
        }
        public static long GCAndSnap(dbugCounterAction codeRgn)
        {
            GC.Collect();
            var newWatch = System.Diagnostics.Stopwatch.StartNew();
            codeRgn();
            newWatch.Stop();
            return newWatch.ElapsedMilliseconds;
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