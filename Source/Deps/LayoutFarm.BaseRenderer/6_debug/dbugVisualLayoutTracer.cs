//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using System.IO;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm
{
#if DEBUG
    public class dbugVisualLayoutTracer
    {
        StreamWriter strmWriter;
        RootGraphic visualroot;
        string outputFileName = null;
        int msgLineNum = 0;
        Stack<object> elementStack = new Stack<object>();
        int indentCount = 0;
        int myTraceCount = 0;
        static int tracerCount = 0;
        public dbugVisualLayoutTracer(RootGraphic visualroot)
        {
            this.visualroot = visualroot;
            myTraceCount = tracerCount;
            ++tracerCount;
            outputFileName = dbugCoreConst.dbugRootFolder + "\\layout_trace\\" + myTraceCount + "_" + Guid.NewGuid().ToString() + ".txt";
        }
        public override string ToString()
        {
            return msgLineNum.ToString();
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
        public void PopVisualElement()
        {
            elementStack.Pop();
            EndCurrentContext();
        }
        public void PushLayerElement(RenderElementLayer layer)
        {
            elementStack.Push(layer);
            BeginNewContext();
        }
        public void PopLayerElement()
        {
            elementStack.Pop();
            EndCurrentContext();
        }

        public object PeekElement()
        {
            return elementStack.Peek();
        }

        public void Start(StreamWriter strmWriter)
        {
            this.strmWriter = strmWriter;
            strmWriter.AutoFlush = true;
        }
        public void Stop()
        {
            strmWriter.Flush();
        }
        public void WriteInfo(RenderElement v, string info, string indentPrefix, string indentPostfix)
        {
            ++msgLineNum;
            ShouldBreak();
            strmWriter.Write(new string('\t', indentCount));
            strmWriter.Write(indentPrefix + indentCount + indentPostfix + info + " ");
            strmWriter.Write(v.dbug_FullElementDescription());
            strmWriter.Write("\r\n"); strmWriter.Flush();
        }
        public void WriteInfo(string info)
        {
            ++msgLineNum;
            ShouldBreak();
            strmWriter.Write(new string('\t', indentCount));
            strmWriter.Write(info);
            strmWriter.Write("\r\n"); strmWriter.Flush();
        }
        public void WriteInfo(string info, RenderElement v)
        {
            ++msgLineNum;
            ShouldBreak();
            strmWriter.Write(new string('\t', indentCount));
            strmWriter.Write(info);
            strmWriter.Write(v.dbug_FullElementDescription());
            strmWriter.Write("\r\n"); strmWriter.Flush();
        }
        public void WriteInfo(string info, RenderElementLayer layer)
        {
            ++msgLineNum;
            ShouldBreak();
            strmWriter.Write(new string('\t', indentCount));
            strmWriter.Write(info);
            strmWriter.Write(layer.ToString());
            strmWriter.Write("\r\n"); strmWriter.Flush();
        }
        public void Flush()
        {
            strmWriter.Flush();
        }
        void ShouldBreak()
        {
        }
    }
#endif
}
