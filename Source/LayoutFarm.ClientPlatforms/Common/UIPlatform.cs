//Apache2, 2014-2016, WinterDev 

namespace LayoutFarm.UI
{
    public abstract class UIPlatform
    {
        public abstract UITimer CreateUITimer();
        public abstract void SetClipboardData(string textData);
        public abstract string GetClipboardData();
        public abstract void ClearClipboardData();
        public static UIPlatform CurrentUIPlatform { get; set; }
      
    }
}