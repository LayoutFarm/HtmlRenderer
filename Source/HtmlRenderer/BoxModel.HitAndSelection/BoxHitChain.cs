//BSD 2014 ,WinterCore 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using HtmlRenderer.Dom;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;

namespace HtmlRenderer.Dom
{
    public struct HitInfo
    {
        public readonly HitObjectKind hitObjectKind;
        public readonly object hitObject;
        public readonly int x;
        public readonly int y;
        public HitInfo(HitObjectKind hitObjectKind, object hitObject, int x, int y)
        {
            this.hitObjectKind = hitObjectKind;
            this.hitObject = hitObject;
            this.x = x;
            this.y = y;
        }
#if DEBUG
        public override string ToString()
        {
            return hitObjectKind + ": " + hitObject.ToString();
        }
#endif
    }
    public class BoxHitChain
    {

        List<HitInfo> hitInfoList = new List<HitInfo>();

        public BoxHitChain()
        {

        }
        public void Clear()
        {
            this.hitInfoList.Clear();
        }
        public int Count
        {
            get
            {
                return this.hitInfoList.Count;
            }
        }
        internal void AddHit(CssBox box, int x, int y)
        {
            hitInfoList.Add(new HitInfo(HitObjectKind.CssBox, box, x, y));
        }
        internal void AddHit(CssLineBox lineBox, int x, int y)
        {
            hitInfoList.Add(new HitInfo(HitObjectKind.LineBox, lineBox, x, y));

        }
        internal void AddHit(CssRun run, int x, int y)
        {
            hitInfoList.Add(new HitInfo(HitObjectKind.Run, run, x, y));
        }
        public HitInfo GetHitInfo(int index)
        {
            return this.hitInfoList[index];
        }
        public HitInfo GetLastHit()
        {
            int j = hitInfoList.Count;
            if (j == 0)
            {
                //empty
                return new HitInfo();
            }
            else
            {
                return hitInfoList[j - 1];
            }
        }
        public object GetLastHitObject()
        {
            int j = hitInfoList.Count;
            if (j == 0)
            {
                return null;
            }
            else
            {
                return this.hitInfoList[j - 1].hitObject;
            }
        }



        public static SelectionRange CreateSelectionRange(IGraphics g, BoxHitChain startChain, BoxHitChain endChain)
        {

            //find connection between start and end 
            HitInfo startHit = startChain.GetLastHit();
            HitInfo endHit = endChain.GetLastHit();

            //swap startpoint / end point if need
            BoxHitChain upper = startChain;
            BoxHitChain lower = endChain;
            if (endHit.y < startHit.y)
            {
                if (endHit.x < startHit.x)
                {
                    upper = endChain;
                    lower = startChain;
                }
            }
            else
            {
                if (endHit.x < startHit.x)
                {
                    upper = endChain;
                    lower = startChain;
                }
            }
            return new SelectionRange(upper, lower, g);

        }


    }
    public enum HitObjectKind : byte
    {
        Unknown,
        CssBox,
        LineBox,
        Run
    }
   
    
}