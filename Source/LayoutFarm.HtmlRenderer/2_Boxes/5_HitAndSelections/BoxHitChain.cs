//BSD 2014 ,WinterDev 
using System;
using System.Collections.Generic; 

namespace LayoutFarm.Boxes
{
    public struct HitInfo
    {

        public readonly int localX;
        public readonly int localY;
        public readonly HitObjectKind hitObjectKind;
        public readonly object hitObject;

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



    public class CssBoxHitChain
    {

        float globalOffsetX;
        float globalOffsetY;

        int rootGlobalX;
        int rootGlobalY;

        List<HitInfo> hitInfoList = new List<HitInfo>();
        public CssBoxHitChain()
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
            globalOffsetX = globalOffsetY = rootGlobalX = rootGlobalY = 0;
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
        public void AddHit(CssBox box, int x, int y)
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