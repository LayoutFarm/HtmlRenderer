//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using Typography.TextBreak;

namespace YourImplementation
{
    public static class RelativePathBuilder
    {
        public static string SearchBackAndBuildFolderPath(string currentdir, string searchBackTo, string thenAppendWith)
        {
            //Helper function
            string[] dirs = currentdir.Split(new char[] { '\\', '/' });
            //count step back
            bool found = false;
            int stepBackCount = 0;
            for (int i = dirs.Length - 1; i >= 0; --i)
            {
                if (dirs[i] == searchBackTo)
                {
                    found = true;
                    break;
                }

                stepBackCount++;
            }

            if (found)
            {
                //create new path
                System.Text.StringBuilder stbuilder = new System.Text.StringBuilder();
                for (int n = 0; n < stepBackCount; ++n)
                {
                    stbuilder.Append("..\\");
                }
                //
                stbuilder.Append(thenAppendWith);
                return stbuilder.ToString();
            }
            else
            {
                //not found
                throw new NotSupportedException();//**
            }
        }
    }
}