using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



namespace LayoutFarm.Presentation
{

#if DEBUG
                    public class dbugVisualInvalidationTracer
    {
        FileStream fs;
        StreamWriter strmWriter;
        VisualRoot visualroot;
        string outputFileName = null;

        int msgCounter = 0;

        Stack<ArtVisualElement> elementStack = new Stack<ArtVisualElement>();

        int indentCount = 0;
        int dbug_Id = 0; 
        static int dbug_totalId = 0;

        public dbugVisualInvalidationTracer(VisualRoot visualroot)
        {
            this.visualroot = visualroot;
            dbug_Id = dbug_totalId;
            ++dbug_totalId;
            outputFileName = dbugCoreConst.dbugRootFolder +"\\invalidate\\" + dbug_Id + "_" + Guid.NewGuid().ToString() + ".txt";

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
            strmWriter.Write("\r\n");            strmWriter.Flush();
        }
        public void WriteInfo(string info,ArtVisualElement ve)
        {
            ++msgCounter;
            ShouldBreak();
            strmWriter.Write(new string('\t', indentCount));
            strmWriter.Write(info);
            strmWriter.Write(ve.dbug_FullElementDescription());
            strmWriter.Write("\r\n");            strmWriter.Flush();
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

