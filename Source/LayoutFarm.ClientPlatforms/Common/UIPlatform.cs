// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;


namespace LayoutFarm.UI
{
    public abstract class UIPlatform
    {
        public abstract UITimer CreateUITimer();
        public abstract void SetClipboardData(string textData);
        public abstract string GetClipboardData();
        public abstract void ClearClipboardData();
    }


}