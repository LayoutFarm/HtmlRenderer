using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.Diagnostics;



using LayoutFarm.Presentation;



namespace LayoutFarm.Presentation
{

    partial class VisualRoot
    {
        
#if DEBUG

                                public bool dbug_ShowRootUpdateArea = false;
        public bool dbug_ShowNativeScrollableElementUpdateArea = false;
        public bool dbug_ShowNativeScrollableElementDrawToThisPage = false;
        public bool dbug_ShowElementOutline = false;
        public bool dbug_ShowObjectIden = false;
        public bool dbug_ForceShowObjectIden = false;         public bool dbug_RecordHitChain = false;        public bool dbug_RecordDrawingChain = false;
                                        public bool dbugEnableGraphicInvalidateTrace = false;
                public bool dbug_StartTestMode = false;
                public bool dbugEnableLayoutProfiler = false;
        public System.IO.StreamWriter dbugLayoutProfilerWriter;
                                        public bool dbugEventIsDragging;


                                        public int dbug_RootUpdateCounter = 0;        public int dbug_drawLevel = 0;
                                                public LinkedList<dbugLayoutMsg> dbug_rootHitChainMsg = new LinkedList<dbugLayoutMsg>();
                               public LinkedList<dbugLayoutMsg> dbug_rootDrawingMsg = new LinkedList<dbugLayoutMsg>();
               public dbugHitTestTracker dbugHitTracker;
       public Stack<dbugVisualInvalidationTracer> dbugInvalidateTracerStack = new Stack<dbugVisualInvalidationTracer>();
       public dbugVisualInvalidationTracer dbugGraphicInvalidateTracer;
       public dbugVisualEvalScrollBarTrace dbugEvalScrollBarTracer;
                                public void dbug_DisableAllDebugInfo()
        {
            dbug_ShowRootUpdateArea = false;
            dbug_ShowNativeScrollableElementUpdateArea = false;
            dbug_ShowNativeScrollableElementDrawToThisPage = false;
            dbug_ShowElementOutline = false;
            dbug_ShowObjectIden = false;
            dbug_ForceShowObjectIden = false;             dbug_RecordHitChain = false;            dbug_RecordDrawingChain = false;
        }
                                                                                                                        public void dbug_EnableAllDebugInfo()
        {
            dbug_ShowRootUpdateArea = true;
            dbug_ShowNativeScrollableElementUpdateArea = true;
            dbug_ShowNativeScrollableElementDrawToThisPage = true;
            dbug_ShowElementOutline = true;
            dbug_ShowObjectIden = true;
            dbug_ForceShowObjectIden = true;             dbug_RecordHitChain = true;            dbug_RecordDrawingChain = true;
        }
       public  void dbug_Init()
        {

            dbugHitTracker = new dbugHitTestTracker();
            dbugEvalScrollBarTracer = new dbugVisualEvalScrollBarTrace(this);

            if (dbugEnableLayoutProfiler)
            {
                string filename = dbugCoreConst.dbugRootFolder + "\\layout_trace\\p_" + Guid.NewGuid().ToString() + ".txt";
                System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Create);

                dbugLayoutProfilerWriter = new System.IO.StreamWriter(fs);
                dbugLayoutProfilerWriter.AutoFlush = true;
            }
            if (dbugEnableGraphicInvalidateTrace)
            {
                dbugGraphicInvalidateTracer = new dbugVisualInvalidationTracer(this);
                dbugGraphicInvalidateTracer.Start();
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


                                                public void dbug_AddDrawElement(ArtVisualElement visualElement, ArtCanvas currentCanvas)
        {
                        dbug_AddDrawElement(visualElement, currentCanvas, null);
        }
                                                        public void dbug_AddDrawElement(ArtVisualElement visualElement, ArtCanvas currentCanvas, string additionalMsg)
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
                stBuilder.Length = 0;                stBuilder.Append(new string('.', dbug_drawLevel));
                stBuilder.Append("[" + dbug_drawLevel + "] c:" + currentCanvas.debug_canvas_id + " ");
                stBuilder.Append(visualElement.dbug_FullElementDescription());
                dbug_rootDrawingMsg.AddLast(new dbugLayoutMsg(visualElement, stBuilder.ToString()));
                                            }

        }

                                        public void dbug_AddDrawLayer(VisualLayer layer)
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
            outputlist.Add(new dbugLayoutMsg(null as ArtVisualElement, "Asc n= " + dbug_rootDrawingMsg.Count));
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

        public void dbug_BeginVisualInvalidateTrace(string strmsg)
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
                    dbugGraphicInvalidateTracer = new dbugVisualInvalidationTracer(this);
                    dbugGraphicInvalidateTracer.Start();
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


        public void dbug_PushInvalidateMsg(dbugVisualRootMsg msg, ArtVisualElement ve)
        {
            if (this.dbugEnableGraphicInvalidateTrace && dbugGraphicInvalidateTracer != null)
            {
                dbugGraphicInvalidateTracer.WriteInfo(msg.msg, ve);
            }
        }
        public dbugVisualInvalidationTracer GetVisualInvalidateTracer()
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
                                public void dbug_BeginLayoutTraceSession(string beginMsg)
        {
            if (dbugLastestDebugVisualLay != null)
            {
                                                                dbugLastestDebugVisualLay.WriteInfo("---------switch to new sesssion---------");
                debugLayoutTracerStack.Push(dbugLastestDebugVisualLay);
            }
                        dbugLastestDebugVisualLay = new dbugVisualLayoutTracer(this);
            dbugLastestDebugVisualLay.Start();
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


        public void dbug_PushLayoutTraceMessage(dbugVisualRootMsg msg, VisualLayer layer)
        {
            if (dbugLastestDebugVisualLay != null)
            {

                dbugLastestDebugVisualLay.WriteInfo(msg.msg, layer);
            }
        }
        public void dbug_PushLayoutTraceMessage(dbugVisualRootMsg msg, ArtVisualElement ve)
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.WriteInfo(msg.msg, ve);            }
        }
                                                
                        public void dbug_LayoutTraceBeginContext(dbugVisualRootMsg msg, ArtVisualElement ve)
        {
            if (dbugLastestDebugVisualLay != null)
            {
                dbugLastestDebugVisualLay.BeginNewContext();                dbugLastestDebugVisualLay.WriteInfo(msg.msg, ve);
            }
        }
        public void dbug_LayoutTraceEndContext(dbugVisualRootMsg msg, ArtVisualElement ve)
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
                
                                                                        
                public ArtVisualElement dbug_GetElementById(object id)
        {
                        return null;
        }
        public static VisualRoot dbugCurrentGlobalVRoot;
#endif
    }
}