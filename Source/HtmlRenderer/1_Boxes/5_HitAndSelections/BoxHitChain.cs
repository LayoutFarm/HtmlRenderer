﻿//BSD 2014 ,WinterDev 
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

        public readonly int localX;
        public readonly int localY;

        public HitInfo(CssBox box, int x, int y)
        {
            this.hitObject = box;
            this.hitObjectKind = HitObjectKind.CssBox;

            this.localX = x;
            this.localY = y;

        }
        internal HitInfo(CssLineBox lineBox, int x, int y)
        {

            this.hitObject = lineBox;
            this.hitObjectKind = HitObjectKind.LineBox;

            this.localX = x;
            this.localY = y;
        }
        public HitInfo(CssRun run, int x, int y)
        {

            this.hitObject = run;
            this.hitObjectKind = HitObjectKind.Run;

            this.localX = x;
            this.localY = y;
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

        int rootGlobalX;
        int rootGlobalY;

        List<HitInfo> hitInfoList = new List<HitInfo>();
        public BoxHitChain()
        {

        }
        public void SetRootGlobalPosition(int globalX, int globalY)
        {
            this.rootGlobalX = globalX;
            this.rootGlobalY = globalY;
        }
        public int RootGlobalX
        {
            get { return this.rootGlobalX; }
        }
        public int RootGlobalY
        {
            get { return this.rootGlobalY; }
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
        internal void AddHit(CssBox box, int x, int y)
        {
            //position x,y relate with (0,0) of its box
            hitInfoList.Add(new HitInfo(box, x, y));
        }
        internal void AddHit(CssLineBox lineBox, int x, int y)
        {
            //position x,y relate with (0,0) of its linebox
            hitInfoList.Add(new HitInfo(lineBox, x, y));
        }
        internal void AddHit(CssRun run, int x, int y)
        {
            //position x,y relate with (0,0) of its run
            hitInfoList.Add(new HitInfo(run, x, y));
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

        internal float GlobalOffsetX
        {
            get { return this.globalOffsetX; }
        }
        internal float GlobalOffsetY
        {
            get { return this.globalOffsetY; }
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