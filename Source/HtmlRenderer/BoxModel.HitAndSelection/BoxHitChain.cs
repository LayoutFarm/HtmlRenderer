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
    public class HitInfo
    {
        public readonly HitObjectKind hitObjectKind;
        public readonly object hitObject;

        public readonly int localX;
        public readonly int localY;

        public readonly float globalX;
        public readonly float globalY;

        public HitInfo(CssBox box, int x, int y, float globalX, float globalY)
        {
            this.hitObject = box;
            this.hitObjectKind = HitObjectKind.CssBox;

            this.localX = x;
            this.localY = y;

            this.globalX = globalX;
            this.globalY = globalY; 

        }
        internal HitInfo(CssLineBox lineBox, int x, int y, float globalX, float globalY)
        {

            this.hitObject = lineBox;
            this.hitObjectKind = HitObjectKind.LineBox;

            this.localX = x;
            this.localY = y;

            this.globalX = globalX;
            this.globalY = globalY;
        }
        public HitInfo(CssRun run, int x, int y, float globalX, float globalY)
        {

            this.hitObject = run;
            this.hitObjectKind = HitObjectKind.Run;

            this.localX = x;
            this.localY = y;


            this.globalX = globalX;
            this.globalY = globalY;
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

        float globalOffsetX;
        float globalOffsetY;

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
        internal void PushContextBox(CssBox box)
        {
            globalOffsetX += box.LocalX;
            globalOffsetY += box.LocalY;
        }
        internal void PopContextBox(CssBox box)
        {
            globalOffsetX -= box.LocalX;
            globalOffsetY -= box.LocalY;
        }
        internal void AddHit(CssBox box, int x, int y, float globalX, float globalY)
        {
            //position x,y relate with (0,0) of its box
            hitInfoList.Add(new HitInfo(box, x, y, globalX, globalY));
        }
        internal void AddHit(CssLineBox lineBox, int x, int y, float globalX, float globalY)
        {
            //position x,y relate with (0,0) of its linebox
            hitInfoList.Add(new HitInfo(lineBox, x, y, globalX, globalY));
        }
        internal void AddHit(CssRun run, int x, int y, float globalX, float globalY)
        {
            //position x,y relate with (0,0) of its run
            hitInfoList.Add(new HitInfo(run, x, y, globalX, globalY));
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
                return null;
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

        internal float GlobalOffsetX
        {
            get { return this.globalOffsetX; }
        }
        internal float GlobalOffsetY
        {
            get { return this.globalOffsetY; }
        }

        public static SelectionRange CreateSelectionRange(IGraphics g, BoxHitChain startChain, BoxHitChain endChain)
        {

            //find connection between start and end 
            HitInfo startHit = startChain.GetLastHit();
            HitInfo endHit = endChain.GetLastHit();

            //swap startpoint / end point if need
            BoxHitChain upper = startChain;
            BoxHitChain lower = endChain;

            if (endHit.globalY < startHit.globalY)
            {
                if (endHit.globalX < startHit.globalX)
                {
                    upper = endChain;
                    lower = startChain;
                }
            }
            else
            {
                if (endHit.globalX < startHit.globalX)
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