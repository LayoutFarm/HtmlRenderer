//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using LayoutFarm.Drawing;
using System.Text;

namespace LayoutFarm
{


    interface ISurfaceViewportControl
    {

        IntPtr Handle { get; }
        void PaintMe();
        void viewport_HScrollChanged(object sender, UIScrollEventArgs e);
        void viewport_HScrollRequest(object sender, ScrollSurfaceRequestEventArgs e);
        void viewport_VScrollChanged(object sender, UIScrollEventArgs e);
        void viewport_VScrollRequest(object sender, ScrollSurfaceRequestEventArgs e);

#if DEBUG
        List<dbugLayoutMsg> dbug_rootDocDebugMsgs { get; }
        void dbug_InvokeVisualRootDrawMsg();
        List<dbugLayoutMsg> dbug_rootDocHitChainMsgs { get; }
        void dbug_InvokeHitChainMsg();
        void dbug_HighlightMeNow(Rectangle r);
#endif
    }
}