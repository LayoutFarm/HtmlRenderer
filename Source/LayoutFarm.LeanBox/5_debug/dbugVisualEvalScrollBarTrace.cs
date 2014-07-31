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
        VisualRoot visualroot;
        string outputFileName = null;

        int msgCounter = 0;

        Stack<ArtVisualElement> elementStack = new Stack<ArtVisualElement>();

        int indentCount = 0;
        int myTraceCount = 0;

        static int tracerCount = 0;
        public dbugVisualEvalScrollBarTrace(VisualRoot visualroot)
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
        public void PushVisualElement(ArtVisualElement v)
        {
            elementStack.Push(v);
            BeginNewContext();

        }

        public ArtVisualElement PopElement()
        {
            ArtVisualElement v = elementStack.Pop();
            EndCurrentContext();
            return v;
        }
        public ArtVisualElement PeekElement()
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

