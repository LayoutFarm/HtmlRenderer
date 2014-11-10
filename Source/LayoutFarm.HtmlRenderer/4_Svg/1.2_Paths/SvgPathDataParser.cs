//Apache2 2014,WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;

namespace Svg.Pathing
{

    public class SvgPathDataParser
    {
        public List<SvgPathSeg> Parse(char[] pathDataBuffer)
        {

            //parse pathdata to pathsegments
            List<SvgPathSeg> pathSegments = new List<SvgPathSeg>();

            int j = pathDataBuffer.Length;
            int currentState = 0;
            for (int i = 0; i < j; ++i)
            {
                //lex and parse
                char c = pathDataBuffer[i];
                switch (currentState)
                {
                    case 0:
                        {
                            //init state


                        } break;
                }
            }
            return pathSegments;
        }
    }
}