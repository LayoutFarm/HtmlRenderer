//BSD, 2014-2017, WinterDev 

using System;
using LayoutFarm.WebLexer;
namespace LayoutFarm.WebDom.Parser
{
    public class TextSource
    {
        TextSnapshot actualSnapshot;
        public TextSource(char[] textBuffer)
        {
            this.actualSnapshot = new TextSnapshot(textBuffer);
        }
        internal TextSnapshot ActualSnapshot
        {
            get { return this.actualSnapshot; }
        }

        public static string GetInternalText(TextSource textsource)
        {
            return new string(TextSnapshot.UnsafeGetInternalBuffer(textsource.actualSnapshot));
        }
    }
}