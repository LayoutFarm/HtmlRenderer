//2014,2015 ,BSD, WinterDev 
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
    }
     
}