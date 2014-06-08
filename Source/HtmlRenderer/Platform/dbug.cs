using System;
namespace HtmlRenderer
{
#if DEBUG
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
    }
#endif

}