//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



namespace LayoutFarm.Presentation
{

#if DEBUG
    public class dbugVisualEvalScrollBarTrace
    {
        FileStream fs;
        StreamWriter strmWriter;
        dbugRootElement visualroot;
        string outputFileName = null;

        int msgCounter = 0;

        Stack<RenderElement> elementStack = new Stack<RenderElement>();

        int indentCount = 0;
        int myTraceCount = 0;

        static int tracerCount = 0;
        public dbugVisualEvalScrollBarTrace(dbugRootElement visualroot)
        {
            this.visualroot = visualroot;
            myTraceCount = tracerCount;
            ++tracerCount;
            outputFileName = dbugCoreConst.dbugRootFolder + "\\invalidate\\" + myTraceCount + "_" + Guid.NewGuid().ToString() + ".txt";

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

        public void Start()
        {

            fs = new FileStream(outputFileName, FileMode.Create);
            strmWriter = new StreamWriter(fs);

        }
        public void Stop()
        {
            strmWriter.Close();
            strmWriter.Dispose();

            fs.Close();
            fs.Dispose();
            fs = null;

        }

        public void WriteInfo(string info)
        {
            ++msgCounter;
            ShouldBreak();
            strmWriter.Write(new string('\t', indentCount));
            strmWriter.Write(info);
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

