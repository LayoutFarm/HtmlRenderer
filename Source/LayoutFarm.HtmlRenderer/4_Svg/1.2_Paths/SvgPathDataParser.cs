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
            List<float> numbers = new List<float>();

            int j = pathDataBuffer.Length;

            int currentState = 0;
            for (int i = 0; i < j; )
            {
                //lex and parse
                char c = pathDataBuffer[i];
                switch (currentState)
                {
                    case 0:
                        {
                            //init state
                            switch (c)
                            {
                                case 'M':
                                case 'm':
                                    {
                                        //move to 
                                        ParseNumberList(pathDataBuffer, i + 1, out i, numbers);
                                        if (numbers.Count == 2)
                                        {
                                            SvgPathSegMoveTo moveTo = new SvgPathSegMoveTo(
                                                 numbers[0],
                                                 numbers[1]);
                                            moveTo.IsRelative = c == 'm';
                                            pathSegments.Add(moveTo);
                                        }
                                        else
                                        {  //error 

                                        }
                                        numbers.Clear();//reset
                                    } break;
                                case 'L':
                                case 'l':
                                    {
                                        //line to
                                        ParseNumberList(pathDataBuffer, i + 1, out i, numbers);
                                        if (numbers.Count == 2)
                                        {
                                            SvgPathSegLineTo lineTo = new SvgPathSegLineTo(
                                                 numbers[0],
                                                 numbers[1]);
                                            lineTo.IsRelative = c == 'l';
                                            pathSegments.Add(lineTo);
                                        }
                                        else
                                        {  //error 

                                        }
                                        numbers.Clear();//reset
                                    } break;
                                case 'H':
                                case 'h':
                                    {
                                        ParseNumberList(pathDataBuffer, i + 1, out i, numbers);
                                        if (numbers.Count == 1)
                                        {
                                            SvgPathSegLineToHorizontal h = new SvgPathSegLineToHorizontal(
                                                numbers[0]);
                                            h.IsRelative = c == 'h';
                                            pathSegments.Add(h);
                                        }
                                        else
                                        {  //error 

                                        }
                                        numbers.Clear();//reset
                                    }break;
                                case 'V':
                                case 'v':
                                    {
                                        ParseNumberList(pathDataBuffer, i + 1, out i, numbers);
                                        if (numbers.Count == 1)
                                        {
                                            SvgPathSegLineToVertical v = new SvgPathSegLineToVertical(
                                                numbers[0]);
                                            v.IsRelative = c == 'v';
                                            pathSegments.Add(v);
                                        }
                                        else
                                        {  //error 

                                        }
                                        numbers.Clear();//reset
                                    } break;
                                case 'Z':
                                case 'z':
                                    {
                                        pathSegments.Add(new SvgPathSegClosePath());
                                        i++;
                                    } break;
                                case 'A':
                                case 'a':
                                    {

                                        ParseNumberList(pathDataBuffer, i + 1, out i, numbers);
                                        if (numbers.Count == 7)
                                        {
                                            SvgPathSegArc arc = new SvgPathSegArc(
                                                numbers[0], numbers[1],
                                                numbers[2], (int)numbers[3], (int)numbers[4],
                                                numbers[5], numbers[6]);
                                            arc.IsRelative = c == 'a';
                                            pathSegments.Add(arc);
                                        }
                                        numbers.Clear();
                                    } break;
                                case 'Q':
                                case 'q':
                                    {
                                        ParseNumberList(pathDataBuffer, i + 1, out i, numbers);
                                        if (numbers.Count == 4)
                                        {
                                            SvgPathSegCurveToQuadratic quadCurve = new SvgPathSegCurveToQuadratic(
                                                numbers[0], numbers[1],
                                                numbers[2], numbers[3]);
                                            quadCurve.IsRelative = c == 'q';
                                            pathSegments.Add(quadCurve);
                                        }
                                        numbers.Clear();
                                    } break;
                                case 'S':
                                case 's':
                                    {
                                        ParseNumberList(pathDataBuffer, i + 1, out i, numbers);
                                        if (numbers.Count == 4)
                                        {
                                            SvgPathSegCurveToCubicSmooth scubicCurve = new SvgPathSegCurveToCubicSmooth(
                                                numbers[0], numbers[1],
                                                numbers[2], numbers[3]); 

                                            scubicCurve.IsRelative = c == 's';
                                            pathSegments.Add(scubicCurve);
                                        }
                                        numbers.Clear();
                                    } break;
                                case 'T':
                                case 't':
                                    {
                                        ParseNumberList(pathDataBuffer, i + 1, out i, numbers);
                                        if (numbers.Count == 4)
                                        {
                                            SvgPathSegCurveToQuadraticSmooth squadCurve = new SvgPathSegCurveToQuadraticSmooth(
                                                numbers[0], numbers[1]);
                                            squadCurve.IsRelative = c == 't';
                                            pathSegments.Add(squadCurve);
                                        }
                                        numbers.Clear();
                                    } break;
                                default:
                                    {
                                    } break;
                            }
                        } break;
                    default:
                        {
                        } break;
                }
            }
            return pathSegments;
        }
        static void ParseNumberList(char[] pathDataBuffer, int startIndex, out int latestIndex, List<float> numbers)
        {
            latestIndex = startIndex;
            //parse coordinate
            int j = pathDataBuffer.Length;
            int currentState = 0;
            int startCollectNumber = -1;

            for (; latestIndex < j; ++latestIndex)
            {
                //lex and parse
                char c = pathDataBuffer[latestIndex];

                if (c == ',' || char.IsWhiteSpace(c))
                {
                    if (startCollectNumber >= 0)
                    {
                        //collect latest number
                        string str = new string(pathDataBuffer, startCollectNumber, latestIndex - startCollectNumber);
                        float number;
                        float.TryParse(str, out number);
                        numbers.Add(number);
                        startCollectNumber = -1;
                        currentState = 0;//reset
                    }
                    continue;
                }

                switch (currentState)
                {
                    case 0:
                        {

                            //--------------------------
                            if (c == '-')
                            {
                                currentState = 1;//negative
                                startCollectNumber = latestIndex;
                            }
                            else if (char.IsNumber(c))
                            {
                                currentState = 2;//number found
                                startCollectNumber = latestIndex;
                            }
                            else
                            {
                                if (startCollectNumber >= 0)
                                {
                                    //collect latest number
                                    string str = new string(pathDataBuffer, startCollectNumber, latestIndex - startCollectNumber);
                                    float number;
                                    float.TryParse(str, out number);
                                    numbers.Add(number);
                                    startCollectNumber = -1;
                                    currentState = 0;//reset
                                }
                                return;
                            }
                        } break;
                    case 1:
                        {
                            //after negative expect first number
                            if (char.IsNumber(c))
                            {
                                //ok collect next
                            }
                            else
                            {
                                //must found number
                                if (startCollectNumber >= 0)
                                {
                                    //collect latest number
                                    string str = new string(pathDataBuffer, startCollectNumber, latestIndex - startCollectNumber);
                                    float number;
                                    float.TryParse(str, out number);
                                    numbers.Add(number);
                                    startCollectNumber = -1;
                                    currentState = 0;//reset
                                }
                                return;
                            }
                        } break;
                    case 2:
                        {
                            //number state
                            if (char.IsNumber(c))
                            {
                                //ok collect next
                            }
                            else if (c == '.')
                            {
                                //collect next
                                currentState = 3;
                            }
                            else
                            {
                                //must found number
                                if (startCollectNumber >= 0)
                                {
                                    //collect latest number
                                    string str = new string(pathDataBuffer, startCollectNumber, latestIndex - startCollectNumber);
                                    float number;
                                    float.TryParse(str, out number);
                                    numbers.Add(number);
                                    startCollectNumber = -1;
                                    currentState = 0;//reset
                                }
                                return;
                            }
                        } break;
                    case 3:
                        {
                            //after .
                            if (char.IsNumber(c))
                            {
                                //ok collect next
                            }
                            else
                            {
                                if (startCollectNumber >= 0)
                                {
                                    //collect latest number
                                    string str = new string(pathDataBuffer, startCollectNumber, latestIndex - startCollectNumber);
                                    float number;
                                    float.TryParse(str, out number);
                                    numbers.Add(number);
                                    startCollectNumber = -1;
                                    currentState = 0;//reset
                                }
                                return;
                                //break hear
                            }
                        } break;
                }
            }
            //-------------------
        }
    }
}