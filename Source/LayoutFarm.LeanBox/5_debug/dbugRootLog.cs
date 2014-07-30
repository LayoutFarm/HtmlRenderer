//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.Diagnostics;



using LayoutFarm.Presentation;



namespace LayoutFarm.Presentation
{
#if DEBUG
    public static partial class dbugRootLog
    {   
        public static bool dbug_ShowRootUpdateArea = false;
        public static bool dbug_ShowNativeScrollableElementUpdateArea = false;
        public static bool dbug_ShowNativeScrollableElementDrawToThisPage = false;
        public static bool dbug_ShowElementOutline = false;
        public static bool dbug_ShowObjectIden = false;
        public static bool dbug_ForceShowObjectIden = false;
        public static bool dbug_RecordHitChain = false;
        public static bool dbug_RecordDrawingChain = false;
        public static bool dbugEnableGraphicInvalidateTrace = false;
        public static bool dbug_StartTestMode = false;
        public static bool dbugEnableLayoutProfiler = false;
        public static System.IO.StreamWriter dbugLayoutProfilerWriter;
        public static bool dbugEventIsDragging;


        public static int dbug_RootUpdateCounter = 0;
        public static int dbug_drawLevel = 0;
        public static LinkedList<dbugLayoutMsg> dbug_rootHitChainMsg = new LinkedList<dbugLayoutMsg>();
        public static LinkedList<dbugLayoutMsg> dbug_rootDrawingMsg = new LinkedList<dbugLayoutMsg>();
        public static dbugHitTestTracker dbugHitTracker;
        public static Stack<dbugVisualInvalidationTracer> dbugInvalidateTracerStack = new Stack<dbugVisualInvalidationTracer>();
        public static dbugVisualInvalidationTracer dbugGraphicInvalidateTracer;
        public static dbugVisualEvalScrollBarTrace dbugEvalScrollBarTracer;
        public static void dbug_DisableAllDebugInfo()
        {
            dbug_ShowRootUpdateArea = false;
            dbug_ShowNativeScrollableElementUpdateArea = false;
            dbug_ShowNativeScrollableElementDrawToThisPage = false;
            dbug_ShowElementOutline = false;
            dbug_ShowObjectIden = false;
            dbug_ForceShowObjectIden = false; dbug_RecordHitChain = false; dbug_RecordDrawingChain = false;
        }
        public static void dbug_EnableAllDebugInfo()
        {
            dbug_ShowRootUpdateArea = true;
            dbug_ShowNativeScrollableElementUpdateArea = true;
            dbug_ShowNativeScrollableElementDrawToThisPage = true;
            dbug_ShowElementOutline = true;
            dbug_ShowObjectIden = true;
            dbug_ForceShowObjectIden = true; dbug_RecordHitChain = true; dbug_RecordDrawingChain = true;
        }
        public static void dbug_Init()
        {

            dbugHitTracker = new dbugHitTestTracker();
            dbugEvalScrollBarTracer = new dbugVisualEvalScrollBarTrace();

            if (dbugEnableLayoutProfiler)
            {
                string filename = dbugCoreConst.dbugRootFolder + "\\layout_trace\\p_" + Guid.NewGuid().ToString() + ".txt";
                System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Create);

                dbugLayoutProfilerWriter = new System.IO.StreamWriter(fs);
                dbugLayoutProfilerWriter.AutoFlush = true;
            }
            if (dbugEnableGraphicInvalidateTrace)
            {
                dbugGraphicInvalidateTracer = new dbugVisualInvalidationTracer();
                dbugGraphicInvalidateTracer.Start();
                dbugGraphicInvalidateTracer.WriteInfo("root_debug_init()");
            }

        }
        public static void dbug_WriteTick(long tick)
        {
            if (dbugEnableLayoutProfiler)
            {
                dbugLayoutProfilerWriter.WriteLine(DateTime.Now.ToString() + "  " + dbugGetFormatTickCount(tick));
            }
        }

        static string dbugGetFormatTickCount(long tickCount)
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


        public static void dbug_AddDrawElement(ArtVisualElement visualElement, ArtCanvas currentCanvas)
        {
            dbug_AddDrawElement(visualElement, currentCanvas, null);
        }
        public static void dbug_AddDrawElement(ArtVisualElement visualElement, ArtCanvas currentCanvas, string additionalMsg)
        {

            StringBuilder stBuilder = new StringBuilder();
            stBuilder.Append(new string('.', dbug_drawLevel));

            stBuilder.Append("[" + dbug_drawLevel + "] c_" + currentCanvas.debug_canvas_id + " ");
            stBuilder.Append(visualElement.dbug_FullElementDescription());



            Rectangle r = visualElement.GetGlobalRect();
            stBuilder.Append(" global(" + r.X + "," + r.Y + "," + r.Width + "," + r.Height + ")");

            Rectangle currentClipRect = currentCanvas.CurrentClipRect;

            stBuilder.Append(" clip(" + currentClipRect.X +
                "," + currentClipRect.Y + "," + currentClipRect.Width + "," + currentClipRect.Height + ") ");


            if (visualElement.ParentVisualElement != null)
            {
                stBuilder.Append(" of " + visualElement.ParentVisualElement.dbug_FullElementDescription());
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

        public static void dbug_AddDrawLayer(VisualLayer layer)
        {

            dbug_rootDrawingMsg.AddLast(new dbugLayoutMsg(layer,
                new string('.', dbug_drawLevel) + "[F" + dbug_drawLevel + "] " + layer.ToString()));
        }
        public static void dbug_AddMessage(dbugLayoutMsg msg)
        {
            dbug_rootDrawingMsg.AddLast(msg);
        }
        public static void dbug_DumpRootDrawingMsg(List<dbugLayoutMsg> outputlist)
        {
            outputlist.Add(new dbugLayoutMsg(null as ArtVisualElement, "Asc n= " + dbug_rootDrawingMsg.Count));
            LinkedListNode<dbugLayoutMsg> u_node = dbug_rootDrawingMsg.First;
            while (u_node != null)
            {
                outputlist.Add(u_node.Value);
                u_node = u_node.Next;
            }
        }


        public static void dbug_DumpCurrentHitChain(List<dbugLayoutMsg> outputlist)
        {
            foreach (dbugLayoutMsg s in dbug_rootHitChainMsg)
            {
                outputlist.Add(s);
            }
        }

        public static void dbug_BeginVisualInvalidateTrace(string strmsg)
        {
            if (dbugEnableGraphicInvalidateTrace)
            {
                if (dbugGraphicInvalidateTracer != null)
                {
                    dbugInvalidateTracerStack.Push(dbugGraphicInvalidateTracer);
                    dbugGraphicInvalidateTracer.Start();
                    dbugGraphicInvalidateTracer.WriteInfo(strmsg);
                }
                else
                {
                    dbugGraphicInvalidateTracer = new dbugVisualInvalidationTracer();
                    dbugGraphicInvalidateTracer.Start();
                    dbugGraphicInvalidateTracer.WriteInfo(strmsg);
                }
            }

        }
        public static void dbug_EndVisualInvalidateTrace()
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


        public static void dbug_PushInvalidateMsg(dbugVisualRootMsg msg, ArtVisualElement ve)
        {
            if (dbugRootLog.dbugEnableGraphicInvalidateTrace && dbugGraphicInvalidateTracer != null)
            {
                dbugGraphicInvalidateTracer.WriteInfo(msg.msg, ve);
            }
        }
        public static dbugVisualInvalidationTracer GetVisualInvalidateTracer()
        {
            return dbugGraphicInvalidateTracer;
        }

        static dbugVisualLayoutTracer dbugLastestDebugVisualLay;
        static Stack<dbugVisualLayoutTracer> debugLayoutTracerStack = new Stack<dbugVisualLayoutTracer>();

        public static dbugVisualLayoutTracer dbug_GetLastestVisualLayoutTracer()
        {
            return dbugLastestDebugVisualLay;
        }
        public static bool dbug_IsRecordLayoutTraceEnable
        {
            get
            {
                return dbugLastestDebugVisualLay != null;
            }
        }
        public static void dbug_BeginLayoutTraceSession(string beginMsg)
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.WriteInfo("---------switch to new sesssion---------");
                debugLayoutTracerStack.Push(dbugLastestDebugVisualLay);
            }
            dbugLastestDebugVisualLay = new dbugVisualLayoutTracer();
            dbugLastestDebugVisualLay.Start();
            dbugLastestDebugVisualLay.WriteInfo("---------Layout Trace---------");
            dbugLastestDebugVisualLay.WriteInfo(beginMsg);
            dbugLastestDebugVisualLay.WriteInfo("------------------------------");
        }
        public static void dbug_FinishLayoutTraceSession()
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

        public static void dbug_PushLayoutTraceMessage(dbugVisualRootMsg msg)
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.WriteInfo(msg.msg);
            }
        }

        public static void dbug_PushLayoutTraceMessage(dbugVisualRootMsg msg, int number)
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.WriteInfo(msg.msg + number);
            }
        }
        public static void dbug_PushLayoutTraceMessage(string str)
        {
            if (dbugLastestDebugVisualLay != null)
            {

                dbugLastestDebugVisualLay.WriteInfo(str);
            }
        }
        public static int dbugNotNeedArrCount = 0;
        public static int dbugNotNeedArrCountEpisode = 0;


        public static void dbug_PushLayoutTraceMessage(dbugVisualRootMsg msg, VisualLayer layer)
        {
            if (dbugLastestDebugVisualLay != null)
            {

                dbugLastestDebugVisualLay.WriteInfo(msg.msg, layer);
            }
        }
        public static void dbug_PushLayoutTraceMessage(dbugVisualRootMsg msg, ArtVisualElement ve)
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.WriteInfo(msg.msg, ve);
            }
        }

        public static void dbug_LayoutTraceBeginContext(dbugVisualRootMsg msg, ArtVisualElement ve)
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.BeginNewContext(); dbugLastestDebugVisualLay.WriteInfo(msg.msg, ve);
            }
        }
        public static void dbug_LayoutTraceEndContext(dbugVisualRootMsg msg, ArtVisualElement ve)
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.WriteInfo(msg.msg, ve);
                dbugLastestDebugVisualLay.EndCurrentContext();
            }
        }
        public static void dbug_FlushLayoutTraceMessage()
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.Flush();
            }
        }
        public static ArtVisualElement dbug_GetElementById(object id)
        {
            return null;
        }
    }
#endif
}