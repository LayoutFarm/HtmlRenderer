//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
 
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{

#if DEBUG
    partial class MyPlatformWindowBridge : IdbugOutputWindow
    {

        public event EventHandler dbug_VisualRootDrawMsg;
        public event EventHandler dbug_VisualRootHitChainMsg;

        List<dbugLayoutMsg> dbugrootDocDebugMsgs = new List<dbugLayoutMsg>();
        List<dbugLayoutMsg> dbugrootDocHitChainMsgs = new List<dbugLayoutMsg>();


        public List<dbugLayoutMsg> dbug_rootDocDebugMsgs
        {
            get
            {
                return this.dbugrootDocDebugMsgs;
            }
        }
        public List<dbugLayoutMsg> dbug_rootDocHitChainMsgs
        {
            get
            {
                return this.dbugrootDocHitChainMsgs;
            }
        }
        System.Drawing.Graphics dbugCreateGraphics()
        {
            throw new NotSupportedException();
            //return this.windowControl.CreateGraphics();
        }
        public void dbug_HighlightMeNow(Rectangle rect)
        {

            using (System.Drawing.Pen mpen = new System.Drawing.Pen(System.Drawing.Brushes.White, 2))
            using (System.Drawing.Graphics g = this.dbugCreateGraphics())
            {

                System.Drawing.Rectangle r = rect.ToRect();
                g.DrawRectangle(mpen, r);
                g.DrawLine(mpen, new System.Drawing.Point(r.X, r.Y), new System.Drawing.Point(r.Right, r.Bottom));
                g.DrawLine(mpen, new System.Drawing.Point(r.X, r.Bottom), new System.Drawing.Point(r.Right, r.Y));
            }

        }
        public void dbug_InvokeVisualRootDrawMsg()
        {
            if (dbug_VisualRootDrawMsg != null)
            {
                dbug_VisualRootDrawMsg(this, EventArgs.Empty);
            }
        }
        public void dbug_InvokeHitChainMsg()
        {
            if (dbug_VisualRootHitChainMsg != null)
            {
                dbug_VisualRootHitChainMsg(this, EventArgs.Empty);
            }
        }
        public void dbug_BeginLayoutTraceSession(string beginMsg)
        {
            this.topwin.dbugVisualRoot.dbug_BeginLayoutTraceSession(beginMsg);
        }
        public void dbug_DisableAllDebugInfo()
        {
            this.topwin.dbugVisualRoot.dbug_DisableAllDebugInfo();
        }
        public void dbug_EnableAllDebugInfo()
        {
            this.topwin.dbugVisualRoot.dbug_EnableAllDebugInfo();
        }
        public void dbug_ReArrangeWithBreakOnSelectedNode()
        {

            vinv_dbugBreakOnSelectedVisuallElement = true;
            this.topwin.TopDownReArrangeContentIfNeed();

        }
        public bool vinv_dbugBreakOnSelectedVisuallElement
        {
            get;
            set;
        }
    }
#endif
}