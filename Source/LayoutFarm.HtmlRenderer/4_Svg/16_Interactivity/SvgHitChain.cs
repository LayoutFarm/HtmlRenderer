//2014,2015, Apache2 WinterDev
//MS-PL, 
using System;
using PixelFarm.Drawing; 
using System.Collections.Generic;
using LayoutFarm.Css;

namespace LayoutFarm.Svg
{
    public class SvgHitChain
    {
        float rootGlobalX;
        float rootGlobalY;

        List<SvgHitInfo> svgList = new List<SvgHitInfo>();
        public SvgHitChain()
        {
        }
        public void AddHit(SvgElement svg, float x, float y)
        {
            svgList.Add(new SvgHitInfo(svg, x, y));
        }
        public int Count
        {
            get
            {
                return this.svgList.Count;
            }
        }
        public SvgHitInfo GetHitInfo(int index)
        {
            return this.svgList[index];
        }
        public SvgHitInfo GetLastHitInfo()
        {
            return this.svgList[svgList.Count - 1];
        }
        public void Clear()
        {
            this.rootGlobalX = this.rootGlobalY = 0;
            this.svgList.Clear();
        }
        public void SetRootGlobalPosition(float x, float y)
        {
            this.rootGlobalX = x;
            this.rootGlobalY = y;
        }
    }


    public struct SvgHitInfo
    {
        public readonly SvgElement svg;
        public readonly float x;
        public readonly float y;
        public SvgHitInfo(SvgElement svg, float x, float y)
        {
            this.svg = svg;
            this.x = x;
            this.y = y;
        }
    }
}