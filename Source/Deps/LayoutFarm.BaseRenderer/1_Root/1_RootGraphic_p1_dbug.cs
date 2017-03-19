//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm
{
#if DEBUG
    partial class RootGraphic
    {
        public bool dbug_ShowRootUpdateArea;
        public bool dbug_ShowNativeScrollableElementUpdateArea;
        public bool dbug_ShowNativeScrollableElementDrawToThisPage;
        public bool dbug_ShowElementOutline;
        public bool dbug_ShowObjectIden;
        public bool dbug_ForceShowObjectIden;
        public bool dbug_RecordHitChain;
        public bool dbug_RecordDrawingChain;
        public bool dbugEnableGraphicInvalidateTrace;
        public bool dbug_StartTestMode;
        public bool dbugEnableLayoutProfiler;
        public System.IO.StreamWriter dbugLayoutProfilerWriter;
        public bool dbugEventIsDragging;
        public int dbug_RootUpdateCounter;
        public int dbug_drawLevel;
        public LinkedList<dbugLayoutMsg> dbug_rootHitChainMsg = new LinkedList<dbugLayoutMsg>();
        public LinkedList<dbugLayoutMsg> dbug_rootDrawingMsg = new LinkedList<dbugLayoutMsg>();
        public dbugHitTestTracker dbugHitTracker;
        public Stack<dbugVisualInvalidationTracer> dbugInvalidateTracerStack = new Stack<dbugVisualInvalidationTracer>();
        public dbugVisualInvalidationTracer dbugGraphicInvalidateTracer;
        public dbugVisualEvalScrollBarTrace dbugEvalScrollBarTracer;
        public void dbug_DisableAllDebugInfo()
        {
            dbug_ShowRootUpdateArea =
            dbug_ShowNativeScrollableElementUpdateArea =
            dbug_ShowNativeScrollableElementDrawToThisPage =
            dbug_ShowElementOutline =
            dbug_ShowObjectIden =
            dbug_ForceShowObjectIden =
            dbug_RecordHitChain =
            dbug_RecordDrawingChain = false;
        }
        public void dbug_EnableAllDebugInfo()
        {
            dbug_ShowRootUpdateArea =
            dbug_ShowNativeScrollableElementUpdateArea =
            dbug_ShowNativeScrollableElementDrawToThisPage =
            dbug_ShowElementOutline =
            dbug_ShowObjectIden =
            dbug_ForceShowObjectIden =
            dbug_RecordHitChain =
            dbug_RecordDrawingChain = true;
        }
        public void dbug_Init(System.IO.StreamWriter hitTestTrackerDebugStreamWriter,
            System.IO.Stream layoutTraceFileStream,
            System.IO.StreamWriter visualInvaldateStreamWriter
            )
        {
            dbugHitTracker = new dbugHitTestTracker(hitTestTrackerDebugStreamWriter);
            dbugEvalScrollBarTracer = new dbugVisualEvalScrollBarTrace(this);
            if (dbugEnableLayoutProfiler)
            {
                //string filename = dbugCoreConst.dbugRootFolder + "\\layout_trace\\p_" + Guid.NewGuid().ToString() + ".txt";
                //System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                dbugLayoutProfilerWriter = new System.IO.StreamWriter(layoutTraceFileStream);
                dbugLayoutProfilerWriter.AutoFlush = true;
            }
            if (dbugEnableGraphicInvalidateTrace)
            {
                dbugGraphicInvalidateTracer = new dbugVisualInvalidationTracer(this);
                dbugGraphicInvalidateTracer.Start(visualInvaldateStreamWriter);
                dbugGraphicInvalidateTracer.WriteInfo("root_debug_init()");
            }
        }
        public void dbug_WriteTick(long tick)
        {
            if (dbugEnableLayoutProfiler)
            {
                dbugLayoutProfilerWriter.WriteLine(DateTime.Now.ToString() + "  " + dbugGetFormatTickCount(tick));
            }
        }

        string dbugGetFormatTickCount(long tickCount)
        {
            string tickInStr = ((double)tickCount / (double)10000000).ToString();
            int pos = tickInStr.IndexOf('.');
            if (pos > 0)
            {
                if (tickInStr.Length > pos + 6)
                {
                    return tickInStr.Substring(0, pos + 6);
                }
                else
                {
                    return tickInStr;
                }
            }
            else
            {
                return tickInStr;
            }
        }


        public void dbug_AddDrawElement(RenderElement visualElement, Canvas currentCanvas)
        {
            dbug_AddDrawElement(visualElement, currentCanvas, null);
        }
        public void dbug_AddDrawElement(RenderElement visualElement, Canvas currentCanvas, string additionalMsg)
        {
            StringBuilder stBuilder = new StringBuilder();
            stBuilder.Append(new string('.', dbug_drawLevel));
            stBuilder.Append("[" + dbug_drawLevel + "] c_" + currentCanvas.debug_canvas_id + " ");
            stBuilder.Append(visualElement.dbug_FullElementDescription());
            Rectangle r = visualElement.dbugGetGlobalRect();
            stBuilder.Append(" global(" + r.X + "," + r.Y + "," + r.Width + "," + r.Height + ")");
            Rectangle currentClipRect = currentCanvas.CurrentClipRect;
            stBuilder.Append(" clip(" + currentClipRect.X +
                "," + currentClipRect.Y + "," + currentClipRect.Width + "," + currentClipRect.Height + ") ");
            if (visualElement.dbugParentVisualElement != null)
            {
                stBuilder.Append(" of " + visualElement.dbugParentVisualElement.dbug_FullElementDescription());
            }

            dbug_rootDrawingMsg.AddLast(new dbugLayoutMsg(visualElement, stBuilder.ToString()));
            if (additionalMsg != null)
            {
                stBuilder.Length = 0; stBuilder.Append(new string('.', dbug_drawLevel));
                stBuilder.Append("[" + dbug_drawLevel + "] c:" + currentCanvas.debug_canvas_id + " ");
                stBuilder.Append(visualElement.dbug_FullElementDescription());
                dbug_rootDrawingMsg.AddLast(new dbugLayoutMsg(visualElement, stBuilder.ToString()));
            }
        }

        public void dbug_AddDrawLayer(RenderElementLayer layer)
        {
            dbug_rootDrawingMsg.AddLast(new dbugLayoutMsg(layer,
                new string('.', dbug_drawLevel) + "[F" + dbug_drawLevel + "] " + layer.ToString()));
        }
        public void dbug_AddMessage(dbugLayoutMsg msg)
        {
            dbug_rootDrawingMsg.AddLast(msg);
        }
        public void dbug_DumpRootDrawingMsg(List<dbugLayoutMsg> outputlist)
        {
            outputlist.Add(new dbugLayoutMsg(null as RenderElement, "Asc n= " + dbug_rootDrawingMsg.Count));
            LinkedListNode<dbugLayoutMsg> u_node = dbug_rootDrawingMsg.First;
            while (u_node != null)
            {
                outputlist.Add(u_node.Value);
                u_node = u_node.Next;
            }
        }


        public void dbug_DumpCurrentHitChain(List<dbugLayoutMsg> outputlist)
        {
            foreach (dbugLayoutMsg s in dbug_rootHitChainMsg)
            {
                outputlist.Add(s);
            }
        }

        System.IO.StreamWriter invalidateTracerStreamWriter;
        public void dbug_BeginVisualInvalidateTrace(string strmsg)
        {
            if (dbugEnableGraphicInvalidateTrace)
            {
                if (dbugGraphicInvalidateTracer != null)
                {
                    dbugInvalidateTracerStack.Push(dbugGraphicInvalidateTracer);
                    dbugGraphicInvalidateTracer.Start(invalidateTracerStreamWriter);
                    dbugGraphicInvalidateTracer.WriteInfo(strmsg);
                }
                else
                {
                    dbugGraphicInvalidateTracer = new dbugVisualInvalidationTracer(this);
                    dbugGraphicInvalidateTracer.Start(invalidateTracerStreamWriter);
                    dbugGraphicInvalidateTracer.WriteInfo(strmsg);
                }
            }
        }
        public void dbug_EndVisualInvalidateTrace()
        {
            if (dbugGraphicInvalidateTracer != null)
            {
                dbugGraphicInvalidateTracer.Stop();
                if (dbugInvalidateTracerStack.Count > 0)
                {
                    dbugGraphicInvalidateTracer = dbugInvalidateTracerStack.Pop();
                }
                else
                {
                    dbugGraphicInvalidateTracer = null;
                }
            }
        }


        public void dbug_PushInvalidateMsg(dbugVisualRootMsg msg, RenderElement ve)
        {
            if (this.dbugEnableGraphicInvalidateTrace && dbugGraphicInvalidateTracer != null)
            {
                dbugGraphicInvalidateTracer.WriteInfo(msg.msg, ve);
            }
        }
        public dbugVisualInvalidationTracer dbugGetVisualInvalidateTracer()
        {
            return dbugGraphicInvalidateTracer;
        }

        dbugVisualLayoutTracer dbugLastestDebugVisualLay;
        Stack<dbugVisualLayoutTracer> debugLayoutTracerStack = new Stack<dbugVisualLayoutTracer>();
        public dbugVisualLayoutTracer dbug_GetLastestVisualLayoutTracer()
        {
            return dbugLastestDebugVisualLay;
        }
        public bool dbug_IsRecordLayoutTraceEnable
        {
            get
            {
                return dbugLastestDebugVisualLay != null;
            }
        }
        System.IO.StreamWriter layoutTraceStreamWriter;
        public void dbug_BeginLayoutTraceSession(string beginMsg)
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.WriteInfo("---------switch to new sesssion---------");
                debugLayoutTracerStack.Push(dbugLastestDebugVisualLay);
            }
            dbugLastestDebugVisualLay = new dbugVisualLayoutTracer(this);
            dbugLastestDebugVisualLay.Start(layoutTraceStreamWriter);
            dbugLastestDebugVisualLay.WriteInfo("---------Layout Trace---------");
            dbugLastestDebugVisualLay.WriteInfo(beginMsg);
            dbugLastestDebugVisualLay.WriteInfo("------------------------------");
        }
        public void dbug_FinishLayoutTraceSession()
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.Stop();
            }
            if (debugLayoutTracerStack.Count > 0)
            {
                dbugLastestDebugVisualLay = debugLayoutTracerStack.Pop();
            }
            else
            {
                dbugLastestDebugVisualLay = null;
            }
        }

        public void dbug_PushLayoutTraceMessage(dbugVisualRootMsg msg)
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.WriteInfo(msg.msg);
            }
        }

        public void dbug_PushLayoutTraceMessage(dbugVisualRootMsg msg, int number)
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.WriteInfo(msg.msg + number);
            }
        }
        public void dbug_PushLayoutTraceMessage(string str)
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.WriteInfo(str);
            }
        }
        public int dbugNotNeedArrCount = 0;
        public int dbugNotNeedArrCountEpisode = 0;
        public void dbug_PushLayoutTraceMessage(dbugVisualRootMsg msg, RenderElementLayer layer)
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.WriteInfo(msg.msg, layer);
            }
        }
        public void dbug_PushLayoutTraceMessage(dbugVisualRootMsg msg, RenderElement ve)
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.WriteInfo(msg.msg, ve);
            }
        }

        public void dbug_LayoutTraceBeginContext(dbugVisualRootMsg msg, RenderElement ve)
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.BeginNewContext(); dbugLastestDebugVisualLay.WriteInfo(msg.msg, ve);
            }
        }
        public void dbug_LayoutTraceEndContext(dbugVisualRootMsg msg, RenderElement ve)
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.WriteInfo(msg.msg, ve);
                dbugLastestDebugVisualLay.EndCurrentContext();
            }
        }


        public void dbug_FlushLayoutTraceMessage()
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.Flush();
            }
        }


        public RenderElement dbug_GetElementById(object id)
        {
            return null;
        }
        public static RootGraphic dbugCurrentGlobalVRoot;
    }
#endif
}