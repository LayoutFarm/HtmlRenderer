// 2015,2014 ,Apache2, WinterDev 

namespace LayoutFarm.UI
{
    public struct ToNotifySizeChangedEvent
    {
        public int xdiff;
        public int ydiff;
        public IEventListener ui;
        public AffectedElementSideFlags affectedSideFlags;
    }
}