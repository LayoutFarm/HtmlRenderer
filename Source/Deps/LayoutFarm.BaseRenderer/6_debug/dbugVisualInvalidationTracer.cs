//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using System.IO;
namespace LayoutFarm
{
#if DEBUG
    public class dbugVisualInvalidationTracer
    {
        StreamWriter strmWriter;
        RootGraphic visualroot;
        string outputFileName = null;
        int msgCounter = 0;
        Stack<RenderElement> elementStack = new Stack<RenderElement>();
        int indentCount = 0;
        int dbug_Id = 0;
        static int dbug_totalId = 0;
        public dbugVisualInvalidationTracer(RootGraphic visualroot)
        {
            this.visualroot = visualroot;
            dbug_Id = dbug_totalId;
            ++dbug_totalId;
            outputFileName = dbugCoreConst.dbugRootFolder + "\\invalidate\\" + dbug_Id + "_" + Guid.NewGuid().ToString() + ".txt";
        }
        public void BeginNewContext()
        {
            ++indentCount;
        }
        public void EndCurrentContext()
        {
            --indentCount;
        }
        public void PushVisualElement(RenderElement v)
        {
            elementStack.Push(v);
            BeginNewContext();
        }

        public RenderElement PopElement()
        {
            RenderElement v = elementStack.Pop();
            EndCurrentContext();
            return v;
        }
        public RenderElement PeekElement()
        {
            return elementStack.Peek();
        }

        public void Start(StreamWriter writer)
        {
            this.strmWriter = writer;
        }
        public void Stop()
        {
            strmWriter.Flush();
        }
        public void WriteInfo(string info)
        {
            ++msgCounter;
            ShouldBreak();
            strmWriter.Write(new string('\t', indentCount));
            strmWriter.Write(info);
            strmWriter.Write("\r\n"); strmWriter.Flush();
        }
        public void WriteInfo(string info, RenderElement ve)
        {
            ++msgCounter;
            ShouldBreak();
            strmWriter.Write(new string('\t', indentCount));
            strmWriter.Write(info);
            strmWriter.Write(ve.dbug_FullElementDescription());
            strmWriter.Write("\r\n"); strmWriter.Flush();
        }
        public void Flush()
        {
            strmWriter.Flush();
        }
        void ShouldBreak()
        {
            if (msgCounter >= 40)
            {
            }
        }
    }
#endif
}

