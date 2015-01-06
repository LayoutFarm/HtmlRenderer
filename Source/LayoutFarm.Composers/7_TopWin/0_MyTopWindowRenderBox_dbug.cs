//2014,2015 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using LayoutFarm;

namespace LayoutFarm.UI
{
#if DEBUG
    partial class MyTopWindowRenderBox
    {


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


        void dbug_DumpAllVisualElementProps(dbugLayoutMsgWriter writer)
        {
            this.dbug_DumpVisualProps(writer);
            writer.Add(new dbugLayoutMsg(this, "FINISH"));
        }
        public override void dbugShowRenderPart(Canvas canvasPage, Rect updateArea)
        {

            RootGraphic visualroot = this.dbugVRoot;
            if (visualroot.dbug_ShowRootUpdateArea)
            {
                canvasPage.FillRectangle(Color.FromArgb(50, Color.Black),
                     updateArea._left, updateArea._top,
                        updateArea.Width - 1, updateArea.Height - 1);
                canvasPage.FillRectangle(Color.White,
                     updateArea._left, updateArea._top, 5, 5);
                canvasPage.DrawRectangle(Color.Yellow,
                        updateArea._left, updateArea._top,
                        updateArea.Width - 1, updateArea.Height - 1);

                Color c_color = canvasPage.CurrentTextColor;
                canvasPage.CurrentTextColor = Color.White;
                canvasPage.DrawText(visualroot.dbug_RootUpdateCounter.ToString().ToCharArray(), updateArea._left, updateArea._top);
                if (updateArea.Height > 25)
                {
                    canvasPage.DrawText(visualroot.dbug_RootUpdateCounter.ToString().ToCharArray(), updateArea._left, updateArea._top + (updateArea.Height - 20));
                }
                canvasPage.CurrentTextColor = c_color;
                visualroot.dbug_RootUpdateCounter++;
            }
        }

    }
#endif
}