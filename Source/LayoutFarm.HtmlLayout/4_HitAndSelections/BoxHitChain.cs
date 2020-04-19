//MIT, 2014-present, WinterDev

using System.Collections.Generic;
namespace LayoutFarm.HtmlBoxes
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
        float _globalOffsetX;
        float _globalOffsetY;
        int _rootGlobalX;
        int _rootGlobalY;
        List<HitInfo> _hitInfoList = new List<HitInfo>();

        internal CssBoxHitChain()
        {
        }
        internal void SetRootGlobalPosition(int globalX, int globalY)
        {
#if DEBUG
            //System.Diagnostics.Debug.WriteLine("hitchain set root global pos:" + globalX + "," + globalY);
#endif
            _rootGlobalX = globalX;
            _rootGlobalY = globalY;
        }
        //
        public int RootGlobalX => _rootGlobalX;

        public int RootGlobalY => _rootGlobalY;

        public int Count => _hitInfoList.Count;
        //
        public void Clear()
        {
            _hitInfoList.Clear();
            _globalOffsetX = _globalOffsetY = _rootGlobalX = _rootGlobalY = 0;
        }
        //

        internal void PushContextBox(CssBox box)
        {
            //TODO: review here 
            _globalOffsetX += box.LocalX;
            _globalOffsetY += box.LocalY;
        }
        internal void PopContextBox(CssBox box)
        {
            _globalOffsetX -= box.LocalX;
            _globalOffsetY -= box.LocalY;
        }
        internal void AddHit(CssBox box, int x, int y)
        {
            //position x,y relate with (0,0) of its box
            _hitInfoList.Add(new HitInfo(box, x, y));
        }

        internal void AddHit(CssLineBox lineBox, int x, int y)
        {
            //position x,y relate with (0,0) of its linebox
            _hitInfoList.Add(new HitInfo(lineBox, x, y));
        }
        internal void AddHit(CssRun run, int x, int y)
        {
            //position x,y relate with (0,0) of its run

            _hitInfoList.Add(new HitInfo(run, x, y));
        }

        public HitInfo GetHitInfo(int index) => _hitInfoList[index];

        public HitInfo GetLastHit()
        {
            int j = _hitInfoList.Count;
            if (j == 0)
            {
                //empty
                return new HitInfo();
            }
            else
            {
                return _hitInfoList[j - 1];
            }
        }
        //
        internal float GlobalOffsetX => _globalOffsetX;

        internal float GlobalOffsetY => _globalOffsetY;


#if DEBUG

        public dbugEventPhase debugEventPhase { get; set; }
        public enum dbugEventPhase
        {
            Unknown,
            MouseDown,
            MouseMove,
            MouseUp,
        }

#endif
    }


    public enum HitObjectKind : byte
    {
        Unknown,
        CssBox,
        LineBox,
        Run
    }
}