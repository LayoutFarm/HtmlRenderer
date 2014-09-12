//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


using LayoutFarm.Presentation;



namespace LayoutFarm.Presentation
{
#if DEBUG
    partial class MyTopWindowRenderBox
    {
#if DEBUG
        public static bool dbugMark01;
        static void dbug_WriteInfo(dbugVisualLayoutTracer debugVisualLay, dbugVisitorMessage msg, RenderElement ve)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.WriteInfo(msg.text, ve);
            }
        }
        static void dbug_BeginNewContext(dbugVisualLayoutTracer debugVisualLay, dbugVisitorMessage msg, RenderElement ve)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.BeginNewContext(); debugVisualLay.WriteInfo(msg.text, ve);
            }
        }
        static void dbug_EndCurrentContext(dbugVisualLayoutTracer debugVisualLay, dbugVisitorMessage msg, RenderElement ve)
        {
            if (debugVisualLay != null)
            {

                debugVisualLay.WriteInfo(msg.text, ve);
                debugVisualLay.EndCurrentContext();
            }
        }
#endif

        public void dbug_DumpAllVisualElementProps(dbugLayoutMsgWriter writer)
        {
            this.dbug_DumpVisualProps(writer);
            writer.Add(new dbugLayoutMsg(this, "FINISH"));

        }

        void dbug_Init()
        {
            hitPointChain.dbugHitTracker = this.dbugVRoot.dbugHitTracker;
        }

        public static RenderElement dbugVE_HighlightMe;

        public override void dbugShowRenderPart(CanvasBase canvasPage, InternalRect updateArea)
        {

            RootGraphic visualroot = this.dbugVRoot;
            if (visualroot.dbug_ShowRootUpdateArea)
            {
                canvasPage.FillRectangle(Color.FromArgb(50, Color.Black),
                    new Rectangle(updateArea._left, updateArea._top,
                        updateArea.Width - 1, updateArea.Height - 1));
                canvasPage.FillRectangle(Color.White,
                    new Rectangle(updateArea._left, updateArea._top, 5, 5));
                canvasPage.DrawRectangle(Color.Yellow,
                    new Rectangle(updateArea._left, updateArea._top,
                        updateArea.Width - 1, updateArea.Height - 1));

                canvasPage.PushTextColor(Color.White);
                canvasPage.DrawText(visualroot.dbug_RootUpdateCounter.ToString().ToCharArray(), updateArea._left, updateArea._top);
                if (updateArea.Height > 25)
                {
                    canvasPage.DrawText(visualroot.dbug_RootUpdateCounter.ToString().ToCharArray(), updateArea._left, updateArea._top + (updateArea.Height - 20));
                }
                canvasPage.PopTextColor();
                visualroot.dbug_RootUpdateCounter++;
            }
        }

    }
#endif
}